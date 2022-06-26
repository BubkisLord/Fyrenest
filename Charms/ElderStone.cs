namespace Fyrenest
{
    internal class ElderStone : Charm
    {
        public static readonly ElderStone Instance = new();
        public override string Sprite => "ElderStone.png";
        public override string Name => "ElderStone";
        public override string Description => "This magnificent charm bears the likeless of the Elderbug.\n\nEmbodies the frailness, the weakness, and the wisdom of the Elderbug into the bearer.";
        public override int DefaultCost => 5;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private ElderStone() {}
        
        public override CharmSettings Settings(SaveSettings s) => s.ElderStone;

        public override void Hook()
        {
            ModHooks.GetPlayerIntHook += BuffNail;
            ModHooks.SetPlayerBoolHook += UpdateNailDamageOnEquip;
            On.HeroController.Move += SpeedUp;
            ModHooks.HeroUpdateHook += ChangeGravity;
            ModHooks.TakeHealthHook += OnHit;
        }

        int EscapeDamage = 0;

        private int OnHit(int damage)
        {
            if (Equipped())
            {
                if (EscapeDamage == 4)
                {
                    return damage;
                }
                else
                {
                    EscapeDamage += 1;
                    damage = 0;
                    return damage;
                }
            }
            else
                return damage;
        }

        private void ChangeGravity()
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
            rb.gravityScale = (Equipped() && HeroController.instance.transitionState == GlobalEnums.HeroTransitionState.WAITING_TO_TRANSITION) ? 0.9f : 0.79f;
        }

        private void SpeedUp(On.HeroController.orig_Move orig, HeroController self, float speed)
        {
            if (Equipped() && speed != 0)
            {
                speed *= 0.5f;
            }
            orig(self, speed);
        }
        private int BuffNail(string intName, int damage)
        {
            if (intName == "nailDamage" && Equipped())
            {
                damage -= 10;
                if (damage < 1)
                {
                    damage = 1;
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