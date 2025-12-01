using Dramalord.Conversations;
using Dramalord.Data;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Dramalord.Behaviors
{
    internal class DramalordCampaignBehavior : CampaignBehaviorBase
    {
        internal static bool HotButterFound = false;

        internal DramalordCampaignBehavior(CampaignGameStarter starter)
        {
            object o = DramalordPersonalities.Instance;
            o = DramalordRelations.Instance;
            o = DramalordPregnancies.Instance;
            o = DramalordDesires.Instance;
            o = DramalordOrphans.Instance;
            o = DramalordQuests.Instance;
            o = DramalordEvents.Instance;

            ConversationQuestions.AddDialogs(starter);
            ConversationPlayer.AddDialogs(starter);
            ConversationPersuasions.AddDialogs(starter);

        }

        public override void RegisterEvents()
        {
            DramalordPersonalities.Instance.InitEvents();
            DramalordRelations.Instance.InitEvents();
            DramalordPregnancies.Instance.InitEvents();
            DramalordDesires.Instance.InitEvents();
            DramalordOrphans.Instance.InitEvents();
            DramalordQuests.Instance.InitEvents();
            DramalordEvents.Instance.InitEvents();

            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(DramalordEvents.Instance.OnHourlyTick));
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(DramalordEvents.Instance.OnConversationEnded));
            CampaignEvents.OnAgentJoinedConversationEvent.AddNonSerializedListener(this, new Action<IAgent>(DramalordEvents.Instance.OnConversationStart));
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
