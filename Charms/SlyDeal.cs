namespace Fyrenest
{
    internal class SlyDeal : Charm
    {
        public static readonly SlyDeal instance = new();
        public override string Sprite => "SlyDeal.png";
        public override string Name => "Sly Deal";
        public override string Description => "This small, lightweight charm bears the likeless of a small bug called Sly.\n\nAlso has a side effect and embodies a foul stench of rancid eggs into the bearer.";
        public override int DefaultCost => 3;
        public override string Scene => "Town";
        public override float X => 0;
        public override float Y => 0;
        private SlyDeal() {}
        public override CharmSettings Settings(SaveSettings s) => s.SlyDeal;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += OnUpdate;
        }

        public void OnUpdate()
        {
            if (Equipped())
            {
                if (PlayerData.instance.maxHealth < 11)
                    PlayerData.instance.slyShellFrag4 = false;
                if (PlayerData.instance.MPReserveMax < 198)
                    PlayerData.instance.slyVesselFrag3 = false;
            }
        }
    }
}