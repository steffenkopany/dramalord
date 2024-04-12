
using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "CheckOffspringsToDeliver", new Type[] { typeof(Hero) })]
    public static class CheckOffspringsToDeliverPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void CheckOffspringsToDeliver(ref Hero hero)
        {
            HeroOffspringData? offspring = Info.GetHeroOffspring(hero);
            if(offspring != null)
            {
                hero.IsPregnant = true;
            }
        }
    }
}
