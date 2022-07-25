global using System;
global using System.IO;
global using System.Collections;
global using Modding;
global using UnityEngine;
global using SFCore;
global using System.Collections.Generic;
global using System.Linq;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Satchel.BetterMenus;
using static Fyrenest.Fyrenest;

namespace Fyrenest
{
    public class Fyrenest : Mod, IMod, ICustomMenuMod, ILocalSettings<CustomLocalSaveData>, ITogglableMod
    {
        //Note, for the ModToggle variable, it is now unused.

        public static int charmSelect = 0;
        public static Fyrenest LoadedInstance { get; set; }

        public static CustomLocalSaveData LocalSaveData { get; set; } = new CustomLocalSaveData();
        
        public void OnLoadLocal(CustomLocalSaveData s) => Fyrenest.LocalSaveData = s;
        // This method gets called when the mod loader needs to save the Local settings.
        public CustomLocalSaveData OnSaveLocal() => Fyrenest.LocalSaveData;

        private readonly static List<Charm> Charms = new()
        {
            Quickfall.Instance,
            Slowfall.Instance,
            SturdyNail.Instance,
            BetterCDash.Instance,
            GlassCannon.Instance,
            HKBlessing.Instance,
            MarkofStrength.Instance,
            PowerfulDash.Instance,
            Fyrechild.Instance,
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
            MatosBlessing.Instance,
            HealthyShell.Instance,
            BlueBlood.Instance,
            ShellShield.Instance,
            WyrmForm.Instance,
            TripleJump.Instance,
            VoidSoul.Instance
        };

        public int NewCharms = Charms.Count; //STARTS AT 1
        public int OldCharms = 40; //STARTS AT 1
        
        internal static Fyrenest Instance;

        private readonly Dictionary<string, Func<bool, bool>> BoolGetters = new();
        private readonly Dictionary<string, Action<bool>> BoolSetters = new();
        private readonly Dictionary<string, Func<int, int>> IntGetters = new();
        private readonly Dictionary<(string, string), Action<PlayMakerFSM>> FSMEdits = new();
        private readonly List<(int Period, Action Func)> Tickers = new();
        public static List<IAbility> Abilities;
        public static Dictionary<string, Dictionary<string, GameObject>> Preloads;

