using HutongGames.PlayMaker.Actions;

namespace CharmMod
{
    internal class BetterCDash : Charm
    {
        public static readonly BetterCDash Instance = new();
        public override string Sprite => "BetterCDash.png";
        public override string Name => "Enraged Crystal Dash";
        public override string Description => "A purple charm resembling the Sprintmaster charm.\n\nMakes the Crystal Dash more powerful, doing more damage, and moves with powerful speed.";
        public override int DefaultCost => 3;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private BetterCDash() { }

        public override CharmSettings Settings(SaveSettings s) => s.BetterCDash;

        public override List<(string, string, Action<PlayMakerFSM>)> FsmEdits => new()
        {
            ("SD Burst", "damages_enemy", IncreaseCDashDamage),
            ("SuperDash Damage", "damages_enemy", IncreaseCDashDamage),
            ("Knight", "Superdash", IncreaseCDashSpeed)
        };

        private const int DamageWhenEquipped = 50;
        private const int DamageWhenUnequipped = 10;

        private void IncreaseCDashDamage(PlayMakerFSM fsm)
        {
            var sendEvent = fsm.GetState("Send Event");
            // Guard against the IntCompare action not being there. That sometimes happens,
            // even though the code works. This is only to keep it from flooding modlog
            // with spurious exceptions.
            var damage = (sendEvent?.Actions[0] as IntCompare)?.integer1;
            if (damage != null && sendEvent != null)
            {
                sendEvent.PrependAction(() => {
                    damage.Value = Equipped() ? DamageWhenEquipped : DamageWhenUnequipped;
                });
            }
        }
        private const int SpeedWhenEquipped = 60;
        private const int SpeedWhenUnequipped = 30;

        private void IncreaseCDashSpeed(PlayMakerFSM fsm)
        {
            var left = fsm.GetState("Left");
            var speed = (left.Actions[0] as SetFloatValue).floatVariable;
            void SetLeftSpeed()
            {
                speed.Value = -(Equipped() ? SpeedWhenEquipped : SpeedWhenUnequipped);
            }
            void SetRightSpeed()
            {
                speed.Value = Equipped() ? SpeedWhenEquipped : SpeedWhenUnequipped;
            }
            left.ReplaceAction(0, SetLeftSpeed);
            fsm.GetState("Right").ReplaceAction(0, SetRightSpeed);
            fsm.GetState("Enter L").ReplaceAction(0, SetLeftSpeed);
            fsm.GetState("Enter R").ReplaceAction(0, SetRightSpeed);
        }
    }
}