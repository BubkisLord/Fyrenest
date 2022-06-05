using Modding;
using UnityEngine;
using GlobalEnums;

namespace CharmMod
{
    internal class Slowfall : Charm
    {
        public static readonly Slowfall Instance = new();
        public override string Sprite => "Slowfall.png";
        public override string Name => "Slowfall";
        public override string Description => "Desc";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private Slowfall() {}

        public override CharmSettings Settings(SaveSettings s) => s.Slowfall;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += ChangeGravity;
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
            rb.gravityScale = (Equipped() && HeroController.instance.transitionState == HeroTransitionState.WAITING_TO_TRANSITION) ? 0.2f : 0.79f;
        }
    }
}