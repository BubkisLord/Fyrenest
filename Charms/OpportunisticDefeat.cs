using Modding;

namespace Fyrenest
{
    internal class OpportunisticDefeat : Charm
    {
        public static readonly OpportunisticDefeat instance = new();
        public override bool Placeable => false;
        public override string Sprite => "LessDamage.png";
        public override string Name => "Tiso's Shield";
        public override string Description => "When worn, the bearer takes the most of getting hit.\n\nWhen attacked, you gain both soul and geo. Plus, every 4th time you get hit, you don't take damage.";
        public override int DefaultCost => 1;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private OpportunisticDefeat() {}

        public override CharmSettings Settings(SaveSettings s) => s.OpportunisticDefeat;

        private static int EscapeDamage = 0;

        public override void Hook()
        {
            ModHooks.TakeHealthHook += GiveGeoAndSoul;
        }
        private int GiveGeoAndSoul(int damage)
        {
            if (Equipped() && !WealthyAmulet.instance.Equipped())
            {
                HeroController.instance.AddGeo(15);
                HeroController.instance.AddMPCharge(33);
                EscapeDamage += damage;
            }
            if (Equipped() && WealthyAmulet.instance.Equipped())
            {
                HeroController.instance.AddGeo(30);
                HeroController.instance.AddMPCharge(33);
                EscapeDamage += damage;
            }
            DmgCheck:
            if (Equipped() && EscapeDamage > 3)
            {
                EscapeDamage -= 4;
                damage -= 1;
                if (EscapeDamage > 3)
                {
                    goto DmgCheck;
                }
            }
            return damage;
        }
    }
}