using Dramalord.Missions;
using HarmonyLib;
using JetBrains.Annotations;
using SandBox.View.Missions;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;

namespace Dramalord.Patches
{
    /*
    [HarmonyPatch(typeof(MissionConversationCameraView), "UpdateAgentLooksForConversation")]
    public static class UpdateAgentLooksForConversationPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool UpdateAgentLooksForConversation()
        {
            if(Mission.Current?.GetMissionBehavior<DuelMissionController>() != null)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MissionConversationCameraView), "UpdateOverridenCamera")]
    public static class UpdateOverridenCameraPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool UpdateOverridenCamera(ref bool __result)
        {
            if (Mission.Current?.GetMissionBehavior<DuelMissionController>() != null)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MissionScreen), "CheckForUpdateCamera")]
    public static class CheckForUpdateCameraPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool CheckForUpdateCamera()
        {
            if (Mission.Current?.GetMissionBehavior<DuelMissionController>() != null)
            {
                DuelMissionController controller = Mission.Current.GetMissionBehavior<DuelMissionController>();
                if(controller._duelHasEnded)
                    return false;
            }
            return true;
        }
    }
    */
}
