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
using GlobalEnums;
using ItemChanger.Internal;
using ItemChanger.Modules;
using UnityEngine.SceneManagement;
using static Fyrenest.Fyrenest;

namespace Fyrenest
{
    public class Fyrenest : Mod, IMod, ICustomMenuMod, ILocalSettings<CustomLocalSaveData>, ITogglableMod
    {
        public override string GetVersion() => "3 - Lore Update";

        #region Variable Declarations
        public static int charmSelect = 0;
        public static Fyrenest Loadedinstance { get; set; }
        public static CustomLocalSaveData LocalSaveData { get; set; } = new CustomLocalSaveData();

        /// <summary>
        /// Instances of all classes derived from Room
        /// </summary>
        readonly List<Room> rooms = new();

        readonly PrefabManager PrefabMan = new();
        // The room instance corresponding to the currently loaded scene
        public Room ActiveRoom = null;
        public Room PreviousRoom = null;
        public RoomMirrorer RoomMirrorer = new();
        // If the mod is activated
        public bool Enabled = false;
        public void OnLoadLocal(CustomLocalSaveData s) => LocalSaveData = s;
        public CustomLocalSaveData OnSaveLocal() => LocalSaveData;
        public const int CurrentRevision = 3;

        public int NewCharms = Charms.Count; //STARTS AT 1
        public int OldCharms = 40; //STARTS AT 1

        internal static Fyrenest instance;

        private readonly Dictionary<string, Func<bool, bool>> BoolGetters = new();
        private readonly Dictionary<string, Action<bool>> BoolSetters = new();
        private readonly Dictionary<string, Func<int, int>> IntGetters = new();
        private readonly Dictionary<(string, string), Action<PlayMakerFSM>> FSMEdits = new();
        private readonly List<(int Period, Action Func)> Tickers = new();
        public static Dictionary<string, Dictionary<string, GameObject>> Preloads;
        #endregion

        private readonly static List<Charm> Charms = new()
        {
            Quickfall.instance,
            Quickjump.instance,
            Slowfall.instance,
            Slowjump.instance,
            SturdyNail.instance,
            BetterCDash.instance,
            GlassCannon.instance,
            HKBlessing.instance,
            PowerfulDash.instance,
            Fyrechild.instance,
            OpportunisticDefeat.instance,
            SoulSpeed.instance,
            SoulSpell.instance,
            SoulHunger.instance,
            RavenousSoul.instance,
            SoulSwitch.instance,
            GeoSwitch.instance,
            WealthyAmulet.instance,
            SoulSlow.instance,
            SlowTime.instance,
            SpeedTime.instance,
            ZoteBorn.instance,
            SlyDeal.instance,
            ElderStone.instance,
            GiantNail.instance,
            MatosBlessing.instance,
            HealthyShell.instance,
            BlueBlood.instance,
            ShellShield.instance,
            WyrmForm.instance,
            TripleJump.instance,
            VoidSoul.instance
        };

        public Fyrenest() : base("Fyrenest")
        {

            //Make sure the main menu text is changed, but disable all other functionalitly
            SetEnabled(false);
            Enabled = true;

            //Instantiate all Room subclasses
            foreach (Type type in this.GetType().Assembly.GetTypes())
            {
                if (type.BaseType == typeof(Room))
                {
                    rooms.Add((Room)Activator.CreateInstance(type));
                }
            }
        }

        private void ModHooks_SavegameLoadHook(int obj)
        {
            PlayerData.instance.CalculateNotchesUsed();
        }

        #region LocalSaveData
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
            public bool SlowjumpGot = false;
            public bool QuickjumpGot = false;


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
            public bool QuickjumpDonePopup = false;
            public bool SlowjumpDonePopup = false;

            public int revision = 0;
            public bool FyrenestEnabled = false;
        }
        #endregion

        private void OnLoadSave(SaveGameData obj)
        {
            PlayerData.instance.CalculateNotchesUsed();

            PlayerData.instance.openedMapperShop = true;
            PlayerData.instance.mapAllRooms = true;
            PlayerData.instance.mapAbyss = true;
            PlayerData.instance.mapCity = true;
            PlayerData.instance.mapCliffs = true;
            PlayerData.instance.mapCrossroads = true;
            PlayerData.instance.mapDeepnest = true;
            PlayerData.instance.mapDirtmouth = true;
            PlayerData.instance.mapFogCanyon = true;
            PlayerData.instance.mapFungalWastes = true;
            PlayerData.instance.mapGreenpath = true;
            PlayerData.instance.mapMines = true;
            PlayerData.instance.mapOutskirts = true;
            PlayerData.instance.mapRestingGrounds = true;
            PlayerData.instance.mapRoyalGardens = true;
            PlayerData.instance.mapWaterways = true;
            PlayerData.instance.hasMap = true;
            PlayerData.instance.UpdateGameMap();
        }

