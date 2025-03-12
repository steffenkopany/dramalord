using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(ChangeRelationAction), "ApplyPlayerRelation", new Type[] { typeof(Hero), typeof(int), typeof(bool), typeof(bool) })]
    public static class ApplyPlayerRelationPatch
    {
        public static bool Prefix(ref Hero gainedRelationWith, ref int relation, ref bool affectRelatives, ref bool showQuickNotification)
        {
            showQuickNotification = DramalordMCM.Instance.ShowRelationChanges;
            return true;
        }

        public static void Postfix(ref Hero gainedRelationWith, ref int relation, ref bool affectRelatives, ref bool showQuickNotification)
        {
            if(gainedRelationWith.IsDramalordLegit())
                new ChangeOpinionIntention(gainedRelationWith, Hero.MainHero, 0, 0, CampaignTime.Now).Action();
        }
    }

    [HarmonyPatch(typeof(ChangeRelationAction), "ApplyRelationChangeBetweenHeroes", new Type[] { typeof(Hero), typeof(Hero), typeof(int), typeof(bool) })]
    public static class ApplyRelationChangeBetweenHeroesPatch
    {
        public static bool Prefix(ref Hero hero, ref Hero gainedRelationWith, ref int relationChange, ref bool showQuickNotification)
        {
            showQuickNotification = DramalordMCM.Instance.ShowRelationChanges;
            return true;
        }

        public static void PostFix(ref Hero hero, ref Hero gainedRelationWith, ref int relationChange, ref bool showQuickNotification)
        {
            if(hero.IsDramalordLegit() && gainedRelationWith.IsDramalordLegit())
                new ChangeOpinionIntention(hero, gainedRelationWith, 0, 0, CampaignTime.Now).Action();
        }
    }

    [HarmonyPatch(typeof(ChangeRelationAction), "ApplyEmissaryRelation", new Type[] { typeof(Hero), typeof(Hero), typeof(int), typeof(bool) })]
    public static class ApplyEmissaryRelationPatch
    {
        public static bool Prefix(ref Hero emissary, ref Hero gainedRelationWith, ref bool relationChange, ref bool showQuickNotification)
        {
            showQuickNotification = DramalordMCM.Instance.ShowRelationChanges;
            return true;
        }

        public static void PostFix(ref Hero emissary, ref Hero gainedRelationWith, ref bool relationChange, ref bool showQuickNotification)
        {
            if (emissary.IsDramalordLegit() && gainedRelationWith.IsDramalordLegit())
                new ChangeOpinionIntention(emissary, gainedRelationWith, 0, 0, CampaignTime.Now).Action();
        }
    }
}
