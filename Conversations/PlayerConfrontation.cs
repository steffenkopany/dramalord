using Dramalord.Actions;
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
    internal static class PlayerConfrontation
    {
        internal static Hero? CheatingHero = null;
        internal static WitnessType WitnessOf = WitnessType.Flirting;
        internal static Hero? LoverOrChild = null;

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("player_start_accusation", "start", "player_accusation_list", "{=Dramalord304}{TITLE}, I... I... [if:convo_astonished]", ConditionPlayerCanConfrontNpc, null, 120);

            starter.AddPlayerLine("Dramalord305", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord305}You carry no child of mine, am I correct?", ConditionPlayerSeesPregnancy, null);
            starter.AddPlayerLine("Dramalord306", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord306}I guess you and {TARGET} were not talking about politics, eh?", ConditionPlayerSeesDate, null);
            starter.AddPlayerLine("Dramalord307", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord307}Looks like you're having a hard time keeping your underpants on.", ConditionPlayerSeesIntercourse, null);
            starter.AddPlayerLine("Dramalord308", "player_accusation_list", "npc_accusation_reaction_list", "{=Dramalord308}{TARGET} is clearly not my child, how dare you to give birth to a bastard?!", ConditionPlayerSeesBastard, null);

            starter.AddDialogLine("Dramalord309", "npc_accusation_reaction_list", "player_accusation_action_list", "{=Dramalord309}You are right. So what? I don't care.[if:convo_angry_voice]", ConditionNpcAccusedDoesntCare, null);
            starter.AddDialogLine("Dramalord310", "npc_accusation_reaction_list", "player_accusation_action_list", "{=Dramalord310}What? I have no idea what you are talking about!", ConditionNpcAccusedPlaysInnocent, null);
            starter.AddDialogLine("Dramalord311", "npc_accusation_reaction_list", "player_accusation_action_list", "{=Dramalord311}{TITLE}, please! This is all a misunderstanding!", ConditionNpcAccusedBegsForgiveness, null);

            starter.AddPlayerLine("Dramalord312", "player_accusation_action_list", "close_window", "{=Dramalord312}This is the last breath you take!", ConditionPlayerCanKillNpc, ConsequencePlayerKillsNpc);
            starter.AddPlayerLine("Dramalord313", "player_accusation_action_list", "close_window", "{=Dramalord313}Get out of my sight! I never want to see you again!", ConditionPlayerCanKickNpcOut, ConsequencePlayerKicksNpcOut);
            starter.AddPlayerLine("Dramalord314", "player_accusation_action_list", "close_window", "{=Dramalord314}That's it! It's over!", ConditionPlayerCanBreakUpOrDivorce, ConsequencePlayerBreaksUpWithNpc);
            starter.AddPlayerLine("Dramalord315", "player_accusation_action_list", "close_window", "{=Dramalord315}Whatever...", null, null);
        }

        //CONDITIONS
        internal static bool ConditionPlayerCanConfrontNpc()
        {
            if (Hero.OneToOneConversationHero != null && (Hero.OneToOneConversationHero.IsLord || Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer))
            {
                bool result = CheatingHero == Hero.OneToOneConversationHero;
                CheatingHero = null;
                if (result)
                {
                    string text = string.Empty;
                    if (Hero.OneToOneConversationHero.Spouse == Hero.MainHero)
                    {
                        text = Hero.MainHero.IsFemale ? new TextObject("{=Dramalord049}wife").ToString() : new TextObject("{=Dramalord048}husband").ToString();
                    }
                    else if (Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero))
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

        internal static bool ConditionPlayerSeesPregnancy()
        {
            return WitnessOf == WitnessType.Pregnancy;
        }

        internal static bool ConditionPlayerSeesDate()
        {
            return WitnessOf == WitnessType.Dating;
        }

        internal static bool ConditionPlayerSeesIntercourse()
        {
            return WitnessOf == WitnessType.Intercourse;
        }

        internal static bool ConditionPlayerSeesBastard()
        {
            return WitnessOf == WitnessType.Bastard;
        }

        internal static bool ConditionNpcAccusedDoesntCare()
        {
            return Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) < DramalordMCM.Get.MinEmotionBeforeDivorce;
        }

        internal static bool ConditionNpcAccusedPlaysInnocent()
        {
            return Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) < DramalordMCM.Get.MinEmotionForMarriage;
        }

        internal static bool ConditionNpcAccusedBegsForgiveness()
        {
            return Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) >= DramalordMCM.Get.MinEmotionForMarriage;
        }

        internal static bool ConditionPlayerCanKillNpc()
        {
            return WitnessOf == WitnessType.Pregnancy || WitnessOf == WitnessType.Bastard || WitnessOf == WitnessType.Intercourse;
        }

        internal static bool ConditionPlayerCanKickNpcOut()
        {
            return Hero.OneToOneConversationHero.Clan == Clan.PlayerClan;
        }

        internal static bool ConditionPlayerCanBreakUpOrDivorce()
        {
            return Hero.MainHero.Spouse == Hero.OneToOneConversationHero || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero);
        }


        // CONSEQUENCES
        internal static void ConsequencePlayerKillsNpc()
        {
            KillReason reason = KillReason.Intercourse;
            if (WitnessOf == WitnessType.Pregnancy)
            {
                reason = KillReason.Pregnancy;
            }
            else if (WitnessOf == WitnessType.Bastard)
            {
                reason = KillReason.Bastard;
            }
            HeroKillAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, LoverOrChild, reason);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            CheatingHero = null;
            WitnessOf = WitnessType.Flirting;
            LoverOrChild = null;
        }

        internal static void ConsequencePlayerKicksNpcOut()
        {
            HeroLeaveClanAction.Apply(Hero.OneToOneConversationHero, Hero.MainHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            CheatingHero = null;
            WitnessOf = WitnessType.Flirting;
            LoverOrChild = null;
        }

        internal static void ConsequencePlayerBreaksUpWithNpc()
        {
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
            WitnessOf = WitnessType.Flirting;
            LoverOrChild = null;
        }
    }
}
