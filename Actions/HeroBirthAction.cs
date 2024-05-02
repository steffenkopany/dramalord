using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Dramalord.Actions
{
    internal static class HeroBirthAction
    {
        internal static void Apply(Hero hero, HeroOffspringData offspring)
        {
            CharacterObject template = (MBRandom.RandomInt(1, 100) > 50) ? hero.CharacterObject : offspring.Father.CharacterObject;
            Settlement bornSettlement = (hero.CurrentSettlement != null) ? hero.CurrentSettlement : hero.HomeSettlement;
            if (bornSettlement == null)
            {
                bornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }
            Clan faction = hero.Clan;
            Hero child = HeroCreator.CreateSpecialHero(template, bornSettlement, faction, null, 0);
            child.Mother = hero;
            child.Father = offspring.Father;
            hero.HeroDeveloper.InitializeHeroDeveloper(isByNaturalGrowth: true);
            BodyProperties bodyProperties = hero.BodyProperties;
            BodyProperties bodyProperties2 = offspring.Father.BodyProperties;
            int seed = MBRandom.RandomInt();
            string hairTags = (child.IsFemale ? hero.HairTags : offspring.Father.HairTags);
            string tattooTags = (child.IsFemale ? hero.TattooTags : offspring.Father.TattooTags);
            child.ModifyPlayersFamilyAppearance(BodyProperties.GetRandomBodyProperties(template.Race, child.IsFemale, bodyProperties, bodyProperties2, 1, seed, hairTags, offspring.Father.BeardTags, tattooTags).StaticProperties);
   
            child.SetNewOccupation(hero.Occupation);
            //child.UpdateHomeSettlement();

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
