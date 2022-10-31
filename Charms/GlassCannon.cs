using Modding;

namespace Fyrenest
{
    internal class GlassCannon : Charm
    {
        public static readonly GlassCannon instance = new();
        public override string Sprite => "GlassCannon.png";
        public override string Name => "Charm of Radiance";
        public override string Description => "A glowing charm radiating power. When worn, makes the bearer's nail glow brightly.\n\nYou rip through enemies at ease, and destroy shells with one fell strike. But it also takes power from your shell, making you easy to fell.";
        public override int DefaultCost => 3;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;

        private GlassCannon() {}

        public override CharmSettings Settings(SaveSettings s) => s.GlassCannon;

        public override void Hook()
        {
            ModHooks.GetPlayerIntHook += BuffNail;
            ModHooks.SetPlayerBoolHook += UpdateNailDamageOnEquip;
            ModHooks.TakeHealthHook += OnHealthTaken;
        }
        private int BuffNail(string intName, int damage)
        {
            if (intName == "nailDamage" && Equipped())
            {
                damage = 6000;
            }
            return damage;
        }
        private int OnHealthTaken(int damage)
        {
            if(Equipped()) {
                PlayerData.instance.health = 0;
                return damage;
            }
            else {
                return damage;
            }
        }
        internal static void UpdateNailDamage()
        {
            IEnumerator WaitThenUpdate()
            {
                yield return null;
                PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            }
            GameManager.instance.StartCoroutine(WaitThenUpdate());
        }
        
        
        private bool UpdateNailDamageOnEquip(string boolName, bool value)
        {
            if (boolName == $"equippedCharm_{Num}")
            {
                UpdateNailDamage();
            }
            return value;
        }
    }
}