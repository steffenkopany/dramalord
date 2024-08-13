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
                string text = GameTexts.FindText("str_missing_info_indicator").ToString();
                string yes = GameTexts.FindText("str_yes").ToString();
                string no = GameTexts.FindText("str_no").ToString();
                TextObject attraction = new TextObject("{=Dramalord154}Attraction");
                TextObject friendship = new TextObject("{=Dramalord136}Trust");
                TextObject love = new TextObject("{=Dramalord138}Love");
                TextObject horny = new TextObject("{=Dramalord133}Horny");
                TextObject sympathy = new TextObject("{=Dramalord155}Sympathy");
                TextObject fertile = new TextObject("{=Dramalord156}Fertile");

                TextObject sexOrientation = new TextObject("{=Dramalord157}Sexual orientation");
                TextObject orientation = new TextObject("{=Dramalord158}Asexual");
                if (hero.GetDesires().AttractionMen >= 50 && hero.GetDesires().AttractionWomen > 50)
                {
                    orientation = new TextObject("{=Dramalord159}Bisexual");
                }
                else if ((hero.IsFemale && hero.GetDesires().AttractionMen >= 50) || (!hero.IsFemale && hero.GetDesires().AttractionWomen >= 50))
                {
                    orientation = new TextObject("{=Dramalord160}Heterosexual");
                }
                else if ((hero.IsFemale && hero.GetDesires().AttractionWomen >= 50) || (!hero.IsFemale && hero.GetDesires().AttractionMen >= 50))
                {
                    orientation = new TextObject("{=Dramalord161}Homosexual");
                }

                __instance.Stats.Add(new StringPairItemVM(attraction.ToString() + ":", __instance.IsInformationHidden ? text : hero.GetAttractionTo(Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(friendship.ToString() + ":", __instance.IsInformationHidden ? text : hero.GetRelationTo(Hero.MainHero).Trust.ToString()));
                __instance.Stats.Add(new StringPairItemVM(love.ToString() + ":", __instance.IsInformationHidden ? text : hero.GetRelationTo(Hero.MainHero).Love.ToString()));
                __instance.Stats.Add(new StringPairItemVM(sympathy.ToString() + ":", __instance.IsInformationHidden ? text : hero.GetSympathyTo(Hero.MainHero).ToString()));
                __instance.Stats.Add(new StringPairItemVM(horny.ToString() + ":", __instance.IsInformationHidden ? text : hero.GetDesires().Horny.ToString()));
                __instance.Stats.Add(new StringPairItemVM(fertile.ToString() + ":", __instance.IsInformationHidden ? text : (hero.IsFertile()) ? yes : no));
                __instance.Stats.Add(new StringPairItemVM(sexOrientation.ToString() + ":", __instance.IsInformationHidden ? text : orientation.ToString()));
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
}
