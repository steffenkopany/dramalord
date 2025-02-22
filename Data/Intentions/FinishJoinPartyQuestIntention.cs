using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Data.Intentions
{
    internal class FinishJoinPartyQuestIntention : Intention
    {
        internal static JoinPlayerQuest? FinishedQuest;

        public FinishJoinPartyQuestIntention(Hero target, Hero intentionHero, JoinPlayerQuest quest, CampaignTime validUntil) : base(intentionHero, target, validUntil)
        {
            FinishedQuest = quest;
        }

        public override bool Action()
        {
            List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
            if (Target == Hero.MainHero && closeHeroes.Contains(Hero.MainHero) && ConversationTools.StartConversation(this, true))
            {
                return true;
            }

            return false;
        }

        public override void OnConversationEnded()
        {
            IntentionHero.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
            FinishedQuest?.QuestSuccess(Hero.MainHero);
            FinishedQuest = null;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine("{player_quest_joinparty_end}")
                .Condition(() => FinishedQuest != null)
                .Consequence(() => ConversationTools.EndConversation())
                .CloseDialog();


            Campaign.Current.ConversationManager.AddDialogFlow(flow);
        }

        public override void OnConversationStart()
        {
            ConversationLines.player_quest_joinparty_end.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord546}These past days by your side have been a gift {TITLE}, and I am grateful for every moment we've shared. Though we part for now, our paths will soon cross again - of that, I have no doubt."
        }
    }
}
