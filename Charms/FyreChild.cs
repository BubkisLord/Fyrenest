// For now, Grimm's Flame has been disabled, as it is WAYYY too overpowered.



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using HutongGames.PlayMaker;
//using Modding;
//using Satchel;
//using UnityEngine;

//namespace Fyrenest
//{
//	internal class Fyrechild : Charm
//    {
//        public static readonly Fyrechild instance = new();
//        public override bool Placeable => false;
//        public override string Sprite => "Fyrechild.png";
//        public override string Name => "Grimm's Flame";
//        public override string Description => "A charm radiating with strength, also bearing some likeness to the Grimmchild.\n\nWhen worn, the bearer's Grimmchild companion will grow in strength, speed, power, and skill. These effects increase the more the Grimmchild's master attacks foes.";
//        public override int DefaultCost => 3;
//        public override string Scene => "Town";
//        public override float X => 0f;
//        public override float Y => 0f;

//		public override CharmSettings Settings(SaveSettings s)
//		{
//			return s.Fyrechild;
//		}
//		public override void Hook()
//		{
//			ModHooks.HeroUpdateHook += this.UpdateHook;
//		}
//		public int CompanionAmount
//		{
//			get
//			{
//				return this._companions.Count((GameObject x) => x.activeSelf);
//			}
//		}
//		private void UpdateHook()
//		{
//			if (Equipped())
//			{
//				List<GameObject> foundCompanions = GameObject.FindGameObjectsWithTag("Grimmchild").ToList<GameObject>();
//				try
//				{
//					foreach (GameObject companion in foundCompanions)
//					{
//						bool flag2 = this._companions.Contains(companion);
//						if (!flag2)
//						{
//							bool flag3 = companion.tag.Equals("Grimmchild");
//							if (flag3)
//							{
//								PlayMakerFSM companionFsm = FSMUtility.LocateMyFSM(companion, "Control");
//								FsmState fsmState = companionFsm.GetState("Antic");
//								bool flag4 = companion.tag.Equals("Grimmchild");
//								if (flag4)
//								{
//									companionFsm = FSMUtility.LocateMyFSM(companion, "Control");
//									fsmState = companionFsm.GetState("Antic");
//									companion.gameObject.transform.SetScaleX(2f);
//									companion.gameObject.transform.SetScaleY(2f);
//								}
//								this._companions.Add(companion);
//							}
//						}
//					}
//				}
//				catch (Exception exception)
//				{
//					Fyrenest.instance.LogError("Error: " + exception.Message);
//				}
//				DamageEnemies damageEnemies = GameObjectUtils.GetAddComponent<DamageEnemies>(foundCompanions[0]);
//				damageEnemies.attackType = AttackTypes.Spell;
//				damageEnemies.circleDirection = true;
//				damageEnemies.damageDealt = 21;
//				damageEnemies.ignoreInvuln = false;
//				damageEnemies.direction = 180f;
//				damageEnemies.moveDirection = true;
//				damageEnemies.magnitudeMult = 1f;
//				damageEnemies.specialType = SpecialTypes.None;
//			}
//			else
//				return;
//		}
//		private List<GameObject> _companions = new List<GameObject>();
//	}
//}
