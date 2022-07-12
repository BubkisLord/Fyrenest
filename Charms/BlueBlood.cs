using Modding;

namespace Fyrenest
{
    internal class BlueBlood : Charm
    {
        public static readonly BlueBlood Instance = new();
        public override string Sprite => "LifeBloodCharm.png";
        public override string Name => "Blue Blood";
        public override string Description => "Allows lifeblood to be regenerated, but only once.";
        public override int DefaultCost => 3;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private BlueBlood() {}

        public override CharmSettings Settings(SaveSettings s) => s.BlueBlood;

        public override void Hook()
        {
            ModHooks.BeforeAddHealthHook += OnHeal;
        }
        private int OnHeal(int damage)
        {
            if (Equipped())
            {
                if (PlayerData.instance.health >= PlayerData.instance.maxHealth)
                {
                    if (PlayerData.instance.equippedCharm_8)
                    {
                        if (PlayerData.instance.equippedCharm_9)
                        {
                            if (PlayerData.instance.healthBlue !>= 6)
                            {
                                PlayerData.instance.healthBlue += 1;
                                PlayerData.instance.UpdateBlueHealth();
                            }
                        }
                        else
                        {
                            if (PlayerData.instance.healthBlue !>= 2)
                            {
                                PlayerData.instance.healthBlue += 1;
                                PlayerData.instance.UpdateBlueHealth();
                            }
                        }
                    }
                    else
                    {
                        if (PlayerData.instance.equippedCharm_9)
                        {
                            if (PlayerData.instance.healthBlue !>= 4)
                            {
                                PlayerData.instance.healthBlue += 1;
                                PlayerData.instance.UpdateBlueHealth();
                            }
                        }
                    }
                }
            }
            return damage;
        }
    }
}