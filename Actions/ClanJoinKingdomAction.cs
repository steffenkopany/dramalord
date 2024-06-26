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

            if (DramalordMCM.Get.KingdomOutput && (clan == Clan.PlayerClan || (Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom == kingdom) || !DramalordMCM.Get.OnlyPlayerClanOutput))
            {
                LogEntry.AddLogEntry(new EncyclopediaLogClanJoinedKingdom(clan, kingdom));
            }
            DramalordEventCallbacks.OnClanJoinedKingdom(clan, kingdom);
        }
    }  
}
