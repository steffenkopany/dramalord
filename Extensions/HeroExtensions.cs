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
        // --- Two-Hero Cache Implementation ---
        private static Hero? _cachedHeroA, _cachedHeroB;
        private static HeroPersonality? _cachedPersonalityA, _cachedPersonalityB;
        private static HeroDesires? _cachedDesiresA, _cachedDesiresB;
        private static Dictionary<Hero, HeroRelation>? _cachedRelationsA, _cachedRelationsB;

        /// <summary>
        /// Ensures hero data is cached. If hero matches one of the two cached slots,
        /// do nothing; else shift slot A to slot B, then load fresh data into slot A.
        /// </summary>
        private static void UpdateCachedData(Hero hero)
        {
            // Already in cache A?
            if (hero == _cachedHeroA)
                return;

            // Or in cache B?
            if (hero == _cachedHeroB)
                return;

            // Not in either slot: shift A -> B, then fill A with new hero
            _cachedHeroB = _cachedHeroA;
            _cachedPersonalityB = _cachedPersonalityA;
            _cachedDesiresB = _cachedDesiresA;
            _cachedRelationsB = _cachedRelationsA;

            _cachedHeroA = hero;
            _cachedPersonalityA = DramalordPersonalities.Instance.GetPersonality(hero);
            _cachedDesiresA = DramalordDesires.Instance.GetDesires(hero);
            _cachedRelationsA = DramalordRelations.Instance.GetAllRelations(hero);
        }

        /// <summary>
        /// Retrieve the hero's personality from the two-hero cache.
        /// </summary>
        private static HeroPersonality? GetCachedPersonality(Hero hero)
            => (hero == _cachedHeroA) ? _cachedPersonalityA
            : (hero == _cachedHeroB) ? _cachedPersonalityB
            : null;

        /// <summary>
        /// Retrieve the hero's desires from the two-hero cache.
        /// </summary>
        private static HeroDesires? GetCachedDesires(Hero hero)
            => (hero == _cachedHeroA) ? _cachedDesiresA
            : (hero == _cachedHeroB) ? _cachedDesiresB
            : null;

        /// <summary>
        /// Retrieve the entire relationship dictionary for the given hero from the two-hero cache.
        /// </summary>
        private static Dictionary<Hero, HeroRelation>? GetCachedRelations(Hero hero)
            => (hero == _cachedHeroA) ? _cachedRelationsA
            : (hero == _cachedHeroB) ? _cachedRelationsB
            : null;

        /// <summary>
        /// Helper that fetches the single (hero->target) HeroRelation from the cached dictionary,
        /// or from DramalordRelations if not found in the dictionary.
        /// </summary>
        private static HeroRelation GetCachedRelation(Hero hero, Hero target)
        {
            // Make sure hero data is in cache
            UpdateCachedData(hero);

            // Attempt to retrieve the dictionary for this hero
            Dictionary<Hero, HeroRelation>? dict = GetCachedRelations(hero);
            if (dict != null && dict.ContainsKey(target))
            {
                return dict[target];
            }
            else
            {
                // Fallback if not found in the dictionary
                return DramalordRelations.Instance.GetRelation(hero, target);
            }
        }

        // ----------------------------------------------------------------------------
        // Public API methods below remain exactly the same signatures as before.
        // ----------------------------------------------------------------------------

        public static int GetTrust(this Hero hero, Hero other)
            => CharacterRelationManager.GetHeroRelation(hero, other);

        public static void SetTrust(this Hero hero, Hero other, int value)
            => CharacterRelationManager.SetHeroRelation(hero, other, (int)Mathf.Min(100, Mathf.Max(value, -100)));

        public static bool IsDramalordLegit(this Hero hero)
        {
            return hero != null && !hero.IsChild
                && (hero.IsLord || hero.IsPlayerCompanion || hero.IsWanderer || hero.IsNotable)
                && hero.IsAlive && !hero.IsDisabled;
        }

        public static HeroDesires GetDesires(this Hero hero)
        {
            UpdateCachedData(hero);
            return GetCachedDesires(hero)!; // We know it's not null after UpdateCachedData
        }

        public static HeroPersonality GetPersonality(this Hero hero)
        {
            UpdateCachedData(hero);
            return GetCachedPersonality(hero)!; // likewise guaranteed non-null
        }

        public static HeroRelation GetRelationTo(this Hero hero, Hero target)
        {
            // This call has been centralized to a single helper
            return GetCachedRelation(hero, target);
        }

        public static Dictionary<Hero, HeroRelation> GetAllRelations(this Hero hero)
        {
            UpdateCachedData(hero);
            return GetCachedRelations(hero)!; // guaranteed non-null
        }

        public static HeroPregnancy? GetPregnancy(this Hero hero)
        {
            return DramalordPregnancies.Instance.GetPregnancy(hero);
        }

        public static bool IsSpouseOf(this Hero hero, Hero target)
        {
            // Always check vanilla spouse property first
            if (hero.Spouse == target)
            {
                return true;
            }

            // If OtherMarriageMod is enabled, fallback
            if (BetrothIntention.OtherMarriageModFound)
            {
                return hero.GetRelationTo(target).Relationship == RelationshipType.Spouse;
            }

            return false;
        }

        public static bool IsBetrothedOf(this Hero hero, Hero target)
        {
            HeroRelation relation = hero.GetRelationTo(target);
            return (relation.Relationship == RelationshipType.Betrothed && !BetrothIntention.OtherMarriageModFound);
        }

        public static bool IsLoverOf(this Hero hero, Hero target)
        {
            return hero.GetRelationTo(target).Relationship == RelationshipType.Lover;
        }

        public static bool IsFriendWithBenefitsOf(this Hero hero, Hero target)
        {
            return hero.GetRelationTo(target).Relationship == RelationshipType.FriendWithBenefits;
        }

        public static bool IsFriendOf(this Hero hero, Hero target)
        {
            return hero.GetRelationTo(target).Relationship == RelationshipType.Friend;
        }

        public static bool IsEmotionalWith(this Hero hero, Hero target)
        {
            // Do only one dictionary lookup
            var relation = hero.GetRelationTo(target);
            return hero.Spouse == target
                || relation.Relationship == RelationshipType.Lover
                || relation.Relationship == RelationshipType.Betrothed
                || relation.Relationship == RelationshipType.Spouse;
        }

        public static bool IsSexualWith(this Hero hero, Hero target)
        {
            // Single lookup
            var relation = hero.GetRelationTo(target);
            return hero.Spouse == target
                || relation.Relationship == RelationshipType.FriendWithBenefits
                || relation.Relationship == RelationshipType.Lover
                || relation.Relationship == RelationshipType.Betrothed
                || relation.Relationship == RelationshipType.Spouse;
        }

        public static bool IsFriendlyWith(this Hero hero, Hero target)
        {
            // Single lookup
            var relation = hero.GetRelationTo(target);
            return relation.Relationship == RelationshipType.Friend
                || relation.Relationship == RelationshipType.FriendWithBenefits;
        }

        public static bool HasAnyRelationshipWith(this Hero hero, Hero target)
        {
            // Single lookup
            var relation = hero.GetRelationTo(target);
            return hero.Spouse == target || relation.Relationship != RelationshipType.None;
        }

        public static bool IsAutonom(this Hero hero)
        {
            if ((hero.Occupation == Occupation.Wanderer && hero.Clan == null && !DramalordMCM.Instance.AllowWandererAutonomy)
                || (hero.IsNotable && hero.Clan == null && !DramalordMCM.Instance.AllowNotablesAutonomy)
                || (hero != Hero.MainHero && hero.Clan == Clan.PlayerClan && !DramalordMCM.Instance.AllowPlayerClanAutonomy)
                || hero.IsPrisoner)
            {
                return false;
            }
            return true;
        }

        // Determines if a hero is married to the player by checking both vanilla and internal systems.
        public static bool IsPlayerSpouse(this Hero hero)
        {
            return hero.Spouse == Hero.MainHero; // Not necessary to request Dramalord data
        }

        // Determines whether a romance attempt is allowed between two heroes.
        public static bool CanPursueRomanceWith(this Hero initiator, Hero target)
        {
            // Always allow romance if one party is the player.
            if (initiator == Hero.MainHero || target == Hero.MainHero)
                return true;

            // If the Player Spouse Faithful setting is enabled, block if either is married to the player.
            if (DramalordMCM.Instance.PlayerSpouseFaithful
                && (initiator.IsPlayerSpouse() || target.IsPlayerSpouse()))
            {
                return false;
            }

            // Block romance if either hero has a toy.
            if (initiator.GetDesires().HasToy || target.GetDesires().HasToy)
                return false;

            // Otherwise, romance is allowed.
            return true;
        }

        public static bool IsRelativeOf(this Hero hero, Hero target)
        {
            if (DramalordMCM.Instance.AllowIncest)
                return false;

            if (hero.Children.Contains(target)
                || target.Children.Contains(hero)
                || hero.Siblings.Contains(target)
                || target.Siblings.Contains(hero))
            {
                return true;
            }
            if ((hero.Father != null && hero.Father.Siblings.Contains(target))
                || (hero.Mother != null && hero.Mother.Siblings.Contains(target)))
            {
                return true;
            }
            if ((target.Father != null && target.Father.Siblings.Contains(hero))
                || (target.Mother != null && target.Mother.Siblings.Contains(hero)))
            {
                return true;
            }

            foreach (Hero child in hero.Children)
            {
                if (child.Children.Contains(target))
                    return true;
            }
            foreach (Hero child in target.Children)
            {
                if (child.Children.Contains(hero))
                    return true;
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

            bool inPeriod = (today >= startPeriod && today <= endPeriod)
                || (nextToday >= startPeriod && nextToday <= endPeriod);

            if (hero.IsFemale && hero.Age <= DramalordMCM.Instance.MaxFertilityAge && !inPeriod)
                return true;

            return false;
        }

        public static int GetSympathyTo(this Hero hero, Hero target)
        {
            // Single calls for personality to avoid repeated lookups
            var heroPersonality = hero.GetPersonality();
            var targetPersonality = target.GetPersonality();

            int sympathy = 10;
            if (target == Hero.MainHero || hero == Hero.MainHero)
                sympathy += DramalordMCM.Instance.PlayerBaseSympathy;

            sympathy -= Math.Abs(heroPersonality.Openness - targetPersonality.Openness) / 25;
            sympathy -= Math.Abs(heroPersonality.Agreeableness - targetPersonality.Agreeableness) / 25;
            sympathy -= Math.Abs(heroPersonality.Conscientiousness - targetPersonality.Conscientiousness) / 25;
            sympathy -= Math.Abs(heroPersonality.Neuroticism - targetPersonality.Neuroticism) / 25;
            sympathy -= Math.Abs(heroPersonality.Extroversion - targetPersonality.Extroversion) / 25;

            // Trait-based bonus
            if (hero.GetHeroTraits().Honor == target.GetHeroTraits().Honor) sympathy++;
            if (hero.GetHeroTraits().Valor == target.GetHeroTraits().Valor) sympathy++;
            if (hero.GetHeroTraits().Calculating == target.GetHeroTraits().Calculating) sympathy++;
            if (hero.GetHeroTraits().Mercy == target.GetHeroTraits().Mercy) sympathy++;
            if (hero.GetHeroTraits().Generosity == target.GetHeroTraits().Generosity) sympathy++;

            return sympathy;
        }

        public static int GetAttractionTo(this Hero hero, Hero target)
        {
            // Single call to fetch hero's desires
            var desires = hero.GetDesires();
            int rating = (target == Hero.MainHero) ? DramalordMCM.Instance.PlayerBaseAttraction : 0;

            rating += target.IsFemale ? desires.AttractionWomen : desires.AttractionMen;
            rating += (hero.Culture == target.Culture) ? 10 : 0;

            rating -= (int)(Math.Abs(desires.AttractionWeight - (int)target.BodyProperties.Weight) / 3);
            rating -= (int)(Math.Abs(desires.AttractionBuild - (int)target.BodyProperties.Build) / 3);

            int idealAge = MBMath.ClampInt(desires.AttractionAgeDiff + (int)hero.Age, 18, 130);
            rating -= (int)(Math.Abs(idealAge - (int)target.Age) / 2);

            // Single relation call
            var relation = hero.GetRelationTo(target);
            rating += relation.Love / 10;
            rating += desires.Horny / 10;

            return MBMath.ClampInt(rating, 0, 100);
        }

        public static bool HasMutualAttractionWith(this Hero hero, Hero target)
        {
            return hero.GetAttractionTo(target) >= DramalordMCM.Instance.MinAttraction
                && target.GetAttractionTo(hero) >= DramalordMCM.Instance.MinAttraction;
        }

        public static bool HasMetRecently(this Hero hero, Hero target)
        {
            // Single relation call
            var relation = hero.GetRelationTo(target);
            return relation.LastInteraction.ElapsedDaysUntilNow < DramalordMCM.Instance.DaysBetweenInteractions;
        }

        public static bool IsCloseTo(this Hero hero, Hero target)
        {
            // Keep logic the same, no repeated calls needed
            return target.IsDramalordLegit()
                && (
                    (hero.CurrentSettlement != null && hero.CurrentSettlement == target.CurrentSettlement)
                    || (hero.PartyBelongedTo != null && hero.PartyBelongedTo == target.PartyBelongedTo)
                    || (hero.PartyBelongedTo != null && target.PartyBelongedTo != null
                        && hero.PartyBelongedTo.Army != null
                        && hero.PartyBelongedTo.Army == target.PartyBelongedTo.Army)
                );
        }

        public static List<Hero> GetCloseHeroes(this Hero hero)
        {
            List<Hero> list = new();
            if (hero.CurrentSettlement != null && hero.CurrentSettlement.Town != null)
            {
                hero.CurrentSettlement.HeroesWithoutParty.ForEach(item =>
                {
                    if (item != hero && !item.IsPrisoner && item.IsDramalordLegit())
                        list.Add(item);
                });

                hero.CurrentSettlement.Parties.ForEach(item =>
                {
                    if (item.LeaderHero != null && item.LeaderHero != hero
                        && !item.LeaderHero.IsPrisoner && item.LeaderHero.IsDramalordLegit())
                    {
                        list.Add(item.LeaderHero);
                    }

                    if (item == MobileParty.MainParty
                        && !Hero.MainHero.IsPrisoner && item.MemberRoster.TotalHeroes > 1)
                    {
                        item.MemberRoster.GetTroopRoster()
                            .Where(troop => troop.Character.HeroObject != null && troop.Character.HeroObject != hero)
                            .Do(troop => list.Add(troop.Character.HeroObject));
                    }
                });
            }

            if (hero.PartyBelongedTo != null)
            {
                if (hero.PartyBelongedTo.Army != null)
                {
                    hero.PartyBelongedTo.Army.Parties.ForEach(item =>
                    {
                        if (item.LeaderHero != null && item.LeaderHero != hero
                            && !item.LeaderHero.IsPrisoner && item.LeaderHero.IsDramalordLegit())
                        {
                            list.Add(item.LeaderHero);
                        }
                    });
                }

                if (hero.PartyBelongedTo.MemberRoster != null && hero.PartyBelongedTo.MemberRoster.TotalHeroes > 1)
                {
                    FlattenedTroopRoster flat = hero.PartyBelongedTo.MemberRoster.ToFlattenedRoster();
                    flat.Troops
                        .Where(item => item.HeroObject != null && item.HeroObject != hero && !item.HeroObject.IsPrisoner && item.HeroObject.IsDramalordLegit())
                        .Do(item => list.Add(item.HeroObject));
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
                town.GetPrisonerHeroes()
                    .Where(h => h.HeroObject.IsDramalordLegit())
                    .Select(h => h.HeroObject)
                    .Do(h => list.Add(h));

                hero.CurrentSettlement.Parties.ForEach(party =>
                {
                    if (party.PrisonRoster.TotalHeroes > 0)
                    {
                        party.PrisonRoster.GetTroopRoster()
                            .Where(h => h.Character.HeroObject != null && h.Character.HeroObject.IsDramalordLegit())
                            .Select(h => h.Character.HeroObject)
                            .Do(h => list.Add(h));
                    }
                });
            }

            if (hero.PartyBelongedTo != null)
            {
                if (hero.PartyBelongedTo.Army != null)
                {
                    hero.PartyBelongedTo.Army.Parties.ForEach(party =>
                    {
                        if (party.PrisonRoster.TotalHeroes > 0)
                        {
                            party.PrisonRoster.GetTroopRoster()
                                .Where(h => h.Character.HeroObject != null && h.Character.HeroObject.IsDramalordLegit())
                                .Select(h => h.Character.HeroObject)
                                .Do(h => list.Add(h));
                        }
                    });
                }

                if (hero.PartyBelongedTo.PrisonRoster != null && hero.PartyBelongedTo.PrisonRoster.TotalHeroes > 0)
                {
                    hero.PartyBelongedTo.PrisonRoster.GetTroopRoster()
                        .Where(h => h.Character.HeroObject != null && h.Character.HeroObject.IsDramalordLegit())
                        .Select(h => h.Character.HeroObject)
                        .Do(h => list.Add(h));
                }
            }
            return list.Distinct().ToList();
        }
    }
}
