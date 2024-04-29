using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Patches
{

    [HarmonyPatch(typeof(ConversationHelper), "GetHeroRelationToHeroTextShort")]
    public static class GetHeroRelationToHeroTextShortPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetHeroRelationToHeroTextShort(Hero baseHero, Hero queriedHero, bool uppercaseFirst, ref string __result)
        {
            if(baseHero.IsChild || !baseHero.IsLord || baseHero.IsDead || baseHero.IsDisabled)
            {
                return;
            }
            else if (queriedHero.IsChild || !queriedHero.IsLord || queriedHero.IsDead || queriedHero.IsDisabled)
            {
                return;
            }

            if (!Info.ValidateHeroMemory(baseHero, queriedHero))
            {
                return;
            }
            /*
            if (Info.IsCoupleWithHero(baseHero, queriedHero) && baseHero.Spouse != queriedHero)
            {
                string text = new TextObject("{=Dramalord075}affair").ToString();
                if (!char.IsLower(text[0]) != uppercaseFirst)
                {
                    char[] array = text.ToCharArray();
                    text = (uppercaseFirst ? array[0].ToString().ToUpper() : array[0].ToString().ToLower());
                    for (int i = 1; i < array.Length; i++)
                    {
                        text += array[i];
                    }
                }
                __result = text;
            }
            else */if (baseHero.Father == queriedHero && queriedHero.IsFemale)
            {
                string text = GameTexts.FindText("str_mother").ToString();
                if (!char.IsLower(text[0]) != uppercaseFirst)
                {
                    char[] array = text.ToCharArray();
                    text = (uppercaseFirst ? array[0].ToString().ToUpper() : array[0].ToString().ToLower());
                    for (int i = 1; i < array.Length; i++)
                    {
                        text += array[i];
                    }
                }
                __result = text;
            }
            else if (baseHero.Mother == queriedHero && !queriedHero.IsFemale)
            {
                string text = GameTexts.FindText("str_father").ToString();
                if (!char.IsLower(text[0]) != uppercaseFirst)
                {
                    char[] array = text.ToCharArray();
                    text = (uppercaseFirst ? array[0].ToString().ToUpper() : array[0].ToString().ToLower());
                    for (int i = 1; i < array.Length; i++)
                    {
                        text += array[i];
                    }
                }
                __result = text;
            }
        }
    }
}
