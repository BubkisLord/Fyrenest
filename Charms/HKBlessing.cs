using HutongGames.PlayMaker.Actions;

namespace Fyrenest
{
    internal class HKBlessing : Charm
    {
        public static readonly HKBlessing instance = new();
        public override string Sprite => "HKBlessing.png";
        public override string Name => "Hollow Knight's Blessing";
        public override string Description => "Contains the blessing of The Hollow Knight.\n\nMakes lifeblood charms more powerful.";
        public override int DefaultCost => 2;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private HKBlessing() { }

        public override CharmSettings Settings(SaveSettings s) => s.HKBlessing;

        public override void Hook()
        {
            ModHooks.BlueHealthHook += BlueHPRestored;
            ModHooks.HeroUpdateHook += OnUpdate;
        }


        public int BlueHPRestored() {
            if (Equipped())
            {
                int retValue = 0;
                if (PlayerData.instance.GetBool("equippedCharm_8")) retValue += 2;
                if (PlayerData.instance.GetBool("equippedCharm_9")) retValue += 4;
                return retValue;
            }
            else
                return 0;
        }
        public void OnUpdate()
        {
            if (PlayerData.instance.GetBool("equippedCharm_27") && Equipped())
            {
                PlayerData.instance.SetInt("joniHealthBlue", (int)((float)PlayerData.instance.maxHealth * 1.7f));
                PlayerData.instance.MaxHealth();
            }
        }
    }
}