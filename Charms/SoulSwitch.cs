using Modding;

namespace Fyrenest
{
    internal class SoulSwitch : Charm
    {
        public static readonly SoulSwitch instance = new();
        public override bool Placeable => false;
        public override string Sprite => "SoulSwitch.png";
        public override string Name => "Switching Soul";
        public override string Description => "Draws power from your shell to give you soul.";
        public override int DefaultCost => 5;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private SoulSwitch() {}

        public override CharmSettings Settings(SaveSettings s) => s.SoulSwitch;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += Update;
        }

        private const int PhysicsFramesPerSecond = 50;

        private const int ChargeInterval = 10 * PhysicsFramesPerSecond;

        private static int ChargeTimer = 0;

        private void Update()
        {
            if (Equipped() && !PlayerData.instance.atBench)
            {
                ChargeTimer++;
                if (ChargeTimer == ChargeInterval)
                {
                    Modding.Logger.Log("Timer Up.");
                    ChargeTimer = 0;
                    if (PlayerData.instance.health == 1)
                    {
                        Modding.Logger.Log("Not enough health");
                    }
                    else {
                        Modding.Logger.Log("Taking health");
                        HeroController.instance.TakeHealth(1);
                        Modding.Logger.Log("Done.");
                        Modding.Logger.Log("Giving player Soul:");
                        HeroController.instance.AddMPCharge(33);
                        Modding.Logger.Log("Done.");
                    }
                }
            }
            return;
        }
    }
}