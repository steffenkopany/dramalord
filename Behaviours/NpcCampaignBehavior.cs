using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Dramalord.Quests;
using Helpers;
using System;
using System.Collections.Generic;
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
            if (hero.IsDramalordLegit() && hero != Hero.MainHero && hero.IsAutonom())
            {
                HeroPersonality personality = hero.GetPersonality();
                HeroDesires desires = hero.GetDesires();

                bool isHorny = desires.Horny >= WeightedRandom(100, 75, 50, 100);
                bool isOpen = personality.Openness >= WeightedRandom(100, 50, 0, 100);
                bool isConcient = personality.Conscientiousness >= WeightedRandom(100, 75, 0, 100);
                bool isExtrovert = personality.Extroversion >= WeightedRandom(100, 75, 0, 100);
                bool isAgreeable = personality.Agreeableness >= WeightedRandom(100, 75, 0, 100);
                bool isNeurotic = personality.Neuroticism >= WeightedRandom(100, 75, 0, 100);

                bool playerClose = false;
                int randomNumber = MBRandom.RandomInt(1, 100);

                if (isExtrovert)
                {
                    List<Hero> closeHeroes = hero.GetCloseHeroes();
                    playerClose = closeHeroes.Contains(Hero.MainHero)
                                  && hero.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow >= DramalordMCM.Instance.DaysBetweenInteractions;

                    if (closeHeroes.Count > 0)
                    {
                        Hero? target = null;

                        // 1) PRISONERS / HORNY-INTERCOURSE / ETC.
                        if (isHorny)
                        {
                            // Attempt normal intercourse
                            target = (playerClose
                                      && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer
                                      && hero.IsSexualWith(Hero.MainHero))
                                ? Hero.MainHero
                                : closeHeroes.GetRandomElementWithPredicate(h =>
                                    h.IsAutonom()
                                    && hero.IsSexualWith(h)
                                    && !hero.HasMetRecently(h)
                                    && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                                    && hero.CanPursueRomanceWith(h)  // EARLY FILTER
                                );

                            if (target != null && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }

                            // Max Horny + certain personality conditions -> friend intercourse
                            if (desires.Horny == 100 && !isConcient && !isNeurotic && isOpen)
                            {
                                target = (playerClose
                                          && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer
                                          && hero.IsFriendOf(Hero.MainHero)
                                          && hero.HasMutualAttractionWith(Hero.MainHero))
                                    ? Hero.MainHero
                                    : closeHeroes.GetRandomElementWithPredicate(h =>
                                        h.IsAutonom()
                                        && hero.IsFriendOf(h)
                                        && !hero.HasMetRecently(h)
                                        && hero.HasMutualAttractionWith(h)
                                        && !hero.IsRelativeOf(h)
                                        && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                                        && hero.CanPursueRomanceWith(h)  // EARLY FILTER
                                    );

                                if (target != null && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                                {
                                    return;
                                }
                            }

                            // If hero's Honor <= 0, maybe do prisoner intercourse
                            if (hero.GetHeroTraits().Honor <= 0)
                            {
                                target = hero.GetClosePrisoners().GetRandomElementWithPredicate(h =>
                                    hero.GetAttractionTo(h) >= DramalordMCM.Instance.MinAttraction
                                    && !hero.HasMetRecently(h)
                                    && hero.CanPursueRomanceWith(h)   // EARLY FILTER
                                );
                                if (target != null && new PrisonIntercourseIntention(target, hero, CampaignTime.Now).Action())
                                {
                                    return;
                                }
                            }

                            // If the hero is the player's lover but isn't near the player, possibly spawn a quest
                            if (!playerClose
                                && hero.IsLoverOf(Hero.MainHero)
                                && randomNumber <= DramalordMCM.Instance.QuestChance
                                && DramalordQuests.Instance.GetQuest(hero) == null)
                            {
                                // Show inquiry logic remains unchanged
                                int speed = (int)Campaign.Current.TimeControlMode;
                                Campaign.Current.SetTimeSpeed(0);
                                TextObject title = new TextObject("{=Dramalord304}{QUESTHERO} requests your presence.");
                                TextObject text = new TextObject("{=Dramalord303}{HERO.LINK} asks you to find them, as they have an urgent matter to discuss. Will you make it in time?");
                                title.SetTextVariable("HERO1", hero.Name);
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
                                return;
                            }
                        }

                        // 2) BETROTH / MARRIAGE / DATE
                        target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.IsEmotionalWith(Hero.MainHero))
                            ? Hero.MainHero
                            : closeHeroes.GetRandomElementWithPredicate(h =>
                                h.IsAutonom()
                                && hero.IsEmotionalWith(h)
                                && !hero.HasMetRecently(h)
                                && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                                && (h.IsFemale != hero.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage)
                                && hero.CanPursueRomanceWith(h)  // EARLY FILTER
                            );

                        if (target != null)
                        {
                            HeroRelation targetRelation = hero.GetRelationTo(target);

                            // Attempt immediate marriage
                            if (!BetrothIntention.OtherMarriageModFound
                                && targetRelation.Relationship == RelationshipType.Betrothed
                                && targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove
                                && DramalordQuests.Instance.GetQuest(hero) == null
                                && new MarriageIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                            // Or become betrothed
                            else if (hero.Spouse == null
                                && !BetrothIntention.OtherMarriageModFound
                                && targetRelation.Relationship == RelationshipType.Lover
                                && targetRelation.LastInteraction.RemainingDaysFromNow < -3
                                && targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove
                                && new BetrothIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                            // Else do a date intention
                            else if (new DateIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                        }

                        // 3) FLIRT
                        target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.HasMutualAttractionWith(Hero.MainHero))
                            ? Hero.MainHero
                            : closeHeroes.GetRandomElementWithPredicate(h =>
                                h.IsAutonom()
                                && hero.HasMutualAttractionWith(h)
                                && !hero.IsRelativeOf(h)
                                && !hero.HasMetRecently(h)
                                && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                                && hero.CanPursueRomanceWith(h)  // EARLY FILTER
                            );

                        if (target != null)
                        {
                            HeroRelation targetRelation = hero.GetRelationTo(target);
                            // Possibly do a date if love is high enough
                            if (targetRelation.Love >= DramalordMCM.Instance.MinDatingLove
                                && new DateIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                            else if (new FlirtIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                        }

                        // 4) TALK (fallback if no romance occurred)
                        target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer)
                                    ? Hero.MainHero
                                    : closeHeroes.GetRandomElementWithPredicate(h =>
                                        h.IsAutonom()
                                        && !hero.HasMetRecently(h)
                                     );

                        if (target != null && new TalkIntention(target, hero, CampaignTime.Now).Action())
                        {
                            return;
                        }
                    }
                }

                // (Optionally do second checks for quest triggers, etc.)
                if (isHorny && hero.CurrentSettlement != Hero.MainHero.CurrentSettlement && hero.PartyBelongedTo != MobileParty.MainParty && hero.IsLoverOf(Hero.MainHero) && randomNumber <= DramalordMCM.Instance.QuestChance && DramalordQuests.Instance.GetQuest(hero) == null)
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
                                () => {
                                    VisitLoverQuest quest = new VisitLoverQuest(hero);
                                    quest.StartQuest();
                                    DramalordQuests.Instance.AddQuest(hero, quest);
                                    Campaign.Current.SetTimeSpeed(speed);
                                },
                                () => {
                                    Campaign.Current.SetTimeSpeed(speed);

                                    TextObject banner = new TextObject("{=Dramalord302}{HERO.LINK} is disappointed by your neglection of their matter...");
                                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                                    MBInformationManager.AddQuickInformation(banner, 0, hero.CharacterObject, "event:/ui/notification/relation");

                                    RelationshipLossAction.Apply(hero, Hero.MainHero, out int loveDamage, out int trustDamage, 10, 17);
                                    new ChangeOpinionIntention(hero, Hero.MainHero, loveDamage, trustDamage, CampaignTime.Now).Action();

                                    hero.GetDesires().Horny = 0;
                                }), true);


                    return;
                }

                // If nothing else fired and hero has a toy, do toy usage:
                if (desires.HasToy)
                {
                    new UseToyIntention(hero, CampaignTime.Now).Action();
                }
                else
                {
                    desires.Horny += desires.Libido;
                }
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(OnDailyHeroTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //nothing to do
        }

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
};
