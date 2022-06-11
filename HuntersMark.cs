using Modding;
using UnityEngine;
using GlobalEnums;
using HutongGames.PlayMaker.Actions;

namespace CharmMod
{
    internal class HuntersMark : Charm
    {
        public static readonly HuntersMark Instance = new();
        public override string Sprite => "HuntersMark.png";
        public override string Name => "The Hunter's Mark";
        public override string Description => "Unlocks all journal entries.\n\nI know, its bad, I was running out of ideas, and haven't bothered to delete it.";
        public override int DefaultCost => 0;
        public override string Scene => "Ruins2_11";
        public override float X => 0f;
        public override float Y => 0f;

        private HuntersMark() {}

        public override CharmSettings Settings(SaveSettings s) => s.HuntersMark;

        public override void Hook()
        {
            ModHooks.HeroUpdateHook += OnStep;
            
        }

        private void OnStep()
        {
            if(Equipped()) {
                PlayerData pd = PlayerData.instance;
                // Whether the player has the Hunter's Journal
                pd.hasJournal = true;
                // Last entry looked at
                pd.lastJournalItem = 0;
                // Whether the player has seen the journal message
                pd.seenJournalMsg = true;
                // Whether the player has seen the hunter message
                pd.seenHunterMsg = true;
                // Whether the player has a full journal
                pd.fillJournal = true;
                // Amount of completed entries
                pd.journalEntriesCompleted = 164;
                // Idk if it is used
                pd.journalNotesCompleted = 164;
                // Amount of total entries
                pd.journalEntriesTotal = 164;
                if (HeroController.instance == null)
                {
                    return;
                }
            }
        }
    }
}