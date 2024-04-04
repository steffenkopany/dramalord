using Bannerlord.UIExtenderEx;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem;
using Dramalord.Data;
using Dramalord.UI;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(DefaultRomanceModel), "GetAttractionValuePercentage")]
    public static class GetAttractionValuePercentagePatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetAttractionValuePercentage(Hero potentiallyInterestedCharacter, Hero heroOfInterest, ref int __result)
        {
            if(Info.ValidateHeroMemory(potentiallyInterestedCharacter,heroOfInterest))
            {
                __result = Info.GetAttractionToHero(potentiallyInterestedCharacter, heroOfInterest);
            }
        }
    }
}
