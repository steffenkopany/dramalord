using Dramalord.Data;
using Dramalord.Data.Deprecated;
using HarmonyLib;
using JetBrains.Annotations;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "Refresh")]
    public static class RefreshPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void Refresh(ref EncyclopediaHeroPageVM __instance)
        {
            Hero? hero = __instance.Obj as Hero;
            if(hero != null && hero != Hero.MainHero && hero.IsDramalordLegit() && hero.IsDramalordLegit())
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
                /*TextObject Openness = new TextObject("{=Dramalord332}Openness:");
                TextObject Conscientiousness = new TextObject("{=Dramalord332}Conscientiousness:");
                TextObject Extroversion = new TextObject("{=Dramalord332}Extroversion:");
                TextObject Agreeableness = new TextObject("{=Dramalord332}Agreeableness:");
                TextObject Neuroticism = new TextObject("{=Dramalord332}Neuroticism:");*/
                __instance.Stats.Add(new StringPairItemVM(attraction.ToString(), __instance.IsInformationHidden ? text : hero.GetDramalordAttractionTo(Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(emotion.ToString(), __instance.IsInformationHidden ? text : hero.GetDramalordFeelings(Hero.MainHero).Emotion.ToString()));
                __instance.Stats.Add(new StringPairItemVM(traitscore.ToString(), __instance.IsInformationHidden ? text : Hero.MainHero.GetDramalordTraitScore(hero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(horny.ToString(), __instance.IsInformationHidden ? text : hero.GetDramalordTraits().Horny.ToString()));
                __instance.Stats.Add(new StringPairItemVM(hastoy.ToString(), __instance.IsInformationHidden ? text : (hero.GetDramalordTraits().HasToy == 1) ? yes : no));
                __instance.Stats.Add(new StringPairItemVM(fertile.ToString(), __instance.IsInformationHidden ? text : (hero.GetDramalordIsFertile()) ? yes : no));
                /*__instance.Stats.Add(new StringPairItemVM(Openness.ToString(), __instance.IsInformationHidden? text : hero.GetDramalordTraits().Openness.ToString()));
                __instance.Stats.Add(new StringPairItemVM(Conscientiousness.ToString(), __instance.IsInformationHidden ? text : hero.GetDramalordTraits().Conscientiousness.ToString()));
                __instance.Stats.Add(new StringPairItemVM(Extroversion.ToString(), __instance.IsInformationHidden ? text : hero.GetDramalordTraits().Extroversion.ToString()));
                __instance.Stats.Add(new StringPairItemVM(Agreeableness.ToString(), __instance.IsInformationHidden ? text : hero.GetDramalordTraits().Agreeableness.ToString()));
                __instance.Stats.Add(new StringPairItemVM(Neuroticism.ToString(), __instance.IsInformationHidden ? text : hero.GetDramalordTraits().Neuroticism.ToString()));*/
            }
            if (hero != null)
            {
                if(Dramalord.DramalordMCM.Get.ShowLoversEncyclopedia)
                {
                    foreach (CharacterObject charObj in hero.GetHeroLovers())
                    {
                        if (charObj.IsHero)
                        {
                            MBBindingList<HeroVM> companions = __instance.Companions;
                            companions.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => companions.Remove(entry));

                            MBBindingList < EncyclopediaFamilyMemberVM > family = __instance.Family;
                            family.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => family.Remove(entry));

                            __instance.Family.Add(new EncyclopediaFamilyMemberVM(charObj.HeroObject, hero));
                        }
                    }
                }

                if (Dramalord.DramalordMCM.Get.ShowFWBEncyclopedia)
                {
                    foreach (CharacterObject charObj in hero.GetHeroFriendsWithBenefits())
                    {
                        if (charObj.IsHero)
                        {
                            MBBindingList<HeroVM> companions = __instance.Companions;
                            companions.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => companions.Remove(entry));

                            MBBindingList<EncyclopediaFamilyMemberVM> family = __instance.Family;
                            family.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => family.Remove(entry));

                            __instance.Family.Add(new EncyclopediaFamilyMemberVM(charObj.HeroObject, hero));
                        }
                    }
                }

                foreach (CharacterObject charObj in hero.GetHeroSpouses())
                {
                    if(charObj.IsHero)
                    {
                        MBBindingList<HeroVM> companions = __instance.Companions;
                        companions.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => companions.Remove(entry));

                        MBBindingList<EncyclopediaFamilyMemberVM> family = __instance.Family;
                        family.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => family.Remove(entry));

                        __instance.Family.Add(new EncyclopediaFamilyMemberVM(charObj.HeroObject, hero));
                    }
                }
            }
        }
    }
}
