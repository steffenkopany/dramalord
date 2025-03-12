using HarmonyLib;
using JetBrains.Annotations;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(MarriageSceneNotificationItem), "GetBanners")]
    public static class GetBannersPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool GetBanners(ref MarriageSceneNotificationItem __instance, ref IEnumerable<Banner> __result)
        {
            __result = new List<Banner>
            {
                (__instance.GroomHero.Clan != null && __instance.GroomHero.CompanionOf == null) ? __instance.GroomHero.Clan.Banner : __instance.GroomHero.CurrentSettlement.OwnerClan.Banner,
                (__instance.BrideHero.Clan != null && __instance.GroomHero.CompanionOf == null) ? __instance.BrideHero.Clan.Banner : __instance.BrideHero.CurrentSettlement.OwnerClan.Banner,
                (__instance.GroomHero.Clan != null && __instance.GroomHero.CompanionOf == null) ? __instance.GroomHero.Clan.Banner : __instance.GroomHero.CurrentSettlement.OwnerClan.Banner,
                (__instance.BrideHero.Clan != null && __instance.GroomHero.CompanionOf == null) ? __instance.BrideHero.Clan.Banner : __instance.BrideHero.CurrentSettlement.OwnerClan.Banner
            };
            return false;
        }
    }
}
