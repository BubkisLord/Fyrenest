using Modding;

namespace Fyrenest
{
    internal class SoulHunger : Charm
    {
        public static readonly SoulHunger instance = new();
        public override bool Placeable => false;
        public override string Sprite => "SoulHunger.png";
        public override string Name => "Hungry Soul";
        public override string Description => "This charm hungers for soul.\n\nYou gain an extreme amount of soul for every hit.";
        public override int DefaultCost => 3;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private SoulHunger() { }

        public override CharmSettings Settings(SaveSettings s) => s.SoulHunger;

        public override void Hook()
        {
            ModHooks.SoulGainHook += OnGetSoul;
        }

        private int OnGetSoul(int amount)
        {
            if (Equipped()) amount *= 6;
            return amount;
        }
    }
}