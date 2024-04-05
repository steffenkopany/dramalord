using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static TaleWorlds.CampaignSystem.Actions.KillCharacterAction;

namespace Dramalord.Data
{
    internal static class Info
    {
        private static Dictionary<Hero, HeroInfoData> HeroInfo = new();
        private static Dictionary<HeroTuple, HeroMemoryData> HeroMemory = new();
        private static Dictionary<Hero, HeroOffspringData> HeroOffspring = new();
        private static List<Hero> HeroOrphanage = new();

        internal static HeroInfoData? GetHeroInfoDataCopy(Hero hero)
        {
            if(ValidateHeroInfo(hero))
            {
                return new HeroInfoData(HeroInfo[hero]);
            }
            return null;
        }

        internal static void AddHeroOffspring(Hero mother, Hero father, bool byForce)
        {
            if (!HeroOffspring.Keys.Contains(mother))
            {
                HeroOffspring.Add(mother, new HeroOffspringData(father, mother, byForce));
            }
        }

        internal static HeroOffspringData? GetHeroOffspring(Hero hero)
        {
            if (HeroOffspring.Keys.Contains(hero))
            {
                return HeroOffspring[hero];
            }
            return null;
        }

        internal static void RemoveHeroOffspring(Hero hero)
        {
            if (HeroOffspring.Keys.Contains(hero))
            {
                HeroOffspring.Remove(hero);
            }
        }

        internal static void ChangeOffspringFather(Hero mother, Hero father)
        {
            if (HeroOffspring.Keys.Contains(mother))
            {
                HeroOffspring[mother].Father = father;
            }
        }

        internal static float GetIntercourseSkill(Hero hero)
        {
            if (ValidateHeroInfo(hero))
            {
                return HeroInfo[hero].IntercourseSkill;
            }
            throw new ArgumentException();
        }

        internal static void ChangeIntercourseSkillBy(Hero hero, float value)
        {
            if (ValidateHeroInfo(hero))
            {
                HeroInfo[hero].Horny = Clamp<float>(HeroInfo[hero].IntercourseSkill + value, 0, 100);
            }
        }

        internal static bool CanGetPregnant(Hero hero)
        {
            if(ValidateHeroInfo(hero))
            {
                if (hero.IsFemale && hero.Age <= DramalordMCM.Get.MaxFertilityAge && (HeroInfo[hero].PeriodDayOfSeason > CampaignTime.Now.GetDayOfSeason || HeroInfo[hero].PeriodDayOfSeason + DramalordMCM.Get.PeriodDuration < CampaignTime.Now.GetDayOfSeason))
                {
                    return true;
                }
                return false;
            }
            throw new ArgumentException();
        }

