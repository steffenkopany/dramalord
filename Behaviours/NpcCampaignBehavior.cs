using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using Dramalord.Quests;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;

namespace Dramalord.Behavior
{
    internal class NpcCampaignBehavior : CampaignBehaviorBase
    {
        internal enum AISurrounding
        {
            Settlement,
            Army,
            Party,
            None
        }

        internal NpcCampaignBehavior(CampaignGameStarter starter)
        {
            NPCApproachingPlayer.AddDialogs(starter);
            NPCConfrontPlayer.AddDialogs(starter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(OnDailyHeroTick));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(OnHourlyTick));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(KingdomDestroyedEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //nothing to do
        }

        internal void OnHourlyTick()
        {
            Hero? confronter = DramalordIntentions.Instance.GetIntentionTowardsPlayer().FirstOrDefault(hero => !hero.IsPrisoner && hero.IsCloseTo(Hero.MainHero));
            HeroIntention? intention = confronter?.GetIntentions().Where(intention => intention.Target == Hero.MainHero && intention.Type == IntentionType.Confrontation).FirstOrDefault();

            if (!ConversationHelper.ConversationRunning && confronter != null && intention != null)
            {
                ConversationHelper.ConversationRunning = true;
                HeroEvent? @event = DramalordEvents.Instance.GetEvent(intention.EventId);
                if (@event != null && (confronter.IsEmotionalWith(@event.Actors.Hero1) || confronter.IsEmotionalWith(@event.Actors.Hero2)))
                {
                    NPCConfrontPlayer.Start(confronter, @event);
                }
                else
                {
                    ConversationHelper.ConversationRunning = false;
                }
                confronter.RemoveIntention(intention);
            }
            /*
            if(confronter != null && intention == null)
            {
                confronter.RemoveIntentionsTo(Hero.MainHero);
            }
            */
        }

