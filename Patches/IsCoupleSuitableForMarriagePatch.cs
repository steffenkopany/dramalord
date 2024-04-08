using Dramalord.Data;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(DefaultMarriageModel), "IsCoupleSuitableForMarriage", new Type[] { typeof(Hero), typeof(Hero) })]
    public static class IsCoupleSuitableForMarriagePatch
    {
        public static bool Prefix(ref Hero firstHero, ref Hero secondHero, ref bool __result)
        {
            if( Info.ValidateHeroMemory(firstHero, secondHero) && firstHero.Spouse == null && secondHero.Spouse == null && Info.GetEmotionToHero(firstHero, secondHero) >= DramalordMCM.Get.MinEmotionForMarriage)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
