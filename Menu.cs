using Satchel.BetterMenus;

namespace CharmMod
{
    public class ClassName : Mod, ICustomMenuMod, ITogglableMod, ILocalSettings<SaveSettings>, IMod
    {
        public ClassName() : base("Charm Mod") { }
        public override string GetVersion() => CharmMod.Instance.GetVersion();
        public override int LoadPriority() => 1;
        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        private int charmSelect = 0;
        public override void Initialize()
        {
            ModHooks.LanguageGetHook += LanguageGet;
        }

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

            if (ZoteBorn.Instance.Equipped() && key == "" && sheetTitle == "Prices")
            {
                return "Test area, TELL ME WHERE U FOUND THIS. Ping me @BubkisLord#5187 (discord)";
            }

            return orig;
        }

        private Menu MenuRef;
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modtoggledelegates)
        {
            MenuRef ??= new Menu(
                        "CharmMod",
                        new Element[]
                        {
                            new MenuButton("Reset health", "Make health to max health.", (_) => HealthReset()),
                            new MenuButton("Add health", "Change health by 1.", (_) => AddHealth()),
                            new MenuButton("Take health", "Change health by -1.", (_) => TakeHealth()),
                            new MenuButton("Reset Max Health", "Sets max health to 5", (_) => MaxHealthReset()),
                            new MenuButton("Add Max Health", "Increase max health by one. (Equip and de-equip the slow soul charm to update)", (_) => AddMaxHealth()),
                            new MenuButton("Take Max Health", "Decrease max health by one. (Equip and de-equip the slow soul charm to update)", (_) => TakeMaxHealth()),
                            new MenuButton("Reset Soul", "Make soul the max soul amount.", (_) => HeroController.instance.AddMPCharge(PlayerData.instance.MPReserveMax)),
                            new MenuButton("Add Soul", "Add one charge of soul.", (_) => HeroController.instance.AddMPCharge(33)),
                            new MenuButton("Take Soul", "Take one charge of soul.", (_) => HeroController.instance.TakeMP(33)),
                            new MenuButton("Give Specific Charm", "+1 to charm select", (_) => SelectCharm(1)),
                            new MenuButton("Give Specific Charm", "Pick the charm", (_) => GiveSpecificCharm(charmSelect)),
                            new MenuButton("Give Charms", "Get all CharmMod charms.", (_) => CharmMod.Instance.GrantAllOurCharms()),
                            new MenuButton("Take Charms", "Take all CharmMod charms.", (_) => CharmMod.Instance.TakeAllOurCharms())
                        }
            );
            return MenuRef.GetMenuScreen(modListMenu);
        }

        public int SelectCharm(int bubkis)
        {
            if (charmSelect > CharmMod.Instance.NewCharms)
            {
                charmSelect = 0;
            }
            charmSelect += bubkis;
            return bubkis;
        }

        public int GiveSpecificCharm(int bubkis)
        {
            CharmMod.Instance.GiveSpecificCharm(bubkis);
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
        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void OnLoadLocal(SaveSettings s)
        {
            throw new NotImplementedException();
        }

        public SaveSettings OnSaveLocal()
        {
            throw new NotImplementedException();
        }
    }
}