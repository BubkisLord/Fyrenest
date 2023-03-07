using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using Modding;
using Satchel;
using UnityEngine;

namespace Fyrenest
{
	internal class Fyrechild : Charm
	{
		public static readonly Fyrechild instance = new();
		public override bool Placeable => false;
		public override string Sprite => "Fyrechild.png";
		public override string Name => "Grimm's Flame";
		public override string Description => "A charm radiating with strength, also bearing some likeness to the Grimmchild.\n\nWhen worn, the bearer's Grimmchild companion will grow in strength, speed, power, and skill. These effects increase the more the Grimmchild's master attacks foes.";
		public override int DefaultCost => 3;
		public override string Scene => "Town";
		public override float X => 0f;
		public override float Y => 0f;

        private List<GameObject> _companions = new List<GameObject>();

        public override CharmSettings Settings(SaveSettings s) => s.Fyrechild;
		public override void Hook() => ModHooks.HeroUpdateHook += this.UpdateHook;
		public int CompanionAmount => this._companions.Count((GameObject x) => x.activeSelf);

		private void UpdateHook()
		{
			if (Equipped())
			{
				List<GameObject> foundCompanions = GameObject.FindGameObjectsWithTag("Grimmchild").ToList<GameObject>();
				try
				{
					foreach (GameObject companion in foundCompanions)
					{
						//PlayMakerFSM companionFsm = FSMUtility.LocateMyFSM(companion, "Control");
						companion.gameObject.transform.SetScaleX(2f);
						companion.gameObject.transform.SetScaleY(2f);
						this._companions.Add(companion);
					}
				}
				catch (Exception exception)
				{
					Fyrenest.instance.LogError("Error: " + exception.Message);
				}
				DamageEnemies damageEnemies = GameObjectUtils.GetAddComponent<DamageEnemies>(foundCompanions[0]);
				damageEnemies.attackType = AttackTypes.Spell;
				damageEnemies.circleDirection = true;
				damageEnemies.damageDealt = 1;
				damageEnemies.ignoreInvuln = false;
				damageEnemies.direction = 180f;
				damageEnemies.moveDirection = true;
				damageEnemies.magnitudeMult = 1f;
				damageEnemies.specialType = SpecialTypes.None;
			}
			else
				return;
		}
	}
}
