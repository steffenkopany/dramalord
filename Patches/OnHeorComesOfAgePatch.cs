using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(TeleportationCampaignBehavior), "OnHeroComesOfAge", new Type[] { typeof(Hero) })]
    public static class OnHeroComesOfAgePatch
    {
        public static bool Prefix(ref Hero hero)
        {
            if(hero.Clan == null)
            {
                return false;
            }
            return true;
        }
    }
}
