using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Notifications.Logs;
using Dramalord.Quests;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class IntercourseIntention : Intention
    {
        internal static bool HotButterFound = false;

        internal static bool OtherPregnancyModFound = false;

        private static bool _accepted = false;

        public IntercourseIntention(Hero target, Hero intentionHero, CampaignTime validUntil, bool alwaysExecute = false) : base(intentionHero, target, validUntil)
        {
            _accepted = alwaysExecute;
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
                IntercourseAction.Apply(IntentionHero, Target, out int loveGain);

                if (Target == Hero.MainHero || IntentionHero == Hero.MainHero)
                {
                    if (HotButterFound)
                    {
                        MBInformationManager.ShowSceneNotification(new HotButterNotification(IntentionHero, Target, IntentionHero.CurrentSettlement));
                    }
                    else
                    {
                        Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                        TextObject banner = new TextObject("{=Dramalord072}You were intimate with {HERO.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
                    }
                }

                new ChangeOpinionIntention(IntentionHero, Target, loveGain, 0, CampaignTime.Now).Action();

                if (DramalordMCM.Instance.IntimateLogs)
                {
                    LogEntry.AddLogEntry(new IntercourseLog(IntentionHero, Target));
                }

                if (!OtherPregnancyModFound && !CampaignOptions.IsLifeDeathCycleDisabled && IntentionHero.IsFemale != Target.IsFemale && MBRandom.RandomInt(1, 100) <= DramalordMCM.Instance.PregnancyChance)
                {
                    Hero female = (IntentionHero.IsFemale) ? IntentionHero : Target;
                    Hero male = (IntentionHero.IsFemale) ? Target : IntentionHero;
                    
                    if (female.IsFertile() && female.IsEmotionalWith(male))
                    {
                        ConceiveAction.Apply(female, male);

                        if (female == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord075}You got pregnant from {HERO.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", male.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, male.CharacterObject, "event:/ui/notification/relation");
                        }
                        else if (male == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord076}{HERO.LINK} got pregnant from you.");
                            StringHelpers.SetCharacterProperties("HERO", female.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, female.CharacterObject, "event:/ui/notification/relation");
                        }

                        if (female.Clan == Clan.PlayerClan || male.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions)
                        {
                            LogEntry.AddLogEntry(new ConceiveChildLog(female, male));
                        }
                    }
                }

                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
                {
                    List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
                    Hero? witness = DramalordMCM.Instance.PlayerAlwaysWitness && Target != Hero.MainHero && closeHeroes.Contains(Hero.MainHero) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h != Target);
                    if (witness != null)
                    {
                        if (witness != Hero.MainHero && (witness.IsEmotionalWith(IntentionHero) || witness.IsEmotionalWith(Target)))
                        {
                            DramalordIntentions.Instance.GetIntentions().Add(new ConfrontIntercourseIntention(IntentionHero, Target, witness, CampaignTime.DaysFromNow(7), true));
                            DramalordIntentions.Instance.GetIntentions().Add(new ConfrontIntercourseIntention(Target, IntentionHero, witness, CampaignTime.DaysFromNow(7), true));
                        }
                        else if (witness != Hero.MainHero && IntentionHero.Spouse != Target)
                        {
                            List<Hero> targets = new() { IntentionHero, Target };
                            DramalordIntentions.Instance.GetIntentions().Add(new GossipIntercourseIntention(this, true, targets, witness, CampaignTime.DaysFromNow(7)));
                        }

                        if (witness == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord073}You caught {HERO.LINK} being intimate with {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", Target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");

                            HeroRelation relation = IntentionHero.GetRelationTo(Target);
                            if (!relation.IsKnownToPlayer)
                            {
                                relation.IsKnownToPlayer = true;
                                TextObject banner2 = new TextObject("{=Dramalord540}You have learned that {HERO} and {TARGET} are {RELATION}.");
                                banner2.SetTextVariable("HERO", IntentionHero.Name);
                                banner2.SetTextVariable("TARGET", Target.Name);
                                TextObject rel = (relation.Relationship == RelationshipType.Betrothed) ? new TextObject("{=Dramalord013}engaged") : (relation.Relationship == RelationshipType.Lover) ? new TextObject("{=Dramalord012}lovers") : new TextObject("{=Dramalord011}friends with benefits");
                                banner2.SetTextVariable("RELATION", rel);//"{=Dramalord011}friends with benefits" "{=Dramalord012}lovers" "{=Dramalord014}married"
                                MBInformationManager.AddQuickInformation(banner2, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
                            }

                            if(Hero.MainHero.IsEmotionalWith(IntentionHero) || Hero.MainHero.IsEmotionalWith(Target))
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
                        else if (IntentionHero == Hero.MainHero || Target == Hero.MainHero)
                        {
                            Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                            TextObject banner = new TextObject("{=Dramalord074}{HERO.LINK} caught you being intimate with {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                        }
                    }
                }

                if(IntentionHero != Hero.MainHero && Target != Hero.MainHero)
                {
                    VisitLoverQuest? quest = DramalordQuests.Instance.GetQuest(IntentionHero) as VisitLoverQuest;
                    if(quest != null)
                    {
                        quest.QuestFail(Target);
                    }

                    VisitLoverQuest? quest2 = DramalordQuests.Instance.GetQuest(Target) as VisitLoverQuest;
                    if (quest2 != null)
                    {
                        quest2.QuestFail(IntentionHero);
                    }
                }

                if(IntentionHero.IsFriendOf(Target))
                {
                    IntentionHero.GetRelationTo(Target).Relationship = RelationshipType.FriendWithBenefits;
                    if (Target == Hero.MainHero || IntentionHero == Hero.MainHero)
                    {
                        Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                        TextObject banner = new TextObject("{=Dramalord080}You and {HERO.LINK} are now friends with benefits.");
                        StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
                    }
                }
            }
            else if (!_accepted)
            {
                RelationshipLossAction.Apply(IntentionHero, Target, out int loveDamage, out int trustDamage, 20, 10);
                new ChangeOpinionIntention(IntentionHero, Target, loveDamage, trustDamage, CampaignTime.Now).Action();
            }

            _accepted = false;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_starts_interaction_unknown}[ib:nervous][if:convo_nervous]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as IntercourseIntention != null && !Hero.OneToOneConversationHero.HasMet)
                        .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                        .GotoDialogState("start_intercourse")
                    .NpcOption("{npc_starts_interaction_known}[ib:nervous2][if:convo_confused_normal]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as IntercourseIntention != null && Hero.OneToOneConversationHero.HasMet)
                        .GotoDialogState("start_intercourse")
                .EndNpcOptions();

            DialogFlow intercourseflow = DialogFlow.CreateDialogFlow("start_intercourse")
                .BeginPlayerOptions()
                    .PlayerOption("{player_interaction_start_react_yes}")
                        .BeginNpcOptions()
                            .NpcOption("{npc_interaction_sex_nofriend_1}[ib:nervous][if:convo_excited]", () => !Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && !Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero))
                                .Variation("{npc_interaction_sex_nofriend_2}[ib:nervous][if:convo_excited]")
                                .GotoDialogState("reply_intercourse")
                            .NpcOption("{npc_interaction_sex_friend_1}[ib:nervous][if:convo_excited]", () => Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero))
                                .Variation("{npc_interaction_sex_friend_2}[ib:nervous][if:convo_excited]")
                                .GotoDialogState("reply_intercourse")
                            .NpcOption("{npc_interaction_sex_friend_wb_1}[ib:confident2][if:convo_focused_happy]", () => Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero))
                                .Variation("{npc_interaction_sex_friend_wb_2}[ib:nervous][if:convo_excited]")
                                .GotoDialogState("reply_intercourse")
                            .NpcOption("{npc_interaction_sex_married_1}[ib:demure][if:convo_bemused]", () => Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != null && !Hero.OneToOneConversationHero.IsSpouseOf(Hero.MainHero))
                                .Variation("{npc_interaction_sex_married_2}[ib:demure][if:convo_bemused]")
                                .GotoDialogState("reply_intercourse")
                            .NpcOption("{npc_interaction_sex_else_1}[ib:demure][if:convo_bemused]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.IsSpouseOf(Hero.MainHero)))
                                .Variation("{npc_interaction_sex_else_2}[ib:demure][if:convo_bemused]")
                                .GotoDialogState("reply_intercourse")
                        .EndNpcOptions()
                    .PlayerOption("{player_interaction_start_react_no}")
                        .Consequence(() => ConversationTools.EndConversation())
                        .CloseDialog()
                .EndPlayerOptions();

            DialogFlow intercourseflow2 = DialogFlow.CreateDialogFlow("reply_intercourse")
                .BeginPlayerOptions()
                    .PlayerOption("{player_reaction_sex_nofriend_yes}")
                        .Condition(() => !Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && !Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero))
                        .Consequence(() => { _accepted = true; ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .PlayerOption("{player_reaction_sex_friend_yes}")
                        .Condition(() => Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero))
                        .Consequence(() => { _accepted = true; ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .PlayerOption("{player_reaction_sex_friend_wb_yes}")
                        .Condition(() => Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero))
                        .Consequence(() => { _accepted = true; ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .PlayerOption("{player_reaction_sex_married_yes}")
                        .Condition(() => Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != null && !Hero.OneToOneConversationHero.IsSpouseOf(Hero.MainHero))
                        .Consequence(() => { _accepted = true; ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .PlayerOption("{player_reaction_sex_else_yes}")
                        .Condition(() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.IsSpouseOf(Hero.MainHero)))
                        .Consequence(() => { _accepted = true; ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .PlayerOption("{player_reaction_no}")
                        .Consequence(() => { _accepted = false; ConversationTools.EndConversation(); })
                        .CloseDialog()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
            Campaign.Current.ConversationManager.AddDialogFlow(intercourseflow);
            Campaign.Current.ConversationManager.AddDialogFlow(intercourseflow2);
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_interaction_unknown.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time."
            ConversationLines.npc_starts_interaction_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, true));// "{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?");
            ConversationLines.player_interaction_start_react_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord029}Of course {TITLE}. How can I be of service?");
            ConversationLines.player_interaction_start_react_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
            ConversationLines.npc_interaction_sex_nofriend_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord041}I will be blunt with you, {TITLE}. I have urgent needs which call for satisfaction and you look like a person who can be discreet. Can I ask for your help?");
            ConversationLines.npc_interaction_sex_nofriend_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord042}This is very embarrassing for me, {TITLE}. I have a certain itch which requires attention, and I was hoping you could... scratch... my itch. What do you say?");
            ConversationLines.npc_interaction_sex_friend_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord043}You are {TITLE} and I trust you. I was wondering if we could help each other in terms of... natural urges... on a regular basis. Would you like that?");
            ConversationLines.npc_interaction_sex_friend_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord044}I was thinking if we could help each other, {TITLE}. In terms of pleasure, to be blunt. We could meet sometimes and enjoy each other. What do you say?");
            ConversationLines.npc_interaction_sex_friend_wb_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord045}I was looking forward to seeing you, {TITLE}. Would you care for a chat and maybe some bed excercise afterwards?");
            ConversationLines.npc_interaction_sex_friend_wb_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord046}Oh, {TITLE}. I was hoping you had some time for a conversation and some anatomical studies after. Are you interested?");
            ConversationLines.npc_interaction_sex_married_1.SetTextVariable("SPOUSE", IntentionHero.Spouse?.Name);// "{=Dramalord047}{SPOUSE} is not around I need to feel you, {TITLE}. Let's head to the bedchamber and enjoy ourselves. Would you like that?");
            ConversationLines.npc_interaction_sex_married_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord047}{SPOUSE} is not around I need to feel you, {TITLE}. Let's head to the bedchamber and enjoy ourselves. Would you like that?");
            ConversationLines.npc_interaction_sex_married_2.SetTextVariable("SPOUSE", IntentionHero.Spouse?.Name);// "{=Dramalord048}Now that {SPOUSE} is not around, I was thinking that you and I could use the empty bed for ourselves, {TITLE}. Does this tempt you?");
            ConversationLines.npc_interaction_sex_married_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord048}Now that {SPOUSE} is not around, I was thinking that you and I could use the empty bed for ourselves, {TITLE}. Does this tempt you?");
            ConversationLines.npc_interaction_sex_else_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord049}Come here and kiss me, {TITLE}. I'm craving for your body and want to enjoy you with all the lust and pleasure there is!");
            ConversationLines.npc_interaction_sex_else_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord050}Let's get rid of that clothes of yours. I show you a different kind of battle in my bedchamber where both sides win!");
            ConversationLines.player_reaction_sex_nofriend_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord060}Don't worry {TITLE}. I understand how you feel and I will gladly help you.");
            ConversationLines.player_reaction_sex_friend_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord061}Interesting proposition, {TITLE}. I think I like the idea.");
            ConversationLines.player_reaction_sex_friend_wb_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord062}Of course, {TITLE}. I could use some entertainment.");
            ConversationLines.player_reaction_sex_married_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord063}Well, things we do for excitement... Lead the way {TITLE}!");
            ConversationLines.player_reaction_sex_else_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord064}As you wish. I can garantuee you will not sleep much, {TITLE}");
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
        }
    }
}
