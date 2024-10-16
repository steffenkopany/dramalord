using Dramalord.Data;
using Dramalord.Extensions;
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

            if (hero != null && hero != Hero.MainHero && hero.IsDramalordLegit() && hero.IsDramalordLegit())
            {
                DramalordMCMEditor.Instance.SetSelected(hero);
                HeroDesires desires = hero.GetDesires();
                HeroPersonality personality = hero.GetPersonality();

                string hidden = GameTexts.FindText("str_missing_info_indicator").ToString();
                string yes = GameTexts.FindText("str_yes").ToString();
                string no = GameTexts.FindText("str_no").ToString();
                TextObject attraction = new TextObject("{=Dramalord154}Attraction");
                TextObject friendship = new TextObject("{=Dramalord136}Trust");
                TextObject love = new TextObject("{=Dramalord138}Love");
                TextObject horny = new TextObject("{=Dramalord133}Horny");
                TextObject sympathy = new TextObject("{=Dramalord155}Sympathy");
                TextObject fertile = new TextObject("{=Dramalord156}Fertile");

                TextObject openness = new TextObject("{=Dramalord110}Openness");
                TextObject conscientiousness = new TextObject("{=Dramalord112}Conscientiousness");
                TextObject extroversion = new TextObject("{=Dramalord114}Extroversion");
                TextObject agreeableness = new TextObject("{=Dramalord116}Agreeableness");
                TextObject neuroticism = new TextObject("{=Dramalord118}Neuroticism");

                TextObject sexOrientation = new TextObject("{=Dramalord157}Sexual orientation");
                TextObject orientation = GameTexts.FindText("str_missing_info_indicator");
                if (desires.InfoKnown && desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction)
                {
                    orientation = new TextObject("{=Dramalord159}Bisexual");
                }
                else if (desires.InfoKnown && ((hero.IsFemale && desires.AttractionMen >= DramalordMCM.Instance.MinAttraction) || (!hero.IsFemale && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction)))
                {
                    orientation = new TextObject("{=Dramalord160}Heterosexual");
                }
                else if (desires.InfoKnown && ((hero.IsFemale && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction) || (!hero.IsFemale && desires.AttractionMen >= DramalordMCM.Instance.MinAttraction)))
                {
                    orientation = new TextObject("{=Dramalord161}Homosexual");
                }
                else if(desires.InfoKnown)
                {
                    orientation = new TextObject("{=Dramalord158}Asexual");
                }

                __instance.Stats.Add(new StringPairItemVM(attraction.ToString() + ":", (__instance.IsInformationHidden || !desires.InfoKnown) ? hidden : hero.GetAttractionTo(Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(friendship.ToString() + ":", __instance.IsInformationHidden ? hidden : hero.GetRelationTo(Hero.MainHero).Trust.ToString()));
                __instance.Stats.Add(new StringPairItemVM(love.ToString() + ":", __instance.IsInformationHidden ? hidden : hero.GetRelationTo(Hero.MainHero).CurrentLove.ToString()));
                __instance.Stats.Add(new StringPairItemVM(sympathy.ToString() + ":", __instance.IsInformationHidden ? hidden : hero.GetSympathyTo(Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(horny.ToString() + ":", __instance.IsInformationHidden ? hidden : hero.GetDesires().Horny.ToString()));
                __instance.Stats.Add(new StringPairItemVM(fertile.ToString() + ":", __instance.IsInformationHidden ? hidden : (hero.IsFertile()) ? yes : no));
                __instance.Stats.Add(new StringPairItemVM(sexOrientation.ToString() + ":", __instance.IsInformationHidden ? hidden : orientation.ToString()));
                __instance.Stats.Add(new StringPairItemVM(openness.ToString() + ":", __instance.IsInformationHidden ? hidden : personality.Openness.ToString()));
                __instance.Stats.Add(new StringPairItemVM(conscientiousness.ToString() + ":", __instance.IsInformationHidden ? hidden : personality.Conscientiousness.ToString()));
                __instance.Stats.Add(new StringPairItemVM(extroversion.ToString() + ":", __instance.IsInformationHidden ? hidden : personality.Extroversion.ToString()));
                __instance.Stats.Add(new StringPairItemVM(agreeableness.ToString() + ":", __instance.IsInformationHidden ? hidden : personality.Agreeableness.ToString()));
                __instance.Stats.Add(new StringPairItemVM(neuroticism.ToString() + ":", __instance.IsInformationHidden ? hidden : personality.Neuroticism.ToString()));
            }
            if(hero != null)
            { 
                foreach (CharacterObject charObj in hero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.Spouse).Select(relation => (relation.Key.Hero1 == hero) ? relation.Key.Hero2.CharacterObject : relation.Key.Hero1.CharacterObject).ToList().Distinct())
                {
                    if (charObj.IsHero && charObj.HeroObject != hero)
                    {
                        MBBindingList<HeroVM> companions = __instance.Companions;
                        companions.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => companions.Remove(entry));

                        MBBindingList<EncyclopediaFamilyMemberVM> family = __instance.Family;
                        family.Where(item => item.Hero == charObj.HeroObject).ToList().ForEach(entry => family.Remove(entry));

                        __instance.Family.Add(new EncyclopediaFamilyMemberVM(charObj.HeroObject, hero));
                    }
                }

                if (DramalordMCM.Instance.ShowLovers)
                {
                    foreach (CharacterObject charObj in hero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.Lover).Select(relation => (relation.Key.Hero1 == hero) ? relation.Key.Hero2.CharacterObject : relation.Key.Hero1.CharacterObject).ToList().Distinct())
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

                if (DramalordMCM.Instance.ShowFriendsWithBenefits)
                {
                    foreach (CharacterObject charObj in hero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.FriendWithBenefits).Select(relation => (relation.Key.Hero1 == hero) ? relation.Key.Hero2.CharacterObject : relation.Key.Hero1.CharacterObject).ToList().Distinct())
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

                if (DramalordMCM.Instance.ShowBetrotheds)
                {
                    foreach (CharacterObject charObj in hero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.Betrothed).Select(relation => (relation.Key.Hero1 == hero) ? relation.Key.Hero2.CharacterObject : relation.Key.Hero1.CharacterObject).ToList().Distinct())
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
            }
        }
    }

    [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "UpdateInformationText")]
    public static class UpdateInformationTextPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void UpdateInformationText(ref EncyclopediaHeroPageVM __instance)
        {
            Hero? hero = __instance.Obj as Hero;
            if (hero != null && hero.IsDramalordLegit())
            {
                TextObject text = new();
                if (hero.Weight >= 0.66 && hero.Build >= 0.66) text = new("{=Dramalord445}{NAME} is known to eat a lot and doing workouts regularly.");
                else if (hero.Weight <= 0.33 && hero.Build >= 0.66) text = new("{=Dramalord446}{NAME} is known to barely eat anything and doing workouts regularly.");
                else if (hero.Weight >= 0.66 && hero.Build <= 0.33) text = new("{=Dramalord447}{NAME} is known to eat a lot and rarely doing workouts.");
                else if (hero.Weight <= 0.33 && hero.Build <= 0.33) text = new("{=Dramalord448}{NAME} is known to barely eat anything and rarely doing workouts.");
                else if (hero.Build <= 0.33) text = new("{=Dramalord449}{NAME} is known to rarely doing workouts.");
                else if (hero.Weight <= 0.33) text = new("{=Dramalord450}{NAME} is known to barely eat anything.");

                
                text.SetTextVariable("NAME", hero.Name);
                __instance.InformationText += "\n" + text.ToString();
            }
        }
    }
}
