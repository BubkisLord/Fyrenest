namespace Fyrenest
{
	internal class TripleJump : Charm
	{
		public TripleJump()
		{
		}

		public override string Sprite
		{
			get
			{
				return "TripleJump.png";
			}
		}

		public override string Name
		{
			get
			{
				return "Triple Jump";
			}
		}

		public override string Description
		{
			get
			{
				return "Resembles silvery wings, in which the following words are enscribed: 'Please, don't forget about me... Not again.'\n\nWhen worn, allows the bearer to use monarch wings three times in quick succession.";
			}
		}

		public override int DefaultCost
		{
			get
			{
				return 3;
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
			return s.TripleJump;
		}

		public override void Hook()
		{
			On.HeroController.DoDoubleJump += AllowExtraJumps;
			On.HeroController.CanDoubleJump += AllowDoubleJump;
            ModHooks.HeroUpdateHook += RefreshJumps;
		}

        private void RefreshJumps()
        {
            if (HeroController.instance.CheckTouchingGround())
            {
				jumpTimes = 0;
            }
			return;
        }

        public int jumpTimes = 0;

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
				GameManager.instance.StartCoroutine(TripleJump.RefreshWings());
			}
		}

		private static IEnumerator RefreshWings()
		{
			if (instance.jumpTimes < 2)
			{
				yield return new WaitUntil(() => !InputHandler.Instance.inputActions.jump.IsPressed);
				ReflectionHelper.SetField<HeroController, bool>(HeroController.instance, "doubleJumped", false);
				instance.jumpTimes += 1;
				yield break;
			}
		}

		public static readonly TripleJump instance = new TripleJump();
	}
}
