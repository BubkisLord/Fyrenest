namespace Fyrenest
{
    internal class VoidSoul : Charm
    {
        public static readonly VoidSoul instance = new();
        public override string Sprite => "VoidSoul.png";
        public override string Name => "Void Soul";
        public override string Description => "A charm radiating with raw power. Made with both pale shell and pure void, this charm is the focused rage and strength of Fyrenest.\n\nWhen worn, while the bearer's shade is alive, the charm accumulates power from the shade and gaining soul. Your speed, jump height, strength, and power is increased. These can be increased more by gaining soul.";
        public override int DefaultCost => 5;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private VoidSoul() {}
        
        public override CharmSettings Settings(SaveSettings s) => s.VoidSoul;

        private int naildamageadd;
        private int extraspeedcount;
        private float extraspeedmodifier;

        public override void Hook()
        {
            ModHooks.GetPlayerIntHook += BuffNail;
            ModHooks.SetPlayerBoolHook += UpdateNailDamageOnEquip;
            On.HeroController.Move += SpeedUp;
            ModHooks.HeroUpdateHook += ChangeGravity;
            ModHooks.SoulGainHook += ModHooks_SoulGainHook;
        }

        private int ModHooks_SoulGainHook(int soulAmount)
        {
            if (PlayerData.instance.shadeScene != "None" && Equipped())
            {
                soulAmount *= 2;
                naildamageadd += 1;
                if (naildamageadd > 2) PlayerData.instance.SetInt("nailDamage", PlayerData.instance.nailDamage + 1); naildamageadd = 0;
                extraspeedcount += 1;
                if (extraspeedcount > 3) extraspeedmodifier += 0.1f; extraspeedcount = 0;
            }
            return soulAmount;
        }

        private void ChangeGravity()
        {
            Fyrenest.UpdateNailDamage();
            if (PlayerData.instance.shadeScene != "None" && Equipped())
            {
                if (HeroController.instance == null)
                {
                    return;
                }
                var rb = HeroController.instance.gameObject.GetComponent<Rigidbody2D>();
                // Gravity gets set to 0 during transitions; we must not mess with that or
                // the game will hardlock bouncing back and forth between two rooms when
                // passing through a horizontal transition.
                if (rb.gravityScale == 0)
                {
                    return;
                }
                // Keep normal gravity after going through upwards transitions, so that the player does not fall
                // through spikes in some rooms before they gain control.
                rb.gravityScale = (Equipped() && HeroController.instance.transitionState == GlobalEnums.HeroTransitionState.WAITING_TO_TRANSITION) ? 0.5f : 0.79f;
                return;
            }
        }

        private void SpeedUp(On.HeroController.orig_Move orig, HeroController self, float speed)
        {
            if (Equipped() && speed != 0 && PlayerData.instance.shadeScene != "None")
            {
                speed *= 2.0f+extraspeedmodifier;
            }
            orig(self, speed);
        }
        private int BuffNail(string intName, int damage)
        {
            if (PlayerData.instance.shadeScene != "None")
            {
                if (intName == "nailDamage" && Equipped())
                {
                    damage *= 2;
                }
            }
            return damage;
        }

        private bool UpdateNailDamageOnEquip(string boolName, bool value)
        {
            if (boolName == $"equippedCharm_{Num}")
            {
                Fyrenest.UpdateNailDamage();
            }
            return value;
        }
    }
}