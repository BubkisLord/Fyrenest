namespace Fyrenest
{
    internal class MatosBlessing : Charm
    {
        public static readonly MatosBlessing instance = new();
        public override string Sprite => "MatosBlessing.png";
        public override string Name => "Mato's Blessing";
        public override string Description => "Imbues the power of Mato into your nail, focusing the strength and honour of a true master.\n\nWhen hitting an enemy with a cyclone slash, you draw health, geo and energy from the target, making you more powerful.";
        public override int DefaultCost => 2;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;

        private MatosBlessing() { }

        public override CharmSettings Settings(SaveSettings s) => s.DefendersMark;

        public bool isCycloning;

        public override void Hook()
        {
            ModHooks.CharmUpdateHook += OnCharmUpdate;
            On.HeroController.StartCyclone += HeroController_StartCyclone;
            On.HeroController.EndCyclone += HeroController_EndCyclone;
            ModHooks.SoulGainHook += SoulGain;
        }

        private int SoulGain(int soulgained)
        {
            if (isCycloning && Equipped())
            {
                soulgained += 11;
                if (PlayerData.instance.health != PlayerData.instance.maxHealth) HeroController.instance.AddHealth(1);
                HeroController.instance.AddGeo(10);
                HeroController.instance.Attack(GlobalEnums.AttackDirection.normal);
            }
            return soulgained;
        }

        private void HeroController_EndCyclone(On.HeroController.orig_EndCyclone orig, HeroController self)
        {
            isCycloning = false;
            orig(self);
        }

        private void HeroController_StartCyclone(On.HeroController.orig_StartCyclone orig, HeroController self)
        {
            isCycloning = true;
            orig(self);
        }

        private void OnCharmUpdate(PlayerData data, HeroController controller)
        {
            if (Equipped())
            {
                HeroController.instance.CYCLONE_HORIZONTAL_SPEED = 10;
            }
        }
    }
}