using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Modding;
using System.Collections;
using Modding.Delegates;
using Satchel;

namespace Fyrenest
{
    internal class ShellShield : Charm
    {
        public static readonly ShellShield instance = new();
        public override bool Placeable => false;
        public override string Sprite => "ShellShield.png";
        public override string Name => "Shell Shield";
        public override string Description => "This magnificent charm bears the likeless of the one, the only Zote The Mighty!\n\nEmbodies the might, the strength, the sheer power of Zote The Mighty into the bearer.";
        public override int DefaultCost => 2;
        public override string Scene => "Town";
        public override float X => 0f;
        public override float Y => 0f;
        private ShellShield() { }

        public override CharmSettings Settings(SaveSettings s) => s.ShellShield;
        public override List<(string, string, Action<PlayMakerFSM>)> FsmEdits => new()
        {
            ("Charm Effects", "Spawn Orbit Shield", DoubleDreamshield)
        };
        public bool Trigger(string boolname, bool _bool)
        {
            if (PlayerData.instance.equippedCharm_38 && Equipped() && boolname == "equippedCharm_38")
            {
                SpawnDuplicateDreamshield();
                DespawnDuplicateDreamshield();
            }
            return _bool;
        }

        private GameObject DreamshieldPrefab;
        private GameObject DuplicateDreamshield;
        private GameObject DuplicateDreamshield2;
        private GameObject DuplicateDreamshield3;
        private GameObject DuplicateDreamshield4;
        private GameObject DuplicateDreamshield5;

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
                    dupeShield.transform.Rotate(0, 0, 120);
                    DuplicateDreamshield = dupeShield;
                    var dupeShield2 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                    dupeShield2.transform.Rotate(0, 0, 240);
                    DuplicateDreamshield2 = dupeShield2;
                    if (SoulSpell.instance.Equipped())
                    {
                        var dupeShield3 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                        dupeShield3.transform.Rotate(0, 0, 60);
                        DuplicateDreamshield2 = dupeShield3;
                        var dupeShield4 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                        dupeShield4.transform.Rotate(0, 0, 180);
                        DuplicateDreamshield2 = dupeShield4;
                        var dupeShield5 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                        dupeShield5.transform.Rotate(0, 0, 300);
                        DuplicateDreamshield2 = dupeShield5;
                    }
                }
            });
            fsm.GetState("Send Slash Event").AppendAction(() =>
            {
                if (DuplicateDreamshield != null)
                {
                    SendEvent(DuplicateDreamshield, "Control", "SLASH");
                }
                if (DuplicateDreamshield2 != null)
                {
                    SendEvent(DuplicateDreamshield2, "Control", "SLASH");
                }
                if (DuplicateDreamshield3 != null)
                {
                    SendEvent(DuplicateDreamshield3, "Control", "SLASH");
                }
                if (DuplicateDreamshield4 != null)
                {
                    SendEvent(DuplicateDreamshield4, "Control", "SLASH");
                }
                if (DuplicateDreamshield5 != null)
                {
                    SendEvent(DuplicateDreamshield5, "Control", "SLASH");
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
            //spawn the 2nd dreamshield
            var dupeShield = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
            if (SoulSpell.instance.Equipped()) dupeShield.transform.localScale *= 1.5f;
            // Put the 2nd shield 1/3 across
            var origShield = GameObject.FindWithTag("Orbit Shield");
            if (origShield != null)
            {
                dupeShield.transform.rotation = origShield.transform.rotation;
                if (SoulSpell.instance.Equipped()) origShield.transform.localScale *= 1.5f;
                DreamshieldPrefab.layer = 17;
                var damageEnemies = DreamshieldPrefab.GetAddComponent<DamageEnemies>();
                damageEnemies.attackType = AttackTypes.Spell;
                damageEnemies.circleDirection = true;
                if (SoulSpell.instance.Equipped()) damageEnemies.damageDealt *= 2;
                if (PlayerData.instance.equippedCharm_19) damageEnemies.damageDealt *= 2;
                damageEnemies.ignoreInvuln = false;
                damageEnemies.direction = 180f;
                damageEnemies.moveDirection = true;
                damageEnemies.magnitudeMult = 1f;
                damageEnemies.specialType = SpecialTypes.None;
            }
            dupeShield.transform.Rotate(0, 0, 120);
            DuplicateDreamshield = dupeShield;


            //spawn the 3rd dreamshield
            var dupeShield2 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
            if (SoulSpell.instance.Equipped()) dupeShield2.transform.localScale *= 1.5f;
            // Put the 3rd shield 2/3 across
            if (origShield != null)
            {
                dupeShield2.transform.rotation = origShield.transform.rotation;
            }
            dupeShield2.transform.Rotate(0, 0, 240);
            DuplicateDreamshield2 = dupeShield2;
            if (SoulSpell.instance.Equipped())
            {
                //spawn the 4th dreamshield
                var dupeShield3 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                // Put the 3rd shield 2/3 across
                if (origShield != null)
                {
                    dupeShield3.transform.rotation = origShield.transform.rotation;
                }
                dupeShield3.transform.Rotate(0, 0, 60);
                DuplicateDreamshield3 = dupeShield3;

                //spawn the 5th dreamshield
                var dupeShield4 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                if (origShield != null)
                {
                    dupeShield4.transform.rotation = origShield.transform.rotation;
                }
                dupeShield4.transform.Rotate(0, 0, 180);
                DuplicateDreamshield4 = dupeShield4;

                //spawn the 6th dreamshield
                var dupeShield5 = DreamshieldPrefab.Spawn(Vector3.zero, Quaternion.Euler(Vector3.up));
                if (origShield != null)
                {
                    dupeShield5.transform.rotation = origShield.transform.rotation;
                }
                dupeShield5.transform.Rotate(0, 0, 300);
                DuplicateDreamshield5 = dupeShield5;
            }
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
            if (DuplicateDreamshield2 != null)
            {
                var shield = DuplicateDreamshield2;
                SendEvent(shield.transform.Find("Shield").gameObject, "Shield Hit", "DISAPPEAR");
                IEnumerator DelayedDestroy()
                {
                    yield return new WaitForSeconds(0.5f);
                    shield.Recycle();
                }
                GameManager.instance.StartCoroutine(DelayedDestroy());
                DuplicateDreamshield2 = null;
            }
            if (DuplicateDreamshield3 != null)
            {
                var shield = DuplicateDreamshield3;
                SendEvent(shield.transform.Find("Shield").gameObject, "Shield Hit", "DISAPPEAR");
                IEnumerator DelayedDestroy()
                {
                    yield return new WaitForSeconds(0.5f);
                    shield.Recycle();
                }
                GameManager.instance.StartCoroutine(DelayedDestroy());
                DuplicateDreamshield3 = null;
            }
            if (DuplicateDreamshield4 != null)
            {
                var shield = DuplicateDreamshield4;
                SendEvent(shield.transform.Find("Shield").gameObject, "Shield Hit", "DISAPPEAR");
                IEnumerator DelayedDestroy()
                {
                    yield return new WaitForSeconds(0.5f);
                    shield.Recycle();
                }
                GameManager.instance.StartCoroutine(DelayedDestroy());
                DuplicateDreamshield4 = null;
            }
            if (DuplicateDreamshield5 != null)
            {
                var shield = DuplicateDreamshield5;
                SendEvent(shield.transform.Find("Shield").gameObject, "Shield Hit", "DISAPPEAR");
                IEnumerator DelayedDestroy()
                {
                    yield return new WaitForSeconds(0.5f);
                    shield.Recycle();
                }
                GameManager.instance.StartCoroutine(DelayedDestroy());
                DuplicateDreamshield5 = null;
            }
        }
    }
}