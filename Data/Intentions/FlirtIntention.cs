using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Quests;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class FlirtIntention : Intention
    {
        private static bool _accepted = false;

        private static int _modifier;

        public FlirtIntention(Hero target, Hero intentionHero, CampaignTime validUntil, int modifier = 1) : base(intentionHero, target, validUntil)
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
                FlirtAction.Apply(IntentionHero, Target, out int loveGain, _modifier);
                new ChangeOpinionIntention(IntentionHero, Target, loveGain, 0, CampaignTime.Now).Action();
            }

            _accepted = false;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_starts_interaction_unknown}[ib:nervous][if:convo_nervous]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as FlirtIntention != null && !Hero.OneToOneConversationHero.HasMet)
                        .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                        .GotoDialogState("start_flirt")
                    .NpcOption("{npc_starts_interaction_known}[ib:nervous2][if:convo_confused_normal]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as FlirtIntention != null && Hero.OneToOneConversationHero.HasMet)
                        .GotoDialogState("start_flirt")
                .EndNpcOptions();

            DialogFlow flirtflow = DialogFlow.CreateDialogFlow("start_flirt")
                .BeginPlayerOptions()
                    .PlayerOption("{player_interaction_start_react_yes}")
                        .NpcLineWithVariation("{npc_interaction_flirt_1}[ib:nervous][if:convo_mocking_teasing]")
                            .Variation("{npc_interaction_flirt_2}[ib:nervous][if:convo_mocking_teasing]")
                            .BeginPlayerOptions()
                                .PlayerOption("{npc_interaction_reply_flirt_yes_1}")
                                    .Consequence(() => { _accepted = false; ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Flirt, 1); })
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
            Campaign.Current.ConversationManager.AddDialogFlow(flirtflow);
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_interaction_unknown.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time."
            ConversationLines.npc_starts_interaction_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, true));// "{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?");
            ConversationLines.player_interaction_start_react_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord029}Of course {TITLE}. How can I be of service?");
            ConversationLines.player_interaction_start_react_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
            ConversationLines.npc_interaction_flirt_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord033}I have to say I really like your smile. Would you mind telling me more about yourself?");
            ConversationLines.npc_interaction_flirt_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord034}Would you mind going for a walk with me? I want to cause jealousy with your outstanding appearance.");
            ConversationLines.npc_interaction_reply_flirt_yes_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord056}Of course {TITLE}, I would love to join you.");
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
        }
    }
}
