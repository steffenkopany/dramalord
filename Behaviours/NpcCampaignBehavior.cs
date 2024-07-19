using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.Library;

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
                //InformationManager.DisplayMessage(new InformationMessage($"{confronter.Name} starts {intention.Type}", new Color(1f, 0f, 0f)));
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
            
        }

        private void OnDailyHeroTick(Hero hero)
        {
            if(hero.IsDramalordLegit() && hero != Hero.MainHero && hero.IsAutonom())
            {
                List<Hero> closeHeroes = hero.GetCloseHeroes();
                HeroPersonality personality = hero.GetPersonality();
                HeroDesires desires = hero.GetDesires();
                Dictionary<Hero, HeroRelation> relationships = hero.GetAllRelations().Where(relation => relation.Value.Trust > -30 && CampaignTime.Now.ToDays - relation.Value.LastInteraction > DramalordMCM.Instance.DaysBetweenInteractions).ToDictionary(keypair => ((keypair.Key.Hero1 == hero) ? keypair.Key.Hero2 : keypair.Key.Hero1), keypair => keypair.Value);

                if (MBRandom.RandomInt(-100, 100) <= personality.Extroversion)
                {
                    bool isMarried = hero.Spouse != null;
                    bool husbandClose = hero.Spouse != null && closeHeroes.Contains(hero.Spouse);

                    // romantic stuff
                    IEnumerable<KeyValuePair<Hero, HeroRelation>> closeRelationships = relationships.Where(relation => (relation.Key == Hero.MainHero || relation.Key.IsAutonom()) && closeHeroes.Contains(relation.Key) && !hero.GetIntentions().Any(intention => intention.Target == relation.Key));

                    KeyValuePair<Hero, HeroRelation> targetRelationship = closeRelationships.FirstOrDefault(relationship => relationship.Value.Relationship == RelationshipType.Spouse || relationship.Value.Relationship == RelationshipType.Betrothed || relationship.Value.Relationship == RelationshipType.Lover);

                    if (targetRelationship.Key != null)
                    {
                        if ((!isMarried || targetRelationship.Key == Hero.MainHero) && targetRelationship.Value.Relationship == RelationshipType.Betrothed && hero.CurrentSettlement != null)
                        {
                            DramalordIntentions.Instance.AddIntention(hero, targetRelationship.Key, IntentionType.Marriage, -1);
                        }
                        else if((!isMarried || targetRelationship.Key == Hero.MainHero) && targetRelationship.Value.Relationship == RelationshipType.Lover && targetRelationship.Value.Love >= DramalordMCM.Instance.MinMarriageLove)
                        {
                            DramalordIntentions.Instance.AddIntention(hero, targetRelationship.Key, IntentionType.Engagement, -1);
                        }
                        else
                        {
                            if (MBRandom.RandomInt(0, 100) <= desires.Horny)
                            {
                                DramalordIntentions.Instance.AddIntention(hero, targetRelationship.Key, IntentionType.Intercourse, -1);
                            }
                            else
                            {
                                DramalordIntentions.Instance.AddIntention(hero, targetRelationship.Key, IntentionType.Date, -1);
                            }    
                        }  
                    }

                    if (MBRandom.RandomInt(-100, 100) <= personality.Openness)
                    {
                        List<Hero> flirtHeroes = closeHeroes.Where(target => (target == Hero.MainHero || hero.IsAutonom()) && !hero.GetIntentions().Any(intention => intention.Target == target) && hero.GetAttractionTo(target) >= DramalordMCM.Instance.MinAttraction).ToList();
                        flirtHeroes.Sort((target1, target2) => hero.GetAttractionTo(target2) - hero.GetAttractionTo(target1));

                        Hero? flirtHero = flirtHeroes.FirstOrDefault(h => !hero.IsEmotionalWith(h) && CampaignTime.Now.ToDays - hero.GetRelationTo(h).LastInteraction > DramalordMCM.Instance.DaysBetweenInteractions);
                        if (flirtHero != null)
                        {
                            if (hero.GetRelationTo(flirtHero).Love >= DramalordMCM.Instance.MinDatingLove && !hero.IsRelativeOf(flirtHero))
                            {
                                DramalordIntentions.Instance.AddIntention(hero, flirtHero, IntentionType.Date, -1);
                            }
                            else if (!hero.IsRelativeOf(flirtHero))
                            {
                                DramalordIntentions.Instance.AddIntention(hero, flirtHero, IntentionType.Flirt, -1);
                            }
                        }
                    }

                    if (MBRandom.RandomInt(0, 100) <= desires.Horny && MBRandom.RandomInt(0, 100) <= personality.Openness)
                    {
                        KeyValuePair<Hero, HeroRelation> targetFwB = closeRelationships.Where(relationship => relationship.Value.Relationship == RelationshipType.FriendWithBenefits && !hero.GetIntentions().Any(intention => intention.Target == relationship.Key)).GetRandomElementInefficiently();
                        if (targetFwB.Key != null && desires.Horny > 50)
                        {
                            DramalordIntentions.Instance.AddIntention(hero, targetFwB.Key, IntentionType.Intercourse, -1);
                        }
                    }

                    if (desires.Horny == 100 && MBRandom.RandomInt(50, 100) <= personality.Openness && MBRandom.RandomInt(-100, 0) > personality.Conscientiousness)
                    {
                        KeyValuePair<Hero, HeroRelation> targetFriend = closeRelationships.Where(relationship => relationship.Value.Relationship == RelationshipType.Friend && !hero.GetIntentions().Any(intention => intention.Target == relationship.Key)).GetRandomElementInefficiently();
                        if (targetFriend.Key != null && !hero.IsRelativeOf(targetFriend.Key))
                        {
                            DramalordIntentions.Instance.AddIntention(hero, targetFriend.Key, IntentionType.Intercourse, -1);
                        }
                    }

                    if(MBRandom.RandomInt(-50, 100) <= personality.Extroversion)
                    {
                        Hero? target = closeHeroes.GetRandomElementWithPredicate(target => (target == Hero.MainHero || hero.IsAutonom()) && !hero.GetIntentions().Any(intention => intention.Target == target) && CampaignTime.Now.ToDays - hero.GetRelationTo(target).LastInteraction > DramalordMCM.Instance.DaysBetweenInteractions);
                        if (target != null)
                        {
                            DramalordIntentions.Instance.AddIntention(hero, target, IntentionType.SmallTalk, -1);
                        }
                    }
                }

                List<HeroIntention> intentions = hero.GetIntentions().ToList();
                intentions.ForEach(intention =>
                {
                    if(closeHeroes.Contains(intention.Target))
                    {
                        if (!ConversationHelper.ConversationRunning && intention.Target == Hero.MainHero && MBRandom.RandomInt(1,100) < DramalordMCM.Instance.ChanceApproachingPlayer && intention.Type != IntentionType.Confrontation)
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
    }
}
