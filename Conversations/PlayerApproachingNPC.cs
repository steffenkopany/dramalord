using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class PlayerApproachingNPC
    {
        private static int _randomChance = 0;
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_approach_start", "hero_main_options", "npc_approach_reply", "{=Dramalord100}{TITLE}, may I occupy a few minutes of your time?", ConditionPlayerCanApproach, null);

            starter.AddDialogLine("npc_approach_reply_yes", "npc_approach_reply", "player_interaction_selection", "{=Dramalord029}Of course {TITLE}. How can I be of service?", ConditionNpcAcceptsApproach, null);
            starter.AddDialogLine("npc_approach_reply_no", "npc_approach_reply", "close_window", "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.", ConditionNpcDeclinesApproach, null);

            starter.AddPlayerLine("player_interaction_talk", "player_interaction_selection", "npc_interaction_reply_talk", "{=Dramalord031}I always like to hear new stories. Tell me about your latest exploits while traveling in the realm.", null, null);
            starter.AddPlayerLine("player_interaction_flirt", "player_interaction_selection", "npc_interaction_reply_flirt", "{=Dramalord034}Would you mind going for a walk with me? I want to cause jealousy with your outstanding appearance.", null, null);
            starter.AddPlayerLine("player_interaction_date_first", "player_interaction_selection", "npc_interaction_reply_date", "{=Dramalord035}I must confess, I can not stop thinking of you. I clearly have feelings for you, {TITLE}, and would like to bring our relationship to the next level. What do you say?", ConditionPlayerCanAskDateFirst, null);
            starter.AddPlayerLine("player_interaction_date", "player_interaction_selection", "npc_interaction_reply_date", "{=Dramalord038}I was looking forward to see you, {TITLE}. Would you like to retreat somewhere more silent, for a more private conversation?", ConditionPlayerCanAskDate, null);
            starter.AddPlayerLine("player_interaction_sex_friend", "player_interaction_selection", "npc_interaction_reply_sex", "{=Dramalord044}I was thinking if we could help each other, {TITLE}. In terms of pleasure, to be blunt. We could meet sometimes and enjoy each other. What do you say?", ConditionPlayerCanAskSexFriend, null);
            starter.AddPlayerLine("player_interaction_sex_fwb", "player_interaction_selection", "npc_interaction_reply_sex", "{=Dramalord045}I was looking forward to see you, {TITLE}. Would you care for a chat and maybe some bed excercise afterwards?", ConditionPlayerCanAskSexFWB, null);
            starter.AddPlayerLine("player_interaction_sex", "player_interaction_selection", "npc_interaction_reply_sex", "{=Dramalord050}Let's get rid off that clothes of yours. I show you a different kind of battle in my bedchamber where both sides win!", ConditionPlayerCanAskSexElse, null);
            starter.AddPlayerLine("player_interaction_engage", "player_interaction_selection", "npc_interaction_reply_engage", "{=Dramalord051}I love you very much, {TITLE}. I think it's time for us to take the next step in our relationship. Will you marry me?", ConditionPlayerCanAskEngage, null);
            starter.AddPlayerLine("player_interaction_marry", "player_interaction_selection", "npc_interaction_reply_marriage", "{=Dramalord053}Now that we are in a settlement, {TITLE}, let's call a priest and finally get married! What do you say, {TITLE}?", ConditionPlayerCanAskMarriage, null);
            starter.AddPlayerLine("player_interaction_breakup", "player_interaction_selection", "npc_interaction_reply_breakup", "{=Dramalord229}It hurts me, {TITLE}, but I have to put an end to our relationship.", ConditionPlayerCanAskBreakup, null);
            starter.AddPlayerLine("player_interaction_abort", "player_interaction_selection", "npc_interaction_abort", "{=Dramalord101}Let's talk about something else, {TITLE}.", null, null);

            starter.AddDialogLine("npc_interaction_abort", "npc_interaction_abort", "hero_main_options", "{=Dramalord186}As you wish, {TITLE}.", null, null);

            starter.AddDialogLineWithVariation("npc_interaction_reply_talk", "npc_interaction_reply_talk", "close_window", ConditionNpcAcceptsTalk, ConsequenceNpcAcceptsTalk)
                .Variation("{=Dramalord055}Sure, let's have a conversation {TITLE}.")
                .Variation("{=Dramalord223}Of course, {TITLE}. Let me tell you a tale of war and power...");

            starter.AddDialogLine("npc_interaction_reply_talk_timeout", "npc_interaction_reply_talk", "player_interaction_selection", "{=Dramalord105}Again? Give me some rest, {TITLE}. Let's talk about it later.", ConditionNpcDeclinesTalkTimeout, null);

            starter.AddDialogLineWithVariation("npc_interaction_reply_flirt_yes", "npc_interaction_reply_flirt", "close_window", ConditionNpcAcceptsFlirt, ConsequenceNpcAcceptsFlirt)
                .Variation("{=Dramalord056}Of course {TITLE}, I would love to join you.")
                .Variation("{=Dramalord224}Oh {TITLE}, you silly! All eyes will turn on you anyway, and I will enjoy that.");

            starter.AddDialogLine("npc_interaction_reply_flirt_no", "npc_interaction_reply_flirt", "player_interaction_selection", "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.", ConditionNpcDeclinesFlirt, null);
            starter.AddDialogLine("npc_interaction_reply_flirt_timeout", "npc_interaction_reply_flirt", "player_interaction_selection", "{=Dramalord105}Again? Give me some rest, {TITLE}. Let's talk about it later.", ConditionNpcDeclinesFlirtTimeout, null);

            starter.AddDialogLineWithVariation("npc_interaction_reply_date_first_yes", "npc_interaction_reply_date", "close_window", ConditionNpcAcceptsDateFirst, ConsequenceNpcAcceptsDateFirst)
                .Variation("{=Dramalord057}Oh {TITLE}, I wish for the same!")
                .Variation("{=Dramalord225}You know {TITLE}... I dreamed of this moment since I laid eyes on you!");

            starter.AddDialogLineWithVariation("npc_interaction_reply_date_yes", "npc_interaction_reply_date", "close_window", ConditionNpcAcceptsDate, ConsequenceNpcAcceptsDate)
                .Variation("{=Dramalord058}Sure {TITLE}, I enjoy every minute with you.")
                .Variation("{=Dramalord226}Of course {TITLE}. Let us spend some quality time, just the two of us.");

            starter.AddDialogLine("npc_interaction_reply_date_no", "npc_interaction_reply_date", "player_interaction_selection", "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.", ConditionNpcDeclinesDate, null);
            starter.AddDialogLine("npc_interaction_reply_date_husband", "npc_interaction_reply_date", "player_interaction_selection", "{=Dramalord102}Apologies, {TITLE}, but my spouse is around I we can not risk it.", ConditionNpcDeclinesDateHusband, null);
            starter.AddDialogLine("npc_interaction_reply_date_timeout", "npc_interaction_reply_date", "player_interaction_selection", "{=Dramalord105}Again? Give me some rest, {TITLE}. Let's talk about it later.", ConditionNpcDeclinesDateTimeout, null);

            starter.AddDialogLine("npc_interaction_reply_sex_friend_yes", "npc_interaction_reply_sex", "close_window", "{=Dramalord061}Interesting proposition, {TITLE}. I think I like the idea.", ConditionNpcAcceptsSexFriend, ConsequenceNpcAcceptsSexFriend);
            starter.AddDialogLine("npc_interaction_reply_sex_fwb_yes", "npc_interaction_reply_sex", "close_window", "{=Dramalord062}Of course, {TITLE}. I could use some entertainment.", ConditionNpcAcceptsSexFWB, ConsequenceNpcAcceptsSexFWB);
            starter.AddDialogLine("npc_interaction_reply_sex_fwb_no", "npc_interaction_reply_sex", "player_interaction_selection", "{=Dramalord239}Not today, {TITLE}. I'm not in the mood, ask me some other time.", ConditionNpcDeclinesSexFWB, null);
            starter.AddDialogLine("npc_interaction_reply_sex_yes", "npc_interaction_reply_sex", "close_window", "{=Dramalord064}As you wish. I can garantuee you will not sleep much, {TITLE}.", ConditionNpcAcceptsSex, ConsequenceNpcAcceptsSex);
            starter.AddDialogLine("npc_interaction_reply_sex_no_husband", "npc_interaction_reply_sex", "player_interaction_selection", "{=Dramalord102}Apologies, {TITLE}, but my spouse is around I we can not risk it.", ConditionNpcDeclinesSexHusband, null);
            starter.AddDialogLine("npc_interaction_reply_sex_no_interest", "npc_interaction_reply_sex", "player_interaction_selection", "{=Dramalord103}Apologies, {TITLE}, but I will not do that with you.", ConditionNpcDeclinesSexInterest, null);
            starter.AddDialogLine("npc_interaction_reply_sex_no_timeout", "npc_interaction_reply_sex", "player_interaction_selection", "{=Dramalord105}Again? Give me some rest, {TITLE}. Let's talk about it later.", ConditionNpcDeclinesSexTimeout, null);

            starter.AddDialogLine("npc_interaction_reply_engage_yes", "npc_interaction_reply_engage", "close_window", "{=Dramalord065}You make my dream come true, {TITLE}. Yes I would love to marry you!", ConditionNpcAcceptsEngagement, ConsequenceNpcAcceptsEngagement);
            starter.AddDialogLine("npc_interaction_reply_engage_no", "npc_interaction_reply_engage", "player_interaction_selection", "{=Dramalord104}I am sorry {TITLE}, but I am not ready for this step just yet.", ConditionNpcDeclinesEngagement, null);

            starter.AddDialogLine("npc_interaction_reply_marry_yes", "npc_interaction_reply_marriage", "close_window", "{=Dramalord067}I agree, {TITLE}. Let's seal that bond of ours.", ConditionNpcAcceptsMarriage, ConsequenceNpcAcceptsMarriage);
            starter.AddDialogLine("npc_interaction_reply_marry_no", "npc_interaction_reply_marriage", "player_interaction_selection", "{=Dramalord104}I am sorry {TITLE}, but I am not ready for this step just yet.", ConditionNpcDeclinesMarriage, null);

            starter.AddDialogLine("player_interaction_breakup_love", "npc_interaction_reply_breakup", "close_window", "{=Dramalord230}Oh no,{TITLE}! You are breaking my heart! But I have no other choice then to accept your decision.", ConditionNpcAcceptsBreakupLove, ConsequenceNpcAcceptsBreakup);
            starter.AddDialogLine("player_interaction_breakup_nolove", "npc_interaction_reply_breakup", "close_window", "{=Dramalord231}Well, eventually it always comes to that, right {TITLE}. It was nice at long it lasted. Thank you.", ConditionNpcAcceptsBreakupNoLove, ConsequenceNpcAcceptsBreakup);
            starter.AddDialogLine("player_interaction_breakup_leave", "npc_interaction_reply_breakup", "close_window", "{=Dramalord232}I.. I don't know what to say. This is shocking me. I have to think if I can stay around you...", ConditionNpcAcceptsBreakupLeave, ConsequenceNpcAcceptsBreakupLeave);
        }

        private static bool NoTimeout() => CampaignTime.Now.ToDays - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).LastInteraction > DramalordMCM.Instance.DaysBetweenInteractions;
        private static bool HusbandClose() => Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.OneToOneConversationHero.IsCloseTo(Hero.OneToOneConversationHero.Spouse) && _randomChance*-1 < Hero.OneToOneConversationHero.GetPersonality().Conscientiousness;

        private static bool ConditionPlayerCanApproach() 
        {
            _randomChance = MBRandom.RandomInt(1, 100);
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(true));  
            return Hero.OneToOneConversationHero.IsDramalordLegit(); 
        }

        private static bool ConditionNpcAcceptsApproach()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationWithPlayer() > -30;
        }

        private static bool ConditionNpcDeclinesApproach()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationWithPlayer() <= -30;
        }

        private static bool ConditionPlayerCanAskDateFirst()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love >= 30 && !Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerCanAskDate()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerCanAskSexElse()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerCanAskSexFriend()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.IsFriendOf(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerCanAskSexFWB()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.IsFriendWithBenefitsOf(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerCanAskEngage()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.IsLoverOf(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerCanAskMarriage()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.IsBetrothedOf(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerCanAskBreakup()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);
        }

        private static bool ConditionNpcAcceptsFlirt()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) >= DramalordMCM.Instance.MinAttraction && NoTimeout();
        }

        private static bool ConditionNpcAcceptsTalk()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return NoTimeout();
        }

        private static bool ConditionNpcDeclinesTalkTimeout()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !NoTimeout();
        }

        private static bool ConditionNpcDeclinesFlirt()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) < DramalordMCM.Instance.MinAttraction && NoTimeout();
        }

        private static bool ConditionNpcDeclinesFlirtTimeout()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !NoTimeout();
        }

        private static bool ConditionNpcAcceptsDateFirst()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !HusbandClose() && NoTimeout() && (Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= 30 && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero));
        }

        private static bool ConditionNpcAcceptsDate()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionNpcDeclinesDate()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return NoTimeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < 50;
        }

        private static bool ConditionNpcDeclinesDateHusband()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionNpcDeclinesDateTimeout()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !NoTimeout();
        }

        private static bool ConditionNpcAcceptsSexFriend()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 75 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Trust >= 75;
        }

        private static bool ConditionNpcAcceptsSexFWB()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 50;
        }

        private static bool ConditionNpcDeclinesSexFWB()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny < 50;
        }

        private static bool ConditionNpcAcceptsSex()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 25;
        }

        private static bool ConditionNpcDeclinesSexHusband()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);
        }

        private static bool ConditionNpcDeclinesSexTimeout()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !NoTimeout() && Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);
        }

        private static bool ConditionNpcDeclinesSexInterest()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return (!Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetDesires().Horny < 25);
        }

        private static bool ConditionNpcAcceptsEngagement()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= 75;
        }

        private static bool ConditionNpcDeclinesEngagement()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < 75;
        }

        private static bool ConditionNpcAcceptsMarriage()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinMarriageLove;
        }

        private static bool ConditionNpcDeclinesMarriage()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinMarriageLove;
        }

        private static bool ConditionNpcAcceptsBreakupLove()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= 30 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism < _randomChance;
        }

        private static bool ConditionNpcAcceptsBreakupNoLove()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < 30;
        }

        private static bool ConditionNpcAcceptsBreakupLeave()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= 30 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism >= _randomChance;
        }

        //CONSEQUENCE
        private static void ConsequenceNpcAcceptsTalk()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.SmallTalk, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsFlirt()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Flirt, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsDateFirst()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Date, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
        private static void ConsequenceNpcAcceptsDate()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Date, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsSexFriend()
        {
            FriendsWithBenefitsAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Intercourse, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsSexFWB()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Intercourse, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsSex()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Intercourse, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsEngagement()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Engagement, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsMarriage()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Marriage, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsBreakup()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.BreakUp, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcAcceptsBreakupLeave()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.BreakUp, Hero.OneToOneConversationHero, -1);
            DramalordIntentions.Instance.AddIntention(Hero.OneToOneConversationHero, Hero.MainHero, IntentionType.LeaveClan, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
