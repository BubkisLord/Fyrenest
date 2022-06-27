using Satchel;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Fyrenest {
    public class WindScythe : IAbility {
        public static readonly WindScythe Instance = new();
        public string Name => "Wind Scythe";
        public AbilityTrigger Trigger => AbilityTrigger.Fireball;

        private GameObject scythePreload;

        public void Load() {
            scythePreload = Fyrenest.Preloads["GG_Mantis_Lords"]["Shot Mantis Lord"];
            scythePreload.RemoveComponent<DamageHero>();
            scythePreload.layer = 17;
            var damageEnemies = scythePreload.GetAddComponent<DamageEnemies>();
            damageEnemies.attackType = AttackTypes.Spell;
            damageEnemies.circleDirection = true;
            if (PlayerData.instance.fireballLevel == 1) damageEnemies.damageDealt = (PlayerData.instance.equippedCharm_19) ? (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) * 2) : PlayerData.instance.GetInt(nameof(PlayerData.nailDamage));
            if (SoulSpell.Instance.Equipped()) damageEnemies.damageDealt *= 2;
            damageEnemies.ignoreInvuln = false;
            damageEnemies.direction = 180f;
            damageEnemies.moveDirection = true;
            damageEnemies.magnitudeMult = 1f;
            damageEnemies.specialType = SpecialTypes.None;
            if (SoulSpell.Instance.Equipped())
            {
                scythePreload.transform.SetScaleX(3f);
                scythePreload.transform.SetScaleY(3f);
            }
        }

        public void Perform()
        {
            if (MarkofStrength.Instance.Equipped())
            {
                var scythe = GameObject.Instantiate(scythePreload, HeroController.instance.transform.position, Quaternion.identity);
                scythe.SetActive(true);

                // Figure out which scythe mode to do
                string dir = (HeroController.instance.cState.facingRight) ? "L" : "R";
                string type = (PlayerData.instance.equippedCharm_19) ? "WIDE" : "NARROW";

                scythe.LocateMyFSM("Control").SendEvent($"{type} {dir}");
            }
        }
    }
}