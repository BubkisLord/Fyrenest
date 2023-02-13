using GlobalEnums;
using GravityMod;
using Modding.Utils;
using Object = UnityEngine.Object;

namespace Fyrenest
{
    internal class GravityCharm : Charm
    {
        private int count;
        public static readonly GravityCharm instance = new();
        public override bool Placeable => false;
        public override string Sprite => "GravityCharm.png";
        public override string Name => "Gravity Charm";
        public override string Description => "Makes all enemies effected by gravity.";
        public override int DefaultCost => 10;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private GravityCharm() { }

        public override CharmSettings Settings(SaveSettings s) => s.GravityCharm;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += AddGravityToAllGameObjects;
        }
        private void AddGravityToAllGameObjects()
        {
            if (Equipped())
            {
                count++;
                if (count > 30)
                {
                    count = 0;
                    foreach (var go in Object.FindObjectsOfType<Rigidbody2D>()
                                 .Select(rb2d => rb2d.gameObject)
                                 .Where(go => go.name != HeroController.instance.gameObject.name))
                    {
                        go.gameObject.GetOrAddComponent<GravityComponent>();
                    }
                }
            }
        }
    }
}