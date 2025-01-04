using Dramalord.LogItems;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class LeaveKingdomAction
    {
        internal static void Apply(Clan clan)
        {
            if (clan.Kingdom != null)
            {
                Kingdom kingdom = clan.Kingdom;
                ChangeKingdomAction.ApplyByLeaveKingdom(clan, false);
                
                if (DramalordMCM.Instance.ClanChangeLogs)
                {
                    LogEntry.AddLogEntry(new LeaveKingdomLog(clan, kingdom));
                }
            }
        }
    }
}
