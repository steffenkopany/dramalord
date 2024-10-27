using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class NPCApproachingPlayer
    {
        private static Hero? ApproachingHero = null;
        private static HeroIntention? Intention = null;

        private static TextObject npc_starts_interaction_unknown = new("{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time.");
        private static TextObject npc_starts_interaction_known = new("{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?");
        private static TextObject player_interaction_start_react_yes = new("{=Dramalord029}Of course {TITLE}. How can I be of service?");
        private static TextObject player_interaction_start_react_no = new("{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
        private static TextObject npc_interaction_talk_1 = new("{=Dramalord031}I always like to hear new stories. Tell me about your latest exploits while traveling in the realm.");
        private static TextObject npc_interaction_talk_2 = new("{=Dramalord032}I would like to hear your opinion of a certain matter which is occupying my mind for a while.");
        private static TextObject npc_interaction_flirt_1 = new("{=Dramalord033}I have to say I really like your smile. Would you mind telling me more about yourself?");
        private static TextObject npc_interaction_flirt_2 = new("{=Dramalord034}Would you mind going for a walk with me? I want to cause jealousy with your outstanding appearance.");
        private static TextObject npc_interaction_date_first_1 = new("{=Dramalord035}I must confess, I can not stop thinking of you. I clearly have feelings for you, {TITLE}, and would like to bring our relationship to the next level. What do you say?");
        private static TextObject npc_interaction_date_first_2 = new("{=Dramalord036}Your presence makes me blush, {TITLE}. I would love to see you more frequently, just the two of us in private. What do you say, {TITLE}?");
        private static TextObject npc_interaction_date_single_1 = new("{=Dramalord037}Oh, {TITLE}, I have been missing you! The servants prepared a meal im my private chambers. Would you care to join me?");
        private static TextObject npc_interaction_date_single_2 = new("{=Dramalord038}I was looking forward to seeing you, {TITLE}. Would you like to retreat somewhere more silent, for a more private conversation?");
        private static TextObject npc_interaction_date_married_1 = new("{=Dramalord039}We are in luck, {TITLE}. {SPOUSE} is currently not around and the chambermaid swore to remain silent. Will you come with me?");
        private static TextObject npc_interaction_date_married_2 = new("{=Dramalord040}Ugh, {SPOUSE} is finally away. Now we have all the rooms for us! The servants will keep it for themselves, care to join me, {TITLE}?");
        private static TextObject npc_interaction_sex_nofriend_1 = new("{=Dramalord041}I will be blunt with you, {TITLE}. I have urgent needs which call for satisfaction and you look like a person who can be discreet. Can I ask for your help?");
        private static TextObject npc_interaction_sex_nofriend_2 = new("{=Dramalord042}This is very embarrassing for me, {TITLE}. I have a certain itch which requires attention, and I was hoping you could... scratch... my itch. What do you say?");
        private static TextObject npc_interaction_sex_friend_1 = new("{=Dramalord043}You are {TITLE} and I trust you. I was wondering if we could help each other in terms of... natural urges... on a regular basis. Would you like that?");
        private static TextObject npc_interaction_sex_friend_2 = new("{=Dramalord044}I was thinking if we could help each other, {TITLE}. In terms of pleasure, to be blunt. We could meet sometimes and enjoy each other. What do you say?");
        private static TextObject npc_interaction_sex_friend_wb_1 = new("{=Dramalord045}I was looking forward to seeing you, {TITLE}. Would you care for a chat and maybe some bed excercise afterwards?");
        private static TextObject npc_interaction_sex_friend_wb_2 = new("{=Dramalord046}Oh, {TITLE}. I was hoping you had some time for a conversation and some anatomical studies after. Are you interested?");
        private static TextObject npc_interaction_sex_married_1 = new("{=Dramalord047}{SPOUSE} is not around I need to feel you, {TITLE}. Let's head to the bedchamber and enjoy ourselves. Would you like that?");
        private static TextObject npc_interaction_sex_married_2 = new("{=Dramalord048}Now that {SPOUSE} is not around, I was thinking that you and I could use the empty bed for ourselves, {TITLE}. Does this tempt you?");
        private static TextObject npc_interaction_sex_else_1 = new("{=Dramalord049}Come here and kiss me, {TITLE}. I'm craving for your body and want to enjoy you with all the lust and pleasure there is!");
        private static TextObject npc_interaction_sex_else_2 = new("{=Dramalord050}Let's get rid of that clothes of yours. I show you a different kind of battle in my bedchamber where both sides win!");
        private static TextObject npc_interaction_betrothed_1 = new("{=Dramalord051}I love you very much, {TITLE}. I think it's time for us to take the next step in our relationship. Will you marry me?");
        private static TextObject npc_interaction_betrothed_2 = new("{=Dramalord052}You know, {TITLE}, I love you deeply, and I know that you are the only one for me. Will you marry me?");
        private static TextObject npc_interaction_marriage_1 = new("{=Dramalord053}Now that we are in a settlement, {TITLE}, let's call a priest and finally get married! What do you say, {TITLE}?");
        private static TextObject npc_interaction_marriage_2 = new("{=Dramalord054}We could use the opportunity being in a settlement, {TITLE}. Let's head over to the church and seal this bond for life!");
        private static TextObject npc_interaction_breakup = new("{=Dramalord229}It hurts me, {TITLE}, but I have to put an end to our relationship.");

        private static TextObject player_reaction_talk_yes = new("{=Dramalord055}Sure, let's have a conversation {TITLE}.");
        private static TextObject player_reaction_flirt_yes = new("{=Dramalord056}Of course {TITLE}, I would love to join you.");
        private static TextObject player_reaction_date_first_yes = new("{=Dramalord057}Oh {TITLE}, I wish for the same!");
        private static TextObject player_reaction_date_single_yes = new("{=Dramalord058}Sure {TITLE}, I enjoy every minute with you.");
        private static TextObject player_reaction_date_married_yes = new("{=Dramalord059}Oh yes, {TITLE}. Let's enjoy the time as long as they're gone.");
        private static TextObject player_reaction_sex_nofriend_yes = new("{=Dramalord060}Don't worry {TITLE}. I understand how you feel and I will gladly help you.");
        private static TextObject player_reaction_sex_friend_yes = new("{=Dramalord061}Interesting proposition, {TITLE}. I think I like the idea.");
        private static TextObject player_reaction_sex_friend_wb_yes = new("{=Dramalord062}Of course, {TITLE}. I could use some entertainment.");
        private static TextObject player_reaction_sex_married_yes = new("{=Dramalord063}Well, things we do for excitement... Lead the way {TITLE}!");
        private static TextObject player_reaction_sex_else_yes = new("{=Dramalord064}As you wish. I can garantuee you will not sleep much, {TITLE}");
        private static TextObject player_reaction_engagement_yes = new("{=Dramalord065}You make my dream come true, {TITLE}. Yes I would love to marry you!");
        private static TextObject player_reaction_engagement_instant_yes = new("{=Dramalord066}We're at a settlement, {TITLE}. Let's marry right now!");
        private static TextObject player_reaction_marry_yes = new("{=Dramalord067}I agree, {TITLE}. Let's seal this bond of ours.");
        private static TextObject player_reaction_breakup_accept = new("{=Dramalord231}Well, eventually it always comes to this, right {TITLE}? It was nice while it lasted, truly. Thank you.");
        private static TextObject player_reaction_breakup_leave = new("{=Dramalord233}Get out of my sight, {NAME}. Pack your belongings and leave.");
        private static TextObject player_reaction_no = new("{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");

        private static void SetupLines()
        {
            npc_starts_interaction_unknown.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_starts_interaction_known.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(true));
            player_interaction_start_react_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_interaction_start_react_no.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            npc_interaction_date_first_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_date_first_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_date_single_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_date_single_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_date_married_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_date_married_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_date_married_1.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);
            npc_interaction_date_married_2.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);
            npc_interaction_sex_nofriend_1.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_nofriend_2.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_friend_1.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_friend_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_friend_wb_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_friend_wb_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_married_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_married_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_married_1.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);
            npc_interaction_sex_married_2.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);
            npc_interaction_sex_else_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_sex_else_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_betrothed_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_betrothed_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_marriage_1.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_marriage_2.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_interaction_breakup.SetTextVariable("TITLE", Hero.MainHero.Name);

            player_reaction_talk_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_flirt_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_date_first_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_date_single_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_date_married_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_sex_nofriend_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_sex_friend_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_sex_friend_wb_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_sex_married_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_sex_else_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_engagement_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_engagement_instant_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_marry_yes.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            player_reaction_breakup_accept.SetTextVariable("TITLE", Hero.OneToOneConversationHero.Name);
            player_reaction_breakup_leave.SetTextVariable("NAME", Hero.OneToOneConversationHero.Name);
            player_reaction_no.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));


            MBTextManager.SetTextVariable("npc_starts_interaction_unknown", npc_starts_interaction_unknown);
            MBTextManager.SetTextVariable("npc_starts_interaction_known", npc_starts_interaction_known);
            MBTextManager.SetTextVariable("player_interaction_start_react_yes", player_interaction_start_react_yes);
            MBTextManager.SetTextVariable("player_interaction_start_react_no", player_interaction_start_react_no);
            MBTextManager.SetTextVariable("npc_interaction_talk_1", npc_interaction_talk_1);
            MBTextManager.SetTextVariable("npc_interaction_talk_2", npc_interaction_talk_2);
            MBTextManager.SetTextVariable("npc_interaction_flirt_1", npc_interaction_flirt_1);
            MBTextManager.SetTextVariable("npc_interaction_flirt_2", npc_interaction_flirt_2);
            MBTextManager.SetTextVariable("npc_interaction_date_first_1", npc_interaction_date_first_1);
            MBTextManager.SetTextVariable("npc_interaction_date_first_2", npc_interaction_date_first_2);
            MBTextManager.SetTextVariable("npc_interaction_date_single_1", npc_interaction_date_single_1);
            MBTextManager.SetTextVariable("npc_interaction_date_single_2", npc_interaction_date_single_2);
            MBTextManager.SetTextVariable("npc_interaction_date_married_1", npc_interaction_date_married_1);
            MBTextManager.SetTextVariable("npc_interaction_date_married_2", npc_interaction_date_married_2);
            MBTextManager.SetTextVariable("npc_interaction_sex_nofriend_1", npc_interaction_sex_nofriend_1);
            MBTextManager.SetTextVariable("npc_interaction_sex_nofriend_2", npc_interaction_sex_nofriend_2);
            MBTextManager.SetTextVariable("npc_interaction_sex_friend_1", npc_interaction_sex_friend_1);
            MBTextManager.SetTextVariable("npc_interaction_sex_friend_2", npc_interaction_sex_friend_2);
            MBTextManager.SetTextVariable("npc_interaction_sex_friend_wb_1", npc_interaction_sex_friend_wb_1);
            MBTextManager.SetTextVariable("npc_interaction_sex_friend_wb_2", npc_interaction_sex_friend_wb_2);
            MBTextManager.SetTextVariable("npc_interaction_sex_married_1", npc_interaction_sex_married_1);
            MBTextManager.SetTextVariable("npc_interaction_sex_married_2", npc_interaction_sex_married_2);
            MBTextManager.SetTextVariable("npc_interaction_sex_else_1", npc_interaction_sex_else_1);
            MBTextManager.SetTextVariable("npc_interaction_sex_else_2", npc_interaction_sex_else_2);
            MBTextManager.SetTextVariable("npc_interaction_betrothed_1", npc_interaction_betrothed_1);
            MBTextManager.SetTextVariable("npc_interaction_betrothed_2", npc_interaction_betrothed_2);
            MBTextManager.SetTextVariable("npc_interaction_marriage_1", npc_interaction_marriage_1);
            MBTextManager.SetTextVariable("npc_interaction_marriage_2", npc_interaction_marriage_2);
            MBTextManager.SetTextVariable("npc_interaction_breakup", npc_interaction_breakup);

            MBTextManager.SetTextVariable("player_reaction_talk_yes", player_reaction_talk_yes);
            MBTextManager.SetTextVariable("player_reaction_flirt_yes", player_reaction_flirt_yes);
            MBTextManager.SetTextVariable("player_reaction_date_first_yes", player_reaction_date_first_yes);
            MBTextManager.SetTextVariable("player_reaction_date_single_yes", player_reaction_date_single_yes);
            MBTextManager.SetTextVariable("player_reaction_date_married_yes", player_reaction_date_married_yes);
            MBTextManager.SetTextVariable("player_reaction_sex_nofriend_yes", player_reaction_sex_nofriend_yes);
            MBTextManager.SetTextVariable("player_reaction_sex_friend_yes", player_reaction_sex_friend_yes);
            MBTextManager.SetTextVariable("player_reaction_sex_friend_wb_yes", player_reaction_sex_friend_wb_yes);
            MBTextManager.SetTextVariable("player_reaction_sex_married_yes", player_reaction_sex_married_yes);
            MBTextManager.SetTextVariable("player_reaction_sex_else_yes", player_reaction_sex_else_yes);
            MBTextManager.SetTextVariable("player_reaction_engagement_yes", player_reaction_engagement_yes);
            MBTextManager.SetTextVariable("player_reaction_engagement_instant_yes", player_reaction_engagement_instant_yes);
            MBTextManager.SetTextVariable("player_reaction_marry_yes", player_reaction_marry_yes);
            MBTextManager.SetTextVariable("player_reaction_marry_yes", player_reaction_marry_yes);
            MBTextManager.SetTextVariable("player_reaction_breakup_accept", player_reaction_breakup_accept);
            MBTextManager.SetTextVariable("player_reaction_breakup_leave", player_reaction_breakup_leave);
            MBTextManager.SetTextVariable("player_reaction_no", player_reaction_no);
        }

        internal static bool Start(Hero hero, HeroIntention intention)
        {
            ApproachingHero = hero;
            Intention = intention;
            bool civilian = hero.CurrentSettlement != null;
            hero.GetRelationTo(Hero.MainHero).UpdateLove();
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(hero.CharacterObject, isCivilianEquipmentRequiredForLeader: civilian, noBodyguards: true, noHorse: true, noWeapon: true));
            return true;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_interaction_unknown", "start", "player_interaction_start_react", "{npc_starts_interaction_unknown}[ib:nervous][if:convo_nervous]", ConditionInteractionStartUnknown, ConsequenceInteractionStart, 120);
            starter.AddDialogLine("npc_starts_interaction_known", "start", "player_interaction_start_react", "{npc_starts_interaction_known}[ib:nervous2][if:convo_confused_normal]", ConditionInteractionStartKnown, null, 120);

            starter.AddPlayerLine("player_interaction_start_react_yes", "player_interaction_start_react", "npc_interaction_choice", "{player_interaction_start_react_yes}", null, null);
            starter.AddPlayerLine("player_interaction_start_react_no", "player_interaction_start_react", "close_window", "{player_interaction_start_react_no}", null, ConsequenceLeaveConversation);

            starter.AddDialogLineWithVariation("npc_interaction_talk", "npc_interaction_choice", "player_interaction_react", ConditionInteractionTalk, null)
                .Variation("{npc_interaction_talk_1}[ib:normal2][if:convo_calm_friendly]")
                .Variation("{npc_interaction_talk_2}[ib:normal2][if:convo_calm_friendly]");

            starter.AddDialogLineWithVariation("npc_interaction_flirt", "npc_interaction_choice", "player_interaction_react", ConditionInteractionFlirt, null)
                .Variation("{npc_interaction_flirt_1}[ib:nervous][if:convo_mocking_teasing]")
                .Variation("{npc_interaction_flirt_2}[ib:nervous][if:convo_mocking_teasing]");

            starter.AddDialogLineWithVariation("npc_interaction_date_first", "npc_interaction_choice", "player_interaction_react", ConditionInteractionDateFirst, null)
                .Variation("{npc_interaction_date_first_1}[ib:nervous][if:convo_merry]")
                .Variation("{npc_interaction_date_first_2}[ib:nervous][if:convo_merry]");

            starter.AddDialogLineWithVariation("npc_interaction_date_single", "npc_interaction_choice", "player_interaction_react", ConditionInteractionDateSingle, null)
                .Variation("{npc_interaction_date_single_1}[ib:confident2][if:convo_focused_happy]")
                .Variation("{npc_interaction_date_single_2}[ib:confident2][if:convo_focused_happy]");

            starter.AddDialogLineWithVariation("npc_interaction_date_married", "npc_interaction_choice", "player_interaction_react", ConditionInteractionDateMarried, null)
                .Variation("{npc_interaction_date_married_1}[ib:nervous][if:convo_merry]")
                .Variation("{npc_interaction_date_married_2}[ib:nervous][if:convo_merry]");

            starter.AddDialogLineWithVariation("npc_interaction_sex_nofriend", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexNoFriend, null)
                .Variation("{npc_interaction_sex_nofriend_1}[ib:nervous][if:convo_excited]")
                .Variation("{npc_interaction_sex_nofriend_2}[ib:nervous][if:convo_excited]");

            starter.AddDialogLineWithVariation("npc_interaction_sex_friend", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexFriend, null)
                .Variation("{npc_interaction_sex_friend_1}[ib:nervous][if:convo_excited]")
                .Variation("{npc_interaction_sex_friend_2}[ib:nervous][if:convo_excited]");

            starter.AddDialogLineWithVariation("npc_interaction_sex_friend_wb", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexFriendWithBenefits, null)
                .Variation("{npc_interaction_sex_friend_wb_1}[ib:confident2][if:convo_focused_happy]")
                .Variation("{npc_interaction_sex_friend_wb_2}[ib:confident2][if:convo_focused_happy]");

            starter.AddDialogLineWithVariation("npc_interaction_sex_married", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexMarried, null)
                .Variation("{npc_interaction_sex_married_1}[ib:demure][if:convo_bemused]")
                .Variation("{npc_interaction_sex_married_2}[ib:demure][if:convo_bemused]");

            starter.AddDialogLineWithVariation("npc_interaction_sex_else", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexElse, null)
                .Variation("{npc_interaction_sex_else_1}[ib:demure][if:convo_bemused]")
                .Variation("{npc_interaction_sex_else_2}[ib:demure][if:convo_bemused]");

            starter.AddDialogLineWithVariation("npc_interaction_betrothed", "npc_interaction_choice", "player_interaction_react", ConditionInteractionEngagement, null)
                .Variation("{npc_interaction_betrothed_1}[ib:confident3][if:convo_excited]")
                .Variation("{npc_interaction_betrothed_2}[ib:confident3][if:convo_excited]");

            starter.AddDialogLineWithVariation("npc_interaction_marriage", "npc_interaction_choice", "player_interaction_react", ConditionInteractionMarriage, null)
                .Variation("{npc_interaction_marriage_1}[ib:confident3][if:convo_excited]")
                .Variation("{npc_interaction_marriage_2}[ib:confident3][if:convo_excited]");


            starter.AddDialogLine("npc_interaction_breakup", "npc_interaction_choice", "player_interaction_react", "{npc_interaction_breakup}", ConditionInteractionBreakup, null);

            starter.AddPlayerLine("player_reaction_talk_yes", "player_interaction_react", "player_challenge_start", "{player_reaction_talk_yes}", ConditionPlayerTalk, ConsequencePlayerAgreesTalk);
            starter.AddPlayerLine("player_reaction_talk_yes_no_challenge", "player_interaction_react", "close_window", "{player_reaction_talk_yes}", ConditionPlayerTalkNoChallenge, ConsequencePlayerAgreesTalkNoChallenge);
            starter.AddPlayerLine("player_reaction_flirt_yes", "player_interaction_react", "player_challenge_start", "{player_reaction_flirt_yes}", ConditionPlayerFlirt, ConsequencePlayerAgreesFlirt);
            starter.AddPlayerLine("player_reaction_flirt_yes_no_challenge", "player_interaction_react", "close_window", "{player_reaction_flirt_yes}", ConditionPlayerFlirtNoChallenge, ConsequencePlayerAgreesFlirtNoChallenge);
            starter.AddPlayerLine("player_reaction_date_first_yes", "player_interaction_react", "player_challenge_start", "{player_reaction_date_first_yes}", ConditionPlayerDateFirst, ConsequencePlayerAgreesDate);
            starter.AddPlayerLine("player_reaction_date_first_yes_no_challenge", "player_interaction_react", "close_window", "{player_reaction_date_first_yes}", ConditionPlayerDateFirstNoChallenge, ConsequencePlayerAgreesDateNoChallenge);
            starter.AddPlayerLine("player_reaction_date_single_yes", "player_interaction_react", "player_challenge_start", "{player_reaction_date_single_yes}", ConditionPlayerDateSingle, ConsequencePlayerAgreesDate);
            starter.AddPlayerLine("player_reaction_date_single_yes_no_challenge", "player_interaction_react", "close_window", "{player_reaction_date_single_yes}", ConditionPlayerDateSingleNoChallenge, ConsequencePlayerAgreesDateNoChallenge);
            starter.AddPlayerLine("player_reaction_date_married_yes", "player_interaction_react", "player_challenge_start", "{player_reaction_date_married_yes}", ConditionPlayerDateMarried, ConsequencePlayerAgreesDate);
            starter.AddPlayerLine("player_reaction_date_married_yes_no_challenge", "player_interaction_react", "close_window", "{player_reaction_date_married_yes}", ConditionPlayerDateMarriedNoChallenge, ConsequencePlayerAgreesDateNoChallenge);
            starter.AddPlayerLine("player_reaction_sex_nofriend_yes", "player_interaction_react", "close_window", "{player_reaction_sex_nofriend_yes}", ConditionPlayerSexNoFriend, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_friend_yes", "player_interaction_react", "close_window", "{player_reaction_sex_friend_yes}", ConditionPlayerSexFriend, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_friend_wb_yes", "player_interaction_react", "close_window", "{player_reaction_sex_friend_wb_yes}", ConditionPlayerSexFriendWithBenefits, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_married_yes", "player_interaction_react", "close_window", "{player_reaction_sex_married_yes}", ConditionPlayerSexMarried, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_else_yes", "player_interaction_react", "close_window", "{player_reaction_sex_else_yes}", ConditionPlayerSexElse, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_engagement_yes", "player_interaction_react", "close_window", "{player_reaction_engagement_yes}", ConditionPlayerEngagement, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_engagement_instant_yes", "player_interaction_react", "close_window", "{player_reaction_engagement_instant_yes}", ConditionPlayerEngagementInstant, ConsequenceHandlePlayerAgreesMarryInstant);
            starter.AddPlayerLine("player_reaction_marry_yes", "player_interaction_react", "close_window", "{player_reaction_marry_yes}", ConditionPlayerMarriage, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_breakup_accept", "player_interaction_react", "close_window", "{player_reaction_breakup_accept}", ConditionPlayerBreakupAccept, ConsequenceHandleBreakupAccept);
            starter.AddPlayerLine("player_reaction_breakup_leave", "player_interaction_react", "close_window", "{player_reaction_breakup_leave}", ConditionPlayerBreakupLeave, ConsequenceHandleBreakupLeave);


            starter.AddPlayerLine("player_reaction_no", "player_interaction_react", "close_window", "{player_reaction_no}", ConditionPlayerCanDecline, ConsequenceHandlePlayerDeclines);
        }

        private static bool ConditionInteractionStartUnknown()
        {
            if(Hero.OneToOneConversationHero != Hero.MainHero && Hero.OneToOneConversationHero.IsDramalordLegit() && ApproachingHero == Hero.OneToOneConversationHero && !ApproachingHero.HasMet)
            {
                ApproachingHero = null;
                SetupLines();
                return true;
            }
            return false;
        }

        private static bool ConditionInteractionStartKnown()
        {
            if (Hero.OneToOneConversationHero.IsDramalordLegit() && ApproachingHero == Hero.OneToOneConversationHero && ApproachingHero.HasMet)
            {
                ApproachingHero = null;
                SetupLines();
                return true;
            }
            return false;
        }

        private static bool ConditionInteractionTalk() => Intention?.Type == IntentionType.SmallTalk;

        private static bool ConditionInteractionFlirt() => Intention?.Type == IntentionType.Flirt;

        private static bool ConditionInteractionDateFirst() => Intention?.Type == IntentionType.Date && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionInteractionDateSingle() => Intention?.Type == IntentionType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionInteractionDateMarried() => Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Intention?.Type == IntentionType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionInteractionSexNoFriend() => Intention?.Type == IntentionType.Intercourse && !Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && !Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);

        private static bool ConditionInteractionSexFriend() => Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero);

        private static bool ConditionInteractionSexFriendWithBenefits() => Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero);

        private static bool ConditionInteractionSexMarried() => Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);

        private static bool ConditionInteractionSexElse() => Intention?.Type == IntentionType.Intercourse && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero) && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionInteractionEngagement() => Intention?.Type == IntentionType.Engagement;

        private static bool ConditionInteractionMarriage() => Intention?.Type == IntentionType.Marriage;

        private static bool ConditionInteractionBreakup() => Intention?.Type == IntentionType.BreakUp;

        private static bool ConditionPlayerTalk() => Intention?.Type == IntentionType.SmallTalk && !DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerTalkNoChallenge() => Intention?.Type == IntentionType.SmallTalk && DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerFlirt() => Intention?.Type == IntentionType.Flirt && !DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerFlirtNoChallenge() => Intention?.Type == IntentionType.Flirt && DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerDateFirst() => Intention?.Type == IntentionType.Date && !Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && !DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerDateFirstNoChallenge() => Intention?.Type == IntentionType.Date && !Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerDateSingle() => Intention?.Type == IntentionType.Date && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero) && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && !DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerDateSingleNoChallenge() => Intention?.Type == IntentionType.Date && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero) && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerDateMarried() => Intention?.Type == IntentionType.Date && Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerDateMarriedNoChallenge() => Intention?.Type == IntentionType.Date && Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && !DramalordMCM.Instance.NoPlayerDialogs;

        private static bool ConditionPlayerSexNoFriend() => Intention?.Type == IntentionType.Intercourse && !Hero.MainHero.IsFriendOf(Hero.OneToOneConversationHero) && !Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);

        private static bool ConditionPlayerSexFriend() => Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero);

        private static bool ConditionPlayerSexFriendWithBenefits() => Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero);

        private static bool ConditionPlayerSexMarried() => Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionPlayerSexElse() => Intention?.Type == IntentionType.Intercourse && (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero) && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionPlayerEngagement() => Intention?.Type == IntentionType.Engagement && Hero.OneToOneConversationHero.IsLoverOf(Hero.MainHero);

        private static bool ConditionPlayerEngagementInstant() => Intention?.Type == IntentionType.Engagement && Hero.OneToOneConversationHero.IsLoverOf(Hero.MainHero) && Hero.MainHero.CurrentSettlement != null && Hero.MainHero.CurrentSettlement.IsTown;

        private static bool ConditionPlayerMarriage() => Intention?.Type == IntentionType.Marriage;

        private static bool ConditionPlayerBreakupAccept() => Intention?.Type == IntentionType.BreakUp;

        private static bool ConditionPlayerBreakupLeave() => Intention?.Type == IntentionType.BreakUp;

        private static bool ConditionPlayerCanDecline() => Intention?.Type != IntentionType.BreakUp;

        private static void ConsequenceInteractionStart() => Hero.OneToOneConversationHero?.SetHasMet();

        private static void ConsequenceLeaveConversation()
        {
            Intention = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequencePlayerAgreesTalk()
        {
            PlayerChallenges.ChallengeNumber = 1;
            PlayerChallenges.ChallengeResult = 0;
            PlayerChallenges.ExitConversation = true;
            PlayerChallenges.GenerateRandomChatChallenge();
        }

        private static void ConsequencePlayerAgreesTalkNoChallenge()
        {
            Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).UpdateLove();
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.SmallTalk, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequencePlayerAgreesFlirt()
        {
            PlayerChallenges.ChallengeNumber = 1;
            PlayerChallenges.ChallengeResult = 0;
            PlayerChallenges.ExitConversation = true;
            PlayerChallenges.GenerateRandomFlirtChallenge();
        }

        private static void ConsequencePlayerAgreesFlirtNoChallenge()
        {
            Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).UpdateLove();
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Flirt, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequencePlayerAgreesDate()
        {
            PlayerChallenges.ChallengeNumber = 3;
            PlayerChallenges.ChallengeResult = 0;
            PlayerChallenges.ExitConversation = true;
            PlayerChallenges.GenerateRandomDateChallenge();
        }

        private static void ConsequencePlayerAgreesDateNoChallenge()
        {
            Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).UpdateLove();
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Date, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceHandlePlayerAgrees()
        {
            ConversationHelper.ConversationEndedIntention = Intention;
            Intention = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceHandleBreakupAccept()
        {
            ConversationHelper.ConversationEndedIntention = Intention;
            Intention = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceHandleBreakupLeave()
        {
            Hero.OneToOneConversationHero.AddIntention(Hero.OneToOneConversationHero, IntentionType.LeaveClan, -1);
            ConversationHelper.ConversationEndedIntention = Intention;
            Intention = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceHandlePlayerAgreesMarryInstant()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Marriage, Hero.MainHero, -1);
            Intention = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceHandlePlayerDeclines()
        {
            Intention = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