        internal static double GetLastDaySeen(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target))
            {
                return HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.LastMet;
            }
            throw new ArgumentException();
        }

        internal static void SetLastDaySeen(Hero hero, Hero target, double days)
        {
            if (ValidateHeroMemory(hero, target) && ValidateHeroMemory(target, hero))
            {
                if (target == Hero.MainHero)
                {
                    hero.LastMeetingTimeWithPlayer = CampaignTime.Days((float)days);
                }
                else if (hero == Hero.MainHero)
                {
                    target.LastMeetingTimeWithPlayer = CampaignTime.Days((float)days);
                }

                HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.LastMet = days;
                HeroMemory.FirstOrDefault(item => item.Key.Actor == target && item.Key.Target == hero).Value.LastMet = days;
            }
        }

        internal static float GetEmotionToHero(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target))
            {
                if (target == Hero.MainHero && DramalordMCM.Get.PlayerAlwaysLoved)
                {
                    return 100;
                }

                return HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.Emotion;
            }
            throw new ArgumentException();
        }

        internal static void ChangeEmotionToHeroBy(Hero hero, Hero target, float change)
        {
            if (ValidateHeroMemory(hero, target))
            {
                HeroTuple tuple = new HeroTuple(hero, target);
                HeroMemory[tuple].Emotion = Clamp<float>(HeroMemory[tuple].Emotion + change, -100, 100);
            }  
        }

        internal static float GetHeroHorny(Hero hero)
        {
            if (ValidateHeroInfo(hero))
            {
                return HeroInfo[hero].Horny;
            }
            throw new ArgumentException();
        }

        internal static void ChangeHeroHornyBy(Hero hero, float change)
        {
            if (ValidateHeroInfo(hero))
            {
                HeroInfo[hero].Horny = Clamp<float>(HeroInfo[hero].Horny + change, 0, 100);
            } 
        }

        internal static float GetHeroLibido(Hero hero)
        {
            if (ValidateHeroInfo(hero))
            {
                return HeroInfo[hero].Libido;
            }
            throw new ArgumentException();
        }

        internal static bool GetHeroHasToy(Hero hero)
        {
            if (ValidateHeroInfo(hero))
            {
                return HeroInfo[hero].HasToy;
            }
            throw new ArgumentException();
        }

        internal static void SetHeroHasToy(Hero hero, bool value)
        {
            if (ValidateHeroInfo(hero))
            {
                HeroInfo[hero].HasToy = value;
            }
        }

        internal static int GetAttractionToHero(Hero hero, Hero target)
        {
            if (ValidateHeroInfo(hero) && ValidateHeroInfo(target))
            {
                if (target == Hero.MainHero && DramalordMCM.Get.PlayerAlwaysAttractive)
                {
                    return 100;
                }

                HeroInfoData heroData = HeroInfo[hero];
                float rating = 0;
                rating += (target.IsFemale) ? heroData.AttractionWomen : heroData.AttractionMen;
                rating += ((hero.IsFemale && target.IsFemale) || (!hero.IsFemale && !target.IsFemale)) ? -DramalordMCM.Get.OtherSexAttractionModifier : DramalordMCM.Get.OtherSexAttractionModifier;
                rating += (hero.Culture == target.Culture) ? 10 : 0;
                rating -= Math.Abs((heroData.AttractionWeight - target.BodyProperties.Weight) * 10);
                rating -= Math.Abs((heroData.AttractionBuild - target.BodyProperties.Build) * 10);
                rating -= Math.Abs(Clamp<float>(heroData.AttractionAgeDiff + hero.Age, 18, 130) - target.Age);

                return Clamp<int>((int)rating, 0, 100);
            }
            throw new ArgumentException();
        }

        internal static int GetFlirtCountWithHero(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target))
            {
                return HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.FlirtCount;
            }
            throw new ArgumentException();
        }

        internal static void IncreaseFlirtCountWithHero(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target))
            {
                HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.FlirtCount++;
            }
        }

        internal static void ResetFlirtCountWithHero(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target))
            {
                HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.FlirtCount = 0;
            }
        }

        internal static bool GetIsCoupleWithHero(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target))
            {
                return HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.IsCouple;
            }
            throw new ArgumentException();
        }

        internal static void SetIsCoupleWithHero(Hero hero, Hero target, bool value)
        {
            if (ValidateHeroMemory(hero, target) && ValidateHeroMemory(target, hero))
            {
                HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.IsCouple = value;
                HeroMemory.FirstOrDefault(item => item.Key.Actor == target && item.Key.Target == hero).Value.IsCouple = value;
            }
        }

        internal static double GetLastDate(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target) && ValidateHeroMemory(target, hero))
            {
                return HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.LastDate;
            }
            throw new ArgumentException();
        }

        internal static void SetLastPrivateMeeting(Hero hero, Hero target, double value)
        {
            if (ValidateHeroMemory(hero, target) && ValidateHeroMemory(target, hero))
            {
                HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.LastDate = value;
                HeroMemory.FirstOrDefault(item => item.Key.Actor == target && item.Key.Target == hero).Value.LastDate = value;
            }
        }

        internal static Hero? PullRandomOrphan()
        {
            if(HeroOrphanage.Count() > 0)
            {
                Hero orphan = HeroOrphanage[MBRandom.RandomInt(HeroOrphanage.Count())];
                HeroOrphanage.Remove(orphan);
                return orphan;
            }
            return null;
        }

        internal static bool IsOrphanageEmpty()
        {
            return HeroOrphanage.IsEmpty();
        }

        internal static bool IsHeroOrphan(Hero hero)
        {
            return HeroOrphanage.Contains(hero);
        }

        internal static void AddOrphan(Hero hero)
        {
            if(!HeroOrphanage.Contains(hero))
            {
                HeroOrphanage.Add(hero);
            }
        }

        internal static void OnOrphanComesOfAge(Hero hero)
        {
            HeroOrphanage.Remove(hero);
        }

        internal static double GetLastAdoption(Hero hero, Hero target)
        {
            if (ValidateHeroMemory(hero, target) && ValidateHeroMemory(target, hero))
            {
                return HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.LastAdoption;
            }
            throw new ArgumentException();
        }

        internal static void SetLastAdoption(Hero hero, Hero target, double value)
        {
            if(ValidateHeroMemory(hero, target) && ValidateHeroMemory(target, hero))
            {
                HeroMemory.FirstOrDefault(item => item.Key.Actor == hero && item.Key.Target == target).Value.LastAdoption = value;
                HeroMemory.FirstOrDefault(item => item.Key.Actor == target && item.Key.Target == hero).Value.LastAdoption = value;
            }
        }

        internal static void ForgetHeroes(Hero hero)
        {
            double days = CampaignTime.Now.ToDays;
            HeroMemory.Where(item => item.Key.Actor == hero && (item.Key.Target != Hero.MainHero || !Info.GetHeroHasToy(hero)) && days - item.Value.LastMet > DramalordMCM.Get.DaysApartCompleteForget).ToList().ForEach( entry => HeroMemory.Remove(entry.Key));
            HeroMemory.Where(item => item.Key.Actor == hero && (item.Key.Target != Hero.MainHero || !Info.GetHeroHasToy(hero)) && days - item.Value.LastMet > DramalordMCM.Get.DaysApartStartForget).ToList().ForEach(entry => HeroMemory[entry.Key].Emotion -= (HeroMemory[entry.Key].Emotion > 0) ? DramalordMCM.Get.AmountEmotionForget : 0);
        }

        internal static void SyncData(IDataStore dataStore)
        {
            if(dataStore.IsLoading)
            {
                Dictionary<Hero, HeroInfoData> info = new();
                Dictionary<HeroTuple, HeroMemoryData> memories = new();
                Dictionary<Hero, HeroOffspringData> offsprings = new();
                List<Hero> orphanage = new List<Hero>();

                dataStore.SyncData("DramalordHeroInfo", ref info);
                dataStore.SyncData("DramalordHeroMemory", ref memories);
                dataStore.SyncData("DramalordHeroOffspring", ref offsprings);
                dataStore.SyncData("DramalordHeroOrphanage", ref orphanage);

                HeroInfo = info;
                HeroMemory = memories;
                HeroOffspring = offsprings;
                HeroOrphanage = orphanage;
            }
            else if(dataStore.IsSaving)
            {
                dataStore.SyncData("DramalordHeroInfo", ref HeroInfo);
                dataStore.SyncData("DramalordHeroMemory", ref HeroMemory);
                dataStore.SyncData("DramalordHeroOffspring", ref HeroOffspring);
                dataStore.SyncData("DramalordHeroOrphanage", ref HeroOrphanage);
            }   
        }

        public static int GetTraitscoreToHero(Hero hero, Hero target)
        {
            if (hero.GetHeroTraits() == null || target.GetHeroTraits() == null)
            {
                return 0;
            }

            int score = 0;
            score += (hero.GetHeroTraits().Mercy < 0 && target.GetHeroTraits().Mercy < 0) || (hero.GetHeroTraits().Mercy > 0 && target.GetHeroTraits().Mercy > 0 || (hero.GetHeroTraits().Mercy == target.GetHeroTraits().Mercy)) ? 1 : 0;
            score += (hero.GetHeroTraits().Mercy < 0 && target.GetHeroTraits().Mercy > 0) || (hero.GetHeroTraits().Mercy > 0 && target.GetHeroTraits().Mercy < 0) ? -1 : 0;
            score += (hero.GetHeroTraits().Generosity < 0 && target.GetHeroTraits().Generosity < 0) || (hero.GetHeroTraits().Generosity > 0 && target.GetHeroTraits().Generosity > 0 || (hero.GetHeroTraits().Generosity == target.GetHeroTraits().Generosity)) ? 1 : 0;
            score += (hero.GetHeroTraits().Generosity < 0 && target.GetHeroTraits().Generosity > 0) || (hero.GetHeroTraits().Generosity > 0 && target.GetHeroTraits().Generosity < 0) ? -1 : 0;
            score += (hero.GetHeroTraits().Honor < 0 && target.GetHeroTraits().Honor < 0) || (hero.GetHeroTraits().Honor > 0 && target.GetHeroTraits().Honor > 0 || (hero.GetHeroTraits().Honor == target.GetHeroTraits().Honor)) ? 1 : 0;
            score += (hero.GetHeroTraits().Honor < 0 && target.GetHeroTraits().Honor > 0) || (hero.GetHeroTraits().Honor > 0 && target.GetHeroTraits().Honor < 0) ? -1 : 0;
            score += (hero.GetHeroTraits().Valor < 0 && target.GetHeroTraits().Valor < 0) || (hero.GetHeroTraits().Valor > 0 && target.GetHeroTraits().Valor > 0 || (hero.GetHeroTraits().Valor == target.GetHeroTraits().Valor)) ? 1 : 0;
            score += (hero.GetHeroTraits().Valor < 0 && target.GetHeroTraits().Valor > 0) || (hero.GetHeroTraits().Valor > 0 && target.GetHeroTraits().Valor < 0) ? -1 : 0;
            return score * DramalordMCM.Get.TraitScoreMultiplyer;
        }

        internal static bool ValidateHeroInfo(Hero hero)
        {
            if(IsHeroLegit(hero))
            {
                if (!HeroInfo.ContainsKey(hero))
                {
                    HeroInfo.Add(hero, new HeroInfoData(
                            MBRandom.RandomInt(10, 100),
                            MBRandom.RandomInt(10, 100),
                            MBRandom.RandomFloatRanged(0, 1),
                            MBRandom.RandomFloatRanged(0, 1),
                            MBRandom.RandomInt(-20, 20),
                            MBRandom.RandomInt(1, 100),
                            MBRandom.RandomInt(1, 10),
                            MBRandom.RandomInt(1, CampaignTime.DaysInSeason),
                            hero.Children.Count * 2
                        ));
                }
                return true;
            }
            return false;
        }

        internal static bool ValidateHeroMemory(Hero hero, Hero target)
        {
            if(IsHeroLegit(hero) && IsHeroLegit(target))
            {
                HeroTuple heroTuple = new HeroTuple(hero, target);
                if(!HeroMemory.ContainsKey(heroTuple))
                {
                    HeroMemory.Add(heroTuple, new HeroMemoryData(
                            (hero.Spouse == target) ? DramalordMCM.Get.MinEmotionForMarriage : Info.GetTraitscoreToHero(hero, target),
                            0,
                            (hero.Spouse == target) ? MBRandom.RandomInt(84, 840) : 0,
                            (hero.Spouse == target),
                            CampaignTime.Now.ToDays - MBRandom.RandomInt(0, CampaignTime.DaysInSeason)
                            ));
                }
                return true;
            }
            return false;
        }

        internal static void OnHeroUnregistered(Hero hero)
        {
            if (HeroInfo.Keys.Contains(hero))
            {
                HeroInfo.Remove(hero);
            }

            HeroMemory.Where(item => item.Key.Actor == hero || item.Key.Target == hero).ToList().ForEach( entry => HeroMemory.Remove(entry.Key));

            if (HeroOrphanage.Contains(hero))
            {
                HeroOrphanage.Remove(hero);
            }
        }

        internal static void OnHeroKilled(Hero victim, Hero murderer, KillCharacterActionDetail detail, bool force)
        {
            if(HeroInfo.Keys.Contains(victim))
            {
                HeroInfo.Remove(victim);
            }

            HeroMemory.Where(item => item.Key.Actor == victim || item.Key.Target == victim).ToList().ForEach(entry => HeroMemory.Remove(entry.Key));

            if(HeroOrphanage.Contains(victim))
            {
                HeroOrphanage.Remove(victim);
            }
        }

        internal static bool IsHeroLegit(Hero hero)
        {
            return hero != null && !hero.IsChild && (hero.IsLord || hero.IsPlayerCompanion || hero.IsWanderer) && hero.IsAlive && !hero.IsDisabled;
        }

        internal static bool IsCloseRelativeTo(Hero hero, Hero target)
        {
            if(hero.Children.Contains(target) || target.Children.Contains(hero) || hero.Siblings.Contains(target) || target.Siblings.Contains(hero))
            {
                return true;
            }
            if((hero.Father != null && hero.Father.Siblings.Contains(target)) || (hero.Mother != null && hero.Mother.Siblings.Contains(target)))
            {
                return true;
            }
            if ((target.Father != null && target.Father.Siblings.Contains(hero)) || (target.Mother != null && target.Mother.Siblings.Contains(hero)))
            {
                return true;
            }
            foreach(Hero child in hero.Children)
            {
                if(child.Children.Contains(target))
                {
                    return true;
                }
            }
            foreach (Hero child in target.Children)
            {
                if (child.Children.Contains(hero))
                {
                    return true;
                }
            }
            
            return false;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
