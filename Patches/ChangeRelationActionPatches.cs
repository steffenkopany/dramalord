using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using static TaleWorlds.CampaignSystem.Actions.ChangeRelationAction;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(ChangeRelationAction), "ApplyInternal")]
    public static class ChangeRelationActionPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static void ApplyInternalPrefix(ref Hero originalHero, ref Hero originalGainedRelationWith, ref int relationChange, ref bool showQuickNotification, ref ChangeRelationDetail detail)
        {
            showQuickNotification = (showQuickNotification && !DramalordMCM.Instance.ShowRelationChanges && detail == ChangeRelationDetail.Default) ? false : showQuickNotification;
        }

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void ApplyInternalPostfix(ref Hero originalHero, ref Hero originalGainedRelationWith, ref int relationChange, ref bool showQuickNotification, ref ChangeRelationDetail detail)
        {
            if (originalHero.IsDramalordLegit() && originalGainedRelationWith.IsDramalordLegit())
            {
                (new RelationshipEvent(originalHero, originalGainedRelationWith)).Action();
            } 
        }
    }
}
