using Dramalord.Data;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Dramalord.Behaviors
{
    internal static class AICampaignHelper
    {
        public static List<Hero> ScopeSurroundingsForFlirts(Hero hero, bool includePlayer)
        {
            List<Hero> list = new();

            if (Info.IsHeroLegit(hero))
            {
                if (hero.CurrentSettlement != null)
                {
                    Settlement settlement = hero.CurrentSettlement;
                    foreach (Hero h in settlement.HeroesWithoutParty)
                    {
                        if (h != hero && (h != Hero.MainHero || includePlayer) && Info.ValidateHeroInfo(h) && Info.GetAttractionToHero(hero, h) >= DramalordMCM.Get.MinAttractionForFlirting && (!Info.IsCloseRelativeTo(hero, h) || !DramalordMCM.Get.ProtectFamily))
                        {
                            list.Add(h);
                        }
                    }

                    foreach (MobileParty mp in settlement.Parties)
                    {
                        if (mp != null && Info.IsHeroLegit(mp.LeaderHero) && mp.LeaderHero != hero && (mp.LeaderHero != Hero.MainHero || includePlayer))
                        {
                            Hero target = mp.LeaderHero;
                            if (Info.GetAttractionToHero(hero, target) >= DramalordMCM.Get.MinAttractionForFlirting && (!Info.IsCloseRelativeTo(hero, target) || !DramalordMCM.Get.ProtectFamily))
                            {
                                list.Add(target);
                            }
                        }
                    }
                }
                else if (hero.PartyBelongedTo != null)
                {
                    MobileParty mp = hero.PartyBelongedTo;
                    if (mp != null && mp.Army != null)
                    {
                        foreach (MobileParty mp2 in mp.Army.Parties)
                        {
                            if (mp2 != null && mp2 != mp && Info.IsHeroLegit(mp2.LeaderHero) && (mp2.LeaderHero != Hero.MainHero || includePlayer))
                            {
                                Hero target = mp2.LeaderHero;
                                if (Info.GetAttractionToHero(hero, mp2.LeaderHero) >= DramalordMCM.Get.MinAttractionForFlirting && (!Info.IsCloseRelativeTo(hero, target) || !DramalordMCM.Get.ProtectFamily))
                                {
                                    list.Add(mp2.LeaderHero);
                                }
                            }
                        }
                    }
                }
            }

            list.Sort((h1, h2) => Info.GetAttractionToHero(hero, h1) > Info.GetAttractionToHero(hero, h2) ? -1 : 1);
            return list.Distinct().ToList().Take(DramalordMCM.Get.FlirtsPerDay).ToList();
        }

        public static List<Hero> ScopeSurroundingsForPartners(Hero hero, bool includePlayer)
        {
            List<Hero> list = new();

            if (Info.IsHeroLegit(hero))
            {
                if (hero.CurrentSettlement != null)
                {
                    Settlement settlement = hero.CurrentSettlement;
                    foreach (Hero h in settlement.HeroesWithoutParty)
                    {
                        if (h != hero && Info.IsHeroLegit(h) && (h != Hero.MainHero || includePlayer) && Info.GetIsCoupleWithHero(hero, h))
                        {
                            list.Add(h);
                        }
                    }

                    foreach (MobileParty mp in settlement.Parties)
                    {
                        if (mp != null && mp.LeaderHero != null && mp.LeaderHero != hero && Info.IsHeroLegit(mp.LeaderHero) && (mp.LeaderHero != Hero.MainHero || includePlayer) && Info.GetIsCoupleWithHero(hero, mp.LeaderHero))
                        {
                            list.Add(mp.LeaderHero);
                        }
                    }
                }
                else if (hero.PartyBelongedTo != null)
                {
                    MobileParty mp = hero.PartyBelongedTo;
                    if (mp != null && mp.Army != null)
                    {
                        foreach (MobileParty mp2 in mp.Army.Parties)
                        {
                            if (mp2 != null && mp2 != mp && mp2.LeaderHero != null && Info.IsHeroLegit(mp2.LeaderHero) && (mp2.LeaderHero != Hero.MainHero || includePlayer) && Info.GetIsCoupleWithHero(hero, mp2.LeaderHero))
                            {
                                list.Add(mp2.LeaderHero);
                            }
                        }
                    }
                }
            }

            list.Sort((h1, h2) => Info.GetEmotionToHero(hero, h1) > Info.GetEmotionToHero(hero, h2) ? -1 : 1);
            return list.Distinct().ToList();
        }

        public static Hero? ScopePrisonersForVictim(Hero hero, bool includePlayer)
        {
            if (Info.IsHeroLegit(hero))
            {
                if (hero.CurrentSettlement != null)
                {
                    Settlement settlement = hero.CurrentSettlement;
                    foreach (Hero h in settlement.HeroesWithoutParty)
                    {
                        if (h != hero && h.IsPrisoner && Info.IsHeroLegit(h) && (h != Hero.MainHero || includePlayer) && Info.GetAttractionToHero(hero, h) >= DramalordMCM.Get.MinAttractionForFlirting && (!Info.IsCloseRelativeTo(hero, h) || !DramalordMCM.Get.ProtectFamily))
                        {
                            return h;
                        }
                    }

                    foreach (MobileParty mp in settlement.Parties)
                    {
                        if (mp != null && mp.LeaderHero != null && mp.LeaderHero.IsPrisoner && Info.IsHeroLegit(mp.LeaderHero) && mp.LeaderHero != hero && (mp.LeaderHero != Hero.MainHero || includePlayer) && Info.GetAttractionToHero(hero, mp.LeaderHero) >= DramalordMCM.Get.MinAttractionForFlirting && (!Info.IsCloseRelativeTo(hero, mp.LeaderHero) || !DramalordMCM.Get.ProtectFamily))
                        {
                            return mp.LeaderHero;
                        }
                    }
                }
                else if (hero.PartyBelongedTo != null)
                {
                    MobileParty mp = hero.PartyBelongedTo;
                    if (mp != null && mp.PrisonRoster != null && mp.PrisonRoster.TotalHeroes > 1)
                    {
                        foreach (TroopRosterElement tre in mp.PrisonRoster.GetTroopRoster())
                        {
                            if (tre.Character.IsHero && tre.Character.HeroObject != hero && Info.IsHeroLegit(tre.Character.HeroObject) && (tre.Character.HeroObject != Hero.MainHero || includePlayer) && Info.GetAttractionToHero(hero, tre.Character.HeroObject) >= DramalordMCM.Get.MinAttractionForFlirting && (!Info.IsCloseRelativeTo(hero, tre.Character.HeroObject) || !DramalordMCM.Get.ProtectFamily))
                            {
                                return tre.Character.HeroObject;
                            }
                        }
                    }
                }
            }

            return null;
        }


        public static bool WantsToDateWith(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target) && Info.GetIsCoupleWithHero(hero, target))
            {
                if (CampaignTime.Now.ToDays - Info.GetLastDate(hero, target) >= DramalordMCM.Get.DaysBetweenDates && CampaignTime.Now.ToDays - Info.GetLastDate(target, hero) >= DramalordMCM.Get.DaysBetweenDates)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool WillHaveIntercourse(Hero hero, Hero target, bool byForce)
        {
            if (Info.ValidateHeroMemory(hero, target))
            {
                if ((Info.GetHeroHorny(hero) >= DramalordMCM.Get.MinHornyForIntercouse || hero == Hero.MainHero) && (Info.GetHeroHorny(target) >= DramalordMCM.Get.MinHornyForIntercouse || target == Hero.MainHero || byForce))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool NoticeUnknownPregnancy(Hero watcher, Hero mother)
        {
            if (Info.ValidateHeroMemory(mother, watcher))
            {
                if (mother != watcher && mother.IsPregnant)
                {
                    HeroOffspringData? offspring = Info.GetHeroOffspring(mother);
                    if (offspring != null && offspring.Father != watcher && CampaignTime.Now.ToDays - offspring.Conceived >= DramalordMCM.Get.DaysUntilPregnancyVisible)
                    {
                        Info.ChangeEmotionToHeroBy(watcher, mother, -DramalordMCM.Get.EmotionalLossPregnancy);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool TryLegitimizePregnancy(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target))
            {
                Hero mother = hero.IsFemale ? hero : target;
                Hero father = target.IsFemale ? hero : target;

                if (mother != father && mother.Spouse == father && mother.IsPregnant)
                {
                    HeroOffspringData? offspring = Info.GetHeroOffspring(mother);
                    if (offspring != null && offspring.Father != father && CampaignTime.Now.ToDays - offspring.Conceived <= DramalordMCM.Get.DaysUntilPregnancyVisible)
                    {
                        Info.ChangeOffspringFather(mother, father);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool WillConceiveOffspring(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target))
            {
                Hero mother = hero.IsFemale ? hero : target;
                Hero father = target.IsFemale ? hero : target;

                if (mother != father && !mother.IsPregnant && Info.CanGetPregnant(mother) && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
                {
                    return true;
                }
            }
            return false;
        }


        public static bool WantsToBreakUpWith(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target) && Info.GetEmotionToHero(hero, target) < DramalordMCM.Get.MinEmotionBeforeDivorce && hero.Spouse != target)
            {
                return true;
            }
            return false;
        }

        internal static bool WantsToDivorceFrom(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target) && Info.GetEmotionToHero(hero, target) < DramalordMCM.Get.MinEmotionBeforeDivorce && hero.Spouse == target)
            {
                return true;
            }
            return false;
        }

        internal static HeroOffspringData? CanGiveBirth(Hero hero)
        {
            if (hero.IsFemale && Info.IsHeroLegit(hero))
            {
                HeroOffspringData? offspring = Info.GetHeroOffspring(hero);
                if (offspring != null)
                {
                    if (CampaignTime.Now.ToDays - offspring.Conceived > DramalordMCM.Get.PregnancyDuration)
                    {
                        return offspring;
                    }
                }
            }
            return null;
        }

        internal static Hero? GetChildBornToday(Hero hero)
        {
            if (Info.IsHeroLegit(hero))
            {
                return hero.Children.FirstOrDefault(item => item.BirthDay.ToDays == CampaignTime.Now.ToDays);
            }
            return null;
        }

        internal static bool CanAdoptFromOrphanage(Hero hero, Hero partner)
        {
            if (hero.Spouse == partner && Info.ValidateHeroMemory(hero, partner))
            {
                if (hero.IsFemale && !partner.IsFemale && hero.Age < DramalordMCM.Get.MaxFertilityAge || !hero.IsFemale && partner.IsFemale && partner.Age < DramalordMCM.Get.MaxFertilityAge)
                {
                    return false; // may still get pregnant
                }

                if (CampaignTime.Now.ToDays - Info.GetLastAdoption(hero, partner) >= DramalordMCM.Get.WaitBetweenAdopting && !Info.IsOrphanageEmpty())
                {
                    return true;
                }
            }
            return false;
        }


        internal static Hero? GetPotentialMarriagePartner(Hero hero)
        {
            if (Info.ValidateHeroInfo(hero) && hero.Spouse == null)
            {
                List<Hero> partners = ScopeSurroundingsForPartners(hero, true);
                Hero? winner = null;
                float score = 0;
                foreach (Hero target in partners)
                {
                    float emotion = Info.GetEmotionToHero(hero, target);
                    if (emotion >= DramalordMCM.Get.MinEmotionForMarriage && Info.GetEmotionToHero(target, hero) >= DramalordMCM.Get.MinEmotionForMarriage && emotion > score && target.Spouse == null)
                    {
                        score = emotion;
                        winner = target;
                    }
                }
                return winner;
            }
            return null;
        }
    }
}
