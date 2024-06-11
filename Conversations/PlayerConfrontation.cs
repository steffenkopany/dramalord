using Dramalord.Actions;
using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal sealed class PlayerConfrontation
    {
        private static Hero? CheatingHero = null;
        private static HeroMemory? Memory = null;
        private static Hero? LoverOrChild = null;

        internal static void start(Hero cheater, HeroMemory memory, Hero otherHero)
        {
            PlayerConfrontation.CheatingHero = cheater;
            PlayerConfrontation.Memory = memory;
            PlayerConfrontation.LoverOrChild = otherHero;
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(CheatingHero.CharacterObject, isCivilianEquipmentRequiredForLeader: true));
        }
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("player_start_accusation", "start", "player_accusation_list", "{=Dramalord304}{TITLE}, I... I... [if:convo_astonished]", ConditionPlayerCanConfrontNpc, null, 120);

            starter.AddPlayerLine("Dramalord305", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord305}You carry no child of mine, am I correct?", ConditionPlayerSeesPregnancy, null);
            starter.AddPlayerLine("Dramalord306", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord306}I guess you and {TARGET} were not talking about politics, eh?", ConditionPlayerSeesDate, null);
            starter.AddPlayerLine("Dramalord307", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord307}Looks like you're having a hard time keeping your underpants on.", ConditionPlayerSeesIntercourse, null);
            starter.AddPlayerLine("Dramalord308", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord308}{TARGET} is clearly not my child, how dare you to give birth to a bastard?!", ConditionPlayerSeesBastard, null);
            starter.AddPlayerLine("player_accusation_marriage", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord336}So you and {TARGET} married behind my back?", ConditionPlayerSeesMarriage, null);

            starter.AddDialogLine("Dramalord309", "npc_accusation_reaction_list", "player_accusation_action_list", "{=Dramalord309}You are right. So what? I don't care.[if:convo_angry_voice]", ConditionNpcAccusedDoesntCare, null);
            starter.AddDialogLine("Dramalord310", "npc_accusation_reaction_list", "player_accusation_action_list", "{=Dramalord310}What? I have no idea what you are talking about!", ConditionNpcAccusedPlaysInnocent, null);
            starter.AddDialogLine("Dramalord311", "npc_accusation_reaction_list", "player_accusation_action_list", "{=Dramalord311}{TITLE}, please! This is all a misunderstanding!", ConditionNpcAccusedBegsForgiveness, null);

            starter.AddPlayerLine("Dramalord312", "player_accusation_action_list", "close_window", "{=Dramalord312}This is the last breath you take!", ConditionPlayerCanKillNpc, ConsequencePlayerKillsNpc);
            starter.AddPlayerLine("Dramalord313", "player_accusation_action_list", "close_window", "{=Dramalord313}Get out of my sight! I never want to see you again!", ConditionPlayerCanKickNpcOut, ConsequencePlayerKicksNpcOut);
            starter.AddPlayerLine("Dramalord314", "player_accusation_action_list", "close_window", "{=Dramalord314}That's it! It's over!", ConditionPlayerCanBreakUpOrDivorce, ConsequencePlayerBreaksUpWithNpc);
            starter.AddPlayerLine("Dramalord315", "player_accusation_action_list", "close_window", "{=Dramalord315}Whatever...", null, ConsequenceWhatever);
        }

        //CONDITIONS
        internal static bool ConditionPlayerCanConfrontNpc()
        {
            if (PlayerConfrontation.CheatingHero != null && PlayerConfrontation.Memory != null)
            {
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(PlayerConfrontation.CheatingHero, Hero.MainHero, true));
                return true;
            }
            return false;
        }

        internal static bool ConditionPlayerSeesPregnancy()
        {
            return PlayerConfrontation.Memory.Event.Type == EventType.Pregnancy;
        }

        internal static bool ConditionPlayerSeesDate()
        {
            return PlayerConfrontation.Memory.Event.Type == EventType.Date;
        }

        internal static bool ConditionPlayerSeesIntercourse()
        {
            return PlayerConfrontation.Memory.Event.Type == EventType.Intercourse;
        }

        internal static bool ConditionPlayerSeesBastard()
        {
            return PlayerConfrontation.Memory.Event.Type == EventType.Birth;
        }

        internal static bool ConditionPlayerSeesMarriage()
        {
            return PlayerConfrontation.Memory.Event.Type == EventType.Marriage;
        }


        internal static bool ConditionNpcAccusedDoesntCare()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion < DramalordMCM.Get.MinEmotionBeforeDivorce;
            
        }

        internal static bool ConditionNpcAccusedPlaysInnocent()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion < DramalordMCM.Get.MinEmotionForMarriage;
        }

        internal static bool ConditionNpcAccusedBegsForgiveness()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion >= DramalordMCM.Get.MinEmotionForMarriage;
        }

        internal static bool ConditionPlayerCanKillNpc()
        {
            return ConditionPlayerSeesBastard() || ConditionPlayerSeesIntercourse() || ConditionPlayerSeesPregnancy();
        }

        internal static bool ConditionPlayerCanKickNpcOut()
        {
            return Hero.OneToOneConversationHero.Clan == Clan.PlayerClan;
        }

        internal static bool ConditionPlayerCanBreakUpOrDivorce()
        {
            return Hero.MainHero.Spouse == Hero.OneToOneConversationHero || Hero.MainHero.IsLover(Hero.OneToOneConversationHero);
        }


        // CONSEQUENCES
        internal static void ConsequencePlayerKillsNpc()
        {
            if (PlayerConfrontation.Memory.Event.Type == EventType.Birth && PlayerConfrontation.LoverOrChild.Father != Hero.MainHero)
            {
                HeroPutInOrphanageAction.Apply(PlayerConfrontation.LoverOrChild, Hero.MainHero);
            }

            //HeroFightAction.Apply(CheatingHero, Hero.MainHero);

            HeroKillAction.Apply(PlayerConfrontation.CheatingHero, Hero.MainHero, PlayerConfrontation.LoverOrChild, PlayerConfrontation.Memory.Event.Type);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            CheatingHero = null;
            Memory = null;
            LoverOrChild = null;
        }

        internal static void ConsequencePlayerKicksNpcOut()
        {
            if (PlayerConfrontation.Memory.Event.Type == EventType.Birth && PlayerConfrontation.LoverOrChild.Father != Hero.MainHero)
            {
                HeroPutInOrphanageAction.Apply(PlayerConfrontation.LoverOrChild, Hero.MainHero);
            }

            HeroLeaveClanAction.Apply(Hero.OneToOneConversationHero, Hero.MainHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            CheatingHero = null;
            Memory = null;
            LoverOrChild = null;
        }

        internal static void ConsequencePlayerBreaksUpWithNpc()
        {
            if (PlayerConfrontation.Memory.Event.Type == EventType.Birth && PlayerConfrontation.LoverOrChild.Father != Hero.MainHero)
            {
                HeroPutInOrphanageAction.Apply(PlayerConfrontation.LoverOrChild, Hero.MainHero);
            }

            if (Hero.OneToOneConversationHero.Spouse == Hero.MainHero)
            {
                HeroDivorceAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            }
            else
            {
                HeroBreakupAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            }

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            CheatingHero = null;
            Memory = null;
            LoverOrChild = null;
        }

        internal static void ConsequenceWhatever()
        {
            CheatingHero = null;
            Memory = null;
            LoverOrChild = null;
        }
    }
}
