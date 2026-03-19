using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(MarriageSceneNotificationItem), "GetBanners")]
    internal static class MarriageSceneNotificationItePatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetBanners(ref Banner[] __result)
        {
            Banner? banner = null;
            bool hasNull = false;
            foreach(Banner b in __result)
            {
                if(b != null && banner == null)
                {
                    banner = b;
                }
                else if (b == null && hasNull == false)
                {
                    hasNull = true;
                }
            }

            if(hasNull)
            {
                for(int i = 0; i < __result.Length; i++)
                {
                    if(__result[i] == null)
                    {
                        __result[i] = banner;
                    }
                }
            }
        }
    }
}
