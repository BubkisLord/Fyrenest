namespace CharmMod
{
    internal class SlowTime : Charm
    {
        public static readonly SlowTime Instance = new();
        public override string Sprite => "SlowTime.png";
        public override string Name => "Slow Time";
        public override string Description => "When holding this charm, everything seems to slow down slightly.\n\nThis charm alters the very fabric of time. When worn, the bearer and anything around it slows down.";
        public override int DefaultCost => 2;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private SlowTime() {}

        public override CharmSettings Settings(SaveSettings s) => s.SlowTime;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += SlowDownTime;
        }

        public static void SlowDownTime()
        {
            if(SlowTime.Instance.Equipped() && !SpeedTime.Instance.Equipped()) {
                float num = 0.60f;
                Time.timeScale = num;
            }
        }
    }
}