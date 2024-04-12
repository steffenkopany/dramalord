using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
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
            return HeroInfo[hero].IntercourseSkill;
        }

        internal static void ChangeIntercourseSkillBy(Hero hero, float value)
        {
            HeroInfo[hero].IntercourseSkill = MBMath.ClampFloat(HeroInfo[hero].IntercourseSkill + value, 0, 100);
        }

        internal static bool CanGetPregnant(Hero hero)
        {
            HeroInfoData info = HeroInfo[hero];
            if (hero.IsFemale && hero.Age <= DramalordMCM.Get.MaxFertilityAge && (info.PeriodDayOfSeason > CampaignTime.Now.GetDayOfSeason || info.PeriodDayOfSeason + DramalordMCM.Get.PeriodDuration < CampaignTime.Now.GetDayOfSeason))
            {
                return true;
            }
            return false;
        }

        internal static double GetLastDaySeen(Hero hero, Hero target)
        {
            HeroTuple tuple = new(hero, target);
            return HeroMemory[tuple].LastMet;
        }

        internal static void SetLastDaySeen(Hero hero, Hero target, double days)
        {
            if (target == Hero.MainHero)
            {
                hero.LastMeetingTimeWithPlayer = CampaignTime.Days((float)days);
            }
            else if (hero == Hero.MainHero)
            {
                target.LastMeetingTimeWithPlayer = CampaignTime.Days((float)days);
            }

            HeroTuple tuple = new(hero, target);
            HeroMemory[tuple].LastMet = days;
        }

        internal static float GetEmotionToHero(Hero hero, Hero target)
        {
            if (target == Hero.MainHero && DramalordMCM.Get.PlayerAlwaysLoved)
            {
                return 100;
            }

            HeroTuple tuple = new(hero, target);
            return HeroMemory[tuple].Emotion;
        }

        internal static void ChangeEmotionToHeroBy(Hero hero, Hero target, float change)
        {
            HeroTuple tuple = new(hero, target);
            HeroMemory[tuple].Emotion = MBMath.ClampFloat(HeroMemory[tuple].Emotion + change, -100, 100);
        }

        internal static float GetHeroHorny(Hero hero)
        {
            return HeroInfo[hero].Horny;
        }

        internal static void ChangeHeroHornyBy(Hero hero, float change)
        {
            HeroInfo[hero].Horny = MBMath.ClampFloat(HeroInfo[hero].Horny + change, 0, 100);
        }

        internal static float GetHeroLibido(Hero hero)
        {
            return HeroInfo[hero].Libido;
        }

        internal static bool GetHeroHasToy(Hero hero)
        {
            return HeroInfo[hero].HasToy;
        }

        internal static void SetHeroHasToy(Hero hero, bool value)
        {
            HeroInfo[hero].HasToy = value;
        }

        internal static int GetAttractionToHero(Hero hero, Hero target)
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
            rating -= Math.Abs(MBMath.ClampFloat(heroData.AttractionAgeDiff + hero.Age, 18, 130) - target.Age);

            return MBMath.ClampInt((int)rating, 0, 100);
        }

        internal static int GetFlirtedWithPlayer(Hero hero)
        {
            HeroTuple tuple = new(hero, Hero.MainHero);
            return HeroMemory[tuple].FlirtCount;
        }

        internal static void SetFlirtedWithPlayer(Hero hero)
        {
            HeroTuple tuple = new(hero, Hero.MainHero);
            HeroMemory[tuple].FlirtCount++;
        }

        internal static void ResetFlirtedWithPlayer(Hero hero)
        {
            HeroTuple tuple = new(hero, Hero.MainHero);
            HeroMemory[tuple].FlirtCount = 0;
        }

        internal static bool IsCoupleWithHero(Hero hero, Hero target)
        {
            HeroTuple tuple = new(hero, target);
            return HeroMemory[tuple].IsCouple;
        }

        internal static void SetIsCoupleWithHero(Hero hero, Hero target, bool value)
        {
            HeroTuple tuple = new(hero, target);
            HeroMemory[tuple].IsCouple = value;
        }

        internal static double GetLastDate(Hero hero, Hero target)
        {
            HeroTuple tuple = new(hero, target);
            return HeroMemory[tuple].LastDate;
        }

        internal static void SetLastPrivateMeeting(Hero hero, Hero target, double value)
        {
            HeroTuple tuple = new(hero, target);
            HeroMemory[tuple].LastDate = value;
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
            HeroTuple tuple = new(hero, target);
            return HeroMemory[tuple].LastAdoption;
        }

        internal static void SetLastAdoption(Hero hero, Hero target, double value)
        {
            HeroTuple tuple = new(hero, target);
            HeroMemory[tuple].LastAdoption = value;
        }

        /*
        internal static void ForgetHeroes(Hero hero)
        {
            double days = CampaignTime.Now.ToDays;
            HeroMemory.Where(item => item.Key.Actor == hero && (item.Key.Target != Hero.MainHero || !Info.GetHeroHasToy(hero)) && days - item.Value.LastMet > DramalordMCM.Get.DaysApartCompleteForget).ToList().ForEach( entry => HeroMemory.Remove(entry.Key));
            HeroMemory.Where(item => item.Key.Actor == hero && (item.Key.Target != Hero.MainHero || !Info.GetHeroHasToy(hero)) && days - item.Value.LastMet > DramalordMCM.Get.DaysApartStartForget).ToList().ForEach(entry => HeroMemory[entry.Key].Emotion -= (HeroMemory[entry.Key].Emotion > 0) ? DramalordMCM.Get.AmountEmotionForget : 0);
        }
        */
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
            CharacterTraits heroTraits = hero.GetHeroTraits();
            CharacterTraits targetTraits = target.GetHeroTraits();

            if (heroTraits == null || targetTraits == null)
            {
                return 0;
            }

            int score = 0;
            /*
            score += (heroTraits.Mercy < 0 && targetTraits.Mercy < 0) || (heroTraits.Mercy > 0 && targetTraits.Mercy > 0 || (heroTraits.Mercy == targetTraits.Mercy)) ? 1 : 0;
            score += (heroTraits.Mercy < 0 && targetTraits.Mercy > 0) || (heroTraits.Mercy > 0 && targetTraits.Mercy < 0) ? -1 : 0;
            score += (heroTraits.Generosity < 0 && targetTraits.Generosity < 0) || (heroTraits.Generosity > 0 && targetTraits.Generosity > 0 || (heroTraits.Generosity == targetTraits.Generosity)) ? 1 : 0;
            score += (heroTraits.Generosity < 0 && targetTraits.Generosity > 0) || (heroTraits.Generosity > 0 && targetTraits.Generosity < 0) ? -1 : 0;
            score += (heroTraits.Honor < 0 && targetTraits.Honor < 0) || (heroTraits.Honor > 0 && targetTraits.Honor > 0 || (heroTraits.Honor == targetTraits.Honor)) ? 1 : 0;
            score += (heroTraits.Honor < 0 && targetTraits.Honor > 0) || (heroTraits.Honor > 0 && targetTraits.Honor < 0) ? -1 : 0;
            score += (heroTraits.Valor < 0 && targetTraits.Valor < 0) || (heroTraits.Valor > 0 && targetTraits.Valor > 0 || (heroTraits.Valor == targetTraits.Valor)) ? 1 : 0;
            score += (heroTraits.Valor < 0 && targetTraits.Valor > 0) || (heroTraits.Valor > 0 && targetTraits.Valor < 0) ? -1 : 0;
            */

            score += ((heroTraits.Mercy > 0 && targetTraits.Mercy > 0) || (heroTraits.Mercy < 0 && targetTraits.Mercy < 0)) ? 1 : 0;
            score += ((heroTraits.Mercy < 0 && targetTraits.Mercy > 0) || (heroTraits.Mercy > 0 && targetTraits.Mercy < 0)) ? -1 : 0;
            score += ((heroTraits.Generosity > 0 && targetTraits.Generosity > 0) || (heroTraits.Generosity < 0 && targetTraits.Generosity < 0)) ? 1 : 0;
            score += ((heroTraits.Generosity < 0 && targetTraits.Generosity > 0) || (heroTraits.Generosity > 0 && targetTraits.Generosity < 0)) ? -1 : 0;
            score += ((heroTraits.Honor > 0 && targetTraits.Honor > 0) || (heroTraits.Honor < 0 && targetTraits.Honor < 0)) ? 1 : 0;
            score += ((heroTraits.Honor < 0 && targetTraits.Honor > 0) || (heroTraits.Honor > 0 && targetTraits.Honor < 0)) ? -1 : 0;
            score += ((heroTraits.Valor > 0 && targetTraits.Valor > 0) || (heroTraits.Valor < 0 && targetTraits.Valor < 0)) ? 1 : 0;
            score += ((heroTraits.Valor < 0 && targetTraits.Valor > 0) || (heroTraits.Valor > 0 && targetTraits.Valor < 0)) ? -1 : 0;
            score += ((heroTraits.Calculating > 0 && targetTraits.Calculating > 0) || (heroTraits.Calculating < 0 && targetTraits.Calculating < 0)) ? 1 : 0;
            score += ((heroTraits.Calculating < 0 && targetTraits.Calculating > 0) || (heroTraits.Calculating > 0 && targetTraits.Calculating < 0)) ? -1 : 0;
            //score += hero.GetSkillValue(DefaultSkills.Charm) / 100;
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
                            (hero.Spouse == target) ? DramalordMCM.Get.MinEmotionForMarriage : 0,
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

        /*public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }*/
    }
}
