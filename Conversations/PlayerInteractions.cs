using Dramalord.Actions;
using Dramalord.Behaviors;
using Dramalord.Data;
using Dramalord.Data.Deprecated;
using Dramalord.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Dramalord.Conversations
{
    internal static class PlayerInteractions
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_starts_interaction", "hero_main_options", "npc_replies_interaction", "{=Dramalord098}I would like to...", ConditionPlayerCanStartInteraction, null);
            starter.AddDialogLine("npc_replies_interaction", "npc_replies_interaction", "player_interaction_list", "{=Dramalord102}What do you want?", null, null);

            starter.AddPlayerLine("player_offers_gift", "player_interaction_list", "npc_gift_reaction", "{=Dramalord103}...give you something.", ConditionPlayerOffersGift, null);
            starter.AddPlayerLine("player_wants_breakup", "player_interaction_list", "npc_breakup_reaction", "{=Dramalord104}...end this love affair.", ConditionPlayerWantsBreakup, null);
            starter.AddPlayerLine("player_wants_divorce", "player_interaction_list", "npc_divorce_reaction", "{=Dramalord105}...end this marriage.", ConditionPlayerWantsDivorce, null);
            starter.AddPlayerLine("player_wants_prisonfun", "player_interaction_list", "npc_prisonfun_reaction", "{=Dramalord296}...consider letting you go for... some special service from you...", ConditionPlayerWantsPrisonFun, null);
            starter.AddPlayerLine("player_stops_interaction", "player_interaction_list", "npc_replies_to_ending_conversation", "{=Dramalord072}Nevermind.", null, null);

            starter.AddDialogLine("npc_gift_reaction", "npc_gift_reaction", "hero_main_options", "{=Dramalord106}Oh! I will put it to good use!", null, ConsequenceNpcGotPresentFromPlayer);

            starter.AddDialogLine("npc_breakup_reaction_nocare", "npc_breakup_reaction", "hero_main_options", "{=Dramalord107}Ugh. Finally this is over!", ConditionNpcReactsBreakupDoesntCare, ConsequenceNpcReactsBreakupDoesntCare);
            starter.AddDialogLine("npc_breakup_reaction_surprised", "npc_breakup_reaction", "close_window", "{=Dramalord108}Oh. This is a suprise. I.. I have to be alone now...", ConditionNpcReactsBreakupSurprised, ConsequenceNpcReactsBreakupSurprised);
            starter.AddDialogLine("npc_breakup_reaction_broken", "npc_breakup_reaction", "close_window", "{=Dramalord109}What? You bastard! I never want to see you again!", ConditionNpcReactsBreakupHeartbroken, ConsequenceNpcReactsBreakupHeartbroken);
            //starter.AddDialogLine("npc_breakup_reaction_suicide", "npc_breakup_reaction", "close_window", "{=Dramalord110}Oh god... my darkes nightmare has come true... I can't live without you...", ConditionNpcReactsBreakupSuicidal, ConsequenceNpcReactsBreakupSuicidal);

            starter.AddDialogLine("npc_breakup_divorce_nocare", "npc_divorce_reaction", "hero_main_options", "{=Dramalord107}Ugh. Finally this is over!", ConditionNpcReactsDivorceDoesntCare, ConsequenceNpcReactsDivorceDoesntCare);
            starter.AddDialogLine("npc_breakup_divorce_surprised", "npc_divorce_reaction", "close_window", "{=Dramalord108}Oh. This is a suprise. I.. I have to be alone now...", ConditionNpcReactsDivorceSurprised, ConsequenceNpcReactsDivorceSurprised);
            starter.AddDialogLine("npc_breakup_divorce_broken", "npc_divorce_reaction", "close_window", "{=Dramalord109}What? You bastard! I never want to see you again!", ConditionNpcReactsDivorceHeartbroken, ConsequenceNpcReactsDivorceHeartbroken);
            //starter.AddDialogLine("npc_breakup_divorce_suicide", "npc_divorce_reaction", "close_window", "{=Dramalord110}Oh god... my darkes nightmare has come true... I can't live without you...", ConditionNpcReactsDivorceSuicidal, ConsequenceNpcReactsDivorceSuicidal);

            starter.AddDialogLine("npc_prisonfun_accept", "npc_prisonfun_reaction", "close_window", "{=Dramalord297}Well come here pretty, you got yourself a deal!", ConditionNpcReactsPrisonFunAgrees, ConsequenceNpcReactsPrisonfunAccept);
            starter.AddDialogLine("npc_prisonfun_decline", "npc_prisonfun_reaction", "player_interaction_list", "{=Dramalord295}Never! You will not taint my honor with such offers!", ConditionNpcReactsPrisonFunDeclines, null);
        }


        //CONDITIONS
        internal static bool ConditionPlayerCanStartInteraction()
        {
            return Hero.OneToOneConversationHero.IsDramalordLegit();
        }

        internal static bool ConditionPlayerOffersGift()
        {
            ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
            ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
            if (Hero.OneToOneConversationHero.IsFemale && Hero.MainHero.PartyBelongedTo != null)
            {
                return Hero.OneToOneConversationHero.GetDramalordTraits().HasToy == 0 && Hero.MainHero.PartyBelongedTo.ItemRoster.FindIndexOfItem(wurst) >= 0;
            }
            else if(Hero.MainHero.PartyBelongedTo != null)
            {
                return Hero.OneToOneConversationHero.GetDramalordTraits().HasToy == 0 && Hero.MainHero.PartyBelongedTo.ItemRoster.FindIndexOfItem(pie) >= 0;
            }
            return false;
        }

        internal static bool ConditionPlayerWantsBreakup()
        {
            return Hero.MainHero.IsLover(Hero.OneToOneConversationHero) && (Hero.MainHero.Spouse == null || Hero.MainHero.Spouse != Hero.OneToOneConversationHero);
        }
        internal static bool ConditionPlayerWantsDivorce()
        {
            return Hero.MainHero.Spouse != null && Hero.MainHero.Spouse == Hero.OneToOneConversationHero;
        }
        internal static bool ConditionPlayerWantsPrisonFun()
        {
            return (Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.PrisonRoster != null && Hero.MainHero.PartyBelongedTo.PrisonRoster.Contains(Hero.OneToOneConversationHero.CharacterObject)) || 
                (Hero.OneToOneConversationHero.IsPrisoner && Hero.OneToOneConversationHero.CurrentSettlement != null && Hero.OneToOneConversationHero.CurrentSettlement.OwnerClan == Hero.MainHero.Clan);
        }
        internal static bool ConditionNpcReactsBreakupDoesntCare()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion <= DramalordMCM.Get.MinEmotionBeforeDivorce;
        }
        internal static bool ConditionNpcReactsBreakupSurprised()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion > DramalordMCM.Get.MinEmotionBeforeDivorce && Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion <= DramalordMCM.Get.MinEmotionForDating;
        }
        internal static bool ConditionNpcReactsBreakupHeartbroken()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion > DramalordMCM.Get.MinEmotionForDating && Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion <= DramalordMCM.Get.MinEmotionForMarriage;
        }
        internal static bool ConditionNpcReactsBreakupSuicidal()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion > DramalordMCM.Get.MinEmotionForMarriage;
        }
        internal static bool ConditionNpcReactsDivorceDoesntCare()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion <= DramalordMCM.Get.MinEmotionBeforeDivorce;
        }
        internal static bool ConditionNpcReactsDivorceSurprised()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion > DramalordMCM.Get.MinEmotionBeforeDivorce && Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion < DramalordMCM.Get.MinEmotionForMarriage;
        }
        internal static bool ConditionNpcReactsDivorceHeartbroken()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion >= DramalordMCM.Get.MinEmotionForMarriage;
        }
        internal static bool ConditionNpcReactsDivorceSuicidal()
        {
            return Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion > DramalordMCM.Get.MinEmotionForMarriage;
        }
        internal static bool ConditionNpcReactsPrisonFunAgrees()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor < 1;
        }
        internal static bool ConditionNpcReactsPrisonFunDeclines()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor >= 1;
        }

        //CONSEQUENCES

        internal static void ConsequenceNpcGotPresentFromPlayer()
        {
            TextObject toy = TextObject.Empty;
            if (Hero.OneToOneConversationHero.IsFemale)
            {
                ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
                Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(wurst, -1);
                Hero.OneToOneConversationHero.GetHeroTraits().SetPropertyValue(HeroTraits.HasToy, 1);
                toy = new TextObject("{=Dramalord052}sausage");
            }
            else
            {
                ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
                Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(pie, -1);
                Hero.OneToOneConversationHero.GetHeroTraits().SetPropertyValue(HeroTraits.HasToy, 1);
                toy = new TextObject("{=Dramalord101}pie");
            }

            TextObject banner = new TextObject("{=Dramalord258}You gave {HERO.LINK} a {TOY}.");
            StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
            banner.SetTextVariable("TOY", toy);
            MBInformationManager.AddQuickInformation(banner, 1000, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
        }

        internal static void ConsequenceNpcReactsBreakupDoesntCare()
        {
            //Hero.MainHero.GetDramalordRelation(Hero.OneToOneConversationHero).Status = RelationshipStatus.Acquaintance;
        }

        internal static void ConsequenceNpcReactsBreakupSurprised()
        {
            HeroBreakupAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcReactsBreakupHeartbroken()
        {
            HeroBreakupAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerBrokeUpNpcLeaveClan;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcReactsDivorceDoesntCare()
        {
            int emotion = Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion;
            HeroDivorceAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion = emotion;
        }

        internal static void ConsequenceNpcReactsDivorceSurprised()
        {
            HeroDivorceAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcReactsDivorceHeartbroken()
        {
            HeroDivorceAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerBrokeUpNpcLeaveClan;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcReactsPrisonfunAccept()
        {
            ConversationHelper.PostConversationAction = ConversationHelper.PlayerPerformsPrisonerDeal;

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void ConsequenceNpcReactsPrisonfunDecline()
        {
            Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion -= DramalordMCM.Get.EmotionalLossCaughtFlirting;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
