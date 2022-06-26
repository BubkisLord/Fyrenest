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

        private void ChangeGravity(PlayerData playerData, HeroController hero)
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
            if(Equipped() && !Slowfall.Instance.Equipped()) rb.gravityScale =  4.4f;
            if(Equipped() && Slowfall.Instance.Equipped()) rb.gravityScale =  0.79f;
            if(!Equipped() && Slowfall.Instance.Equipped()) rb.gravityScale =  0.25f;
            if(!Equipped() && !Slowfall.Instance.Equipped()) rb.gravityScale =  0.79f;
            return;
        }
    }
}