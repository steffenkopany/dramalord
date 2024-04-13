using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class HeroBirthAction
    {
        internal static void Apply(Hero hero, HeroOffspringData offspring)
        {
            CharacterObject template = (MBRandom.RandomInt(1, 100) > 50) ? hero.CharacterObject : offspring.Father.CharacterObject;
            Settlement bornSettlement = (hero.CurrentSettlement != null) ? hero.CurrentSettlement : hero.HomeSettlement;
            Clan faction = hero.Clan;
            Hero child = HeroCreator.CreateSpecialHero(template, bornSettlement, faction, null, 0);
            child.Mother = hero;
            child.Father = offspring.Father;

            if (child.BornSettlement == null)
            {
                child.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }

            hero.SetNewOccupation(hero.Occupation);
            hero.UpdateHomeSettlement();

            Info.RemoveHeroOffspring(hero);

            if (child.Father == Hero.MainHero || hero == Hero.MainHero || hero == Hero.MainHero.Spouse)
            {
                MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(child.Father, hero, CampaignTime.Now));
            }
            
            if (DramalordMCM.Get.BirthOutput)
            {
                LogEntry.AddLogEntry(new EncyclopediaLogBirth(hero, offspring.Father, child));
            }
                
            DramalordEvents.OnHeroesBorn(hero, offspring.Father, child);
        }
    }
}
