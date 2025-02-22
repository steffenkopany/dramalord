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
    /*
    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "CheckOffspringsToDeliver")]
    public static class CheckOffspringsToDeliverPatch2
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool CheckOffspringsToDeliver(ref object pregnancy)
        {
            Type? pregType = AccessTools.TypeByName("PregnancyCampaignBehavior.Pregnancy");
            if(pregnancy.GetType() == pregType)
            {
                FieldInfo? mom = pregType.GetField("Mother");
                FieldInfo? dad = pregType.GetField("Father");
                
                if(mom.GetValue(pregnancy) == null)
                {
                    mom.SetValue(pregnancy, Hero.AllAliveHeroes.GetRandomElementWithPredicate(s => s.IsFemale && !s.IsChild && s.IsLord && !s.IsPregnant && s.Clan != Clan.PlayerClan));
                }

                if(dad.GetValue(pregnancy) == null)
                {
                    dad.SetValue(pregnancy, Hero.AllAliveHeroes.GetRandomElementWithPredicate(s => !s.IsFemale && !s.IsChild && s.IsLord && s.Clan != Clan.PlayerClan));
                }
            }
            return true;
        }
    }
    */

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
