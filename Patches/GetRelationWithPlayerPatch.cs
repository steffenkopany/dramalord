using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(Hero), "GetRelationWithPlayer")]
    public static class GetRelationWithPlayerPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetRelation(ref Hero __instance, ref float __result)
        {
            if (Info.ValidateHeroMemory(__instance, Hero.MainHero))
            {
                __result = Info.GetEmotionToHero(__instance, Hero.MainHero);
            }            
        }
    }
}
