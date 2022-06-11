using Modding;

namespace CharmMod
{
    internal class SoulSlow : Charm
    {
        public static readonly SoulSlow Instance = new();
        public override string Sprite => "SoulSlow.png";
        public override string Name => "Slow Soul";
        public override string Description => "This thick, callous charm glows brightly when near your shell.\n\nWhen worn, the bearer gains increased max health but makes you slower.";
        public override int DefaultCost => 4;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private SoulSlow() {}

        public override CharmSettings Settings(SaveSettings s) => s.SoulSlow;

        public override void Hook()
        {
            On.HeroController.Move += SlowDown;
            ModHooks.HeroUpdateHook += Update;
        }

        private const int PhysicsFramesPerSecond = 50;

        private const int ChargeInterval = 3 * PhysicsFramesPerSecond;

        private static int ChargeTimer = 0;

        private static bool Worn = false;

        public void Update()
        {
            if (!Worn && Equipped())
            {
                Worn = true;
                int maxHp = 11;
                int hp = PlayerData.instance.health;
                int hpAdd = maxHp - hp;
                if(hpAdd > 4) HeroController.instance.AddToMaxHealth(4);
                else HeroController.instance.AddToMaxHealth(4);
            }
            if (Worn && !Equipped())
            {
                Worn = false;
                HeroController.instance.AddToMaxHealth(-4);
            }
        }

        private void SlowDown(On.HeroController.orig_Move orig, HeroController self, float speed)
        {
            if (Equipped() && speed != 0)
            {
                speed -= speed*0.4f;
                ChargeTimer++;
                if (ChargeTimer == ChargeInterval)
                {
                    ChargeTimer = 0;
                }
            }
            orig(self, speed);
        }
    }
}