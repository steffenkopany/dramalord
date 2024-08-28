using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using SandBox.Missions.MissionLogics;
using SandBox.View.Missions;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;

namespace Dramalord.Missions
{
    internal static class DuelMissions
    {

        public static Mission OpenDuelMission(
          CharacterObject duelCharacter,
          bool spawnBothSidesWithHorse,
          bool friendlyDuel)
        {
            List<string> forbiddenScenes = new List<string>()
            {
                "battle_terrain_biome_030",
                "battle_terrain_biome_053",
                "battle_terrain_biome_088"
            }; 

            if (duelCharacter == null)
                duelCharacter = PlayerEncounter.EncounteredParty.LeaderHero.CharacterObject;

            string scene;
            bool isInsideSettlement;
            if (PlayerEncounter.Current != null && PlayerEncounter.InsideSettlement)
            {
                var loc = PlayerEncounter.LocationEncounter;
                Settlement currentSettlement = Settlement.CurrentSettlement;
                scene = currentSettlement.LocationComplex.GetLocationWithId("arena").GetSceneName(currentSettlement.IsTown ? currentSettlement.Town.GetWallLevel() : 1);
                isInsideSettlement = true;
            }
            else
            {
                scene = PlayerEncounter.GetBattleSceneForMapPatch(Campaign.Current.MapSceneWrapper.GetMapPatchAtPosition(MobileParty.MainParty.Position2D));
                isInsideSettlement = false;
            }
            if (forbiddenScenes.Contains(scene))
                scene = "battle_terrain_biome_065";

            MissionInitializerRecord initializerRecord = new MissionInitializerRecord(scene);
            initializerRecord.DamageToPlayerMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToPlayerMultiplier();
            initializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
            initializerRecord.PlayingInCampaignMode = Campaign.Current.GameMode == CampaignGameMode.Campaign;
            initializerRecord.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.GetLogicalPosition());
            initializerRecord.SceneLevels = "";
            initializerRecord.DoNotUseLoadingScreen = false;

            return MissionState.OpenNew("DuelMission", initializerRecord, (a) => new MissionBehavior[]
            {
                new MissionCampaignView(),
                new CampaignMissionComponent(),
                new MissionOptionsComponent(),
                new DuelMissionController(duelCharacter, spawnBothSidesWithHorse, friendlyDuel, isInsideSettlement),
                ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
                ViewCreator.CreateMissionAgentStatusUIHandler(a),
                ViewCreator.CreateMissionMainAgentEquipmentController(a),
                (MissionView) new MissionSingleplayerViewHandler(),
                isInsideSettlement ? new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.3f) : null,
                //(MissionView) new MusicBattleMissionView(false),
                new MissionBoundaryWallView(),
                new MissionItemContourControllerView(),
                new MissionAgentContourControllerView(),
                new MissionGauntletOptionsUIHandler(),
                new AgentHumanAILogic(),
                ViewCreator.CreateOptionsUIHandler(),
                ViewCreator.CreateMissionLeaveView(),
                ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
                ViewCreator.CreatePhotoModeView(),
            }, true, true);
        }
    }
}
