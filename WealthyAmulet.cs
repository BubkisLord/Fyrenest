using Modding;

namespace CharmMod
{
    internal class WealthyAmulet : Charm
    {
        public static readonly WealthyAmulet Instance = new();
        public override string Sprite => "WealthyAmulet.png";
        public override string Name => "Amulet of Wealth";
        public override string Description => "Desc";
        public override int DefaultCost => 2;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private WealthyAmulet() {}

        public override CharmSettings Settings(SaveSettings s) => s.WealthyAmulet;

        public override void Hook()
        {
            ModHooks.BeforeAddHealthHook += BeforeAddHealth;
            ModHooks.SoulGainHook += GainSoul;
        }

        public int GainSoul(int amount)
        {
            if(Equipped()) {
                if (PlayerData.instance.health == 1) {
                    HeroController.instance.AddGeo(20);
                    return amount * 2;
                }
                else {
                    HeroController.instance.AddGeo(10);
                    return amount;
                }
            }
            else {
                return amount;
            }
        }

        //healing costs 100 geo.
        public int BeforeAddHealth( int amount ) {
            if(Equipped()) {
                if(PlayerData.instance.geo > 100 ) {
                    HeroController.instance.TakeGeo(100);
                    return amount;
                }
                else
                    return 0;
            }
            else
                return amount;
        }
    }
}