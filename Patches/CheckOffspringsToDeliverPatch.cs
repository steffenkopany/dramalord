
/* Unmerged change from project 'Dramalord (net6)'
Before:
using HarmonyLib;
using System;
After:
using Dramalord.Data;
using Dramalord.UI;
using HarmonyLib;
using JetBrains.Annotations;
using System;
*/
using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
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
using JetBrains.Annotations;
using Dramalord.Data;
After:
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;
*/
CampaignBehaviors;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "CheckOffspringsToDeliver", new Type[] { typeof(Hero) })]
    public static class CheckOffspringsToDeliverPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void CheckOffspringsToDeliver(ref Hero hero)
        {
            HeroOffspringData? offspring = Info.GetHeroOffspring(hero);
            if(offspring != null)
            {
                hero.IsPregnant = true;
            }
        }
    }
}
