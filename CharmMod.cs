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
            WealthyAmulet.Instance,
            TripleJump.Instance
        };

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
            ModHooks.HeroUpdateHook += SetDefaultNotchCosts;
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
        private bool Equipped(Charm c) => c.Settings(Settings).Equipped;

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
        public override string GetVersion() => "6.3.2";

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
        private void SetDefaultNotchCosts()
        {
            GrantAllOurCharms();
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

        private void RandomizeNotchCosts(int seed)
        {
            // This log statement is here to help diagnose a possible bug where charms cost more than
            // they ever should.
            Log("Randomizing notch costs");
            var rng = new System.Random(seed);
            var total = Charms.Select(x => x.DefaultCost).Sum();
            for (var i = 0; i < total; i++)
            {
                var possiblePicks = Charms.Select(x => x.Settings(Settings)).Where(s => s.Cost < 6).ToList();
                if (possiblePicks.Count == 0)
                {
                    break;
                }
                var pick = rng.Next(possiblePicks.Count);
                possiblePicks[pick].Cost++;
            }
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

        public void OnLoadLocal(SaveSettings s)
        {
            ((ILocalSettings<SaveSettings>)Instance).OnLoadLocal(s);
        }

        SaveSettings ILocalSettings<SaveSettings>.OnSaveLocal()
        {
            return ((ILocalSettings<SaveSettings>)Instance).OnSaveLocal();
        }

        public SaveSettings OnSaveLocal()
        {
            throw new NotImplementedException();
        }
    }
}