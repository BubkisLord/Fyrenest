using Modding;

namespace Fyrenest
{
    internal class SoulSpell : Charm
    {
        public static readonly SoulSpell instance = new();
        public override bool Placeable => false;
        public override string Sprite => "SoulSpell.png";
        public override string Name => "Spell Soul";
        public override string Description => "Spells are much bigger, but you get half the soul from enemies.";
        public override int DefaultCost => 2;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private SoulSpell() { }

        public override CharmSettings Settings(SaveSettings s) => s.SoulSpell;

        public override List<(string, string, Action<PlayMakerFSM>)> FsmEdits => new()
        {
            ("Fireball(Clone)", "Fireball Control", EnlargeVengefulSpirit),
            ("Fireball2 Spiral(Clone)", "Fireball Control", EnlargeShadeSoul),
            ("Q Slam", "Hit Box Control", EnlargeDive),
            ("Q Slam 2", "Hit Box Control", EnlargeDDarkPart1),
            ("Q Mega", "Hit Box Control", EnlargeDDarkPart2),
            ("Scr Heads", "Hit Box Control", EnlargeScream),
            ("Scr Heads 2", "FSM", EnlargeShriek)
        };

        public override void Hook()
        {
            ModHooks.ObjectPoolSpawnHook += EnlargeFlukes;
            ModHooks.SoulGainHook += OnGetSoul;
        }

        private int OnGetSoul(int amount)
        {
            if (SoulHunger.instance.Equipped())
            {
                return amount;
            }
            else
            {
                if (RavenousSoul.instance.Equipped())
                {
                    return amount;
                }
                else {
                    amount /= 2;
                    return amount;
                }
            }
        }

        private static bool ShamanStoneEquipped() => PlayerData.instance.GetBool("equippedCharm_19");

        private static float EnlargementFactor() => 3;

        
        private void EnlargeVengefulSpirit(PlayMakerFSM fsm)
        {
            var setDamage = fsm.GetState("Set Damage");
            setDamage.ReplaceAction(0, () => {
                var scaleX = 1.0f;
                var scaleY = 1.0f;
                if (Equipped())
                {
                    var k = EnlargementFactor();
                    scaleX *= k;
                    scaleY *= k;
                }
                if (ShamanStoneEquipped())
                {
                    var k = EnlargementFactor();
                    var enlargex = k + 1.3f;
                    var enlargey = k + 1.6f;
                    scaleX *= enlargex;
                    scaleY *= enlargey;
                }
                fsm.gameObject.transform.localScale = new Vector3(scaleX, scaleY, 0f);
            });
            setDamage.ReplaceAction(6, () => { });
        }

        private void EnlargeShadeSoul(PlayMakerFSM fsm)
        {
            fsm.GetState("Set Damage").ReplaceAction(0, () => {
                var scale = 1.8f * (Equipped() ? EnlargementFactor() : 1);
                fsm.gameObject.transform.localScale = new Vector3(scale, scale, 0f);
            });
        }

        private Vector3? OriginalDiveSize = null;
        private Vector3? OriginalDDarkPart1Size = null;
        private Vector3? OriginalDDarkPart2Size = null;

        // We can't capture a ref parameter in a lambda, so we have to repeat ourselves
        // a little bit.
        private void EnlargeDive(PlayMakerFSM fsm)
        {
            var obj = fsm.gameObject;
            fsm.GetState("Activate").PrependAction(() => {
                RestoreOriginalSize(obj, ref OriginalDiveSize);
                EnlargeDivePartIfEquipped(obj);
            });
        }

        private void EnlargeDDarkPart1(PlayMakerFSM fsm)
        {
            var obj = fsm.gameObject;
            fsm.GetState("Activate").PrependAction(() => {
                RestoreOriginalSize(obj, ref OriginalDDarkPart1Size);
                EnlargeDivePartIfEquipped(obj);
            });
        }

        private void EnlargeDDarkPart2(PlayMakerFSM fsm)
        {
            var obj = fsm.gameObject;
            fsm.GetState("Activate").PrependAction(() => {
                RestoreOriginalSize(obj, ref OriginalDDarkPart2Size);
                EnlargeDivePartIfEquipped(obj);
            });
        }

        // Our increase to some object's sizes persists after the scream/dive
        // is done (presumably the game is reusing the object).
        // Keep the original size so we don't end up repeatedy embiggening it.
        private void RestoreOriginalSize(GameObject obj, ref Vector3? origSize)
        {
            if (origSize is Vector3 v)
            {
                obj.transform.localScale = v;
            }
            else
            {
                origSize = obj.transform.localScale;
            }
        }

        private void EnlargeDivePartIfEquipped(GameObject obj)
        {
            if (Equipped())
            {
                var vec = obj.transform.localScale;
                obj.transform.localScale = new Vector3(vec.x * EnlargementFactor(), vec.y * EnlargementFactor(), vec.z);
            }
        }

        private Vector3? OriginalScreamSize = null;
        private Vector3? OriginalShriekSize = null;

        private void EnlargeScream(PlayMakerFSM fsm)
        {
            var obj = fsm.gameObject;
            fsm.GetState("Activate").PrependAction(() => {
                RestoreOriginalSize(obj, ref OriginalScreamSize);
                EnlargeScreamIfEquipped(obj);
            });
        }

        private void EnlargeShriek(PlayMakerFSM fsm)
        {
            var obj = fsm.gameObject;
            fsm.GetState("Wait").PrependAction(() => {
                RestoreOriginalSize(obj, ref OriginalShriekSize);
                EnlargeScreamIfEquipped(obj);
            });
        }

        private void EnlargeScreamIfEquipped(GameObject obj)
        {
            if (Equipped())
            {
                var vec = obj.transform.localScale;
                var k = EnlargementFactor();
                obj.transform.localScale = new Vector3(vec.x * k, vec.y * k, vec.z);
            }
        }

        private GameObject EnlargeFlukes(GameObject obj)
        {
            if (obj.name.StartsWith("Spell Fluke") && Equipped())
            {
                var vec = obj.transform.localScale;
                var k = EnlargementFactor();
                obj.transform.localScale = new Vector3(vec.x * k, vec.y * k, vec.z);
            }
            return obj;
        }
    }
}