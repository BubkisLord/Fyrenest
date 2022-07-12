namespace Fyrenest
{
    internal class ZoteBorn : Charm
    {
        public static readonly ZoteBorn Instance = new();
        public override string Sprite => "ZoteBorn.png";
        public override string Name => "ZoteBorn";
        public override string Description => "This magnificent charm bears the likeless of the one, the only Zote The Mighty!\n\nEmbodies the might, the strength, the sheer power of Zote The Mighty into the bearer.";
        public override int DefaultCost => 6;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private ZoteBorn() {}
        
        public override CharmSettings Settings(SaveSettings s) => s.ZoteBorn;

        public override void Hook()
        {
            ModHooks.GetPlayerIntHook += BuffNail;
            ModHooks.SetPlayerBoolHook += UpdateNailDamageOnEquip;
            On.HeroController.Move += SpeedUp;
            ModHooks.HeroUpdateHook += ChangeGravity;
        }

        private void ChangeGravity()
        {
            if (Input.GetKey(KeyCode.Z) && Equipped())
            {
                PlayMakerFSM.BroadcastEvent("INSTA KILL");
            }

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
        }

        private void SpeedUp(On.HeroController.orig_Move orig, HeroController self, float speed)
        {
            if (Equipped() && speed != 0)
            {
                speed *= 2.0f;
            }
            orig(self, speed);
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
                Fyrenest.UpdateNailDamage();
            }
            return value;
        }
    }
}