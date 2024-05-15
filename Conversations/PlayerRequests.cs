using Dramalord.Behaviors;
using Dramalord.Data;
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
            starter.AddPlayerLine("player_asks_for_divorcing_husband", "npc_player_actions_list", "npc_replies_to_divorcing_husband", "{=Dramalord050}...get divorced so we can focus on us?", ConditionPlayerAskForDivorcingHusband, null);
            starter.AddPlayerLine("player_asks_for_marriage", "npc_player_actions_list", "npc_replies_to_marriage", "{=Dramalord051}...marry me?", ConditionPlayerAskForMarriage, null);
            starter.AddPlayerLine("player_stops_asking", "npc_player_actions_list", "npc_replies_to_ending_conversation", "{=Dramalord072}Nevermind.", null, null);

            starter.AddDialogLine("npc_accepts_flirt", "npc_replies_to_flirt", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditionNpcAcceptsFlirt, ConsequenceNpcAcceptsFlirt);
            starter.AddDialogLine("npc_declines_flirt_ugly", "npc_replies_to_flirt", "npc_player_actions_list", "{=Dramalord330}I'm sorry, but you're not my type.", ConditionNpcDeclinesFlirtUgly, ConsequenceNpcDeclinesFlirtUgly);
            starter.AddDialogLine("npc_declines_flirt_wait", "npc_replies_to_flirt", "npc_player_actions_list", "{=Dramalord331}Again? Please, let me rest a little...", ConditionNpcDeclinesFlirtWait, null);

            starter.AddDialogLine("npc_accepts_date", "npc_replies_to_date", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditionNpcAcceptsDate, ConsequenceNpcAcceptsDate);
            starter.AddDialogLine("npc_considers_date", "npc_replies_to_date", Persuasions.PlayerDateArgument, "{=Dramalord088}Why should I spend time with you?", ConditionNpcConsidersDate, ConsequenceNpcConsidersDate);
            starter.AddDialogLine("npc_declines_date_wait", "npc_replies_to_date", "npc_player_actions_list", "{=Dramalord331}Again? Please, let me rest a little...", ConditionNpcDeclinesDateWait, ConsequenceNpcDeclinesDateWait);
            starter.AddDialogLine("npc_declines_date", "npc_replies_to_date", "npc_player_actions_list", "{=Dramalord046}No I would not like to do that.", ConditionNpcDeclinesDate, ConsequenceNpcDeclinesDate);

            starter.AddDialogLine("Dramalord745", "npc_replies_to_divorcing_husband", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditionNpcAcceptsDivorcingSpouse, ConsequenceNpcAcceptsDivorcingHusband);
            starter.AddDialogLine("Dramalord748", "npc_replies_to_divorcing_husband", Persuasions.PlayerDivorceArgument, "{=Dramalord061}Why should I give up my marriage?", ConditionNpcConsidersDivorcingSpouse, ConsequenceNpcConsidersDivorcingHusband);
            starter.AddDialogLine("Dramalord746", "npc_replies_to_divorcing_husband", "npc_player_actions_list", "{=Dramalord068}I will not give up my marriage for you!", ConditionNpcDeclinesDivorcingSpouse, ConsequenceNpcDeclinesDivorcingHusband);

            starter.AddDialogLine("Dramalord545", "npc_replies_to_marriage", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", ConditonNpcAcceptsMarriage, ConsequenceNpcAcceptsMarriage);
            starter.AddDialogLine("Dramalord546", "npc_replies_to_marriage", Persuasions.PlayerMarryArgument, "{=Dramalord082}Why should I marry you", ConditionNpcConsidersMarriage, ConsquenceNpcConsidersMarriage);
            starter.AddDialogLine("Dramalord545", "npc_replies_to_marriage", "npc_player_actions_list", "{=Dramalord046}No I would not like to do that.", ConditionNpcDeclinesMarriage, ConsquenceNpcDeclinesMarriage);
        }


        //CONDITIONS

        internal static bool ConditionPlayerCanAskForActions()
        {
            return Info.ValidateHeroInfo(Hero.OneToOneConversationHero) && Info.ValidateHeroMemory(Hero.MainHero, Hero.OneToOneConversationHero);
        }

        internal static bool ConditionPlayerAskForDate()
        {
            return Hero.MainHero.Spouse == Hero.OneToOneConversationHero || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero) || Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) >= DramalordMCM.Get.MinEmotionForDating;
        }

        internal static bool ConditionPlayerAskForDivorcingHusband()
        {
            return Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero);
        }

        internal static bool ConditionPlayerAskForMarriage()
        {
            bool isInArmy = Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.Army != null;
            return !isInArmy && Hero.OneToOneConversationHero.Spouse == null && Hero.MainHero.Spouse == null && Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero);
        }

        internal static bool ConditionNpcAcceptsFlirt()
        {
            return (Info.GetAttractionToHero(Hero.OneToOneConversationHero, Hero.MainHero) >= DramalordMCM.Get.MinAttractionForFlirting || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero)) && 
                CampaignTime.Now.ToDays - Info.GetLastDaySeen(Hero.MainHero, Hero.OneToOneConversationHero) > 0;
        }

        internal static bool ConditionNpcDeclinesFlirtUgly()
        {
            return Info.GetAttractionToHero(Hero.OneToOneConversationHero, Hero.MainHero) < DramalordMCM.Get.MinAttractionForFlirting && !Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero);
        }
        internal static bool ConditionNpcDeclinesFlirtWait()
        {
            return (Info.GetAttractionToHero(Hero.OneToOneConversationHero, Hero.MainHero) >= DramalordMCM.Get.MinAttractionForFlirting || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero)) && 
                CampaignTime.Now.ToDays - Info.GetLastDaySeen(Hero.MainHero, Hero.OneToOneConversationHero) < 1;
        }

        internal static bool ConditionNpcAcceptsDate()
        {
            return (Hero.MainHero.Spouse != Hero.OneToOneConversationHero || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero) && CampaignTime.Now.ToDays - Info.GetLastDate(Hero.MainHero, Hero.OneToOneConversationHero) >= DramalordMCM.Get.DaysBetweenDates) || 
                (ConversationManager.GetPersuasionIsActive() && ConversationManager.GetPersuasionProgressSatisfied());
        }

        internal static bool ConditionNpcConsidersDate()
        {
            return Hero.MainHero.Spouse != Hero.OneToOneConversationHero && !Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero) && Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) >= DramalordMCM.Get.MinEmotionForDating && 
                CampaignTime.Now.ToDays - Info.GetLastDate(Hero.MainHero, Hero.OneToOneConversationHero) >= DramalordMCM.Get.DaysBetweenDates;
        }

        internal static bool ConditionNpcDeclinesDateWait()
        {
            return (Hero.MainHero.Spouse == Hero.OneToOneConversationHero || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero)) && CampaignTime.Now.ToDays - Info.GetLastDate(Hero.MainHero, Hero.OneToOneConversationHero) < DramalordMCM.Get.DaysBetweenDates;
        }

        internal static bool ConditionNpcDeclinesDate()
        {
            return !ConditionNpcAcceptsDate() && !ConditionNpcConsidersDate() && !ConditionNpcDeclinesDateWait();
            /*
            return (Hero.MainHero.Spouse != Hero.OneToOneConversationHero && !Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero) && Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) < DramalordMCM.Get.MinEmotionForDating) ||
                ConversationManager.GetPersuasionIsActive() && !ConversationManager.GetPersuasionProgressSatisfied();
            */
        }

        internal static bool ConditionNpcAcceptsDivorcingSpouse()
        {
            return (Hero.OneToOneConversationHero.Spouse != null && Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.Spouse) < DramalordMCM.Get.MinEmotionBeforeDivorce) || 
                ConversationManager.GetPersuasionIsActive() && ConversationManager.GetPersuasionProgressSatisfied();
        }

        internal static bool ConditionNpcConsidersDivorcingSpouse()
        {
            return Hero.OneToOneConversationHero.Spouse != null && Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.Spouse) < Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.MainHero);
        }

        internal static bool ConditionNpcDeclinesDivorcingSpouse()
        {
            return (Hero.OneToOneConversationHero.Spouse != null && Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.Spouse) >= Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.MainHero)) ||
                ConversationManager.GetPersuasionIsActive() && !ConversationManager.GetPersuasionProgressSatisfied();
        }

        internal static bool ConditonNpcAcceptsMarriage()
        {
            bool isInArmy = Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.Army != null;
            return !isInArmy && (
                (Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero) && Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) >= DramalordMCM.Get.MinEmotionForMarriage) || 
                ConversationManager.GetPersuasionIsActive() && ConversationManager.GetPersuasionProgressSatisfied()
                );
        }

        internal static bool ConditionNpcConsidersMarriage()
        {
            bool isInArmy = Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.Army != null;
            return !isInArmy && Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) < DramalordMCM.Get.MinEmotionForMarriage &&
                Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) > DramalordMCM.Get.MinEmotionForDating;
        }

        internal static bool ConditionNpcDeclinesMarriage()
        {
            bool isInArmy = Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.Army != null;
            return isInArmy || Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) < DramalordMCM.Get.MinEmotionForMarriage;
        }

        // CONSEQUENCES

        internal static void ConsequenceNpcAcceptsFlirt()
        {
            PlayerCampaignActions.PostConversationAction = PlayerCampaignActions.PlayerFlirtAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcDeclinesFlirtUgly()
        {
            Info.SetLastDaySeen(Hero.MainHero, Hero.OneToOneConversationHero, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Hero.MainHero, Hero.OneToOneConversationHero, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
        }

        internal static void ConsequenceNpcAcceptsDate()
        {
            Persuasions.ClearCurrentPersuasion();
            PlayerCampaignActions.PostConversationAction = PlayerCampaignActions.PlayerDateAction;

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
            Info.SetLastPrivateMeeting(Hero.MainHero, Hero.OneToOneConversationHero, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Hero.MainHero, Hero.OneToOneConversationHero, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
            Persuasions.ClearCurrentPersuasion();
        }

        internal static void ConsequenceNpcAcceptsDivorcingHusband()
        {
            Persuasions.ClearCurrentPersuasion();

            PlayerCampaignActions.PostConversationAction = PlayerCampaignActions.PlayerConvincedDivorceAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcConsidersDivorcingHusband()
        {
            Persuasions.CreatePersuasionTaskForDivorce();
        }

        internal static void ConsequenceNpcDeclinesDivorcingHusband()
        {
            Persuasions.ClearCurrentPersuasion();
            Info.ChangeEmotionToHeroBy(Hero.MainHero, Hero.OneToOneConversationHero, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
        }

        internal static void ConsequenceNpcAcceptsMarriage()
        {
            Persuasions.ClearCurrentPersuasion();
            PlayerCampaignActions.PostConversationAction = PlayerCampaignActions.PlayerMarriageAction;

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
            Info.ChangeEmotionToHeroBy(Hero.MainHero, Hero.OneToOneConversationHero, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
        }
    }
}