        public override List<(string, string)> GetPreloadNames() => new()
        {
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
            if (Fyrenest.LoadedInstance != null) return;
            Fyrenest.LoadedInstance = this;

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
            On.GameManager.SaveGame += OnSave;

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

        public class CustomGlobalSaveData
        {
            // The number of times the player has loaded into a save.
            public int SavesLoaded;
        }

        // The local data to store that is specific to saves.
        public class CustomLocalSaveData
        {
            // What charms the player has in the specific save.
            public bool QuickfallGot = false;
            public bool SlowfallGot = false;
            public bool SturdyNailGot = false;
            public bool BetterCDashGot = false;
            public bool GlassCannonGot = false;
            public bool HKBlessingGot = false;
            public bool MarkofStrengthGot = false;
            public bool PowerfulDashGot = false;
            public bool HealthyShellGot = false;
            public bool OpportunisticDefeatGot = false;
            public bool SoulSpeedGot = false;
            public bool SoulSpellGot = false;
            public bool SoulHungerGot = false;
            public bool RavenousSoulGot = false;
            public bool SoulSwitchGot = false;
            public bool GeoSwitchGot = false;
            public bool WealthyAmuletGot = false;
            public bool SoulSlowGot = false;
            public bool SlowTimeGot = false;
            public bool SpeedTimeGot = false;
            public bool ZoteBornGot = false;
            public bool SlyDealGot = false;
            public bool ElderStoneGot = false;
            public bool GiantNailGot = false;
            public bool MatosBlessingGot = false;
            public bool ShellShieldGot = false;
            public bool VoidSoulGot = false;
            public bool BlueBloodGot = false;


            public bool QuickfallDonePopup = false;
            public bool SlowfallDonePopup = false;
            public bool SturdyNailDonePopup = false;
            public bool BetterCDashDonePopup = false;
            public bool GlassCannonDonePopup = false;
            public bool HKBlessingDonePopup = false;
            public bool MarkofStrengthDonePopup = false;
            public bool PowerfulDashDonePopup = false;
            public bool HealthyShellDonePopup = false;
            public bool OpportunisticDefeatDonePopup = false;
            public bool SoulSpeedDonePopup = false;
            public bool SoulSpellDonePopup = false;
            public bool SoulHungerDonePopup = false;
            public bool RavenousSoulDonePopup = false;
            public bool SoulSwitchDonePopup = false;
            public bool GeoSwitchDonePopup = false;
            public bool WealthyAmuletDonePopup = false;
            public bool SoulSlowDonePopup = false;
            public bool SlowTimeDonePopup = false;
            public bool SpeedTimeDonePopup = false;
            public bool ZoteBornDonePopup = false;
            public bool SlyDealDonePopup = false;
            public bool ElderStoneDonePopup = false;
            public bool GiantNailDonePopup = false;
            public bool MatosBlessingDonePopup = false;
            public bool ShellShieldDonePopup = false;
            public bool VoidSoulDonePopup = false;
            public bool BlueBloodDonePopup = false;
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
            orig(self);
        }

        private int ReadCharmCosts(string intName, int value)
        {
            bool flag = this.IntGetters.TryGetValue(intName, out Func<int, int> cost);
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
            if (insanity)
            {
                for (int i = 0; i < 40; i++)
                {
                    PlayerData.instance.SetInt($"charmCost_{i}", 0);
                }
            }
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
            if (PlayerData.instance.hasAllNailArts && PlayerData.instance.hasKingsBrand) MatosBlessing.Instance.Settings(Settings).Got = true;
            PowerfulDash.Instance.Settings(Settings).Cost = 10;
            if (PlayerData.instance.maxHealth < 1)
            {
                HeroController.instance.AddToMaxHealth(1);
            }
            foreach (Charm charm in Charms)
            {
                if (!insanity)
                    charm.Settings(Settings).Cost = charm.DefaultCost;
                else
                    charm.Settings(Settings).Cost = 0;
            }
            AchievementHelper.AddAchievement("voidsoulachievement", EmbeddedSprites.Get("VoidSoulAchievement.png"), "Soul of Void", "Gain and wear the Void Soul charm.", false);
            if (VoidSoul.Instance.Equipped() && VoidSoul.Instance.Settings(Settings).Got) GameManager.instance.AwardAchievement("voidsoulachievement");
            //change dreamshield to cost 2 notches.
            PlayerData.instance.charmCost_38 = 2;

            //change local save data
            if (!LocalSaveData.QuickfallGot && Quickfall.Instance.Settings(Settings).Got) LocalSaveData.QuickfallGot = true;
            if (!LocalSaveData.SlowfallGot && Slowfall.Instance.Settings(Settings).Got) LocalSaveData.SlowfallGot = true;
            if (!LocalSaveData.SturdyNailGot && SturdyNail.Instance.Settings(Settings).Got) LocalSaveData.SturdyNailGot = true;
            if (!LocalSaveData.BetterCDashGot && BetterCDash.Instance.Settings(Settings).Got) LocalSaveData.BetterCDashGot = true;
            if (!LocalSaveData.GlassCannonGot && GlassCannon.Instance.Settings(Settings).Got) LocalSaveData.GlassCannonGot = true;
            if (!LocalSaveData.HKBlessingGot && HKBlessing.Instance.Settings(Settings).Got) LocalSaveData.HKBlessingGot = true;
            if (!LocalSaveData.MarkofStrengthGot && MarkofStrength.Instance.Settings(Settings).Got) LocalSaveData.MarkofStrengthGot = true;
            if (!LocalSaveData.PowerfulDashGot && PowerfulDash.Instance.Settings(Settings).Got) LocalSaveData.PowerfulDashGot = true;
            if (!LocalSaveData.HealthyShellGot && HealthyShell.Instance.Settings(Settings).Got) LocalSaveData.HealthyShellGot = true;
            if (!LocalSaveData.OpportunisticDefeatGot && OpportunisticDefeat.Instance.Settings(Settings).Got) LocalSaveData.OpportunisticDefeatGot = true;
            if (!LocalSaveData.SoulSpeedGot && SoulSpeed.Instance.Settings(Settings).Got) LocalSaveData.SoulSpeedGot = true;
            if (!LocalSaveData.SoulSpellGot && SoulSpell.Instance.Settings(Settings).Got) LocalSaveData.SoulSpellGot = true;
            if (!LocalSaveData.SoulHungerGot && SoulHunger.Instance.Settings(Settings).Got) LocalSaveData.SoulHungerGot = true;
            if (!LocalSaveData.RavenousSoulGot && RavenousSoul.Instance.Settings(Settings).Got) LocalSaveData.RavenousSoulGot = true;
            if (!LocalSaveData.SoulSwitchGot && SoulSwitch.Instance.Settings(Settings).Got) LocalSaveData.SoulSwitchGot = true;
            if (!LocalSaveData.GeoSwitchGot && GeoSwitch.Instance.Settings(Settings).Got) LocalSaveData.GeoSwitchGot = true;
            if (!LocalSaveData.WealthyAmuletGot && WealthyAmulet.Instance.Settings(Settings).Got) LocalSaveData.WealthyAmuletGot = true;
            if (!LocalSaveData.SoulSlowGot && SoulSlow.Instance.Settings(Settings).Got) LocalSaveData.SoulSlowGot = true;
            if (!LocalSaveData.SlowTimeGot && SlowTime.Instance.Settings(Settings).Got) LocalSaveData.SlowTimeGot = true;
            if (!LocalSaveData.SpeedTimeGot && SpeedTime.Instance.Settings(Settings).Got) LocalSaveData.SpeedTimeGot = true;
            if (!LocalSaveData.ZoteBornGot && ZoteBorn.Instance.Settings(Settings).Got) LocalSaveData.ZoteBornGot = true;
            if (!LocalSaveData.SlyDealGot && SlyDeal.Instance.Settings(Settings).Got) LocalSaveData.SlyDealGot = true;
            if (!LocalSaveData.ElderStoneGot && ElderStone.Instance.Settings(Settings).Got) LocalSaveData.ElderStoneGot = true;
            if (!LocalSaveData.GiantNailGot && GiantNail.Instance.Settings(Settings).Got) LocalSaveData.GiantNailGot = true;
            if (!LocalSaveData.MatosBlessingGot && MatosBlessing.Instance.Settings(Settings).Got) LocalSaveData.MatosBlessingGot = true;
            if (!LocalSaveData.ShellShieldGot && ShellShield.Instance.Settings(Settings).Got) LocalSaveData.ShellShieldGot = true;
        }

        public bool insanity = false;
        private void OnSave(On.GameManager.orig_SaveGame orig, GameManager self)
        {
            CheckCharmPopup();
            orig(self);
        }

        public bool ToggleButtonInsideMenu => true;

        public string selectedCharm = Charms[charmSelect].ToString().Replace("Fyrenest.", "").Replace(".Instance", "");
        public void OnLoadLocal(SaveSettings s) => Settings = s;

        //UNUSED
        public static int ModToggle = 0;

        private Menu MenuRef;
        private Menu ExtraMenuRef;
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            MenuRef ??= new Menu(
                        "Fyrenest",
                        new Element[]
                        {
                            new TextPanel("Fyrenest Settings", 1000, 70),
                            new MenuButton("Reset health", "Make health to max health.", (_) => HealthReset(), false, Id:"Test"),
                            new TextPanel("Health Settings", 1000, 70),
                            new MenuButton("Reset health", "Make health to max health.", (_) => HealthReset(), false, Id:"Reset Health"),
                            new MenuButton("Add health", "Change health by 1.", (_) => AddHealth(), false, Id:"Add Health"),
                            new MenuButton("Take health", "Change health by -1.", (_) => TakeHealth(), false, Id:"Take Health"),
                            new TextPanel("Max Health Settings", 1000, 70),
                            new MenuButton("Reset Max Health", "Sets max health to 5", (_) => MaxHealthReset()),
                            new MenuButton("Add Max Health", "Increase max health by one. (Equip and de-equip the slow soul charm to update)", (_) => AddMaxHealth()),
                            new MenuButton("Take Max Health", "Decrease max health by one. (Equip and de-equip the slow soul charm to update)", (_) => TakeMaxHealth()),
                            new TextPanel("Soul Settings", 1000, 70),
                            new MenuButton("Reset Soul", "Make soul the max soul amount.", (_) => HeroController.instance.AddMPCharge(PlayerData.instance.MPReserveMax)),
                            new MenuButton("Add Soul", "Add one charge of soul.", (_) => HeroController.instance.AddMPCharge(33)),
                            new MenuButton("Take Soul", "Take one charge of soul.", (_) => HeroController.instance.TakeMP(33)),
                            new TextPanel("Charm Settings", 1000, 70),
                            new MenuRow(new List<Element>() //first parameter is a list of elements you want to add to the row
                                {
                                    new MenuButton("Select Specific Charm", "Current Charm Selected: Quickfall", (_) => {
                                        //find element by Id
                                        Element elem = MenuRef.Find("SelectSpecificCharm");
                                        MenuButton buttonElem = elem as MenuButton;
                                        buttonElem.Name = "Select Specific Charm"; //change name
                                        string SelectedCharm =  Charms[charmSelect].ToString();
                                        string charmNameSelected = SelectedCharm.Replace("Fyrenest.", "").Replace(".Instance", "");
                                        string desc = "Current selected charm: "+charmNameSelected; //set desc to the new wanted description
                                        buttonElem.Description = desc; //change description
                                        buttonElem.Update();//Update button
                                        MenuRef.Update();

                                        SelectCharm(1); //trigger normal function
                                    }, false, Id:"SelectSpecificCharm"),
                                    new MenuButton("Give Specific Charm", "Add the charm to your inventory.", (_) => GiveSpecificCharm(1), false, Id:"GiveSpecCharm"),
                                }, "Specific Charm Selection"),
                            new MenuButton("Give Charms", "Get all CharmMod charms.", (_) => GrantAllOurCharms()),
                            new MenuButton("Take Charms", "Take all CharmMod charms.", (_) => TakeAllOurCharms()),
                            new MenuButton("Update Charms", "Reloads charm effects.", (_) => ReloadCharmEffects()),
                            new MenuButton("More Settings", "", (_) => UIManager.instance.UIGoToDynamicMenu(ExtraMenuRef.menuScreen)),
                        }
            );

            ExtraMenuRef ??= new Menu(
                "Extra Fyrenest Settings",
                new Element[]
                {
                    new TextPanel("Extra Mod Settings",1000, 100, Id:"ModToggleTitle"),
                    new HorizontalOption( "Insanity Mode", "Toggle insanity mode.", new string[]{ "INSANITY", "Normal"}, (setting) => { ModToggle = setting; if(setting == 0) { insanity = true; } else { insanity = false; } }, () => ModToggle),
                    new MenuButton("Back button", "Go back to main page.", (_) => UIManager.instance.UIGoToDynamicMenu(MenuRef.menuScreen)),
                }
            );
            MenuRef.GetMenuScreen(modListMenu);
            ExtraMenuRef.GetMenuScreen(ExtraMenuRef.menuScreen);

            return MenuRef.menuScreen;
        }
        
