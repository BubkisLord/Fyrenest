global using HutongGames.PlayMaker;
global using HutongGames;
global using HutongGames.Extensions;
global using HutongGames.Utility;
using Satchel;

namespace Fyrenest
{
    internal class MarkofStrength : Charm
    {
        public static readonly MarkofStrength instance = new();
        public override string Sprite => "MarkofStrength.png";
        public override string Name => "Mark of Strength";
        public override string Description => "Given by the Sisters of Battle to mark one's and strength in the face of danger.\n\nWhen worn, allows the bearer to throw their nail as a spinning blade. This ability replaces Vengeful Spirit and Shade Soul.";
        public override int DefaultCost => 2;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private MarkofStrength() {}

        public override CharmSettings Settings(SaveSettings s) => s.MarkofStrength;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += UpdateTriggers;
        }

        public void UpdateTriggers()
        {
            if (Equipped())
            {
                //Remove ShadeSoul
                FsmState castShadeSoul = HeroController.instance.spellControl.GetState("Fireball 2");
                castShadeSoul.RemoveAction(3);

                //Add Wind Scythe
                castShadeSoul.InsertCustomAction("Fireball 2", () => HandleTrigger(AbilityTrigger.Fireball), 3);
            }
            if (!Equipped())
            {
                //Add ShadeSoul back.
                FsmState castShadeSoul = HeroController.instance.spellControl.GetState("Fireball 2");
                FsmStateAction action = HeroController.instance.spellControl.GetAction("Fireball 2", 0);
                HeroController.instance.spellControl.InsertAction("Fireball 2", action, 0);
            }
            WindScythe.instance.Load();
            return;
        }
        private void HandleTrigger(AbilityTrigger trigger)
        {
            foreach (IAbility ability in Fyrenest.Abilities)
            {
                if (ability.Trigger == trigger) ability.Perform();
            }
        }
    }
}