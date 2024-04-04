using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using Dramalord.UI;

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
