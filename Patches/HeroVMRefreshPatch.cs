using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Localization;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "Refresh")]
    public static class HeroVMRefreshPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void RefreshPatch(ref EncyclopediaHeroPageVM __instance)
        {
            Hero? hero = __instance.Obj as Hero;
            if(hero != null && hero != Hero.MainHero && Info.ValidateHeroInfo(hero) && Info.ValidateHeroMemory(hero, Hero.MainHero))
            {
                string text = GameTexts.FindText("str_missing_info_indicator").ToString();
                string yes = GameTexts.FindText("str_yes").ToString();
                string no = GameTexts.FindText("str_no").ToString();
                TextObject attraction = new TextObject("{=Dramalord229}Attraction:");
                TextObject emotion = new TextObject("{=Dramalord230}Emotion:");
                TextObject horny = new TextObject("{=Dramalord231}Horny:");
                TextObject hastoy = new TextObject("{=Dramalord232}Has Toy:");
                TextObject traitscore = new TextObject("{=Dramalord233}Trait Score:");
                TextObject fertile = new TextObject("{=Dramalord332}Fertile:");
                __instance.Stats.Add(new StringPairItemVM(attraction.ToString(), __instance.IsInformationHidden ? text : Info.GetAttractionToHero(hero, Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(emotion.ToString(), __instance.IsInformationHidden ? text : Info.GetEmotionToHero(hero, Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(traitscore.ToString(), __instance.IsInformationHidden ? text : Info.GetTraitscoreToHero(Hero.MainHero, hero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(horny.ToString(), __instance.IsInformationHidden ? text : Info.GetHeroHorny(hero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(hastoy.ToString(), __instance.IsInformationHidden ? text : (Info.GetHeroHasToy(hero)) ? yes : no));
                __instance.Stats.Add(new StringPairItemVM(fertile.ToString(), __instance.IsInformationHidden ? text : (Info.IsHeroFertile(hero)) ? yes : no));
            }  
        }
    }
}
