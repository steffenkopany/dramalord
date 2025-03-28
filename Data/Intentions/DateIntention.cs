using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications.Logs;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class DateIntention : Intention
    {
        private static bool _accepted = false;

        private static int _modifier;

        public DateIntention(Hero target, Hero intentionHero, CampaignTime validUntil, int modifier = 1) : base(intentionHero, target, validUntil)
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
                DateAction.Apply(IntentionHero, Target, out int loveGain, out int trustGain, _modifier);
                new ChangeOpinionIntention(IntentionHero, Target, loveGain, trustGain, CampaignTime.Now).Action();

                HeroRelation relation = IntentionHero.GetRelationTo(Target);
                if ((relation.Relationship == RelationshipType.None || relation.Relationship == RelationshipType.Friend || relation.Relationship == RelationshipType.FriendWithBenefits) && relation.Love >= DramalordMCM.Instance.MinDatingLove)
                {
                    StartRelationshipAction.Apply(IntentionHero, Target, relation, RelationshipType.Lover);
                    if (DramalordMCM.Instance.RelationshipLogs)
                    {
                        LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Lover));
                    }

                    if(IntentionHero == Hero.MainHero || Target == Hero.MainHero)
                    {
                        Hero other = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                        TextObject banner = new TextObject("{=Dramalord106}You are now the lover of {HERO.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", other.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, other.CharacterObject, "event:/ui/notification/relation");
                    }
                }

                Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                if ((IntentionHero == Hero.MainHero || Target == Hero.MainHero) && _modifier > 2 && otherHero.GetDesires().Horny > 50)
                {
                    int speed = (int)Campaign.Current.TimeControlMode;
                    Campaign.Current.SetTimeSpeed(0);
                    TextObject title = new TextObject("{=Dramalord568}Opportunity for intimacy");
                    TextObject text = new TextObject("{=Dramalord569}There is a certain heat in {HERO1}'s gaze, their smile laced with unmistakable intent. Will you answer their invitation?");
                    text.SetTextVariable("HERO1", otherHero.Name);
                    InformationManager.ShowInquiry(
                            new InquiryData(
                                title.ToString(),
                                text.ToString(),
                                true,
                                true,
                                GameTexts.FindText("str_yes").ToString(),
                                GameTexts.FindText("str_no").ToString(),
                                () => {
                                    Campaign.Current.SetTimeSpeed(speed);
                                    new IntercourseIntention(otherHero, Hero.MainHero, CampaignTime.Now, true).OnConversationEnded();
                                },
                                () => {
                                    Campaign.Current.SetTimeSpeed(speed);
                                    HandleWittness(relation);
                                }), true);
                }
                else
                {
                    HandleWittness(relation);
                }
            }
            else
            {
                RelationshipLossAction.Apply(IntentionHero, Target, out int loveDamage, out int trustDamage, 5, 5);
                new ChangeOpinionIntention(IntentionHero, Target, loveDamage, trustDamage, CampaignTime.Now).Action();
            }

            _accepted = false;
        }

        internal void HandleWittness(HeroRelation relation)
        {
            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
            {
                List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
                Hero? witness = DramalordMCM.Instance.PlayerAlwaysWitness && Target != Hero.MainHero && closeHeroes.Contains(Hero.MainHero) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h != Target);
                if (witness != null)
                {
                    if (witness != Hero.MainHero && (witness.IsEmotionalWith(IntentionHero) || witness.IsEmotionalWith(Target)))
                    {
                        DramalordIntentions.Instance.GetIntentions().Add(new ConfrontDateIntention(IntentionHero, Target, witness, CampaignTime.DaysFromNow(7), true));
                        DramalordIntentions.Instance.GetIntentions().Add(new ConfrontDateIntention(Target, IntentionHero, witness, CampaignTime.DaysFromNow(7), true));
                    }
                    else if (witness != Hero.MainHero && !IntentionHero.IsSpouseOf(Target))
                    {
                        if (Hero.MainHero.Spouse != null && (witness.GetHeroTraits().Mercy < 0 || witness.GetHeroTraits().Honor < 0) && witness.GetTrust(IntentionHero) < DramalordMCM.Instance.MinTrustFriends && witness.GetTrust(Target) < DramalordMCM.Instance.MinTrustFriends)
                        {
                            DramalordIntentions.Instance.GetIntentions().Add(new BlackmailDateIntention(this, witness, IntentionHero, CampaignTime.DaysFromNow(7)));
                            DramalordIntentions.Instance.GetIntentions().Add(new BlackmailDateIntention(this, witness, Target, CampaignTime.DaysFromNow(7)));
                        }
                        else
                        {
                            List<Hero> targets = new() { IntentionHero, Target };
                            DramalordIntentions.Instance.GetIntentions().Add(new GossipDateIntention(this, true, targets, witness, CampaignTime.DaysFromNow(7)));
                        }
                    }

                    if (witness == Hero.MainHero && (IntentionHero.Spouse != Target || witness.IsEmotionalWith(Target) || witness.IsEmotionalWith(IntentionHero)))
                    {
                        TextObject banner = new TextObject("{=Dramalord070}You saw {HERO.LINK} having a date with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", Target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");

                        if (!relation.IsKnownToPlayer)
                        {
                            relation.IsKnownToPlayer = true;
                            TextObject banner2 = new TextObject("{=Dramalord540}You have learned that {HERO} and {TARGET} are {RELATION}.");
                            banner2.SetTextVariable("HERO", IntentionHero.Name);
                            banner2.SetTextVariable("TARGET", Target.Name);
                            TextObject rel = (relation.Relationship == RelationshipType.Betrothed) ? new TextObject("{=Dramalord013}engaged") : new TextObject("{=Dramalord012}lovers");
                            banner2.SetTextVariable("RELATION", rel);//"{=Dramalord011}friends with benefits" "{=Dramalord012}lovers" "{=Dramalord014}married"
                            MBInformationManager.AddQuickInformation(banner2, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
                        }

                        if (Hero.MainHero.IsEmotionalWith(IntentionHero) || Hero.MainHero.IsEmotionalWith(Target))
                        {
                            int speed = (int)Campaign.Current.TimeControlMode;
                            Campaign.Current.SetTimeSpeed(0);
                            TextObject title = new TextObject("{=Dramalord579}Interrupt {HERO1} and {HERO2} in the act");
                            TextObject text = new TextObject("{=Dramalord580}You caught {HERO1} and {HERO2} in the act. Do you wish to interrupt them?");
                            title.SetTextVariable("HERO1", IntentionHero.Name);
                            title.SetTextVariable("HERO2", Target.Name);
                            text.SetTextVariable("HERO1", IntentionHero.Name);
                            text.SetTextVariable("HERO2", Target.Name);
                            InformationManager.ShowInquiry(
                                    new InquiryData(
                                        title.ToString(),
                                        text.ToString(),
                                        true,
                                        true,
                                        GameTexts.FindText("str_yes").ToString(),
                                        GameTexts.FindText("str_no").ToString(),
                                        () => { new ConfrontationPlayerIntention(this, Hero.MainHero.IsEmotionalWith(IntentionHero) ? IntentionHero : Target, CampaignTime.Now).Action(); },
                                        () => { Campaign.Current.SetTimeSpeed(speed); }), true);
                        }
                    }
                    else if ((IntentionHero == Hero.MainHero || Target == Hero.MainHero) && !IntentionHero.IsSpouseOf(Target))
                    {
                        Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                        TextObject banner = new TextObject("{=Dramalord071}{HERO.LINK} saw you having a date with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
            }
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_starts_interaction_unknown}[ib:nervous][if:convo_nervous]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as DateIntention != null && !Hero.OneToOneConversationHero.HasMet)
                        .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                        .GotoDialogState("start_date")
                    .NpcOption("{npc_starts_interaction_known}[ib:nervous2][if:convo_confused_normal]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as DateIntention != null && Hero.OneToOneConversationHero.HasMet)
                        .GotoDialogState("start_date")
                .EndNpcOptions();

            DialogFlow flow2 = DialogFlow.CreateDialogFlow("start_date")
                .BeginPlayerOptions()
                    .PlayerOption("{player_interaction_start_react_yes}")
                        .GotoDialogState("start_date2")
                    .PlayerOption("{player_interaction_start_react_no}")
                        .Consequence(() => ConversationTools.EndConversation())
                        .CloseDialog()
                .EndPlayerOptions();

            DialogFlow flow3 = DialogFlow.CreateDialogFlow("start_date2")
                .BeginNpcOptions()
                    .NpcOptionWithVariation("{npc_interaction_date_first_1}[ib:nervous][if:convo_merry]", () => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                        .Variation("{npc_interaction_date_first_2}[ib:nervous][if:convo_merry]")
                        .GotoDialogState("start_date3")
                    .NpcOptionWithVariation("{npc_interaction_date_single_1}[ib:confident2][if:convo_focused_happy]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero))
                        .Variation("{npc_interaction_date_single_2}[ib:confident2][if:convo_focused_happy]")
                        .GotoDialogState("start_date3")
                    .NpcOptionWithVariation("{npc_interaction_date_married_1}[ib:nervous][if:convo_mocking_teasing]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero)
                        .Variation("{npc_interaction_date_married_2}[ib:nervous][if:convo_mocking_teasing]")
                        .GotoDialogState("start_date3")
                .EndNpcOptions();

            DialogFlow flow4 = DialogFlow.CreateDialogFlow("start_date3")
                .BeginPlayerOptions()
                    .PlayerOption("{npc_interaction_reply_date_first_yes_1}")
                        .Condition(() => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                        .Consequence(() => { _accepted = false; ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Date, 3, true); })
                        .GotoDialogState("start_challenge")
                    .PlayerOption("{npc_interaction_reply_date_yes_1}")
                        .Condition(() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero))
                        .Consequence(() => { _accepted = false; ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Date, 3, true); })
                        .GotoDialogState("start_challenge")
                    .PlayerOption("{player_reaction_date_married_yes}")
                        .Condition(() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero)
                        .Consequence(() => { _accepted = false; ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Date, 3, true); })
                        .GotoDialogState("start_challenge")
                    .PlayerOption("{player_reaction_no}")
                        .Consequence(() => { _accepted = false; ConversationTools.EndConversation(); })
                        .CloseDialog()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
            Campaign.Current.ConversationManager.AddDialogFlow(flow2);
            Campaign.Current.ConversationManager.AddDialogFlow(flow3);
            Campaign.Current.ConversationManager.AddDialogFlow(flow4);
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_interaction_unknown.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time."
            ConversationLines.npc_starts_interaction_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, true));// "{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?");
            ConversationLines.player_interaction_start_react_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord029}Of course {TITLE}. How can I be of service?");
            ConversationLines.player_interaction_start_react_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
            ConversationLines.npc_interaction_date_first_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord035}I must confess, I can not stop thinking of you. I clearly have feelings for you, {TITLE}, and would like to bring our relationship to the next level. What do you say?");
            ConversationLines.npc_interaction_date_first_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord036}Your presence makes me blush, {TITLE}. I would love to see you more frequently, just the two of us in private. What do you say, {TITLE}?");
            ConversationLines.npc_interaction_date_single_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord037}Oh, {TITLE}, I have been missing you! The servants prepared a meal im my private chambers. Would you care to join me?");
            ConversationLines.npc_interaction_date_single_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord038}I was looking forward to seeing you, {TITLE}. Would you like to retreat somewhere more silent, for a more private conversation?");
            ConversationLines.npc_interaction_date_married_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord039}We are in luck, {TITLE}. {SPOUSE} is currently not around and the chambermaid swore to remain silent. Will you come with me?");
            ConversationLines.npc_interaction_date_married_1.SetTextVariable("SPOUSE", IntentionHero.Spouse?.Name);//"{=Dramalord039}We are in luck, {TITLE}. {SPOUSE} is currently not around and the chambermaid swore to remain silent. Will you come with me?");
            ConversationLines.npc_interaction_date_married_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord040}Ugh, {SPOUSE} is finally away. Now we have all the rooms for us! The servants will keep it for themselves, care to join me, {TITLE}?");
            ConversationLines.npc_interaction_date_married_2.SetTextVariable("SPOUSE", IntentionHero.Spouse?.Name);//"{=Dramalord040}Ugh, {SPOUSE} is finally away. Now we have all the rooms for us! The servants will keep it for themselves, care to join me, {TITLE}?");
            ConversationLines.npc_interaction_reply_date_first_yes_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord057}Oh {TITLE}, I wish for the same!");
            ConversationLines.npc_interaction_reply_date_yes_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord058}Sure {TITLE}, I enjoy every minute with you.");
            ConversationLines.player_reaction_date_married_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord059}Oh yes, {TITLE}. Let's enjoy the time as long as they're gone.");
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
        }
    }
}
