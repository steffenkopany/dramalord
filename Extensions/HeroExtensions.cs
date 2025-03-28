using Dramalord.Data;
using Dramalord.Data.Intentions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Dramalord.Extensions
{
    internal static class HeroExtensions
    {
        private static Hero? CachedHero = null;
        private static HeroPersonality? CachedPersonality = null;
        private static HeroDesires? CachedDesires = null;
        private static Dictionary<Hero, HeroRelation>? CachedRelation = null;

        private static void UpdateCachedData(Hero hero)
        {
            if(CachedHero != hero)
            {
                CachedHero = hero;
                CachedPersonality = DramalordPersonalities.Instance.GetPersonality(hero);
                CachedDesires = DramalordDesires.Instance.GetDesires(hero);
                CachedRelation = DramalordRelations.Instance.GetAllRelations(hero);
            }
        }

        public static int GetTrust(this Hero hero, Hero other) => CharacterRelationManager.GetHeroRelation(hero, other);

        public static void SetTrust(this Hero hero, Hero other, int value) => CharacterRelationManager.SetHeroRelation(hero, other, (int)Mathf.Min(100,Mathf.Max(value, -100)));

        public static bool IsDramalordLegit(this Hero hero)
        {
            return hero != null && !hero.IsChild && (hero.IsLord || hero.IsPlayerCompanion || hero.IsWanderer || hero.IsNotable) && hero.IsAlive && !hero.IsDisabled;
        }

        public static HeroDesires GetDesires(this Hero hero)
        {
            UpdateCachedData(hero);
            return CachedDesires;
        }

        public static HeroPersonality GetPersonality(this Hero hero)
        {
            UpdateCachedData(hero);
            return CachedPersonality;
        }

        public static HeroRelation GetRelationTo(this Hero hero, Hero target)
        {
            UpdateCachedData(hero);
            if(CachedRelation.ContainsKey(target))
            {
                return CachedRelation[target];
            }
            else
            {
                return DramalordRelations.Instance.GetRelation(hero, target);
            }
        }

        public static Dictionary<Hero, HeroRelation> GetAllRelations(this Hero hero)
        {
            UpdateCachedData(hero);
            return CachedRelation;
        }

        public static RelationshipRule GetDefaultRelationshipRule(this Hero hero)
        {
            UpdateCachedData(hero);
            if(CachedPersonality.Openness > 40 && CachedPersonality.Agreeableness > 40 && CachedPersonality.Neuroticism < -25 && CachedPersonality.Conscientiousness < -25)
            {
                return RelationshipRule.Open;
            }
            else if (CachedPersonality.Openness > 25 && CachedPersonality.Agreeableness > 25 && CachedPersonality.Neuroticism < 0 && CachedPersonality.Conscientiousness < 0)
            {
                return RelationshipRule.Poly;
            }
            else if (CachedPersonality.Openness > 10 && CachedPersonality.Conscientiousness < 0)
            {
                return RelationshipRule.Playful;
            }

            return RelationshipRule.Faithful;
        }

        public static HeroPregnancy? GetPregnancy(this Hero hero)
        {
            return DramalordPregnancies.Instance.GetPregnancy(hero);
        }

        public static bool IsSpouseOf(this Hero hero, Hero target)
        {
            // Always check the vanilla spouse property first
            if (hero.Spouse == target)
            {
                return true;
            }

            // If OtherMarriageMod is enabled, don't ignore Dramalord data completely—use it as a fallback
            if (!BetrothIntention.OtherMarriageModFound)
            {
                return GetRelationTo(hero, target).Relationship == RelationshipType.Spouse;
            }

            return false;
        }

        public static bool IsBetrothedOf(this Hero hero, Hero target)
        {
            return (GetRelationTo(hero, target).Relationship == RelationshipType.Betrothed && !BetrothIntention.OtherMarriageModFound);
        }

        public static bool IsLoverOf(this Hero hero, Hero target)
        {
            return GetRelationTo(hero, target).Relationship == RelationshipType.Lover;
        }

        public static bool IsFriendWithBenefitsOf(this Hero hero, Hero target)
        {
            return GetRelationTo(hero, target).Relationship == RelationshipType.FriendWithBenefits;
        }

        public static bool IsFriendOf(this Hero hero, Hero target)
        {
            return GetRelationTo(hero, target).Relationship == RelationshipType.Friend;
        }

        public static bool IsEmotionalWith(this Hero hero, Hero target)
        {
            HeroRelation relation = GetRelationTo(hero, target);
            return hero.Spouse == target || relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Spouse;
        }

        public static bool IsSexualWith(this Hero hero, Hero target)
        {
            HeroRelation relation = GetRelationTo(hero, target);
            return hero.Spouse == target || relation.Relationship == RelationshipType.FriendWithBenefits || relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Spouse;
        }

        public static bool IsFriendlyWith(this Hero hero, Hero target)
        {
            HeroRelation relation = GetRelationTo(hero, target);
            return relation.Relationship == RelationshipType.Friend || relation.Relationship == RelationshipType.FriendWithBenefits;
        }

        public static bool HasAnyRelationshipWith(this Hero hero, Hero target)
        {
            return hero.Spouse == target || GetRelationTo(hero, target).Relationship != RelationshipType.None;
        }

        public static bool IsAutonom(this Hero hero)
        {
            if((hero.Occupation == Occupation.Wanderer && hero.Clan == null && !DramalordMCM.Instance.AllowWandererAutonomy) ||
                (hero.IsNotable && hero.Clan == null && !DramalordMCM.Instance.AllowNotablesAutonomy) ||
                (hero != Hero.MainHero && hero.Clan == Clan.PlayerClan && !DramalordMCM.Instance.AllowPlayerClanAutonomy) ||
                hero.IsPrisoner)
            {
                return false;
            }
            return true;
        }

        // Determines if a hero is married to the player by checking both vanilla and internal systems.
        public static bool IsPlayerSpouse(this Hero hero)
        {
            return hero.Spouse == Hero.MainHero; // not necessary to request Dramalord data!
        }

        // Determines whether a romance attempt is allowed between two heroes.
        public static bool CanPursueRomanceWith(this Hero initiator, Hero target)
        {
            // Always allow romance if one party is the player.
            if (initiator == Hero.MainHero || target == Hero.MainHero)
            {
                return true;
            }

            // If the Player Spouse Faithful setting is enabled,
            // block romance if either party is married to the player.
            if (DramalordMCM.Instance.PlayerSpouseFaithful && (initiator.IsPlayerSpouse() || target.IsPlayerSpouse()))
            {
                return false;
            }

            // Block romance if either hero has a toy.
            if (initiator.GetDesires().HasToy || target.GetDesires().HasToy)
            {
                return false;
            }

            // Otherwise, romance is allowed.
            return true;
        }

        public static bool IsRelativeOf(this Hero hero, Hero target)
        {
            if(DramalordMCM.Instance.AllowIncest)
            {
                return false;
            }
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

            if (hero.IsFemale && hero.Age <= DramalordMCM.Instance.MaxFertilityAge && !inPeriod)
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
            sympathy -= Math.Abs(heroPersonality.Openness - targetPersonality.Openness) / 25;
            sympathy -= Math.Abs(heroPersonality.Agreeableness - targetPersonality.Agreeableness) / 25;
            sympathy -= Math.Abs(heroPersonality.Conscientiousness - targetPersonality.Conscientiousness) / 25;
            sympathy -= Math.Abs(heroPersonality.Neuroticism - targetPersonality.Neuroticism) / 25;
            sympathy -= Math.Abs(heroPersonality.Extroversion - targetPersonality.Extroversion) / 25;

            //bonus
            sympathy += hero.GetHeroTraits().Honor == target.GetHeroTraits().Honor ? 1 : 0; 
            sympathy += hero.GetHeroTraits().Valor == target.GetHeroTraits().Valor ? 1 : 0;
            sympathy += hero.GetHeroTraits().Calculating == target.GetHeroTraits().Calculating ? 1 : 0;
            sympathy += hero.GetHeroTraits().Mercy == target.GetHeroTraits().Mercy ? 1 : 0;
            sympathy += hero.GetHeroTraits().Generosity == target.GetHeroTraits().Generosity ? 1 : 0;
            return sympathy;
        }

        public static int GetAttractionTo(this Hero hero, Hero target)
        {
            HeroDesires desires = hero.GetDesires();
            int rating = (target == Hero.MainHero) ? DramalordMCM.Instance.PlayerBaseAttraction : 0;
            rating += (target.IsFemale) ? desires.AttractionWomen : desires.AttractionMen;
            rating += (hero.Culture == target.Culture) ? 10 : 0;
            rating -= (int)(Math.Abs(desires.AttractionWeight - (int)(target.Weight*100))/2);
            rating -= (int)(Math.Abs(desires.AttractionBuild - (int)(target.Build*100))/2);
            rating -= (int)(Math.Abs((MBMath.ClampInt(desires.AttractionAgeDiff + (int)hero.Age, 18, 130) - (int)target.Age)) /2);
            rating += hero.GetRelationTo(target).Love / 10;
            rating += desires.Horny / 10;

            return MBMath.ClampInt(rating, 0, 100);
        }

        public static bool HasMutualAttractionWith(this Hero hero, Hero target) => hero.GetAttractionTo(target) >= DramalordMCM.Instance.MinAttraction && target.GetAttractionTo(hero) >= DramalordMCM.Instance.MinAttraction;

        public static bool HasMetRecently(this Hero hero, Hero target) => hero.GetRelationTo(target).LastInteraction.ElapsedDaysUntilNow < DramalordMCM.Instance.DaysBetweenInteractions;

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
                    if(item == MobileParty.MainParty && !Hero.MainHero.IsPrisoner && item.MemberRoster.TotalHeroes > 1)
                    {
                        item.MemberRoster.GetTroopRoster().Where(troop => troop.Character.HeroObject != null && troop.Character.HeroObject != hero).Do(troop => list.Add(troop.Character.HeroObject));
                    }
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

        public static List<Hero> GetClosePrisoners(this Hero hero)
        {
            List<Hero> list = new();
            if (hero.CurrentSettlement != null && hero.CurrentSettlement.Town != null)
            {
                Town town = hero.CurrentSettlement.Town;
                town.GetPrisonerHeroes().Where(h => h.HeroObject.IsDramalordLegit()).Select(h => h.HeroObject).Do(h => list.Add(h));

                hero.CurrentSettlement.Parties.ForEach(party =>
                {
                    if(party.PrisonRoster.TotalHeroes > 0)
                    {
                        party.PrisonRoster.GetTroopRoster().Where(h => h.Character.HeroObject != null && h.Character.HeroObject.IsDramalordLegit()).Select(h => h.Character.HeroObject).Do(h => list.Add(h));
                    }
                });
            }
            if (hero.PartyBelongedTo != null)
            {
                if (hero.PartyBelongedTo.Army != null)
                {
                    hero.PartyBelongedTo.Army?.Parties.ForEach(party =>
                    {
                        if (party.PrisonRoster.TotalHeroes > 0)
                        {
                            party.PrisonRoster.GetTroopRoster().Where(h => h.Character.HeroObject != null && h.Character.HeroObject.IsDramalordLegit()).Select(h => h.Character.HeroObject).Do(h => list.Add(h));
                        }
                    });
                }
                if (hero.PartyBelongedTo.PrisonRoster != null && hero.PartyBelongedTo.PrisonRoster.TotalHeroes > 0)
                {
                    hero.PartyBelongedTo.PrisonRoster.GetTroopRoster().Where(h => h.Character.HeroObject != null && h.Character.HeroObject.IsDramalordLegit()).Select(h => h.Character.HeroObject).Do(h => list.Add(h));
                }
            }
            return list.Distinct().ToList();
        }
    }
}
