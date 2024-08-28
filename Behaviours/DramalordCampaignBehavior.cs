using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Behavior
{
    internal class DramalordCampaignBehavior : CampaignBehaviorBase
    {
        internal DramalordCampaignBehavior(CampaignGameStarter starter)
        {
            object o = DramalordPersonalities.Instance;
            o = DramalordRelations.Instance;
            o = DramalordPregancies.Instance;
            o = DramalordDesires.Instance;
            o = DramalordOrphans.Instance;
            o = DramalordEvents.Instance;
            o = DramalordIntentions.Instance;
            o = DramalordQuests.Instance;

            OrphanageConversation.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            DramalordPersonalities.Instance.InitEvents();
            DramalordRelations.Instance.InitEvents();
            DramalordPregancies.Instance.InitEvents();
            DramalordDesires.Instance.InitEvents();
            DramalordOrphans.Instance.InitEvents();
            DramalordEvents.Instance.InitEvents();
            DramalordIntentions.Instance.InitEvents();
            DramalordQuests.Instance.InitEvents();

            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(DramalordPregancies.Instance.OnHourlyTick));
            CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(ConversationHelper.OnConversationEnded));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(AdoptionMenu.AddGameMenus));
        }

        public override void SyncData(IDataStore dataStore)
        {
            if(dataStore.IsSaving)
            {
                DramalordDataHandler.All.ForEach(saver => saver.SaveData(dataStore));
            }
            else if(dataStore.IsLoading)
            {
                DramalordDataHandler.All.ForEach(loader => loader.LoadData(dataStore));
            }
        }
    }
}