        private void OnPause(On.UIManager.orig_UIGoToPauseMenu orig, UIManager self)
        {
            Time.timeScale = 0.0f;
            orig(self);
        }
        private void OnUnPause(On.UIManager.orig_UIClosePauseMenu orig, UIManager self)
        {
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
            if (PlayerData.instance.dreamOrbs >= 2400)
            {
                Room.instance.SetTransition("RestingGrounds_07", "right1", "RestingGrounds_17", "right1");
            }
            //change dreamshield to cost 2 notches.
            PlayerData.instance.charmCost_38 = 2;
            foreach (Charm charm in Charms)
            {
                charm.Settings(Settings).Cost = charm.DefaultCost;
            }
            
            //give charms when certain things are done.
            if (PlayerData.instance.colosseumBronzeCompleted) Quickfall.instance.Settings(Settings).Got = true; LocalSaveData.QuickfallGot = true;
            if (PlayerData.instance.colosseumSilverCompleted) Slowfall.instance.Settings(Settings).Got = true; LocalSaveData.SlowfallGot = true;
            if (PlayerData.instance.hasShadowDash) PowerfulDash.instance.Settings(Settings).Got = true; LocalSaveData.PowerfulDashGot = true;
            if (PlayerData.instance.hasNailArt) SturdyNail.instance.Settings(Settings).Got = true; LocalSaveData.SturdyNailGot = true;
            if (PlayerData.instance.hasDreamGate) SoulHunger.instance.Settings(Settings).Got = true; LocalSaveData.SoulHungerGot = true;
            if (PlayerData.instance.hasDreamNail) SoulSlow.instance.Settings(Settings).Got = true; LocalSaveData.SoulSlowGot = true;
            if (PlayerData.instance.hasSuperDash && PlayerData.instance.gaveSlykey) BetterCDash.instance.Settings(Settings).Got = true; LocalSaveData.BetterCDashGot = true;
            if (PlayerData.instance.killedHollowKnight) HKBlessing.instance.Settings(Settings).Got = true; LocalSaveData.HKBlessingGot = true;
            if (PlayerData.instance.hasKingsBrand) HealthyShell.instance.Settings(Settings).Got = true; LocalSaveData.HealthyShellGot = true;
            if (PlayerData.instance.killedHollowKnightPrime) GlassCannon.instance.Settings(Settings).Got = true; LocalSaveData.GlassCannonGot = true;
            if (PlayerData.instance.bankerAccountPurchased) WealthyAmulet.instance.Settings(Settings).Got = true; LocalSaveData.WealthyAmuletGot = true;
            if (PlayerData.instance.colosseumGoldCompleted) RavenousSoul.instance.Settings(Settings).Got = true; LocalSaveData.RavenousSoulGot = true;
            if (PlayerData.instance.canOvercharm) OpportunisticDefeat.instance.Settings(Settings).Got = true; LocalSaveData.OpportunisticDefeatGot = true;
            if (PlayerData.instance.collectorDefeated) SoulSpell.instance.Settings(Settings).Got = true; LocalSaveData.SoulSpellGot = true;
            if (PlayerData.instance.grubsCollected > 10) SlowTime.instance.Settings(Settings).Got = true; LocalSaveData.SlowTimeGot = true;
            if (PlayerData.instance.statueStateCollector.completedTier2) SpeedTime.instance.Settings(Settings).Got = true; LocalSaveData.SpeedTimeGot = true;
            if (PlayerData.instance.mageLordDreamDefeated) GeoSwitch.instance.Settings(Settings).Got = true; LocalSaveData.GeoSwitchGot = true;
            if (PlayerData.instance.killedMageLord) SoulSwitch.instance.Settings(Settings).Got = true; LocalSaveData.SoulSwitchGot = true;
            if (PlayerData.instance.nailsmithConvoArt) SoulSpeed.instance.Settings(Settings).Got = true; LocalSaveData.SoulSpeedGot = true;
            if (PlayerData.instance.zotePrecept > 56) ZoteBorn.instance.Settings(Settings).Got = true; LocalSaveData.ZoteBornGot = true;
            if (PlayerData.instance.visitedWhitePalace) ElderStone.instance.Settings(Settings).Got = true; LocalSaveData.ElderStoneGot = true;
            if (PlayerData.instance.gaveSlykey && PlayerData.instance.slyConvoNailHoned && PlayerData.instance.completionPercentage > 100) SlyDeal.instance.Settings(Settings).Got = true; LocalSaveData.SlyDealGot = true;
            if (PlayerData.instance.honedNail) GiantNail.instance.Settings(Settings).Got = true; LocalSaveData.GiantNailGot = true;
            if (PlayerData.instance.hasAllNailArts && PlayerData.instance.hasKingsBrand) MatosBlessing.instance.Settings(Settings).Got = true; LocalSaveData.MatosBlessingGot = true;
            if (PlayerData.instance.geo > 100 && PlayerData.instance.hasCityKey) Quickjump.instance.Settings(Settings).Got = true; LocalSaveData.QuickjumpGot = true;
            if (PlayerData.instance.killedJellyfish && PlayerData.instance.killsJellyCrawler > 20) Slowjump.instance.Settings(Settings).Got = true; LocalSaveData.SlowjumpGot = true;

            if (PlayerData.instance.maxHealth < 1)
            {
                HeroController.instance.AddToMaxHealth(1);
            }
            AchievementHelper.AddAchievement("voidsoulachievement", EmbeddedSprite.Get("VoidSoulAchievement.png"), "Soul of Void", "Gain and wear the Void Soul charm.", false);
            

            //change local save data
            if (!LocalSaveData.QuickfallGot && Quickfall.instance.Settings(Settings).Got) LocalSaveData.QuickfallGot = true;
            if (!LocalSaveData.SlowfallGot && Slowfall.instance.Settings(Settings).Got) LocalSaveData.SlowfallGot = true;
            if (!LocalSaveData.SturdyNailGot && SturdyNail.instance.Settings(Settings).Got) LocalSaveData.SturdyNailGot = true;
            if (!LocalSaveData.BetterCDashGot && BetterCDash.instance.Settings(Settings).Got) LocalSaveData.BetterCDashGot = true;
            if (!LocalSaveData.GlassCannonGot && GlassCannon.instance.Settings(Settings).Got) LocalSaveData.GlassCannonGot = true;
            if (!LocalSaveData.HKBlessingGot && HKBlessing.instance.Settings(Settings).Got) LocalSaveData.HKBlessingGot = true;
            if (!LocalSaveData.PowerfulDashGot && PowerfulDash.instance.Settings(Settings).Got) LocalSaveData.PowerfulDashGot = true;
            if (!LocalSaveData.HealthyShellGot && HealthyShell.instance.Settings(Settings).Got) LocalSaveData.HealthyShellGot = true;
            if (!LocalSaveData.OpportunisticDefeatGot && OpportunisticDefeat.instance.Settings(Settings).Got) LocalSaveData.OpportunisticDefeatGot = true;
            if (!LocalSaveData.SoulSpeedGot && SoulSpeed.instance.Settings(Settings).Got) LocalSaveData.SoulSpeedGot = true;
            if (!LocalSaveData.SoulSpellGot && SoulSpell.instance.Settings(Settings).Got) LocalSaveData.SoulSpellGot = true;
            if (!LocalSaveData.SoulHungerGot && SoulHunger.instance.Settings(Settings).Got) LocalSaveData.SoulHungerGot = true;
            if (!LocalSaveData.RavenousSoulGot && RavenousSoul.instance.Settings(Settings).Got) LocalSaveData.RavenousSoulGot = true;
            if (!LocalSaveData.SoulSwitchGot && SoulSwitch.instance.Settings(Settings).Got) LocalSaveData.SoulSwitchGot = true;
            if (!LocalSaveData.GeoSwitchGot && GeoSwitch.instance.Settings(Settings).Got) LocalSaveData.GeoSwitchGot = true;
            if (!LocalSaveData.WealthyAmuletGot && WealthyAmulet.instance.Settings(Settings).Got) LocalSaveData.WealthyAmuletGot = true;
            if (!LocalSaveData.SoulSlowGot && SoulSlow.instance.Settings(Settings).Got) LocalSaveData.SoulSlowGot = true;
            if (!LocalSaveData.SlowTimeGot && SlowTime.instance.Settings(Settings).Got) LocalSaveData.SlowTimeGot = true;
            if (!LocalSaveData.SpeedTimeGot && SpeedTime.instance.Settings(Settings).Got) LocalSaveData.SpeedTimeGot = true;
            if (!LocalSaveData.ZoteBornGot && ZoteBorn.instance.Settings(Settings).Got) LocalSaveData.ZoteBornGot = true;
            if (!LocalSaveData.SlyDealGot && SlyDeal.instance.Settings(Settings).Got) LocalSaveData.SlyDealGot = true;
            if (!LocalSaveData.ElderStoneGot && ElderStone.instance.Settings(Settings).Got) LocalSaveData.ElderStoneGot = true;
            if (!LocalSaveData.GiantNailGot && GiantNail.instance.Settings(Settings).Got) LocalSaveData.GiantNailGot = true;
            if (!LocalSaveData.MatosBlessingGot && MatosBlessing.instance.Settings(Settings).Got) LocalSaveData.MatosBlessingGot = true;
            if (!LocalSaveData.ShellShieldGot && ShellShield.instance.Settings(Settings).Got) LocalSaveData.ShellShieldGot = true;
            if (!LocalSaveData.QuickjumpGot && ShellShield.instance.Settings(Settings).Got) LocalSaveData.QuickjumpGot = true;
            if (!LocalSaveData.SlowjumpGot && ShellShield.instance.Settings(Settings).Got) LocalSaveData.SlowjumpGot = true;

            //GameObject map = GameObject.Find("");
            //if (map != null)
            //{
            //    var assembly = Assembly.GetExecutingAssembly();

            //    using (var stream = assembly.GetManifestResourceStream("Fyrenest.Resources.map.png"))
            //    {
            //        Sprite mapSprite = SpriteManager.Load(stream);
            //        map.GetComponent<SpriteRenderer>().sprite = mapSprite;
            //    }
            //}
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Log("Initiating OnWorldInit()");
                //Call OnWorldInit for all Room subclasses
                foreach (Room room in rooms)
                {
                    room.OnWorldInit();
                    Log("Initialized " + room.RoomName);
                }
                Log("Re-initialized all rooms.");
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                // Toggle if Fyrenest is enabled.
                LocalSaveData.FyrenestEnabled = true;
                Log("Initiating OnWorldInit()");
                //Call OnWorldInit for all Room subclasses
                foreach (Room room in rooms)
                {
                    room.OnWorldInit();
                    Log("Initialized " + room.RoomName);
                }
                Log("Re-initialized all rooms.");
            }
        }

        public bool insanity = false;
        private void OnSave(On.GameManager.orig_SaveGame orig, GameManager self)
        {
            CheckCharmPopup();
            orig(self);
        }

        public bool ToggleButtonInsideMenu => true;

        public string selectedCharm = Charms[charmSelect].ToString().Replace("Fyrenest.", "").Replace(".instance", "");

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
                                        SelectCharm(1); //trigger normal function
                                        Element elem = MenuRef.Find("SelectSpecificCharm");
                                        MenuButton buttonElem = elem as MenuButton;
                                        buttonElem.Name = "Select Specific Charm"; //change name
                                        string SelectedCharm =  Charms[charmSelect].Name.ToString();
                                        string charmNameSelected = SelectedCharm.Replace("Fyrenest.", "").Replace(".instance", "");
                                        string desc = "Current selected charm: "+SelectedCharm; //set desc to the new wanted description
                                        buttonElem.Description = desc; //change description
                                        buttonElem.Update();//Update button
                                        MenuRef.Update();

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
                    new HorizontalOption( "Insanity Mode", "Toggle insanity mode.", new string[]{ "Normal", "INSANITY" }, (setting) => { ModToggle = setting; if(setting == 0) { insanity = false; } else { insanity = true; } }, () => ModToggle),
                    new MenuButton("Back button", "Go back to main page.", (_) => UIManager.instance.UIGoToDynamicMenu(MenuRef.menuScreen)),
                    new TextPanel("Do not press the default back button.", 500, 50, "BackButtonWarning")
                }
            );
            MenuRef.GetMenuScreen(modListMenu);
            ExtraMenuRef.GetMenuScreen(ExtraMenuRef.menuScreen);

            return MenuRef.menuScreen;
        }
        
        private void ReloadCharmEffects()
        {
            GameManager.instance.UpdateBlueHealth();
            GameManager.instance.SaveGame();
            PlayerData.instance.equippedCharms.Clear();
            foreach (var charm in Charms)
            {
                charm.Settings(Settings).Equipped = false;
            }
            HeroController.instance.CharmUpdate();
            PlayerData.instance.CalculateNotchesUsed();
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

        #region LanguageReplacements
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
                return "Test area, please tell me where this is. Ping me @BubkisLord#5187 (discord)";
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
                return "Hi there, it's me! Conifer. Clearly, I have left, but you can come at see me or Pine at dirtmouth!";
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
                return "...Raw energy... ...The troupe...<page>Why?";
            }
            if (key == "CP2" && sheetTitle == "GRIMMSYCOPHANT_DREAM")
            {
                return "...I founded the troupe... ...Destroyed kingdoms... ...For Grimm...";
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
                return "The Adventurer";
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
            if (SlyDeal.instance.Equipped() && sheetTitle == "Prices")
            {
                try
                {
                    float numOrig = float.Parse(orig);
                    numOrig /= 1.5f;
                    orig = numOrig.ToString();
                    return orig;
                }
                catch (Exception)
                {
                    return orig;
                }
            }
            if (ZoteBorn.instance.Equipped() && sheetTitle == "Prices")
            {
                return "10000";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Elderbug")
            {
                return "I don't think I can speak to you right now. There is a disgusting whiff of something coming by...";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Iselda")
            {
                return "Ugh... Please, leave.. The smell!<page>Just buy something. Or don't. Please leave the shop.";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Enemy Dreams")
            {
                return "...That stench... ...disgusting...";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Sly")
            {
                return "I see you are wearing that charm I gave you... Not near me please.";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Cornifer")
            {
                return "You can have a map! Have everything! But get me away from that stench!";
            }
            if (sheetTitle == "Lore Tablets" && key == "WISHING_WELL_INSPECT")
            {
                return "A true follower of the Pale King gives all they own for the kingdom.";
            }
            if (sheetTitle == "Lore Tablets" && key == "TUT_TAB_02")
            {
                return "For those who enter Fyrenest from the far lands of Hallownest and The Glimmering Realm, note this. Beyond this point you enter the land of the light. Step across this threshold and obey our laws. Bear witness to one of the last civilisations, one of the eternal Kingdoms.\n\nFyrenest.";
            }
            if (sheetTitle == "Lore Tablets" && key == "RANDOM_POEM_STUFF")
            {
                return "Fyrenest is great.\nThe very last great kingdom.\nFyrenest rules all.\n                                                 - Haiku by Monomon the Teacher.";
            }
            if (sheetTitle == "Lore Tablets" && key == "RUINS_FOUNTAIN")
            {
                return "Memorial to the Hollow Vessel.\n\nIn its vault far above, stopping the death.\n\nThrough its sacrifice Fyrenest will last eternal.";
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
            if (orig.ToLower().Contains("the world of infected vessel"))
            {
                return orig.Replace("Infected Vessel", "Hollow Knight");
            }
            if (key == "WITCH_REWARD_8A")
            {
                return "Yes. The time has come.<page>The Dream Nail... And you as well, Wielder. It is time for you both to awaken.<page>The Essence you have collected... Finally, I will be able to re-enact the second stage of my plan. Pure potential! Let the power course through you and into the Dream Nail!<page>Hold it aloft, Wielder!<page>AWAKEN!";
            }
            if (key == "WITCH_FINAL_1")
            {
                return "So much Essence... Finally. So bright! I will be able to retake this land.<page>You see, the folk of my tribe were born from a light. Light similar to Essence, similar to that powerful blade, though much brighter still.<page>They were content to bask in that light and honoured it. Worshipped it. For a time...<br>But we lost our way. Forgot our traditions...<page>But another light appeared in our world... A wyrm that took the form of a king. He was born here, from the remanents of Fyrenest's essence and light. Fyrenest's power was forever destroyed, absorbed by a narcissistic king.<page>How fickle my ancestors must have been. They forsook the light that spawned them. Turned their backs to it... Forgot it even.<page>I have rectified their mistakes. I will ascend to a might never seen before. All those champions you slew, all those warriors you killed, I have been harvesting their power. Did you see the prison I set their spirits in? A eternal fighting ring.<page>You have been collecting essence since you came here. Going out and fetching more for me. Now I have enough to ascend. Ascension is my final goal. You see, I am the light! I am the blinding radiance Fyrenest needs! Everyone will worship me for the end of time.<page>No one shall forget me. No one shall lose their way. I will be their god! Their ruler!";
            }
            if (key == "WITCH_FINAL_3")
            {
                return "IT IS HAPPENING!\nI AM ASCENDING!<page>SO BRIGHT!<page>I SEE THE LIGHT! THE RADIANCE!<page>ME.";
            }
            if (key == "WITCH_MEET_A")
            {
                return "Ah, welcome back to the waking world. Those Dreamers. They inhabit every civilisation wherever you go. They are nothing, just ignore them.";
            }
            if (key == "WITCH_REWARD_5")
            {
                return "So, you already have 700 Essence. Take your gift and continue collecting Essence for me.<page>I need more if I want to ascend. Just know that once you have 900, you must come back and visit me.";
            }
            if (key == "WITCH_REWARD_7")
            {
                return "So, you already have 1500 Essence. Soon you will gain a special gift.<page>Continue on your path, Wielder. I know not what guides you, nor what will it is that drives you forward. But know, if that drive disappears, you will pay dearly. Once you have collected 1800 Essence though, I will be here waiting.<page>Take this gift, may it grant you strength and help you to collect the Essence of this world!";
            }
            if (key == "WITCH_REWARD_4")
            {
                return "Ahhhhh. 500 Essence. You're a master in the making. Well done! Well done! I've a small reward for you. Don't let it get in your head though, we still have a long way to go. I need more.<page>Plucked from one of my most precious memories, this Charm will bring you and the Dream Nail closer together still. The secrets of this kingdom won't be able to hide from you any longer!<page>Take it, and return once you have collected 700 Essence. More gifts await you... and me...";
            }
            if (key == "WITCH_GENERIC")
            {
                return "Explore Fyrenest and collect Essence. There are beings of great power that harbor extreme power. Seek them out, reveal them, and gather the light inside...";
            }
            if (key == "WITCH_MEET_B")
            {
                return "Ahhhh, you've found your way back to me. When you awoke you just left. You made a good choice coming back here. I have a use for your services.";
            }
            if (key == "WITCH_MEET_B")
            {
                return "Ahhhh, you've found your way back to me. When you awoke you just left. You made a good choice coming back here. I have a use for your services.";
            }
            if (key == "WITCH_REWARD_8B")
            {
                return "Ahhhh, ah ha ha ha ha, yes...<page>No dream can hide itself from you now. You can peer into the darkest places... You just need to find the right crack.<page>What will you do with such a power, Wielder? Whose memories will you hunt down?<page>Hah. Do as you wish, once my plan is fulfilled. Find the last remaining scraps of Essence. Seek it out. Find it, and bring it to me. I want it all.";
            }
            if (key == "WITCH_FINAL_2")
            {
                return "Finally. I ascend! The time has come!\n\nI WILL RULE ALL OF FYRENEST! NOTHING SHALL STOP ME!<page>I SHALL BE ETERNAL, I SHALL BE THE LIGHT!";
            }
            if (key == "WITCH_INTRO")
            {
                return "Those figures, those Dreamers... they reached out with what little power they still have and dragged you into that hidden place. They feel threatened by you. They were weak.<page>They couldn't do what needed to be done.<br>Let's see if you fare better.<page>Wait, that talisman you now wield, the Dream Nail... it can cut through the veil that separates the waking world from our dreams. Maybe you shall fare better.<page>Though I must admit, that sacred blade has dulled over time. Together perhaps, we can restore its power. You only have to bring me Essence. Yes... That works.<page>Essence... they are precious fragments of light and energy collected from dreams. Collect it wherever you find any, and bring it to me. Once we have enough, we can work wonders together. We will re-create all of Fyrenest!<page>Go out into the world, Wielder. Hunt down the Essence that lingers there!<page>Collect 100 Essence and return to me. I will teach you more... The ways of ascension.";
            }
            //if (key == "WITCH_FINAL_2")
            //{
            //    return "It is time for us to be remembered.<page>It is time for the light to be remembered, to be seen again.";
            //}
            if (key == "HINT_WITCH_DREAMPLANT")
            {
                return "Essence can be found wherever dreams take root.<page>Have you seen them? Those whispering plants that grow all over this old Kingdom? I believe there is one just outside. Why not strike it with your Dream Nail, and see what happens? Collect my Essence.";
            }
            if (key == "WITCH_REWARD_1")
            {
                return "Hmm, already you've collected 100 Essence. Quick work! Things come naturally to you, don't they?<page>No wonder the Dreamers tried to bury you in that old dream. Perhaps being prisoners themselves, they desired your company?<page>In any case, now their prison is better sealed and their Essence is still being harvested. Do not worry about those Dreamers. They won't bother us anymore. Take this old trinket as encouragement from me, and return when you have collected 200 Essence.";
            }
            if (key == "WITCH_REWARD_6")
            {
                return "The Dream Nail glows bright... It holds over 1200 Essence. Looking into it I can see so many memories peering back at me. So many asking to be remembered.<page>None of us can live forever, and so we ask those who survive to remember us.<page>Hold something in your mind and it lives on with you, but forget it and you seal it away forever. That is the only death that matters.<page>Huh, so they say! Enough of that though. Take this relic and come back to me with 1500 Essence. Go, get me more Essence!";
            }
            if (key == "WITCH_REWARD_2A")
            {
                return "Ahhh... Your Dream Nail holds over 200 Essence. You're proving your talent in its collection.<page>Have you seen that great door just outside? My tribe closed it long ago and forbade its opening.<page>But, since I want more essence, I am going to open it. The spirits of strong beings you have killed will be imprisoned there. While imprisoned, I can harvest their energy, giving me even more Essence.<page>You can also visit, and battle them over and over. Every victory gives me more Essence.";
            }
            for (int i = 1; i < 10; i++)
            {
                if (key == "WITCH_QUEST_"+i.ToString())
                {
                    return "You still require more essence before I give you another reward.";
                }
            }
            if (key == "WITCH_REWARD_2B")
            {
                return "There we go! The door to the prison is open! Go ahead and fight them if you want. Just know, the Essence will go straight to me, not your dream nail. And I won't reward you for prowess in combat against them.";
            }
            if (key == "WITCH_QUEST_5B")
            {
                return "My, my, look at you! Once you collect 900 Essence, I will teach you something hidden for a very long time. You are going to like it.";
            }
            if (key == "WITCH_GREET")
            {
                return "Ah, Wielder, you've returned. Let me have a look at the Dream Nail...";
            }
            if (key == "WITCH_REWARD_5B")
            {
                return "The Dream Nail now holds 900 Essence within its core!<page>Yes, you're starting to see them. The connections between us and the dreams we leave behind, like prints in the dust. The time has come for you to learn how to revisit the places connected to you!<page>Hold the Dream Nail tight, wielder, and imagine a great gate opening before you!";
            }
            if (key == "WITCH_HINT_XERO")
            {
                return "Sometimes people can be infused with Essence. Some of the former members of our tribe conducted experiments on themselves. I created an experiment on a willing tribe member, and it turned out exactly the way it was designed. I am the only one who knows how to do an Essence infusion successfully. One day, with enough Essence, I will do it on myself.<page>Although they are bountiful sources of Essence, instead of slaying them we banished them across the kingdom.<page>You should search carefully near graves and other monuments. Why, I believe I saw an interesting gravestone here in the Resting Grounds.<page>If you do decide to disturb those dreams though, be prepared for a fight... Their spirits are easily angered.";
            }
            if (key == "WITCH_DREAM1")
            {
                return "What a terrible fate they've visited upon you.<page>To cast you away into this space between body and soul.<page>Will you accept their judgement and fade slowly away?<page>Or will you take the weapon before you, and cut your way out of this sad, forgotten dream?";
            }
            if (key == "WITCH_DREAM_FALL")
            {
                return "Though you may fall, your will shall carry you forward.<page>A dream is endless, but a Kingdom is not.<page>The power to wake this world from its slumber... Only worshipping the light will restore that.";
            }
            if (key == "WITCH_DREAM")
            {
                return "Once I get the Essence, I will be unstoppable. I will re-create Fyrenest in my own image. People will worship the light once more. Wait. I sense...<br>Get out of my mind. I trusted you with a powerful weapon such as this, and you disobey me? GET OUT!";
            }
            return orig;
        }
        #endregion

        #region Random Useless Stuff 
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
            SoulSlow.instance.Settings(Settings).Equipped = true;
            SoulSlow.instance.Settings(Settings).Equipped = false;
        }

        public void TakeMaxHealth()
        {
            HeroController.instance.AddToMaxHealth(-1);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.instance.Settings(Settings).Equipped = true;
            SoulSlow.instance.Settings(Settings).Equipped = false;
        }
        public void MaxHealthReset()
        {
            var CurrentMaxHp = PlayerData.instance.maxHealth;
            var WantedMaxHp = 5;
            var MaxHpChangeAmount = WantedMaxHp - CurrentMaxHp;
            HeroController.instance.AddToMaxHealth(MaxHpChangeAmount);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.instance.Settings(Fyrenest.instance.Settings).Equipped = true;
            SoulSlow.instance.Settings(Fyrenest.instance.Settings).Equipped = false;
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
            ShellShield.instance.Trigger(boolName, value);
            if (BoolSetters.TryGetValue(boolName, out var f))
            {
                f(value);
            }
            return value;
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
        public Sprite SpriteGet(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Fyrenest.Resources." + fileName);
            Sprite sprite = SpriteManager.Load(stream);
            return sprite;
        }
        private void CheckCharmPopup()
        {
            if (PlayerData.instance.colosseumBronzeCompleted && !LocalSaveData.QuickfallDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("Quickfall.png"), "Gained Charm"); LocalSaveData.QuickfallDonePopup = true;
            if (PlayerData.instance.colosseumSilverCompleted && !LocalSaveData.SlowfallDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("Slowfall.png"), "Gained Charm"); LocalSaveData.SlowfallDonePopup = true;
            if (PlayerData.instance.hasShadowDash && !LocalSaveData.PowerfulDashDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("PowerfulDash.png"), "Gained Charm"); LocalSaveData.PowerfulDashDonePopup = true;
            if (PlayerData.instance.hasNailArt && !LocalSaveData.SturdyNailDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SturdyNail.png"), "Gained Charm"); LocalSaveData.SturdyNailDonePopup = true;
            if (PlayerData.instance.statueStateMantisLordsExtra.isUnlocked && !LocalSaveData.MarkofStrengthDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("MarkofStrength.png"), "Gained Charm"); LocalSaveData.MarkofStrengthDonePopup = true;
            if (PlayerData.instance.hasDreamGate && !LocalSaveData.SoulHungerDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SoulHunger.png"), "Gained Charm"); LocalSaveData.SoulHungerDonePopup = true;
            if (PlayerData.instance.hasDreamNail && !LocalSaveData.SoulSlowDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SoulSlow.png"), "Gained Charm"); LocalSaveData.SoulSlowDonePopup = true;
            if (PlayerData.instance.hasSuperDash && PlayerData.instance.gaveSlykey && !LocalSaveData.BetterCDashDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("BetterCDash.png"), "Gained Charm"); LocalSaveData.BetterCDashDonePopup = true;
            if (PlayerData.instance.killedHollowKnight && !LocalSaveData.HKBlessingDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("HKBlessing.png"), "Gained Charm"); LocalSaveData.HKBlessingDonePopup = true;
            if (PlayerData.instance.hasKingsBrand && !LocalSaveData.HealthyShellDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("HealthyShell.png"), "Gained Charm"); LocalSaveData.HealthyShellDonePopup = true;
            if (PlayerData.instance.killedHollowKnightPrime && !LocalSaveData.GlassCannonDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("GlassCannon.png"), "Gained Charm"); LocalSaveData.GlassCannonDonePopup = true;
            if (PlayerData.instance.bankerAccountPurchased && !LocalSaveData.WealthyAmuletDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("WealthyAmulet.png"), "Gained Charm"); LocalSaveData.WealthyAmuletDonePopup = true;
            if (PlayerData.instance.colosseumGoldCompleted && !LocalSaveData.RavenousSoulDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("RavenousSoul.png"), "Gained Charm"); LocalSaveData.RavenousSoulDonePopup = true;
            if (PlayerData.instance.canOvercharm && !LocalSaveData.OpportunisticDefeatDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("OpportunisticDefeat.png"), "Gained Charm"); LocalSaveData.OpportunisticDefeatDonePopup = true;
            if (PlayerData.instance.collectorDefeated && !LocalSaveData.SoulSpellDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SoulSpell.png"), "Gained Charm"); LocalSaveData.SoulSpellDonePopup = true;
            if (PlayerData.instance.grubsCollected > 10 && !LocalSaveData.SlowTimeDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SlowTime.png"), "Gained Charm"); LocalSaveData.SlowTimeDonePopup = true;
            if (PlayerData.instance.statueStateCollector.completedTier2 && !LocalSaveData.SpeedTimeDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SpeedTime.png"), "Gained Charm"); LocalSaveData.SpeedTimeDonePopup = true;
            if (PlayerData.instance.mageLordDreamDefeated && !LocalSaveData.GeoSwitchDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("GeoSwitch.png"), "Gained Charm"); LocalSaveData.GeoSwitchDonePopup = true;
            if (PlayerData.instance.killedMageLord && !LocalSaveData.SoulSwitchDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SoulSwitch.png"), "Gained Charm"); LocalSaveData.SoulSwitchDonePopup = true;
            if (PlayerData.instance.nailsmithConvoArt && !LocalSaveData.SoulSpeedDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SoulSpeed.png"), "Gained Charm"); LocalSaveData.SoulSpeedDonePopup = true;
            if (PlayerData.instance.zotePrecept > 56 && !LocalSaveData.ZoteBornDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("ZoteBorn.png"), "Gained Charm"); LocalSaveData.ZoteBornDonePopup = true;
            if (PlayerData.instance.visitedWhitePalace && !LocalSaveData.ElderStoneDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("Elderstone.png"), "Gained Charm"); LocalSaveData.ElderStoneDonePopup = true;
            if (PlayerData.instance.gaveSlykey && PlayerData.instance.slyConvoNailHoned && PlayerData.instance.completionPercentage > 100 && !LocalSaveData.SlyDealDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("SlyDeal.png"), "Gained Charm"); LocalSaveData.SlyDealDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.GiantNailDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("GiantNail.png"), "Gained Charm"); LocalSaveData.GiantNailDonePopup = true;
            if (PlayerData.instance.hasAllNailArts && PlayerData.instance.hasKingsBrand && !LocalSaveData.MatosBlessingDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("MatosBlessing.png"), "Gained Charm"); LocalSaveData.MatosBlessingDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.ShellShieldDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("ShellShield.png"), "Gained Charm"); LocalSaveData.ShellShieldDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.VoidSoulDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("VoidSoulPopup.png"), "Gained Charm"); LocalSaveData.VoidSoulDonePopup = true;

            // make it buy from salubra
            if (PlayerData.instance.geo > 100 && PlayerData.instance.hasCityKey && !LocalSaveData.QuickjumpDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("Quickjump.png"), "Gained Charm"); LocalSaveData.QuickjumpDonePopup = true;
            if (PlayerData.instance.killedJellyfish && PlayerData.instance.killsJellyCrawler > 20 && !LocalSaveData.SlowjumpDonePopup) ItemChanger.Internal.MessageController.Enqueue(EmbeddedSprite.Get("Slowjump.png"), "Gained Charm"); LocalSaveData.SlowjumpDonePopup = true;
        }
        #endregion

        public void Unload()
        {
            foreach (Charm charm in Charms)
            {
                charm.Settings(Settings).Got = false;
                charm.Settings(Settings).Equipped = false;
            }
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
            //GameCompletion.Enabled = enabled;
            if (!enabled)
            {
                ActiveRoom = null;
            }
        }

        public override List<(string, string)> GetPreloadNames()
        {
            return PrefabMan.GetPreloadNames();
        }


        /// <summary>
        /// Hook for a new file starting, sets up ItemChanger 
        /// </summary>
        public void InitializeWorld(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            if (Enabled)
            {
                Log("Initializing World");

                //initialize IC
                ItemChangerMod.CreateSettingsProfile(false, false);
                ItemChangerMod.Modules.GetOrAdd<TransitionFixes>();
                ItemChangerMod.Modules.GetOrAdd<MenderbugUnlock>();
                ItemChangerMod.Modules.GetOrAdd<ElevatorPass>();

                //save that this is a Fyrenest save file
                LocalSaveData.FyrenestEnabled = true;
                LocalSaveData.revision = CurrentRevision;

                //Call OnWorldInit for all Room subclasses
                foreach (Room room in rooms)
                {
                    room.OnWorldInit();
                    Log("Initialized " + room.RoomName);
                }
                Log("World Initialized");
            }

            orig(self, permaDeath, bossRush);
        }

        /// <summary>
        /// Correct erroneous 7 grub reward from Grubfather
        /// </summary>
        public void CorrectGrubfather()
        {
            if (!LocalSaveData.FyrenestEnabled) return;

            Settings set = (Settings)typeof(ItemChangerMod).GetField("SET", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            foreach (var placement in set.GetPlacements())
            {
                if (placement.Name == LocationNames.Grubfather)
                {
                    foreach (var item in placement.Items)
                    {
                        if (item.name == ItemNames.Pale_Ore)
                        {
                            item.GetTag<CostTag>().Cost = new PDIntCost(6, nameof(PlayerData.grubsCollected), 6 + " Grubs");
                        }
                    }
                }
            }
        }

        public void OnSaveLoad()
        {
            if (!LocalSaveData.FyrenestEnabled) return;

            if (LocalSaveData.revision < CurrentRevision)
            {
                RevisionManager.OnRevision(LocalSaveData.revision, CurrentRevision);

                //apply changes from new revisions if old revisions are loaded
                foreach (Room room in rooms)
                {
                    if (room.Revision > LocalSaveData.revision)
                    {
                        room.OnWorldInit();
                    }
                }
            }
            LocalSaveData.revision = CurrentRevision;
        }


        /// <summary>
        /// Called when the mod is loaded
        /// </summary>
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing Mod.\nInitializing Part 1...");

            instance = this;

            //set up hooks
            On.GameManager.OnNextLevelReady += OnSceneLoad;
            On.UIManager.StartNewGame += InitializeWorld;
            On.HeroController.TakeDamage += OnDamage;

            On.MenuStyleTitle.SetTitle += OnMainMenu;

            On.HeroController.Start += OnGameStart;

            On.GrimmEnemyRange.GetTarget += DisableGrimmchildShooting;

            //Events.OnEnterGame += CorrectGrubfather;
            Events.OnEnterGame += OnSaveLoad;

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnBeforeSceneLoad;

            ModHooks.CharmUpdateHook += OnCharmUpdate;
            ModHooks.GetPlayerBoolHook += ReadCharmBools;
            ModHooks.SetPlayerBoolHook += WriteCharmBools;
            ModHooks.GetPlayerIntHook += ReadCharmCosts;
            // This will run after Rando has already set up its item placements.
            On.PlayMakerFSM.OnEnable += EditFSMs;
            On.PlayerData.CountCharms += CountOurCharms;
            ModHooks.AfterSavegameLoadHook += OnLoadSave;
            ModHooks.HeroUpdateHook += OnUpdate;
            ModHooks.LanguageGetHook += LanguageGet;
            On.UIManager.UIGoToPauseMenu += OnPause;
            On.UIManager.UIClosePauseMenu += OnUnPause;
            On.GameManager.SaveGame += OnSave;
            ModHooks.SavegameLoadHook += ModHooks_SavegameLoadHook;
            //intialize the Prefabs
            PrefabMan.InitializePrefabs(preloadedObjects);


            RoomMirrorer.Hook();

            //GameCompletion.Hook();

            FyrenestModeMenu.Register();

            //Call OnInit for all Room subclasses
            foreach (Room room in rooms)
            {
                room.OnInit();
                Log("Initialized " + room.RoomName);
            }

            //load general text changes
            //GeneralChanges.ChangeText();

            Log("Initialization Part 1 Complete.");
            if (Fyrenest.Loadedinstance != null) return;
            Fyrenest.Loadedinstance = this;

            Preloads = preloadedObjects;

            //On.HeroController.Awake += delegate (On.HeroController.orig_Awake orig, HeroController self) {
            //    orig.Invoke(self);

            //    foreach (IAbility ability in Abilities)
            //    {
            //        Log($"Loading ability {ability.Name}!");
            //        ability.Load();
            //    }
            //};
            Log("Initializing Part 2...");

            foreach (var charm in Charms)
            {
                var num = CharmHelper.AddSprites(EmbeddedSprite.Get(charm.Sprite))[0];
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

            StartTicking();

            if (ModHooks.GetMod("DebugMod") != null)
            {
                DebugModHook.GiveAllCharms(() =>
                {
                    GrantAllOurCharms();
                    PlayerData.instance.CountCharms();
                });
            }
            Log("Initializing Part 2 Complete.\n\nAll Initializing Complete.");
        }

        private void OnCharmUpdate(PlayerData data, HeroController controller)
        {
            if (VoidSoul.instance.Equipped() && VoidSoul.instance.Settings(Settings).Got) MessageController.Enqueue(EmbeddedSprite.Get("VoidSoulAchievement.png"), "Unlocked Achievement"); GameManager.instance.AwardAchievement("voidsoulachievement");
        }

        /// <summary>
        /// Called before every scene load, locks a bunch of events to certain states
        /// </summary>
        public static void OnSceneLoad()
        {
            Fyrenest.instance.RoomMirrorer.UpdateFlipping();
            
            //fix bug when diving into direction changing transition
            string entryGate = GameManager.instance.entryGateName;
            if (entryGate.StartsWith("left") || entryGate.StartsWith("right")) HeroController.instance.exitedQuake = false;
            if (entryGate.StartsWith("top") || entryGate.StartsWith("bot")) HeroController.instance.exitedSuperDashing = false;
            
            //fix playerdata bools
            if (HeroController.instance != null)
            {
                SetBool("crossroadsInfected", false); //uninfect crossroads
                SetBool("marmOutside", false); //Put Lemm inside
                SetBool("corn_crossroadsLeft", false); //make Cornifer stay in all the places forever
                SetBool("corn_greenpathLeft", false);
                SetBool("corn_fogCanyonLeft", false);
                SetBool("corn_fungalWastesLeft", false);
                SetBool("corn_cityLeft", false);
                SetBool("corn_waterwaysLeft", false);
                SetBool("corn_minesLeft", false);
                SetBool("corn_cliffsLeft", false);
                SetBool("corn_deepnestLeft", false);
                SetBool("corn_outskirtsLeft", false);
                SetBool("corn_royalGardensLeft", false);
                SetBool("corn_abyssLeft", false);
                SetBool("corniferAtHome", false);
                SetBool("city2_sewerDoor", true); //open Emilitia entrance
                SetBool("bathHouseWall", true); //break wall in right city elevator
                SetBool("whitePalaceMidWarp", false); //so the TFK dream warp works
                //SetBool("quirrelLeftEggTemple", true); //despawn quirrel in TBE


                // TEST IF COMMAND WORKS, AND IF SO, THEN DO MANTIS TALK
                ReplaceText("DESC_MANTIS_LORD", "Leaders of the Mantis tribe and its finest warriors. In legend, they were said to fight a god-like being of immense power, and its misguided followers. One among them was a mysterious bug who hides away somewhere in Fyrenest, who is now the last of the followers.");
            }
        }

        /// <summary>
        /// Adds a replacement to the TextChanger
        /// </summary>
        static void ReplaceText(string key, string text, string sheet = "")
        {
            Fyrenest.instance.AddReplacement(key, text, sheetKey: sheet);
        }

        /// <summary>
        /// All changed texts
        /// </summary>
        List<TextReplacement> texts = new();

        /// <summary>
        /// Add a new text change
        /// </summary>
        public void AddReplacement(string key, string text, string sheetKey = "")
        {
            texts.Add(new TextReplacement(key, text, sheetKey));
        }

        /// <summary>
        /// Sets a save value in PlayerData
        /// </summary>
        static void SetBool(string key, bool val)
        {
            HeroController.instance.playerData.SetBool(key, val);
        }

        public GameObject DisableGrimmchildShooting(On.GrimmEnemyRange.orig_GetTarget orig, global::GrimmEnemyRange self)
        {
            if (!Enabled) return orig(self);
            return null;
        }

        /// <summary>
        /// Called when a file is started, ensures the mod gets enabled/disabled based on if the file is a Glimmering Realm save
        /// </summary>
        public void OnGameStart(On.HeroController.orig_Start orig, global::HeroController self)
        {
            orig(self);


            //Make sure Fyrenest is only active in the correct worlds
            if (LocalSaveData.FyrenestEnabled)
            {
                SetEnabled(true);

                //workaround if HoGE messes with the PV fight
                if (ModHooks.GetMod("Hall of Gods Extras") is Mod)
                {
                    IMod hog = ModHooks.GetMod("Hall of Gods Extras");
                    object settings = hog.GetType().GetField("_localSettings", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                    settings.GetType().GetField("_statueStateHollowKnight", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(settings, new BossStatue.Completion { hasBeenSeen = true, isUnlocked = true, usingAltVersion = true });
                }
            }
            else
            {
                SetEnabled(false);
            }
        }

        /// <summary>
        /// Called when the player takes damage, enforces the MinDamage feature of the Room class
        /// </summary>
        public void OnDamage(On.HeroController.orig_TakeDamage orig, global::HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            if (Enabled && ActiveRoom != null)
            {
                if (damageAmount < ActiveRoom.MinDamage && damageAmount > 0)
                {
                    damageAmount = ActiveRoom.MinDamage;
                }

                if (damageAmount > ActiveRoom.MaxDamage)
                {
                    damageAmount = ActiveRoom.MaxDamage;
                }
            }
            orig(self, go, damageSide, damageAmount, hazardType);
        }

        /// <summary>
        /// Called before any "Start" functions in the new scene are executed
        /// </summary>
        public void OnBeforeSceneLoad(Scene current, Scene next)
        {
            if (!Enabled) return;

            string scene = next.name;
            ActiveRoom = null;

            //notify every module that needs it
            RoomMirrorer.BeforeSceneLoad();

            PreviousRoom = ActiveRoom;

            //find and set the active room (if there is one)
            foreach (Room room in rooms)
            {
                if (room.RoomName == scene)
                {
                    ActiveRoom = room;
                    room.OnBeforeLoad();
                }
            }

            //update whether the map should be flipped
            RoomMirrorer.UpdateFlipping();

        }

        /// <summary>
        /// Called after the "Start" functions of a newly loaded scene have executed
        /// </summary>
        public void OnSceneLoad(On.GameManager.orig_OnNextLevelReady orig, global::GameManager self)
        {
            orig(self);

            if (!Enabled) return;

            if (HeroController.instance != null)
            {
                HeroController.instance.cState.inConveyorZone = false;
                HeroController.instance.cState.onConveyor = false;
                HeroController.instance.cState.onConveyorV = false;
            }

            //call the OnLoad functions of the current room
            string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Log("Loading " + scene);
            foreach (Room room in rooms)
            {
                if (room.RoomName == scene)
                {
                    room.OnLoad();
                }
            }
        }

        /// <summary>
        /// Called when the main menu is opened, replaces the Hollow Knight Logo with the Fyrenest Logo
        /// </summary>
        public void OnMainMenu(On.MenuStyleTitle.orig_SetTitle orig, global::MenuStyleTitle self, int index)
        {
            orig(self, index);

            //disable everything except text changer
            SetEnabled(false);

            GameObject title = GameObject.Find("LogoTitle");
            if (title != null)
            {
                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream("Fyrenest.Resources.title.png");
                Sprite titleSprite = SpriteManager.Load(stream);
                title.GetComponent<SpriteRenderer>().sprite = titleSprite;
                //slightly blue, to make it stand apart from the background
                title.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(193 / 255f, 225 / 255f, 253 / 255f);
                title.transform.SetScaleMatching(3.2f);
            }
        }
    }

    /// <summary>
    /// Identifies a changed text
    /// </summary>
    class TextReplacement
    {
        public string Key;
        public string SheetKey;
        public string Text;

        public TextReplacement(string key, string text, string sheetKey = "")
        {
            Key = key;
            Text = text;
            SheetKey = sheetKey;
        }
    }
}