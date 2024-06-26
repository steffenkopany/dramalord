using Dramalord.Data;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroFlirtAction
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

                target.ChangeRelationTo(hero, (((heroTraitScore > targetTraitScore) ? targetTraitScore : heroTraitScore) / 2));

                heroFeelings.LastInteractionDay = (uint)CampaignTime.Now.ToDays;
                targetFeeling.LastInteractionDay = (uint)CampaignTime.Now.ToDays;

                if (target == Hero.MainHero || hero == Hero.MainHero)
                {
                    Hero otherHero = (hero == Hero.MainHero) ? target : hero;

                    TextObject banner = new TextObject("{=Dramalord257}You flirted with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
                }

                if(DramalordMCM.Get.FlirtOutput && (hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan || !DramalordMCM.Get.OnlyPlayerClanOutput))
                {
                    LogEntry.AddLogEntry(new LogFlirt(hero, target));
                }

                DramalordEventCallbacks.OnHeroesFlirt(hero, target);

                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                {
                    Hero? witness = closeHeroes.Where(item => item != hero && item != target).GetRandomElementInefficiently();
                    if(witness != null)
                    {
                        if(DramalordMCM.Get.FlirtOutput && (hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan || !DramalordMCM.Get.OnlyPlayerClanOutput))
                        {
                            LogEntry.AddLogEntry(new LogWitnessFlirt(hero, target, witness));
                        }

                        if (witness == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord264}You saw {HERO.LINK} flirting with {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                        }
                        else if (hero == Hero.MainHero || target == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord265}{HERO.LINK} saw you flirting with {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 1000, witness.CharacterObject, "event:/ui/notification/relation");
                        }

                        if(witness.IsSpouse(hero) || witness.IsLover(hero))
                        {
                            if (!witness.GetDramalordPersonality().AcceptsOtherFlirts)
                            {
                                int emotionChange = witness.GetDramalordPersonality().GetEmotionalChange(EventType.Flirt);
                                HeroFeelings witnessFeelings = witness.GetDramalordFeelings(hero);
                                witnessFeelings.Emotion += emotionChange;
                                if(DramalordMCM.Get.LinkEmotionToRelation)
                                {
                                    witness.ChangeRelationTo(hero, (emotionChange / 2));
                                }
                                
                                witness.MakeAngryWith(hero, DramalordMCM.Get.AngerDaysFlirt);
                            }
                                
                        }
                        else if (witness.IsSpouse(target) || witness.IsLover(target))
                        {
                            if (!witness.GetDramalordPersonality().AcceptsOtherFlirts)
                            {
                                int emotionChange = witness.GetDramalordPersonality().GetEmotionalChange(EventType.Flirt);
                                HeroFeelings witnessFeelings = witness.GetDramalordFeelings(target);
                                witnessFeelings.Emotion += emotionChange;
                                if (DramalordMCM.Get.LinkEmotionToRelation)
                                {
                                    witness.ChangeRelationTo(target, (emotionChange / 2));
                                }
                                    
                                witness.MakeAngryWith(target, DramalordMCM.Get.AngerDaysFlirt);
                            }
                        }
                    }
                }
            }
        }
    }  
}
