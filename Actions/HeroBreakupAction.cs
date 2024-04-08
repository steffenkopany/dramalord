using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroBreakupAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if(Info.ValidateHeroMemory(hero, target))
            {
                Info.SetIsCoupleWithHero(hero, target, false);
                Info.ChangeEmotionToHeroBy(target, hero, -DramalordMCM.Get.EmotionalLossBreakup);

                if (target == Hero.MainHero)
                {
                    TextObject textObject = new TextObject("{=Dramalord132}{HERO.LINK} ended the affair with you.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                }

                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogBreakup(hero, target));
                }

                DramalordEvents.OnHeroesBreakup(hero, target);
            }
        }
    }
}
