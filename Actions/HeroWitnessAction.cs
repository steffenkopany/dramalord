using Dramalord.Data;
using Dramalord.Data.Deprecated;
using Dramalord.UI;
using Helpers;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroWitnessAction
    {
        internal static void Apply(Hero hero, Hero target, Hero witness, EventType type, int eventID)
        {
            if(witness.IsDramalordLegit())
            {
                witness.AddDramalordMemory(eventID, MemoryType.Witness, witness, true);

                if (type == EventType.Flirt)
                {
                    if (DramalordMCM.Get.FlirtOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessFlirt(hero, target, witness));
                    }

                    if(witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord264}You saw {HERO.LINK} flirting with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (hero == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord265}{HERO.LINK} saw you flirting with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
                else if (type == EventType.Date)
                {
                    if (DramalordMCM.Get.AffairOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessDate(hero, target, witness));
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord266}You saw {HERO.LINK} meeting in secret with {TARGET.LINK}");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (hero == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord267}{HERO.LINK} saw you meeting in secret with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
                else if (type == EventType.Intercourse)
                {
                    witness.GetDramalordFeelings(hero).Emotion -= DramalordMCM.Get.EmotionalLossCaughtIntercourse;
                    if (DramalordMCM.Get.AffairOutput)
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
                    else if (hero == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord269}{HERO.LINK} caught you being intimate with {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                }
                else if (type == EventType.Pregnancy)
                {
                    witness.GetDramalordFeelings(hero).Emotion -= DramalordMCM.Get.EmotionalLossPregnancy;
                    if (DramalordMCM.Get.AffairOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessPregnancy(hero, witness));
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord270}You noticed {HERO.LINK} is pregnant from someone else");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (hero == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord271}{HERO.LINK} noticed you are pregnant from someone else.");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                }
                else if (type == EventType.Birth)
                {
                    witness.GetDramalordFeelings(hero).Emotion -= DramalordMCM.Get.EmotionalLossCaughtFlirting;
                    if (DramalordMCM.Get.BirthOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessBastard(hero, target, witness));
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord272}You noticed that {TARGET.LINK} born by {HERO.LINK} is not your child");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (hero == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord273}{HERO.LINK} noticed your child {TARGET.LINK} is not theirs.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
            }
            
            DramalordEventCallbacks.OnHeroesWitness(hero, target, witness, type);  
        }
    }
}
