global using System;
global using System.IO;
global using System.Collections;
global using Modding;
global using UnityEngine;
global using SFCore;
global using System.Collections.Generic;
global using System.Linq;
global using ItemChanger;
global using ItemChanger.Tags;
global using ItemChanger.UIDefs;
global using Satchel.BetterMenus;
global using GlobalEnums;
global using ItemChanger.Internal;
global using ItemChanger.Modules;
global using UnityEngine.SceneManagement;
global using System.Net;
global using HutongGames.PlayMaker;
global using ItemChanger.Locations;
global using ItemChanger.Placements;
global using UnityEngine.UIElements;
global using RandomizerMod.Extensions;
global using RandomizerMod.IC;
global using RandomizerMod.Menu;
global using RandomizerMod.Settings;
global using RandomizerMod;
global using RandomizerMod.RandomizerData;
global using RandomizerCore;
global using static Fyrenest.Fyrenest;

namespace Fyrenest
{
    public class Fyrenest : Mod, IMod, ICustomMenuMod, ILocalSettings<CustomLocalSaveData>, ITogglableMod
    {
        /// <summary>
        /// Gets the version of the mod
        /// </summary>
        public override string GetVersion() => "3.1 - Transition Update";

        #region Variable Declarations

        /// <summary>
        /// The number of the selected charm (for the mod menu).
        /// </summary>
        public static int charmSelect = 0;
        public static Fyrenest Loadedinstance { get; set; }

        /// <summary>
        /// The data for the loaded save
        /// </summary>
        public static CustomLocalSaveData LocalSaveData { get; set; } = new CustomLocalSaveData();
        
        /// <summary>
        /// Globally saved data - runs across all saves
        /// </summary>
        public static CustomGlobalSaveData GlobalSaveData { get; set; } = new CustomGlobalSaveData();

        /// <summary>
        /// Instances of all classes derived from Room
        /// </summary>
        readonly List<Room> rooms = new();

        readonly PrefabManager PrefabMan = new();
        /// <summary>
        /// The room instance corresponding to the currently loaded scene.
        /// </summary>
        public Room ActiveRoom = null;
        /// <summary>
        /// The room that the player was previously in.
        /// </summary>
        public Room PreviousRoom = null;

        public RoomMirrorer RoomMirrorer = new();

        /// <summary>
        /// If the mod is activated
        /// </summary>
        public bool Enabled = false;
        public void OnLoadLocal(CustomLocalSaveData s) => LocalSaveData = s;
        public void OnLoadGlobal(CustomGlobalSaveData gs) => GlobalSaveData = gs;

        public CustomLocalSaveData OnSaveLocal() => LocalSaveData;
        public CustomGlobalSaveData OnSaveGlobal() => GlobalSaveData;
        public const int CurrentRevision = 3;

        /// <summary>
        /// Amount of new charms made by the mod.
        /// </summary>
        public int NewCharms = Charms.Count; //STARTS AT 1 FOR SOME REASON

        /// <summary>
        /// Amount of original charms.
        /// </summary>
        public int OldCharms = 40; //STARTS AT 1 FOR SOME REASON

        internal static Fyrenest instance;

        private readonly Dictionary<string, Func<bool, bool>> BoolGetters = new();
        private readonly Dictionary<string, Action<bool>> BoolSetters = new();
        private readonly Dictionary<string, Func<int, int>> IntGetters = new();
        private readonly Dictionary<(string, string), Action<PlayMakerFSM>> FSMEdits = new();
        private readonly List<(int Period, Action Func)> Tickers = new();
        public static Dictionary<string, Dictionary<string, GameObject>> Preloads;
        #endregion

