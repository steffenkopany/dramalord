using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using Dramalord.UI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Actions;

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
