﻿using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Dramalord.Conversations
{
    internal static class PlayerApproachingNPC
    {
        private static int _randomChance = 0;

        private static TextObject player_approach_start = new("{=Dramalord100}{TITLE}, may I occupy a few minutes of your time?");
        private static TextObject npc_approach_reply_yes = new("{=Dramalord029}Of course {TITLE}. How can I be of service?");
        private static TextObject npc_approach_reply_no = new("{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
        private static TextObject player_interaction_talk = new("{=Dramalord339}I know you have good stories, tell me some of them! (Friendly chat)");
        private static TextObject player_interaction_ask = new("{=Dramalord318}{TITLE} I always wondered what looks you prefer. (Information)");
        private static TextObject player_interaction_flirt = new("{=Dramalord340}You look amazing, but I'm sure you already know that... (Flirting)");
        private static TextObject player_interaction_date_first = new("{=Dramalord341}I must confess I have feelings for you, {TITLE}! (Love Affair)");
        private static TextObject player_interaction_date = new("{=Dramalord342}There's nothing I'd like more than to be with you and only you... (Dating)");
        private static TextObject player_interaction_sex_friend = new("{=Dramalord343}Do you urge for pleaseure as well, {TITLE}? (Friend with benefits)");
        private static TextObject player_interaction_sex_fwb = new("{=Dramalord344}Would you care for some bed excercise, {TITLE}? (Intimacy)");
        private static TextObject player_interaction_sex = new("{=Dramalord345}I want you. I know you feel the same way. Meet me in my chambers. (Intimacy)");
        private static TextObject player_interaction_engage = new("{=Dramalord346}I love you very much, {TITLE}. Will you marry me? (Betrothed)");
        private static TextObject player_interaction_marry = new("{=Dramalord347}We are in a settlement, {TITLE}, let's  get married! (Marriage)");
        private static TextObject player_interaction_breakup = new("{=Dramalord348}It's just not working between you and I. Let's end this farce before it festers. (Break up)");
        private static TextObject player_interaction_gift = new("{=Dramalord292}{TITLE}, let me give you this exceptional {GIFT} as a token of my affection.");
        private static TextObject player_interaction_abort = new("{=Dramalord101}Let's talk about something else, {TITLE}.");
        private static TextObject npc_interaction_abort = new("{=Dramalord186}As you wish, {TITLE}.");
        private static TextObject npc_interaction_reply_talk_1 = new("{=Dramalord055}Sure, let's have a conversation {TITLE}.");
        private static TextObject npc_interaction_reply_talk_2 = new("{=Dramalord223}Of course, {TITLE}. Let me tell you a tale of war and power...");
        private static TextObject npc_interaction_reply_talk_timeout = new("{=Dramalord105}Again? Give me some rest, {TITLE}. Let's talk about it later.");
        private static TextObject npc_interaction_reply_ask_deny = new("{=Dramalord319}Apologies, {TITLE}, but I don't trust you enough to tell you about my personal preferences.");
        private static TextObject npc_interaction_reply_ask_accept = new("{=Dramalord320}Certainly, {TITLE}. If you really want know I can tell you...");
        private static TextObject npc_interaction_reply_orientation_hetero = new("{=Dramalord321}I feel drawn to the other sex and I don't have much interest in persons of my own.");
        private static TextObject npc_interaction_reply_orientation_gay = new("{=Dramalord322}I don't have much interest in the other sex, I rather prefer persons of my own.");
        private static TextObject npc_interaction_reply_orientation_bi = new("{=Dramalord323}I find people of the other sex attractive, but also feel drawn to those of my own.");
        private static TextObject npc_interaction_reply_orientation_none = new("{=Dramalord324}I don't have any affections for either sex. They don't interest me much.");
        private static TextObject npc_interaction_reply_weight_thin = new("{=Dramalord325}I think slim people are more grazile then others.");
        private static TextObject npc_interaction_reply_weight_normal = new("{=Dramalord326}I don't like thin or fat. The middle is just right.");
        private static TextObject npc_interaction_reply_weight_fat = new("{=Dramalord327}I like people with more weight. There's more to grab for me.");
        private static TextObject npc_interaction_reply_build_low = new("{=Dramalord328}Muscles are overrated. I like it skinny and want to see bones.");
        private static TextObject npc_interaction_reply_build_average = new("{=Dramalord329}Medium muscles are just right for me. I don't need something special.");
        private static TextObject npc_interaction_reply_build_high = new("{=Dramalord330}I love powerful people. The bulkier the better.");
        private static TextObject npc_interaction_reply_age = new("{=Dramalord331}I like people who are {AGEDIFF} then me. Best around the age of {AGE}.");
        private static TextObject npc_interaction_reply_summary = new("{=Dramalord332}You {TITLE}, I think you are a {RATING} out of 100, I would say.");

        private static TextObject npc_interaction_reply_flirt_yes_1 = new("{=Dramalord056}Of course {TITLE}, I would love to join you.");
        private static TextObject npc_interaction_reply_flirt_yes_2 = new("{=Dramalord224}Oh {TITLE}, you silly! All eyes will turn on you anyway, and I will enjoy that.");
        private static TextObject npc_interaction_reply_talk_no = new("{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
        private static TextObject npc_interaction_reply_date_first_yes_1 = new("{=Dramalord057}Oh {TITLE}, I wish for the same!");
        private static TextObject npc_interaction_reply_date_first_yes_2 = new("{=Dramalord225}You know {TITLE}... I dreamed of this moment since I laid eyes on you!");
        private static TextObject npc_interaction_reply_date_yes_1 = new("{=Dramalord058}Sure {TITLE}, I enjoy every minute with you.");
        private static TextObject npc_interaction_reply_date_yes_2 = new("{=Dramalord226}Of course {TITLE}. Let us spend some quality time, just the two of us.");
        private static TextObject npc_interaction_reply_date_husband = new("{=Dramalord102}Apologies, {TITLE}, but my spouse is around I we can not risk it.");
        private static TextObject npc_interaction_reply_sex_friend_yes = new("{=Dramalord061}Interesting proposition, {TITLE}. I think I like the idea.");
        private static TextObject npc_interaction_reply_sex_fwb_yes = new("{=Dramalord062}Of course, {TITLE}. I could use some entertainment.");
        private static TextObject npc_interaction_reply_sex_fwb_no = new("{=Dramalord239}Not today, {TITLE}. I'm not in the mood, ask me some other time.");
        private static TextObject npc_interaction_reply_sex_yes = new("{=Dramalord064}As you wish. I can garantuee you will not sleep much, {TITLE}.");
        private static TextObject npc_interaction_reply_sex_no_interest = new("{=Dramalord103}Apologies, {TITLE}, but I will not do that with you.");
        private static TextObject npc_interaction_reply_engage_yes = new("{=Dramalord065}You make my dream come true, {TITLE}. Yes I would love to marry you!");
        private static TextObject npc_interaction_reply_engage_no = new("{=Dramalord104}I am sorry {TITLE}, but I am not ready for this step just yet.");
        private static TextObject npc_interaction_reply_marry_yes = new("{=Dramalord067}I agree, {TITLE}. Let's seal that bond of ours.");
        private static TextObject npc_interaction_breakup_love = new("{=Dramalord230}Oh no, {TITLE}! You are breaking my heart! But I have no other choice then to accept your decision.");
        private static TextObject npc_interaction_breakup_nolove = new("{=Dramalord231}Well, eventually it always comes to that, right {TITLE}. It was nice at long it lasted. Thank you.");
        private static TextObject npc_interaction_breakup_leave = new("{=Dramalord232}I.. I don't know what to say. This is shocking me. I have to think if I can stay around you...");
        private static TextObject npc_interaction_reply_gift = new("{=Dramalord293}Thank you {TITLE}! I will keep it close to my... bed as a reminder of your... affection and enjoy it every day!");

        private static void SetupLines()
        {
            player_approach_start.SetTextVariable("TITLE", ConversationHelper.NpcTitle(true));
            npc_approach_reply_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_approach_reply_no.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            player_interaction_talk.SetTextVariable("TITLE", ConversationHelper.NpcTitle(true));
            player_interaction_flirt.SetTextVariable("TITLE", ConversationHelper.NpcTitle(true));
            player_interaction_ask.SetTextVariable("TITLE", ConversationHelper.NpcTitle(true));
            player_interaction_date_first.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_interaction_sex_friend.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_interaction_sex_fwb.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            player_interaction_sex.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_interaction_engage.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            player_interaction_marry.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            player_interaction_gift.SetTextVariable("TITLE", ConversationHelper.NpcTitle(true));
            player_interaction_gift.SetTextVariable("GIFT", Hero.OneToOneConversationHero.IsFemale ? new TextObject("{=Dramalord240}Sausage") : new TextObject("{=Dramalord241}Pie"));
            player_interaction_abort.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            npc_interaction_abort.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_talk_1.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_talk_2.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_talk_timeout.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_ask_deny.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_ask_accept.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));

            int diff = Hero.OneToOneConversationHero.GetDesires().AttractionAgeDiff;
            TextObject ageDiff = (diff < 5) ? new TextObject("{Dramalord333}younger") : (diff > 5) ? new TextObject("{Dramalord334}older") : new TextObject("{Dramalord335}not older and not younger");
            TextObject age = new TextObject(MBMath.ClampInt((int)Hero.OneToOneConversationHero.Age + diff, 18, 120));
            npc_interaction_reply_age.SetTextVariable("AGEDIFF", ageDiff);
            npc_interaction_reply_age.SetTextVariable("AGED", age);
            npc_interaction_reply_summary.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_summary.SetTextVariable("RATING", new TextObject(Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero)));

            npc_interaction_reply_flirt_yes_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_flirt_yes_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_talk_no.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_date_first_yes_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_date_first_yes_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_date_yes_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_date_yes_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_date_husband.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_sex_friend_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_sex_fwb_yes.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_sex_fwb_no.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_sex_yes.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_sex_no_interest.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_engage_yes.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_engage_no.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_marry_yes.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_breakup_love.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_breakup_nolove.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_breakup_leave.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_reply_gift.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));



            MBTextManager.SetTextVariable("player_approach_start", player_approach_start);
            MBTextManager.SetTextVariable("npc_approach_reply_yes", npc_approach_reply_yes);
            MBTextManager.SetTextVariable("npc_approach_reply_no", npc_approach_reply_no);
            MBTextManager.SetTextVariable("player_interaction_talk", player_interaction_talk);
            MBTextManager.SetTextVariable("player_interaction_ask", player_interaction_ask);
            MBTextManager.SetTextVariable("player_interaction_flirt", player_interaction_flirt);
            MBTextManager.SetTextVariable("player_interaction_date_first", player_interaction_date_first);
            MBTextManager.SetTextVariable("player_interaction_date", player_interaction_date);
            MBTextManager.SetTextVariable("player_interaction_sex_friend", player_interaction_sex_friend);
            MBTextManager.SetTextVariable("player_interaction_sex_fwb", player_interaction_sex_fwb);
            MBTextManager.SetTextVariable("player_interaction_sex", player_interaction_sex);
            MBTextManager.SetTextVariable("player_interaction_engage", player_interaction_engage);
            MBTextManager.SetTextVariable("player_interaction_marry", player_interaction_marry);
            MBTextManager.SetTextVariable("player_interaction_breakup", player_interaction_breakup);
            MBTextManager.SetTextVariable("player_interaction_gift", player_interaction_gift);
            MBTextManager.SetTextVariable("player_interaction_abort", player_interaction_abort);
            MBTextManager.SetTextVariable("npc_interaction_abort", npc_interaction_abort);
            MBTextManager.SetTextVariable("npc_interaction_reply_talk_1", npc_interaction_reply_talk_1);
            MBTextManager.SetTextVariable("npc_interaction_reply_talk_2", npc_interaction_reply_talk_2);
            MBTextManager.SetTextVariable("npc_interaction_reply_talk_timeout", npc_interaction_reply_talk_timeout);
            MBTextManager.SetTextVariable("npc_interaction_reply_ask_deny", npc_interaction_reply_ask_deny);
            MBTextManager.SetTextVariable("npc_interaction_reply_ask_accept", npc_interaction_reply_ask_accept);
            MBTextManager.SetTextVariable("npc_interaction_reply_orientation_hetero", npc_interaction_reply_orientation_hetero);
            MBTextManager.SetTextVariable("npc_interaction_reply_orientation_gay", npc_interaction_reply_orientation_gay);
            MBTextManager.SetTextVariable("npc_interaction_reply_orientation_bi", npc_interaction_reply_orientation_bi);
            MBTextManager.SetTextVariable("npc_interaction_reply_orientation_none", npc_interaction_reply_orientation_none);
            MBTextManager.SetTextVariable("npc_interaction_reply_weight_thin", npc_interaction_reply_weight_thin);
            MBTextManager.SetTextVariable("npc_interaction_reply_weight_normal", npc_interaction_reply_weight_normal);
            MBTextManager.SetTextVariable("npc_interaction_reply_weight_fat", npc_interaction_reply_weight_fat);
            MBTextManager.SetTextVariable("npc_interaction_reply_build_low", npc_interaction_reply_build_low);
            MBTextManager.SetTextVariable("npc_interaction_reply_build_average", npc_interaction_reply_build_average);
            MBTextManager.SetTextVariable("npc_interaction_reply_build_high", npc_interaction_reply_build_high);
            MBTextManager.SetTextVariable("npc_interaction_reply_age", npc_interaction_reply_age);
            MBTextManager.SetTextVariable("npc_interaction_reply_summary", npc_interaction_reply_summary);

            MBTextManager.SetTextVariable("npc_interaction_reply_flirt_yes_1", npc_interaction_reply_flirt_yes_1);
            MBTextManager.SetTextVariable("npc_interaction_reply_flirt_yes_2", npc_interaction_reply_flirt_yes_2);
            MBTextManager.SetTextVariable("npc_interaction_reply_talk_no", npc_interaction_reply_talk_no);
            MBTextManager.SetTextVariable("npc_interaction_reply_date_first_yes_1", npc_interaction_reply_date_first_yes_1);
            MBTextManager.SetTextVariable("npc_interaction_reply_date_first_yes_2", npc_interaction_reply_date_first_yes_2);
            MBTextManager.SetTextVariable("npc_interaction_reply_date_yes_1", npc_interaction_reply_date_yes_1);
            MBTextManager.SetTextVariable("npc_interaction_reply_date_yes_2", npc_interaction_reply_date_yes_2);
            MBTextManager.SetTextVariable("npc_interaction_reply_date_husband", npc_interaction_reply_date_husband);
            MBTextManager.SetTextVariable("npc_interaction_reply_sex_friend_yes", npc_interaction_reply_sex_friend_yes);
            MBTextManager.SetTextVariable("npc_interaction_reply_sex_fwb_yes", npc_interaction_reply_sex_fwb_yes);
            MBTextManager.SetTextVariable("npc_interaction_reply_sex_fwb_no", npc_interaction_reply_sex_fwb_no);
            MBTextManager.SetTextVariable("npc_interaction_reply_sex_yes", npc_interaction_reply_sex_yes);
            MBTextManager.SetTextVariable("npc_interaction_reply_sex_no_interest", npc_interaction_reply_sex_no_interest);
            MBTextManager.SetTextVariable("npc_interaction_reply_engage_yes", npc_interaction_reply_engage_yes);
            MBTextManager.SetTextVariable("npc_interaction_reply_engage_no", npc_interaction_reply_engage_no);
            MBTextManager.SetTextVariable("npc_interaction_reply_marry_yes", npc_interaction_reply_marry_yes);
            MBTextManager.SetTextVariable("npc_interaction_breakup_love", npc_interaction_breakup_love);
            MBTextManager.SetTextVariable("npc_interaction_breakup_nolove", npc_interaction_breakup_nolove);
            MBTextManager.SetTextVariable("npc_interaction_breakup_leave", npc_interaction_breakup_leave);
            MBTextManager.SetTextVariable("npc_interaction_reply_gift", npc_interaction_reply_gift);
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_approach_start", "hero_main_options", "npc_approach_reply", "{player_approach_start}", ConditionPlayerCanApproach, null);

            starter.AddDialogLine("npc_approach_reply_yes", "npc_approach_reply", "player_interaction_selection", "{npc_approach_reply_yes}", ConditionNpcAcceptsApproach, null);
            starter.AddDialogLine("npc_approach_reply_no", "npc_approach_reply", "close_window", "{npc_approach_reply_no}", ConditionNpcDeclinesApproach, null);

            starter.AddPlayerLine("player_interaction_talk", "player_interaction_selection", "npc_interaction_reply_talk", "{player_interaction_talk}", null, null);
            starter.AddPlayerLine("player_interaction_ask", "player_interaction_selection", "npc_interaction_reply_ask", "{player_interaction_ask}", ConditionPersonalInfoUnknown, null);
            starter.AddPlayerLine("player_interaction_flirt", "player_interaction_selection", "npc_interaction_reply_flirt", "{player_interaction_flirt}", null, null);
            starter.AddPlayerLine("player_interaction_date_first", "player_interaction_selection", "npc_interaction_reply_date", "{player_interaction_date_first}", ConditionPlayerCanAskDateFirst, null);
            starter.AddPlayerLine("player_interaction_date", "player_interaction_selection", "npc_interaction_reply_date", "{player_interaction_date}", null, null);
            starter.AddPlayerLine("player_interaction_sex_friend", "player_interaction_selection", "npc_interaction_reply_sex", "{player_interaction_sex_friend}", ConditionPlayerCanAskSexFriend, null);
            starter.AddPlayerLine("player_interaction_sex_fwb", "player_interaction_selection", "npc_interaction_reply_sex", "{player_interaction_sex_fwb}", ConditionPlayerCanAskSexFWB, null);
            starter.AddPlayerLine("player_interaction_sex", "player_interaction_selection", "npc_interaction_reply_sex", "{player_interaction_sex}", ConditionPlayerCanAskSexElse, null);
            starter.AddPlayerLine("player_interaction_engage", "player_interaction_selection", "npc_interaction_reply_engage", "{player_interaction_engage}", ConditionPlayerCanAskEngage, null);
            starter.AddPlayerLine("player_interaction_marry", "player_interaction_selection", "npc_interaction_reply_marriage", "{player_interaction_marry}", ConditionPlayerCanAskMarriage, null);
            starter.AddPlayerLine("player_interaction_breakup", "player_interaction_selection", "npc_interaction_reply_breakup", "{player_interaction_breakup}", ConditionPlayerCanAskBreakup, null);
            starter.AddPlayerLine("player_interaction_gift", "player_interaction_selection", "npc_interaction_reply_gift", "{player_interaction_gift}", ConditionPlayerCanGiveGift, null);
            starter.AddPlayerLine("player_interaction_abort", "player_interaction_selection", "npc_interaction_abort", "{player_interaction_abort}", null, null);

            starter.AddDialogLine("npc_interaction_abort", "npc_interaction_abort", "hero_main_options", "{npc_interaction_abort}", null, null);

            starter.AddDialogLineWithVariation("npc_interaction_reply_talk", "npc_interaction_reply_talk", "player_challenge_start", ConditionNpcAcceptsTalk, ConsequenceNpcAcceptsTalk)
                .Variation("{npc_interaction_reply_talk_1}")
                .Variation("{npc_interaction_reply_talk_2}");

            starter.AddDialogLine("npc_interaction_reply_talk_timeout", "npc_interaction_reply_talk", "player_interaction_selection", "{npc_interaction_reply_talk_timeout}", ConditionNpcDeclinesTalkTimeout, null);

            starter.AddDialogLine("npc_interaction_reply_ask_deny", "npc_interaction_reply_ask", "player_interaction_selection", "{npc_interaction_reply_ask_deny}", ConditionNpcDeclinesAsk, null);
            starter.AddDialogLine("npc_interaction_reply_ask_accept", "npc_interaction_reply_ask", "npc_interaction_reply_orientation", "{npc_interaction_reply_ask_accept}", ConditionNpcAcceptsAsk, ConsequenceNpcAcceptsAsk);

            starter.AddDialogLine("npc_interaction_reply_orientation_hetero", "npc_interaction_reply_orientation", "npc_interaction_reply_weight", "{npc_interaction_reply_orientation_hetero}", ConditionNpcAcceptsAskHetero, null);
            starter.AddDialogLine("npc_interaction_reply_orientation_gay", "npc_interaction_reply_orientation", "npc_interaction_reply_weight", "{npc_interaction_reply_orientation_gay}", ConditionNpcAcceptsAskGay, null);
            starter.AddDialogLine("npc_interaction_reply_orientation_bi", "npc_interaction_reply_orientation", "npc_interaction_reply_weight", "{npc_interaction_reply_orientation_bi}", ConditionNpcAcceptsAskBi, null);
            starter.AddDialogLine("npc_interaction_reply_orientation_none", "npc_interaction_reply_orientation", "npc_interaction_reply_weight", "{npc_interaction_reply_orientation_none}", ConditionNpcAcceptsAskNone, null);

            starter.AddDialogLine("npc_interaction_reply_weight_thin", "npc_interaction_reply_weight", "npc_interaction_reply_build", "{npc_interaction_reply_weight_thin}", ConditionNpcAcceptsAskThin, null);
            starter.AddDialogLine("npc_interaction_reply_weight_normal", "npc_interaction_reply_weight", "npc_interaction_reply_build", "{npc_interaction_reply_weight_normal}", ConditionNpcAcceptsAskNormal, null);
            starter.AddDialogLine("npc_interaction_reply_weight_fat", "npc_interaction_reply_weight", "npc_interaction_reply_build", "{npc_interaction_reply_weight_fat}", ConditionNpcAcceptsAskFat, null);

            starter.AddDialogLine("npc_interaction_reply_build_low", "npc_interaction_reply_build", "npc_interaction_reply_age", "{npc_interaction_reply_build_low}", ConditionNpcAcceptsAskLow, null);
            starter.AddDialogLine("npc_interaction_reply_build_average", "npc_interaction_reply_build", "npc_interaction_reply_age", "{npc_interaction_reply_build_average}", ConditionNpcAcceptsAskAverage, null);
            starter.AddDialogLine("npc_interaction_reply_build_high", "npc_interaction_reply_build", "npc_interaction_reply_age", "{npc_interaction_reply_build_high}", ConditionNpcAcceptsAskHigh, null);

            starter.AddDialogLine("npc_interaction_reply_age", "npc_interaction_reply_age", "npc_interaction_reply_summary", "{npc_interaction_reply_age}", null, null);

            starter.AddDialogLine("npc_interaction_reply_summary", "npc_interaction_reply_summary", "player_interaction_selection", "{npc_interaction_reply_summary}", null, null);

            starter.AddDialogLineWithVariation("npc_interaction_reply_flirt_yes", "npc_interaction_reply_flirt", "player_challenge_start", ConditionNpcAcceptsFlirt, ConsequenceNpcAcceptsFlirt)
                .Variation("{npc_interaction_reply_flirt_yes_1}")
                .Variation("{npc_interaction_reply_flirt_yes_2}");

            starter.AddDialogLine("npc_interaction_reply_talk_no", "npc_interaction_reply_flirt", "player_interaction_selection", "{npc_interaction_reply_talk_no}", ConditionNpcDeclinesFlirt, null);
            starter.AddDialogLine("npc_interaction_reply_talk_timeout", "npc_interaction_reply_flirt", "player_interaction_selection", "{npc_interaction_reply_talk_timeout}", ConditionNpcDeclinesFlirtTimeout, null);


            

            starter.AddDialogLineWithVariation("npc_interaction_reply_date_first_yes", "npc_interaction_reply_date", "player_challenge_start", ConditionNpcAcceptsDateFirst, ConsequenceNpcAcceptsDateFirst)
                .Variation("{npc_interaction_reply_date_first_yes_1}")
                .Variation("{npc_interaction_reply_date_first_yes_2}");

            starter.AddDialogLineWithVariation("npc_interaction_reply_date_yes", "npc_interaction_reply_date", "player_challenge_start", ConditionNpcAcceptsDate, ConsequenceNpcAcceptsDate)
                .Variation("{npc_interaction_reply_date_yes_1}")
                .Variation("{npc_interaction_reply_date_yes_2}");

            starter.AddDialogLine("npc_interaction_reply_date_first_maybe", "npc_interaction_reply_date", "npc_persuasion_challenge", "{=Dramalord437}Uh... well...", ConditionNpcMaybeDate, ConsequenceNpcMaybeDate);
            starter.AddDialogLine("npc_interaction_reply_date_no", "npc_interaction_reply_date", "player_interaction_selection", "{npc_interaction_reply_talk_no}", ConditionNpcDeclinesDate, null);
            starter.AddDialogLine("npc_interaction_reply_date_husband", "npc_interaction_reply_date", "player_interaction_selection", "{npc_interaction_reply_date_husband}", ConditionNpcDeclinesDateHusband, null);
            starter.AddDialogLine("npc_interaction_reply_talk_timeout", "npc_interaction_reply_date", "player_interaction_selection", "{npc_interaction_reply_talk_timeout}", ConditionNpcDeclinesDateTimeout, null);

            starter.AddDialogLine("npc_interaction_reply_sex_friend_yes", "npc_interaction_reply_sex", "close_window", "{npc_interaction_reply_sex_friend_yes}", ConditionNpcAcceptsSexFriend, ConsequenceNpcAcceptsSexFriend);
            starter.AddDialogLine("npc_interaction_reply_sex_friend_maybe", "npc_interaction_reply_sex", "npc_persuasion_challenge", "{=Dramalord437}Uh... well...", ConditionNpcMaybeSexFriend, ConsequenceNpcMaybeSexFriend);
            starter.AddDialogLine("npc_interaction_reply_sex_fwb_yes", "npc_interaction_reply_sex", "close_window", "{npc_interaction_reply_sex_fwb_yes}", ConditionNpcAcceptsSexFWB, ConsequenceNpcAcceptsSexFWB);
            starter.AddDialogLine("npc_interaction_reply_sex_fwb_no", "npc_interaction_reply_sex", "player_interaction_selection", "{npc_interaction_reply_sex_fwb_no}", ConditionNpcDeclinesSexFWB, null);
            starter.AddDialogLine("npc_interaction_reply_sex_yes", "npc_interaction_reply_sex", "close_window", "{npc_interaction_reply_sex_yes}", ConditionNpcAcceptsSex, ConsequenceNpcAcceptsSex);
            starter.AddDialogLine("npc_interaction_reply_date_husband", "npc_interaction_reply_sex", "player_interaction_selection", "{npc_interaction_reply_date_husband}", ConditionNpcDeclinesSexHusband, null);
            starter.AddDialogLine("npc_interaction_reply_sex_no_interest", "npc_interaction_reply_sex", "player_interaction_selection", "{npc_interaction_reply_sex_no_interest}", ConditionNpcDeclinesSexInterest, null);
            starter.AddDialogLine("npc_interaction_reply_talk_timeout", "npc_interaction_reply_sex", "player_interaction_selection", "{npc_interaction_reply_talk_timeout}", ConditionNpcDeclinesSexTimeout, null);

            starter.AddDialogLine("npc_interaction_reply_engage_yes", "npc_interaction_reply_engage", "close_window", "{npc_interaction_reply_engage_yes}", ConditionNpcAcceptsEngagement, ConsequenceNpcAcceptsEngagement);
            starter.AddDialogLine("npc_interaction_reply_engage_maybe", "npc_interaction_reply_engage", "npc_persuasion_challenge", "{=Dramalord437}Uh... well...", ConditionNpcMaybeEngagement, ConsequenceNpcMaybeEngagement);
            starter.AddDialogLine("npc_interaction_reply_engage_no", "npc_interaction_reply_engage", "player_interaction_selection", "{npc_interaction_reply_engage_no}", ConditionNpcDeclinesEngagement, null);

            starter.AddDialogLine("npc_interaction_reply_marry_yes", "npc_interaction_reply_marriage", "close_window", "{npc_interaction_reply_marry_yes}", ConditionNpcAcceptsMarriage, ConsequenceNpcAcceptsMarriage);
            starter.AddDialogLine("npc_interaction_reply_engage_no", "npc_interaction_reply_marriage", "player_interaction_selection", "{npc_interaction_reply_engage_no}", ConditionNpcDeclinesMarriage, null);

            starter.AddDialogLine("npc_interaction_breakup_love", "npc_interaction_reply_breakup", "close_window", "{npc_interaction_breakup_love}", ConditionNpcAcceptsBreakupLove, ConsequenceNpcAcceptsBreakup);
            starter.AddDialogLine("npc_interaction_breakup_nolove", "npc_interaction_reply_breakup", "close_window", "{npc_interaction_breakup_nolove}", ConditionNpcAcceptsBreakupNoLove, ConsequenceNpcAcceptsBreakup);
            starter.AddDialogLine("npc_interaction_breakup_leave", "npc_interaction_reply_breakup", "close_window", "{npc_interaction_breakup_leave}", ConditionNpcAcceptsBreakupLeave, ConsequenceNpcAcceptsBreakupLeave);


            starter.AddDialogLine("npc_interaction_reply_gift", "npc_interaction_reply_gift", "player_interaction_selection", "{npc_interaction_reply_gift}", null, ConsequencNpceAcceptsGift);


            //starter.AddPlayerLine("player_duel", "hero_main_options", "close_window", "Lets duel to death", null, ConsequenceDuel);
        }

        private static bool NoTimeout() => CampaignTime.Now.ToDays - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).LastInteraction > DramalordMCM.Instance.DaysBetweenInteractions;
        private static bool HusbandClose() => Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.OneToOneConversationHero.IsCloseTo(Hero.OneToOneConversationHero.Spouse) && _randomChance*-1 < Hero.OneToOneConversationHero.GetPersonality().Conscientiousness;

        private static bool ConditionPlayerCanApproach() 
        {
            if(Hero.OneToOneConversationHero.IsDramalordLegit() && !Hero.OneToOneConversationHero.IsPrisoner)
            {
                _randomChance = MBRandom.RandomInt(1, 100);
                SetupLines();
                return true;
            }

            return false; 
        }

        private static bool ConditionNpcAcceptsApproach() => Hero.OneToOneConversationHero.GetRelationWithPlayer() > -30;

        private static bool ConditionNpcDeclinesApproach() => Hero.OneToOneConversationHero.GetRelationWithPlayer() <= -30;

        private static bool ConditionPersonalInfoUnknown() => Hero.OneToOneConversationHero.GetDesires().InfoKnown == false;

        private static bool ConditionNpcDeclinesAsk() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Trust < DramalordMCM.Instance.MinTrust;

        private static bool ConditionNpcAcceptsAsk() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Trust >= DramalordMCM.Instance.MinTrust;

        private static bool ConditionNpcAcceptsAskBi() => Hero.OneToOneConversationHero.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction && Hero.OneToOneConversationHero.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction;

        private static bool ConditionNpcAcceptsAskHetero()
        {
            return !ConditionNpcAcceptsAskBi() && 
                ((Hero.OneToOneConversationHero.IsFemale && Hero.OneToOneConversationHero.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (!Hero.OneToOneConversationHero.IsFemale && Hero.OneToOneConversationHero.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction));
        }

        private static bool ConditionNpcAcceptsAskGay()
        {
            return !ConditionNpcAcceptsAskBi() && 
                !ConditionNpcAcceptsAskHetero() && 
                ((Hero.OneToOneConversationHero.IsFemale && Hero.OneToOneConversationHero.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction) || (!Hero.OneToOneConversationHero.IsFemale && Hero.OneToOneConversationHero.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction));
        }

        private static bool ConditionNpcAcceptsAskNone() => !ConditionNpcAcceptsAskBi() && !ConditionNpcAcceptsAskHetero() && !ConditionNpcAcceptsAskGay();

        private static bool ConditionNpcAcceptsAskThin() => Hero.OneToOneConversationHero.GetDesires().AttractionWeight <= 33;

        private static bool ConditionNpcAcceptsAskFat() => Hero.OneToOneConversationHero.GetDesires().AttractionWeight >= 66;

        private static bool ConditionNpcAcceptsAskNormal() => !ConditionNpcAcceptsAskThin() && !ConditionNpcAcceptsAskFat();

        private static bool ConditionNpcAcceptsAskLow() => Hero.OneToOneConversationHero.GetDesires().AttractionBuild <= 33;

        private static bool ConditionNpcAcceptsAskHigh() => Hero.OneToOneConversationHero.GetDesires().AttractionBuild >= 66;

        private static bool ConditionNpcAcceptsAskAverage() => !ConditionNpcAcceptsAskLow() && !ConditionNpcAcceptsAskHigh();

        private static bool ConditionPlayerCanAskDateFirst() => Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love >= DramalordMCM.Instance.MinDatingLove && !Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerCanAskDate() => Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerCanAskSexElse() => Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerCanAskSexFriend() => Hero.MainHero.IsFriendOf(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerCanAskSexFWB() => Hero.MainHero.IsFriendWithBenefitsOf(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerCanAskEngage() => Hero.MainHero.IsLoverOf(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerCanAskMarriage() => Hero.MainHero.IsBetrothedOf(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerCanAskBreakup() => Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);

        private static bool ConditionNpcAcceptsFlirt() => (Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) >= DramalordMCM.Instance.MinAttraction) && NoTimeout();

        private static bool ConditionNpcAcceptsTalk() => NoTimeout();

        private static bool ConditionNpcDeclinesTalkTimeout() => !NoTimeout();

        private static bool ConditionNpcDeclinesFlirt() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Relationship == RelationshipType.None && Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) < DramalordMCM.Instance.MinAttraction && NoTimeout();

        private static bool ConditionNpcDeclinesFlirtTimeout() => !NoTimeout();

        private static bool ConditionNpcMaybeDate() => NoTimeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love > 0 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove;

        private static void ConsequenceNpcMaybeDate() => Persuasions.CreatePersuasionTaskForDate();

        private static bool ConditionNpcAcceptsDateFirst() => !HusbandClose() && NoTimeout() && (Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero));

        private static bool ConditionNpcAcceptsDate() => !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionNpcDeclinesDate() => NoTimeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love <=0;

        private static bool ConditionNpcDeclinesDateHusband() => HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionNpcDeclinesDateTimeout() => !NoTimeout();

        private static bool ConditionNpcMaybeSexFriend() => !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 25 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Trust >= 25;

        private static void ConsequenceNpcMaybeSexFriend() => Persuasions.CreatePersuasionTaskForFWB();

        private static bool ConditionNpcAcceptsSexFriend() => !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= DramalordMCM.Instance.MinTrustFWB && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Trust >= DramalordMCM.Instance.MinTrustFWB;

        private static bool ConditionNpcAcceptsSexFWB() => !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 50;

        private static bool ConditionNpcDeclinesSexFWB() => !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny < 50;

        private static bool ConditionNpcAcceptsSex() => !HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 25;

        private static bool ConditionNpcDeclinesSexHusband() => HusbandClose() && NoTimeout() && Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);

        private static bool ConditionNpcDeclinesSexTimeout() => !NoTimeout() && Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);

        private static bool ConditionNpcDeclinesSexInterest() => !Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetDesires().Horny < 25;

        private static bool ConditionNpcAcceptsEngagement() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinMarriageLove;

        private static bool ConditionNpcDeclinesEngagement() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < 25;

        private static bool ConditionNpcMaybeEngagement() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= 25 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinMarriageLove && Hero.OneToOneConversationHero.IsLoverOf(Hero.MainHero);

        private static void ConsequenceNpcMaybeEngagement() => Persuasions.CreatePersuasionTaskForEngage();

        private static bool ConditionNpcAcceptsMarriage() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinMarriageLove;

        private static bool ConditionNpcDeclinesMarriage() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinMarriageLove;

        private static bool ConditionNpcAcceptsBreakupLove() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove && Hero.OneToOneConversationHero.GetPersonality().Neuroticism < _randomChance;

        private static bool ConditionNpcAcceptsBreakupNoLove() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove;

        private static bool ConditionNpcAcceptsBreakupLeave() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove && Hero.OneToOneConversationHero.GetPersonality().Neuroticism >= _randomChance;

        private static bool ConditionPlayerCanGiveGift()
        {
            ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
            ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
            if (Hero.OneToOneConversationHero.IsFemale && Hero.MainHero.PartyBelongedTo != null)
            {
                if(Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && !Hero.OneToOneConversationHero.GetDesires().HasToy && Hero.MainHero.PartyBelongedTo.ItemRoster.FindIndexOfItem(wurst) >= 0)
                {
                    return true;
                }
                return false;
            }
            else if (Hero.MainHero.PartyBelongedTo != null)
            {
                if(Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && !Hero.OneToOneConversationHero.GetDesires().HasToy && Hero.MainHero.PartyBelongedTo.ItemRoster.FindIndexOfItem(pie) >= 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }


        //CONSEQUENCE
        private static void ConsequenceNpcAcceptsTalk()
        {
            PlayerChallenges.ChallengeNumber = 1;
            PlayerChallenges.ChallengeResult = 0;
            PlayerChallenges.ExitConversation = false;
            PlayerChallenges.GenerateRandomChatChallenge();
        }

        private static void ConsequenceNpcAcceptsFlirt()
        {
            PlayerChallenges.ChallengeNumber = 1;
            PlayerChallenges.ChallengeResult = 0;
            PlayerChallenges.ExitConversation = false;
            PlayerChallenges.GenerateRandomFlirtChallenge();
        }

        private static void ConsequenceNpcAcceptsDateFirst()
        {
            PlayerChallenges.ChallengeNumber = 3;
            PlayerChallenges.ChallengeResult = 0;
            PlayerChallenges.ExitConversation = false;
            PlayerChallenges.GenerateRandomDateChallenge();
        }
        private static void ConsequenceNpcAcceptsDate()
        {
            
            PlayerChallenges.ChallengeNumber = 3;
            PlayerChallenges.ChallengeResult = 0;
            PlayerChallenges.ExitConversation = false;
            PlayerChallenges.GenerateRandomDateChallenge();
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

        private static void ConsequencNpceAcceptsGift()
        {
            TextObject toy = new TextObject();
            if (Hero.OneToOneConversationHero.IsFemale)
            {
                ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
                Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(wurst, -1);
                Hero.OneToOneConversationHero.GetDesires().HasToy = true;
                toy = new TextObject("{=Dramalord240}Sausage");
            }
            else
            {
                ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
                Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(pie, -1);
                Hero.OneToOneConversationHero.GetDesires().HasToy = true;
                toy = new TextObject("{=Dramalord241}Pie");
            }

            TextObject banner = new TextObject("{=Dramalord294}You gave {HERO.LINK} a {TOY}.");
            StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
            banner.SetTextVariable("TOY", toy);
            MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
        }

        private static void ConsequenceNpcAcceptsAsk()
        {
            Hero.OneToOneConversationHero.GetDesires().InfoKnown = true;
            TextObject banner = new TextObject("{=Dramalord336}You learned the physical preferences of {HERO.LINK}.");
            StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
            MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
        }

        private static void ConsequenceDuel()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Duel, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
