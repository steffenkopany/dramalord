using Dramalord.LogItems;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class JoinClanAction
    {
        internal static bool Apply(Hero hero, Clan clan, bool forMarriage)
        {
            if (hero.Occupation == Occupation.Wanderer)
            {
                hero.SetName(hero.FirstName, hero.FirstName);
            }

            Clan oldClan = hero.Clan;
            hero.Clan = clan;
            hero.UpdateHomeSettlement();
            hero.SetNewOccupation(Occupation.Lord);
            //MakeHeroFugitiveAction.Apply(hero);
            if(!forMarriage)
            {
                TeleportHeroAction.ApplyDelayedTeleportToSettlement(hero, hero.HomeSettlement);
            }

            if ((clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new JoinClanLog(hero, clan));
            }

            return true;
        }
    }
}
