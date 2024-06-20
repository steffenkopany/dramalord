using Dramalord.Behaviors;
using Dramalord.Data;
using Dramalord.Data.Deprecated;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;

namespace Dramalord.Conversations
{
    internal static class PlayerRequests
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_asks_for_action", "hero_main_options", "npc_player_actions_reply", "{=Dramalord042}Would you like to...", ConditionPlayerCanAskForActions, null);

            starter.AddDialogLine("npc_player_actions_reply", "npc_player_actions_reply", "npc_player_actions_list", "{=Dramalord043}Do what?", null, null);

            starter.AddPlayerLine("player_asks_for_flirt", "npc_player_actions_list", "npc_replies_to_flirt", "{=Dramalord070}...go for a walk with me?", null, null);
            starter.AddPlayerLine("player_asks_for_date", "npc_player_actions_list", "npc_replies_to_date", "{=Dramalord044}...retreat to somewhere more silent?", ConditionPlayerAskForDate, null);
            //starter.AddPlayerLine("player_asks_for_divorcing_husband", "npc_player_actions_list", "npc_replies_to_divorcing_husband", "{=Dramalord050}...get divorced so we can focus on us?", ConditionPlayerAskForDivorcingHusband, null);
            starter.AddPlayerLine("player_asks_for_marriage", "npc_player_actions_list", "npc_replies_to_marriage", "{=Dramalord051}...marry me?", ConditionPlayerAskForMarriage, null);
            starter.AddPlayerLine("player_stops_asking", "npc_player_actions_list", "npc_replies_to_ending_conversation", "{=Dramalord072}Nevermind.", null, null);

            starter.AddDialogLine("npc_accepts_flirt", "npc_replies_to_flirt", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditionNpcAcceptsFlirt, ConsequenceNpcAcceptsFlirt);
            starter.AddDialogLine("npc_declines_flirt_ugly", "npc_replies_to_flirt", "npc_player_actions_list", "{=Dramalord330}I'm sorry, but you're not my type.", ConditionNpcDeclinesFlirtUgly, ConsequenceNpcDeclinesFlirtUgly);
            starter.AddDialogLine("npc_declines_flirt_wait", "npc_replies_to_flirt", "npc_player_actions_list", "{=Dramalord331}Again? Please, let me rest a little...", ConditionNpcDeclinesFlirtWait, null);