        /// <summary>
        /// A list of all added charms.
        /// </summary>
        private readonly static List<Charm> Charms = new()
        {
            SturdyNail.instance,
            BetterCDash.instance,
            GlassCannon.instance,
            HKBlessing.instance,
            PowerfulDash.instance,
            //Fyrechild.instance,
            OpportunisticDefeat.instance,
            SoulSpeed.instance,
            SoulHunger.instance,
            RavenousSoul.instance,
            SoulSpell.instance,
            SoulSwitch.instance,
            GeoSwitch.instance,
            WealthyAmulet.instance,
            SoulSlow.instance,
            SlowTime.instance,
            SpeedTime.instance,
            ZoteBorn.instance,
            GravityCharm.instance,
            BulbousInfection.instance,
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



        /// <summary>
        /// Called when the mod is loaded
        /// </summary>
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing Mod.\nInitializing Part 1...");

            Log("Adding Achievements...");
            AchievementHelper.AddAchievement("voidSoulAchievement", EmbeddedSprite.Get("VoidSoulAchievement.png"), "Soul of Void", "Gain and wear the Void Soul charm.", false);
            AchievementHelper.AddAchievement("allCharmsGained", EmbeddedSprite.Get("AllCharms.png"), "Charmed Vessel", "Gain all charms in Fyrenest", false);
            AchievementHelper.AddAchievement("completedVessel", EmbeddedSprite.Get("CompletedVessel.png"), "Completed Vessel", "Gain all charms in the the whole game.", true);
            if (GlobalSaveData.ALLCharmsGainedAchGot) GameManager.instance.AwardAchievement("completedVessel");
            if (GlobalSaveData.allCharmsGainedAchGot) GameManager.instance.AwardAchievement("allCharmsGained");
            if (GlobalSaveData.voidsoulachievementAchGot) GameManager.instance.AwardAchievement("voidSoulAchievement");
            Log("Successfully Added Achievements.");

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

            ModHooks.LanguageGetHook += GetCharmStrings;
            ModHooks.GetPlayerBoolHook += ReadCharmBools;
            ModHooks.SetPlayerBoolHook += WriteCharmBools;
            ModHooks.GetPlayerIntHook += ReadCharmCosts;
            // This will run after Rando has already set up its item placements.
            On.PlayMakerFSM.OnEnable += EditFSMs;
            On.PlayerData.CountCharms += CountOurCharms;
            ModHooks.AfterSavegameLoadHook += OnLoadSave;
            ModHooks.HeroUpdateHook += OnUpdate;
            On.GameManager.SaveGame += OnSave;
            ModHooks.SavegameLoadHook += ModHooks_SavegameLoadHook;
            On.PlayerData.CountGameCompletion += SetGameCompletion;
            Events.OnEnterGame += GiveStartingItemsAndSetEnabled;

            //intialize the Prefabs
            PrefabMan.InitializePrefabs(preloadedObjects);


            RoomMirrorer.Hook();
            TextReplacements.instance.Hook();

            FyrenestModeMenu.Register();

            //Call OnInit for all Room subclasses
            foreach (Room room in rooms)
            {
                room.OnInit();
                Log("Initialized " + room.RoomName);
            }

            Log("Initialization Part 1 Complete.");
            if (Fyrenest.Loadedinstance != null) return;
            Fyrenest.Loadedinstance = this;

            Preloads = preloadedObjects;

            // Keep this for now, BUT DO NOT UNCOMMENT!!!
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
        
        public static void PlaceAllCharms()
        {
            var placements = new List<AbstractPlacement>();
            foreach (Charm charm in Charms)
            {
                var name = charm.Name.Replace(" ", "_");
                var placement = new CoordinateLocation() { x = charm.X, y = charm.Y, elevation = 0, sceneName = charm.Scene, name = name }.Wrap() as MutablePlacement;
                placement.Add(charm);
            }
            ItemChangerMod.AddPlacements(placements, conflictResolution: PlacementConflictResolution.Ignore);
        }

        //private static void TestPlacements()
        //{
        //    ItemChangerMod.AddPlacements(new List<AbstractPlacement>()
        //    {
        //        AddCharmPlacement(VoidSoul.instance),
        //        AddCharmPlacementExtended(TripleJump.instance, "GG_Atrium", 155.6f, 61.4f)
        //    }, conflictResolution: PlacementConflictResolution.Ignore);
        //}

        //private static MutablePlacement AddCharmPlacement(Charm charm)
        //{
        //    var repairPlacement = new CoordinateLocation() { x = charm.X, y = charm.Y, elevation = 0, sceneName = charm.Scene, name = charm.Name }.Wrap() as MutablePlacement;
        //    for (int i = 0; i < Charms.Count; i++)
        //    {
        //        if (Charms[i].Name == charm.Name) repairPlacement.Add(Charms[i]);
        //    }
        //    return repairPlacement;
        //}

        //private static MutablePlacement AddCharmPlacementExtended(Charm charm, string Scene, float x, float y)
        //{
        //    var repairPlacement = new CoordinateLocation() { x = x, y = y, elevation = 0, sceneName = Scene, name = charm.Name }.Wrap() as MutablePlacement;
        //    for (int i = 0; i < Charms.Count; i++)
        //    {
        //        if (Charms[i].Name == charm.Name) repairPlacement.Add(Charms[i]);
        //    }
        //    return repairPlacement;
        //}

        private void GiveStartingItemsAndSetEnabled()
        {
            if (LocalSaveData.FyrenestEnabled) Enabled = true;
            if (Enabled) LocalSaveData.FyrenestEnabled = true;
        }

        private void SetGameCompletion(On.PlayerData.orig_CountGameCompletion orig, global::PlayerData self)
        {
            orig(self);

            if (!Enabled) return;

            float Completion = 0;

            // 35% Max
            Completion += Charms.Count(c => c.Settings(Settings).Got);

            // 36% Max
            if (PlayerData.instance.metIselda) Completion++;

            // 37% Max
            if (PlayerData.instance.iseldaConvoGrimm) Completion++;

            // 42% Max
            if (PlayerData.instance.elderbugGaveFlower) Completion += 5;

            // 43% Max
            if (PlayerData.instance.tisoDead) Completion++;

            // 45% Max
            if (PlayerData.instance.dreamNailUpgraded) Completion += 2;

            // 69% Max
            Completion += Mathf.Clamp(PlayerData.instance.dreamOrbs, 0, 2400) / 100;

            // 79% Max
            Completion += Mathf.Clamp(PlayerData.instance.rancidEggs, 0, 10);

            // 83% Max
            Completion += Mathf.Clamp(PlayerData.instance.royalCharmState, 0, 4);

            // 84% Max
            if (PlayerData.instance.salubraBlessing) Completion++;

            // 93% Max
            if (PlayerData.instance.notchFogCanyon) Completion++;
            if (PlayerData.instance.notchShroomOgres) Completion++;
            if (PlayerData.instance.gotGrimmNotch) Completion++;
            if (PlayerData.instance.salubraNotch1) Completion++;
            if (PlayerData.instance.salubraNotch2) Completion++;
            if (PlayerData.instance.salubraNotch3) Completion++;
            if (PlayerData.instance.salubraNotch4) Completion++;
            if (PlayerData.instance.slyNotch1) Completion++;
            if (PlayerData.instance.slyNotch2) Completion++;

            // 97% Max
            Completion += Mathf.Clamp(PlayerData.instance.maxHealth, 5, 9) - 5;

            // 100% Max
            if (PlayerData.instance.metBanker) Completion++;
            if (PlayerData.instance.metCornifer) Completion++;
            if (PlayerData.instance.metGiraffe) Completion++;

            Completion = Mathf.Clamp(Completion, 0, 100);

            self.completionPercentage = Completion;
        }

        private void ModHooks_SavegameLoadHook(int obj)
        {
            PlayerData.instance.CalculateNotchesUsed();
        }

        #region SaveData
        public class CustomGlobalSaveData
        {
            public bool allCharmsGainedAchGot = false;
            public bool voidsoulachievementAchGot = false;
            public bool ALLCharmsGainedAchGot = false;
        }

        // The local data to store that is specific to saves.
        public class CustomLocalSaveData
        {
            // What charms the player has in the specific save.
            public bool SturdyNailGot = false;
            public bool BetterCDashGot = false;
            public bool GlassCannonGot = false;
            public bool HKBlessingGot = false;
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
            public bool TripleJumpGot = false;
            public bool GravityCharmGot = false;
            public bool BulbousInfectionGot = false;
            public bool WyrmFormGot = false;
            public bool FyreChildGot = false;


            public bool QuickfallDonePopup = false;
            public bool SlowfallDonePopup = false;
            public bool SturdyNailDonePopup = false;
            public bool BetterCDashDonePopup = false;
            public bool GlassCannonDonePopup = false;
            public bool HKBlessingDonePopup = false;
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
            public bool TripleJumpDonePopup = false;
            public bool GravityCharmDonePopup = false;
            public bool BulbousInfectionDonePopup = false;
            public bool WyrmFormDonePopup = false;
            public bool FyreChildDonePopup = false;

            public int revision = 0;
            public bool FyrenestEnabled = false;

            public bool allCharmsGainedShown = false;
            public bool ALLCharmsGainedShown = false;
            public bool voidsoulachievementShown = false;
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
            //change dreamshield to cost 2 notches.
            PlayerData.instance.charmCost_38 = 2;
            foreach (Charm charm in Charms)
            {
                charm.Settings(Settings).Cost = charm.DefaultCost;
            }

            #region Ugly block of code (do not look at it, it is GROSS AS)
            //give charms when certain things are done.
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
            if (!LocalSaveData.GravityCharmGot && GravityCharm.instance.Settings(Settings).Got) LocalSaveData.GravityCharmGot = true;
            if (!LocalSaveData.BulbousInfectionGot && BulbousInfection.instance.Settings(Settings).Got) LocalSaveData.BulbousInfectionGot = true;
            //if (!LocalSaveData.FyreChildGot && Fyrechild.instance.Settings(Settings).Got) LocalSaveData.FyreChildGot = true;
            if (!LocalSaveData.WyrmFormGot && WyrmForm.instance.Settings(Settings).Got) LocalSaveData.WyrmFormGot = true;
            if (!LocalSaveData.VoidSoulGot && VoidSoul.instance.Settings(Settings).Got) LocalSaveData.WyrmFormGot = true;
            #endregion

            #region More dumb code stuff
            if (PlayerData.instance.maxHealth < 1)
            {
                HeroController.instance.AddToMaxHealth(1);
            }
            if (CheckIfGotAllCharms() && !LocalSaveData.allCharmsGainedShown) MessageController.Enqueue(EmbeddedSprite.Get("AllCharms.png"), "Unlocked Achievement"); GameManager.instance.AwardAchievement("allCharmsGained"); LocalSaveData.allCharmsGainedShown = true; GlobalSaveData.allCharmsGainedAchGot = true;
            if (VoidSoul.instance.Equipped() && VoidSoul.instance.Settings(Settings).Got && !LocalSaveData.voidsoulachievementShown) MessageController.Enqueue(EmbeddedSprite.Get("VoidSoulAchievement.png"), "Unlocked Achievement"); GameManager.instance.AwardAchievement("voidSoulAchievement"); LocalSaveData.voidsoulachievementShown = true; GlobalSaveData.voidsoulachievementAchGot = true;
            if (CheckIfObtainedALLCharms() && !LocalSaveData.ALLCharmsGainedShown) MessageController.Enqueue(EmbeddedSprite.Get("CompletedVessel.png"), "Hidden Achievement Unlocked: Completed Vessel"); GameManager.instance.AwardAchievement("completedVessel"); LocalSaveData.ALLCharmsGainedShown = true; GlobalSaveData.ALLCharmsGainedAchGot = true;

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
                PlaceAllCharms();
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
                PlaceAllCharms();
                //Call OnWorldInit for all Room subclasses
                foreach (Room room in rooms)
                {
                    room.OnWorldInit();
                    Log("Initialized " + room.RoomName);
                }
                Log("Re-initialized all rooms.");
            }
            #endregion
        }
        public bool CheckIfGotAllCharms()
        {
            int counter = 0;
            foreach (Charm charm in Charms)
            {
                if (charm.Settings(Settings).Got)
                {
                    counter++;
                }
            }
            if (counter == Charms.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckIfObtainedALLCharms()
        {
            int counter = 0;
            foreach (Charm charm in Charms)
            {
                if (charm.Settings(Settings).Got)
                {
                    counter++;
                }
            }
            // sorry for that super long line...
            if (counter == Charms.Count && PlayerData.instance.gotCharm_1 && PlayerData.instance.gotCharm_2 &&
                PlayerData.instance.gotCharm_3 && PlayerData.instance.gotCharm_4 && PlayerData.instance.gotCharm_5 &&
                PlayerData.instance.gotCharm_6 && PlayerData.instance.gotCharm_7 && PlayerData.instance.gotCharm_8 &&
                PlayerData.instance.gotCharm_9 && PlayerData.instance.gotCharm_10 && PlayerData.instance.gotCharm_11 &&
                PlayerData.instance.gotCharm_12 && PlayerData.instance.gotCharm_13 && PlayerData.instance.gotCharm_14 &&
                PlayerData.instance.gotCharm_15 && PlayerData.instance.gotCharm_16 && PlayerData.instance.gotCharm_17 &&
                PlayerData.instance.gotCharm_18 && PlayerData.instance.gotCharm_19 && PlayerData.instance.gotCharm_20 &&
                PlayerData.instance.gotCharm_21 && PlayerData.instance.gotCharm_22 && PlayerData.instance.gotCharm_23 &&
                PlayerData.instance.gotCharm_24 && PlayerData.instance.gotCharm_25 && PlayerData.instance.gotCharm_26 &&
                PlayerData.instance.gotCharm_27 && PlayerData.instance.gotCharm_28 && PlayerData.instance.gotCharm_29 &&
                PlayerData.instance.gotCharm_30 && PlayerData.instance.gotCharm_31 && PlayerData.instance.gotCharm_32 &&
                PlayerData.instance.gotCharm_33 && PlayerData.instance.gotCharm_34 && PlayerData.instance.gotCharm_35 &&
                PlayerData.instance.gotCharm_36 && PlayerData.instance.gotCharm_37 && PlayerData.instance.gotCharm_38 && 
                PlayerData.instance.gotCharm_39 && PlayerData.instance.gotCharm_40)
            {
                return true;
            }
            else
            {
                return false;
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

        /// <summary>
        /// UNUSED - YOU SHOULD NOT NEED THIS VARIABLE
        /// </summary>
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
                                        MenuRef.Update(); // Update Menu

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

#region Charm Language Replacements
        private string GetCharmStrings(string key, string sheetName, string orig)
        {
            if (TextEdits.TryGetValue((key, sheetName), out var text))
            {
                return text();
            }
            return orig;
        }
#endregion

#region Random Useless Stuff 
        public float grav = 0f;
        public float gravsaved = 0f;

        public static void PlaceCharmsAtFixedPositions()
        {
            var placements = new List<AbstractPlacement>();
            foreach (var charm in Charms)
            {
                // make it so only some charms get placed, not all of them
                if (charm.Placeable)
                {
                    var name = charm.Name.Replace(" ", "_");
                    placements.Add(
                        new CoordinateLocation() { x = charm.X, y = charm.Y, elevation = 0, sceneName = charm.Scene, name = name }
                        .Wrap()
                        .Add(Finder.GetItem(name)));
                }
            }
            ItemChangerMod.AddPlacements(placements, conflictResolution: PlacementConflictResolution.Ignore);
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
            int CurrentMaxHp = PlayerData.instance.maxHealth;
            int WantedMaxHp = 5;
            int MaxHpChangeAmount = WantedMaxHp - CurrentMaxHp;
            HeroController.instance.AddToMaxHealth(MaxHpChangeAmount);
            PlayerData.instance.MaxHealth();
            HeroController.instance.MaxHealth();
            SoulSlow.instance.Settings(Settings).Equipped = true;
            SoulSlow.instance.Settings(Settings).Equipped = false;
        }

        public void HealthReset()
        {
            int hp = PlayerData.instance.health;
            int maxhp = PlayerData.instance.maxHealth;
            int changeamount = maxhp - hp;
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
#region Ugly code
            if (PlayerData.instance.colosseumBronzeCompleted && !LocalSaveData.QuickfallDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("Quickfall.png"), "Gained Charm"); LocalSaveData.QuickfallDonePopup = true;
            if (PlayerData.instance.colosseumSilverCompleted && !LocalSaveData.SlowfallDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("Slowfall.png"), "Gained Charm"); LocalSaveData.SlowfallDonePopup = true;
            if (PlayerData.instance.hasShadowDash && !LocalSaveData.PowerfulDashDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("PowerfulDash.png"), "Gained Charm"); LocalSaveData.PowerfulDashDonePopup = true;
            if (PlayerData.instance.hasNailArt && !LocalSaveData.SturdyNailDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SturdyNail.png"), "Gained Charm"); LocalSaveData.SturdyNailDonePopup = true;
            if (PlayerData.instance.hasDreamGate && !LocalSaveData.SoulHungerDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SoulHunger.png"), "Gained Charm"); LocalSaveData.SoulHungerDonePopup = true;
            if (PlayerData.instance.hasDreamNail && !LocalSaveData.SoulSlowDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SoulSlow.png"), "Gained Charm"); LocalSaveData.SoulSlowDonePopup = true;
            if (PlayerData.instance.hasSuperDash && PlayerData.instance.gaveSlykey && !LocalSaveData.BetterCDashDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("BetterCDash.png"), "Gained Charm"); LocalSaveData.BetterCDashDonePopup = true;
            if (PlayerData.instance.killedHollowKnight && !LocalSaveData.HKBlessingDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("HKBlessing.png"), "Gained Charm"); LocalSaveData.HKBlessingDonePopup = true;
            if (PlayerData.instance.hasKingsBrand && !LocalSaveData.HealthyShellDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("HealthyShell.png"), "Gained Charm"); LocalSaveData.HealthyShellDonePopup = true;
            if (PlayerData.instance.killedHollowKnightPrime && !LocalSaveData.GlassCannonDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("GlassCannon.png"), "Gained Charm"); LocalSaveData.GlassCannonDonePopup = true;
            if (PlayerData.instance.bankerAccountPurchased && !LocalSaveData.WealthyAmuletDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("WealthyAmulet.png"), "Gained Charm"); LocalSaveData.WealthyAmuletDonePopup = true;
            if (PlayerData.instance.colosseumGoldCompleted && !LocalSaveData.RavenousSoulDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("RavenousSoul.png"), "Gained Charm"); LocalSaveData.RavenousSoulDonePopup = true;
            if (PlayerData.instance.canOvercharm && !LocalSaveData.OpportunisticDefeatDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("OpportunisticDefeat.png"), "Gained Charm"); LocalSaveData.OpportunisticDefeatDonePopup = true;
            if (PlayerData.instance.collectorDefeated && !LocalSaveData.SoulSpellDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SoulSpell.png"), "Gained Charm"); LocalSaveData.SoulSpellDonePopup = true;
            if (PlayerData.instance.grubsCollected > 10 && !LocalSaveData.SlowTimeDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SlowTime.png"), "Gained Charm"); LocalSaveData.SlowTimeDonePopup = true;
            if (PlayerData.instance.statueStateCollector.completedTier2 && !LocalSaveData.SpeedTimeDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SpeedTime.png"), "Gained Charm"); LocalSaveData.SpeedTimeDonePopup = true;
            if (PlayerData.instance.mageLordDreamDefeated && !LocalSaveData.GeoSwitchDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("GeoSwitch.png"), "Gained Charm"); LocalSaveData.GeoSwitchDonePopup = true;
            if (PlayerData.instance.killedMageLord && !LocalSaveData.SoulSwitchDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SoulSwitch.png"), "Gained Charm"); LocalSaveData.SoulSwitchDonePopup = true;
            if (PlayerData.instance.nailsmithConvoArt && !LocalSaveData.SoulSpeedDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SoulSpeed.png"), "Gained Charm"); LocalSaveData.SoulSpeedDonePopup = true;
            if (PlayerData.instance.zotePrecept > 56 && !LocalSaveData.ZoteBornDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("ZoteBorn.png"), "Gained Charm"); LocalSaveData.ZoteBornDonePopup = true;
            if (PlayerData.instance.visitedWhitePalace && !LocalSaveData.ElderStoneDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("Elderstone.png"), "Gained Charm"); LocalSaveData.ElderStoneDonePopup = true;
            if (PlayerData.instance.gaveSlykey && PlayerData.instance.slyConvoNailHoned && PlayerData.instance.completionPercentage > 100 && !LocalSaveData.SlyDealDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("SlyDeal.png"), "Gained Charm"); LocalSaveData.SlyDealDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.GiantNailDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("GiantNail.png"), "Gained Charm"); LocalSaveData.GiantNailDonePopup = true;
            if (PlayerData.instance.hasAllNailArts && PlayerData.instance.hasKingsBrand && !LocalSaveData.MatosBlessingDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("MatosBlessing.png"), "Gained Charm"); LocalSaveData.MatosBlessingDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.ShellShieldDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("ShellShield.png"), "Gained Charm"); LocalSaveData.ShellShieldDonePopup = true;
            if (PlayerData.instance.honedNail && !LocalSaveData.VoidSoulDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("VoidSoulPopup.png"), "Gained Charm"); LocalSaveData.VoidSoulDonePopup = true;

            // make it buy from salubra
            if (PlayerData.instance.geo > 100 && PlayerData.instance.hasCityKey && !LocalSaveData.QuickjumpDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("Quickjump.png"), "Gained Charm"); LocalSaveData.QuickjumpDonePopup = true;
            if (PlayerData.instance.killedJellyfish && PlayerData.instance.killsJellyCrawler > 20 && !LocalSaveData.SlowjumpDonePopup) MessageController.Enqueue(EmbeddedSprite.Get("Slowjump.png"), "Gained Charm"); LocalSaveData.SlowjumpDonePopup = true;
#endregion
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

                PlaceAllCharms();
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

            if (LocalSaveData.BetterCDashGot) BetterCDash.instance.Settings(Settings).Got = true;
            if (LocalSaveData.BlueBloodGot) BlueBlood.instance.Settings(Settings).Got = true;
            if (LocalSaveData.ElderStoneGot) ElderStone.instance.Settings(Settings).Got = true;
            if (LocalSaveData.GeoSwitchGot) GeoSwitch.instance.Settings(Settings).Got = true;
            if (LocalSaveData.GiantNailGot) GiantNail.instance.Settings(Settings).Got = true;
            if (LocalSaveData.GlassCannonGot) GlassCannon.instance.Settings(Settings).Got = true;
            if (LocalSaveData.HealthyShellGot) HealthyShell.instance.Settings(Settings).Got = true;
            if (LocalSaveData.HKBlessingGot) HKBlessing.instance.Settings(Settings).Got = true;
            if (LocalSaveData.MatosBlessingGot) MatosBlessing.instance.Settings(Settings).Got = true;
            if (LocalSaveData.OpportunisticDefeatGot) OpportunisticDefeat.instance.Settings(Settings).Got = true;
            if (LocalSaveData.PowerfulDashGot) PowerfulDash.instance.Settings(Settings).Got = true;
            if (LocalSaveData.RavenousSoulGot) RavenousSoul.instance.Settings(Settings).Got = true;
            if (LocalSaveData.ShellShieldGot) ShellShield.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SlowTimeGot) SlowTime.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SlyDealGot) SlyDeal.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SoulHungerGot) SoulHunger.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SoulSlowGot) SoulSlow.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SoulSpeedGot) SoulSpeed.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SoulSpellGot) SoulSpell.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SoulSwitchGot) SoulSwitch.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SpeedTimeGot) SpeedTime.instance.Settings(Settings).Got = true;
            if (LocalSaveData.SturdyNailGot) SturdyNail.instance.Settings(Settings).Got = true;
            if (LocalSaveData.TripleJumpGot) TripleJump.instance.Settings(Settings).Got = true;
            if (LocalSaveData.VoidSoulGot) VoidSoul.instance.Settings(Settings).Got = true;
            if (LocalSaveData.WealthyAmuletGot) WealthyAmulet.instance.Settings(Settings).Got = true;
            if (LocalSaveData.ZoteBornGot) ZoteBorn.instance.Settings(Settings).Got = true;
            if (LocalSaveData.WyrmFormGot) WyrmForm.instance.Settings(Settings).Got = true;
            //if (LocalSaveData.FyreChildGot) Fyrechild.instance.Settings(Settings).Got = true;
            if (LocalSaveData.GravityCharmGot) GravityCharm.instance.Settings(Settings).Got = true;
            if (LocalSaveData.BulbousInfectionGot) BulbousInfection.instance.Settings(Settings).Got = true;

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

        private void OnCharmUpdate(PlayerData data, HeroController controller)
        {
            
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
                }
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
                title.GetComponent<SpriteRenderer>().color = new Color(193 / 255f, 225 / 255f, 253 / 255f);
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
}/*
If the textures don't work, put this back into a file, otherwise, leave it here.
namespace Fyrenest.Consts
{
    public class TextureStrings
    {
#region Misc
        public const string QuickfallKey = "Quickfall";
        private const string QuickfallFile = "Fyrenest.Resources.Quickfall.png";
        public const string SlowfallKey = "Slowfall";
        private const string SlowfallFile = "Fyrenest.Resources.Slowfall.png";
        public const string SturdyNailKey = "SturdyNail";
        private const string SturdyNailFile = "Fyrenest.Resources.SturdyNail.png";
        public const string BetterCDashKey = "BetterCDash";
        private const string BetterCDashFile = "Fyrenest.Resources.BetterCDash.png";
        public const string GlassCannonKey = "GlassCannon";
        private const string GlassCannonFile = "Fyrenest.Resources.GlassCannon.png";
        public const string HKBlessingKey = "HKBlessing";
        private const string HKBlessingFile = "Fyrenest.Resources.HKBlessing.png";
        public const string HuntersMarkKey = "HuntersMark";
        private const string HuntersMarkFile = "Fyrenest.Resources.HuntersMark.png";
        public const string PowerfulDashKey = "PowerfulDash";
        private const string PowerfulDashFile = "Fyrenest.Resources.PowerfulDash.png";
        public const string WealthyAmuletKey = "WealthyAmulet";
        private const string WealthyAmuletFile = "Fyrenest.Resources.WealthyAmulet.png";
        public const string TripleJumpKey = "TripleJump";
        private const string TripleJumpFile = "Fyrenest.Resources.TripleJump.png";
#endregion Misc

        private readonly Dictionary<string, Sprite> _dict;

        public TextureStrings()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            _dict = new Dictionary<string, Sprite>();
            Dictionary<string, string> tmpTextures = new Dictionary<string, string>();
            tmpTextures.Add(QuickfallKey, QuickfallFile);
            tmpTextures.Add(SlowfallKey, SlowfallFile);
            tmpTextures.Add(SturdyNailKey, SturdyNailFile);
            tmpTextures.Add(BetterCDashKey, BetterCDashFile);
            tmpTextures.Add(GlassCannonKey, GlassCannonFile);
            tmpTextures.Add(HKBlessingKey, HKBlessingFile);
            tmpTextures.Add(HuntersMarkKey, HuntersMarkFile);
            tmpTextures.Add(PowerfulDashKey, PowerfulDashFile);
            tmpTextures.Add(WealthyAmuletKey, WealthyAmuletFile);
            tmpTextures.Add(TripleJumpKey, TripleJumpFile);
            foreach (var t in tmpTextures)
            {
                using (Stream s = asm.GetManifestResourceStream(t.Value))
                {
                    if (s == null) continue;

                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    //Create texture from bytes
                    var tex = new Texture2D(2, 2);

                    tex.LoadImage(buffer, true);

                    // Create sprite from texture
                    // Split is to cut off the TestOfTeamwork.Resources. and the .png
                    _dict.Add(t.Key, Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f)));
                }
            }
        }

        public Sprite Get(string key)
        {
            return _dict[key];
        }
    }
}*/