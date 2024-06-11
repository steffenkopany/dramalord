using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Deprecated;
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
            Persuasions.AddDialogs(starter);
            IntimateQuestions.AddDialogs(starter);
            PlayerRequests.AddDialogs(starter);
            PlayerInteractions.AddDialogs(starter);
            NpcInteractions.AddDialogs(starter);
            PlayerConfrontation.AddDialogs(starter);
            QuestInteractions.AddDialogs(starter);
            NPCConfrontation.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(GameMenus.AddGameMenus));
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(ConversationHelper.OnConversationEnded));
        }

        public override void SyncData(IDataStore dataStore)
        {
            HeroDataSaver.SyncData(dataStore);
        }
    }
}
