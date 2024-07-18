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
            showQuickNotification = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(ChangeRelationAction), "ApplyRelationChangeBetweenHeroes", new Type[] { typeof(Hero), typeof(Hero), typeof(int), typeof(bool) })]
    public static class ApplyRelationChangeBetweenHeroesPatch
    {
        public static bool Prefix(ref Hero hero, ref Hero gainedRelationWith, ref int relationChange, ref bool showQuickNotification)
        {
            showQuickNotification = false;
            return true;
        }
    }

    [HarmonyPatch(typeof(ChangeRelationAction), "ApplyEmissaryRelation", new Type[] { typeof(Hero), typeof(Hero), typeof(int), typeof(bool) })]
    public static class ApplyEmissaryRelationPatch
    {
        public static bool Prefix(ref Hero emissary, ref Hero gainedRelationWith, ref bool relationChange, ref bool showQuickNotification)
        {
            showQuickNotification = false;
            return true;
        }
    }
}
