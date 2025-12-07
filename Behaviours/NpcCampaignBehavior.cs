using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Extensions;
using Dramalord.Quests;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
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
                bool isExtrovert = personality.Sociability >= WeightedRandom(100, 75, 0, 100);

                bool acceptsPlayer = hero.Clan != null && hero.Clan.Tier > Clan.PlayerClan.Tier ? false : (hero.GetTrust(Hero.MainHero) <= DramalordMCM.Instance.MaxTrustEnemies) ? false : !hero.GetRelationTo(Hero.MainHero).IsBlocked();

                if (isExtrovert)
                {
                    List<Hero> closeHeroes = hero.GetCloseHeroes();
                    bool playerClose = closeHeroes.Contains(Hero.MainHero)
                                  && hero.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow >= DramalordMCM.Instance.DaysBetweenInteractions;


                    foreach(var item in hero.GetAllRelations())
                    {
                        //cleanup
                        if(item.Key != Hero.MainHero && item.Value.Love <= 0 && (item.Value.Relationship == RelationshipType.Lover || item.Value.Relationship == RelationshipType.Spouse))
                        {
                            (new RelationshipEvent(hero, item.Key)).Action(); //DramalordEvents.Instance.StartIntention(new RelationshipEvent(hero, item.Key)); 
                        }
                        else if (item.Key != Hero.MainHero && hero.GetTrust(item.Key) <= 0 && item.Value.Relationship == RelationshipType.Friend)
                        {
                            (new RelationshipEvent(hero, item.Key)).Action(); //DramalordEvents.Instance.StartIntention(new RelationshipEvent(hero, item.Key));
                        }
                    }

                    if (closeHeroes.Count > 0)
                    {
                        Hero? target = null;

                        // 1) PRISONERS / HORNY-INTERCOURSE / ETC.
                        if (isHorny)
                        {
                            
                            // If hero's Honor <= 0, maybe do prisoner intercourse
                            if ((hero.GetTraitLevel(DefaultTraits.Honor) < 1 && hero.GetPersonality().Empathy < 50) && (hero.PartyBelongedTo == null || !hero.PartyBelongedTo.IsMainParty))
                            {
                                target = hero.GetClosePrisoners().GetRandomElementWithPredicate(h =>
                                    hero.GetAttractionTo(h) >= DramalordMCM.Instance.MinAttraction
                                    && !hero.HasMetRecently(h)
                                );
                                if (target != null)
                                {
                                    if(target == Hero.MainHero && !hero.HasMet)
                                    {
                                        hero.SetHasMet();
                                    }
                                    DramalordEvents.Instance.StartIntention(new PrisonSexEvent(hero, target));
                                    return;
                                }
                            }

                            // If the hero is the player's lover but isn't near the player, possibly spawn a quest
                            if (!playerClose
                                && hero.IsLoverOf(Hero.MainHero)
                                && MBRandom.RandomInt(1, 100) <= DramalordMCM.Instance.QuestChance
                                && DramalordQuests.Instance.GetQuest(hero) == null
                                && hero.GetRelationTo(Hero.MainHero).Love > 0)
                            {
                                VisitLoverQuest quest = new VisitLoverQuest(hero);
                                quest.StartQuest();
                                DramalordQuests.Instance.AddQuest(hero, quest);

                                return;
                            }
                        }

                        // 2) BETROTH / MARRIAGE / DATE
                        target = (playerClose && MBRandom.RandomInt(1, 100) <= DramalordMCM.Instance.ChanceApproachingPlayer)
                            ? Hero.MainHero
                            : closeHeroes.GetRandomElementWithPredicate(h =>
                                h.IsAutonom()
                                && h != Hero.MainHero
                                && hero.IsEmotionalWith(h)
                                && !hero.HasMetRecently(h)
                                && !hero.IsBlockedBy(h)
                                && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                            );

                        if (target != null && hero.IsEmotionalWith(target))
                        {
                            HeroRelation targetRelation = hero.GetRelationTo(target);

                            // Attempt immediate marriage
                            if (targetRelation.Relationship == RelationshipType.Lover
                                && targetRelation.Love >= DramalordMCM.Instance.MinMarriageLove
                                && DramalordQuests.Instance.GetQuest(hero) == null
                                && (target.IsFemale != hero.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage))
                            {
                                DramalordEvents.Instance.StartIntention(new MarriageEvent(hero, target));
                                return;
                            }
                            // Else do a date intention
                            else
                            {
                                DramalordEvents.Instance.StartIntention(new DateEvent(hero, target));
                                return;
                            }
                        }

                        // 3) FLIRT
                        target = (playerClose && acceptsPlayer && MBRandom.RandomInt(1,100) <= DramalordMCM.Instance.ChanceApproachingPlayer && hero.HasMutualAttractionWith(Hero.MainHero))
                            ? Hero.MainHero
                            : closeHeroes.GetRandomElementWithPredicate(h =>
                                h.IsAutonom()
                                && h != Hero.MainHero
                                && hero.GetAttractionTo(h) >= DramalordMCM.Instance.MinAttraction
                                && !hero.IsRelativeOf(h)
                                && !hero.HasMetRecently(h)
                                && !hero.IsBlockedBy(h)
                                && (DramalordMCM.Instance.AllowSocialClassMix || h.IsLord == hero.IsLord)
                            );

                        if (target != null)
                        {
                            HeroRelation targetRelation = hero.GetRelationTo(target);
                            // Possibly do a date if love is high enough
                            if (targetRelation.Love >= DramalordMCM.Instance.MinDatingLove)
                            {
                                DramalordEvents.Instance.StartIntention(new DateEvent(hero, target));
                                return;
                            }
                            else
                            {
                                DramalordEvents.Instance.StartIntention(new FlirtEvent(hero, target));
                                return;
                            }
                        }

                        // 4) TALK (fallback if no romance occurred)
                        target = (playerClose && acceptsPlayer && MBRandom.RandomInt(1, 100) <= DramalordMCM.Instance.ChanceApproachingPlayer)
                                    ? Hero.MainHero
                                    : closeHeroes.GetRandomElementWithPredicate(h =>
                                        h.IsAutonom()
                                        && h != Hero.MainHero
                                        && !hero.HasMetRecently(h)
                                        && !hero.IsBlockedBy(h)
                                     );

                        if (target != null)
                        {
                            DramalordEvents.Instance.StartIntention(new TalkEvent(hero, target));
                            return;
                        }
                    }
                }

                // (Optionally do second checks for quest triggers, etc.)
                if (isHorny && ((hero.CurrentSettlement == null) || (hero.CurrentSettlement != Hero.MainHero.CurrentSettlement)) && hero.PartyBelongedTo != MobileParty.MainParty && hero.IsLoverOf(Hero.MainHero)
                                && hero.GetRelationTo(Hero.MainHero).Love > 0 && MBRandom.RandomInt(1, 100) <= DramalordMCM.Instance.QuestChance && DramalordQuests.Instance.GetQuest(hero) == null)
                {
                    VisitLoverQuest quest = new VisitLoverQuest(hero);
                    quest.StartQuest();
                    DramalordQuests.Instance.AddQuest(hero, quest);

                    return;
                }

                if(hero.Spouse != null && hero.Spouse != Hero.MainHero && (hero.IsFemale == hero.Spouse.IsFemale || (!hero.IsFertile() && !hero.Spouse.IsFertile())))
                {
                    if(hero.Children.Count == 0)
                    {
                        Hero? child = DramalordOrphans.Instance.GetRandomOrphan();
                        if (child != null)
                        {
                            DramalordEvents.Instance.StartIntention(new AdoptEvent(hero, child)); 
                        }
                    }
                    return;
                }

                // If nothing else fired and hero has a toy, do toy usage:
                desires.Horny += desires.Libido;
                
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
            int result;
            do
            {
                // Box-Muller requires two independent uniform randoms in (0,1]
                float u1 = MBRandom.RandomFloat;
                float u2 = MBRandom.RandomFloat;
                if (u1 <= 0f) u1 = float.Epsilon;
                if (u2 <= 0f) u2 = float.Epsilon;
                float rand_std_normal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2));
                result = (int)(peakValue + normalValue * rand_std_normal);
            } while (result < minValue || result > maxValue);

            return result;
        }

        /*
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
        */
    }
}
