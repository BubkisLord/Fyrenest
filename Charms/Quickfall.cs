using Modding;
using UnityEngine;
using GlobalEnums;

namespace Fyrenest
{
    internal class Quickfall : Charm
    {
        public static readonly Quickfall Instance = new();
        public override string Sprite => "Quickfall.png";
        public override string Name => "Quickfall";
        public override string Description => "This charm falls faster than it should when dropped.\n\nWhen worn, the bearer falls at a very fast rate.";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private Quickfall() {}

        public override CharmSettings Settings(SaveSettings s) => s.Quickfall;

        public override void Hook()
        {
            ModHooks.CharmUpdateHook += ChangeGravity;
        }

        private void ChangeGravity(PlayerData data, HeroController hero)
        {
            var rb = hero.gameObject.GetComponent<Rigidbody2D>();
            //decide what gravity it should be.
            if (Equipped())
            {
                if (Slowfall.Instance.Equipped())
                {
                    rb.gravityScale = 0.79f;
                }
                else
                {
                    rb.gravityScale = 1.4f;
                }
            }
            else
            {
                if (Slowfall.Instance.Equipped())
                {
                    rb.gravityScale = 0.35f;
                }
                else
                {
                    rb.gravityScale = 0.79f;
                }
            }
            return;
        }
    }
}