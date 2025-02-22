using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal static class JoinClanAction
    {
        internal static void Apply(Hero hero, Clan clan)
        {
            if (hero.Occupation != Occupation.Lord)
            {
                hero.SetName(hero.FirstName, hero.FirstName);
            }

            hero.SetNewOccupation(Occupation.Lord);
            hero.Clan = clan;
            hero.UpdateHomeSettlement();
        }
    }
}
