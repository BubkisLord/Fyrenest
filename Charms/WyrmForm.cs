using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Modding;
using Modding.Delegates;
using On;
using UnityEngine;

namespace Fyrenest
{
	internal class WyrmForm : Charm
	{
		private WyrmForm()
		{
		}

		public override string Sprite
		{
			get
			{
				return "WyrmForm.png";
			}
		}

		public override string Name
		{
			get
			{
				return "Wyrm Form";
			}
		}

		public override string Description
		{
			get
			{
				return "This smooth, pale charm glistens while touching air.\n\nWhen worn, allows the bearer to fly, but at the cost of the bearer's nail to be weakened drastically.";
			}
		}

		public override int DefaultCost
		{
			get
			{
				return 4;
			}
		}

		public override string Scene
		{
			get
			{
				return "Fungus1_17";
			}
		}

		public override float X
		{
			get
			{
				return 71.5f;
			}
		}

		public override float Y
		{
			get
			{
				return 24.4f;
			}
		}

		public override CharmSettings Settings(SaveSettings s)
		{
			return s.WyrmForm;
		}

		public override void Hook()
		{
			On.HeroController.DoDoubleJump += AllowExtraJumps;
			On.HeroController.CanDoubleJump += AllowDoubleJump;
			ModHooks.GetPlayerIntHook += new GetIntProxy(this.BuffNail);
			ModHooks.SetPlayerBoolHook += new SetBoolProxy(this.UpdateNailDamageOnEquip);
		}

		private int BuffNail(string intName, int damage)
		{
			bool flag = intName == "nailDamage" && base.Equipped();
			if (flag)
			{
				damage /= 4;
			}
			return damage;
		}

		private bool UpdateNailDamageOnEquip(string boolName, bool value)
		{
			bool flag = boolName == string.Format("equippedCharm_{0}", base.Num);
			if (flag)
			{
				Fyrenest.UpdateNailDamage();
			}
			return value;
		}

		private bool AllowDoubleJump(On.HeroController.orig_CanDoubleJump orig, HeroController self)
		{
			bool PretendToHaveWings(string boolName, bool value) =>
				boolName == "hasDoubleJump" ? (value || Equipped()) : value;
			bool result3;
			if (Equipped())
			{
				ModHooks.GetPlayerBoolHook += PretendToHaveWings;
				bool result = orig.Invoke(self);
				ModHooks.GetPlayerBoolHook -= PretendToHaveWings;
				result3 = result;
			}
			else
			{
				bool result2 = orig.Invoke(self);
				result3 = result2;
			}
			return result3;
		}

		private void AllowExtraJumps(On.HeroController.orig_DoDoubleJump orig, HeroController self)
		{
			bool flag = !base.Equipped();
			if (flag)
			{
				orig.Invoke(self);
			}
			else
			{
				self.dJumpWingsPrefab.SetActive(false);
				self.dJumpFlashPrefab.SetActive(false);
				orig.Invoke(self);
				GameManager.instance.StartCoroutine(WyrmForm.RefreshWings());
			}
		}

		private static IEnumerator RefreshWings()
		{
			yield return new WaitUntil(() => !InputHandler.Instance.inputActions.jump.IsPressed);
			ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "doubleJumped", false);
			yield break;
		}

		public static readonly WyrmForm Instance = new WyrmForm();
	}
}
