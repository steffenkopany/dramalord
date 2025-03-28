using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class TalkIntention : Intention
    {
        internal static bool HotButterFound = false;

        private static bool _accepted = false;
        private static int _modifier;

        public TalkIntention(Hero target, Hero intentionHero, CampaignTime validUntil, int modifier = 1) : base(intentionHero, target, validUntil)
        {
            _accepted = true;
            _modifier = modifier;
        }

        public override bool Action()
        {
            _accepted = false;

            List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
            if (Target == Hero.MainHero && closeHeroes.Contains(Hero.MainHero) && ConversationTools.StartConversation(this, true))
            {
                return true;
            }
            else if (Target != Hero.MainHero && closeHeroes.Contains(Target))
            {
                _accepted = true;
                OnConversationEnded();
                return true;
            }

            return false;
        }

        public override void OnConversationEnded()
        {
            IntentionHero.GetRelationTo(Target).LastInteraction = CampaignTime.Now;

            if (_accepted)
            {
                TalkAction.Apply(IntentionHero, Target, out int trustGain, _modifier);
                new ChangeOpinionIntention(IntentionHero, Target, 0, trustGain, CampaignTime.Now).Action();
            }

            _accepted = false;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_starts_interaction_unknown}[ib:nervous][if:convo_nervous]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as TalkIntention != null && !Hero.OneToOneConversationHero.HasMet)
                        .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                        .GotoDialogState("start_talk")
                    .NpcOption("{npc_starts_interaction_known}[ib:nervous2][if:convo_confused_normal]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as TalkIntention != null && Hero.OneToOneConversationHero.HasMet)
                        .GotoDialogState("start_talk")
                .EndNpcOptions();

            DialogFlow talkflow = DialogFlow.CreateDialogFlow("start_talk")
                .BeginPlayerOptions()
                    .PlayerOption("{player_interaction_start_react_yes}")
                        .NpcLineWithVariation("{npc_interaction_talk_1}[ib:normal2][if:convo_calm_friendly]")
                            .Variation("{npc_interaction_talk_2}[ib:normal2][if:convo_calm_friendly]")
                        .BeginPlayerOptions()
                            .PlayerOption("{npc_interaction_reply_talk_1}")
                                .Consequence(() => { _accepted = false; ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Chat, 1, true); })
                                .GotoDialogState("start_challenge")
                            .PlayerOption("{player_reaction_no}")
                                .Consequence(() => { _accepted = false; ConversationTools.EndConversation(); })
                                .CloseDialog()
                        .EndPlayerOptions()
                    .PlayerOption("{player_interaction_start_react_no}")
                        .Consequence(() => ConversationTools.EndConversation())
                        .CloseDialog()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
            Campaign.Current.ConversationManager.AddDialogFlow(talkflow);
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_interaction_unknown.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time."
            ConversationLines.npc_starts_interaction_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, true));// "{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?");
            ConversationLines.player_interaction_start_react_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord029}Of course {TITLE}. How can I be of service?");
            ConversationLines.player_interaction_start_react_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
            ConversationLines.npc_interaction_talk_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord031}I always like to hear new stories. Tell me about your latest exploits while traveling in the realm."
            ConversationLines.npc_interaction_talk_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// ""{=Dramalord032}I would like to hear your opinion of a certain matter which is occupying my mind for a while.";
            ConversationLines.npc_interaction_reply_talk_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord055}Sure, let's have a conversation {TITLE}.");
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
        }
    }
}
