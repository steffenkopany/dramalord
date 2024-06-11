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
            hero.ClearAllRelationships(target);

            hero.GetDramalordFeelings(target).Emotion -= DramalordMCM.Get.EmotionalLossBreakup;
            target.GetDramalordFeelings(hero).Emotion -= DramalordMCM.Get.EmotionalLossBreakup;

            if (target == Hero.MainHero)
            {
                TextObject textObject = new TextObject("{=Dramalord132}{HERO.LINK} ended the affair with you.");
                StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
            }
            else if(hero == Hero.MainHero)
            {
                TextObject textObject = new TextObject("{=Dramalord317}You broke up with {HERO.LINK}");
                StringHelpers.SetCharacterProperties("HERO", target.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
            }

            if (DramalordMCM.Get.AffairOutput)
            {
                LogEntry.AddLogEntry(new EncyclopediaLogBreakup(hero, target));
            }

            DramalordEventCallbacks.OnHeroesBreakup(hero, target);
        }
        
    }
}
