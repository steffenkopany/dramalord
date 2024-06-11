using Dramalord.Data;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Patches
{
    /*
    [HarmonyPatch(typeof(DefaultMarriageModel), "IsCoupleSuitableForMarriage", new Type[] { typeof(Hero), typeof(Hero) })]
    public static class IsCoupleSuitableForMarriagePatch
    {
        public static bool Prefix(ref Hero firstHero, ref Hero secondHero, ref bool __result)
        {
            if (IsSuitableForMarriage(firstHero) && IsSuitableForMarriage(secondHero) && (firstHero.Clan != null || secondHero.Clan != null))
            {
                if (firstHero.Spouse == null && secondHero.Spouse == null)
                {
                    return true;
                }

                if (firstHero.IsDramalordRelativeTo(secondHero) && DramalordMCM.Get.ProtectFamily)
                {
                    return false;
                }

                if (firstHero == Hero.MainHero || secondHero == Hero.MainHero)
                {
                    return true;
                }

                DramalordTraits firstTraits = firstHero.GetDramalordTraits();
                DramalordTraits secondTraits = secondHero.GetDramalordTraits();

                bool firstPoly = firstTraits.Openness == 2 && firstTraits.Agreeableness == 2 && firstTraits.Extroversion > 0 && firstTraits.Neuroticism < 0;
                bool secondPoly = secondTraits.Openness == 2 && secondTraits.Agreeableness == 2 && secondTraits.Extroversion > 0 && secondTraits.Neuroticism < 0;

                return firstPoly && secondPoly;
            }
            return false;
        }

        public static bool IsSuitableForMarriage(Hero maidenOrSuitor)
        {
            return maidenOrSuitor.IsDramalordLegit() && maidenOrSuitor.Age > 18;
        }
    }

    [HarmonyPatch(typeof(DefaultMarriageModel), "GetClanAfterMarriage", new Type[] { typeof(Hero), typeof(Hero) })]
    public static class GetClanAfterMarriagePatch
    {
        public static bool Prefix(ref Hero firstHero, ref Hero secondHero, ref Clan __result)
        {
            if (firstHero.IsHumanPlayerCharacter)
            {
                __result = firstHero.Clan;
            }
            else if (secondHero.IsHumanPlayerCharacter)
            {
                __result = secondHero.Clan;
            }
            else if (firstHero.Clan != null && firstHero.Clan.Leader == firstHero)
            {
                __result = firstHero.Clan;
            }
            else if (secondHero.Clan != null && secondHero.Clan.Leader == secondHero)
            {
                __result = secondHero.Clan;
            }
            else if (firstHero.Clan != null && secondHero.Clan == null)
            {
                __result = firstHero.Clan;
            }
            else if (secondHero.Clan != null && firstHero.Clan == null)
            {
                __result = secondHero.Clan;
            }
            else if (!firstHero.IsFemale)
            {
                __result = firstHero.Clan;
            }
            else
            {
                __result = secondHero.Clan;
            }
            
            return false;
        }
    }

    [HarmonyPatch(typeof(DefaultMarriageModel), "IsClanSuitableForMarriage", new Type[] { typeof(Clan)})]
    public static class IsClanSuitableForMarriagePatch
    {
        public static bool Prefix(ref Clan clan, ref bool __result)
        {
            if (DramalordMCM.Get.AllowDefaultMarriages)
            {
                if (clan != null && !clan.IsBanditFaction)
                {
                    __result = !clan.IsRebelClan;
                }
            }
            else
            {
                __result = false;
            }
    
            return false;
        }

    }
   */
}