        private void OnDailyHeroTick(Hero hero)
        {
            if(hero.IsDramalordLegit() && hero != Hero.MainHero && hero.IsAutonom())
            {
                AISurrounding surrounding = new();
                surrounding = AISurrounding.None;

                if (hero.CurrentSettlement != null)
                {
                    surrounding = AISurrounding.Settlement;
                }
                else if (hero.PartyBelongedTo != null)
                {
                    if (hero.PartyBelongedTo.Army != null && DramalordMCM.Instance.AllowArmyInteraction)
                    {
                        surrounding = AISurrounding.Army;
                    }
                    else
                    {
                        surrounding = AISurrounding.Party;
                    }
                }

                if(surrounding != AISurrounding.None)
                {
                    HeroPersonality personality = hero.GetPersonality();
                    List<HeroIntention> intentionsCheck = hero.GetIntentions();
                    List<Hero> closeHeroes = hero.GetCloseHeroes();
                    HeroDesires desires = hero.GetDesires();

                    List<Hero> availableHeroes = closeHeroes.Where(h => !intentionsCheck.Any(i => i.Target == h) && h.IsAutonom()).ToList();

                    if (availableHeroes.Count > 0)
                    {
                        int randomHorny = Decide(100, 125, 0, 100);
                        int randomOpenness = Decide(100, 125, -100, 100);
                        int randomConscientiousness = Decide(-100, -50, -100, 0);
                        int randomExtroversion = Decide(100, 125, -100, 100);
                        bool isMarried = hero.Spouse != null;

                        if (personality.Extroversion >= randomExtroversion)
                        {
                            Hero? selectedTarget = GetAvailableTarget(hero, availableHeroes);
                            if(selectedTarget != null)
                            {
                                HeroRelation relation = hero.GetRelationTo(selectedTarget);

                                bool isHorny = randomHorny <= desires.Horny;
                                bool isOpen = randomOpenness <= personality.Openness;
                                bool isNotConscientous = randomConscientiousness > personality.Conscientiousness;
                                bool isExtrovert = randomExtroversion <= personality.Extroversion;
                                bool isRelative = hero.IsRelativeOf(selectedTarget);

                                HeroDesires targetDesires = DramalordDesires.Instance.GetDesires(selectedTarget);
                                bool isTargetHorny = randomHorny <= targetDesires.Horny;
                                bool isGenderCompatible = (hero.IsFemale) ? targetDesires.AttractionWomen >= 50 : targetDesires.AttractionMen >= 50;

                                if (relation.Relationship == RelationshipType.Betrothed)
                                {
                                    if(!isHorny && surrounding == AISurrounding.Settlement && (!isMarried || selectedTarget == Hero.MainHero) && (hero.IsFemale != selectedTarget.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage))
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Marriage, -1);
                                    }
                                    if (!isHorny && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if(isHorny && isTargetHorny)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if(relation.CurrentLove < DramalordMCM.Instance.MinDatingLove && selectedTarget != Hero.MainHero)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                    else if(relation.CurrentLove <= 0 && selectedTarget == Hero.MainHero)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.Lover)
                                {
                                    if (!isHorny && (!isMarried || selectedTarget == Hero.MainHero) && relation.CurrentLove >= DramalordMCM.Instance.MinMarriageLove && (hero.IsFemale != selectedTarget.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage))
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Engagement, -1);
                                    }
                                    if (!isHorny && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if(isHorny && isTargetHorny)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if (relation.CurrentLove < DramalordMCM.Instance.MinDatingLove && selectedTarget != Hero.MainHero)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                    else if (relation.CurrentLove <= 0 && selectedTarget == Hero.MainHero)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.Spouse)
                                {
                                    if (!isHorny && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if(isHorny && isTargetHorny)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if(relation.CurrentLove <= 0)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.FriendWithBenefits)
                                {
                                    if(!isHorny && !isOpen && hero.GetAttractionTo(selectedTarget) >= DramalordMCM.Instance.MinAttraction && relation.CurrentLove < DramalordMCM.Instance.MinDatingLove)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Flirt, -1);
                                    }
                                    else if (!isHorny && !isOpen && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if (isHorny && isOpen && isTargetHorny)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if (hero.GetTrust(selectedTarget) <= 0)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                    else
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.SmallTalk, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.Friend)
                                {
                                    if (!isHorny && !isOpen && hero.GetAttractionTo(selectedTarget) >= DramalordMCM.Instance.MinAttraction && relation.CurrentLove < DramalordMCM.Instance.MinDatingLove && !isRelative)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Flirt, -1);
                                    }
                                    else if (!isHorny && !isOpen && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove && !isRelative)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if (isHorny && isOpen && isNotConscientous && isExtrovert && desires.Horny == 100 && !isRelative && isTargetHorny && isGenderCompatible)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if (hero.GetTrust(selectedTarget) <= 0)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                    else
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.SmallTalk, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.None && (isHorny || isOpen || isExtrovert || isNotConscientous))
                                {
                                    if (hero.GetAttractionTo(selectedTarget) >= DramalordMCM.Instance.MinAttraction && relation.CurrentLove < DramalordMCM.Instance.MinDatingLove && !isRelative)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Flirt, -1);
                                    }
                                    else if (relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove && !isRelative)
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if (isHorny && isOpen && isNotConscientous && isExtrovert && desires.Horny == 100 && selectedTarget == Hero.MainHero && !isRelative && (Hero.MainHero.IsFemale ? desires.AttractionWomen >= 50 : desires.AttractionMen >= 50))
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else
                                    {
                                        hero.AddIntention(selectedTarget, IntentionType.SmallTalk, -1);
                                    }
                                }
                            }
                        }        
                    }
                    else
                    {
                        HeroRelation relation = hero.GetRelationTo(Hero.MainHero);
                        if(relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Spouse)
                        {
                            if(CampaignTime.Now.ToDays - relation.LastInteraction > DramalordMCM.Instance.DaysBetweenPlayerInteractions && MBRandom.RandomInt(1,100) < DramalordMCM.Instance.QuestChance && desires.Horny == 100)
                            {
                                VisitQuest? quest = DramalordQuests.Instance.GetLoverQuest(hero);
                                if(quest == null)
                                {
                                    DramalordQuests.Instance.CreateLoverQuest(hero).StartQuest();
                                }
                            }
                        }
                    }

                    if (hero.GetHeroTraits()?.Mercy < 0 && personality.Agreeableness < 0 && desires.Horny > 50 && hero.PartyBelongedTo?.LeaderHero == hero && hero.PartyBelongedTo?.PrisonRoster?.TotalHeroes > 0)
                    {
                        Hero? victim = hero.PartyBelongedTo.PrisonRoster.GetTroopRoster().Select(h => h.Character).FirstOrDefault(c => c.IsHero /*&& c.HeroObject != Hero.MainHero*/ && hero.GetAttractionTo(c.HeroObject) > DramalordMCM.Instance.MinAttraction)?.HeroObject;
                        if (victim != null)
                        {
                            hero.AddIntention(victim, IntentionType.PrisonIntercourse, -1);
                            Hero.SetHeroEncyclopediaTextAndLinks(hero);
                        }
                    }
                }
                
                hero.GetDesires().Horny += hero.GetDesires().Libido; // test
                
            }
            else if (hero.IsDramalordLegit() && hero != Hero.MainHero && hero.GetDesires().HasToy)
            {
                hero.RemoveAllIntentions();
                ToyAction.Apply(hero);
            }

            List<HeroIntention> intentions = hero.GetIntentions().ToList();
            if(intentions.Count > 0)
            {
                List<Hero> closeHeroes = hero.GetCloseHeroes();
                for(int i = 0; i < intentions.Count; i++)
                { 
                    HeroIntention intention = intentions[i];
                    if (closeHeroes.Contains(intention.Target))
                    {
                        if (!ConversationHelper.ConversationRunning && intention.Target == Hero.MainHero && MBRandom.RandomInt(1, 100) < DramalordMCM.Instance.ChanceApproachingPlayer && intention.Type != IntentionType.Confrontation && CampaignTime.Now.ToDays - hero.GetRelationTo(Hero.MainHero).LastInteraction > DramalordMCM.Instance.DaysBetweenPlayerInteractions)
                        {
                            ConversationHelper.ConversationRunning = true;
                            NPCApproachingPlayer.Start(hero, intention);
                        }
                        else if (intention.Target != Hero.MainHero)
                        {
                            if (intention.Type == IntentionType.Confrontation)
                            {
                                ConfrontationAction.Apply(hero, intention);
                            }
                            else if (intention.Type == IntentionType.SmallTalk)
                            {
                                TalkAction.Apply(hero, intention.Target);
                            }
                            else if (intention.Type == IntentionType.Flirt)
                            {
                                FlirtAction.Apply(hero, intention.Target);
                            }
                            else if (intention.Type == IntentionType.Date)
                            {
                                DateAction.Apply(hero, intention.Target, closeHeroes);

                                if (!hero.IsEmotionalWith(intention.Target) && hero.GetRelationTo(intention.Target).CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                {
                                    Dictionary<Hero, HeroRelation> lovers = hero.GetAllRelations().Where(item => item.Value.Relationship == RelationshipType.Lover).ToDictionary(item => item.Key, item => item.Value);
                                    int lcount = lovers.Count;
                                    if (lovers.Count > 0)
                                    {
                                        lovers.Do(pair =>
                                        {
                                            if (pair.Value.Love >= 0 && pair.Key != Hero.MainHero)
                                            {
                                                BreakupAction.Apply(hero, pair.Key);
                                                lcount--;
                                            }
                                        });
                                    }
                                    if (lcount == 0)
                                    {
                                        LoverAction.Apply(hero, intention.Target);
                                    }
                                }
                            }
                            else if (intention.Type == IntentionType.Intercourse)
                            {
                                IntercourseAction.Apply(hero, intention.Target, closeHeroes);
                                if (hero.IsFemale != intention.Target.IsFemale && MBRandom.RandomInt(1, 100) < DramalordMCM.Instance.PregnancyChance)
                                {
                                    Hero female = (hero.IsFemale) ? hero : intention.Target;
                                    if (female.IsFertile())
                                    {
                                        ConceiveAction.Apply((hero.IsFemale) ? hero : intention.Target, (hero.IsFemale) ? intention.Target : hero);
                                    }
                                }

                                if (hero.IsFriendOf(intention.Target))
                                {
                                    FriendsWithBenefitsAction.Apply(hero, intention.Target);
                                }
                            }
                            else if (intention.Type == IntentionType.Engagement)
                            {
                                EngagementAction.Apply(hero, intention.Target, closeHeroes);
                            }
                            else if (intention.Type == IntentionType.Marriage)
                            {
                                Actions.MarriageAction.Apply(hero, intention.Target, closeHeroes);
                            }
                            else if (intention.Type == IntentionType.BreakUp)
                            {
                                BreakupAction.Apply(hero, intention.Target);
                            }


                            if (hero.GetRelationTo(intention.Target).Relationship == RelationshipType.None && hero.GetTrust(intention.Target) >= DramalordMCM.Instance.MinTrust)
                            {
                                List<Hero> noFriends = hero.GetAllRelations().Where(item => item.Value.Relationship == RelationshipType.Friend && hero.GetTrust(item.Key) <= 0).Select(item => item.Key).ToList();
                                noFriends.ForEach(item =>
                                {
                                    BreakupAction.Apply(hero, item);
                                });

                                FriendshipAction.Apply(hero, intention.Target);
                            }
                            else if (hero.IsFriendlyWith(intention.Target) && hero.GetTrust(intention.Target) <= 0)
                            {
                                BreakupAction.Apply(hero, intention.Target);
                            }
                        }
                    }
   
                    if (intention.Type == IntentionType.PrisonIntercourse && intention.Target != Hero.MainHero)
                    {
                        IntercourseAction.Apply(hero, intention.Target, closeHeroes);
                        if (hero.IsFemale != intention.Target.IsFemale && MBRandom.RandomInt(1, 100) < DramalordMCM.Instance.PregnancyChance)
                        {
                            Hero female = (hero.IsFemale) ? hero : intention.Target;
                            if (female.IsFertile())
                            {
                                ConceiveAction.Apply((hero.IsFemale) ? hero : intention.Target, (hero.IsFemale) ? intention.Target : hero);
                            }
                        }
                        if ((hero.Clan == Clan.PlayerClan || intention.Target.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
                        {
                            LogEntry.AddLogEntry(new PrisonIntercourseLog(hero, intention.Target));
                        }
                        EndCaptivityAction.ApplyByRansom(intention.Target, hero);
                    }
                    else if (intention.Type == IntentionType.LeaveClan)
                    {
                        if (hero.Clan != null && hero.Clan == intention.Target.Clan)
                        {
                            LeaveClanAction.Apply(hero, hero, false);

                            Clan? targetClan = hero.GetAllRelations().Where(keyvalue => keyvalue.Key == hero.Father || keyvalue.Key == hero.Mother )
                                .Select(keyvalue => keyvalue.Key)
                                .FirstOrDefault(selected => selected.Clan != null && selected.Clan != intention.Target.Clan && selected.Clan != Clan.PlayerClan)?.Clan;

                            if(targetClan == null)
                            {
                                targetClan = hero.GetAllRelations().Where(keyvalue => keyvalue.Value.Relationship == RelationshipType.Friend)
                                        .Select(keyvalue => keyvalue.Key)
                                        .FirstOrDefault(selected => selected.Clan != null && selected.Clan != intention.Target.Clan && selected.Clan != Clan.PlayerClan)?.Clan;
                            }

                            if (targetClan != null)
                            {
                                JoinClanAction.Apply(hero, targetClan, true);
                            }
                        }
                    }
                    else if (intention.Type == IntentionType.LeaveKingdom)
                    {
                        if (hero.Clan != null && hero.Clan.Kingdom != null && intention.Target.Clan != null && intention.Target.Clan.Kingdom != null && hero.Clan.Kingdom == intention.Target.Clan.Kingdom)
                        {
                            LeaveKingdomAction.Apply(hero.Clan);
                            int love = 0;
                            Kingdom? target = null;
                            hero.GetAllRelations().Do(item =>
                            {
                                if ((item.Value.Relationship == RelationshipType.Lover || item.Value.Relationship == RelationshipType.Betrothed) && item.Value.Love > love && item.Key.Clan.Kingdom != null && item.Key.Clan.Kingdom != intention.Target.Clan.Kingdom)
                                {
                                    love = item.Value.Love;
                                    target = item.Key.Clan.Kingdom;
                                }
                            });

                            if (target == null)
                            {
                                hero.GetAllRelations().Do(item =>
                                {
                                    if ((item.Value.Relationship == RelationshipType.Friend || item.Value.Relationship == RelationshipType.FriendWithBenefits) && hero.GetTrust(item.Key) > love && item.Key.Clan.Kingdom != null && item.Key.Clan.Kingdom != intention.Target.Clan.Kingdom)
                                    {
                                        love = hero.GetTrust(item.Key);
                                        target = item.Key.Clan.Kingdom;
                                    }
                                });
                            }

                            if (target != null)
                            {
                                JoinKingdomAction.Apply(hero.Clan, target);
                            }
                        }
                    }
                    else if (intention.Type == IntentionType.Orphanize)
                    {
                        if (intention.Target.IsChild)
                        {
                            OrphanizeAction.Apply(hero, intention.Target);
                        }
                    }
                    else if (intention.Type == IntentionType.Adopt && hero.Spouse != null && DramalordOrphans.Instance.CountOrphans(false) + DramalordOrphans.Instance.CountOrphans(true) > 0)
                    {
                        Hero? child = DramalordOrphans.Instance.GetRandomOrphan();
                        if (child != null)
                        {
                            AdoptAction.Apply(hero, hero.Spouse, child);
                        }
                    }
                    else if (intention.Type == IntentionType.Abortion)
                    {
                        AbortionAction.Apply(hero);
                    }
                    
                    
                    hero.RemoveIntention(intention);

                }
            }
            
        }

        private Hero? GetAvailableTarget(Hero hero, List<Hero> targets)
        {
            if (targets.Count == 0)
                return null;

            Hero target = targets.GetRandomElement();
            double daysPassed = CampaignTime.Now.ToDays - hero.GetRelationTo(target).LastInteraction;
            double daysNecessary = (hero == Hero.MainHero) ? DramalordMCM.Instance.DaysBetweenPlayerInteractions : DramalordMCM.Instance.DaysBetweenInteractions;
            if (daysPassed >= daysNecessary)
            {
                return target;
            }
            else
            {
                targets.Remove(target);
                return GetAvailableTarget(hero, targets);
            }
        }

        private int Decide(int peak, int normal, int min, int max)
        {
            float rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
            int result = (int)(peak + normal * rand_std_normal);
            while (result < min || result > max)
            {
                rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
                result = (int)(peak + normal * rand_std_normal);
            }

            return result;
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            if(DramalordMCM.Instance.AllowClansChangingKingdoms)
            {
                Dictionary<Clan, Kingdom> targetList = new();
                kingdom.Clans.ForEach(clan =>
                {
                    Hero leader = clan.Leader;
                    int love = 0;
                    Kingdom? target = null;
                    leader.GetAllRelations().Do(item =>
                    {
                        if ((item.Value.Relationship == RelationshipType.Lover || item.Value.Relationship == RelationshipType.Betrothed) && item.Value.Love > love && item.Key.Clan.Kingdom != null && item.Key.Clan.Kingdom != kingdom)
                        {
                            love = item.Value.Love;
                            target = item.Key.Clan.Kingdom;
                        }
                    });

                    if (target == null)
                    {
                        leader.GetAllRelations().Do(item =>
                        {
                            if ((item.Value.Relationship == RelationshipType.Friend || item.Value.Relationship == RelationshipType.FriendWithBenefits) && leader.GetTrust(item.Key) > love && item.Key.Clan.Kingdom != null && item.Key.Clan.Kingdom != kingdom)
                            {
                                love = leader.GetTrust(item.Key);
                                target = item.Key.Clan.Kingdom;
                            }
                        });
                    }

                    if (target != null)
                    {
                        targetList.Add(clan, target);
                    }
                });

                targetList.Do(item =>
                {
                    LeaveKingdomAction.Apply(item.Key);
                    JoinKingdomAction.Apply(item.Key, item.Value);
                });
            }
        }
    }
}
