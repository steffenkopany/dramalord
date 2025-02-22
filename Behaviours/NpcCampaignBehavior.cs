using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Dramalord.Quests;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

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

                if(isExtrovert)
                {
                    int randomNumber = MBRandom.RandomInt(1, 100);
                    List<Hero> closeHeroes = hero.GetCloseHeroes();
                    bool playerClose = closeHeroes.Contains(Hero.MainHero) && hero.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow >= DramalordMCM.Instance.DaysBetweenInteractions;
                    if(closeHeroes.Count > 0)
                    {
                        Hero? target = null;
                        if (isHorny)
                        {
                            target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.IsSexualWith(Hero.MainHero)) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h.IsAutonom() && hero.IsSexualWith(h) && !hero.HasMetRecently(h) && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord));
                            if(target != null && (target == Hero.MainHero || !desires.HasToy || !target.IsFaithful()) && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }

                            if(desires.Horny == 100 && !isConcient && !isNeurotic && isOpen)
                            {
                                target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.IsFriendOf(Hero.MainHero) && hero.HasMutualAttractionWith(Hero.MainHero)) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h.IsAutonom() && hero.IsFriendOf(h) && !hero.HasMetRecently(h) && hero.HasMutualAttractionWith(h) && !hero.IsRelativeOf(h) && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord));
                                if (target != null && (target == Hero.MainHero || !desires.HasToy || !target.IsFaithful()) && new IntercourseIntention(target, hero, CampaignTime.Now).Action())
                                {
                                    return;
                                }
                            }

                            if(!isAgreeable)
                            {
                                target = hero.GetClosePrisoners().GetRandomElementWithPredicate(h => h.IsAutonom() && hero.GetAttractionTo(h) >= DramalordMCM.Instance.MinAttraction && !hero.HasMetRecently(h));
                                if (target != null && (target == Hero.MainHero || !desires.HasToy || !target.IsFaithful()) && new PrisonIntercourseIntention(target, hero, CampaignTime.Now).Action())
                                {
                                    return;
                                }
                            }

                            if (!playerClose && hero.IsLoverOf(Hero.MainHero) && randomNumber <= DramalordMCM.Instance.QuestChance && DramalordQuests.Instance.GetQuest(hero) == null)
                            {
                                VisitLoverQuest quest = new VisitLoverQuest(hero);
                                quest.StartQuest();
                                DramalordQuests.Instance.AddQuest(hero, quest);
                                return;
                            }
                        }
                        
                        target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.IsEmotionalWith(Hero.MainHero)) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h.IsAutonom() && hero.IsEmotionalWith(h) && !hero.HasMetRecently(h) && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord) && (h.IsFemale != hero.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage));
                        if(target != null)
                        {
                            HeroRelation targetRelation = hero.GetRelationTo(target);
                            if(!BethrothIntention.OtherMarriageModFound && targetRelation.Relationship == RelationshipType.Betrothed && targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove && DramalordQuests.Instance.GetQuest(hero) == null && new MarriageIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                            else if (!BethrothIntention.OtherMarriageModFound && targetRelation.Relationship == RelationshipType.Lover && targetRelation.LastInteraction.RemainingDaysFromNow < -3 && targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove && new BethrothIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                            else if ((target == Hero.MainHero || !desires.HasToy || !target.IsFaithful() || hero.Spouse == target) && new DateIntention(target, hero, CampaignTime.Now).Action())
                            {
                                return;
                            }
                        }

                        //if (isOpen)
                        //{
                            target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.HasMutualAttractionWith(Hero.MainHero)) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h.IsAutonom() && hero.HasMutualAttractionWith(h) && !hero.IsRelativeOf(h) && !hero.HasMetRecently(h) && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord));
                            if (target != null)
                            {
                                HeroRelation targetRelation = hero.GetRelationTo(target);
                                if (targetRelation.Love >= DramalordMCM.Instance.MinDatingLove && (target == Hero.MainHero || !desires.HasToy || !target.IsFaithful() || hero.Spouse == target) && new DateIntention(target, hero, CampaignTime.Now).Action())
                                {
                                    return;
                                }
                                else if (new FlirtIntention(target, hero, CampaignTime.Now).Action())
                                {
                                    return;
                                }
                            }
                        //}

                        target = (playerClose && randomNumber <= DramalordMCM.Instance.ChanceApproachingPlayer) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h.IsAutonom() && !hero.HasMetRecently(h));
                        if (target != null && new TalkIntention(target, hero, CampaignTime.Now).Action())
                        {
                            return;
                        }
                    }
                }

                if(desires.HasToy)
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
}
