using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace Dramalord.Patches
{

    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_lord_greets_under_24_hours_on_condition")]
    public static class LordConversationsCampaignBehaviorPatch
    {
         
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void conversation_lord_greets_under_24_hours_on_condition(ref bool __result)
        {
            if (Hero.MainHero == null || Hero.OneToOneConversationHero == null)
            {
                return;
            }

            if (Hero.OneToOneConversationHero.IsChild || !Hero.OneToOneConversationHero.IsLord || Hero.OneToOneConversationHero.IsDead || Hero.OneToOneConversationHero.IsDisabled)
            {
                return;
            }

            if (Info.GetIsCoupleWithHero(Hero.OneToOneConversationHero, Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && !Hero.MainHero.IsFemale)
            {
                TextObject textObject = new TextObject("{=!}{SALUTATION}...");
                textObject.SetTextVariable("SALUTATION", new TextObject("{=Dramalord096}My lover"));
                MBTextManager.SetTextVariable("SHORT_ABSENCE_GREETING", textObject);
            }
            else if (Info.GetIsCoupleWithHero(Hero.OneToOneConversationHero, Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.MainHero.IsFemale)
            {
                TextObject textObject = new TextObject("{=!}{SALUTATION}...");
                textObject.SetTextVariable("SALUTATION", new TextObject("{=Dramalord097}My love"));
                MBTextManager.SetTextVariable("SHORT_ABSENCE_GREETING", textObject);
            }
            else if (Hero.OneToOneConversationHero.Father == Hero.MainHero && Hero.MainHero.IsFemale)
            {
                TextObject textObject = new TextObject("{=!}{SALUTATION}...");
                textObject.SetTextVariable("SALUTATION", GameTexts.FindText("str_mother"));
                MBTextManager.SetTextVariable("SHORT_ABSENCE_GREETING", textObject);
            }
            else if (Hero.OneToOneConversationHero.Mother == Hero.MainHero && !Hero.MainHero.IsFemale)
            {
                TextObject textObject = new TextObject("{=!}{SALUTATION}...");
                textObject.SetTextVariable("SALUTATION", GameTexts.FindText("str_father"));
                MBTextManager.SetTextVariable("SHORT_ABSENCE_GREETING", textObject);
            }
        }
    }

    [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_lord_greets_over_24_hours_on_condition")]
    public static class LordConversationsCampaignBehaviorPatch2
    {

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void conversation_lord_greets_over_24_hours_on_condition(ref bool __result)
        {
            if (Hero.MainHero == null || Hero.OneToOneConversationHero == null)
            {
                return;
            }

            if (Hero.OneToOneConversationHero.IsChild || !Hero.OneToOneConversationHero.IsLord || Hero.OneToOneConversationHero.IsDead || Hero.OneToOneConversationHero.IsDisabled)
            {
                return;
            }

            if (Info.GetIsCoupleWithHero(Hero.OneToOneConversationHero, Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && !Hero.MainHero.IsFemale)
            {
                MBTextManager.SetTextVariable("STR_SALUTATION", new TextObject("{=Dramalord096}My lover"));
            }
            else if (Info.GetIsCoupleWithHero(Hero.OneToOneConversationHero, Hero.MainHero) && Hero.OneToOneConversationHero.Spouse != Hero.MainHero && Hero.MainHero.IsFemale)
            {
                MBTextManager.SetTextVariable("STR_SALUTATION", new TextObject("{=Dramalord097}My love"));
            }
            else if (Hero.OneToOneConversationHero.Father == Hero.MainHero && Hero.MainHero.IsFemale)
            {
                MBTextManager.SetTextVariable("STR_SALUTATION", GameTexts.FindText("str_mother"));
            }
            else if (Hero.OneToOneConversationHero.Mother == Hero.MainHero && !Hero.MainHero.IsFemale)
            {
                MBTextManager.SetTextVariable("STR_SALUTATION", GameTexts.FindText("str_father"));
            }
        }
    }
}