        private void ReloadCharmEffects()
        {
            HeroController.instance.CharmUpdate();
            GameManager.instance.UpdateBlueHealth();
            GameManager.instance.SaveGame();
            return;
        }
        public static MenuButton NavigateToMenu(string name, string description, Func<MenuScreen> getScreen)
        {
            return new MenuButton(
                 name,
                 description,
                 (mb) =>
                 {
                     UIManager.instance.UIGoToDynamicMenu(getScreen());
                 },
                 proceed: true
             );
        }
        public string LanguageGet(string key, string sheetTitle, string orig)
        {
            if (key == "HU_DEFEAT" && sheetTitle == "Ghosts")
            {
                return "My mind... it clears. Have we been sleeping?<page>No, I am not speaking to you, little knight.<page>...I remember... ...those proud lords, were they truly monsters? Their eyes, bright and clear. Why, why did I fear them so? They were going to help...<page>...it was I who brought the madness...<page>...No, not I. You, the voice... ...I remember now... ...But finally, you have stopped, you cruel voice...<page>...Who are you? Whose voice wonders through my mind?...<page>...How?...";
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
                return "WHAT WAS THAT? THE ONLY VISITOR FOR YEARS JUST WALKS PAST ME?<page>Okay, look. I haven't talked to someone in so long. I just want to talk. Well, consider it forgotten. No point in keeping grudges!";
            }

            if (key == "ELDERBUG_DREAM" && sheetTitle == "Elderbug")
            {
                return "Hello? Is someone there?<page>Who is that? Aah! What was that? That feeling...<page>...Like the cold, terrifying embrace of death...";
            }

            if (key == "BELIEVE_TAB_50" && sheetTitle == "Backer Messages")
            {
                return "Good job on completing the game with Fyrenest on! What should I update? What new features would you like to see next? What could I improve on? Tell me on the Discord Hollow Knight Modding Server!";
            }

            if (key == "BANKER_DREAM_SPA" && sheetTitle == "Banker")
            {
                return "Ahh! Leave me be, you stout little knight! What grossness!";
            }

            if (key == "BANKER_DEPOSIT" && sheetTitle == "Banker")
            {
                return "Thank you! I will keep your hard earned geo safe! Hee hee heeeee!";
            }

            if (key == "BANKER_BALANCE_ZERO_REPEAT" && sheetTitle == "Banker")
            {
                return "Please! Please don't hurt me! I have no geo left!";
            }

            if (key == "BANKER_SPA_REPEAT" && sheetTitle == "Banker")
            {
                return "We're still friends right? Remember all those great times together! All the banking!";
            }

            if (key == "CP2" && sheetTitle == "GRIMMSYCOPHANT_INSPECT")
            {
                return "Raw energy radiates off this corpse... What could this mean?";
            }

            if (key == "CP2" && sheetTitle == "GRIMMSYCOPHANT_DREAM")
            {
                return "...I founded the troupe... ...Destroyed kingdoms... ...Grimm...";
            }

            if (key == "CROSSROADS_SUB" && sheetTitle == "Titles")
            {
                return "Of Flame";
            }

            if (key == "CROSSROADS_SUB_INF" && sheetTitle == "Titles")
            {
                return "Of Flame";
            }

            if (key == "CORNIFER_SUB" && sheetTitle == "Titles")
            {
                return "The Explorer";
            }

            if (key == "" && sheetTitle == "")
            {
                return "";
            }

            if (key == "" && sheetTitle == "")
            {
                return "";
            }

            if (key == "" && sheetTitle == "")
            {
                return "";
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
                orig.Replace("2", "1");
                orig.Replace("3", "2");
                orig.Replace("4", "2");
                orig.Replace("5", "3");
                orig.Replace("6", "3");
                orig.Replace("7", "4");
                orig.Replace("8", "4");
                orig.Replace("8", "4");
                orig.Replace("9", "5");
                return orig;
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
                return "Ugh... Please, leave.. The smell!<page>Just buy something. Or don't. Please leave the shop.";
            }
            if (SlyDeal.Instance.Equipped() && sheetTitle == "Enemy Dreams")
            {
                return "...That stench... ...disgusting...";
            }
            if (SlyDeal.Instance.Equipped() && sheetTitle == "Sly")
            {
                return "I see you are wearing that charm I gave you... Not near me please.";
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
            if (sheetTitle == "Cornifer" && key == "FUNGAL_WASTES_GREET")
            {
                return "Ahh my short friend, you've caught me at the perfect time. I'm impressed that you have got this far! I'm just about finished charting these noxious caverns. Very territorial types make their homes within this area. I'd suggest avoiding this place. I don't think it would be very safe for a fragile little one like you. I have heard of a group of deadly warriors, they seemed an intelligent bunch. I wouldn't go down there if I were you.";
            }
            if (sheetTitle == "Cornifer" && key == "GREENPATH_GREET")
            {
                return "Oh, hello there! I didn't think you would be here! You are surely having someone's help traversing this path... Surely, someone of your small stature couldn't get around like this! Buy a map, it will help you to get back to Fyrecamp. You are clearly lost for some reason.";
            }
            if (sheetTitle == "Cornifer" && key == "MINES_GREET")
            {
                return "Hello, my short little friend! What a suprise finding you here! Have you come to scale the mountains? I'm afraid you are much to pathetic to do that. Here, buy a map instead! It might help you find a way out.";
            }
            if (sheetTitle == "Cornifer" && key == "CROSSROADS_GREET")
            {
                return "Hello again! Still winding your way through these twisting highways? Just imagine how they must have looked during the kingdom's prime, thick with traffic and bustling with life! I wish I could have seen it. Oh, it is a shame that our old king is gone... Would you like to buy a map of the area to help you get out safely? You don't seem the adventurous type.";
            }
            if (sheetTitle == "Cornifer" && key == "CLIFFS_GREET")
            {
                return "Are you enjoying the bracing air? I doubt you have experienced something like this before, since you are always scrounging around underground, looking for geo like a hermit. Anyway, we are quite close to the borders of Fyrenest, and the desolate plains that surround it. I have heard that these plains make bugs go mad... Seeking escape, only to find lost memories and distant towns. I have heard of a place far away, where our king went when he left us. A place called Hallownest, a distant kingdom never to be found... I had a brother named Cornifer, he left in search of that horrid place... I dread what has happened to him... But, lingering on the past doesn't accomplish anything. I've drawn out a small map for the area, although simple, it is helpful nonetheless. Not knowing the full extents of a region can be quite frustrating.";
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

        private readonly Dictionary<(string Key, string Sheet), Func<string>> TextEdits = new();

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
            ShellShield.Instance.Trigger(boolName, value);
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
            static IEnumerator WaitThenUpdate()
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

        private void CheckCharmPopup()
        {
            if (PlayerData.instance.colosseumBronzeCompleted && !LocalSaveData.QuickfallDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("Quickfall.png"), "Gained Charm"); LocalSaveData.QuickfallDonePopup = true;
            if (PlayerData.instance.colosseumSilverCompleted && !LocalSaveData.SlowfallDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("Slowfall.png"), "Gained Charm"); LocalSaveData.SlowfallDonePopup = true;
            if (PlayerData.instance.hasShadowDash && !LocalSaveData.PowerfulDashDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("PowerfulDash.png"), "Gained Charm"); LocalSaveData.PowerfulDashDonePopup = true;
            if (PlayerData.instance.hasNailArt && !LocalSaveData.SturdyNailDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SturdyNail.png"), "Gained Charm"); LocalSaveData.SturdyNailDonePopup = true;
            if (PlayerData.instance.statueStateMantisLordsExtra.isUnlocked && !LocalSaveData.MarkofStrengthDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("MarkofStrength.png"), "Gained Charm"); LocalSaveData.MarkofStrengthDonePopup = true;
            if (PlayerData.instance.hasDreamGate && !LocalSaveData.SoulHungerDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SoulHunger.png"), "Gained Charm"); LocalSaveData.SoulHungerDonePopup = true;
            if (PlayerData.instance.hasDreamNail && !LocalSaveData.SoulSlowDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SoulSlow.png"), "Gained Charm"); LocalSaveData.SoulSlowDonePopup = true;
            if (PlayerData.instance.hasSuperDash && PlayerData.instance.gaveSlykey && !LocalSaveData.BetterCDashDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("BetterCDash.png"), "Gained Charm"); LocalSaveData.BetterCDashDonePopup = true;
            if (PlayerData.instance.killedHollowKnight && !LocalSaveData.HKBlessingDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("HKBlessing.png"), "Gained Charm"); LocalSaveData.HKBlessingDonePopup = true;
            if (PlayerData.instance.hasKingsBrand && !LocalSaveData.HealthyShellDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("HealthyShell.png"), "Gained Charm"); LocalSaveData.HealthyShellDonePopup = true;
            if (PlayerData.instance.killedHollowKnightPrime && !LocalSaveData.GlassCannonDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("GlassCannon.png"), "Gained Charm"); LocalSaveData.GlassCannonDonePopup = true;
            if (PlayerData.instance.bankerAccountPurchased && !LocalSaveData.WealthyAmuletDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("WealthyAmulet.png"), "Gained Charm"); LocalSaveData.WealthyAmuletDonePopup = true;
            if (PlayerData.instance.colosseumGoldCompleted && !LocalSaveData.RavenousSoulDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("RavenousSoul.png"), "Gained Charm"); LocalSaveData.RavenousSoulDonePopup = true;
            if (PlayerData.instance.canOvercharm && !LocalSaveData.OpportunisticDefeatDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("OpportunisticDefeat.png"), "Gained Charm"); LocalSaveData.OpportunisticDefeatDonePopup = true;
            if (PlayerData.instance.collectorDefeated && !LocalSaveData.SoulSpellDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SoulSpell.png"), "Gained Charm"); LocalSaveData.SoulSpellDonePopup = true;
            if (PlayerData.instance.grubsCollected > 10 && !LocalSaveData.SlowTimeDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SlowTime.png"), "Gained Charm"); LocalSaveData.SlowTimeDonePopup = true;
            if (PlayerData.instance.statueStateCollector.completedTier2 && !LocalSaveData.SpeedTimeDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SpeedTime.png"), "Gained Charm"); LocalSaveData.SpeedTimeDonePopup = true;
            if (PlayerData.instance.mageLordDreamDefeated && !LocalSaveData.GeoSwitchDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("GeoSwitch.png"), "Gained Charm"); LocalSaveData.GeoSwitchDonePopup = true;
            if (PlayerData.instance.killedMageLord && !LocalSaveData.SoulSwitchDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SoulSwitch.png"), "Gained Charm"); LocalSaveData.SoulSwitchDonePopup = true;
            if (PlayerData.instance.nailsmithConvoArt && !LocalSaveData.SoulSpeedDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SoulSpeed.png"), "Gained Charm"); LocalSaveData.SoulSpeedDonePopup = true;
            if (PlayerData.instance.zotePrecept > 56 && !LocalSaveData.ZoteBornDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("ZoteBorn.png"), "Gained Charm"); LocalSaveData.ZoteBornDonePopup = true;
            if (PlayerData.instance.visitedWhitePalace && !LocalSaveData.ElderStoneDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("Elderstone.png"), "Gained Charm"); LocalSaveData.ElderStoneDonePopup = true;
            if (PlayerData.instance.gaveSlykey && PlayerData.instance.slyConvoNailHoned && PlayerData.instance.completionPercentage > 100 && !LocalSaveData.SlyDealDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("SlyDeal.png"), "Gained Charm"); LocalSaveData.SlyDealDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.GiantNailDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("GiantNail.png"), "Gained Charm"); LocalSaveData.GiantNailDonePopup = true;
            if (PlayerData.instance.hasAllNailArts && PlayerData.instance.hasKingsBrand && !LocalSaveData.MatosBlessingDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("MatosBlessing.png"), "Gained Charm"); LocalSaveData.MatosBlessingDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.ShellShieldDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("ShellShield.png"), "Gained Charm"); LocalSaveData.ShellShieldDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.VoidSoulDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprites.Get("VoidSoulPopup.png"), "Gained Charm"); LocalSaveData.VoidSoulDonePopup = true;
        }

        public override string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}