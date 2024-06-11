using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class ClanJoinKingdomAction
    {
        internal static void Apply(Clan clan, Kingdom kingdom)
        {
            if(clan.Kingdom == kingdom)
            {
                return;
            }
            if(clan.Kingdom != null)
            {
                ClanLeaveKingdomAction.Apply(clan, false);
            }
            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, false);

            if (DramalordMCM.Get.KingdomOutput)
            {
                LogEntry.AddLogEntry(new EncyclopediaLogClanJoinedKingdom(clan, kingdom));
            }
            DramalordEventCallbacks.OnClanJoinedKingdom(clan, kingdom);
        }
    }  
}
