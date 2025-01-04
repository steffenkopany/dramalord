using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using Dramalord.LogItems;
using TaleWorlds.CampaignSystem.Extensions;
using HarmonyLib;

namespace Dramalord.Actions
{
    internal static class JoinKingdomAction
    {
        internal static void Apply(Clan clan, Kingdom kingdom)
        {
            if (clan.Kingdom == kingdom)
            {
                return;
            }
            if (clan.Kingdom != null)
            {
                LeaveKingdomAction.Apply(clan);
            }

            clan.Stances.Do(item => 
            {
                IFaction other = clan == item.Faction1 ? item.Faction2 : item.Faction1;
                if (item.IsAtWar && !kingdom.IsAtWarWith(other))
                {
                    MakePeaceAction.Apply(item.Faction1, item.Faction2);
                }
            });

            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, false);

            if (DramalordMCM.Instance.ClanChangeLogs)
            {
                LogEntry.AddLogEntry(new JoinKingdomLog(clan, kingdom));
            }
        }
    }
}
