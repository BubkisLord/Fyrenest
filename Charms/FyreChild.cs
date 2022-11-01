using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using Modding;
using Satchel;
using UnityEngine;

namespace Fyrenest
{
	// Token: 0x0200000A RID: 10
	internal class Fyrechild : Charm
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00005D0E File Offset: 0x00003F0E
		public override string Sprite
		{
			get
			{
				return "Fyrechild.png";
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00005D15 File Offset: 0x00003F15
		public override string Name
		{
			get
			{
				return "Grimm's Flame";
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00005D1C File Offset: 0x00003F1C
		public override string Description
		{
			get
			{
				return "A charm radiating with strength, and seems to bear some likeness to the Grimmchild.\n\nWhen worn, the bearer's Grimmchild companion will grow in strength, speed, power, and skill. These effects increase the more the Grimmchild's master attacks foes.";
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00005D23 File Offset: 0x00003F23
		public override int DefaultCost
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00005D26 File Offset: 0x00003F26
		public override string Scene
		{
			get
			{
				return "Town";
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00005D2D File Offset: 0x00003F2D
		public override float X
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00005D34 File Offset: 0x00003F34
		public override float Y
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005D3B File Offset: 0x00003F3B
		private Fyrechild()
		{
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005D50 File Offset: 0x00003F50
		public override CharmSettings Settings(SaveSettings s)
		{
			return s.Fyrechild;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005D58 File Offset: 0x00003F58
		public override void Hook()
		{
			ModHooks.HeroUpdateHook += this.UpdateHook;
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00005D6D File Offset: 0x00003F6D
		public int CompanionAmount
		{
			get
			{
				return this._companions.Count((GameObject x) => x.activeSelf);
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005D9C File Offset: 0x00003F9C
		private void UpdateHook()
		{
			if (Equipped())
			{
				List<GameObject> foundCompanions = GameObject.FindGameObjectsWithTag("Grimmchild").ToList<GameObject>();
				try
				{
					foreach (GameObject companion in foundCompanions)
					{
						bool flag2 = this._companions.Contains(companion);
						if (!flag2)
						{
							bool flag3 = companion.tag.Equals("Grimmchild");
							if (flag3)
							{
								PlayMakerFSM companionFsm = FSMUtility.LocateMyFSM(companion, "Control");
								FsmState fsmState = companionFsm.GetState("Antic");
								bool flag4 = companion.tag.Equals("Grimmchild");
								if (flag4)
								{
									companionFsm = FSMUtility.LocateMyFSM(companion, "Control");
									fsmState = companionFsm.GetState("Antic");
									companion.gameObject.transform.SetScaleX(2f);
									companion.gameObject.transform.SetScaleY(2f);
								}
								this._companions.Add(companion);
							}
						}
					}
				}
				catch (Exception exception)
				{
					Fyrenest.instance.LogError("Error: " + exception.Message);
				}
				DamageEnemies damageEnemies = GameObjectUtils.GetAddComponent<DamageEnemies>(foundCompanions[0]);
				damageEnemies.attackType = AttackTypes.Spell;
				damageEnemies.circleDirection = true;
				damageEnemies.damageDealt = 21;
				damageEnemies.ignoreInvuln = false;
				damageEnemies.direction = 180f;
				damageEnemies.moveDirection = true;
				damageEnemies.magnitudeMult = 1f;
				damageEnemies.specialType = SpecialTypes.None;
			}
			else
				return;
		}

		// Token: 0x04000027 RID: 39
		public static readonly Fyrechild instance = new Fyrechild();

		// Token: 0x04000028 RID: 40
		private List<GameObject> _companions = new List<GameObject>();
	}
}
