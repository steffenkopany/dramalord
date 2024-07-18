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

        internal static bool Start(Hero hero, HeroIntention intention)
        {
            if (ApproachingHero != null) return false;

            ApproachingHero = hero;
            Intention = intention;
            bool civilian = hero.CurrentSettlement != null;
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(hero.CharacterObject, isCivilianEquipmentRequiredForLeader: civilian, noBodyguards: true, noHorse: true, noWeapon: true));
            return true;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_interaction_unknown", "start", "player_interaction_start_react", "{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time.", ConditionInteractionStartUnknown, ConsequenceInteractionStart, 120);
            starter.AddDialogLine("npc_starts_interaction_known", "start", "player_interaction_start_react", "{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?", ConditionInteractionStartKnown, null, 120);

            starter.AddPlayerLine("player_interaction_start_react_yes", "player_interaction_start_react", "npc_interaction_choice", "{=Dramalord029}Of course {TITLE}. How can I be of service?", ConditionPlayerSetTitle, null);
            starter.AddPlayerLine("player_interaction_start_react_yes", "player_interaction_start_react", "close_window", "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.", ConditionPlayerSetTitle, ConsequenceLeaveConversation);

            starter.AddDialogLineWithVariation("npc_interaction_talk", "npc_interaction_choice", "player_interaction_react", ConditionInteractionTalk, null)
                .Variation("{=Dramalord031}I always like to hear new stories. Tell me about your latest exploits while traveling in the realm.")
                .Variation("{=Dramalord032}I would like to hear your opinion of a certain matter which is occupying my mind for a while.");

            starter.AddDialogLineWithVariation("npc_interaction_flirt", "npc_interaction_choice", "player_interaction_react", ConditionInteractionFlirt, null)
                .Variation("{=Dramalord033}I have to say I really like your smile. Would you mind telling me more about yourself?")
                .Variation("{=Dramalord034}Would you mind going for a walk with me? I want to cause jealousy with your outstanding appearance.");

            starter.AddDialogLineWithVariation("npc_interaction_date_first", "npc_interaction_choice", "player_interaction_react", ConditionInteractionDateFirst, null)
                .Variation("{=Dramalord035}I must confess, I can not stop thinking of you. I clearly have feelings for you, {TITLE}, and would like to bring our relationship to the next level. What do you say?")
                .Variation("{=Dramalord036}Your presence makes me blush, {TITLE}. I would love to see you more frequently, just the two of us in private. What do you say, {TITLE}?");

            starter.AddDialogLineWithVariation("npc_interaction_date_single", "npc_interaction_choice", "player_interaction_react", ConditionInteractionDateSingle, null)
                .Variation("{=Dramalord037}Oh, {TITLE}, I have been missing you! The servants prepared a meal im my private chambers. Would you care to join me?")
                .Variation("{=Dramalord038}I was looking forward to see you, {TITLE}. Would you like to retreat somewhere more silent, for a more private conversation?");

            starter.AddDialogLineWithVariation("npc_interaction_date_married", "npc_interaction_choice", "player_interaction_react", ConditionInteractionDateMarried, null)
                .Variation("{=Dramalord039}We are in luck, {TITLE}. {SPOUSE} is currently not around and the chambermaid swore to remain silent. Will you come with me?")
                .Variation("{=Dramalord040}Ugh, {SPOUSE} is finally away. Now we have all the rooms for us! The servants will keep it for themselves, care to join me, {TITLE}?");

            starter.AddDialogLineWithVariation("npc_interaction_sex_nofriend", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexNoFriend, null)
                .Variation("{=Dramalord041}I will be blunt with you, {TITLE}. I have urgent needs which call for satisfaction and you look like a person who can be descreet. Can I ask for your help?")
                .Variation("{=Dramalord042}This is very embarassing for me, {TITLE}. I have a certain itch which requires attention, and I was hoping you could... scratch... my itch. What do you say?");

            starter.AddDialogLineWithVariation("npc_interaction_sex_friend", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexFriend, null)
                .Variation("{=Dramalord043}You are {TITLE} and I trust you. I was wondering if we could help each other in terms of... natural urges... on a regular basis. Would you like that?")
                .Variation("{=Dramalord044}I was thinking if we could help each other, {TITLE}. In terms of pleasure, to be blunt. We could meet sometimes and enjoy each other. What do you say?");

            starter.AddDialogLineWithVariation("npc_interaction_sex_friend_wb", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexFriendWithBenefits, null)
                .Variation("{=Dramalord045}I was looking forward to see you, {TITLE}. Would you care for a chat and maybe some bed excercise afterwards?")
                .Variation("{=Dramalord046}Oh, {TITLE}. I was hoping you had some time for a conversation and some anatomical studies later on. Are you interested?");

            starter.AddDialogLineWithVariation("npc_interaction_sex_married", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexMarried, null)
                .Variation("{=Dramalord047}{SPOUSE} is not around I need to feel you, {TITLE}. Let's head to the bedchamber and enjoy ourselves. Would you like that?")
                .Variation("{=Dramalord048}Now that {SPOUSE} is not around, I was thinking that you and I could use the empty bed for ourselves, {TITLE}. Does this tempt you?");

            starter.AddDialogLineWithVariation("npc_interaction_sex_else", "npc_interaction_choice", "player_interaction_react", ConditionInteractionSexElse, null)
                .Variation("{=Dramalord049}Come here and kiss me, {TITLE}. I'm craving for your body and want to enjoy you with all the lust and pleasure there is!")
                .Variation("{=Dramalord050}Let's get rid off that clothes of yours. I show you a different kind of battle in my bedchamber where both sides win!");

            starter.AddDialogLineWithVariation("npc_interaction_betrothed", "npc_interaction_choice", "player_interaction_react", ConditionInteractionEngagement, null)
                .Variation("{=Dramalord051}I love you very much, {TITLE}. I think it's time for us to take the next step in our relationship. Will you marry me?")
                .Variation("{=Dramalord052}You know, {TITLE}, I have deep feelings for you and I think to know that you are the one. Would you like to marry me?");

            starter.AddDialogLineWithVariation("npc_interaction_marriage", "npc_interaction_choice", "player_interaction_react", ConditionInteractionMarriage, null)
                .Variation("{=Dramalord053}Now that we are in a settlement, {TITLE}, let's call a priest and finally get married! What do you say, {TITLE}?")
                .Variation("{=Dramalord054}We could use the opportunity being in a settlement, {TITLE}. Let's head over to the church and seal the bond for life!");

            starter.AddPlayerLine("player_reaction_talk_yes", "player_interaction_react", "close_window", "{=Dramalord055}Sure, let's have a conversation {TITLE}.", ConditionPlayerTalk, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_flirt_yes", "player_interaction_react", "close_window", "{=Dramalord056}Of course {TITLE}, I would love to join you.", ConditionPlayerFlirt, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_date_first_yes", "player_interaction_react", "close_window", "{=Dramalord057}Oh {TITLE}, I wish for the same!", ConditionPlayerDateFirst, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_date_single_yes", "player_interaction_react", "close_window", "{=Dramalord058}Sure {TITLE}, I enjoy every minute with you.", ConditionPlayerDateSingle, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_date_married_yes", "player_interaction_react", "close_window", "{=Dramalord059}Oh yes, {TITLE}. Let's enjoy the time as long as they're gone.", ConditionPlayerDateMarried, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_nofriend_yes", "player_interaction_react", "close_window", "{=Dramalord060}Don't worry {TITLE}. I understand how you feel and I will gladly help you.", ConditionPlayerSexNoFriend, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_friend_yes", "player_interaction_react", "close_window", "{=Dramalord061}Interesting proposition, {TITLE}. I think I like the idea.", ConditionPlayerSexFriend, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_friend_wb_yes", "player_interaction_react", "close_window", "{=Dramalord062}Of course, {TITLE}. I could use some entertainment.", ConditionPlayerSexFriendWithBenefits, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_married_yes", "player_interaction_react", "close_window", "{=Dramalord063}Well, things we do for excitement... Lead the way {TITLE}!", ConditionPlayerSexMarried, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_sex_else_yes", "player_interaction_react", "close_window", "{=Dramalord064}As you wish. I can garantuee you will not sleep much, {TITLE}", ConditionPlayerSexElse, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_engagement_yes", "player_interaction_react", "close_window", "{=Dramalord065}You make my dream come true, {TITLE}. Yes I would love to marry you!", ConditionPlayerEngagement, ConsequenceHandlePlayerAgrees);
            starter.AddPlayerLine("player_reaction_engagement_instant_yes", "player_interaction_react", "close_window", "{=Dramalord066}We're at a settlement, {TITLE}. Let's marry right now!", ConditionPlayerEngagementInstant, ConsequenceHandlePlayerAgreesMarryInstant);
            starter.AddPlayerLine("player_reaction_marry_yes", "player_interaction_react", "close_window", "{=Dramalord067}I agree, {TITLE}. Let's seal that bond of ours.", ConditionPlayerMarriage, ConsequenceHandlePlayerAgrees);


            starter.AddPlayerLine("player_reaction_no", "player_interaction_react", "close_window", "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.", null, ConsequenceHandlePlayerDeclines);
        }

        private static bool ConditionInteractionStartUnknown()
        {
            if(Hero.OneToOneConversationHero.IsDramalordLegit() && ApproachingHero == Hero.OneToOneConversationHero && !ApproachingHero.HasMet)
            {
                ApproachingHero = null;
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                return true;
            }
            return false;
        }

        private static bool ConditionInteractionStartKnown()
        {
            if (Hero.OneToOneConversationHero.IsDramalordLegit() && ApproachingHero == Hero.OneToOneConversationHero && ApproachingHero.HasMet)
            {
                ApproachingHero = null;
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(true));
                return true;
            }
            return false;
        }

        private static bool ConditionPlayerSetTitle()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return true;
        }

        private static bool ConditionInteractionTalk()
        {
            return Intention?.Type == IntentionType.SmallTalk;
        }

        private static bool ConditionInteractionFlirt()
        {
            return Intention?.Type == IntentionType.Flirt;
        }

        private static bool ConditionInteractionDateFirst()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Intention?.Type == IntentionType.Date && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionInteractionDateSingle()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false)); ;
            return Intention?.Type == IntentionType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionInteractionDateMarried()
        {
            if(Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero)
            {
                MBTextManager.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse.Name);
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                return Intention?.Type == IntentionType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
            }
            return false;
        }

        private static bool ConditionInteractionSexNoFriend()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Intention?.Type == IntentionType.Intercourse && !Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && !Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);
        }

        private static bool ConditionInteractionSexFriend()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero);
        }

        private static bool ConditionInteractionSexFriendWithBenefits()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero);
        }

        private static bool ConditionInteractionSexMarried()
        {
            if (Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero)
            {
                MBTextManager.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse.Name);
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                return Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero);
            }
            return false;
        }

        private static bool ConditionInteractionSexElse()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Intention?.Type == IntentionType.Intercourse && 
                (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero) && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionInteractionEngagement()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Intention?.Type == IntentionType.Engagement;
        }

        private static bool ConditionInteractionMarriage()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Intention?.Type == IntentionType.Marriage;
        }

        private static bool ConditionPlayerTalk()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Intention?.Type == IntentionType.SmallTalk;
        }

        private static bool ConditionPlayerFlirt()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Intention?.Type == IntentionType.Flirt;
        }

        private static bool ConditionPlayerDateFirst()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false));
            return Intention?.Type == IntentionType.Date && !Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerDateSingle()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Date &&
                (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero) && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionPlayerDateMarried()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Date &&
                Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionPlayerSexNoFriend()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Intercourse && !Hero.MainHero.IsFriendOf(Hero.OneToOneConversationHero) && !Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero);
        }

        private static bool ConditionPlayerSexFriend()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero);
        }

        private static bool ConditionPlayerSexFriendWithBenefits()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Intercourse && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero);
        }

        private static bool ConditionPlayerSexMarried()
        {
            if (Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero)
            {
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
                return Intention?.Type == IntentionType.Intercourse &&
                Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
            }
            return false;
        }

        private static bool ConditionPlayerSexElse()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Intercourse &&
                (Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero) && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static bool ConditionPlayerEngagement()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Engagement && Hero.OneToOneConversationHero.IsLoverOf(Hero.MainHero);
        }

        private static bool ConditionPlayerEngagementInstant()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Engagement && Hero.OneToOneConversationHero.IsLoverOf(Hero.MainHero) && Hero.MainHero.CurrentSettlement != null && Hero.MainHero.CurrentSettlement.IsTown;
        }

        private static bool ConditionPlayerMarriage()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return Intention?.Type == IntentionType.Marriage;
        }



        private static void ConsequenceInteractionStart()
        {
            Hero.OneToOneConversationHero?.SetHasMet();
        }

        private static void ConsequenceLeaveConversation()
        {
            Intention = null;
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
