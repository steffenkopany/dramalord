using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(TeleportationCampaignBehavior), "OnHeroComesOfAge")]
    internal static class OnHeroComesOfAgePatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool OnHeroComesOfAge(ref Hero hero)
        {
            if(hero.Clan == null)
            {
                return false;
            }
            return true;
        }
    }
}
