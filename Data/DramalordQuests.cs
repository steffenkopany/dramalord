using Dramalord.Quests;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Dramalord.Data
{
    internal class DramalordQuests : DramalordData
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

        private readonly Dictionary<Hero, DramalordQuest> _dramalordQuests;

        public DramalordQuests() : base("DramalordQuests")
        {
            _dramalordQuests = new();
        }

        internal Dictionary<Hero, DramalordQuest> GetAllQuests() => _dramalordQuests;

        internal DramalordQuest? GetQuest(Hero hero)
        {
            if(_dramalordQuests.ContainsKey(hero))
            {
                return _dramalordQuests[hero];
            }
            return null;
        }

        public void AddQuest(Hero hero, DramalordQuest quest)
        {
            if (!_dramalordQuests.ContainsKey(hero))
            {
                _dramalordQuests.Add(hero, quest);
            }
        }

        public void RemoveQuest(Hero hero)
        {
            _dramalordQuests.Remove(hero);
        }

        internal override void LoadData(IDataStore dataStore)
        {
            _dramalordQuests.Clear();

            if(!IsOldData)
            {
                Dictionary<Hero, DramalordQuest> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);

                data.Do(keypair =>
                {
                    _dramalordQuests.Add(keypair.Key, keypair.Value);
                });
            }
        }

        internal override void SaveData(IDataStore dataStore)
        {
            Dictionary<Hero, DramalordQuest> data = new();
            _dramalordQuests.Do(keypair =>
            {
                data.Add(keypair.Key, keypair.Value);
            });

            dataStore.SyncData(SaveIdentifier, ref data);
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
            if(_dramalordQuests.ContainsKey(victim))
            {
                _dramalordQuests[victim].QuestTimeout();
                _dramalordQuests.Remove(victim);
            }
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            if (_dramalordQuests.ContainsKey(hero))
            {
                _dramalordQuests[hero].QuestTimeout();
                _dramalordQuests.Remove(hero);
            }
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _dramalordQuests.Clear();
        }
    }
}
