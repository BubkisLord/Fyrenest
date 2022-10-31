using Modding;
using UnityEngine;
using GlobalEnums;

namespace Fyrenest
{
    internal class Quickfall : Charm
    {
        public static readonly Quickfall instance = new();
        public override string Sprite => "Quickfall.png";
        public override string Name => "Quickfall";
        public override string Description => "This charm falls faster than it should when dropped.\n\nWhen worn, the bearer falls at a very fast rate.";
        public override int DefaultCost => 1;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;

        private Quickfall() {}

        public override CharmSettings Settings(SaveSettings s) => s.Quickfall;
        public override void Hook()
        {
            ModHooks.HeroUpdateHook += ChangeGravity;
            On.HeroController.ShouldHardLand += FallSoftly;
        }
        private bool FallSoftly(On.HeroController.orig_ShouldHardLand orig, HeroController self, Collision2D collision) =>
            orig(self, collision) && !Equipped();

        private static readonly HashSet<string> InventoryClosedStates = new()
        {
            "Init",
            "Init Enemy List",
            "Closed",
            "Can Open Inventory?"
        };
        

        private static bool InInventory()
        {
            var invState = GameManager.instance?.inventoryFSM?.Fsm.ActiveStateName;
            return invState != null && !InventoryClosedStates.Contains(invState);
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
            
            rb.gravityScale = (Equipped() && !InInventory() && HeroController.instance.transitionState == HeroTransitionState.WAITING_TO_TRANSITION && HeroController.instance.cState.falling) ? 1.58f : 0.79f;
        }
    }
}