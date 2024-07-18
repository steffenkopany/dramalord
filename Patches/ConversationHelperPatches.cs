using Dramalord.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using System;
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
            if(!baseHero.IsDramalordLegit() || !queriedHero.IsDramalordLegit())
            {
                return;
            }

            if (baseHero.IsSpouseOf(queriedHero))
            {
                string text = GameTexts.FindText("str_spouse").ToString();
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
                return;
            }

            if (baseHero.IsLoverOf(queriedHero))
            {
                string text = new TextObject("{=Dramalord148}Lover").ToString();
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
                return;
            }

            if (baseHero.IsFriendWithBenefitsOf(queriedHero))
            {
                string text = new TextObject("{=Dramalord146}Friend with Benefits").ToString();
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
                return;
            }

            if (baseHero.IsBetrothedOf(queriedHero))
            {
                string text = new TextObject("{=Dramalord150}Betrothed").ToString();
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
                return;
            }

            if (baseHero.Father == queriedHero && queriedHero.IsFemale)
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
            else if(baseHero.IsSpouseOf(queriedHero))
            {
                string text = GameTexts.FindText("str_spouse").ToString();
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

    [HarmonyPatch(typeof(ConversationHelper), "HeroRefersToHero", new Type[] { typeof(Hero), typeof(Hero), typeof(bool) })]
    public static class HeroRefersToHeroPatch
    {

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void HeroRefersToHero(Hero talkTroop, Hero referringTo, bool uppercaseFirst, ref string __result)
        {
            if (talkTroop.IsChild || !talkTroop.IsLord || talkTroop.IsDead || talkTroop.IsDisabled)
            {
                return;
            }
            else if (referringTo.IsChild || !referringTo.IsLord || referringTo.IsDead || referringTo.IsDisabled)
            {
                return;
            }

            if (talkTroop.IsLoverOf(referringTo) && talkTroop.Spouse != referringTo && !referringTo.IsFemale)
            {
                string text = new TextObject("{=Dramalord148}lover").ToString();
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
            else if (talkTroop.IsLoverOf(referringTo) && talkTroop.Spouse != referringTo && referringTo.IsFemale)
            {
                string text = new TextObject("{=Dramalord138}love").ToString();
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
            else if (talkTroop.Father == referringTo && referringTo.IsFemale)
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
            else if (talkTroop.Mother == referringTo && !referringTo.IsFemale)
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
