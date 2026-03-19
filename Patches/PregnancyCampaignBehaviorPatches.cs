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
            if (pregnancy != null)
            {
                hero.IsPregnant = true;
            }
        }
    }
    

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "ChildConceived", new Type[] { typeof(Hero) })]
    public static class ChildConceivedPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool ChildConceived(ref Hero mother)
        {
            HeroPregnancy? pregnancy = mother.GetPregnancy();
            if (pregnancy != null)
            {
                mother.IsPregnant = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "RefreshSpouseVisit", new Type[] { typeof(Hero) })]
    public static class RefreshSpouseVisitPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool RefreshSpouseVisit(ref Hero hero)
        {
            if(!DramalordMCM.Instance.AllowDefaultPregnancies)
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
            if(hero.IsFemale && (hero.IsPregnant || hero.GetPregnancy() != null))
            {
                hero.IsPregnant = true;
                return false;
            }
            if (hero.Spouse != null && hero.Spouse.IsFemale && (hero.Spouse.IsPregnant || hero.Spouse.GetPregnancy() != null))
            {
                hero.Spouse.IsPregnant = true;
                return false;
            }
            return true;
        }
    }
}
