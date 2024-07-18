using Dramalord.Conversations;
using Dramalord.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
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
            if(!Hero.OneToOneConversationHero.IsDramalordLegit())
            {
                return;
            }

            TextObject textObject = new TextObject("{=!}{SALUTATION}...");
            textObject.SetTextVariable("SALUTATION", ConversationHelper.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, true));
            MBTextManager.SetTextVariable("SHORT_ABSENCE_GREETING", textObject);
        }
    }

    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_lord_greets_over_24_hours_on_condition")]
    public static class conversation_lord_greets_over_24_hours_on_conditionPatch
    {

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void conversation_lord_greets_over_24_hours_on_condition(ref bool __result)
        {
            if (!Hero.OneToOneConversationHero.IsDramalordLegit())
            {
                return;
            }
            MBTextManager.SetTextVariable("STR_SALUTATION", ConversationHelper.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, true));
        }
    }
}
