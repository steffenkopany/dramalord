using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class QuestConversation
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_quest_visit_start_open", "lord_start", "player_quest_visit_reply", "{=Dramalord306}Oh {TITLE} you are finally here! I was desperately waiting for you!", ConditionNpcHasQuestOpen, null, 120);
            starter.AddDialogLine("npc_quest_visit_start_fail", "lord_start", "player_quest_visit_reply", "{=Dramalord307}Look who shows up. You weren't in a hurry {TITLE}, right?", ConditionNpcHasQuestFail, null, 120);

            starter.AddPlayerLine("player_quest_visit_reply", "player_quest_visit_reply", "npc_quest_visit_resolution", "{=Dramalord308}Here I am. What is the urgent matter you asked me to take care about?", null, null);

            starter.AddDialogLine("npc_quest_visit_success", "npc_quest_visit_resolution", "close_window", "{=Dramalord309}Drop your clothes {TITLE} and follow me. I need to make use of your resourcefulness in a very delicate matter!", ConditionNpcHasQuestOpen, ConsequenceVisitQuestSuccess);
            starter.AddDialogLine("npc_quest_visit_fail", "npc_quest_visit_resolution", "close_window", "{=Dramalord310}Well {TITLE}, someone else was was taking care of my... matter. In a very pleasant way. Good bye.", ConditionNpcHasQuestFail, ConsequenceVisitQuestFail);
        }

        private static bool ConditionNpcHasQuestOpen()
        {
            VisitQuest? quest = DramalordQuests.Instance.GetLoverQuest(Hero.OneToOneConversationHero);
            if (Hero.OneToOneConversationHero.IsDramalordLegit() && quest != null && Hero.OneToOneConversationHero.GetDesires().Horny == 100)
            {
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                
                return true;
            }
            return false;
        }

        private static bool ConditionNpcHasQuestFail()
        {
            VisitQuest? quest = DramalordQuests.Instance.GetLoverQuest(Hero.OneToOneConversationHero);
            if (Hero.OneToOneConversationHero.IsDramalordLegit() && quest != null && Hero.OneToOneConversationHero.GetDesires().Horny < 100)
            {
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                return true;
            }
            return false;
        }

        internal static void ConsequenceVisitQuestSuccess()
        {
            DramalordQuests.Instance.GetLoverQuest(Hero.OneToOneConversationHero)?.QuestSuccess();
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        
        internal static void ConsequenceVisitQuestFail()
        {
            DramalordQuests.Instance.GetLoverQuest(Hero.OneToOneConversationHero)?.QuestFail();
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