            starter.AddDialogLine("npc_accepts_date", "npc_replies_to_date", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditionNpcAcceptsDate, ConsequenceNpcAcceptsDate);
            starter.AddDialogLine("npc_considers_date", "npc_replies_to_date", Persuasions.PlayerDateArgument, "{=Dramalord088}Why should I spend time with you?", ConditionNpcConsidersDate, ConsequenceNpcConsidersDate);
            starter.AddDialogLine("npc_declines_date_wait", "npc_replies_to_date", "npc_player_actions_list", "{=Dramalord331}Again? Please, let me rest a little...", ConditionNpcDeclinesDateWait, ConsequenceNpcDeclinesDateWait);
            starter.AddDialogLine("npc_declines_date", "npc_replies_to_date", "npc_player_actions_list", "{=Dramalord046}No I would not like to do that.", ConditionNpcDeclinesDate, ConsequenceNpcDeclinesDate);
            /*
            starter.AddDialogLine("Dramalord745", "npc_replies_to_divorcing_husband", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditionNpcAcceptsDivorcingSpouse, ConsequenceNpcAcceptsDivorcingHusband);
            starter.AddDialogLine("Dramalord748", "npc_replies_to_divorcing_husband", Persuasions.PlayerDivorceArgument, "{=Dramalord061}Why should I give up my marriage?", ConditionNpcConsidersDivorcingSpouse, ConsequenceNpcConsidersDivorcingHusband);
            starter.AddDialogLine("Dramalord746", "npc_replies_to_divorcing_husband", "npc_player_actions_list", "{=Dramalord068}I will not give up my marriage for you!", ConditionNpcDeclinesDivorcingSpouse, ConsequenceNpcDeclinesDivorcingHusband);
            */
            starter.AddDialogLine("Dramalord545", "npc_replies_to_marriage", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditonNpcAcceptsMarriage, ConsequenceNpcAcceptsMarriage);
            starter.AddDialogLine("Dramalord546", "npc_replies_to_marriage", Persuasions.PlayerMarryArgument, "{=Dramalord082}Why should I marry you", ConditionNpcConsidersMarriage, ConsquenceNpcConsidersMarriage);
            starter.AddDialogLine("Dramalord545", "npc_replies_to_marriage", "npc_player_actions_list", "{=Dramalord046}No I would not like to do that.", ConditionNpcDeclinesMarriage, ConsquenceNpcDeclinesMarriage);
        }


        //CONDITIONS

        internal static bool ConditionPlayerCanAskForActions()
        {
            return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsDramalordLegit() && !Hero.OneToOneConversationHero.IsPrisoner;
        }

        internal static bool ConditionPlayerAskForDate()
        {
            return Hero.MainHero.IsSpouse(Hero.OneToOneConversationHero) || Hero.MainHero.IsLover(Hero.OneToOneConversationHero) || Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion >= DramalordMCM.Get.MinEmotionForDating;
        }
        
        /*
        internal static bool ConditionPlayerAskForDivorcingHusband()
        {
            return Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.MainHero.IsLover(Hero.OneToOneConversationHero);
        }
        */

        internal static bool ConditionPlayerAskForMarriage()
        {
            bool isInArmy = Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.Army != null;
            return !isInArmy && Hero.MainHero.IsLover(Hero.OneToOneConversationHero) && !Hero.MainHero.IsSpouse(Hero.OneToOneConversationHero);
        }

        internal static bool ConditionNpcAcceptsFlirt()
        {
            return (Hero.OneToOneConversationHero.GetDramalordAttractionTo(Hero.MainHero) >= DramalordMCM.Get.MinAttractionForFlirting || Hero.MainHero.IsLover(Hero.OneToOneConversationHero) || Hero.MainHero.IsSpouse(Hero.OneToOneConversationHero)) && 
                CampaignTime.Now.ToDays - Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).LastInteractionDay >= 1;
        }

        internal static bool ConditionNpcDeclinesFlirtUgly()
        {
            return Hero.OneToOneConversationHero.GetDramalordAttractionTo(Hero.MainHero) < DramalordMCM.Get.MinAttractionForFlirting && !Hero.MainHero.IsLover(Hero.OneToOneConversationHero) && !Hero.MainHero.IsSpouse(Hero.OneToOneConversationHero);
        }
        internal static bool ConditionNpcDeclinesFlirtWait()
        {
            return CampaignTime.Now.ToDays - Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).LastInteractionDay < 1;
        }

        internal static bool ConditionNpcAcceptsDate()
        {
            return ((Hero.MainHero.IsSpouse(Hero.OneToOneConversationHero) || Hero.MainHero.IsLover(Hero.OneToOneConversationHero) && CampaignTime.Now.ToDays - Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).LastInteractionDay >= DramalordMCM.Get.DaysBetweenDates)) || 
                (ConversationManager.GetPersuasionIsActive() && ConversationManager.GetPersuasionProgressSatisfied());
        }

        internal static bool ConditionNpcConsidersDate()
        {
            return !Hero.MainHero.IsSpouse(Hero.OneToOneConversationHero) && !Hero.MainHero.IsLover(Hero.OneToOneConversationHero) && Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion >= DramalordMCM.Get.MinEmotionForDating;
        }

        internal static bool ConditionNpcDeclinesDateWait()
        {
            return CampaignTime.Now.ToDays - Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).LastInteractionDay < DramalordMCM.Get.DaysBetweenDates;
        }

        internal static bool ConditionNpcDeclinesDate()
        {
            return !ConditionNpcAcceptsDate() && !ConditionNpcConsidersDate() && !ConditionNpcDeclinesDateWait();
        }
        /*
        internal static bool ConditionNpcAcceptsDivorcingSpouse()
        {
            return (Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.OneToOneConversationHero.Spouse).Emotion < DramalordMCM.Get.MinEmotionBeforeDivorce) || 
                ConversationManager.GetPersuasionIsActive() && ConversationManager.GetPersuasionProgressSatisfied();
        }

        internal static bool ConditionNpcConsidersDivorcingSpouse()
        {
            return Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.OneToOneConversationHero.Spouse).Emotion < Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).Emotion;
        }

        internal static bool ConditionNpcDeclinesDivorcingSpouse()
        {
            return (Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.OneToOneConversationHero.Spouse).Emotion >= Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.MainHero).Emotion) ||
                ConversationManager.GetPersuasionIsActive() && !ConversationManager.GetPersuasionProgressSatisfied(); 
        }
        */
        internal static bool ConditonNpcAcceptsMarriage()
        {
            return (Hero.MainHero.IsLover(Hero.OneToOneConversationHero) && Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion >= DramalordMCM.Get.MinEmotionForMarriage) || 
                ConversationManager.GetPersuasionIsActive() && ConversationManager.GetPersuasionProgressSatisfied();
        }

        internal static bool ConditionNpcConsidersMarriage()
        {
            return Hero.MainHero.IsLover(Hero.OneToOneConversationHero) && Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion < DramalordMCM.Get.MinEmotionForMarriage &&
                Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion > DramalordMCM.Get.MinEmotionForDating;
        }

        internal static bool ConditionNpcDeclinesMarriage()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion < DramalordMCM.Get.MinEmotionForMarriage;
        }

        // CONSEQUENCES

        internal static void ConsequenceNpcAcceptsFlirt()
        {
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerFlirtAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcDeclinesFlirtUgly()
        {
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).LastInteractionDay = (uint)CampaignTime.Now.ToDays;
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion -= DramalordMCM.Get.EmotionalLossCaughtFlirting;
        }

        internal static void ConsequenceNpcAcceptsDate()
        {
            Persuasions.ClearCurrentPersuasion();
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerDateAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcConsidersDate()
        {
            Persuasions.CreatePersuasionTaskForDate();
        }

        internal static void ConsequenceNpcDeclinesDateWait()
        {
            Persuasions.ClearCurrentPersuasion();
        }

        internal static void ConsequenceNpcDeclinesDate()
        {
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).LastInteractionDay = (uint)CampaignTime.Now.ToDays;
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion -= DramalordMCM.Get.EmotionalLossCaughtFlirting;
            Persuasions.ClearCurrentPersuasion();
        }
        /*
        internal static void ConsequenceNpcConsidersDivorcingHusband()
        {
            Persuasions.CreatePersuasionTaskForDivorce();
        }

        internal static void ConsequenceNpcDeclinesDivorcingHusband()
        {
            Persuasions.ClearCurrentPersuasion();
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion -= DramalordMCM.Get.EmotionalLossCaughtFlirting;
        }
        */
        internal static void ConsequenceNpcAcceptsMarriage()
        {
            Persuasions.ClearCurrentPersuasion();
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerMarriageAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsquenceNpcConsidersMarriage()
        {
            Persuasions.CreatePersuasionTaskForMarriage();
        }

        internal static void ConsquenceNpcDeclinesMarriage()
        {
            Persuasions.ClearCurrentPersuasion();
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion -= DramalordMCM.Get.EmotionalLossCaughtFlirting;
        }
    }
}
