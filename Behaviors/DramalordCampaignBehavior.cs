using Dramalord.Data;
using Dramalord.UI;
using System;
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
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero,Hero, KillCharacterActionDetail,bool>(Info.OnHeroKilled));
            CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener(this, new Action<Hero>(Info.OnHeroUnregistered));
            CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(Info.OnOrphanComesOfAge));

            //Campaign.Current.CampaignEvents

            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(AICampaignActions.DailyHeroUpdate));
            //CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(AICampaignActions.DailyRomance));
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(AICampaignActions.DailyViolence));
        }

        public override void SyncData(IDataStore dataStore)
        {
            Info.SyncData(dataStore);
        }
    }
}
