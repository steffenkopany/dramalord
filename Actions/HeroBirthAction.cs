using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class HeroBirthAction
    {
        internal static void Apply(Hero hero, HeroOffspringData offspring)
        {
            if (Info.ValidateHeroMemory(hero, offspring.Father))
            {
                Hero child = HeroCreator.DeliverOffSpring(hero, offspring.Father, MBRandom.RandomInt(1, 100) > 50);
                child.Clan = hero.Clan;

                Info.RemoveHeroOffspring(hero);

                if (child.Father == Hero.MainHero || hero == Hero.MainHero)
                {
                    MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(child.Father, hero, CampaignTime.Now));
                }
                else if (hero == Hero.MainHero.Spouse)
                {
                    MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(child.Father, hero, CampaignTime.Now));
                }

                if (DramalordMCM.Get.BirthOutput)
                    LogEntry.AddLogEntry(new EncyclopediaLogBirth(hero, offspring.Father, child));
                DramalordEvents.OnHeroesBorn(hero, offspring.Father, child);
            }
        }
    }
}
