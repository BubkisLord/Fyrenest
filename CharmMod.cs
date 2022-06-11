global using System;
global using System.IO;
global using System.Collections;
global using Modding;
global using UnityEngine;
global using SFCore;
global using System.Collections.Generic;
global using System.Linq;
using Satchel.BetterMenus;

namespace CharmMod
{
    public class CharmMod : Mod, ICustomMenuMod, IMod
    {
        private static List<Charm> Charms = new()
        {
            Quickfall.Instance,
            Slowfall.Instance,
            SturdyNail.Instance,
            BetterCDash.Instance,
            GlassCannon.Instance,
            HKBlessing.Instance,
            HuntersMark.Instance,
            PowerfulDash.Instance,
            HealthyShell.Instance,
            DoubleDash.Instance,
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
            ElderStone.Instance
        };

        public int NewCharms = Charms.Count; //STARTS AT 0 (almost definately)
        public int OldCharms = 40; //STARTS AT 1 (for some reason)
        internal static CharmMod Instance;

        private Dictionary<string, Func<bool, bool>> BoolGetters = new();
        private Dictionary<string, Action<bool>> BoolSetters = new();
        private Dictionary<string, Func<int, int>> IntGetters = new();
        private Dictionary<(string, string), Action<PlayMakerFSM>> FSMEdits = new();
        private List<(int Period, Action Func)> Tickers = new();

        public override void Initialize()
        {
            Log("Initializing");
            Instance = this;
            foreach (var charm in Charms)
            {
                var num = CharmHelper.AddSprites(EmbeddedSprites.Get(charm.Sprite))[0];
                charm.Num = num;
                var settings = charm.Settings;
                this.IntGetters[string.Format("charmCost_{0}", num)] = ((int _) => settings(this.Settings).Cost);
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
            }
            for (var i = 1; i <= 40; i++)
            {
                var num = i; // needed for closure to capture a different copy of the variable each time
                BoolGetters[$"equippedCharm_{num}"] = value => value;
                this.IntGetters[string.Format("charmCost_{0}", num)] = ((int value) => value);
            }

            ModHooks.GetPlayerBoolHook += ReadCharmBools;
            ModHooks.SetPlayerBoolHook += WriteCharmBools;
            ModHooks.GetPlayerIntHook += ReadCharmCosts;
            ModHooks.LanguageGetHook += GetCharmStrings;
            // This will run after Rando has already set up its item placements.
            On.PlayMakerFSM.OnEnable += EditFSMs;
            On.PlayerData.CountCharms += CountOurCharms;
            ModHooks.NewGameHook += GiveCharms;
            ModHooks.HeroUpdateHook += OnUpdate;
            ModHooks.LanguageGetHook += LanguageGet;

            StartTicking();
            if (ModHooks.GetMod("DebugMod") != null)
            {
                DebugModHook.GiveAllCharms(() => {
                    GrantAllOurCharms();
                    PlayerData.instance.CountCharms();
                });
            }
        }
        private int charmSelect = 0;
        public string LanguageGet(string key, string sheetTitle, string orig)
        {
            //Check for the key and sheet for MainMenu "Yes" text
            if (key == "HU_DEFEAT" && sheetTitle == "Ghosts")
            {
                return "My mind... it clears. Have we been sleeping? No, not you, little knight. ...I remember... ...those proud lords, were they truly monsters? Their eyes, bright and clear. Why, why did you tell me to fear them so? They were going to help...  ...it was I who brought the madness... ...finally, you have stopped, you cruel voice...    ...who are you?...";
            }

            if (key == "TOWN" && sheetTitle == "Map Zones")
            {
                return "Elderbug The Amazing's Domain";
            }

            if (key == "CLIFFS" && sheetTitle == "Map Zones")
            {
                return "The Forgotten Edge";
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
                return "Give him the Delicate Flower like a giga-chad?";
            }

            if (key == "ELDERBUG_INTRO_VISITEDCROSSROAD" && sheetTitle == "Elderbug")
            {
                return "WHAT THE HELL WAS THAT? THE ONLY VISITOR FOR YEARS JUST WALKS PAST ME? HOW DARE YOU! Go away!";
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

            if (orig.Contains("Mantis"))
            {
                return orig.Replace("Mantis", "Bubkis");
            }

            if (orig.Contains("mantis"))
            {
                return orig.Replace("mantis", "bubkis");
            }

            if (orig.Contains("The Forgotten Town"))
            {
                return orig.Replace("The Forgotten Town", "The Realm of Elderbug");
            }
            if (SlyDeal.Instance.Equipped() && sheetTitle == "Prices")
            {
                return "120";
            }
            if (ZoteBorn.Instance.Equipped() && sheetTitle == "Prices")
            {
                return "10000";
            }

            return orig;
        }

        public float grav = 0f;
        public float gravsaved = 0f;

        private Menu MenuRef;
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates)
        {
            MenuRef ??= new Menu(
                "CharmMod",
                new Element[]
                {
                    new TextPanel("Health Changing", 1000, 60),
                    new MenuButton("Reset health", "Make health to max health.", (_) => HealthReset()),
                    new MenuButton("Add health", "Change health by 1.", (_) => AddHealth()),
                    new MenuButton("Take health", "Change health by -1.", (_) => TakeHealth()),
                    new TextPanel("Max Health Changing", 1000, 60),
                    new MenuButton("Reset Max Health", "Sets max health to 5", (_) => MaxHealthReset()),
                    new MenuButton("Add Max Health", "Increase max health by one. (Equip and de-equip the slow soul charm to update)", (_) => AddMaxHealth()),
                    new MenuButton("Take Max Health", "Decrease max health by one. (Equip and de-equip the slow soul charm to update)", (_) => TakeMaxHealth()),
                    new TextPanel("Soul Changing", 1000, 60),
                    new MenuButton("Reset Soul", "Make soul the max soul amount.", (_) => HeroController.instance.AddMPCharge(PlayerData.instance.MPReserveMax)),
                    new MenuButton("Add Soul", "Add one charge of soul.", (_) => HeroController.instance.AddMPCharge(33)),
                    new MenuButton("Take Soul", "Take one charge of soul.", (_) => HeroController.instance.TakeMP(33)),
                    new TextPanel("Charms", 1000, 60),
                    new MenuButton("Select Specific Charm", "Current Charm Selected: Quickfall", (_) => {
                        //find element by Id
                        Element elem = MenuRef.Find("Select Specific Charm");
                        MenuButton buttonElem = elem as MenuButton;
                        buttonElem.Name = "Select Specific Charm"; //change name
                        CharmMod.Instance.FindSelectedCharm(charmSelect);
                        var SelectedCharm =  Charms[charmSelect];
                        string SelCharm = SelectedCharm.ToString();
                        string FinalSelcharm = SelCharm.Replace("CharmMod.", "");
                        buttonElem.Description = "Current selected charm: "+FinalSelcharm; //change description
                        buttonElem.Update();

                        SelectCharm(1); //trigger code
                    }),
                    new MenuButton("Give Specific Charm", "Pick the charm", (_) => GiveSpecificCharm(charmSelect)),
                    new MenuButton("Give Charms", "Get all CharmMod charms.", (_) => CharmMod.Instance.GrantAllOurCharms()),
                    new MenuButton("Take Charms", "Take all CharmMod charms.", (_) => CharmMod.Instance.TakeAllOurCharms()),
                }
            );
            return MenuRef.GetMenuScreen(modListMenu);
        }

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
            SoulSlow.Instance.Settings(CharmMod.Instance.Settings).Equipped = true;
            SoulSlow.Instance.Settings(CharmMod.Instance.Settings).Equipped = false;
        }

