namespace CharmMod
{
    internal class SlyDeal : Charm
    {
        public static readonly SlyDeal Instance = new();
        public override string Sprite => "SlyDeal.png";
        public override string Name => "Sly Deal";
        public override string Description => "Desc";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private SlyDeal() {}

        public override CharmSettings Settings(SaveSettings s) => s.SlyDeal;
    }
}