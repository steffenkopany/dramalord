using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Dramalord.Data
{
    internal static class HeroExtensions
    {
        public static DramalordTraits GetDramalordTraits(this Hero hero)
        {
            return new DramalordTraits(hero);
        }

        public static HeroFeelings GetDramalordFeelings(this Hero hero, Hero other)
        {
            return DramalordRelations.GetHeroFeelings(hero, other);
        }

        public static int GetRelationTo(this Hero hero, Hero target)
        {
            return hero.GetBaseHeroRelation(target);
        }

        public static void ChangeRelationTo(this Hero hero, Hero target, int relationChange)
        {
            hero.SetPersonalRelation(target, hero.GetBaseHeroRelation(target) + relationChange);
        }

        public static HeroPersonality GetDramalordPersonality(this Hero hero)
        {
            return new HeroPersonality(hero);
        }

        public static List<CharacterObject> GetHeroSpouses(this Hero hero)
        {
            return DramalordRelations.GetHeroSpouses(hero);
        }

        public static List<CharacterObject> GetHeroLovers(this Hero hero)
        {
            return DramalordRelations.GetHeroLovers(hero);
        }

        public static List<CharacterObject> GetHeroFriendsWithBenefits(this Hero hero)
        {
            return DramalordRelations.GetHeroFriendsWithBenefits(hero);
        }

        public static bool IsSpouse(this Hero hero, Hero other)
        {
            return hero.Spouse == other || DramalordRelations.GetHeroSpouses(hero).Contains(other.CharacterObject);
        }

        public static bool IsLover(this Hero hero, Hero other)
        {
            return DramalordRelations.GetHeroLovers(hero).Contains(other.CharacterObject);
        }

        public static bool IsFriendWithBenefits(this Hero hero, Hero other)
        {
            return DramalordRelations.GetHeroFriendsWithBenefits(hero).Contains(other.CharacterObject);
        }

        public static void SetSpouse(this Hero hero, Hero other)
        {
            DramalordRelations.GetHeroFriendsWithBenefits(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroLovers(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroFriendsWithBenefits(other).Remove(hero.CharacterObject);
            DramalordRelations.GetHeroLovers(other).Remove(hero.CharacterObject);

            List<CharacterObject> heroSpouses = DramalordRelations.GetHeroSpouses(hero);
            List<CharacterObject> otherSpouses = DramalordRelations.GetHeroSpouses(other);
            if(!heroSpouses.Contains(other.CharacterObject))
            {
                heroSpouses.Add(other.CharacterObject);
            }
            if (!otherSpouses.Contains(hero.CharacterObject))
            {
                otherSpouses.Add(hero.CharacterObject);
            }
        }

        public static void SetLover(this Hero hero, Hero other)
        {
            DramalordRelations.GetHeroFriendsWithBenefits(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroSpouses(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroFriendsWithBenefits(other).Remove(hero.CharacterObject);
            DramalordRelations.GetHeroSpouses(other).Remove(hero.CharacterObject);

            List<CharacterObject> heroLovers = DramalordRelations.GetHeroLovers(hero);
            List<CharacterObject> otherLovers = DramalordRelations.GetHeroLovers(other);
            if (!heroLovers.Contains(other.CharacterObject))
            {
                heroLovers.Add(other.CharacterObject);
            }
            if (!otherLovers.Contains(hero.CharacterObject))
            {
                otherLovers.Add(hero.CharacterObject);
            }
        }

        public static void SetFriendWithBenefits(this Hero hero, Hero other)
        {
            DramalordRelations.GetHeroLovers(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroSpouses(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroLovers(other).Remove(hero.CharacterObject);
            DramalordRelations.GetHeroSpouses(other).Remove(hero.CharacterObject);

            List<CharacterObject> heroFWB = DramalordRelations.GetHeroFriendsWithBenefits(hero);
            List<CharacterObject> otherFWB = DramalordRelations.GetHeroFriendsWithBenefits(other);
            if (!heroFWB.Contains(other.CharacterObject))
            {
                heroFWB.Add(other.CharacterObject);
            }
            if (!otherFWB.Contains(hero.CharacterObject))
            {
                otherFWB.Add(hero.CharacterObject);
            }
        }

        public static void ClearAllRelationships(this Hero hero, Hero other)
        {
            DramalordRelations.GetHeroLovers(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroLovers(other).Remove(hero.CharacterObject);
            DramalordRelations.GetHeroSpouses(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroSpouses(other).Remove(hero.CharacterObject);
            DramalordRelations.GetHeroFriendsWithBenefits(hero).Remove(other.CharacterObject);
            DramalordRelations.GetHeroFriendsWithBenefits(other).Remove(hero.CharacterObject);
        }

        public static bool IsDramalordPregnant(this Hero hero)
        {
            return DramalordPregnancies.GetHeroPregnancy(hero) != null;
        }

        public static HeroPregnancy? GetDramalordPregnancy(this Hero hero)
        {
            return DramalordPregnancies.GetHeroPregnancy(hero);
        }

        public static bool MakeDramalordPregnant(this Hero hero, Hero father)
        {
            if(hero.IsDramalordLegit() && father.IsDramalordLegit() && DramalordPregnancies.GetHeroPregnancy(hero) == null)
            {
                int eventID = DramalordEvents.AddHeroEvent(hero, father, EventType.Pregnancy, 1000);
                DramalordPregnancies.AddHeroPregnancy(hero, father, eventID);
                hero.IsPregnant = true;
                return true;
            }
            return false;
        }

        public static void ClearDramalordPregnancy(this Hero hero)
        {
            DramalordPregnancies.RemovePregnancy(hero);
        }

        public static bool IsOrphan(this Hero hero)
        {
            return DramalordOrphanage.Orphans.Where(item => item.Character == hero.CharacterObject).Any();
        }

        public static bool CanInteractWithPlayer(this Hero hero)
        {
            return (hero.PartyBelongedTo == null || hero.PartyBelongedTo.Army == null || DramalordMCM.Get.AllowApproachInArmy) &&
                (hero.CurrentSettlement == null || DramalordMCM.Get.AllowApproachInSettlement) &&
                (hero.PartyBelongedTo == null || hero.PartyBelongedTo != MobileParty.MainParty || DramalordMCM.Get.AllowApproachInParty);
        }

        public static List<HeroMemory> GetDramalordMemory(this Hero hero)
        {
            return DramalordMemories.GetHeroMemory(hero);
        }

        public static void AddDramalordMemory(this Hero hero, int eventID, MemoryType type, Hero source, bool active)
        {
            DramalordMemories.AddHeroMemory(hero, eventID, type, source.CharacterObject, active);
        }

        public static bool HasDramalordMemory(this Hero hero, int eventID)
        {
            return DramalordMemories.HasHeroMemory(hero, eventID);
        }

        public static void ActivateDramalordMemory(this Hero hero, int eventID, bool active)
        {
            DramalordMemories.SetHeroMemoryActive(hero, eventID, active);
        }

        public static bool IsAngryWith(this Hero hero, Hero target)
        {
            if(hero.IsDramalordLegit() && target.IsDramalordLegit())
            {
                return DramalordMemories.GetHeroMemory(hero).Where(item => item.Event.Type == EventType.Anger && item.Event.Hero2 == target.CharacterObject).Any();
            }
            return false;
        }

        public static void MakeAngryWith(this Hero hero, Hero target, int dayDuration)
        {
            DramalordMemories.GetHeroMemory(hero).Where(item => item.Event.Type == EventType.Anger && item.Event.Hero2 == target.CharacterObject).Do(item =>
            {
                item.Event.CampaignDay = (uint)CampaignTime.Now.ToDays;
                item.Event.DaysAlive = (uint)dayDuration;
                return;
            });

            int eventId = DramalordEvents.AddHeroEvent(hero, target, EventType.Anger, dayDuration);
            hero.AddDramalordMemory(eventId, MemoryType.Participant, hero, true);
        }

        public static bool IsDramalordLegit(this Hero hero)
        {
            return hero != null && !hero.IsChild && (hero.IsLord || hero.IsPlayerCompanion || hero.IsWanderer) && hero.IsAlive && !hero.IsDisabled;
        }

        public static int GetDramalordAttractionTo(this Hero hero, Hero target)
        {
            int rating = DramalordMCM.Get.PlayerBaseAttraction;
            rating += (target.IsFemale) ? hero.GetDramalordTraits().AttractionWomen : hero.GetDramalordTraits().AttractionMen;
            rating += ((hero.IsFemale && !target.IsFemale) ||(!hero.IsFemale && target.IsFemale)) ? DramalordMCM.Get.OtherSexAttractionModifier : -DramalordMCM.Get.OtherSexAttractionModifier;
            rating += (hero.Culture == target.Culture) ? 10 : 0;
            rating -= Math.Abs(hero.GetDramalordTraits().AttractionWeight - (int)(target.BodyProperties.Weight * 100f));
            rating -= Math.Abs(hero.GetDramalordTraits().AttractionBuild - (int)(target.BodyProperties.Build * 100f));
            rating -= Math.Abs(MBMath.ClampInt(hero.GetDramalordTraits().AttractionAgeDiff + (int)hero.Age, 18, 130) - (int)target.Age);
            rating += (target == Hero.MainHero) ? hero.GetDramalordFeelings(target).Emotion / 4 : 0;

            return MBMath.ClampInt((int)rating, 0, 100);
        }

        public static bool GetDramalordIsFertile(this Hero hero)
        {
            if (!hero.IsFemale)
            {
                return true;
            }
            else if (hero.IsPregnant)
            {
                return false;
            }

            int today = (int)CampaignTime.Now.GetDayOfSeason;
            int nextToday = today + CampaignTime.DaysInSeason;
            int startPeriod = hero.GetDramalordTraits().PeriodDayOfSeason;
            int endPeriod = startPeriod + DramalordMCM.Get.PeriodDuration;

            bool inPeriod = (today >= startPeriod && today <= endPeriod) || (nextToday >= startPeriod && nextToday <= endPeriod);

            if (hero.IsFemale && hero.Age <= DramalordMCM.Get.MaxFertilityAge && !inPeriod)
            {
                return true;
            }
            return false;
        }

        public static int GetDramalordTraitScore(this Hero hero, Hero target)
        {
            CharacterTraits heroTraits = hero.GetHeroTraits();
            CharacterTraits targetTraits = target.GetHeroTraits();

            DramalordTraits heroDramaTraits = hero.GetDramalordTraits();
            DramalordTraits targetDramaTraits = target.GetDramalordTraits();

            if (heroTraits == null || targetTraits == null)
            {
                return 0;
            }

            int score = 0;

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
            score += ((heroDramaTraits.Openness > 0 && targetDramaTraits.Openness > 0) || (heroDramaTraits.Openness < 0 && targetDramaTraits.Openness < 0)) ? 1 : 0;
            score += ((heroDramaTraits.Openness < 0 && targetDramaTraits.Openness > 0) || (heroDramaTraits.Openness > 0 && targetDramaTraits.Openness < 0)) ? -1 : 0;
            score += ((heroDramaTraits.Agreeableness > 0 && targetDramaTraits.Agreeableness > 0) || (heroDramaTraits.Agreeableness < 0 && targetDramaTraits.Agreeableness < 0)) ? 1 : 0;
            score += ((heroDramaTraits.Agreeableness < 0 && targetDramaTraits.Agreeableness > 0) || (heroDramaTraits.Agreeableness > 0 && targetDramaTraits.Agreeableness < 0)) ? -1 : 0;
            score += ((heroDramaTraits.Conscientiousness > 0 && targetDramaTraits.Conscientiousness > 0) || (heroDramaTraits.Conscientiousness < 0 && targetDramaTraits.Conscientiousness < 0)) ? 1 : 0;
            score += ((heroDramaTraits.Conscientiousness < 0 && targetDramaTraits.Conscientiousness > 0) || (heroDramaTraits.Conscientiousness > 0 && targetDramaTraits.Conscientiousness < 0)) ? -1 : 0;
            score += ((heroDramaTraits.Neuroticism > 0 && targetDramaTraits.Neuroticism > 0) || (heroDramaTraits.Neuroticism < 0 && targetDramaTraits.Neuroticism < 0)) ? 1 : 0;
            score += ((heroDramaTraits.Neuroticism < 0 && targetDramaTraits.Neuroticism > 0) || (heroDramaTraits.Neuroticism > 0 && targetDramaTraits.Neuroticism < 0)) ? -1 : 0;
            score += ((heroDramaTraits.Extroversion > 0 && targetDramaTraits.Extroversion > 0) || (heroDramaTraits.Extroversion < 0 && targetDramaTraits.Extroversion < 0)) ? 1 : 0;
            score += ((heroDramaTraits.Extroversion < 0 && targetDramaTraits.Extroversion > 0) || (heroDramaTraits.Extroversion > 0 && targetDramaTraits.Extroversion < 0)) ? -1 : 0;
            score += (target == Hero.MainHero) ? target.GetSkillValue(DefaultSkills.Charm) / 20 : 0;

            return score * DramalordMCM.Get.TraitScoreMultiplyer;
        }

        public static bool IsDramalordRelativeTo(this Hero hero, Hero target)
        {
            if (hero.Children.Contains(target) || target.Children.Contains(hero) || hero.Siblings.Contains(target) || target.Siblings.Contains(hero))
            {
                return true;
            }
            if ((hero.Father != null && hero.Father.Siblings.Contains(target)) || (hero.Mother != null && hero.Mother.Siblings.Contains(target)))
            {
                return true;
            }
            if ((target.Father != null && target.Father.Siblings.Contains(hero)) || (target.Mother != null && target.Mother.Siblings.Contains(hero)))
            {
                return true;
            }
            foreach (Hero child in hero.Children)
            {
                if (child.Children.Contains(target))
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

        public static bool IsNearby(this Hero hero, Hero target)
        {
            return target.IsDramalordLegit() && (hero.CurrentSettlement != null && hero.CurrentSettlement == target.CurrentSettlement) ||
                (hero.PartyBelongedTo != null && hero.PartyBelongedTo == target.PartyBelongedTo) ||
                (hero.PartyBelongedTo != null && target.PartyBelongedTo != null && hero.PartyBelongedTo.Army != null && hero.PartyBelongedTo.Army == target.PartyBelongedTo.Army);
        }

        public static List<Hero> GetCloseHeroes(this Hero hero)
        {
            List<Hero> list = new();
            if (hero.CurrentSettlement != null && hero.CurrentSettlement.Town != null)
            {
                hero.CurrentSettlement.HeroesWithoutParty.ForEach(item =>
                {
                    if (item != hero && !item.IsPrisoner && item.IsDramalordLegit()) list.Add(item);
                });

                hero.CurrentSettlement.Parties.ForEach(item =>
                {
                    if(item.LeaderHero != null && item.LeaderHero != hero && !item.LeaderHero.IsPrisoner && item.LeaderHero.IsDramalordLegit()) list.Add(item.LeaderHero);
                });
            }
            if(hero.PartyBelongedTo != null)
            {
                if(hero.PartyBelongedTo.Army != null && Dramalord.DramalordMCM.Get.AllowArmyInteractionAI)
                {
                    hero.PartyBelongedTo.Army.Parties.ForEach(item =>
                    {
                        if (item.LeaderHero != null && item.LeaderHero != hero && !item.LeaderHero.IsPrisoner && item.LeaderHero.IsDramalordLegit()) list.Add(item.LeaderHero);
                    });
                }
                if(hero.PartyBelongedTo.MemberRoster != null && hero.PartyBelongedTo.MemberRoster.TotalHeroes > 1)
                {
                    FlattenedTroopRoster flat = hero.PartyBelongedTo.MemberRoster.ToFlattenedRoster();
                    flat.Troops.Where(item => item.HeroObject != null && item.HeroObject != hero && !item.HeroObject.IsPrisoner && item.HeroObject.IsDramalordLegit()).Do(item => list.Add(item.HeroObject));
                }
            }
            return list.Distinct().ToList();
        }

        public static List<Hero> GetClosePrisoners(this Hero hero)
        {
            List<Hero> list = new();
            if(hero.CurrentSettlement != null && hero.CurrentSettlement.Town != null)
            {
                Town town = hero.CurrentSettlement.Town;
                town.GetPrisonerHeroes().ForEach(item => list.Add(item.HeroObject));
            }
            if(hero.PartyBelongedTo != null && hero.PartyBelongedTo.PrisonRoster != null && hero.PartyBelongedTo.PrisonRoster.TotalHeroes > 0)
            {
                hero.PartyBelongedTo.PrisonRoster.GetTroopRoster().ForEach(item =>
                {
                    if (item.Character.IsHero)
                    {
                        list.Add(item.Character.HeroObject);
                    }
                });
            }
            return list.Distinct().ToList();
        }
    }
}
