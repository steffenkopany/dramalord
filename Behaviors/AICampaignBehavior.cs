using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Quests;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace Dramalord.Behaviors
{
    internal class AICampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(OnDailyHeroTick));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(DramalordEvents.GroomEvents));
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(OnHeroKilled));
            CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener(this, new Action<Hero>(OnHeroUnregistered));
            CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(OnNewCompanionAdded));
            CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(OnHeroComesOfAge));
        }

        public override void SyncData(IDataStore dataStore)
        {
            //HeroDataSaver.SyncData(dataStore);
        }

        internal AICampaignBehavior(CampaignGameStarter starter)
        {

        }

        internal void OnDailyHeroTick(Hero hero)
        {
            if(hero.IsDramalordLegit() && hero != Hero.MainHero && !(hero.Clan == Clan.PlayerClan && !DramalordMCM.Get.AllowPlayerClanAI))
            {
                int dailyChance = MBRandom.RandomInt(0, 100);
                DramalordTraits traits = hero.GetDramalordTraits();

                //daily check if hero wants to communicate
                int checknumber = 50 + (traits.Extroversion * 20);
                if(checknumber > dailyChance)
                {
                    List<HeroMemory> memories = hero.GetDramalordMemory();
                    List<Hero> closeHeroes = hero.GetCloseHeroes();
                    List<Hero> closeHeroesCopy = closeHeroes.ToList();

                    if (closeHeroes.Count > 0)
                    {
                        memories.Where(item => item.Active).Do(item =>
                        {
                            if (closeHeroes.Contains(item.Event.Hero1.HeroObject) && (hero.IsLover(item.Event.Hero1.HeroObject) || hero.IsSpouse(item.Event.Hero1.HeroObject)))
                            {
                                HeroConfrontationAction.Apply(hero, item.Event.Hero1.HeroObject, traits, item);
                                closeHeroes.Remove(item.Event.Hero1.HeroObject);
                                hero.ActivateDramalordMemory(item.EventId, false);
                            }
                            if (closeHeroes.Contains(item.Event.Hero2.HeroObject) && (hero.IsLover(item.Event.Hero2.HeroObject) || hero.IsSpouse(item.Event.Hero2.HeroObject)))
                            {
                                HeroConfrontationAction.Apply(hero, item.Event.Hero2.HeroObject, traits, item);
                                closeHeroes.Remove(item.Event.Hero1.HeroObject);
                                hero.ActivateDramalordMemory(item.EventId, false);
                            }
                        });

                        if (traits.HasToy == 1)
                        {
                            HeroToyAction.Apply(hero);
                            return;
                        }

                        foreach (Hero charObj in closeHeroes)
                        {
                            if (!HandleCloseHeroes(hero, charObj, traits, memories, closeHeroes))
                            {
                                return;
                            }
                        }
                        HandlePrisoners(hero, traits);

                    }

                    ThinkOverMemories(hero, traits, memories);
                }
                else
                {
                    traits.Horny = traits.Horny + traits.Libido;
                }
            }
        }

        internal bool HandleCloseHeroes(Hero hero, Hero target, DramalordTraits traits, List<HeroMemory> memories, List<Hero> potentialWitnesses)
        {
            return NPCConversationAction.Apply(hero, target, traits, memories, potentialWitnesses);
        }

        internal void ThinkOverMemories(Hero hero, DramalordTraits traits, List<HeroMemory> memories)
        {
            if(hero.Clan == null && hero.GetHeroSpouses().Count > 0)
            {
                CharacterObject? result = hero.GetHeroSpouses().Where(item => item.HeroObject.Clan != null).First();
                if(result != null)
                {
                    //TODO - check if player
                    HeroJoinClanAction.Apply(hero, result.HeroObject.Clan);
                }
            }

            if(hero.IsLover(Hero.MainHero) || hero.IsSpouse(Hero.MainHero))
            {
                if(hero.GetDramalordTraits().Horny >= DramalordMCM.Get.MinHornyForIntercourse && !VisitLoverQuest.HeroList.ContainsKey(hero) && MBRandom.RandomInt(1,100) < DramalordMCM.Get.ChanceNPCQuestVisitPlayer)
                {
                    (new VisitLoverQuest(hero)).StartQuest();
                }
            }
        }

        internal void HandlePrisoners(Hero hero, DramalordTraits traits)
        {
            if(traits.Horny >= DramalordMCM.Get.MinHornyForIntercourse)
            {
                List<Hero> heroList = hero.GetClosePrisoners();
                heroList.ForEach(item =>
                {
                    if (hero.GetDramalordAttractionTo(item) > DramalordMCM.Get.MinAttractionForFlirting)
                    {
                        if (item == Hero.MainHero)
                        {
                            NpcInteractions.start(hero, EventType.Intercourse);
                        }
                        else if (item.GetDramalordTraits().Openness > 0)
                        {
                            HeroIntercourseAction.Apply(hero, item, new List<Hero>());
                            EndCaptivityAction.ApplyByRansom(item, hero);
                        }
                    }
                });
            }
        }

        internal void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool ShowStuff)
        {
            OnHeroUnregistered(victim);
        }

        internal void OnHeroUnregistered(Hero hero)
        {
            DramalordRelations.Partners.Where(item => item.Key == hero.CharacterObject).ToList().ForEach(item =>
            {
                item.Value.Spouses.ForEach(it1 =>
                {
                    if (DramalordRelations.Partners.ContainsKey(it1))
                    {
                        DramalordRelations.Partners[it1].Spouses.Remove(item.Key);
                    }
                });

                item.Value.Lovers.ForEach(it1 =>
                {
                    if (DramalordRelations.Partners.ContainsKey(it1))
                    {
                        DramalordRelations.Partners[it1].Spouses.Remove(item.Key);
                    }
                });

                item.Value.FriendsWithBenefits.ForEach(it1 =>
                {
                    if (DramalordRelations.Partners.ContainsKey(it1))
                    {
                        DramalordRelations.Partners[it1].Spouses.Remove(item.Key);
                    }
                });

                DramalordRelations.Partners.Remove(item.Key);
            });

            DramalordPregnancies.RemovePregnancy(hero);
            DramalordOrphanage.RemoveOrphan(hero.CharacterObject);
            DramalordEvents.Events.Where(item => item.Value.Hero1 == hero.CharacterObject || item.Value.Hero2 == hero.CharacterObject).ToList().ForEach(item => DramalordEvents.Events.Remove(item.Key));
            DramalordMemories.Memories.Where(item => item.Key == hero.CharacterObject).ToList().ForEach(item => DramalordMemories.Memories.Remove(item.Key));
        }

        internal void OnNewCompanionAdded(Hero companion)
        {
            if (companion.Clan == Clan.PlayerClan)
            {
                foreach (Hero child in companion.Children)
                {
                    if (child.IsChild && child.Clan == null)
                    {
                        child.Clan = companion.Clan;
                        child.UpdateHomeSettlement();
                        child.SetNewOccupation(companion.Occupation);
                        child.ChangeState(Hero.CharacterStates.Active);
                    }
                }
            }
        }

        internal void OnHeroComesOfAge(Hero hero)
        {
            DramalordOrphanage.RemoveOrphan(hero.CharacterObject);
        }
    }
}
