using Modding;

namespace Fyrenest
{
    internal class SoulSpeed : Charm
    {
        public static readonly SoulSpeed Instance = new();
        public override string Sprite => "SoulSpeed.png";
        public override string Name => "Speed Soul";
        public override string Description => "Slowly takes soul for a great increase in speed.";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private SoulSpeed() {}

        public override CharmSettings Settings(SaveSettings s) => s.SoulSpeed;

        public override void Hook()
        {
            On.HeroController.Move += SpeedUp;
        }

        private const int PhysicsFramesPerSecond = 50;

        private const int ChargeInterval = 3 * PhysicsFramesPerSecond;

        private const float MaxSpeedupFactor = 3.0f;

        private static int ChargeTimer = 0;

        private void SpeedUp(On.HeroController.orig_Move orig, HeroController self, float speed)
        {
            if (Equipped() && speed != 0)
            {
                speed += speed*0.4f;
                ChargeTimer++;
                if (ChargeTimer == ChargeInterval)
                {
                    ChargeTimer = 0;
                    self.TakeMP(33);
                }
            }
            orig(self, speed);
        }
    }
}