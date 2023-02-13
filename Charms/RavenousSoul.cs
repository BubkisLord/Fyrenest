using Modding;

namespace Fyrenest
{
    internal class RavenousSoul : Charm
    {
        public static readonly RavenousSoul instance = new();
        public override bool Placeable => false;
        public override string Sprite => "RavenousSoul.png";
        public override string Name => "Ravenous Soul";
        public override string Description => "This charm is ravenous for soul.\n\nYou gain an extraordinary amount of soul for every hit, but take double damage.";
        public override int DefaultCost => 4;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private RavenousSoul() { }

        public override CharmSettings Settings(SaveSettings s) => s.RavenousSoul;

        public override void Hook()
        {
            ModHooks.SoulGainHook += OnGetSoul;
            ModHooks.TakeHealthHook += OnHealthTaken;
        }

        private int OnGetSoul(int amount)
        {
            if (Equipped())
            {
                amount = 198;
                return amount;
            }
            else
                return amount;
        }

        private int OnHealthTaken(int damage)
        {
            if(Equipped()) {
                damage *= 2;
                return damage;
            }
            else {
                return damage;
            }
        }
    }
}