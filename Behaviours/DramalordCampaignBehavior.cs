using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Quests;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Dramalord.Behavior
{
    internal class DramalordCampaignBehavior : CampaignBehaviorBase
    {
        internal DramalordCampaignBehavior(CampaignGameStarter starter)
        {
            object o = DramalordPersonalities.Instance;
            o = DramalordRelations.Instance;
            o = DramalordPregnancies.Instance;
            o = DramalordDesires.Instance;
            o = DramalordOrphans.Instance;
            o = DramalordIntentions.Instance;
            o = DramalordQuests.Instance;

            ConversationLines.Init();
            ConversationQuestions.AddDialogs(starter);
            ConversationRelationship.AddDialogs(starter);
            ConversationPersuasions.AddDialogs(starter);
            ConversationTrade.AddDialogs(starter);
            ConversationRealm.AddDialogs(starter);
            ConversationFamily.AddDialogs(starter);

            TalkIntention.AddDialogs(starter);
            FlirtIntention.AddDialogs(starter);
            IntercourseIntention.AddDialogs(starter);
            DateIntention.AddDialogs(starter);
            BetrothIntention.AddDialogs(starter);
            MarriageIntention.AddDialogs(starter);
            ConfrontIntercourseIntention.AddDialogs(starter);
            ConfrontDateIntention.AddDialogs(starter);
            ConfrontBetrothedIntention.AddDialogs(starter);
            ConfrontMarriageIntention.AddDialogs(starter);
            ConfrontBirthIntention.AddDialogs(starter);
            GossipBetrothedIntention.AddDialogs(starter);
            GossipBirthIntention.AddDialogs(starter);
            GossipDateIntention.AddDialogs(starter);
            GossipIntercourseIntention.AddDialogs(starter);
            GossipMarriageIntention.AddDialogs(starter);
            PrisonIntercourseIntention.AddDialogs(starter);
            FinishJoinPartyQuestIntention.AddDialogs(starter);
            ConfrontationPlayerIntention.AddDialogs(starter);
            DuelIntention.AddDialogs(starter);
            BlackmailBetrothedIntention.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            DramalordPersonalities.Instance.InitEvents();
            DramalordRelations.Instance.InitEvents();
            DramalordPregnancies.Instance.InitEvents();
            DramalordDesires.Instance.InitEvents();
            DramalordOrphans.Instance.InitEvents();
            DramalordIntentions.Instance.InitEvents();
            DramalordQuests.Instance.InitEvents();

            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(DramalordIntentions.Instance.OnHourlyTick));
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(ConversationTools.OnConversationEnded));
            CampaignEvents.OnAgentJoinedConversationEvent.AddNonSerializedListener(this, new Action<IAgent>(ConversationTools.OnConversationStart));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if(dataStore.IsSaving)
            {
                DramalordData.SaveAllData(dataStore);
            }
            else if(dataStore.IsLoading)
            {
                DramalordData.LoadAllData(dataStore);
            }
        }
    }
}
