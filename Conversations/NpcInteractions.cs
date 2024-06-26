using Dramalord.Actions;
using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal sealed class NpcInteractions
    {
        internal static Hero? ApproachingHero;
        internal static EventType? Intention;

        internal static void start(Hero approachingHero, EventType intention)
        {
            NpcInteractions.ApproachingHero = approachingHero;
            NpcInteractions.Intention = intention;
            ConversationHelper.ConversationIntention = ConversationType.NPCInteraction;
            Campaign.Current.SetTimeSpeed(0);
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(ApproachingHero.CharacterObject, isCivilianEquipmentRequiredForLeader: true));
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_interaction", "lord_start", "npc_interaction_path", "{=Dramalord255}{TITLE}, I would like to talk to you.", ConditionNPCStartsInteraction, null, 120);

            starter.AddDialogLine("npc_request_start", "npc_interaction_path", "npc_request_list", "{=Dramalord042}Would you like to...", ConditionNpcAsksSomething, null);
            starter.AddDialogLine("npc_statement_start", "npc_interaction_path", "player_request_list", "{=Dramalord098}I would like to...", ConditionNpcTellsSomething, null);

            starter.AddDialogLine("npc_requests_flirt", "npc_request_list", "player_flirt_request_reply", "{=Dramalord070}...go for a walk with me?", ConditionNpcAsksForFlirt, null);
            starter.AddDialogLine("npc_requests_date", "npc_request_list", "player_date_request_reply", "{=Dramalord044}...retreat to somewhere more silent?", ConditionNpcAksForDate, null);
            starter.AddDialogLine("npc_requests_prisonfun", "npc_request_list", "player_prisonfun_reply", "{=Dramalord296}...consider letting you go for... some special service from you...", ConditionNpcAskForPrisonFun, null);
            starter.AddDialogLine("npc_requests_marriage", "npc_request_list", "player_marriage_request_reply", "{=Dramalord051}...marry me?", ConditionNpcAsksForMarriage, null);

            starter.AddDialogLine("Dramalord970", "player_request_list", "close_window", "{=Dramalord104}...end this love affair.", ConditionNpcWantsBreakup, ConsequenceNpcBrokeUpWithPlayer);
            starter.AddDialogLine("Dramalord944", "player_request_list", "close_window", "{=Dramalord105}...end this marriage.", ConditionNpcWantsDivorce, ConsequenceNpcDivorcedPlayer);

            starter.AddPlayerLine("Dramalord845", "player_flirt_request_reply", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", null, ConsequencePlayerAcceptedFlirt);
            starter.AddPlayerLine("Dramalord846", "player_flirt_request_reply", "close_window", "{=Dramalord046}No I would not like to do that.", null, ConsequencePlayerDeclinesFlirt);

            starter.AddPlayerLine("Dramalord645", "player_date_request_reply", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", null, ConsequencePlayerAcceptedDate);
            starter.AddPlayerLine("Dramalord646", "player_date_request_reply", "close_window", "{=Dramalord046}No I would not like to do that.", null, ConsequencePlayerDeclinesDate);

            starter.AddPlayerLine("Dramalord445", "player_marriage_request_reply", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", null, ConsequencePlayerAcceptsMarriage);
            starter.AddPlayerLine("Dramalord446", "player_marriage_request_reply", "close_window", "{=Dramalord046}No I would not like to do that.", null, ConsquencePlayerDeclinesMarriage);

            starter.AddPlayerLine("npc_prisonfun_accept", "player_prisonfun_reply", "close_window", "{=Dramalord297}Well come here pretty, you got yourself a deal!", null, ConsequencePlayerReactsPrisonfunAccept);
            starter.AddPlayerLine("npc_prisonfun_decline", "player_prisonfun_reply", "close_window", "{=Dramalord295}Never! You will not taint my honor with such offers!", null, null);
        }

        // CONDITIONS
        internal static bool ConditionNPCStartsInteraction()
        {
            if (NpcInteractions.ApproachingHero != null && ConversationHelper.ConversationIntention == ConversationType.NPCInteraction)
            {
                ConversationHelper.ConversationIntention = ConversationType.PlayerInteraction;
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(NpcInteractions.ApproachingHero, Hero.MainHero, true));
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcAsksSomething()
        {
            return ConditionNpcAsksForFlirt() || ConditionNpcAksForDate() || ConditionNpcAsksForMarriage();
        }

        internal static bool ConditionNpcTellsSomething()
        {
            return ConditionNpcWantsDivorce() || ConditionNpcWantsBreakup() || ConditionNpcAskForPrisonFun();
        }

        internal static bool ConditionNpcAsksForFlirt()
        {
            return Intention == EventType.Flirt;
        }

        internal static bool ConditionNpcAksForDate()
        {
            return Intention == EventType.Date;
        }

        internal static bool ConditionNpcAskForPrisonFun()
        {
            return Hero.MainHero.IsPrisoner;
        }

        internal static bool ConditionNpcAsksForMarriage()
        {
            return Intention == EventType.Marriage;
        }

        internal static bool ConditionNpcWantsDivorce()
        {
            return Intention == EventType.Divorce;
        }

        internal static bool ConditionNpcWantsBreakup()
        {
            return Intention == EventType.BreakUp;
        }

        //CONSEQUENCES
        internal static void ConsequencePlayerAcceptedFlirt()
        {
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerFlirtAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsequencePlayerAcceptedDate()
        {
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerDateAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsequencePlayerAcceptsMarriage()
        {
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerMarriageAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsequenceNpcBrokeUpWithPlayer()
        {
            HeroBreakupAction.Apply(Hero.OneToOneConversationHero, Hero.MainHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsequenceNpcDivorcedPlayer()
        {
            HeroDivorceAction.Apply(Hero.OneToOneConversationHero, Hero.MainHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsequencePlayerDeclinesDate()
        {
            Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).LastInteractionDay = (uint)CampaignTime.Now.ToDays;
            Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).Emotion += Hero.OneToOneConversationHero.GetDramalordPersonality().GetEmotionalChange(EventType.Date);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsequencePlayerDeclinesFlirt()
        {
            Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).LastInteractionDay = (uint)CampaignTime.Now.ToDays;
            Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).Emotion += Hero.OneToOneConversationHero.GetDramalordPersonality().GetEmotionalChange(EventType.Flirt);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsquencePlayerDeclinesMarriage()
        {
            Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).LastInteractionDay = (uint)CampaignTime.Now.ToDays;
            Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).Emotion += Hero.OneToOneConversationHero.GetDramalordPersonality().GetEmotionalChange(EventType.Marriage);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }

        internal static void ConsequencePlayerReactsPrisonfunAccept()
        {
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerPerformsPrisonerDeal;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }

            NpcInteractions.ApproachingHero = null;
            NpcInteractions.Intention = null;
        }
    }
}
