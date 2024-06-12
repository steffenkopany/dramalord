using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Patches;
using Dramalord.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Dramalord.Actions
{
    internal static class NPCConversationAction
    {
        internal static bool Apply(Hero hero, Hero target, DramalordTraits heroTraits, List<HeroMemory> memories, List<Hero> potentialWitnesses)
        {
            if(target == Hero.MainHero)
            {
                if(MBRandom.RandomInt(1,100) <= DramalordMCM.Get.ChanceNPCApproachPlayer)
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

                    if(!isLover && !isSpouse && isAttracted && !isRelative)
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
                    else if(isLover || isSpouse)
                    {
                        NpcInteractions.start(hero, EventType.Date);
                    }
                }
                return true;
            }

            HeroFeelings heroFeelings = hero.GetDramalordFeelings(target);
            HeroFeelings targetFeelings = target.GetDramalordFeelings(hero);
            DramalordTraits targetTraits = target.GetDramalordTraits();

            // tell secrets
            if (heroFeelings.Trust > DramalordMCM.Get.MinimumTrustLevel)
            {
                memories.Where(item => item.Type == MemoryType.Participant || item.Type == MemoryType.Confession).Do(memory =>
                {
                    target.AddDramalordMemory(memory.EventId, (memory.Type == MemoryType.Participant) ? MemoryType.Confession : MemoryType.Gossip, hero, true);
                    hero.ActivateDramalordMemory(memory.EventId, false);
                    target.SetPersonalRelation(hero, target.GetBaseHeroRelation(hero) + 1);
                    targetFeelings.Trust += 1;
                    if((memory.Type == MemoryType.Participant))
                    {
                        if (DramalordMCM.Get.GossipOutput)
                        {
                            LogEntry.AddLogEntry(new LogGossipConversation(hero, target, memory.Event.Hero1.HeroObject, memory.Event.Hero2.HeroObject, memory.Event.Type, MemoryType.Confession));
                        }

                        if(target.IsLover(memory.Event.Hero1.HeroObject) || target.IsSpouse(memory.Event.Hero1.HeroObject))
                        {
                            target.GetDramalordFeelings(memory.Event.Hero1.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
                            target.GetDramalordFeelings(memory.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(memory.Event.Hero2.HeroObject) || target.IsSpouse(memory.Event.Hero2.HeroObject))
                        {
                            target.GetDramalordFeelings(memory.Event.Hero2.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
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
                            target.GetDramalordFeelings(memory.Event.Hero1.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
                            target.GetDramalordFeelings(memory.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(memory.Event.Hero2.HeroObject) || target.IsSpouse(memory.Event.Hero2.HeroObject))
                        {
                            target.GetDramalordFeelings(memory.Event.Hero2.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
                            target.GetDramalordFeelings(memory.Event.Hero2.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                    }
                    
                });
            }

            // tell gossip
            if (heroFeelings.Trust >= 0)
            {
                memories.Where(item => item.Type == MemoryType.Gossip || item.Type == MemoryType.Witness).Do(item =>
                {
                    if(target.HasDramalordMemory(item.EventId))
                    {
                        return;
                    }

                    target.AddDramalordMemory(item.EventId, MemoryType.Gossip, hero, true);
                    hero.ActivateDramalordMemory(item.EventId, false);
                    target.SetPersonalRelation(hero, target.GetBaseHeroRelation(hero) + 1);
                    targetFeelings.Trust += 1;
                    if(item.Type == MemoryType.Witness)
                    {
                        if (DramalordMCM.Get.GossipOutput)
                        {
                            LogEntry.AddLogEntry(new LogGossipConversation(hero, target, item.Event.Hero1.HeroObject, item.Event.Hero2.HeroObject, item.Event.Type, MemoryType.Witness));
                        }

                        if (target.IsLover(item.Event.Hero1.HeroObject) || target.IsSpouse(item.Event.Hero1.HeroObject))
                        {
                            target.GetDramalordFeelings(item.Event.Hero1.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
                            target.GetDramalordFeelings(item.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(item.Event.Hero2.HeroObject) || target.IsSpouse(item.Event.Hero2.HeroObject))
                        {
                            target.GetDramalordFeelings(item.Event.Hero2.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
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
                            target.GetDramalordFeelings(item.Event.Hero1.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
                            target.GetDramalordFeelings(item.Event.Hero1.HeroObject).Emotion -= DramalordMCM.Get.EmotionalLossGossip;
                        }
                        else if (target.IsLover(item.Event.Hero2.HeroObject) || target.IsSpouse(item.Event.Hero2.HeroObject))
                        {
                            target.GetDramalordFeelings(item.Event.Hero2.HeroObject).Trust -= DramalordMCM.Get.EmotionalLossGossip;
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
            bool isCoupleTrusty = heroFeelings.Trust >= DramalordMCM.Get.MinimumTrustLevel && heroFeelings.Trust >= DramalordMCM.Get.MinimumTrustLevel;

            bool isCoupleFwb = target.IsFriendWithBenefits(hero);
            bool isCoupleLover = target.IsLover(hero);
            bool isCoupleMarried = target.IsSpouse(hero);

            if(isCoupleTension && isCoupleHorny && isCoupleNoHate && !isCoupleLove && !isCoupleFwb && !isCoupleLover && !isCoupleMarried)
            {
                hero.SetFriendWithBenefits(target);
                Notification.PrintText(hero.Name + " and " + target.Name + " are now friends with benefits");
                isCoupleFwb = true;
            }
            else if(isCoupleLove && !isCoupleLover && !isCoupleMarried)
            {
                hero.SetLover(target);
                LogEntry.AddLogEntry(new EncyclopediaLogStartAffair(hero, target));
                isCoupleLover = true;
            }
            else if(isCoupleMarryLove && isCoupleTrusty && isCoupleLover)
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
                    ChildConceivedPatch.Father = hero.IsFemale ? target.CharacterObject : hero.CharacterObject;
                    //MakePregnantAction.Apply(hero.IsFemale ? hero : target);
                    Hero mother = hero.IsFemale ? hero : target;
                    ChildConceivedPatch.ChildConceived(ref mother);
                }
            }

            return true;
        }
    }
}
