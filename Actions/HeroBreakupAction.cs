using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Dramalord.Actions
{
    internal static class HeroBreakupAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            hero.ClearAllRelationships(target);

            HeroFeelings heroFeelings = hero.GetDramalordFeelings(target);
            HeroFeelings targetFeelings = target.GetDramalordFeelings(hero);

            targetFeelings.Emotion -= DramalordMCM.Get.EmotionalLossBreakup; 
            if (DramalordMCM.Get.LinkEmotionToRelation)
            {
                target.ChangeRelationTo(hero, (DramalordMCM.Get.EmotionalLossBreakup / 2) * -1);
            }
            target.MakeAngryWith(hero, DramalordMCM.Get.AngerDaysDate);

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

            if (DramalordMCM.Get.AffairOutput && (hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan || !DramalordMCM.Get.OnlyPlayerClanOutput))
            {
                LogEntry.AddLogEntry(new EncyclopediaLogBreakup(hero, target));
            }

            DramalordEventCallbacks.OnHeroesBreakup(hero, target);
        }
        
    }
}