        public void TakeMaxHealth()
        {
            HeroController.instance.AddToMaxHealth(-1);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.Instance.Settings(CharmMod.Instance.Settings).Equipped = true;
            SoulSlow.Instance.Settings(CharmMod.Instance.Settings).Equipped = false;
        }
        public void MaxHealthReset()
        {
            var CurrentMaxHp = PlayerData.instance.maxHealth;
            var WantedMaxHp = 5;
            var MaxHpChangeAmount = WantedMaxHp - CurrentMaxHp;
            HeroController.instance.AddToMaxHealth(MaxHpChangeAmount);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.Instance.Settings(CharmMod.Instance.Settings).Equipped = true;
            SoulSlow.Instance.Settings(CharmMod.Instance.Settings).Equipped = false;
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
        
        private Dictionary<(string Key, string Sheet), Func<string>> TextEdits = new();

        internal void AddTextEdit(string key, string sheetName, string text)
        {
            TextEdits.Add((key, sheetName), () => text);
        }

        internal void AddTextEdit(string key, string sheetName, Func<string> text)
        {
            TextEdits.Add((key, sheetName), text);
        }
        private void GiveCharms()
        {
            GrantAllOurCharms();
        }

        public override string GetVersion() => "8.30.3";

        internal SaveSettings Settings = new();

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

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

        private string GetCharmStrings(string key, string sheetName, string orig)
        {
            if (TextEdits.TryGetValue((key, sheetName), out var text))
            {
                return text();
            }
            return orig;
        }
        private void OnUpdate()
        {
            //give charms when certain things are done.

            if (PlayerData.instance.colosseumBronzeCompleted) Quickfall.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.colosseumSilverCompleted) Slowfall.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasShadowDash) PowerfulDash.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasNailArt) SturdyNail.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasHuntersMark) HuntersMark.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasDreamGate) SoulHunger.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasDreamNail) SoulSlow.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasSuperDash && PlayerData.instance.gaveSlykey) BetterCDash.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.killedHollowKnight) HKBlessing.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasKingsBrand) HealthyShell.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.killedHollowKnightPrime) GlassCannon.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.bankerAccountPurchased) WealthyAmulet.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.colosseumGoldCompleted) RavenousSoul.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.canOvercharm) DoubleDash.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.collectorDefeated) SoulSpell.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.grubsCollected > 10) SlowTime.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.fatGrubKing) SpeedTime.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.mageLordDreamDefeated) GeoSwitch.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.killedMageLord) SoulSwitch.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.nailsmithConvoArt) SoulSpeed.Instance.Settings(Settings).Got = true;

            //end
            //ik it is messy, but what else is there to do. Also, if u are seeing this code and think that there is a more appropriate time to give the charm, DM me on discord. I am BubkisLord#5187


            if (PlayerData.instance.maxHealth < 1)
            {
                HeroController.instance.AddToMaxHealth(1);
            }
            foreach (Charm charm in CharmMod.Charms)
            {
                charm.Settings(this.Settings).Cost = charm.DefaultCost;
            }
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

        public void GrantAllOurCharms()
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
    }
}