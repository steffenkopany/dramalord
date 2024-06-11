using Dramalord.Data;
using Dramalord.Data.Deprecated;
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
            if(hero.IsDramalordLegit())
            {
                bool broke = false;
                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ToyBreakChance)
                {
                    hero.GetHeroTraits().SetPropertyValue(HeroTraits.HasToy, 0);
                    TextObject textObject = new TextObject("{=Dramalord130}{HERO.LINK}s toy broke!");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");

                    broke = true;
                }

                hero.GetDramalordFeelings(Hero.MainHero).Emotion += 1;
                hero.GetHeroTraits().SetPropertyValue(HeroTraits.Horny, hero.GetDramalordTraits().Horny + 1);
                if (hero.Spouse != null && hero.Spouse != Hero.MainHero)
                {
                    hero.GetDramalordFeelings(hero.Spouse).Emotion -= 1;
                }

                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new LogUsedToy(hero));
                }

                DramalordEventCallbacks.OnHeroesUsedToy(hero, broke);
            }  
        }
    }
}
