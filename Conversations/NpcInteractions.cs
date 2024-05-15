using Dramalord.Actions;
using Dramalord.Behaviors;
using Dramalord.Data;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class NpcInteractions
    {
        internal static Hero? ApproachingHero;
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_interaction", "lord_start", "npc_interaction_path", "{=Dramalord255}{TITLE}, I would like to talk to you.", ConditionNPCStartsInteraction, null, 120);

            starter.AddDialogLine("npc_request_start", "npc_interaction_path", "npc_request_list", "{=Dramalord042}Would you like to...", ConditionNpcAsksSomething, null);
            starter.AddDialogLine("npc_statement_start", "npc_interaction_path", "player_request_list", "{=Dramalord098}I would like to...", ConditionNpcTellsSomething, null);

            starter.AddDialogLine("npc_requests_flirt", "npc_request_list", "player_flirt_request_reply", "{=Dramalord070}...go for a walk with me?", ConditionNpcAsksForFlirt, null);
            starter.AddDialogLine("npc_requests_date", "npc_request_list", "player_date_request_reply", "{=Dramalord044}...retreat to somewhere more silent?", ConditionNpcAksForDate, null);
            starter.AddDialogLine("npc_requests_marriage", "npc_request_list", "player_marriage_request_reply", "{=Dramalord051}...marry me?", ConditionNpcAsksForMarriage, null);

            starter.AddDialogLine("Dramalord970", "player_request_list", "close_window", "{=Dramalord104}...end this love affair.", ConditionNpcWantsBreakup, ConsequenceNpcBrokeUpWithPlayer);
            starter.AddDialogLine("Dramalord944", "player_request_list", "close_window", "{=Dramalord105}...end this marriage.", ConditionNpcWantsDivorce, ConsequenceNpcDivorcedPlayer);

            starter.AddPlayerLine("Dramalord845", "player_flirt_request_reply", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", null, ConsequencePlayerAcceptedFlirt);
            starter.AddPlayerLine("Dramalord846", "player_flirt_request_reply", "close_window", "{=Dramalord046}No I would not like to do that.", null, ConsequencePlayerDeclinesFlirt);

            starter.AddPlayerLine("Dramalord645", "player_date_request_reply", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", null, ConsequencePlayerAcceptedDate);
            starter.AddPlayerLine("Dramalord646", "player_date_request_reply", "close_window", "{=Dramalord046}No I would not like to do that.", null, ConsequencePlayerDeclinesDate);

            starter.AddPlayerLine("Dramalord445", "player_marriage_request_reply", "close_window", "{=Dramalord045}Oh... well, I think I would like that.", null, ConsequencePlayerAcceptsMarriage);
            starter.AddPlayerLine("Dramalord446", "player_marriage_request_reply", "close_window", "{=Dramalord046}No I would not like to do that.", null, ConsquencePlayerDeclinesMarriage);
        }

        // CONDITIONS
        internal static bool ConditionNPCStartsInteraction()
        {
            if (Hero.OneToOneConversationHero != null && (Hero.OneToOneConversationHero.IsLord || Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer))
            {
                bool result = ApproachingHero == Hero.OneToOneConversationHero;
                ApproachingHero = null;
                if (result)
                {
                    string text = string.Empty;
                    if (Hero.OneToOneConversationHero.Spouse == Hero.MainHero)
                    {
                        text = Hero.MainHero.IsFemale ? new TextObject("{=Dramalord049}wife").ToString() : new TextObject("{=Dramalord048}husband").ToString();
                    }
                    else if (Info.IsCoupleWithHero(Hero.OneToOneConversationHero, Hero.MainHero))
                    {
                        text = Hero.MainHero.IsFemale ? new TextObject("{=Dramalord097}My love").ToString() : new TextObject("{=Dramalord096}My lover").ToString();
                    }
                    else
                    {
                        text = Hero.MainHero.IsFemale ? GameTexts.FindText("str_my_lady").ToString() : GameTexts.FindText("str_my_lord").ToString();
                    }

                    char[] array = text.ToCharArray();
                    text = array[0].ToString().ToUpper();
                    for (int i = 1; i < array.Length; i++)
                    {
                        text += array[i];
                    }

                    MBTextManager.SetTextVariable("TITLE", text);
                }
                return result;
            }
            return false;
        }

        internal static bool ConditionNpcAsksSomething()
        {
            return ConditionNpcAsksForFlirt() || ConditionNpcAksForDate() || ConditionNpcAsksForMarriage();
        }

        internal static bool ConditionNpcTellsSomething()
        {
            return ConditionNpcWantsDivorce() || ConditionNpcWantsBreakup();
        }

        internal static bool ConditionNpcAsksForFlirt()
        {
            bool wantsFlirt;
            Info.GetInteractionVariables(Hero.OneToOneConversationHero, Hero.MainHero, out wantsFlirt, out _, out _, out _, out _);
            return wantsFlirt;
        }

        internal static bool ConditionNpcAksForDate()
        {
            bool wantsDate;
            Info.GetInteractionVariables(Hero.OneToOneConversationHero, Hero.MainHero, out _, out wantsDate, out _, out _, out _);
            return wantsDate;
        }

        internal static bool ConditionNpcAsksForMarriage()
        {
            bool wantsToMarry;
            Info.GetInteractionVariables(Hero.OneToOneConversationHero, Hero.MainHero, out _, out _, out wantsToMarry, out _, out _);
            return wantsToMarry;
        }

        internal static bool ConditionNpcWantsDivorce()
        {
            bool wantsToDivorce;
            Info.GetInteractionVariables(Hero.OneToOneConversationHero, Hero.MainHero, out _, out _, out _, out _, out wantsToDivorce);
            return wantsToDivorce;
        }

        internal static bool ConditionNpcWantsBreakup()
        {
            bool wantsToBreakUp;
            Info.GetInteractionVariables(Hero.OneToOneConversationHero, Hero.MainHero, out _, out _, out _, out wantsToBreakUp, out _);
            return wantsToBreakUp;
        }

        //CONSEQUENCES
        internal static void ConsequencePlayerAcceptedFlirt()
        {
            PlayerCampaignActions.PostConversationAction = PlayerCampaignActions.PlayerFlirtAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequencePlayerAcceptedDate()
        {
            PlayerCampaignActions.PostConversationAction = PlayerCampaignActions.PlayerDateAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequencePlayerAcceptsMarriage()
        {
            PlayerCampaignActions.PostConversationAction = PlayerCampaignActions.PlayerMarriageAction;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcBrokeUpWithPlayer()
        {
            HeroBreakupAction.Apply(Hero.OneToOneConversationHero, Hero.MainHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcDivorcedPlayer()
        {
            HeroDivorceAction.Apply(Hero.OneToOneConversationHero, Hero.MainHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequencePlayerDeclinesDate()
        {
            Info.SetLastPrivateMeeting(Hero.MainHero, Hero.OneToOneConversationHero, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Hero.MainHero, Hero.OneToOneConversationHero, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
        }

        internal static void ConsequencePlayerDeclinesFlirt()
        {
            Info.SetLastDaySeen(Hero.MainHero, Hero.OneToOneConversationHero, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Hero.MainHero, Hero.OneToOneConversationHero, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
        }

        internal static void ConsquencePlayerDeclinesMarriage()
        {
            Info.SetLastDaySeen(Hero.MainHero, Hero.OneToOneConversationHero, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Hero.MainHero, Hero.OneToOneConversationHero, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
        }
    }
}
