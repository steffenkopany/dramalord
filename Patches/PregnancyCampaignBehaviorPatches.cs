using Dramalord.Data;
using Dramalord.Extensions;
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
            HeroPregnancy? pregnancy = hero.GetPregnancy();
            if(pregnancy != null)
            {
                hero.IsPregnant = true;
            }
        }
    }

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "RefreshSpouseVisit", new Type[] { typeof(Hero) })]
    public static class RefreshSpouseVisitPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool RefreshSpouseVisit(ref Hero hero)
        {
            if(!DramalordMCM.Instance.AllowDefaultPregnanciesGlobal)
            {
                return false;
            }
            if((hero == Hero.MainHero || hero.Spouse == Hero.MainHero) && !DramalordMCM.Instance.AllowDefaultPregnanciesPlayer)
            {
                return false;
            }
            if(hero.Spouse != null && hero.Spouse.IsFemale == hero.IsFemale)
            {
                return false;
            }
            if(hero.Clan == null)
            {
                return false;
            }
            return true;
        }
    }
}
