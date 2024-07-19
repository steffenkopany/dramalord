using Dramalord.Data;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;

namespace Dramalord.Extensions
{
    internal static class HeroExtensions
    {
        public static bool IsDramalordLegit(this Hero hero)
        {
            return hero != null && !hero.IsChild && (hero.IsLord || hero.IsPlayerCompanion || hero.IsWanderer) && hero.IsAlive && !hero.IsDisabled;
        }

        public static List<HeroIntention> GetIntentions(this Hero hero)
        {
            return DramalordIntentions.Instance.GetIntentions(hero);
        }

        public static HeroDesires GetDesires(this Hero hero)
        {
            return DramalordDesires.Instance.GetDesires(hero);
        }

        public static HeroPersonality GetPersonality(this Hero hero)
        {
            return DramalordPersonalities.Instance.GetPersonality(hero);
        }

        public static HeroRelation GetRelationTo(this Hero hero, Hero target)
        {
            return DramalordRelations.Instance.GetRelation(hero, target);
        }

        public static IEnumerable<KeyValuePair<HeroTuple, HeroRelation>> GetAllRelations(this Hero hero)
        {
            return DramalordRelations.Instance.GetAllRelations(hero);
        }

        public static HeroPregnancy? GetPregnancy(this Hero hero)
        {
            return DramalordPregancies.Instance.GetPregnancy(hero);
        }

        public static bool IsSpouseOf(this Hero hero, Hero target)
        {
            return hero.Spouse == target || DramalordRelations.Instance.GetRelation(hero, target).Relationship == RelationshipType.Spouse;
        }

        public static bool IsBetrothedOf(this Hero hero, Hero target)
        {
            return DramalordRelations.Instance.GetRelation(hero, target).Relationship == RelationshipType.Betrothed;
        }

        public static bool IsLoverOf(this Hero hero, Hero target)
        {
            return DramalordRelations.Instance.GetRelation(hero, target).Relationship == RelationshipType.Lover;
        }

        public static bool IsFriendWithBenefitsOf(this Hero hero, Hero target)
        {
            return DramalordRelations.Instance.GetRelation(hero, target).Relationship == RelationshipType.FriendWithBenefits;
        }

        public static bool IsFriendOf(this Hero hero, Hero target)
        {
            return DramalordRelations.Instance.GetRelation(hero, target).Relationship == RelationshipType.Friend;
        }

        public static bool IsEmotionalWith(this Hero hero, Hero target)
        {
            HeroRelation relation = DramalordRelations.Instance.GetRelation(hero, target);
            return hero.Spouse == target || relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Spouse;
        }

        public static bool IsSexualWith(this Hero hero, Hero target)
        {
            HeroRelation relation = DramalordRelations.Instance.GetRelation(hero, target);
            return hero.Spouse == target || relation.Relationship == RelationshipType.FriendWithBenefits || relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Spouse;
        }

        public static bool IsFriendlyWith(this Hero hero, Hero target)
        {
            HeroRelation relation = DramalordRelations.Instance.GetRelation(hero, target);
            return relation.Relationship == RelationshipType.Friend || relation.Relationship == RelationshipType.FriendWithBenefits;
        }

        public static bool HasAnyRelationshipWith(this Hero hero, Hero target)
        {
            return hero.Spouse == target || DramalordRelations.Instance.GetRelation(hero, target).Relationship != RelationshipType.None;
        }

        public static bool IsAutonom(this Hero hero)
        {
            if((hero.Occupation == Occupation.Wanderer && hero.Clan == null && !DramalordMCM.Instance.AllowWandererAutonomy) ||
                (hero.Clan == Clan.PlayerClan && !DramalordMCM.Instance.AllowPlayerClanAutonomy) ||
                hero.IsPrisoner)
            {
                return false;
            }
            return true;
        }

        public static bool IsRelativeOf(this Hero hero, Hero target)
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

        public static bool IsFertile(this Hero hero)
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
            int startPeriod = hero.GetDesires().PeriodDayOfSeason;
            int endPeriod = startPeriod + 5;

            bool inPeriod = (today >= startPeriod && today <= endPeriod) || (nextToday >= startPeriod && nextToday <= endPeriod);

            if (hero.IsFemale && hero.Age <= 45 && !inPeriod)
            {
                return true;
            }
            return false;
        }

