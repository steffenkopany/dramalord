using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(Hero), "GetBaseHeroRelation")]
    public static class GetBaseHeroRelationPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetRelation(Hero otherHero, ref Hero __instance, ref int __result)
        {
            if (__instance.IsLord && otherHero.IsLord && Info.ValidateHeroMemory(__instance, otherHero))
            {
                __result = (int)Info.GetEmotionToHero(__instance, otherHero);
            }
        }
    }
}
