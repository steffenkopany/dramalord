using Dramalord.Actions;
using Dramalord.Data;
using HarmonyLib;
using Helpers;
using JetBrains.Annotations;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Patches
{

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "RefreshSpouseVisit", new Type[] { typeof(Hero) })]
    public static class RefreshSpouseVisitPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool RefreshSpouseVisit(ref Hero hero)
        {
            if(!DramalordMCM.Get.AllowDefaultPregnancies)
            {
                return false;
            }

            if (hero.Spouse != null && hero.IsNearby(hero.Spouse) && MBRandom.RandomInt(1,100) <= DramalordMCM.Get.PregnancyChance)
            {
                HeroConceiveAction.Apply(hero, hero.Spouse);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "CheckOffspringsToDeliver", new Type[] { typeof(Hero) })]
    public static class CheckOffspringsToDeliverPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool CheckOffspringsToDeliver(ref Hero hero)
        {
            HeroPregnancy? pregnancy = hero.GetDramalordPregnancy();
            if(hero.IsPregnant && pregnancy == null)
            {
                hero.IsPregnant = false;
            }
            else if(CampaignTime.Now.ToDays - pregnancy.Conceived > DramalordMCM.Get.PregnancyDuration)
            {
                HeroBirthAction.Apply(hero, pregnancy, hero.GetCloseHeroes());
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "ChildConceived", new Type[] { typeof(Hero) })]
    public static class ChildConceivedPatch
    {
        internal static Hero? Father;

        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool ChildConceived(ref Hero mother)
        {
            HeroConceiveAction.Apply(mother, Father ?? mother.Spouse);
            return false;
        }
    }   
}
