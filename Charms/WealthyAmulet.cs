using Modding;

namespace Fyrenest
{
    internal class WealthyAmulet : Charm
    {
        public static readonly WealthyAmulet Instance = new();
        public override string Sprite => "WealthyAmulet.png";
        public override string Name => "Amulet of Wealth";
        public override string Description => "Allows the bearer to accumulate large amounts of geo.\n\nYou gain geo when attacking an enemy, but it costs 100 geo to heal.";
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
            if(Equipped() && SlyDeal.Instance.Equipped()) {
                if (PlayerData.instance.health == 1) {
                    HeroController.instance.AddGeo(40);
                    return amount * 2;
                }
                else {
                    HeroController.instance.AddGeo(20);
                    return amount;
                }
            }
            if (Equipped() && !SlyDeal.Instance.Equipped())
            {
                if (PlayerData.instance.health == 1)
                {
                    HeroController.instance.AddGeo(20);
                    return amount * 2;
                }
                else
                {
                    HeroController.instance.AddGeo(10);
                    return amount;
                }
            }
            return amount;
        }

        //healing costs 100 geo.
        public int BeforeAddHealth( int amount ) {
            //just wealthy amulet equipped
            if(Equipped() && !HealthyShell.Instance.Equipped()) {
                if(PlayerData.instance.geo > 100 ) {
                    HeroController.instance.TakeGeo(100);
                    return amount;
                }
                else
                    return 0;
            }
            //both equipped (no cost to heal)
            if (Equipped() && HealthyShell.Instance.Equipped())
            {
                return amount;
            }
            //not equipped
            if (!Equipped() && HealthyShell.Instance.Equipped())
                return amount;
            if (!Equipped() && !HealthyShell.Instance.Equipped())
                return amount;
            //so BeforeAddHealth() doesn't have an error.
            else
                return amount;
        }
    }
}