/*using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Modding;
using System.Collections;

namespace Fyrenest
{
    internal class ShellShield : Charm
    {
        public static readonly ShellShield Instance = new();
        public override string Sprite => "ShellShield.png";
        public override string Name => "Shell Shield";
        public override string Description => "This magnificent charm bears the likeless of the one, the only Zote The Mighty!\n\nEmbodies the might, the strength, the sheer power of Zote The Mighty into the bearer.";
        public override int DefaultCost => 3;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;
        private ShellShield() { }

        public override CharmSettings Settings(SaveSettings s) => s.ShellShield;
        private GameObject DreamshieldPrefab;
        private GameObject DuplicateDreamshield;

        private void DoubleDreamshield(PlayMakerFSM fsm)
        {
            var spawn = fsm.GetState("Spawn");
            var origSpawn = spawn.Actions[2] as SpawnObjectFromGlobalPool;
            spawn.SpliceAction(3, () =>
            {
                if (DreamshieldPrefab == null)
                {
                    DreamshieldPrefab = origSpawn.gameObject.Value;
                }
                if (Equipped())
                {
                    var dupeShield = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                    dupeShield.transform.Rotate(0, 0, 180);
                    DuplicateDreamshield = dupeShield;
                }
            });
            fsm.GetState("Send Slash Event").AppendAction(() =>
            {
                if (DuplicateDreamshield != null)
                {
                    SendEvent(DuplicateDreamshield, "Control", "SLASH");
                }
            });
        }
        private static void SendEvent(GameObject obj, string fsmName, string eventName)
        {
            FSMUtility.LocateMyFSM(obj, fsmName).Fsm.Event(new FsmEventTarget()
            {
                target = FsmEventTarget.EventTarget.GameObject,
                gameObject = new FsmOwnerDefault() { GameObject = new FsmGameObject(obj) }
            }, eventName);
        }
        private void SpawnDuplicateDreamshield()
        {
            if (DreamshieldPrefab == null)
            {
                return;
            }
            var dupeShield = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
            // Put the duplicate shield on the opposite side from the original.
            var origShield = GameObject.FindWithTag("Orbit Shield");
            if (origShield != null)
            {
                dupeShield.transform.rotation = origShield.transform.rotation;
            }
            dupeShield.transform.Rotate(0, 0, 180);
            DuplicateDreamshield = dupeShield;
        }

        private void DespawnDuplicateDreamshield()
        {
            if (DuplicateDreamshield != null)
            {
                var shield = DuplicateDreamshield;
                SendEvent(shield.transform.Find("Shield").gameObject, "Shield Hit", "DISAPPEAR");
                IEnumerator DelayedDestroy()
                {
                    // not sure how long this should be exactly. Orbit Shield's Control FSM says 1 second, but
                    // that's clearly too long; the despawn animation doesn't actually end with the shield gone.
                    yield return new WaitForSeconds(0.5f);
                    shield.Recycle();
                }
                GameManager.instance.StartCoroutine(DelayedDestroy());
                DuplicateDreamshield = null;
            }
        }
    }
}
*/