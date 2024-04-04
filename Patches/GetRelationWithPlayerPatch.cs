using Dramalord.Data;
using Dramalord.UI;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(Hero), "GetRelationWithPlayer")]
    public static class GetRelationWithPlayerPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetRelation(ref Hero __instance, ref float __result)
        {
            if (Info.ValidateHeroInfo(__instance))
            {
                __result = Info.GetEmotionToHero(__instance, Hero.MainHero);
            }            
        }
    }
}
