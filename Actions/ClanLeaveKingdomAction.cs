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
                if(DramalordMCM.Get.KingdomOutput && (clan == Clan.PlayerClan || (Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom == kingdom) || !DramalordMCM.Get.OnlyPlayerClanOutput))
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogClanLeftKingdom(clan, kingdom, forced));
                }
                DramalordEventCallbacks.OnClanLeftKingdom(clan, kingdom, forced);
            }
        }
    }  
}
