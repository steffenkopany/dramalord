using Dramalord.Data;
using Dramalord.Quests;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;

namespace Dramalord.Conversations
{
    internal static class QuestInteractions
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_quest_visit_start", "lord_start" , "player_quest_visit_reply", "{=Dramalord291}Finally you are here!", ConditionNpcHasQuest, null, 120);

            starter.AddPlayerLine("player_quest_visit_reply", "player_quest_visit_reply", "npc_quest_visit_resolution", "{=Dramalord292}Here I am. What is the urgent matter you asked me to take care about?", null, null);

            starter.AddDialogLine("Dramalord293", "npc_quest_visit_resolution", "close_window", "{=Dramalord293}The matter... was resolved by someone very skillful meanwhile.", ConditionNpcQuestFail, ConsequenceVisitQuestFail);
            starter.AddDialogLine("Dramalord294", "npc_quest_visit_resolution", "close_window", "{=Dramalord294}Follow me. I would like to make use of your resourcefulness...", ConditionNpcQuestSuccess, ConsequenceVisitQuestSuccess);
        }

        //CONDITIONS
        internal static bool ConditionNpcHasQuest()
        {
            if (Hero.OneToOneConversationHero != null && (Hero.OneToOneConversationHero.IsLord || Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer))
            {
                return VisitLoverQuest.HeroList.ContainsKey(Hero.OneToOneConversationHero);
            }
            return false;
        }

        internal static bool ConditionNpcQuestFail()
        {
            return Hero.OneToOneConversationHero.GetDramalordTraits().Horny < DramalordMCM.Get.MinHornyForIntercourse;
        }

        internal static bool ConditionNpcQuestSuccess()
        {
            return Hero.OneToOneConversationHero.GetDramalordTraits().Horny >= DramalordMCM.Get.MinHornyForIntercourse;
        }

        //CONSEQUENCES
        internal static void ConsequenceVisitQuestFail()
        {
            if (VisitLoverQuest.HeroList.ContainsKey(Hero.OneToOneConversationHero))
            {
                VisitLoverQuest.HeroList[Hero.OneToOneConversationHero].QuestFail();
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceVisitQuestSuccess()
        {
            if (VisitLoverQuest.HeroList.ContainsKey(Hero.OneToOneConversationHero))
            {
                VisitLoverQuest.HeroList[Hero.OneToOneConversationHero].QuestSuccess();
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
