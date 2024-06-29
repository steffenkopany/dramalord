using SandBox.Missions.MissionLogics;
using SandBox.View.Missions;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Objects;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using SandBox.Missions.AgentBehaviors;
using SandBox;
using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using SandBox.Issues;
using Dramalord.Conversations;
using HarmonyLib;
using TaleWorlds.CampaignSystem.AgentOrigins;
using Extensions = TaleWorlds.Core.Extensions;
using TaleWorlds.CampaignSystem.Settlements;

namespace Dramalord.Actions
{
    internal static class HeroFightAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if(hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero opponent = (hero == Hero.MainHero) ? target : hero;
            }
        }

        internal static void FightOver(bool playerWon)
        {

        }


        public static Mission OpenDuelMission(
          string scene,
          CharacterObject duelCharacter,
          bool isInsideSettlement)
        {
            return MissionState.OpenNew("DuelMission", CreateDuelMissionInitializerRecord(scene), (a) => new MissionBehavior[]
            {
                new MissionCampaignView(),
                new CampaignMissionComponent(),
                new MissionOptionsComponent(),
                new DuelMissionController(duelCharacter, false, false, isInsideSettlement),
                ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
                ViewCreator.CreateMissionAgentStatusUIHandler(a),
                ViewCreator.CreateMissionMainAgentEquipmentController(a),
                (MissionView) new MissionSingleplayerViewHandler(),
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

        public static MissionInitializerRecord CreateDuelMissionInitializerRecord(
          string sceneName,
          string sceneLevels = "",
          bool doNotUseLoadingScreen = false)
        {
            MissionInitializerRecord initializerRecord = new MissionInitializerRecord(sceneName);
            initializerRecord.DamageToPlayerMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToPlayerMultiplier();
            initializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
            initializerRecord.PlayingInCampaignMode = Campaign.Current.GameMode == CampaignGameMode.Campaign;
            initializerRecord.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.GetLogicalPosition());
            initializerRecord.SceneLevels = sceneLevels;
            initializerRecord.DoNotUseLoadingScreen = doNotUseLoadingScreen;
            return initializerRecord;
        }

        internal class DuelMissionController : MissionLogic
        {
            private CharacterObject _duelCharacter;
            private bool _spawnBothSidesWithHorses;
            private bool _duelHasEnded;
            private BasicMissionTimer _duelEndTimer;
            private Agent _playerAgent;
            private Agent _duelAgent;
            private bool _duelWon;
            private bool _friendlyDuel;
            private bool _isInsideSettlement;
            private Scene _fallbackScene;

            public DuelMissionController(
              CharacterObject duelCharacter,
              bool spawnBothSidesWithHorses,
              bool friendlyDuel,
              bool isInsideSettlement)
            {
                _duelCharacter = duelCharacter;
                _spawnBothSidesWithHorses = spawnBothSidesWithHorses;
                _friendlyDuel = friendlyDuel;
                _isInsideSettlement = isInsideSettlement;
            }

            public override void AfterStart()
            {
                Mission.SetMissionMode(MissionMode.Duel, true);
                _duelHasEnded = false;
                _duelEndTimer = new BasicMissionTimer();
                InitializeMissionTeams();
                MatrixFrame playerSpawnFrame;
                MatrixFrame opponentSpawnFrame;
                if (_isInsideSettlement)
                {
                    getArenaSpawnFrames(out playerSpawnFrame, out opponentSpawnFrame);
                }
                else
                {
                    var attackerEntity = Mission.Current.Scene.FindEntityWithTag("attacker_infantry") ?? Mission.Current.Scene.FindEntityWithName("sp_attacker_infantry");

                    Vec3 globalPosition = attackerEntity.GlobalPosition;
                    getBattleSpawnFrames(globalPosition.AsVec2, out playerSpawnFrame, out opponentSpawnFrame);
                }
                _playerAgent = SpawnAgent(CharacterObject.PlayerCharacter, playerSpawnFrame);
                Mission.CameraIsFirstPerson = false;
                _duelAgent = SpawnAgent(_duelCharacter, opponentSpawnFrame);
            }

            public override void OnMissionTick(float dt)
            {
                if (!_duelHasEnded || (double)_duelEndTimer.ElapsedTime <= 4.0)
                    return;
                
                _duelEndTimer.Reset();
            }

            public override InquiryData OnEndMissionRequest(out bool canLeave)
            {
                canLeave = true;
                return _duelHasEnded ? null :
                    new InquiryData("", GameTexts.FindText("str_give_up_fight", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(Mission.OnEndMissionResult), null, "");
            }

            public override void OnAgentRemoved(
              Agent affectedAgent,
              Agent affectorAgent,
              AgentState agentState,
              KillingBlow killingBlow)
            {
                if (!affectedAgent.IsHuman)
                    return;
                if (affectedAgent == _duelAgent)
                    _duelWon = true;
                _duelHasEnded = true;
            }

            public override bool MissionEnded(ref MissionResult missionResult) => false;

            protected override void OnEndMission()
            {
                if (_duelWon)
                {
                    HeroKillAction.Apply(Hero.MainHero, _duelCharacter.HeroObject, Hero.MainHero, Data.EventType.Anger);
                }
                else
                {
                    HeroKillAction.Apply(_duelCharacter.HeroObject, Hero.MainHero, _duelCharacter.HeroObject, Data.EventType.Anger);
                }
            }

            private Agent SpawnAgent(CharacterObject character, MatrixFrame spawnFrame)
            {
                AgentBuildData agentBuildData1 = new AgentBuildData(character);
                agentBuildData1.BodyProperties(character.GetBodyPropertiesMax());
                Team team = character == CharacterObject.PlayerCharacter ? Mission.PlayerTeam : Mission.PlayerEnemyTeam;
                Mission mission = Mission;
                AgentBuildData agentBuildData2 = agentBuildData1.Team(team).InitialPosition(spawnFrame.origin);
                Vec2 vec2 = spawnFrame.rotation.f.AsVec2;
                vec2 = vec2.Normalized();
                ref Vec2 local = ref vec2;
                AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(local).NoHorses(!_spawnBothSidesWithHorses).Equipment(character.FirstBattleEquipment).TroopOrigin(GetAgentOrigin(character)).ClothingColor1(character.Culture.Color).ClothingColor2(character.Culture.Color2);
                Agent agent = mission.SpawnAgent(agentBuildData3, false);
                agent.FadeIn();
                if (character == CharacterObject.PlayerCharacter)
                    agent.Controller = (Agent.ControllerType)2;
                if (agent.IsAIControlled)
                    agent.SetWatchState((Agent.WatchState)2);
                agent.WieldInitialWeapons((Agent.WeaponWieldActionType)2);
                return agent;
            }

            private IAgentOriginBase GetAgentOrigin(CharacterObject character) =>
                !_friendlyDuel ? new PartyAgentOrigin(character.HeroObject.PartyBelongedTo.Party, character, character.Level, new UniqueTroopDescriptor(), false) :
                (IAgentOriginBase)new SimpleAgentOrigin(character, character.Level, null, new UniqueTroopDescriptor());

            private void InitializeMissionTeams()
            {
                Mission.Teams.Add(0, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2, Hero.MainHero.Clan.Banner, true, false, true);
                Mission.Teams.Add((BattleSideEnum)1, _duelCharacter.Culture.Color, _duelCharacter.Culture.Color2, _duelCharacter.HeroObject.Clan.Banner, true, false, true);
                Mission.PlayerTeam = Mission.Teams.Defender;
            }

            private void getBattleSpawnFrames(
              Vec2 spawnPoint,
              out MatrixFrame playerSpawnFrame,
              out MatrixFrame opponentSpawnFrame)
            {
                float num = 0.0f;
                Vec2 vec2_1 = new Vec2(spawnPoint.X, spawnPoint.Y + 10f);
                Mission.Scene.GetHeightAtPoint(vec2_1, (BodyFlags)2208137, ref num);
                Vec3 vec3_1 = new Vec3(vec2_1.X, vec2_1.Y, num, -1);
                Mat3 mat3_1 = new Mat3(Vec3.Side, new Vec3(0f, -1f, 0f, -1f), Vec3.Up);
                Vec2 vec2_2 = new Vec2(spawnPoint.X, spawnPoint.Y - 10f);
                Mission.Scene.GetHeightAtPoint(vec2_2, (BodyFlags)2208137, ref num);
                Vec3 vec3_2 = new Vec3(vec2_2.X, vec2_2.Y, num, -1);
                Mat3 mat3_2 = new Mat3(Vec3.Side, Vec3.Forward, Vec3.Up);
                playerSpawnFrame = new MatrixFrame(mat3_1, vec3_1);
                opponentSpawnFrame = new MatrixFrame(mat3_2, vec3_2);
            }

            private void getArenaSpawnFrames(
              out MatrixFrame playerSpawnFrame,
              out MatrixFrame opponentSpawnFrame)
            {
                List<MatrixFrame> list = Mission.Scene.FindEntitiesWithTag("sp_arena").Select<GameEntity, MatrixFrame>(e => e.GetGlobalFrame()).ToList<MatrixFrame>();
                for (int index = 0; index < list.Count; ++index)
                {
                    MatrixFrame matrixFrame = list[index];
                    matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
                    list[index] = matrixFrame;
                }
                playerSpawnFrame = Extensions.GetRandomElement<MatrixFrame>(list);
                list.Remove(playerSpawnFrame);
                opponentSpawnFrame = Extensions.GetRandomElement<MatrixFrame>(list);
            }

            private Vec2 getMiddlePoint(Vec3 pointA, Vec3 pointB) => new Vec2((float)(((double)pointA.X + (double)pointB.X) / 2.0), (float)(((double)pointA.Y + (double)pointB.Y) / 2.0));
        }
    }
}
