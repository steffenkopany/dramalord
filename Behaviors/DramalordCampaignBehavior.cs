using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Behaviors
{
    internal class DramalordCampaignBehavior : CampaignBehaviorBase
    {
        internal DramalordCampaignBehavior(CampaignGameStarter starter)
        {
            Persuasions.AddDialogs(starter);
            IntimateQuestions.AddDialogs(starter);
            PlayerRequests.AddDialogs(starter);
            PlayerInteractions.AddDialogs(starter);
            NpcInteractions.AddDialogs(starter);
            PlayerConfrontation.AddDialogs(starter);
            QuestInteractions.AddDialogs(starter);
            NPCConfrontation.AddDialogs(starter);
            OrphanageConversation.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(GameMenus.AddGameMenus));
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(ConversationHelper.OnConversationEnded));
            //CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(HeroFightAction.OnMissionTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            HeroDataSaver.SyncData(dataStore);
        }
    }
}