        public static int GetSympathyTo(this Hero hero, Hero target)
        {
            HeroPersonality heroPersonality = hero.GetPersonality();
            HeroPersonality targetPersonality = target.GetPersonality();

            int sympathy = 10;
            if (target == Hero.MainHero || hero == Hero.MainHero) sympathy += DramalordMCM.Instance.PlayerBaseSympathy;
            sympathy -= Math.Abs(heroPersonality.Openness - targetPersonality.Openness) / 20;
            sympathy -= Math.Abs(heroPersonality.Agreeableness - targetPersonality.Agreeableness) / 20;
            sympathy -= Math.Abs(heroPersonality.Conscientiousness - targetPersonality.Conscientiousness) / 20;
            sympathy -= Math.Abs(heroPersonality.Neuroticism - targetPersonality.Neuroticism) / 20;
            sympathy -= Math.Abs(heroPersonality.Extroversion - targetPersonality.Extroversion) / 20;

            //bonus
            sympathy += hero.GetHeroTraits().Honor == target.GetHeroTraits().Honor ? 1 : 0;
            sympathy += hero.GetHeroTraits().Valor == target.GetHeroTraits().Valor ? 1 : 0;
            sympathy += hero.GetHeroTraits().Calculating == target.GetHeroTraits().Calculating ? 1 : 0;
            sympathy += hero.GetHeroTraits().Mercy == target.GetHeroTraits().Mercy ? 1 : 0;
            return sympathy;
                /*
            int score = 0;
            score += ((heroPersonality.Openness > 0 && targetPersonality.Openness > 0) || (heroPersonality.Openness < 0 && targetPersonality.Openness < 0)) ? 1 : 0;
            score += ((heroPersonality.Openness < 0 && targetPersonality.Openness > 0) || (heroPersonality.Openness > 0 && targetPersonality.Openness < 0)) ? -1 : 0;
            score += ((heroPersonality.Agreeableness > 0 && targetPersonality.Agreeableness > 0) || (heroPersonality.Agreeableness < 0 && targetPersonality.Agreeableness < 0)) ? 1 : 0;
            score += ((heroPersonality.Agreeableness < 0 && targetPersonality.Agreeableness > 0) || (heroPersonality.Agreeableness > 0 && targetPersonality.Agreeableness < 0)) ? -1 : 0;
            score += ((heroPersonality.Conscientiousness > 0 && targetPersonality.Conscientiousness > 0) || (heroPersonality.Conscientiousness < 0 && targetPersonality.Conscientiousness < 0)) ? 1 : 0;
            score += ((heroPersonality.Conscientiousness < 0 && targetPersonality.Conscientiousness > 0) || (heroPersonality.Conscientiousness > 0 && targetPersonality.Conscientiousness < 0)) ? -1 : 0;
            score += ((heroPersonality.Neuroticism > 0 && targetPersonality.Neuroticism > 0) || (heroPersonality.Neuroticism < 0 && targetPersonality.Neuroticism < 0)) ? 1 : 0;
            score += ((heroPersonality.Neuroticism < 0 && targetPersonality.Neuroticism > 0) || (heroPersonality.Neuroticism > 0 && targetPersonality.Neuroticism < 0)) ? -1 : 0;
            score += ((heroPersonality.Extroversion > 0 && targetPersonality.Extroversion > 0) || (heroPersonality.Extroversion < 0 && targetPersonality.Extroversion < 0)) ? 1 : 0;
            score += ((heroPersonality.Extroversion < 0 && targetPersonality.Extroversion > 0) || (heroPersonality.Extroversion > 0 && targetPersonality.Extroversion < 0)) ? -1 : 0;

            return score;
                */
        }

        public static int GetAttractionTo(this Hero hero, Hero target)
        {
            HeroDesires desires = hero.GetDesires();
            int rating = (target == Hero.MainHero) ? DramalordMCM.Instance.PlayerBaseAttraction : 0;
            rating += (target.IsFemale) ? desires.AttractionWomen : desires.AttractionMen;
            rating += (hero.Culture == target.Culture) ? 10 : 0;
            rating -= (int)(Math.Abs(desires.AttractionWeight - (int)(target.BodyProperties.Weight))/3);
            rating -= (int)(Math.Abs(desires.AttractionBuild - (int)(target.BodyProperties.Build))/3);
            rating -= (int)(Math.Abs((MBMath.ClampInt(desires.AttractionAgeDiff + (int)hero.Age, 18, 130) - (int)target.Age)) /2);
            rating += hero.GetRelationTo(target).Love / 10;
            rating += desires.Horny / 10;

            return MBMath.ClampInt(rating, 0, 100);
        }

        public static bool IsCloseTo(this Hero hero, Hero target)
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
                    if (item.LeaderHero != null && item.LeaderHero != hero && !item.LeaderHero.IsPrisoner && item.LeaderHero.IsDramalordLegit()) list.Add(item.LeaderHero);
                });
            }
            if (hero.PartyBelongedTo != null)
            {
                if (hero.PartyBelongedTo.Army != null)
                {
                    hero.PartyBelongedTo.Army?.Parties.ForEach(item =>
                    {
                        if (item.LeaderHero != null && item.LeaderHero != hero && !item.LeaderHero.IsPrisoner && item.LeaderHero.IsDramalordLegit()) list.Add(item.LeaderHero);
                    });
                }
                if (hero.PartyBelongedTo.MemberRoster != null && hero.PartyBelongedTo.MemberRoster.TotalHeroes > 1)
                {
                    FlattenedTroopRoster flat = hero.PartyBelongedTo.MemberRoster.ToFlattenedRoster();
                    flat.Troops.Where(item => item.HeroObject != null && item.HeroObject != hero && !item.HeroObject.IsPrisoner && item.HeroObject.IsDramalordLegit()).Do(item => list.Add(item.HeroObject));
                }
            }
            return list.Distinct().ToList();
        }
    }
}
