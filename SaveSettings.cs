namespace CharmMod
{
    public class SaveSettings
    {
        public CharmSettings Quickfall = new();
        public CharmSettings Slowfall = new();
        public CharmSettings SturdyNail = new();
        public CharmSettings BetterCDash = new();
        public CharmSettings HuntersMark = new();
        public CharmSettings HKBlessing = new();
        public CharmSettings GlassCannon = new();
        public CharmSettings WealthyAmulet = new();
        public CharmSettings PowerfulDash = new();
        public CharmSettings DoubleDash = new();
        public CharmSettings SoulSpeed = new();
        public CharmSettings SoulSpell = new();
        public CharmSettings SoulHunger = new();
        public CharmSettings RavenousSoul = new();
        public CharmSettings SoulSwitch = new();
        public CharmSettings HealthyShell = new();
        public CharmSettings GeoSwitch = new();
        public CharmSettings SoulSlow = new();
        public CharmSettings SlowTime = new();
        public CharmSettings SpeedTime = new();
        public CharmSettings ZoteBorn = new();
        public CharmSettings SlyDeal = new();
        public CharmSettings ElderStone = new();

        public bool[] gotCharms = new[] { true, true, true, true };
        public bool[] newCharms = new[] { false, false, false, false };
        public bool[] equippedCharms = new[] { false, false, false, false };
        public int[] charmCosts = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }
}