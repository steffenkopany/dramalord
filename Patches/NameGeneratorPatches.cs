using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(NameGenerator), "CalculateNameScore")]
    public static class CalculateNameScorePatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool CalculateNameScore(ref Hero hero, ref TextObject name, ref int __result)
        {
            if(hero.Clan == null)
            {
                __result = MBRandom.RandomInt(0, 5000);
                return false;
            }
            return true;
        }
    }
}
