using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace Dramalord.Data
{
    internal class DramalordOrphans : DramalordDataHandler
    {
        private static DramalordOrphans? _instance;

        internal static DramalordOrphans Instance
        {
            get
            {
                _instance ??= new DramalordOrphans();
                return _instance;
            }
        }

        private readonly List<Hero> _orphans;

        private DramalordOrphans() : base("DramalordOrphans")
        {
            _orphans = new();
        }

        internal Hero? GetRandomOrphan()
        {
            return _orphans.GetRandomElement();
        }

        internal void AddOrphan(Hero hero)
        {
            if(!_orphans.Contains(hero) && hero.IsChild)
            {
                _orphans.Add(hero);
            }
        }

        internal int CountOrphans(bool female)
        {
            return _orphans.Where(orphan => orphan.IsFemale == female).Count();
        }

        internal List<Hero> GetOrphans(bool female)
        {
            return _orphans.Where(orphan => orphan.IsFemale == female).ToList();
        }

        internal void RemoveOrphan(Hero hero)
        {
            _orphans.Remove(hero);
        }

        public override void LoadData(IDataStore dataStore)
        {
            _orphans.Clear();
            List<string> data = new();
            dataStore.SyncData(SaveIdentifier, ref data);

            data.Do(heroid =>
            {
                Hero? orphan = Hero.AllAliveHeroes.Where(hero => hero.StringId == heroid).FirstOrDefault();
                if(orphan != null && !_orphans.Contains(orphan))
                {
                    _orphans.Add(orphan);
                }
            });
        }

        public override void SaveData(IDataStore dataStore)
        {
            List<string> data = new();
            _orphans.ForEach(orphan =>
            {
                if(!data.Contains(orphan.StringId))
                {
                    data.Add(orphan.StringId);
                }
            });

            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _orphans.Remove(victim);
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _orphans.Remove(hero);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            _orphans.Remove(hero);
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            // nothing to do
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _orphans.Clear();
        }
    }
}
