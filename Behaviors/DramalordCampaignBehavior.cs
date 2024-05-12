using Dramalord.Data;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using static TaleWorlds.CampaignSystem.Actions.KillCharacterAction;

namespace Dramalord.Behaviors
{
    internal class DramalordCampaignBehavior : CampaignBehaviorBase
    {
        internal DramalordCampaignBehavior(CampaignGameStarter starter)
        {
            Conversations.AddDialogs(starter);
            Persuasions.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(GameMenus.AddGameMenus));

            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero,Hero, KillCharacterActionDetail,bool>(Info.OnHeroKilled));
            CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener(this, new Action<Hero>(Info.OnHeroUnregistered));
            CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(Info.OnOrphanComesOfAge));
            CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(AICampaignActions.OnNewCompanionAdded));
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(PlayerCampaignActions.OnConversationEnded));

            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(AICampaignActions.DailyHeroUpdate));
        }

        public override void SyncData(IDataStore dataStore)
        {
            Info.SyncData(dataStore);
        }
    }
}
