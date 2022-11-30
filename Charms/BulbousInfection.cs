using GlobalEnums;
using Modding.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fyrenest
{
    internal class BulbousInfection : Charm
    {
        private int count;
        public static readonly BulbousInfection instance = new();
        public override string Sprite => "BulbousInfection.png";
        public override string Name => "Bulbous Infection";
        public override string Description => "Increases levels of infection in enemies, making them bulbous and large.";
        public override int DefaultCost => 6;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private BulbousInfection() { }

        public override CharmSettings Settings(SaveSettings s) => s.BulbousInfection;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += MakeEnemiesBigger;
        }
        private void MakeEnemiesBigger()
        {
            if (BulbousInfection.instance.Equipped())
            {
                count++;
                if (count > 10)
                {
                    count = 0;
                    foreach (GameObject go in Object.FindObjectsOfType(typeof(DamageHero)))
                    {
                        if (go.name != HeroController.instance.name)
                        {
                            go.transform.localScale *= 4f;
                            var goSprite = go.GetComponent<SpriteRenderer>();
                            goSprite.transform.localScale *= 4f;
                            var goCollider = go.GetComponent<Collider2D>();
                            goCollider.transform.localScale *= 4f;
                        }
                    }
                }
            }
        }
    }
}