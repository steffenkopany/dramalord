using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class HeroJoinClanAction
    {
        internal static void Apply(Hero hero, Clan clan)
        {
            if (hero.Occupation == Occupation.Wanderer)
            {
                hero.SetName(hero.FirstName, hero.FirstName);
            }

            hero.Clan = clan;
            hero.UpdateHomeSettlement();
            hero.SetNewOccupation(Occupation.Lord);
            hero.ChangeState(Hero.CharacterStates.Active);

            foreach (Hero child in hero.Children)
            {
                if (child.IsChild && child.Clan == null)
                {
                    child.Clan = clan;
                    child.UpdateHomeSettlement();
                    child.SetNewOccupation(Occupation.Lord);
                    child.ChangeState(Hero.CharacterStates.Active);
                }
            }

            if (DramalordMCM.Get.ClanOutput)
            {
                LogEntry.AddLogEntry(new EncyclopediaLogJoinClan(hero, clan));
            }
                
            DramalordEventCallbacks.OnHeroesJoinClan(hero, clan);
        }
    }
}
