using Modding;

namespace Fyrenest
{
    internal class BlueBlood : Charm
    {
        public static readonly BlueBlood instance = new();
        public override bool Placeable => false;
        public override string Sprite => "LifeBloodCharm.png";
        public override string Name => "Blue Blood";
        public override string Description => "Allows lifeblood to be regenerated, but only once.";
        public override int DefaultCost => 3;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private BlueBlood() {}

        public override CharmSettings Settings(SaveSettings s) => s.BlueBlood;

        public override void Hook()
        {
            On.HeroController.AddHealth += OnHeal;
        }
        private void OnHeal(On.HeroController.orig_AddHealth orig, HeroController hero, int amount)
        {
            if (Equipped())
            {
                if (PlayerData.instance.health >= PlayerData.instance.maxHealth)
                {
                    if (PlayerData.instance.equippedCharm_8)
                    {
                        if (PlayerData.instance.equippedCharm_9)
                        {
                            if (PlayerData.instance.healthBlue < 6)
                            {
                                EventRegister.SendEvent("ADD BLUE HEALTH");
                            }
                        }
                        else
                        {
                            if (PlayerData.instance.healthBlue < 2)
                            {
                                EventRegister.SendEvent("ADD BLUE HEALTH");
                            }
                        }
                    }
                    else
                    {
                        if (PlayerData.instance.equippedCharm_9)
                        {
                            if (PlayerData.instance.healthBlue < 4)
                            {
                                EventRegister.SendEvent("ADD BLUE HEALTH");
                            }
                        }
                    }
                }
            }
            orig(hero, amount);
        }
    }
}