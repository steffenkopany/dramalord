using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Dramalord.Behavior
{
    internal class NpcCampaignBehavior : CampaignBehaviorBase
    {
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
                HeroPersonality personality = hero.GetPersonality();
                List<Hero> closeHeroes = hero.GetCloseHeroes();

                if (Decide(100, 125, -100, 100) <= personality.Extroversion)
                {
                    bool isMarried = hero.Spouse != null;
                    bool husbandClose = hero.Spouse != null && closeHeroes.Contains(hero.Spouse);

                    if(hero.CurrentSettlement != null)
                    {
                        HeroDesires desires = hero.GetDesires();
                        Hero? closeHero = closeHeroes.GetRandomElement();
                        if(closeHero != null && closeHero.IsAutonom() && !hero.IsRelativeOf(closeHero) && !hero.GetIntentions().Any(intention => intention.Target == closeHero))
                        {
                            HeroRelation relation = hero.GetRelationTo(closeHero);
                            if ((closeHero == Hero.MainHero && CampaignTime.Now.ToDays - relation.LastInteraction > DramalordMCM.Instance.DaysBetweenPlayerInteractions) ||
                                (closeHero != Hero.MainHero && CampaignTime.Now.ToDays - relation.LastInteraction > DramalordMCM.Instance.DaysBetweenInteractions))
                            {
                                if ((!isMarried || closeHero == Hero.MainHero) && relation.Relationship == RelationshipType.Betrothed && (hero.IsFemale != closeHero.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage))
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Marriage, -1);
                                }
                                else if ((!isMarried || closeHero == Hero.MainHero) && relation.Relationship == RelationshipType.Lover && relation.Love >= DramalordMCM.Instance.MinMarriageLove && (hero.IsFemale != closeHero.IsFemale || DramalordMCM.Instance.AllowSameSexMarriage))
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Engagement, -1);
                                }
                                else if (relation.Relationship == RelationshipType.Spouse || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Lover)
                                {
                                    if (Decide(100, 150, 0, 100) <= desires.Horny)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                    }
                                    else
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Date, -1);
                                    }
                                }
                                else if (Decide(100, 150, 0, 100) <= personality.Openness && (relation.Relationship == RelationshipType.None || relation.Relationship == RelationshipType.Friend) && hero.GetAttractionTo(closeHero) >= DramalordMCM.Instance.MinAttraction)
                                {
                                    if (relation.Love >= DramalordMCM.Instance.MinDatingLove)
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Date, -1);
                                    }
                                    else
                                    {
                                        DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Flirt, -1);
                                    }
                                }
                                else if (Decide(100, 150, 0, 100) <= personality.Openness && relation.Relationship == RelationshipType.FriendWithBenefits && desires.Horny >= 50)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (desires.Horny == 100 && Decide(100, 125, 50, 100) <= personality.Openness && Decide(-100, -75, -100, 0) > personality.Conscientiousness && relation.Relationship == RelationshipType.Friend)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (Decide(100, 125, 0, 100) <= personality.Extroversion)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.SmallTalk, -1);
                                }
                            }
                        }
                    }
                    else if(hero.PartyBelongedTo != null)
                    {
                        if(hero.PartyBelongedTo.Army != null && DramalordMCM.Instance.AllowArmyInteraction)
                        {
                            HeroDesires desires = hero.GetDesires();
                            Hero? closeHero = closeHeroes.GetRandomElement();
                            if (closeHero != null && closeHero.IsAutonom() && !hero.IsRelativeOf(closeHero) && !hero.GetIntentions().Any(intention => intention.Target == closeHero))
                            {
                                HeroRelation relation = hero.GetRelationTo(closeHero);
                                if (relation.Relationship == RelationshipType.Spouse || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Lover && Decide(100, 125, 0, 100) <= desires.Horny)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (Decide(100, 125, 0, 100) <= personality.Openness && (relation.Relationship == RelationshipType.None || relation.Relationship == RelationshipType.Friend) && hero.GetAttractionTo(closeHero) >= DramalordMCM.Instance.MinAttraction)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Flirt, -1);
                                }
                                else if (Decide(100, 125, 0, 100) <= personality.Openness && relation.Relationship == RelationshipType.FriendWithBenefits && desires.Horny >= 50)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (desires.Horny == 100 && Decide(100, 125, 50, 100) <= personality.Openness && Decide(-100, -75, -100, 0) > personality.Conscientiousness && relation.Relationship == RelationshipType.Friend)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (Decide(100, 125, 0, 100) <= personality.Extroversion)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.SmallTalk, -1);
                                }
                            }
                        }
                        else 
                        {
                            HeroDesires desires = hero.GetDesires();
                            Hero? closeHero = closeHeroes.GetRandomElementWithPredicate(other => other.PartyBelongedTo == hero.PartyBelongedTo);
                            if (closeHero != null && closeHero.IsAutonom() && !hero.IsRelativeOf(closeHero) && !hero.GetIntentions().Any(intention => intention.Target == closeHero))
                            {
                                HeroRelation relation = hero.GetRelationTo(closeHero);
                                if (relation.Relationship == RelationshipType.Spouse || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Lover && Decide(100, 125, 0, 100) <= desires.Horny)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (Decide(100, 125, 0, 100) <= personality.Openness && (relation.Relationship == RelationshipType.None || relation.Relationship == RelationshipType.Friend) && hero.GetAttractionTo(closeHero) >= DramalordMCM.Instance.MinAttraction)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Flirt, -1);
                                }
                                else if (Decide(100, 125, 0, 100) <= personality.Openness && relation.Relationship == RelationshipType.FriendWithBenefits && desires.Horny >= 50)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (desires.Horny == 100 && Decide(100, 125, 50, 100) <= personality.Openness && Decide(-100, -75, -100, 0) > personality.Conscientiousness && relation.Relationship == RelationshipType.Friend)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.Intercourse, -1);
                                }
                                else if (Decide(100, 125, 0, 100) <= personality.Extroversion)
                                {
                                    DramalordIntentions.Instance.AddIntention(hero, closeHero, IntentionType.SmallTalk, -1);
                                }
                            }
                        }
                    }
                }

                List<HeroIntention> intentions = hero.GetIntentions().ToList();
                intentions.ForEach(intention =>
                {
                    if(closeHeroes.Contains(intention.Target))
                    {
                        if (!ConversationHelper.ConversationRunning && intention.Target == Hero.MainHero && MBRandom.RandomInt(1,100) < DramalordMCM.Instance.ChanceApproachingPlayer && intention.Type != IntentionType.Confrontation && CampaignTime.Now.ToDays - hero.GetRelationTo(Hero.MainHero).LastInteraction > DramalordMCM.Instance.DaysBetweenPlayerInteractions)
                        {
                            ConversationHelper.ConversationRunning = true;
                            NPCApproachingPlayer.Start(hero, intention);
                        }
                        else if (intention.Target != Hero.MainHero)
                        {
                            if(intention.Type == IntentionType.Confrontation)
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

                                if (!hero.IsEmotionalWith(intention.Target) && hero.GetRelationTo(intention.Target).Love >= DramalordMCM.Instance.MinDatingLove)
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
                                    if(female.IsFertile())
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
                                MarriageAction.Apply(hero, intention.Target, closeHeroes);
                            }
                            else if(intention.Type == IntentionType.BreakUp)
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
                        else if(intention.Type == IntentionType.LeaveClan)
                        {
                            if(hero.Clan != null && hero.Clan == intention.Target.Clan)
                            {
                                LeaveClanAction.Apply(hero, hero, false);
                                Clan? targetClan = hero.GetAllRelations().Where(keyvalue => keyvalue.Value.Relationship == RelationshipType.Friend)
                                            .Select(keyvalue => (keyvalue.Key.Hero1 == hero) ? keyvalue.Key.Hero2 : keyvalue.Key.Hero1)
                                            .FirstOrDefault(selected => selected.Clan != null && selected.Clan != intention.Target.Clan && selected.Clan != Clan.PlayerClan)?.Clan;


                                if (targetClan != null)
                                {
                                    JoinClanAction.Apply(hero, targetClan, true);
                                }
                            }
                        }

                        DramalordIntentions.Instance.RemoveIntention(hero, intention.Target, intention.Type, intention.EventId);
                    } 
                });
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
