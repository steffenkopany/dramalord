using Dramalord.Data.Intentions;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Dramalord.Data
{
    internal class DramalordIntentions : DramalordData
    {
        private static DramalordIntentions? _instance;

        internal static DramalordIntentions Instance
        {
            get
            {
                _instance ??= new DramalordIntentions();
                return _instance;
            }
        }
 
        private readonly List<Intention> _intentions;

        private DramalordIntentions() : base("DramalordIntentions")
        {
            _intentions = new();
        }

        internal List<Intention> GetIntentions()
        {
            _intentions.RemoveAll(intention => intention.ValidUntil.IsPast);
            return _intentions;
        }

        internal void OnHourlyTick()
        {
            List<Hero> heroList = new();
            List<Intention> garbage = new();

            List<Intention> intentions = GetIntentions().ToList();
            intentions.ForEach(intention =>
            {
                if(!heroList.Contains(intention.IntentionHero) && intention.Action())
                {
                    heroList.Add(intention.IntentionHero);
                    garbage.Add(intention);
                }
            });

            garbage.ForEach(g => _intentions.Remove(g));
        }

        internal override void LoadData(IDataStore dataStore)
        {
            _intentions.Clear();

            if(!IsOldData)
            {
                List<Intention> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);
                _intentions.AddRange(data);
            }
        }

        internal override void SaveData(IDataStore dataStore)
        {
            List<Intention> data = new();

            data.AddRange(_intentions);

            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            //nothing todo
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            // nothing todo
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _intentions.RemoveAll(intention => intention.IntentionHero == victim);
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _intentions.RemoveAll(intention => intention.IntentionHero == hero);
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _intentions.Clear();
        }
    }
}
