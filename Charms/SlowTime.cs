namespace Fyrenest
{
    internal class SlowTime : Charm
    {
        public static readonly SlowTime instance = new();
        public override string Sprite => "SlowTime.png";
        public override string Name => "Slow Time";
        public override string Description => "When holding this charm, everything seems to slow down slightly.\n\nThis charm alters the very fabric of time. When worn, the bearer and anything around it slows down.";
        public override int DefaultCost => 2;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;

        private SlowTime() {}

        public override CharmSettings Settings(SaveSettings s) => s.SlowTime;

        public override void Hook()
        {
            ModHooks.CharmUpdateHook += SlowDownTime;
        }

        public static void SlowDownTime(PlayerData data, HeroController controller)
        {
            if (SlowTime.instance.Equipped() && !SpeedTime.instance.Equipped() && !GameManager.instance.isPaused)
            {
                float num3 = 0.60f;
                Time.timeScale = num3;
            }
        }
    }
}