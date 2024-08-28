using Dramalord.Quests;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Dramalord.Data
{
    internal class DramalordQuests : DramalordDataHandler
    {
        private static DramalordQuests? _instance;

        internal static DramalordQuests Instance
        {
            get
            {
                _instance ??= new DramalordQuests();
                return _instance;
            }
        }

        private readonly Dictionary<Hero, VisitQuest> _loverQuests;

        public DramalordQuests() : base("DramalordQuests")
        {
            _loverQuests = new();
        }

        internal VisitQuest? GetLoverQuest(Hero hero)
        {
            if(_loverQuests.ContainsKey(hero))
            {
                return _loverQuests[hero];
            }
            return null;
        }

        internal VisitQuest? CreateLoverQuest(Hero hero)
        {
            if(!_loverQuests.ContainsKey(hero))
            {
                VisitQuest newQuest = new VisitQuest(hero);
                _loverQuests.Add(hero, newQuest);
                return newQuest;
            }
            return null;
        }

        internal void RemoveLoverQuest(Hero hero)
        {
            _loverQuests.Remove(hero);
        }

        public override void LoadData(IDataStore dataStore)
        {
            _loverQuests.Clear();
            Dictionary<string, VisitQuest> data = new();
            dataStore.SyncData(SaveIdentifier + "_lover", ref data);

            data.Do(keypair =>
            {
                Hero? questGiver = Hero.AllAliveHeroes.Where(hero => hero.StringId == keypair.Key).FirstOrDefault();
                if (questGiver != null && !_loverQuests.ContainsKey(questGiver))
                {
                    _loverQuests.Add(questGiver, keypair.Value);
                }
            });
        }

        public override void SaveData(IDataStore dataStore)
        {
            Dictionary<string, VisitQuest> data = new();
            _loverQuests.Do(keypair =>
            {
                if (!data.ContainsKey(keypair.Key.StringId))
                {
                    data.Add(keypair.Key.StringId, keypair.Value);
                }
            });

            dataStore.SyncData(SaveIdentifier + "_lover", ref data);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            //nothing to do
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            //nothing to do
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            if(_loverQuests.ContainsKey(victim))
            {
                _loverQuests[victim].QuestFail();
                _loverQuests.Remove(victim);
            }
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            if (_loverQuests.ContainsKey(hero))
            {
                _loverQuests[hero].QuestFail();
                _loverQuests.Remove(hero);
            }
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _loverQuests.Clear();
        }
    }
}
