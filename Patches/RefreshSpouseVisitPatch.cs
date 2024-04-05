using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem
/* Unmerged change from project 'Dramalord (net6)'
Before:
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;
After:
using TaleWorlds.CampaignBehaviors;
*/
.
/* Unmerged change from project 'Dramalord (net6)'
Before:
using Dramalord.UI;
After:
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;
*/
CampaignBehaviors;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "RefreshSpouseVisit", new Type[] { typeof(Hero) })]
    public static class RefreshSpouseVisitPatch
    {
        public static bool Prefix(ref Hero hero)
        {
            return false;
        }
    }
}
