using Modding;

namespace Fyrenest
{
    internal class GeoSwitch : Charm
    {
        public static readonly GeoSwitch Instance = new();
        public override string Sprite => "GeoSwitch.png";
        public override string Name => "Geo Switch";
        public override string Description => "Draws power from your shell to give you geo.";

        public override int DefaultCost => 5;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private GeoSwitch() {}

        public override CharmSettings Settings(SaveSettings s) => s.GeoSwitch;

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
                        Modding.Logger.Log("Giving player geo:");
                        HeroController.instance.AddGeo(50);
                        Modding.Logger.Log("Done.");
                    }
                }
            }
            return;
        }
    }
}