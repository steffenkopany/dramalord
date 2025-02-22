using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Dramalord.Data
{
    internal abstract class DramalordData
    {
        internal static List<DramalordData> All = new();

        protected string SaveIdentifier;

        internal static bool IsOldData = false;

        public DramalordData(string saveIdentifier)
        {
            SaveIdentifier = saveIdentifier;
            All.Add(this);
        }

        internal virtual void InitEvents()
        {
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(OnHeroKilled));
            CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener(this, new Action<Hero>(OnHeroUnregistered));
            CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(OnHeroComesOfAge));
            CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(OnHeroCreated));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnNewGameCreated));
        }

        internal static void LoadAllData(IDataStore dataStore)
        {
            bool data = true;
            dataStore.SyncData("DramalordData4", ref data);
            IsOldData = data;

            All.ForEach(loader => loader.LoadData(dataStore));
        }

        internal static void SaveAllData(IDataStore dataStore)
        {
            bool data = false;
            dataStore.SyncData("DramalordData4", ref data);

            All.ForEach(saver => saver.SaveData(dataStore));
        }

        internal abstract void LoadData(IDataStore dataStore);

        internal abstract void SaveData(IDataStore dataStore);

        protected abstract void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications);

        protected abstract void OnHeroUnregistered(Hero hero);

        protected abstract void OnHeroComesOfAge(Hero hero);

        protected abstract void OnHeroCreated(Hero hero, bool born);

        protected abstract void OnNewGameCreated(CampaignGameStarter starter);
    }
}
