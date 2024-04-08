using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroToyAction
    {
        internal static void Apply(Hero hero)
        {
            if(Info.ValidateHeroInfo(hero))
            {
                bool broke = false;
                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ToyBreakChance)
                {
                    Info.SetHeroHasToy(hero, false);
                    TextObject textObject = new TextObject("{=Dramalord130}{HERO.LINK} played with their toy.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");

                    broke = true;
                }

                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new LogUsedToy(hero));
                }

                DramalordEvents.OnHeroesUsedToy(hero, broke);
            }  
        }
    }
}
