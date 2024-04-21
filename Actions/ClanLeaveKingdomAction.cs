using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class ClanLeaveKingdomAction
    {
        internal static void Apply(Clan clan, bool forced)
        {
            if(clan.Kingdom != null)
            {
                Kingdom kingdom = clan.Kingdom;
                ChangeKingdomAction.ApplyByLeaveKingdom(clan, false);
                if(DramalordMCM.Get.KingdomOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogClanLeftKingdom(clan, kingdom, forced));
                }
                DramalordEvents.OnClanLeftKingdom(clan, kingdom, forced);
            }
        }
    }  
}
