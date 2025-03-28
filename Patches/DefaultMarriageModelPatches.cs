using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(DefaultMarriageModel), "IsSuitableForMarriage")]
    public static class IsSuitableForMarriagePatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void IsSuitableForMarriage(ref Hero maidenOrSuitor, ref bool __result)
        {
            if(!DramalordMCM.Instance.AllowDefaultMarriages)
            {
                __result = false;
            }
            else if(__result && !BetrothIntention.OtherMarriageModFound)
            {
                __result = maidenOrSuitor.GetRelationTo(Hero.MainHero).Relationship != RelationshipType.Spouse;
            }
        }
    } 
}
