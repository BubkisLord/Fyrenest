global using System;
global using System.IO;
global using System.Collections;
global using Modding;
global using UnityEngine;
global using SFCore;
global using System.Collections.Generic;
global using System.Linq;

namespace CharmMod
{
    public class CharmMod : Mod, ILocalSettings<SaveSettings>
    {
        private static List<Charm> Charms = new()
        {
            Quickfall.Instance,
            Slowfall.Instance,
            SturdyNail.Instance,
            BetterCDash.Instance,
            GlassCannon.Instance,
            HKBlessing.Instance,
            HuntersMark.Instance,
            PowerfulDash.Instance,
            HealthyShell.Instance,
            DoubleDash.Instance,
            SoulSpeed.Instance,
            SoulSpell.Instance,
            SoulHunger.Instance,
            RavenousSoul.Instance,
            SoulSwitch.Instance,
            GeoSwitch.Instance,
            WealthyAmulet.Instance,
            SoulSlow.Instance,
            SlowTime.Instance,
            SpeedTime.Instance
        };

        public int NewCharms = Charms.Count; //STARTS AT 0 (almost definately)
        public int OldCharms = 40; //STARTS AT 1 (for some reason)

        internal static CharmMod Instance;

        private Dictionary<string, Func<bool, bool>> BoolGetters = new();
        private Dictionary<string, Action<bool>> BoolSetters = new();
        private Dictionary<string, Func<int, int>> IntGetters = new();
        private Dictionary<(string, string), Action<PlayMakerFSM>> FSMEdits = new();
        private List<(int Period, Action Func)> Tickers = new();

        public override void Initialize()
        {
            Log("Initializing");
            Instance = this;
            foreach (var charm in Charms)
            {
                var num = CharmHelper.AddSprites(EmbeddedSprites.Get(charm.Sprite))[0];
                charm.Num = num;
                var settings = charm.Settings;
                this.IntGetters[string.Format("charmCost_{0}", num)] = ((int _) => settings(this.Settings).Cost);
                AddTextEdit($"CHARM_NAME_{num}", "UI", charm.Name);
                AddTextEdit($"CHARM_DESC_{num}", "UI", () => charm.Description);
                BoolGetters[$"equippedCharm_{num}"] = _ => settings(Settings).Equipped;
                BoolSetters[$"equippedCharm_{num}"] = value => settings(Settings).Equipped = value;
                BoolGetters[$"gotCharm_{num}"] = _ => settings(Settings).Got;
                BoolSetters[$"gotCharm_{num}"] = value => settings(Settings).Got = value;
                BoolGetters[$"newCharm_{num}"] = _ => settings(Settings).New;
                BoolSetters[$"newCharm_{num}"] = value => settings(Settings).New = value;
                charm.Hook();
                foreach (var edit in charm.FsmEdits)
                {
                    AddFsmEdit(edit.obj, edit.fsm, edit.edit);
                }
                Tickers.AddRange(charm.Tickers);
            }
            for (var i = 1; i <= 40; i++)
            {
                var num = i; // needed for closure to capture a different copy of the variable each time
                BoolGetters[$"equippedCharm_{num}"] = value => value;
                this.IntGetters[string.Format("charmCost_{0}", num)] = ((int value) => value);
            }

            ModHooks.GetPlayerBoolHook += ReadCharmBools;
            ModHooks.SetPlayerBoolHook += WriteCharmBools;
            ModHooks.GetPlayerIntHook += ReadCharmCosts;
            ModHooks.LanguageGetHook += GetCharmStrings;
            // This will run after Rando has already set up its item placements.
            On.PlayMakerFSM.OnEnable += EditFSMs;
            On.PlayerData.CountCharms += CountOurCharms;
            ModHooks.NewGameHook += GiveCharms;
            ModHooks.HeroUpdateHook += OnUpdate;
            ModHooks.SoulGainHook += GrantAllOurCharmsOnSoul;
            
            StartTicking();
            if (ModHooks.GetMod("DebugMod") != null)
            {
                DebugModHook.GiveAllCharms(() => {
                    GrantAllOurCharms();
                    PlayerData.instance.CountCharms();
                });
            }
        }
        // breaks infinite loop when reading equippedCharm_X

        private Dictionary<(string Key, string Sheet), Func<string>> TextEdits = new();

        internal void AddTextEdit(string key, string sheetName, string text)
        {
            TextEdits.Add((key, sheetName), () => text);
        }

        internal void AddTextEdit(string key, string sheetName, Func<string> text)
        {
            TextEdits.Add((key, sheetName), text);
        }
        private void GiveCharms()
        {
            GrantAllOurCharms();
        }

        public override string GetVersion() => "8.30.3";

        internal SaveSettings Settings = new();



        private bool ReadCharmBools(string boolName, bool value)
        {
            if (BoolGetters.TryGetValue(boolName, out var f))
            {
                return f(value);
            }
            return value;
        }

