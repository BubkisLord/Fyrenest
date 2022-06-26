global using System;
global using System.IO;
global using System.Collections;
global using Modding;
global using UnityEngine;
global using SFCore;
global using System.Collections.Generic;
global using System.Linq;
global using Satchel.BetterMenus;
global using RandomizerMod.Menu;
global using RandomizerMod.Settings;
global using RandomizerMod.Logging;
global using RandomizerMod.RC;
global using RandomizerCore;
global using RandomizerCore.Logic;
global using RandomizerCore.LogicItems;
using ItemChanger;
using ItemChanger.Containers;
using ItemChanger.Extensions;
using ItemChanger.Items;
using ItemChanger.Tags;
using ItemChanger.Placements;
using ItemChanger.Locations;
using ItemChanger.Modules;
using ItemChanger.UIDefs;
using ItemChanger.StartDefs;
using ItemChanger.FsmStateActions;
using ItemChanger.Components;
using System.Reflection;

namespace Fyrenest
{
    public class Fyrenest : Mod, IMod
    {

        public override string GetVersion() => "2.10.41.38";

        private static List<Charm> Charms = new()
        {
            Quickfall.Instance,
            Slowfall.Instance,
            SturdyNail.Instance,
            BetterCDash.Instance,
            GlassCannon.Instance,
            HKBlessing.Instance,
            MarkofStrength.Instance,
            PowerfulDash.Instance,
            HealthyShell.Instance,
            OpportunisticDefeat.Instance,
            SoulSpeed.Instance,
            SoulSpell.Instance,
            SoulHunger.Instance,
            RavenousSoul.Instance,
            SoulSwitch.Instance,
            GeoSwitch.Instance,
            WealthyAmulet.Instance,
            SoulSlow.Instance,
            SlowTime.Instance,
            SpeedTime.Instance,
            ZoteBorn.Instance,
            SlyDeal.Instance,
            ElderStone.Instance,
            GiantNail.Instance,
            MatosBlessing.Instance
        };
        public bool Enabled = true;
        public int NewCharms = Charms.Count; //STARTS AT 1
        public int OldCharms = 40; //STARTS AT 1

        internal static Fyrenest Instance;

        private Dictionary<string, Func<bool, bool>> BoolGetters = new();
        private Dictionary<string, Action<bool>> BoolSetters = new();
        private Dictionary<string, Func<int, int>> IntGetters = new();
        private Dictionary<(string, string), Action<PlayMakerFSM>> FSMEdits = new();
        private List<(int Period, Action Func)> Tickers = new();
        public static List<IAbility> Abilities;
        public static Dictionary<string, Dictionary<string, GameObject>> Preloads;

        public override List<(string, string)> GetPreloadNames() => new List<(string, string)> {
            ("GG_Mantis_Lords", "Shot Mantis Lord")
        };



        private void LoadAbilities()
        {
            Abilities = new List<IAbility>();

            // Find all abilities
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterface("IAbility") != null)
                {
                    // Type is an ability
                    Abilities.Add(Activator.CreateInstance(type) as IAbility);
                }
            }

