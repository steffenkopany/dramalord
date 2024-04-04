using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(Hero), "GetRelation")]
    public static class GetRelationshipPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetRelation(Hero otherHero, ref Hero __instance, ref int __result)
        {
            if (Info.ValidateHeroMemory(__instance, otherHero))
            {
                __result = (int)Info.GetEmotionToHero(__instance, otherHero);
            }
        }
    }
}
