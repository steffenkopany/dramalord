using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Dramalord.Quests;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Behaviours
{
    internal class NpcCampaignBehavior : CampaignBehaviorBase
    {
        internal NpcCampaignBehavior(CampaignGameStarter starter)
        {
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(OnDailyHeroTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            // Nothing to do.
        }

        public void OnDailyHeroTick(Hero hero)
        {
            // Only process valid non-player autonomous heroes.
            if (!hero.IsDramalordLegit() || hero == Hero.MainHero || !hero.IsAutonom())
                return;

            // Cache common hero data once.
            HeroPersonality personality = hero.GetPersonality();
            HeroDesires desires = hero.GetDesires();
            List<Hero> closeHeroes = hero.GetCloseHeroes();
            int randomNumber = MBRandom.RandomInt(1, 100);
            bool playerClose = closeHeroes.Contains(Hero.MainHero)
                               && hero.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow >= DramalordMCM.Instance.DaysBetweenInteractions;

            // Pre-calculate personality flags.
            bool isHorny = desires.Horny >= WeightedRandom(100, 75, 50, 100);
            bool isOpen = personality.Openness >= WeightedRandom(100, 50, 0, 100);
            bool isConcient = personality.Conscientiousness >= WeightedRandom(100, 75, 0, 100);
            bool isExtrovert = personality.Extroversion >= WeightedRandom(100, 75, 0, 100);
            bool isAgreeable = personality.Agreeableness >= WeightedRandom(100, 75, 0, 100);
            bool isNeurotic = personality.Neuroticism >= WeightedRandom(100, 75, 0, 100);

            // Only extroverts process interactions.
            if (!isExtrovert)
                return;

            // Try each interaction in order until one succeeds.
            if (isHorny)
            {
                if (TryNormalIntercourse(hero, closeHeroes, playerClose, randomNumber))
                    return;

                if (desires.Horny == 100 && !isConcient && !isNeurotic && isOpen)
                {
                    if (TryFriendIntercourse(hero, closeHeroes, playerClose, randomNumber))
                        return;
                }

                if (hero.GetHeroTraits().Honor <= 0)
                {
                    if (TryPrisonerIntercourse(hero))
                        return;
                }

                // If hero is lover of player but not close, spawn a quest.
                if (!playerClose && hero.IsLoverOf(Hero.MainHero) && randomNumber <= DramalordMCM.Instance.QuestChance && DramalordQuests.Instance.GetQuest(hero) == null)
                {
                    ShowQuestInquiry(hero);
                    return;
                }
            }

            if (TryBetrothMarriageDate(hero, closeHeroes, playerClose))
                return;

            if (TryFlirtOrDate(hero, closeHeroes, playerClose))
                return;

            if (TryTalk(hero, closeHeroes, playerClose))
                return;

            // Second quest trigger if conditions are met.
            if (isHorny
                && hero.CurrentSettlement != Hero.MainHero.CurrentSettlement
                && hero.PartyBelongedTo != MobileParty.MainParty
                && hero.IsLoverOf(Hero.MainHero)
                && randomNumber <= DramalordMCM.Instance.QuestChance
                && DramalordQuests.Instance.GetQuest(hero) == null)
            {
                ShowQuestInquiry(hero);
                return;
            }

            // Fallback: use toy or add libido.
            if (desires.HasToy)
            {
                new UseToyIntention(hero, CampaignTime.Now).Action();
            }
            else
            {
                desires.Horny += desires.Libido;
            }
        }

        #region Interaction Helper Methods

        /// <summary>
        /// Attempts normal intercourse with a candidate from the pool.
        /// </summary>
        private bool TryNormalIntercourse(Hero hero, List<Hero> closeHeroes, bool playerClose, int randomNumber)
        {
            Hero? target = GetCandidate(hero, closeHeroes, playerClose, randomNumber,
                DramalordMCM.Instance.ChanceApproachingPlayer,
                h => h.IsAutonom()
                     && hero.IsSexualWith(h)
                     && !hero.HasMetRecently(h)
                     && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                     && hero.CanPursueRomanceWith(h));

            if (target != null && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                return true;
            return false;
        }

        /// <summary>
        /// Attempts friend intercourse (a special case) with a candidate.
        /// </summary>
        private bool TryFriendIntercourse(Hero hero, List<Hero> closeHeroes, bool playerClose, int randomNumber)
        {
            Hero? target = GetCandidate(hero, closeHeroes, playerClose, randomNumber,
                DramalordMCM.Instance.ChanceApproachingPlayer,
                h => h.IsAutonom()
                     && hero.IsFriendOf(h)
                     && !hero.HasMetRecently(h)
                     && hero.HasMutualAttractionWith(h)
                     && !hero.IsRelativeOf(h)
                     && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                     && hero.CanPursueRomanceWith(h));

            if (target != null && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                return true;
            return false;
        }

        /// <summary>
        /// Attempts prisoner intercourse if hero has low honor.
        /// </summary>
        private bool TryPrisonerIntercourse(Hero hero)
        {
            Hero? target = hero.GetClosePrisoners().GetRandomElementWithPredicate(h =>
                hero.GetAttractionTo(h) >= DramalordMCM.Instance.MinAttraction
                && !hero.HasMetRecently(h)
                && (!hero.IsPlayerSpouse() || !DramalordMCM.Instance.PlayerSpouseFaithful));

            if (target != null && new PrisonIntercourseIntention(target, hero, CampaignTime.Now).Action())
                return true;
            return false;
        }

        /// <summary>
        /// Attempts betrothal, marriage, or a dating interaction.
        /// </summary>
        private bool TryBetrothMarriageDate(Hero hero, List<Hero> closeHeroes, bool playerClose)
        {
            Hero? target = GetCandidate(hero, closeHeroes, playerClose, MBRandom.RandomInt(1, 100),
                DramalordMCM.Instance.ChanceApproachingPlayer,
                h => h.IsAutonom()
                     && hero.IsEmotionalWith(h)
                     && !hero.HasMetRecently(h)
                     && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                     && (h.IsFemale != hero.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage)
                     && hero.CanPursueRomanceWith(h));

            if (target == null)
                return false;

            HeroRelation targetRelation = hero.GetRelationTo(target);

            // Immediate marriage if conditions met.
            if (!BetrothIntention.OtherMarriageModFound
                && targetRelation.Relationship == RelationshipType.Betrothed
                && targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove
                && DramalordQuests.Instance.GetQuest(hero) == null
                && new MarriageIntention(target, hero, CampaignTime.Now).Action())
            {
                return true;
            }
            // Become betrothed.
            else if (hero.Spouse == null
                && !BetrothIntention.OtherMarriageModFound
                && targetRelation.Relationship == RelationshipType.Lover
                && targetRelation.LastInteraction.RemainingDaysFromNow < -3
                && targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove
                && new BetrothIntention(target, hero, CampaignTime.Now).Action())
            {
                return true;
            }
            // Or go on a date.
            else if (new DateIntention(target, hero, CampaignTime.Now).Action())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts a flirt or dating interaction.
        /// </summary>
        private bool TryFlirtOrDate(Hero hero, List<Hero> closeHeroes, bool playerClose)
        {
            Hero? target = GetCandidate(hero, closeHeroes, playerClose, MBRandom.RandomInt(1, 100),
                DramalordMCM.Instance.ChanceApproachingPlayer,
                h => h.IsAutonom()
                     && hero.HasMutualAttractionWith(h)
                     && !hero.IsRelativeOf(h)
                     && !hero.HasMetRecently(h)
                     && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                     && hero.CanPursueRomanceWith(h));
            if (target == null)
                return false;

            HeroRelation targetRelation = hero.GetRelationTo(target);
            // If love is high, try a date.
            if (targetRelation.Love >= DramalordMCM.Instance.MinDatingLove
                && new DateIntention(target, hero, CampaignTime.Now).Action())
            {
                return true;
            }
            else if (new FlirtIntention(target, hero, CampaignTime.Now).Action())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts a talk interaction.
        /// </summary>
        private bool TryTalk(Hero hero, List<Hero> closeHeroes, bool playerClose)
        {
            Hero? target = GetCandidate(hero, closeHeroes, playerClose, MBRandom.RandomInt(1, 100),
                DramalordMCM.Instance.ChanceApproachingPlayer,
                h => h.IsAutonom() && !hero.HasMetRecently(h));
            if (target != null && new TalkIntention(target, hero, CampaignTime.Now).Action())
                return true;
            return false;
        }

        /// <summary>
        /// Helper to show a quest inquiry when conditions are met.
        /// </summary>
        private void ShowQuestInquiry(Hero hero)
        {
            int speed = (int)Campaign.Current.TimeControlMode;
            Campaign.Current.SetTimeSpeed(0);
            TextObject title = new TextObject("{=Dramalord304}{QUESTHERO} requests your presence.");
            TextObject text = new TextObject("{=Dramalord303}{HERO.LINK} asks you to find them, as they have an urgent matter to discuss. Will you make it in time?");
            title.SetTextVariable("QUESTHERO", hero.Name);
            StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, text);

            InformationManager.ShowInquiry(
                new InquiryData(
                    title.ToString(),
                    text.ToString(),
                    true,
                    true,
                    GameTexts.FindText("str_yes").ToString(),
                    GameTexts.FindText("str_no").ToString(),
                    () =>
                    {
                        VisitLoverQuest quest = new VisitLoverQuest(hero);
                        quest.StartQuest();
                        DramalordQuests.Instance.AddQuest(hero, quest);
                        Campaign.Current.SetTimeSpeed(speed);
                    },
                    () =>
                    {
                        Campaign.Current.SetTimeSpeed(speed);
                        TextObject banner = new TextObject("{=Dramalord302}{HERO.LINK} is disappointed in you, for you have failed to fulfill their urgent request...");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, hero.CharacterObject, "event:/ui/notification/relation");
                        RelationshipLossAction.Apply(hero, Hero.MainHero, out int loveDamage, out int trustDamage, 10, 17);
                        new ChangeOpinionIntention(hero, Hero.MainHero, loveDamage, trustDamage, CampaignTime.Now).Action();
                        hero.GetDesires().Horny = 0;
                    }
                ),
                true
            );
        }

        /// <summary>
        /// Helper method to select a candidate from the list.
        /// Checks for player approach first if flagged.
        /// </summary>
        private Hero? GetCandidate(Hero hero, List<Hero> candidates, bool playerClose, int randomNumber, int chanceApproachingPlayer, Func<Hero, bool> predicate)
        {
            // If player is close and chance condition is met, return the player.
            if (playerClose && randomNumber <= chanceApproachingPlayer)
                return Hero.MainHero;

            // Otherwise, return a random candidate that satisfies the predicate.
            return candidates.GetRandomElementWithPredicate(predicate);
        }

        #endregion

        /// <summary>
        /// Returns a weighted random value between min and max.
        /// </summary>
        private int WeightedRandom(int peakValue, int normalValue, int minValue, int maxValue)
        {
            normalValue = (normalValue < peakValue) ? peakValue + (peakValue - normalValue) : normalValue;
            float rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
            int result = (int)(peakValue + normalValue * rand_std_normal);
            while (result < minValue || result > maxValue)
            {
                rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
                result = (int)(peakValue + normalValue * rand_std_normal);
            }
            return result;
        }
    }
}