            foreach (IAbility ability in Abilities)
            {
                Log($"Registered ability {ability.Name}!");
            }
        }
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Preloads = preloadedObjects;
            LoadAbilities();

            On.HeroController.Awake += delegate (On.HeroController.orig_Awake orig, HeroController self) {
                orig.Invoke(self);

                foreach (IAbility ability in Abilities)
                {
                    Log($"Loading ability {ability.Name}!");
                    ability.Load();
                }
            };
            Log("Initializing");
            Instance = this;
            foreach (var charm in Charms)
            {
                var num = CharmHelper.AddSprites(EmbeddedSprites.Get(charm.Sprite))[0];
                charm.Num = num;
                var settings = charm.Settings;
                IntGetters[$"charmCost_{num}"] = _ => settings(Settings).Cost;
                AddTextEdit($"CHARM_NAME_{num}", "UI", charm.Name);
                AddTextEdit($"CHARM_DESC_{num}", "UI", () => charm.Description);
                BoolGetters[$"equippedCharm_{num}"] = _ => settings(Settings).Equipped;
                BoolSetters[$"equippedCharm_{num}"] = value => settings(Settings).Equipped = value;
                BoolGetters[$"gotCharm_{num}"] = _ => settings(Settings).Got;
                BoolSetters[$"gotCharm_{num}"] = value => settings(Settings).Got = value;
                BoolGetters[$"newCharm_{num}"] = _ => settings(Settings).New;
                BoolSetters[$"newCharm_{num}"] = value => settings(Settings).New = value;
                charm.Hook();
                foreach (var edit in charm.FsmEdits)
                {
                    AddFsmEdit(edit.obj, edit.fsm, edit.edit);
                }
                Tickers.AddRange(charm.Tickers);

                var item = new ItemChanger.Items.CharmItem()
                {
                    charmNum = charm.Num,
                    name = charm.Name.Replace(" ", "_"),
                    UIDef = new MsgUIDef()
                    {
                        name = new LanguageString("UI", $"CHARM_NAME_{charm.Num}"),
                        shopDesc = new LanguageString("UI", $"CHARM_DESC_{charm.Num}"),
                    }
                };
                // Tag the item for ConnectionMetadataInjector, so that MapModS and
                // other mods recognize the items we're adding as charms.
                var mapmodTag = item.AddTag<InteropTag>();
                mapmodTag.Message = "RandoSupplementalMetadata";
                mapmodTag.Properties["ModSource"] = GetName();
                mapmodTag.Properties["PoolGroup"] = "Charms";
                Finder.DefineCustomItem(item);
            }
            for (var i = 1; i <= 40; i++)
            {
                var num = i; // needed for closure to capture a different copy of the variable each time
                BoolGetters[$"equippedCharm_{num}"] = value => value;
                IntGetters[$"charmCost_{num}"] = value => value;
            }

            ModHooks.GetPlayerBoolHook += ReadCharmBools;
            ModHooks.SetPlayerBoolHook += WriteCharmBools;
            ModHooks.GetPlayerIntHook += ReadCharmCosts;
            ModHooks.LanguageGetHook += GetCharmStrings;
            // This will run after Rando has already set up its item placements.
            On.PlayMakerFSM.OnEnable += EditFSMs;
            On.PlayerData.CountCharms += CountOurCharms;
            ModHooks.NewGameHook += TransitionSet;
            ModHooks.HeroUpdateHook += OnUpdate;
            ModHooks.LanguageGetHook += LanguageGet;
            On.UIManager.UIGoToPauseMenu += OnPause;
            On.UIManager.UIClosePauseMenu += OnUnPause;

            StartTicking();

            if (ModHooks.GetMod("DebugMod") != null)
            {
                DebugModHook.GiveAllCharms(() =>
                {
                    GrantAllOurCharms();
                    PlayerData.instance.CountCharms();
                });
            }
        }

        private void TransitionSet()
        {
            return;
        }
        private void OnPause(On.UIManager.orig_UIGoToPauseMenu orig, UIManager self)
        {
            HeroController.instance.RelinquishControl();
            Time.timeScale = 0.0f;
            orig(self);
        }
        private void OnUnPause(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
        {
            HeroController.instance.RegainControl();
            Time.timeScale = 1.0f;
            HeroController.instance.CharmUpdate();
            orig(self);
        }
        private int ReadCharmCosts(string intName, int value)
        {
            Func<int, int> cost;
            bool flag = this.IntGetters.TryGetValue(intName, out cost);
            int result;
            if (flag)
            {
                result = cost(value);
            }
            else
            {
                result = value;
            }
            return result;
        }
        private void OnUpdate()
        {
            if (Enabled)
            {
                //give charms when certain things are done.
                if (PlayerData.instance.colosseumBronzeCompleted) Quickfall.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.colosseumSilverCompleted) Slowfall.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.hasShadowDash) PowerfulDash.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.hasNailArt) SturdyNail.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.statueStateMantisLordsExtra.isUnlocked) MarkofStrength.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.hasDreamGate) SoulHunger.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.hasDreamNail) SoulSlow.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.hasSuperDash && PlayerData.instance.gaveSlykey) BetterCDash.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.killedHollowKnight) HKBlessing.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.hasKingsBrand) HealthyShell.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.killedHollowKnightPrime) GlassCannon.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.bankerAccountPurchased) WealthyAmulet.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.colosseumGoldCompleted) RavenousSoul.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.canOvercharm) OpportunisticDefeat.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.collectorDefeated) SoulSpell.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.grubsCollected > 10) SlowTime.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.statueStateCollector.completedTier2) SpeedTime.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.mageLordDreamDefeated) GeoSwitch.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.killedMageLord) SoulSwitch.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.nailsmithConvoArt) SoulSpeed.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.zotePrecept > 56) ZoteBorn.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.visitedWhitePalace) ElderStone.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.gaveSlykey && PlayerData.instance.slyConvoNailHoned && PlayerData.instance.completionPercentage > 100) SlyDeal.Instance.Settings(Settings).Got = true;
                if (PlayerData.instance.honedNail) GiantNail.Instance.Settings(Settings).Got = true;
                //end
                //ik it is messy, but what else is there to do. Also, if u are seeing this code and think that there is a more appropriate time to give the charm, DM me on discord. I am BubkisLord#5187            

                if (PlayerData.instance.maxHealth < 1)
                {
                    HeroController.instance.AddToMaxHealth(1);
                }
                foreach (Charm charm in Fyrenest.Charms)
                {
                    charm.Settings(Settings).Cost = charm.DefaultCost;
                }
            }
        }
        private bool optionOne;
        private bool optionTwo;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new IMenuMod.MenuEntry {
                    Name = "Charms Enabled",
                    Description = "Toggle if charms are enabled.",
                    Values = new string[] {
                        "Off",
                        "On"
                    },
                    Saver = opt => this.optionOne = opt switch {
                        0 => false,
                        1 => true,
                        // This should never be called
                        _ => throw new InvalidOperationException()
                    },
                    Loader = () => this.optionOne switch {
                        false => 0,
                        true => 1,
                    }
                },
                new IMenuMod.MenuEntry {
                    Name = "Lore Enabled",
                    Description = "Toggle if custom text is enabled",
                    Values = new string[] {
                        "Off",
                        "On"
                    },
                    Saver = opt => this.optionTwo = opt switch {
                        0 => false,
                        1 => true,
                        // This should never be called
                        _ => throw new InvalidOperationException()
                    },
                    Loader = () => this.optionTwo switch {
                        false => 0,
                        true => 1,
                    }
                }
            };
        }


        private int charmSelect = 1;
        public string LanguageGet(string key, string sheetTitle, string orig)
        {
            if (Enabled)
            {
                if (string.IsNullOrEmpty(orig))
                {
                    return "";
                }

                if (key == "HU_DEFEAT" && sheetTitle == "Ghosts")
                {
                    return "My mind... it clears. Have we been sleeping? No, I am not speaking to you, little knight. ...I remember... ...those proud lords, were they truly monsters? Their eyes, bright and clear. Why, why did I fear them so? They were going to help... ...it was I who brought the madness... ...No, not I. You, the voice... ...I remember now... ...But finally, you have stopped, you cruel voice... ...Who are you? Whose voice wonders through my mind?...";
                }

                if (key == "DISTANT_VILLAGE" && sheetTitle == "Map Zones")
                {
                    return "Herrah's Den";
                }

                if (key == "BONE_FOREST" && sheetTitle == "Map Zones")
                {
                    return "Bone Forest";
                }

                if (key == "TEST_AREA" && sheetTitle == "Map Zones")
                {
                    return "Test area, TELL ME WHERE U FOUND THIS. Ping me @BubkisLord#5187 (discord)";
                }

                if (key == "DREAM_WORLD" && sheetTitle == "Map Zones")
                {
                    return "Dream World";
                }

                if (key == "ELDERBUG_FLOWER" && sheetTitle == "Prompts")
                {
                    return "Give the Elderbug a flower for literal no reason?";
                }

                if (key == "CARD" && sheetTitle == "Cornifer")
                {
                    return "Hi there, it's me! Conifer. It seems you are here, but I have left. Come at see me or Pine at dirtmouth!";
                }

                if (key == "ELDERBUG_INTRO_VISITEDCROSSROAD" && sheetTitle == "Elderbug")
                {
                    return "WHAT WAS THAT? THE ONLY VISITOR FOR YEARS JUST WALKS PAST ME? HOW DARE YOU!";
                }

                if (key == "ELDERBUG_DREAM" && sheetTitle == "Elderbug")
                {
                    return "Hello? Is someone there? Who is that? Aah! What was that? That feeling. ...Like the cold, terrifying embrace of death...";
                }

                if (orig.Contains("Hollow Knight"))
                {
                    return orig.Replace("Hollow Knight", "Infected Vessel");
                }

                if (orig.Contains("Pure Vessel"))
                {
                    return orig.Replace("Pure Vessel", "Hollow Vessel");
                }

                if (orig.Contains("The Fading Town"))
                {
                    return orig.Replace("The Fading Town", "The Realm of The Old One");
                }
                if (orig.Contains("Elderbug"))
                {
                    return orig.Replace("Elderbug", "The Old One");
                }
                if (orig.Contains("Dirtmouth"))
                {
                    return orig.Replace("Dirtmouth", "Fyrecamp");
                }
                if (orig.Contains("Deepnest"))
                {
                    return orig.Replace("Deepnest", "Deepfyre");
                }
                if (orig.Contains("Iselda"))
                {
                    return orig.Replace("Iselda", "Pine");
                }
                if (orig.Contains("Cornifer"))
                {
                    return orig.Replace("Conifer", "Conifer");
                }
                if (orig.Contains("Hallownest"))
                {
                    return orig.Replace("Hallownest", "Fyrenest");
                }
                if (orig.Contains("Howling Cliffs"))
                {
                    return orig.Replace("Howling Cliffs", "Inferno's Peak");
                }
                if (orig.Contains("Fungal Wastes"))
                {
                    return orig.Replace("Fungal Wastes", "Defiled Wastelands");
                }
                if (orig.Contains("Kingdom's Edge"))
                {
                    return orig.Replace("Kingdom's Edge", "Fyre's Edge");
                }
                if (orig.Contains("Ancient Basin"))
                {
                    return orig.Replace("Ancient Basin", "Void Basin");
                }
                if (orig.Contains("Abyss"))
                {
                    return orig.Replace("Abyss", "Pit");
                }
                if (orig.Contains("Crossroads"))
                {
                    return orig.Replace("Crossroads", "Crossroads of Flame");
                }
                if (orig.Contains("Resting Grounds"))
                {
                    return orig.Replace("Resting Grounds", "Spirit Sanctuary");
                }
                if (orig.Contains("Crystal Peak"))
                {
                    return orig.Replace("Crystal Peak", "Crystalline Mountain");
                }
                if (orig.Contains("City of Tears"))
                {
                    return orig.Replace("City of Tears", "City of Flame");
                }
                if (SlyDeal.Instance.Equipped() && sheetTitle == "Prices")
                {
                    return "120";
                }
                if (ZoteBorn.Instance.Equipped() && sheetTitle == "Prices")
                {
                    return "10000";
                }
                if (SlyDeal.Instance.Equipped() && sheetTitle == "Elderbug")
                {
                    return "I don't think I can speak to you right now. There is a disgusting whiff of something coming by...";
                }
                if (SlyDeal.Instance.Equipped() && sheetTitle == "Iselda")
                {
                    return "Ugh... Please, leave.. The smell! I'm sorry, it is really bad.";
                }
                if (SlyDeal.Instance.Equipped() && sheetTitle == "Enemy Dreams")
                {
                    return "...That stench... ...disgusting...";
                }
                if (SlyDeal.Instance.Equipped() && sheetTitle == "Sly")
                {
                    return "I see you are wearing that charm I gave you.. Not near me please.";
                }
                if (SlyDeal.Instance.Equipped() && sheetTitle == "Cornifer")
                {
                    return "You can have a map! Have everything! But get me away from that stench!";
                }
                if (MarkofStrength.Instance.Equipped() && orig.Contains("Shade Soul"))
                {
                    return orig.Replace("Shade Soul", "Void Scythe");
                }
                if (MarkofStrength.Instance.Equipped() && orig.Contains("Vengeful Spirit"))
                {
                    return orig.Replace("Vengeful Spirit", "Mantis Scythe");
                }
                if (sheetTitle == "Lore Tablets" && key == "WISHING_WELL_INSPECT")
                {
                    return "A true follower of the Pale King gives all they own for the kingdom.";
                }
                if (sheetTitle == "Lore Tablets" && key == "TUT_TAB_02")
                {
                    return "For those who enter Fyrenest from the far lands, Hallownest, and The Glimmering Realm. Beyond this point you enter the land of the Pale King's home. Step across this threshold and obey our laws. Bear witness to one of the last civilisations, one of the eternal Kingdoms.\n\nFyrenest.";
                }
                if (sheetTitle == "Lore Tablets" && key == "ARCHIVE_02")
                {
                    return "ZONEMY WILL BRING LIGHT. ZONEMY WILL BRING POWER. ZONEMY WILL BRING PEACE, VOID AGAINST THE LIGHT.";
                }
                if (sheetTitle == "Lore Tablets" && key == "RANDOM_POEM_STUFF")
                {
                    return "In the wilds beyond, in Hallownest, in the far lands, they speak our name. Of our never ending prowess, of our ability to contain the light. The ability to stop what looms above.                                                 - Excerpt from Ode to Fyrenest by Zonemy the Teacher.";
                }
                if (sheetTitle == "Lore Tablets" && key == "RUINS_FOUNTAIN")
                {
                    return "Memorial to the Infected Vessel.\n\nIn its vault, far above, stopping the light.\n\nThrough its sacrifice Fyrenest lasts eternal.";
                }
                if (sheetTitle == "Lore Tablets" && key == "")
                {
                    return "";
                }
                if (sheetTitle == "Lore Tablets" && key == "")
                {
                    return "";
                }
                if (sheetTitle == "Lore Tablets" && key == "")
                {
                    return "";
                }
            }
            return orig;
        }

        public float grav = 0f;
        public float gravsaved = 0f;

        public int SelectCharm(int bubkis)
        {
            if (charmSelect > Charms.Count)
            {
                charmSelect = 0;
            }
            charmSelect += bubkis;
            return bubkis;
        }

        public void AddHealth()
        {
            HeroController.instance.AddHealth(1);
        }
        public void TakeHealth()
        {
            HeroController.instance.TakeHealth(1);
        }
        public void AddMaxHealth()
        {
            HeroController.instance.AddToMaxHealth(1);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.Instance.Settings(Settings).Equipped = true;
            SoulSlow.Instance.Settings(Settings).Equipped = false;
        }

        public void TakeMaxHealth()
        {
            HeroController.instance.AddToMaxHealth(-1);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.Instance.Settings(Settings).Equipped = true;
            SoulSlow.Instance.Settings(Settings).Equipped = false;
        }
        public void MaxHealthReset()
        {
            var CurrentMaxHp = PlayerData.instance.maxHealth;
            var WantedMaxHp = 5;
            var MaxHpChangeAmount = WantedMaxHp - CurrentMaxHp;
            HeroController.instance.AddToMaxHealth(MaxHpChangeAmount);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.Instance.Settings(Fyrenest.Instance.Settings).Equipped = true;
            SoulSlow.Instance.Settings(Fyrenest.Instance.Settings).Equipped = false;
        }

        public void HealthReset()
        {
            var hp = PlayerData.instance.health;
            var maxhp = PlayerData.instance.maxHealth;
            var changeamount = maxhp - hp;
            HeroController.instance.AddHealth(changeamount);
        }
        public int FindSelectedCharm(int charmNumberino)
        {
            var _SelectedCharm = Charms[charmNumberino];
            return charmNumberino;
        }

        // breaks infinite loop when reading equippedCharm_X
        private bool Equipped(Charm c) => c.Settings(Settings).Equipped;

        private Dictionary<(string Key, string Sheet), Func<string>> TextEdits = new();

        internal void AddTextEdit(string key, string sheetName, string text)
        {
            TextEdits.Add((key, sheetName), () => text);
        }

        internal void AddTextEdit(string key, string sheetName, Func<string> text)
        {
            TextEdits.Add((key, sheetName), text);
        }

        internal SaveSettings Settings = new();

        private bool ReadCharmBools(string boolName, bool value)
        {
            if (BoolGetters.TryGetValue(boolName, out var f))
            {
                return f(value);
            }
            return value;
        }

        private bool WriteCharmBools(string boolName, bool value)
        {
            if (BoolSetters.TryGetValue(boolName, out var f))
            {
                f(value);
            }
            return value;
        }

        private string GetCharmStrings(string key, string sheetName, string orig)
        {
            if (TextEdits.TryGetValue((key, sheetName), out var text))
            {
                return text();
            }
            return orig;
        }

        internal void AddFsmEdit(string objName, string fsmName, Action<PlayMakerFSM> edit)
        {
            var key = (objName, fsmName);
            var newEdit = edit;
            if (FSMEdits.TryGetValue(key, out var orig))
            {
                newEdit = fsm => {
                    orig(fsm);
                    edit(fsm);
                };
            }
            FSMEdits[key] = newEdit;
        }

        private void EditFSMs(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);
            if (FSMEdits.TryGetValue((fsm.gameObject.name, fsm.FsmName), out var edit))
            {
                edit(fsm);
            }
        }

        private void StartTicking()
        {
            // Use our own object to hold timers so that GameManager.StopAllCoroutines
            // does not kill them.
            var timerHolder = new GameObject("Timer Holder");
            GameObject.DontDestroyOnLoad(timerHolder);
            var timers = timerHolder.AddComponent<DummyMonoBehaviour>();
            foreach (var t in Tickers)
            {
                IEnumerator ticker()
                {
                    while (true)
                    {
                        try
                        {
                            t.Func();
                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                        }
                        yield return new WaitForSeconds(t.Period);
                    }
                }

                timers.StartCoroutine(ticker());
            }
        }

        private void CountOurCharms(On.PlayerData.orig_CountCharms orig, PlayerData self)
        {
            orig(self);
            self.SetInt("charmsOwned", self.GetInt("charmsOwned") + Charms.Count(c => c.Settings(Settings).Got));
        }
        internal static void UpdateNailDamage()
        {
            IEnumerator WaitThenUpdate()
            {
                yield return null;
                PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            }
            GameManager.instance.StartCoroutine(WaitThenUpdate());
        }

        private void GrantAllOurCharms()
        {
            foreach (var charm in Charms)
            {
                charm.Settings(Settings).Got = true;
            }
        }

        public int GiveSpecificCharm(int numberino)
        {
            Charms[numberino].Settings(Settings).Got = true;
            return numberino;
        }

        public void TakeAllOurCharms()
        {
            foreach (var charm in Charms)
            {
                charm.Settings(Settings).Got = false;
            }
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
