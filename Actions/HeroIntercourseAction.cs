using Dramalord.Data;
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
    internal static class HeroIntercourseAction
    {
        internal static void Apply(Hero hero, Hero target, List<Hero> closeHeroes)
        {
            if(hero.IsDramalordLegit() && target.IsDramalordLegit())
            {
                hero.GetHeroTraits().SetPropertyValue(HeroTraits.IntercourseSkill, hero.GetDramalordTraits().IntercourseSkill + 1);
                target.GetHeroTraits().SetPropertyValue(HeroTraits.IntercourseSkill, target.GetDramalordTraits().IntercourseSkill + 1);
                hero.GetHeroTraits().SetPropertyValue(HeroTraits.Horny, hero.GetDramalordTraits().Horny - DramalordMCM.Get.HornyLossIntercourse);
                target.GetHeroTraits().SetPropertyValue(HeroTraits.Horny, target.GetDramalordTraits().Horny - DramalordMCM.Get.HornyLossIntercourse);

                hero.GetDramalordFeelings(target).Tension -= DramalordMCM.Get.TensionLossIntercourse;
                target.GetDramalordFeelings(hero).Tension -= DramalordMCM.Get.TensionLossIntercourse;

                if (target == Hero.MainHero || hero == Hero.MainHero)
                {
                    Hero otherHero = (hero == Hero.MainHero) ? target : hero;
          
                    TextObject banner = new TextObject("{=Dramalord141}You were intimate with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
                }

                Hero mother = hero.IsFemale ? hero : target;
                Hero father = target.IsFemale ? hero : target;

                if (mother != father && mother.Spouse == father && mother.IsPregnant)
                {
                    HeroPregnancy? offspring = target.GetDramalordPregnancy();
                    if (offspring != null && offspring.Father != father.CharacterObject && CampaignTime.Now.ToDays - offspring.Conceived < DramalordMCM.Get.DaysUntilPregnancyVisible)
                    {
                        offspring.Father = father.CharacterObject;
                        TextObject banner = new TextObject("{=Dramalord142}{HERO.LINK} tricked {SPOUSE.LINK} being the cause of her pregancy.");
                        StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("SPOUSE", father.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                }

                int eventID = DramalordEvents.AddHeroEvent(hero, target, EventType.Intercourse, DramalordMCM.Get.IntercourseMemoryDuration);
                hero.AddDramalordMemory(eventID, MemoryType.Participant, hero, true);
                target.AddDramalordMemory(eventID, MemoryType.Participant, target, true);

                if(DramalordMCM.Get.AffairOutput &&  (hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan || !DramalordMCM.Get.OnlyPlayerClanOutput))
                {
                    LogEntry.AddLogEntry(new LogIntercourse(hero, target));
                }
                
                DramalordEventCallbacks.OnHeroesIntercourse(hero, target);

                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                {
                    Hero? witness = closeHeroes.Where(item => item != hero && item != target).GetRandomElementInefficiently();
                    if (witness != null)
                    {
                        witness.AddDramalordMemory(eventID, MemoryType.Witness, witness, true);

                        if (DramalordMCM.Get.AffairOutput && (hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan || !DramalordMCM.Get.OnlyPlayerClanOutput))
                        {
                            LogEntry.AddLogEntry(new LogWitnessIntercourse(hero, target, witness));
                        }
                        

                        if (witness == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord268}You caught {HERO.LINK} being intimate with {TARGET.LINK}");
                            StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                        }
                        else if (hero == Hero.MainHero || target == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord269}{HERO.LINK} caught you being intimate with {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                        }

                        if (witness.IsSpouse(hero) || witness.IsLover(hero))
                        {
                            if (!witness.GetDramalordPersonality().AcceptsOtherIntercourse)
                            {
                                int emotionChange = witness.GetDramalordPersonality().GetEmotionalChange(EventType.Intercourse);
                                HeroFeelings witnessFeelings = witness.GetDramalordFeelings(hero);
                                witnessFeelings.Emotion += emotionChange;
                                if (DramalordMCM.Get.LinkEmotionToRelation)
                                {
                                    witness.ChangeRelationTo(hero, (emotionChange / 2));
                                }
                                witness.MakeAngryWith(hero, DramalordMCM.Get.AngerDaysIntercourse);
                            }
                        }
                        else if (witness.IsSpouse(target) || witness.IsLover(target))
                        {
                            if (!witness.GetDramalordPersonality().AcceptsOtherIntercourse)
                            {
                                int emotionChange = witness.GetDramalordPersonality().GetEmotionalChange(EventType.Intercourse);
                                HeroFeelings witnessFeelings = witness.GetDramalordFeelings(target);
                                witnessFeelings.Emotion += emotionChange;
                                if (DramalordMCM.Get.LinkEmotionToRelation)
                                {
                                    witness.ChangeRelationTo(target, (emotionChange / 2));
                                }
                                witness.MakeAngryWith(target, DramalordMCM.Get.AngerDaysIntercourse);
                            } 
                        }
                    }
                }
            }
        }
    }
}
