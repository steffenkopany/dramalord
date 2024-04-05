using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

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
