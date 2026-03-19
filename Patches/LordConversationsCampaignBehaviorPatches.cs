using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications;
using HarmonyLib;
using Helpers;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_lord_greets_under_24_hours_on_condition")]
    public static class conversation_lord_greets_under_24_hours_on_conditionPatch
    {
         
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void conversation_lord_greets_under_24_hours_on_condition(ref bool __result)
        {
            if(__result && Hero.OneToOneConversationHero.IsDramalordLegit())
            {
                TextObject textObject = new TextObject("{=!}{SALUTATION}...");
                textObject.SetTextVariable("SALUTATION", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, true));
                MBTextManager.SetTextVariable("SHORT_ABSENCE_GREETING", textObject);
                return;
            }
        }
    }

    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_lord_greets_over_24_hours_on_condition")]
    public static class conversation_lord_greets_over_24_hours_on_conditionPatch
    {

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void conversation_lord_greets_over_24_hours_on_condition(ref bool __result)
        {
            if (__result && Hero.OneToOneConversationHero.IsDramalordLegit())
            {
                MBTextManager.SetTextVariable("STR_SALUTATION", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, true));
                return;
            }
        }
    }
    
    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_lord_introduction_on_condition")]
    public static class conversation_lord_introduction_on_conditionPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool conversation_lord_introduction_on_condition(ref bool __result)
        {
            if (Hero.OneToOneConversationHero != null && Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsLord && Hero.OneToOneConversationHero.Clan != null)
            {
                return true;
            }

            __result = false;
            if (Hero.OneToOneConversationHero != null && Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsLord && !Hero.OneToOneConversationHero.IsMinorFactionHero && !Hero.OneToOneConversationHero.IsRebel)
            {
                string id = "str_comment_noble_generic_intro";
                TextObject textObject = Campaign.Current.ConversationManager.FindMatchingTextOrNull(id, CharacterObject.OneToOneConversationCharacter);
                CharacterObject.OneToOneConversationCharacter.HeroObject.SetPropertiesToTextObject(textObject, "CONVERSATION_CHARACTER");
                textObject.SetTextVariable("CLAN_NAME", Hero.OneToOneConversationHero.Clan?.EncyclopediaLinkWithName);
                MBTextManager.SetTextVariable("LORD_INTRODUCTION_STRING", textObject);
                List<TextObject> list = new List<TextObject>();
                foreach (Settlement item in Campaign.Current.Settlements.Where((Settlement settlement) => settlement.IsTown).ToList())
                {
                    if (item.OwnerClan.Leader == Hero.OneToOneConversationHero)
                    {
                        list.Add(item.EncyclopediaLinkWithName);
                    }
                }
                __result = true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_wanderer_introduction_on_condition")]
    public static class conversation_wanderer_introduction_on_conditionPatch
    {

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void conversation_wanderer_introduction_on_condition(ref bool __result)
        {
            if (Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer && Hero.OneToOneConversationHero.Clan == null)
            {
                TextObject check = null;
                string stringId = Hero.OneToOneConversationHero.Template.StringId;
                if (!GameTexts.TryGetText("backstory_a", out check, stringId))
                {
                    if(Hero.OneToOneConversationHero.IsFemale)
                    {
                        //MBTextManager.SetTextVariable("IMPERIALCAPITAL", Settlement.FindFirst((Settlement x) => x.StringId == "town_ES4").Name);
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_A", new TextObject(DramalordTexts.BACKSTORY_GENERAL));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_B", new TextObject(DramalordTexts.BACKSTORY_FEMALE_1));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_C", new TextObject(DramalordTexts.BACKSTORY_FEMALE_2));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_1", new TextObject(DramalordTexts.BACKSTORY_FEMALE_2_REPLY_1));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_2", new TextObject(DramalordTexts.BACKSTORY_FEMALE_2_REPLY_2));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_D", new TextObject(DramalordTexts.BACKSTORY_FEMALE_3));
                        StringHelpers.SetCharacterProperties("MET_WANDERER", Hero.OneToOneConversationHero.CharacterObject);
                    }
                    else
                    {
                        //MBTextManager.SetTextVariable("IMPERIALCAPITAL", Settlement.FindFirst((Settlement x) => x.StringId == "town_ES4").Name);
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_A", new TextObject(DramalordTexts.BACKSTORY_GENERAL));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_B", new TextObject(DramalordTexts.BACKSTORY_MALE_1));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_C", new TextObject(DramalordTexts.BACKSTORY_MALE_2));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_1", new TextObject(DramalordTexts.BACKSTORY_MALE_2_REPLY_1));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_2", new TextObject(DramalordTexts.BACKSTORY_MALE_2_REPLY_2));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_D", new TextObject(DramalordTexts.BACKSTORY_MALE_3));
                        StringHelpers.SetCharacterProperties("MET_WANDERER", Hero.OneToOneConversationHero.CharacterObject);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_wanderer_preintroduction_on_condition")]
    public static class conversation_wanderer_preintroduction_on_conditionPatch
    {

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void conversation_wanderer_preintroduction_on_condition(ref bool __result)
        {
            if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer && Hero.OneToOneConversationHero.Clan == null)
            {
                string stringId = Hero.OneToOneConversationHero.Template.StringId;
                if (!GameTexts.TryGetText("prebackstory", out TextObject check, stringId))
                {

                    MBTextManager.SetTextVariable("WANDERER_PREBACKSTORY", new TextObject(DramalordTexts.BACKSTORY_START));
                }
            }
        }
    }
}
