using Dramalord.Data;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroDivorceAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if(Info.ValidateHeroMemory(hero, target))
            {
                Info.SetIsCoupleWithHero(hero, target, false);
                Info.ChangeEmotionToHeroBy(target, hero, -DramalordMCM.Get.EmotionalLossDivorce);

                hero.ExSpouses.Remove(target);
                target.ExSpouses.Remove(hero);

                foreach (Romance.RomanticState romanticState in Romance.RomanticStateList.ToList())
                {
                    if ((romanticState.Person1 == target && romanticState.Person2 == hero) || (romanticState.Person1 == hero && romanticState.Person2 == target))
                    {
                        romanticState.Level = Romance.RomanceLevelEnum.FailedInPracticalities;
                    }
                }
                hero.Spouse = null;
                target.Spouse = null;

                if (target == Hero.MainHero)
                {
                    TextObject textObject = new TextObject("{=Dramalord131}{HERO.LINK} divorced from you.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                }
                else if (hero == Hero.MainHero)
                {
                    TextObject textObject = new TextObject("{=Dramalord318}You divorced from {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", target.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                }

                if (DramalordMCM.Get.MarriageOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogDivorce(hero, target));
                }

                DramalordEvents.OnHeroesDivorced(hero, target);
            } 
        }
    }
}
