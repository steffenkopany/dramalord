using HarmonyLib;
using System;
using
/* Unmerged change from project 'Dramalord (net6)'
Before:
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;
After:
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
*/
TaleWorlds.
/* Unmerged change from project 'Dramalord (net6)'
Before:
using Dramalord.UI;
using TaleWorlds.Library;
After:
using TaleWorlds.Library;
*/
Library;
/* Unmerged change from project 'Dramalord (net6)'
Before:
using TaleWorlds.CampaignSystem.Actions;
After:
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;
*/


namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(InformationManager), "DisplayMessage", new Type[] { typeof(InformationMessage) })]
    public static class DisplayMessagePatch
    {
        public static bool Prefix(ref InformationMessage message)
        {
            if((message.Information.Contains("prisoner") || message.Information.Contains("escaped") || message.Information.Contains("released") || message.Information.Contains("defended")) && DramalordMCM.Get.NoCaptivityMessages)
            {
                return false; //Campaignevents.onstartbattle //onheroprisonerreleased //onheroprisonertaken // "freed"
                //TaleWorlds.CampaignSystem.SceneInformationPopupTypes.MarriageSceneNotificationItem.TitleText
                //LordConversationsCampaignBehavior CampaignSystem.Actions.TakePrisonerAction Actions.EndCaptivityAction StartBattleAction
            }
            return true;
        }
    }
}
