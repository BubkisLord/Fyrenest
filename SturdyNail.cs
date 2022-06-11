namespace CharmMod
{
    internal class SturdyNail : Charm
    {
        public static readonly SturdyNail Instance = new();
        public override string Sprite => "SturdyNail.png";
        public override string Name => "Sturdy Nail";
        public override string Description => "This strong charm allows the bearer to slash with more powerful strikes.\n\nGreatly increases the strength of the bearer's nail.";
        public override int DefaultCost => 2;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private SturdyNail() {}
        
        public override CharmSettings Settings(SaveSettings s) => s.SturdyNail;

        public override void Hook()
        {
            ModHooks.GetPlayerIntHook += BuffNail;
            ModHooks.SetPlayerBoolHook += UpdateNailDamageOnEquip;
        }

        private int BuffNail(string intName, int damage)
        {
            if (intName == "nailDamage" && Equipped())
            {
                damage *= 2;
            }
            return damage;
        }

        private bool UpdateNailDamageOnEquip(string boolName, bool value)
        {
            if (boolName == $"equippedCharm_{Num}")
            {
                CharmMod.UpdateNailDamage();
            }
            return value;
        }
    }
}