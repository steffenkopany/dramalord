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
        public static void GetRelationWithPlayer(ref Hero __instance, ref float __result)
        {
            if(DramalordMCM.Get.IndividualRelation)
            {
                __result = Hero.MainHero.GetBaseHeroRelation(__instance);
            }  
        }
    }

    [HarmonyPatch(typeof(Hero), "GetRelation")]
    public static class GetRelationPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetRelation(ref Hero __instance, ref Hero otherHero, ref int __result)
        {
            if (DramalordMCM.Get.IndividualRelation)
            {
                __result = otherHero.GetBaseHeroRelation(__instance);
            }
        }
    }
}
