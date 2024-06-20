using Dramalord.Conversations;
using Dramalord.Data;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class HeroConversationAction
    {
        internal static bool Apply(Hero hero, Hero target, DramalordTraits heroTraits, List<HeroMemory> memories, List<Hero> potentialWitnesses)
        {
            if(target == Hero.MainHero)
            {
                if(hero.CanInteractWithPlayer() && !hero.IsAngryWith(target) && MBRandom.RandomInt(1,100) <= DramalordMCM.Get.ChanceNPCApproachPlayer && hero.HasMet)
                {
                    bool isLover = hero.IsLover(target);
                    bool isSpouse = hero.IsSpouse(target);
                    bool isAttracted = hero.GetDramalordAttractionTo(target) > DramalordMCM.Get.MinAttractionForFlirting;
                    int emotion = hero.GetDramalordFeelings(target).Emotion;
                    uint daysPassed = (uint)CampaignTime.Now.ToDays - hero.GetDramalordFeelings(target).LastInteractionDay;
                    bool isRelative = (hero.IsDramalordRelativeTo(target) && DramalordMCM.Get.ProtectFamily);

                    if(daysPassed == 0)
                    {
                        return true;
                    }

                    if(!isLover && !isSpouse && isAttracted && !isRelative && emotion < DramalordMCM.Get.MinEmotionForDating)
                    {
                        NpcInteractions.start(hero, EventType.Flirt);
                    }
                    else if(isLover && !isSpouse && emotion >= DramalordMCM.Get.MinEmotionForMarriage)
                    {
                        NpcInteractions.start(hero, EventType.Marriage);
                    }
                    else if(isLover && !isSpouse && emotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                    {
                        NpcInteractions.start(hero, EventType.BreakUp);
                    }
                    else if(isSpouse && emotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                    {
                        NpcInteractions.start(hero, EventType.Divorce);
                    }
                    else if(isLover || isSpouse || emotion >= DramalordMCM.Get.MinEmotionForDating)
                    {
                        NpcInteractions.start(hero, EventType.Date);
                    }
                }
                return true;
            }

            HeroFeelings heroFeelings = hero.GetDramalordFeelings(target);
            HeroFeelings targetFeelings = target.GetDramalordFeelings(hero);
            DramalordTraits targetTraits = target.GetDramalordTraits();

            if(hero.IsAngryWith(target))
            {
                return true;
            }

            // tell secrets
            if (hero.GetRelationTo(target) > DramalordMCM.Get.MinimumRelationLevel)
            {
                memories.Where(item => item.Type == MemoryType.Participant || item.Type == MemoryType.Confession).Do(memory =>
                {
                    target.AddDramalordMemory(memory.EventId, (memory.Type == MemoryType.Participant) ? MemoryType.Confession : MemoryType.Gossip, hero, true);
                    hero.ActivateDramalordMemory(memory.EventId, false);
                    target.ChangeRelationTo(hero, 1);
                    
                    if((memory.Type == MemoryType.Participant))
                    {
                        if (DramalordMCM.Get.GossipOutput)
                        {
                            LogEntry.AddLogEntry(new LogGossipConversation(hero, target, memory.Event.Hero1.HeroObject, memory.Event.Hero2.HeroObject, memory.Event.Type, MemoryType.Confession));
                        }

                        if(target.IsLover(memory.Event.Hero1.HeroObject) || target.IsSpouse(memory.Event.Hero1.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(memory.Event.Hero1.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            target.GetDramalordFeelings(memory.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(memory.Event.Hero2.HeroObject) || target.IsSpouse(memory.Event.Hero2.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(memory.Event.Hero2.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            target.GetDramalordFeelings(memory.Event.Hero2.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                    }
                    else
                    {
                        if (DramalordMCM.Get.GossipOutput)
                        {
                            LogEntry.AddLogEntry(new LogGossipConversation(hero, target, memory.Event.Hero1.HeroObject, memory.Event.Hero2.HeroObject, memory.Event.Type, MemoryType.Gossip));
                        }

                        if (target.IsLover(memory.Event.Hero1.HeroObject) || target.IsSpouse(memory.Event.Hero1.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(memory.Event.Hero1.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            
                            target.GetDramalordFeelings(memory.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(memory.Event.Hero2.HeroObject) || target.IsSpouse(memory.Event.Hero2.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(memory.Event.Hero2.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            target.GetDramalordFeelings(memory.Event.Hero2.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                    }
                    
                });
            }

            // tell gossip
            if (hero.GetRelationTo(target) >= 0)
            {
                memories.Where(item => item.Type == MemoryType.Gossip || item.Type == MemoryType.Witness).Do(item =>
                {
                    if(target.HasDramalordMemory(item.EventId))
                    {
                        return;
                    }

                    target.AddDramalordMemory(item.EventId, MemoryType.Gossip, hero, true);
                    hero.ActivateDramalordMemory(item.EventId, false);
                    target.ChangeRelationTo(hero, 1);

                    if (item.Type == MemoryType.Witness)
                    {
                        if (DramalordMCM.Get.GossipOutput)
                        {
                            LogEntry.AddLogEntry(new LogGossipConversation(hero, target, item.Event.Hero1.HeroObject, item.Event.Hero2.HeroObject, item.Event.Type, MemoryType.Witness));
                        }

                        if (target.IsLover(item.Event.Hero1.HeroObject) || target.IsSpouse(item.Event.Hero1.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(item.Event.Hero1.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            target.GetDramalordFeelings(item.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(item.Event.Hero2.HeroObject) || target.IsSpouse(item.Event.Hero2.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(item.Event.Hero2.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            target.GetDramalordFeelings(item.Event.Hero2.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                    }
                    else
                    {
                        if (DramalordMCM.Get.GossipOutput)
                        {
                            LogEntry.AddLogEntry(new LogGossipConversation(hero, target, item.Event.Hero1.HeroObject, item.Event.Hero2.HeroObject, item.Event.Type, MemoryType.Gossip));
                        }

                        if (target.IsLover(item.Event.Hero1.HeroObject) || target.IsSpouse(item.Event.Hero1.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(item.Event.Hero1.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            target.GetDramalordFeelings(item.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(item.Event.Hero2.HeroObject) || target.IsSpouse(item.Event.Hero2.HeroObject))
                        {
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(item.Event.Hero2.HeroObject, (DramalordMCM.Get.EmotionalLossGossip / 2) * -1);
                            }
                            
                            target.GetDramalordFeelings(item.Event.Hero2.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                    }
                });
            }

            //check player secrets
            //TODO

            if(hero.IsDramalordRelativeTo(target) && DramalordMCM.Get.ProtectFamily)
            {
                return true;
            }

            // flirt
            if(hero.GetDramalordAttractionTo(target) >= DramalordMCM.Get.MinAttractionForFlirting)
            {
                HeroFlirtAction.Apply(hero, target, potentialWitnesses);
            }

            bool isCoupleTension = heroFeelings.Tension >= 50 && targetFeelings.Tension >= 50;
            bool isCoupleHorny = heroTraits.Horny >= DramalordMCM.Get.MinHornyForIntercourse && targetTraits.Horny >= DramalordMCM.Get.MinHornyForIntercourse;
            bool isCoupleNoHate = heroFeelings.Emotion >= 0 && targetFeelings.Emotion >= 0;
            bool isCoupleLove = heroFeelings.Emotion >= DramalordMCM.Get.MinEmotionForDating && targetFeelings.Emotion >= DramalordMCM.Get.MinEmotionForDating;
            bool isCoupleMarryLove = heroFeelings.Emotion >= DramalordMCM.Get.MinEmotionForMarriage && targetFeelings.Emotion >= DramalordMCM.Get.MinEmotionForMarriage;
            bool isCoupleTrusty = hero.GetRelationTo(target) >= DramalordMCM.Get.MinimumRelationLevel && target.GetRelationTo(hero) >= DramalordMCM.Get.MinimumRelationLevel;

            bool isCoupleFwb = target.IsFriendWithBenefits(hero);
            bool isCoupleLover = target.IsLover(hero);
            bool isCoupleMarried = target.IsSpouse(hero);

            if(isCoupleTension && isCoupleHorny && isCoupleNoHate && !isCoupleLove && !isCoupleFwb && !isCoupleLover && !isCoupleMarried)
            {
                hero.SetFriendWithBenefits(target);
                isCoupleFwb = true;
                if(DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogStartFWB(hero, target));
                }
            }
            else if(isCoupleLove && !isCoupleLover && !isCoupleMarried)
            {
                hero.SetLover(target);
                isCoupleLover = true;
                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogStartAffair(hero, target));
                }
            }
            else if(isCoupleMarryLove && isCoupleTrusty && isCoupleLover && DramalordMCM.Get.AllowMarriages)
            {
                HeroMarriageAction.Apply(hero, target, potentialWitnesses);
                isCoupleMarried = true;
            }

            if(isCoupleLover || isCoupleMarried)
            {
                HeroDateAction.Apply(hero, target, hero.GetCloseHeroes());
            }

            if(isCoupleHorny && (isCoupleFwb || isCoupleLover || isCoupleMarried))
            {
                HeroIntercourseAction.Apply(hero, target, potentialWitnesses);
                if(hero.IsFemale != target.IsFemale && MBRandom.RandomInt(1,100) <= DramalordMCM.Get.PregnancyChance)
                {
                    HeroConceiveAction.Apply(hero.IsFemale ? hero : target, hero.IsFemale ? target : hero);
                }
            }

            return true;
        }
    }
}
