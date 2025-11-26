using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.Notifications;
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


                TextObject orientation = new TextObject(hidden);
                if (desires.IsKnowToPlayer && desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction)
                {
                    orientation = new(DramalordTexts.NAME_BISEXUAL);
                }
                else if (desires.IsKnowToPlayer && ((hero.IsFemale && desires.AttractionMen >= DramalordMCM.Instance.MinAttraction) || (!hero.IsFemale && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction)))
                {
                    orientation = new(DramalordTexts.NAME_HETEROSEXUAL);
                }
                else if (desires.IsKnowToPlayer && ((hero.IsFemale && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction) || (!hero.IsFemale && desires.AttractionMen >= DramalordMCM.Instance.MinAttraction)))
                {
                    orientation = new(DramalordTexts.NAME_HOMOSEXUAL);
                }
                else if(desires.IsKnowToPlayer)
                {
                    orientation = new(DramalordTexts.NAME_ASEXUAL);
                }

                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_ATTRACTION) + ":", (__instance.IsInformationHidden || !desires.IsKnowToPlayer) ? hidden : hero.GetAttractionTo(Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_LOVE) + ":", __instance.IsInformationHidden ? hidden : hero.GetRelationTo(Hero.MainHero).Love.ToString()));
                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_SYMPATHY) + ":", __instance.IsInformationHidden ? hidden : hero.GetSympathyTo(Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_AROUSAL) + ":", (__instance.IsInformationHidden || !hero.IsSpouseOf(Hero.MainHero))? hidden : hero.GetDesires().Horny.ToString()));
                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_SEX_ORIENTATION) + ":", __instance.IsInformationHidden ? hidden : orientation.ToString()));
                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_JEALOUSY) + ":", __instance.IsInformationHidden ? hidden : personality.Jealousy.ToString()));
                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_EMPATHY) + ":", __instance.IsInformationHidden ? hidden : personality.Empathy.ToString()));
                __instance.Stats.Add(new StringPairItemVM(new TextObject(DramalordTexts.NAME_SOCIABILITY) + ":", __instance.IsInformationHidden ? hidden : personality.Sociability.ToString()));
            }
            if(hero != null)
            { 
                
                foreach (CharacterObject charObj in hero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.Spouse).Select(relation => relation.Key.CharacterObject).ToList().Distinct())
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

                foreach (CharacterObject charObj in hero.GetAllRelations().Where(relation => relation.Value.Relationship == RelationshipType.Lover).Select(relation => relation.Key.CharacterObject).ToList().Distinct())
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

    [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "UpdateInformationText")]
    public static class UpdateInformationTextPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool UpdateInformationTextPrefix(ref EncyclopediaHeroPageVM __instance)
        {
            Hero? hero = __instance.Obj as Hero;
            __instance.InformationText = "";
            if (!TextObject.IsNullOrEmpty(hero.EncyclopediaText))
            {
                __instance.InformationText = hero.EncyclopediaText.ToString();
            }
            else if (hero.CharacterObject.Occupation == Occupation.Lord && hero.Clan != null)
            {
                __instance.InformationText = Hero.SetHeroEncyclopediaTextAndLinks(hero).ToString();
            }
            return false;
        }

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void UpdateInformationText(ref EncyclopediaHeroPageVM __instance)
        {
            Hero? hero = __instance.Obj as Hero;
            if (hero != null && hero.IsDramalordLegit())
            {
                TextObject text = TextObject.GetEmpty();
                if (hero.Weight >= 0.66 && hero.Build >= 0.66) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_FAT_AND_STRONG), hero);
                else if (hero.Weight <= 0.33 && hero.Build >= 0.66) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_THIN_AND_STRONG), hero);
                else if (hero.Weight >= 0.66 && hero.Build <= 0.33) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_FAT_AND_WEAK), hero);
                else if (hero.Weight <= 0.33 && hero.Build <= 0.33) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_THIN_AND_WEAK), hero);
                else if (hero.Build <= 0.33) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_WEAK), hero);
                else if (hero.Weight <= 0.33) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_THIN), hero);
                else if (hero.Build >= 0.66) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_STRONG), hero);
                else if (hero.Weight >= 0.66) text = ConversationTools.SetCharacterObjects(new(DramalordTexts.ENC_HERO_FAT), hero);

                __instance.InformationText += "\n" + text.ToString();
            }
        }
    }
}
