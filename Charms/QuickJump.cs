using Modding;
using UnityEngine;
using GlobalEnums;
using static UnityEngine.UI.CanvasScaler;

namespace Fyrenest
{
    internal class Quickjump : Charm
    {
        public static readonly Quickjump instance = new();
        public override string Sprite => "Quickjump.png";
        public override string Name => "Quickjump";
        public override string Description => "This charm falls faster than it should when dropped.\n\nWhen worn, the bearer jumps at a very fast rate, but cannot jump as high.";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private Quickjump() {}

        public override CharmSettings Settings(SaveSettings s) => s.Quickjump;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += ChangeGravity;
            On.HeroController.ShouldHardLand += FallSoftly;
        }

        private static readonly HashSet<string> InventoryClosedStates = new()
        {
            "Init",
            "Init Enemy List",
            "Closed",
            "Can Open Inventory?"
        };

        private bool FallSoftly(On.HeroController.orig_ShouldHardLand orig, HeroController self, Collision2D collision) =>
            orig(self, collision) && !Equipped();

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
            
            rb.gravityScale = (Equipped() && !InInventory() && HeroController.instance.transitionState == HeroTransitionState.WAITING_TO_TRANSITION && HeroController.instance.cState.jumping) ? 1.58f : 0.79f;
        }
    }
}