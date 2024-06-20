using Dramalord.Data;
using HarmonyLib;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroDateAction
    {
        internal static void Apply(Hero hero, Hero target, List<Hero> closeHeroes)
        {
            if(hero.IsDramalordLegit() && target.IsDramalordLegit())
            {
                HeroFeelings heroFeelings = hero.GetDramalordFeelings(target);
                HeroFeelings targetFeeling = target.GetDramalordFeelings(hero);

                int heroAttractionScore = hero.GetDramalordAttractionTo(target) / 10;
                int targetAttractionScore = target.GetDramalordAttractionTo(hero) / 10;

                hero.GetHeroTraits().SetPropertyValue(HeroTraits.Horny, hero.GetDramalordTraits().Horny + heroAttractionScore);
                target.GetHeroTraits().SetPropertyValue(HeroTraits.Horny, target.GetDramalordTraits().Horny + targetAttractionScore);

                heroFeelings.Tension += heroAttractionScore;
                targetFeeling.Tension += targetAttractionScore;

                int heroTraitScore = hero.GetDramalordTraitScore(target);
                int targetTraitScore = target.GetDramalordTraitScore(hero);

                heroFeelings.Emotion += heroTraitScore;
                targetFeeling.Emotion += targetTraitScore;

                hero.ChangeRelationTo(target, (((heroTraitScore > targetTraitScore) ? targetTraitScore : heroTraitScore) / 2));

                if (hero != Hero.MainHero)
                {
                    hero.GetHeroSpouses().Where(item => item != target.CharacterObject).Do(item =>
                    {
                        HeroFeelings itemFeelings = hero.GetDramalordFeelings(item.HeroObject);
                        itemFeelings.Tension -= heroAttractionScore / 2;
                        itemFeelings.Emotion -= heroTraitScore / 2;
                    });
                    hero.GetHeroLovers().Where(item => item != target.CharacterObject).Do(item =>
                    {
                        HeroFeelings itemFeelings = hero.GetDramalordFeelings(item.HeroObject);
                        itemFeelings.Tension -= heroAttractionScore / 2;
                        itemFeelings.Emotion -= heroTraitScore / 2;
                    });
                }
                
                if(target != Hero.MainHero)
                {
                    target.GetHeroSpouses().Where(item => item != hero.CharacterObject).Do(item =>
                    {
                        HeroFeelings itemFeelings = target.GetDramalordFeelings(item.HeroObject);
                        itemFeelings.Tension -= targetAttractionScore / 2;
                        itemFeelings.Emotion -= targetTraitScore / 2;
                    });
                    target.GetHeroLovers().Where(item => item != hero.CharacterObject).Do(item =>
                    {
                        HeroFeelings itemFeelings = target.GetDramalordFeelings(item.HeroObject);
                        itemFeelings.Tension -= targetAttractionScore / 2;
                        itemFeelings.Emotion -= targetTraitScore / 2;
                    });
                }

                heroFeelings.LastInteractionDay = (uint)CampaignTime.Now.ToDays;
                targetFeeling.LastInteractionDay = (uint)CampaignTime.Now.ToDays;

                if (target == Hero.MainHero || hero == Hero.MainHero)
                {
                    Hero otherHero = (hero == Hero.MainHero) ? target : hero;

                    TextObject banner = new TextObject("{=Dramalord256}You met in private with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
                }

                int eventID = DramalordEvents.AddHeroEvent(hero, target, EventType.Date, DramalordMCM.Get.DateMemoryDuration);
                hero.AddDramalordMemory(eventID, MemoryType.Participant, hero, true);
                target.AddDramalordMemory(eventID, MemoryType.Participant, target, true);

                if(DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new LogAffairMeeting(hero, target));
                }
                
                DramalordEventCallbacks.OnHeroesAffairMeeting(hero, target);

                if(hero.IsPregnant)
                {
                    HeroPregnancy? pregnancy = hero.GetDramalordPregnancy();
                    if(pregnancy != null && pregnancy.Father != target.CharacterObject && (uint)CampaignTime.Now.ToDays - pregnancy.Conceived >= DramalordMCM.Get.DaysUntilPregnancyVisible && !target.HasDramalordMemory(pregnancy.EventID))
                    {
                        target.AddDramalordMemory(pregnancy.EventID, MemoryType.Witness, target, true);
                        LogEntry.AddLogEntry(new LogWitnessPregnancy(hero, target));

                        if (target == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord270}You noticed {HERO.LINK} is pregnant from someone else");
                            StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                        }
                        else if (hero == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord271}{HERO.LINK} noticed you are pregnant from someone else.");
                            StringHelpers.SetCharacterProperties("HERO", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, target.CharacterObject, "event:/ui/notification/relation");
                        }

                        if (!target.GetDramalordTraits().IsEmotionallyOpen)
                        {
                            targetFeeling.Emotion -= DramalordMCM.Get.EmotionalLossPregnancy;
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                target.ChangeRelationTo(hero, (DramalordMCM.Get.EmotionalLossPregnancy / 2) * -1);
                            }

                            target.MakeAngryWith(hero, DramalordMCM.Get.AngerDaysPregnancy);
                        }
                    }
                }

                if (target.IsPregnant)
                {
                    HeroPregnancy? pregnancy = target.GetDramalordPregnancy();
                    if (pregnancy != null && pregnancy.Father != hero.CharacterObject && (uint)CampaignTime.Now.ToDays - pregnancy.Conceived >= DramalordMCM.Get.DaysUntilPregnancyVisible && !hero.HasDramalordMemory(pregnancy.EventID))
                    {
                        hero.AddDramalordMemory(pregnancy.EventID, MemoryType.Witness, hero, true);
                        LogEntry.AddLogEntry(new LogWitnessPregnancy(target, hero));

                        if (hero == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord270}You noticed {HERO.LINK} is pregnant from someone else");
                            StringHelpers.SetCharacterProperties("HERO", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, target.CharacterObject, "event:/ui/notification/relation");
                        }
                        else if (target == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord271}{HERO.LINK} noticed you are pregnant from someone else.");
                            StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                        }

                        if (!hero.GetDramalordTraits().IsEmotionallyOpen)
                        {
                            heroFeelings.Emotion -= DramalordMCM.Get.EmotionalLossPregnancy;
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                hero.ChangeRelationTo(target, (DramalordMCM.Get.EmotionalLossPregnancy / 2) * -1);
                            }

                            hero.MakeAngryWith(target, DramalordMCM.Get.AngerDaysPregnancy);
                        }   
                    }
                }

                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                {
                    Hero? witness = closeHeroes.Where(item => item != hero && item != target).GetRandomElementInefficiently();
                    if (witness != null)
                    {
                        witness.AddDramalordMemory(eventID, MemoryType.Witness, witness, true);
                        LogEntry.AddLogEntry(new LogWitnessFlirt(hero, target, witness));

                        if (witness == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord266}You saw {HERO.LINK} meeting in secret with {TARGET.LINK}");
                            StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                        }
                        else if (hero == Hero.MainHero || target == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord267}{HERO.LINK} saw you meeting in secret with {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, witness.CharacterObject, "event:/ui/notification/relation");
                        }

                        if (witness.IsSpouse(hero) || witness.IsLover(hero))
                        {
                            if (!witness.GetDramalordTraits().IsEmotionallyOpen)
                            {
                                HeroFeelings witnessFeelings = witness.GetDramalordFeelings(hero);
                                witnessFeelings.Emotion -= DramalordMCM.Get.EmotionalLossCaughtDate;
                                if (DramalordMCM.Get.LinkEmotionToRelation)
                                {
                                    witness.ChangeRelationTo(hero, (DramalordMCM.Get.EmotionalLossCaughtDate / 2) * -1);
                                }

                                witness.MakeAngryWith(hero, DramalordMCM.Get.AngerDaysDate);
                            } 
                        }
                        else if (witness.IsSpouse(target) || witness.IsLover(target))
                        {
                            if (!witness.GetDramalordTraits().IsEmotionallyOpen)
                            {
                                HeroFeelings witnessFeelings = witness.GetDramalordFeelings(target);
                                witnessFeelings.Emotion -= DramalordMCM.Get.EmotionalLossCaughtDate;
                                if (DramalordMCM.Get.LinkEmotionToRelation)
                                {
                                    witness.ChangeRelationTo(target, (DramalordMCM.Get.EmotionalLossCaughtDate / 2) * -1);
                                }
                                
                                witness.MakeAngryWith(target, DramalordMCM.Get.AngerDaysDate);
                            }  
                        }
                    }
                }
            }
        }
    }
}