        private bool WriteCharmBools(string boolName, bool value)
        {
            if (BoolSetters.TryGetValue(boolName, out var f))
            {
                f(value);
            }
            return value;
        }

        private int ReadCharmCosts(string intName, int value)
        {
            Func<int, int> cost;
            bool flag = this.IntGetters.TryGetValue(intName, out cost);
            int result;
            if (flag)
            {
                result = cost(value);
            }
            else
            {
                result = value;
            }
            return result;
        }

        private string GetCharmStrings(string key, string sheetName, string orig)
        {
            if (TextEdits.TryGetValue((key, sheetName), out var text))
            {
                return text();
            }
            return orig;
        }
        private void OnUpdate()
        {
            //give charms when certain things are done.

            if (PlayerData.instance.colosseumBronzeCompleted) Quickfall.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.colosseumSilverCompleted) Slowfall.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasShadowDash) PowerfulDash.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasNailArt) SturdyNail.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasHuntersMark) HuntersMark.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasDreamGate) SoulHunger.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasDreamNail) SoulSlow.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasSuperDash && PlayerData.instance.gaveSlykey) BetterCDash.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.killedHollowKnight) HKBlessing.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.hasKingsBrand) HealthyShell.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.killedHollowKnightPrime) GlassCannon.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.bankerAccountPurchased) WealthyAmulet.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.colosseumGoldCompleted) RavenousSoul.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.canOvercharm) DoubleDash.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.collectorDefeated) SoulSpell.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.grubsCollected > 10) SlowTime.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.fatGrubKing) SpeedTime.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.mageLordDreamDefeated) GeoSwitch.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.killedMageLord) SoulSwitch.Instance.Settings(Settings).Got = true;
            if (PlayerData.instance.nailsmithConvoArt) SoulSpeed.Instance.Settings(Settings).Got = true;

            //end
            //ik it is messy, but what else is there to do. Also, if u are seeing this code and think that there is a more appropriate time to give the charm, DM me on discord. I am BubkisLord#5187


            if (PlayerData.instance.maxHealth < 1)
            {
                HeroController.instance.AddToMaxHealth(1);
            }
            foreach (Charm charm in CharmMod.Charms)
            {
                charm.Settings(this.Settings).Cost = charm.DefaultCost;
            }
        }

        internal void AddFsmEdit(string objName, string fsmName, Action<PlayMakerFSM> edit)
        {
            var key = (objName, fsmName);
            var newEdit = edit;
            if (FSMEdits.TryGetValue(key, out var orig))
            {
                newEdit = fsm => {
                    orig(fsm);
                    edit(fsm);
                };
            }
            FSMEdits[key] = newEdit;
        }

        private void EditFSMs(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);
            if (FSMEdits.TryGetValue((fsm.gameObject.name, fsm.FsmName), out var edit))
            {
                edit(fsm);
            }
        }

        private void StartTicking()
        {
            // Use our own object to hold timers so that GameManager.StopAllCoroutines
            // does not kill them.
            var timerHolder = new GameObject("Timer Holder");
            GameObject.DontDestroyOnLoad(timerHolder);
            var timers = timerHolder.AddComponent<DummyMonoBehaviour>();
            foreach (var t in Tickers)
            {
                IEnumerator ticker()
                {
                    while (true)
                    {
                        try
                        {
                            t.Func();
                        }
                        catch (Exception ex)
                        {
                            LogError(ex);
                        }
                        yield return new WaitForSeconds(t.Period);
                    }
                }

                timers.StartCoroutine(ticker());
            }
        }

        private void CountOurCharms(On.PlayerData.orig_CountCharms orig, PlayerData self)
        {
            orig(self);
            self.SetInt("charmsOwned", self.GetInt("charmsOwned") + Charms.Count(c => c.Settings(Settings).Got));
        }

        internal static void UpdateNailDamage()
        {
            IEnumerator WaitThenUpdate()
            {
                yield return null;
                PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
            }
            GameManager.instance.StartCoroutine(WaitThenUpdate());
        }

        public void GrantAllOurCharms()
        {
            foreach (var charm in Charms)
            {
                charm.Settings(Settings).Got = true;
            }
        }

        public int GrantAllOurCharmsOnSoul(int soulamount)
        {
            foreach (var charm in Charms)
            {
                charm.Settings(Settings).Got = true;
            }
            return soulamount;
        }

        public int GiveSpecificCharm(int numberino)
        {
            Charms[numberino].Settings(Settings).Got = true;
            return numberino;
        }

        public void TakeAllOurCharms()
        {
            foreach (var charm in Charms)
            {
                charm.Settings(Settings).Got = false;
            }
        }

        SaveSettings ILocalSettings<SaveSettings>.OnSaveLocal()
        {
            return ((ILocalSettings<SaveSettings>)Instance).OnSaveLocal();
        }

        public SaveSettings OnSaveLocal()
        {
            throw new NotImplementedException();
        }

        public void OnLoadLocal(SaveSettings s)
        {
            throw new NotImplementedException();
        }
    }
}