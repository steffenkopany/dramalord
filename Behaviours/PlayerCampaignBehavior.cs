using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using SandBox.View.Map;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Dramalord.Behaviours
{
    internal class PlayerCampaignBehavior : CampaignBehaviorBase
    {
        internal PlayerCampaignBehavior(CampaignGameStarter starter)
        {
            PlayerApproachingNPC.AddDialogs(starter);
            PlayerConfrontNPC.AddDialogs(starter);
            PrisonerConversation.AddDialogs(starter);
            QuestConversation.AddDialogs(starter);
            PlayerChallenges.AddDialogs(starter);
            Persuasions.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(OnHourlyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //throw new NotImplementedException();
        }

        internal void OnHourlyTick()
        {
            HeroIntention intention = Hero.MainHero.GetIntentions().FirstOrDefault(intention => intention.Type == IntentionType.Confrontation);
            {
                if(intention != null && intention.Target.IsCloseTo(Hero.MainHero))
                {
                    HeroEvent? @event = DramalordEvents.Instance.GetEvent(intention.EventId);
                    if (!ConversationHelper.ConversationRunning && @event != null && intention.Target.IsEmotionalWith(Hero.MainHero))
                    {
                        ConversationHelper.ConversationRunning = true;
                        PlayerConfrontNPC.Start(intention.Target, @event); 
                    }
                    else if(@event != null && intention.Target.IsEmotionalWith(Hero.MainHero))
                    {
                        return;
                    }

                    DramalordIntentions.Instance.RemoveIntention(Hero.MainHero, intention.Target, intention.Type, intention.EventId);
                    return;
                }
            }
        }
    }
}
