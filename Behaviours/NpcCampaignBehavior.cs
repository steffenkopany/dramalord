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

        internal void OnDailyHeroTick(Hero hero)
        {
            // Only process if the hero qualifies
            if (!hero.IsDramalordLegit() || hero == Hero.MainHero || !hero.IsAutonom())
                return;

            // Cache personality and desires locally
            HeroPersonality personality = hero.GetPersonality();
            HeroDesires desires = hero.GetDesires();

            // Evaluate personality thresholds using WeightedRandom; note: variable naming corrections applied.
            bool isHorny = desires.Horny >= WeightedRandom(100, 75, 50, 100);
            bool isOpen = personality.Openness >= WeightedRandom(100, 50, 0, 100);
            bool isConscientious = personality.Conscientiousness >= WeightedRandom(100, 75, 0, 100);
            bool isExtrovert = personality.Extroversion >= WeightedRandom(100, 75, 0, 100);
            bool isAgreeable = personality.Agreeableness >= WeightedRandom(100, 75, 0, 100);
            bool isNeurotic = personality.Neuroticism >= WeightedRandom(100, 75, 0, 100);

            int randomNumber = MBRandom.RandomInt(1, 100);

            // Only extroverts consider interacting
            if (!isExtrovert)
                return;

            // Get the pool of close heroes once and cache it.
            List<Hero> closeHeroes = hero.GetCloseHeroes();

            // Determine if the player is nearby and if enough time has elapsed
            bool playerClose = closeHeroes.Contains(Hero.MainHero) &&
                hero.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow >= DramalordMCM.Instance.DaysBetweenInteractions;

            // If no candidates are present, nothing to do
            if (closeHeroes.Count == 0)
                return;

            // Process interaction branches in order. If one action fires, we return.
            if (isHorny)
            {
                // Note: Pass isConscientious, isNeurotic, and isOpen into ProcessIntercourse.
                if (ProcessIntercourse(hero, closeHeroes, playerClose, randomNumber, desires, isConscientious, isNeurotic, isOpen))
                    return;
            }

            // Process betroth/marriage/date interactions
            if (ProcessBetrothMarriageDate(hero, closeHeroes, playerClose, randomNumber))
                return;

            // Process flirt interactions
            if (ProcessFlirt(hero, closeHeroes, playerClose, randomNumber))
                return;

            // Process talk as a fallback
            if (ProcessTalk(hero, closeHeroes, playerClose, randomNumber))
                return;

            // Additional quest trigger check (second chance) when not near the player.
            if (isHorny &&
                hero.CurrentSettlement != Hero.MainHero.CurrentSettlement &&
                hero.PartyBelongedTo != MobileParty.MainParty &&
                hero.IsLoverOf(Hero.MainHero) &&
                randomNumber <= DramalordMCM.Instance.QuestChance &&
                DramalordQuests.Instance.GetQuest(hero) == null)
            {
                TriggerQuestInquiry(hero);
                return;
            }

            // If nothing else fired, use toy if available, else increment Horny by Libido.
            if (desires.HasToy)
            {
                new UseToyIntention(hero, CampaignTime.Now).Action();
            }
            else
            {
                desires.Horny += desires.Libido;
            }
        }

        #region Candidate Selection Helpers

        /// <summary>
        /// Returns either Hero.MainHero if player is close and chance is met and candidate meets predicate,
        /// or a random element from the pool that satisfies the predicate.
        /// </summary>
        private Hero? SelectCandidate(List<Hero> pool, bool playerClose, int chanceApproach, Func<Hero, bool> predicate)
        {
            if (playerClose && MBRandom.RandomInt(1, 100) <= chanceApproach && predicate(Hero.MainHero))
                return Hero.MainHero;
            return pool.GetRandomElementWithPredicate(predicate);
        }

        #endregion

        #region Interaction Processing Helpers

        /// <summary>
        /// Process intercourse-related interactions.
        /// Includes normal intercourse, friend intercourse (if conditions are met),
        /// and prisoner intercourse if hero's Honor is low.
        /// </summary>
        private bool ProcessIntercourse(Hero hero, List<Hero> pool, bool playerClose, int randomNumber, HeroDesires desires,
                                        bool isConscientious, bool isNeurotic, bool isOpen)
        {
            Hero? target = null;

            // -- Normal Intercourse --
            target = playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer &&
                     hero.IsSexualWith(Hero.MainHero)
                     ? Hero.MainHero
                     : SelectCandidate(pool, playerClose, DramalordMCM.Instance.ChanceApproachingPlayer, h =>
                        h.IsAutonom() &&
                        hero.IsSexualWith(h) &&
                        !hero.HasMetRecently(h) &&
                        (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord) &&
                        hero.CanPursueRomanceWith(h));
            if (target != null && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                return true;

            // -- Friend (Intercourse) Branch: Only if maximum Horny, and personality conditions (non-conscientious, non-neurotic, open)
            if (desires.Horny == 100 && !isConscientious && !isNeurotic && isOpen)
            {
                target = playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer &&
                         hero.IsFriendOf(Hero.MainHero) &&
                         hero.HasMutualAttractionWith(Hero.MainHero)
                         ? Hero.MainHero
                         : SelectCandidate(pool, playerClose, DramalordMCM.Instance.ChanceApproachingPlayer, h =>
                            h.IsAutonom() &&
                            hero.IsFriendOf(h) &&
                            !hero.HasMetRecently(h) &&
                            hero.HasMutualAttractionWith(h) &&
                            !hero.IsRelativeOf(h) &&
                            (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord) &&
                            hero.CanPursueRomanceWith(h));
                if (target != null && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                    return true;
            }

            // -- Prisoner Intercourse Branch: If hero's Honor is <= 0, try prisoner intercourse.
            if (hero.GetHeroTraits().Honor <= 0)
            {
                target = hero.GetClosePrisoners().GetRandomElementWithPredicate(h =>
                    hero.GetAttractionTo(h) >= DramalordMCM.Instance.MinAttraction &&
                    !hero.HasMetRecently(h) &&
                    (!hero.IsPlayerSpouse() || !DramalordMCM.Instance.PlayerSpouseFaithful));
                if (target != null && new PrisonIntercourseIntention(target, hero, CampaignTime.Now).Action())
                    return true;
            }

            // -- Quest Trigger Branch --
            // If hero is player's lover but not near the player, maybe spawn a quest inquiry.
            if (!playerClose &&
                hero.IsLoverOf(Hero.MainHero) &&
                randomNumber <= DramalordMCM.Instance.QuestChance &&
                DramalordQuests.Instance.GetQuest(hero) == null)
            {
                TriggerQuestInquiry(hero);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Process betroth, marriage, or date interactions.
        /// </summary>
        private bool ProcessBetrothMarriageDate(Hero hero, List<Hero> pool, bool playerClose, int randomNumber)
        {
            Hero? target = playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.IsEmotionalWith(Hero.MainHero)
                ? Hero.MainHero
                : SelectCandidate(pool, playerClose, DramalordMCM.Instance.ChanceApproachingPlayer, h =>
                    h.IsAutonom() &&
                    hero.IsEmotionalWith(h) &&
                    !hero.HasMetRecently(h) &&
                    (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord) &&
                    (h.IsFemale != hero.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage) &&
                    hero.CanPursueRomanceWith(h));
            if (target != null)
            {
                HeroRelation targetRelation = hero.GetRelationTo(target);

                // Immediate marriage if betrothed and love threshold is met
                if (!BetrothIntention.OtherMarriageModFound &&
                    targetRelation.Relationship == RelationshipType.Betrothed &&
                    targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove &&
                    DramalordQuests.Instance.GetQuest(hero) == null &&
                    new MarriageIntention(target, hero, CampaignTime.Now).Action())
                {
                    return true;
                }
                // Otherwise, attempt to become betrothed.
                else if (hero.Spouse == null &&
                         !BetrothIntention.OtherMarriageModFound &&
                         targetRelation.Relationship == RelationshipType.Lover &&
                         targetRelation.LastInteraction.RemainingDaysFromNow < -3 &&
                         targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove &&
                         new BetrothIntention(target, hero, CampaignTime.Now).Action())
                {
                    return true;
                }
                // Else try a dating intention.
                else if (new DateIntention(target, hero, CampaignTime.Now).Action())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Process flirt interactions (which may lead to a date or a simple flirt).
        /// </summary>
        private bool ProcessFlirt(Hero hero, List<Hero> pool, bool playerClose, int randomNumber)
        {
            Hero? target = playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.HasMutualAttractionWith(Hero.MainHero)
                ? Hero.MainHero
                : SelectCandidate(pool, playerClose, DramalordMCM.Instance.ChanceApproachingPlayer, h =>
                    h.IsAutonom() &&
                    hero.HasMutualAttractionWith(h) &&
                    !hero.IsRelativeOf(h) &&
                    !hero.HasMetRecently(h) &&
                    (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord) &&
                    hero.CanPursueRomanceWith(h));
            if (target != null)
            {
                HeroRelation targetRelation = hero.GetRelationTo(target);
                // Attempt a date if love is high enough
                if (targetRelation.Love >= DramalordMCM.Instance.MinDatingLove &&
                    new DateIntention(target, hero, CampaignTime.Now).Action())
                {
                    return true;
                }
                else if (new FlirtIntention(target, hero, CampaignTime.Now).Action())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Process talk interactions as a fallback.
        /// </summary>
        private bool ProcessTalk(Hero hero, List<Hero> pool, bool playerClose, int randomNumber)
        {
            Hero? target = playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer
                ? Hero.MainHero
                : SelectCandidate(pool, playerClose, DramalordMCM.Instance.ChanceApproachingPlayer, h =>
                    h.IsAutonom() && !hero.HasMetRecently(h));
            if (target != null && new TalkIntention(target, hero, CampaignTime.Now).Action())
                return true;
            return false;
        }

        /// <summary>
        /// Displays the quest inquiry for a hero.
        /// </summary>
        private void TriggerQuestInquiry(Hero hero)
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

        #endregion

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(OnDailyHeroTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            // nothing to do
        }

        /// <summary>
        /// Generates a weighted random integer based on a normal distribution.
        /// </summary>
        private int WeightedRandom(int peakValue, int normalValue, int minValue, int maxValue)
        {
            normalValue = (normalValue < peakValue) ? peakValue + (peakValue - normalValue) : normalValue;
            float randStdNormal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
            int result = (int)(peakValue + normalValue * randStdNormal);
            while (result < minValue || result > maxValue)
            {
                randStdNormal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
                result = (int)(peakValue + normalValue * randStdNormal);
            }
            return result;
        }
    }
}
