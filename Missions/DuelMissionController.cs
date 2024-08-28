using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static SandBox.SandBoxHelpers;

namespace Dramalord.Missions
{
    /*
    public static class MissionHelper
    {
        public static Agent SpawnAgent(
          CharacterObject character,
          MatrixFrame spawnFrame,
          bool isPlayer,
          bool isFriend,
          bool spawnBothSidesWithHorses)
        {
            Team team = isPlayer ? Mission.Current.PlayerTeam : Mission.Current.PlayerEnemyTeam;
            AgentBuildData agentBuildData = new AgentBuildData((BasicCharacterObject)character).Team(team).InitialPosition(spawnFrame.origin);
            Vec2 vec2 = ((Vec3)spawnFrame.rotation.f).AsVec2;
            vec2 = ((Vec2)vec2).Normalized();
            ref Vec2 local = ref vec2;
            Agent agent = Mission.Current.SpawnAgent(agentBuildData.InitialDirection(local).NoHorses(!spawnBothSidesWithHorses).Equipment(character.FirstBattleEquipment).TroopOrigin(MissionHelper.GetAgentOrigin(isFriend, character)).ClothingColor1(((BasicCultureObject)character.Culture).Color).ClothingColor2(((BasicCultureObject)character.Culture).Color2), false);
            agent.FadeIn();
            if (isPlayer)
            {
                agent.Controller = (Agent.ControllerType)2;
                Mission.Current.MainAgent = agent;
            }
            else if (agent.IsAIControlled)
                agent.SetWatchState((Agent.WatchState)2);
            agent.WieldInitialWeapons((Agent.WeaponWieldActionType)2, (Equipment.InitialWeaponEquipPreference)0);
            return agent;
        }

        public static (MatrixFrame, MatrixFrame) GetBattleSpawnFrames()
        {
            Vec3 globalPosition = (Mission.Current.Scene.FindEntityWithTag("attacker_infantry") ?? Mission.Current.Scene.FindEntityWithName("sp_attacker_infantry")).GlobalPosition;
            return MissionHelper.CalculateSpawnFrames(((Vec3)globalPosition).AsVec2, 10f);
        }

        private static (MatrixFrame playerSpawnFrame, MatrixFrame opponentSpawnFrame) CalculateSpawnFrames(
          Vec2 centerPoint,
          float distance)
        {
            float num = 0.0f;
            Vec2 vec2_1 = new Vec2(centerPoint.X, centerPoint.Y + 10f);
            Mission.Current.Scene.GetHeightAtPoint(vec2_1, (BodyFlags)2208137, ref num);
            Vec3 vec3_1 = new Vec3(vec2_1.X, vec2_1.Y, num, -1);
            Mat3 mat3_1 = new Mat3(Vec3.Side, new Vec3(0f, -1f, 0f, -1f), Vec3.Up);
            Vec2 vec2_2 = new Vec2(centerPoint.X, centerPoint.Y - 10f);
            Mission.Current.Scene.GetHeightAtPoint(vec2_2, (BodyFlags)2208137, ref num);
            Vec3 vec3_2 = new Vec3(vec2_2.X, vec2_2.Y, num, -1);
            Mat3 mat3_2 = new Mat3(Vec3.Side, Vec3.Forward, Vec3.Up);
            return (new MatrixFrame(mat3_2, vec3_2), new MatrixFrame(mat3_1, vec3_1));
        }

        public static (MatrixFrame, MatrixFrame) GetArenaSpawnFrames()
        {
            List<MatrixFrame> list = Mission.Current.Scene.FindEntitiesWithTag("sp_arena").Select<GameEntity, MatrixFrame>((Func<GameEntity, MatrixFrame>)(e => e.GetGlobalFrame())).ToList<MatrixFrame>();
            list.ForEach((Action<MatrixFrame>)(sp => ((Mat3)sp.rotation).OrthonormalizeAccordingToForwardAndKeepUpAsZAxis()));
            MatrixFrame randomElement1 = list.GetRandomElement();
            list.Remove(randomElement1);
            MatrixFrame randomElement2 = list.GetRandomElement();
            return (randomElement1, randomElement2);
        }

        

        private static IAgentOriginBase GetAgentOrigin(bool isFriendDuel, CharacterObject character)
        {
            return isFriendDuel ? (IAgentOriginBase)new SimpleAgentOrigin((BasicCharacterObject)character, ((BasicCharacterObject)character).Level, (Banner)null, new UniqueTroopDescriptor()) : (IAgentOriginBase)new PartyAgentOrigin(character.HeroObject.PartyBelongedTo?.Party, character, ((BasicCharacterObject)character).Level, new UniqueTroopDescriptor(), false);
        }
    }
    internal class DuelMissionController : MissionLogic
    {
        private readonly bool _spawnBothSidesWithHorses;
        private readonly bool _friendlyDuel;
        private readonly bool _isInsideSettlement;
        private BasicMissionTimer _duelEndTimer;
        private Agent _duelAgent;
        private bool _duelHasEnded;
        private bool _duelWon;
        private CharacterObject duelCharacter;

        public DuelMissionController(
          CharacterObject duelCharacter,
          bool spawnBothSidesWithHorses,
          bool friendlyDuel,
          bool isInsideSettlement)
        {
            this._spawnBothSidesWithHorses = spawnBothSidesWithHorses;
            this._friendlyDuel = friendlyDuel;
            this._isInsideSettlement = isInsideSettlement;
        }

        public virtual void AfterStart()
        {
            this.SetupMission();
            this.SetupAgents();
        }

        private void SetupMission()
        {
            ((MissionBehavior)this).Mission.SetMissionMode((MissionMode)3, true);
            this._duelHasEnded = false;
            this._duelEndTimer = new BasicMissionTimer();
            this.InitializeMissionTeams();
        }

        private void SetupAgents()
        {
            MatrixFrame spawnFrame1;
            MatrixFrame spawnFrame2;
            if (!this._isInsideSettlement)
                (spawnFrame1, spawnFrame2) = MissionHelper.GetBattleSpawnFrames();
            else
                (spawnFrame1, spawnFrame2) = MissionHelper.GetArenaSpawnFrames();
            MissionHelper.SpawnAgent(CharacterObject.PlayerCharacter, spawnFrame1, true, this._friendlyDuel, this._spawnBothSidesWithHorses);
            this._duelAgent = MissionHelper.SpawnAgent(duelCharacter, spawnFrame2, false, this._friendlyDuel, this._spawnBothSidesWithHorses);
            ((MissionBehavior)this).Mission.CameraIsFirstPerson = false;
        }

        public virtual void OnMissionTick(float dt)
        {
            if (!this._duelHasEnded || (double)this._duelEndTimer.ElapsedTime <= 4.0)
                return;
            this.DisplayDuelEndInformation();
            this._duelEndTimer.Reset();
        }

        private void DisplayDuelEndInformation()
        {
            GameTexts.SetVariable("leave_key", GameKeyTextExtensions.GetHotKeyGameTextFromKeyID(Game.Current.GameTextManager, ((List<GameKey>)HotKeyManager.GetAllCategories().FirstOrDefault<GameKeyContext>((Func<GameKeyContext, bool>)(r => r.GameKeyCategoryId == "Generic"))?.RegisteredGameKeys)[4].KeyboardKey.ToString()).ToString());
            MBInformationManager.AddQuickInformation(GameTexts.FindText("str_duel_has_ended", (string)null), 0, (BasicCharacterObject)null, "");
        }

        public virtual InquiryData OnEndMissionRequest(out bool canLeave)
        {
            canLeave = true;
            return this._duelHasEnded ? (InquiryData)null : new InquiryData("", GameTexts.FindText("str_give_up_fight", (string)null).ToString(), true, true, GameTexts.FindText("str_ok", (string)null).ToString(), GameTexts.FindText("str_cancel", (string)null).ToString(), new Action(((MissionBehavior)this).Mission.OnEndMissionResult), (Action)null, "", 0.0f, (Action)null, (Func<(bool, string)>)null, (Func<(bool, string)>)null);
        }

        public virtual void OnAgentRemoved(
          Agent affectedAgent,
          Agent affectorAgent,
          AgentState agentState,
          KillingBlow killingBlow)
        {
            if (!affectedAgent.IsHuman)
                return;
            this._duelHasEnded = true;
            this._duelWon = affectedAgent == this._duelAgent;
        }

        protected virtual void OnEndMission()
        {
            //DuelManager.SetDuelMissionResult(this._duelWon);
            //DuelManager.CallBackEndEvent(this._duelWon);
            PlayerEncounter.Finish(true);
        }

        private void InitializeMissionTeams()
        {
            ((MissionBehavior)this).Mission.Teams.Add((BattleSideEnum)0, Hero.MainHero.MapFaction.Color, Hero.MainHero.MapFaction.Color2, Hero.MainHero.Clan.Banner, true, false, true);
            ((MissionBehavior)this).Mission.Teams.Add((BattleSideEnum)1, ((BasicCultureObject)duelCharacter.Culture).Color, ((BasicCultureObject)duelCharacter.Culture).Color2, duelCharacter.HeroObject?.Clan?.Banner, true, false, true);
            ((MissionBehavior)this).Mission.PlayerTeam = ((MissionBehavior)this).Mission.DefenderTeam;
        }
    }
    */
    internal class DuelMissionController : MissionLogic
    {
        private CharacterObject _duelCharacter;
        private bool _spawnBothSidesWithHorses;
        internal bool _duelHasEnded;
        private BasicMissionTimer _duelEndTimer;
        private Agent _playerAgent;
        private Agent _duelAgent;
        private bool _duelWon;
        private bool _friendlyDuel;
        private bool _isInsideSettlement;

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
            GameTexts.SetVariable("leave_key", GameKeyTextExtensions.GetHotKeyGameTextFromKeyID(Game.Current.GameTextManager, HotKeyManager.GetAllCategories().FirstOrDefault(r => r.GameKeyCategoryId == "Generic").RegisteredGameKeys[4].KeyboardKey.ToString()).ToString());
            MBInformationManager.AddQuickInformation(GameTexts.FindText("str_duel_has_ended", null), 0, null, "");
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

        public override bool MissionEnded(ref MissionResult missionResult) => _duelHasEnded;

        protected override void OnEndMission() => PlayerEncounter.Finish(true);

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
            Mission.Teams.Add((BattleSideEnum)1, _duelCharacter.Culture.Color, _duelCharacter.Culture.Color2, _duelCharacter.HeroObject.Clan?.Banner, true, false, true);
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
            playerSpawnFrame = list.GetRandomElement();
            list.Remove(playerSpawnFrame);
            opponentSpawnFrame = list.GetRandomElement();
        }

        private Vec2 getMiddlePoint(Vec3 pointA, Vec3 pointB) => new Vec2((float)(((double)pointA.X + (double)pointB.X) / 2.0), (float)(((double)pointA.Y + (double)pointB.Y) / 2.0));
    }
    
}
