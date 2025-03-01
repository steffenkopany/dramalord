using Dramalord.Conversations;
using Dramalord.Extensions;
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
                        MBTextManager.SetTextVariable("IMPERIALCAPITAL", Settlement.FindFirst((Settlement x) => x.StringId == "town_ES4").Name);
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_A", new TextObject("{=Dramalord452}The local gang leader fed us barely and sent us stealing as soon as we were able to walk. If we weren't successful we were beaten."));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_B", new TextObject("{=Dramalord453}As soon as I grew teats I was sent to the tavern. I had to entertain disgusting men and make them spend their money on drinks.. or on me."));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_C", new TextObject("{=Dramalord454}Once there was an old, orange faced guy who constantly grabbed me by the... you know... without paying. So I kicked him in the nuts and fled out of the town."));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_1", new TextObject("{=Dramalord459}Lord Stickyfinger didn't deserve better I suppose."));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_2", new TextObject("{=Dramalord460}Ouch! That's a harsh punishment."));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_D", new TextObject("{=Dramalord455}Anyway, I figured stabbing is better then getting stabbed... if you get my meaning. So I made it my new business."));
                        StringHelpers.SetCharacterProperties("MET_WANDERER", Hero.OneToOneConversationHero.CharacterObject);
                    }
                    else
                    {
                        MBTextManager.SetTextVariable("IMPERIALCAPITAL", Settlement.FindFirst((Settlement x) => x.StringId == "town_ES4").Name);
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_A", new TextObject("{=Dramalord452}The local gang leader fed us barely and sent us stealing as soon as we were able to walk. If we weren't successful we were beaten."));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_B", new TextObject("{=Dramalord456}As soon as my armpit hair grew I was sent to castle as a servant for elderly ladies. For entertainment while their husbands were busy with war."));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_C", new TextObject("{=Dramalord457}One day, while I was... planting seedlings in the silver forest, the heart of... the forest's owner stopped. So I had to flee and leave the town."));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_1", new TextObject("{=Dramalord461}Plowing dry soil sounds like a though job."));
                        MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_2", new TextObject("{=Dramalord462}Well, young timber doesn't belong in ancient forests, right?"));
                        MBTextManager.SetTextVariable("WANDERER_BACKSTORY_D", new TextObject("{=Dramalord458}So, the times working with my wood are over, switched to sword and axe instead. Turns out I'm quite good with that too."));
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
            if (Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer && Hero.OneToOneConversationHero.Clan == null)
            {
                TextObject check = null;
                string stringId = Hero.OneToOneConversationHero.Template.StringId;
                if (!GameTexts.TryGetText("prebackstory", out check, stringId))
                {

                    MBTextManager.SetTextVariable("WANDERER_PREBACKSTORY", new TextObject("{=Dramalord451}You know, I didn't have a nice childhood as I grew up in an orphanage."));
                }
            }
        }
    }
}
