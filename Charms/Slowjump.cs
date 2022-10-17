using Modding;
using UnityEngine;
using GlobalEnums;
using static UnityEngine.UI.CanvasScaler;

namespace Fyrenest
{
    internal class Slowjump : Charm
    {
        public static readonly Slowjump instance = new();
        public override string Sprite => "Slowjump.png";
        public override string Name => "Slowjump";
        public override string Description => "This charm falls slower than it should when dropped.\n\nWhen worn, the bearer falls at a very slow rate.";
        public override int DefaultCost => 1;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private Slowjump() {}

        public override CharmSettings Settings(SaveSettings s) => s.Slowjump;

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
            rb.gravityScale = (Equipped() && !InInventory() && HeroController.instance.transitionState == HeroTransitionState.WAITING_TO_TRANSITION) ? 0.01f : 0.79f;
        }
    }
}