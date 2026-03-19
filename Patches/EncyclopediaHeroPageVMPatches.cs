using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.Notifications;
using HarmonyLib;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
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

            if (hero != null && hero != Hero.MainHero && hero.IsDramalordLegit())
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
            if(hero != null && hero.IsDramalordLegit())
            {
                MBBindingList<EncyclopediaFamilyMemberVM> newFamily = new();
                Dictionary<Hero, HeroRelation> relations = hero.GetAllRelations().ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Hero, HeroRelation> playerSpouses = Hero.MainHero.GetAllRelations().ToDictionary(x => x.Key, x => x.Value);

                foreach (var item in relations)
                {
                    Hero h = item.Key;
                    HeroRelation relation = item.Value;

                    if(h != null && relation != null && (relation.Relationship == RelationshipType.Spouse || relation.Relationship == RelationshipType.Lover))
                    {
                        if (!newFamily.Any(fam => fam.Hero == h))
                        {
                            if (h.IsDramalordLegit() && Campaign.Current.EncyclopediaManager.GetPageOf(typeof(Hero)).IsValidEncyclopediaItem(h))
                            {
                                newFamily.Add(new EncyclopediaFamilyMemberVM(h, hero));
                            }                           
                        }
                    }
                }

                if (hero.IsPlayerSpouse())
                {
                    foreach (var item in playerSpouses)
                    {
                        HeroRelation relation = item.Value;
                        Hero h = item.Key;

                        if (h != null && relation != null && relation.Relationship == RelationshipType.Spouse && h != hero)
                        {
                            if (!newFamily.Any(fam => fam.Hero == h))
                            {
                                newFamily.Add(new EncyclopediaFamilyMemberVM(h, hero));
                            }
                        }
                    }
                }

                __instance.Family.Where(h => !newFamily.Any(h2 => h2.Hero == h.Hero)).Do(h => newFamily.Add(h));
                __instance.Family.Clear();
                foreach (var familyMember in newFamily)
                {
                    __instance.Family.Add(familyMember);
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
            if (hero != null && hero.IsDramalordLegit())
            {
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
            return true;
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
