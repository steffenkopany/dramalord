using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using Dramalord.Quests;
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
                DramalordIntentions.Instance.RemoveIntention(confronter, intention.Target, intention.Type, intention.EventId);
            }
            if(confronter != null && intention == null)
            {
                DramalordIntentions.Instance.RemoveIntentionsTo(confronter, Hero.MainHero);
            }
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
                        int randomHorny = Decide(100, 150, 0, 100);
                        int randomOpenness = Decide(100, 150, -100, 100);
                        int randomConscientiousness = Decide(-100, -50, -100, 0);
                        int randomExtroversion = Decide(100, 150, -100, 100);
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

                                if (relation.Relationship == RelationshipType.Betrothed)
                                {
                                    if(!isHorny && surrounding == AISurrounding.Settlement && (!isMarried || selectedTarget == Hero.MainHero) && (hero.IsFemale != selectedTarget.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage))
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Marriage, -1);
                                    }
                                    if (!isHorny && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if(isHorny)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.Lover)
                                {
                                    if (!isHorny && (!isMarried || selectedTarget == Hero.MainHero) && relation.CurrentLove >= DramalordMCM.Instance.MinMarriageLove && (hero.IsFemale != selectedTarget.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage))
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Engagement, -1);
                                    }
                                    if (!isHorny && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if(isHorny)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.Spouse)
                                {
                                    if (!isHorny && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if(isHorny)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if(relation.CurrentLove <= 0)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.FriendWithBenefits)
                                {
                                    if(!isHorny && !isOpen && hero.GetAttractionTo(selectedTarget) >= DramalordMCM.Instance.MinAttraction && relation.CurrentLove < DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Flirt, -1);
                                    }
                                    else if (!isHorny && !isOpen && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if (isHorny && isOpen)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if (relation.Trust <= 0)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                    else
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.SmallTalk, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.Friend)
                                {
                                    if (!isHorny && !isOpen && hero.GetAttractionTo(selectedTarget) >= DramalordMCM.Instance.MinAttraction && relation.CurrentLove < DramalordMCM.Instance.MinDatingLove && !hero.IsRelativeOf(selectedTarget))
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Flirt, -1);
                                    }
                                    else if (!isHorny && !isOpen && relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if (isHorny && isOpen && isNotConscientous && isExtrovert && desires.Horny == 100 && !hero.IsRelativeOf(selectedTarget))
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else if (relation.Trust <= 0)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.BreakUp, -1);
                                    }
                                    else
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.SmallTalk, -1);
                                    }
                                }
                                else if (relation.Relationship == RelationshipType.None && (isHorny || isOpen || isExtrovert || isNotConscientous))
                                {
                                    if (hero.GetAttractionTo(selectedTarget) >= DramalordMCM.Instance.MinAttraction && relation.CurrentLove < DramalordMCM.Instance.MinDatingLove && !hero.IsRelativeOf(selectedTarget))
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Flirt, -1);
                                    }
                                    else if (relation.CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Date, -1);
                                    }
                                    else if (isHorny && isOpen && isNotConscientous && isExtrovert && desires.Horny == 100 && selectedTarget == Hero.MainHero && !hero.IsRelativeOf(selectedTarget))
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.Intercourse, -1);
                                    }
                                    else
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, selectedTarget, IntentionType.SmallTalk, -1);
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

                    if (hero.GetHeroTraits()?.Mercy < 0 && personality.Agreeableness < 0 && desires.Horny > 50 && hero.PartyBelongedTo?.PrisonRoster?.TotalHeroes > 0)
                    {
                        Hero? victim = hero.PartyBelongedTo.PrisonRoster.GetTroopRoster().Select(h => h.Character).FirstOrDefault(c => c.IsHero && c.HeroObject != Hero.MainHero && hero.GetAttractionTo(c.HeroObject) > DramalordMCM.Instance.MinAttraction)?.HeroObject;
                        if (victim != null)
                        {
                            DramalordIntentions.Instance.AddIntention(hero, victim, IntentionType.PrisonIntercourse, -1);
                        }
                    }

                    List<HeroIntention> intentions = hero.GetIntentions().ToList();
                    intentions.ForEach(intention =>
                    {
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
                                        LoverAction.Apply(hero, intention.Target);
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
                                

                                if (hero.GetRelationTo(intention.Target).Relationship == RelationshipType.None && hero.GetRelationTo(intention.Target).Trust >= DramalordMCM.Instance.MinTrust)
                                {
                                    FriendshipAction.Apply(hero, intention.Target);
                                }
                                else if (hero.IsFriendlyWith(intention.Target) && hero.GetRelationTo(intention.Target).Trust <= 0)
                                {
                                    BreakupAction.Apply(hero, intention.Target);
                                }
                            }
                        }
                        else if (intention.Type == IntentionType.PrisonIntercourse)
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
                                Clan? targetClan = hero.GetAllRelations().Where(keyvalue => keyvalue.Value.Relationship == RelationshipType.Friend)
                                            .Select(keyvalue => keyvalue.Key)
                                            .FirstOrDefault(selected => selected.Clan != null && selected.Clan != intention.Target.Clan && selected.Clan != Clan.PlayerClan)?.Clan;


                                if (targetClan != null)
                                {
                                    JoinClanAction.Apply(hero, targetClan, true);
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

                        DramalordIntentions.Instance.RemoveIntention(hero, intention.Target, intention.Type, intention.EventId);
                        
                    });
                }
            }
            else if (hero.IsDramalordLegit() && hero != Hero.MainHero && hero.GetDesires().HasToy)
            {
                DramalordIntentions.Instance.RemoveAllIntentions(hero);
                ToyAction.Apply(hero);
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
    }
}
