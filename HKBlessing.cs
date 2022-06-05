using HutongGames.PlayMaker.Actions;

namespace CharmMod
{
    internal class HKBlessing : Charm
    {
        public static readonly HKBlessing Instance = new();
        public override string Sprite => "HKBlessing.png";
        public override string Name => "Hollow Knight's Blessing";
        public override string Description => "Desc";
        public override int DefaultCost => 2;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private HKBlessing() { }

        public override CharmSettings Settings(SaveSettings s) => s.HKBlessing;

        public override void Hook()
        {
            ModHooks.BlueHealthHook += BlueHPRestored;
        }


        //This snippet makes the lifeblood charms twice as effective.
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
    }
}