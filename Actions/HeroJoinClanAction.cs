using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class HeroJoinClanAction
    {
        internal static void Apply(Hero hero, Clan clan, bool withChildren)
        {
            if (Info.ValidateHeroInfo(hero) && hero.Clan == null)
            {
                hero.Clan = clan;
                hero.UpdateHomeSettlement();
                hero.SetNewOccupation(Occupation.Lord);
                hero.ChangeState(Hero.CharacterStates.Active);

                if (withChildren)
                {
                    foreach (Hero child in hero.Children)
                    {
                        if (child.IsChild)
                        {
                            child.Clan = clan;
                            child.UpdateHomeSettlement();
                            child.SetNewOccupation(Occupation.Lord);
                            child.ChangeState(Hero.CharacterStates.Active);
                        }
                    }
                }

                if (DramalordMCM.Get.ClanOutput)
                    LogEntry.AddLogEntry(new EncyclopediaLogJoinClan(hero, clan));
                DramalordEvents.OnHeroesJoinClan(hero, clan);
            }
        }
    }
}
