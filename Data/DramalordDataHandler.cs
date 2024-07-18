using Dramalord.LogItems;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal abstract class DramalordDataHandler
    {
        internal static List<DramalordDataHandler> All = new();

        protected string SaveIdentifier;

        public DramalordDataHandler(string saveIdentifier)
        {
            SaveIdentifier = saveIdentifier;
            All.Add(this);
        }

        public abstract void LoadData(IDataStore dataStore);

        public abstract void SaveData(IDataStore dataStore);

        internal virtual void InitEvents()
        {
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(OnHeroKilled));
            CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener(this, new Action<Hero>(OnHeroUnregistered));
            CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(OnHeroComesOfAge));
            CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(OnHeroCreated));
        }

        protected abstract void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications);

        protected abstract void OnHeroUnregistered(Hero hero);

        protected abstract void OnHeroComesOfAge(Hero hero);

        protected abstract void OnHeroCreated(Hero hero, bool born);
    }

    internal sealed class DramalordSaveableTypeDefiner : SaveableTypeDefiner
    {
        public DramalordSaveableTypeDefiner() : base(20110311)
        {

        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(StartRelationshipLog), 100);
            AddClassDefinition(typeof(EndRelationshipLog), 101);
            AddClassDefinition(typeof(LeaveClanLog), 102);
            AddClassDefinition(typeof(JoinClanLog), 103);
            AddClassDefinition(typeof(DramalordTakePrisonerLogEntry), 104);
            AddClassDefinition(typeof(DramalordEndCaptivityLogEntry), 105);  
            AddClassDefinition(typeof(ConceiveChildLog), 106);
            AddClassDefinition(typeof(BirthChildLog), 107);

            AddClassDefinition(typeof(HeroPersonality), 1000);
            AddClassDefinition(typeof(HeroRelation), 1001);
            AddClassDefinition(typeof(HeroPregnancySave), 1002); 
            AddClassDefinition(typeof(HeroDesires), 1003);
            AddClassDefinition(typeof(HeroEventSave), 1004); 
            AddClassDefinition(typeof(HeroIntentionSave), 1005);
            //AddClassDefinition(typeof(RelationshipType), 1006);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<string, HeroPersonality>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroRelation>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroPregnancySave>));
            ConstructContainerDefinition(typeof(List<string>));
            ConstructContainerDefinition(typeof(Dictionary<string, HeroDesires>));
            ConstructContainerDefinition(typeof(Dictionary<int, HeroEventSave>));
            ConstructContainerDefinition(typeof(List<HeroIntentionSave>));
            ConstructContainerDefinition(typeof(Dictionary<string, List<HeroIntentionSave>>));
            //ConstructContainerDefinition(typeof(List<HeroRelationshipSave>));
            //ConstructContainerDefinition(typeof(Dictionary<string, List<HeroRelationshipSave>>));
        }
    }
}
