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
using JetBrains.Annotations;
using Dramalord.Data;

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
