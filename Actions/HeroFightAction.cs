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

namespace Dramalord.Actions
{
    internal static class HeroFightAction
    {
        internal static FightMission? fightMission = null;
        internal static void Apply(Hero hero, Hero target)
        {
            if(hero != Hero.MainHero && target != Hero.MainHero)
            {
                NPCFight(hero, target);
                return;
            }

            Hero challenger = (hero == Hero.MainHero) ? target : hero;

            string arena = string.Empty; //lordshall

            if (challenger.CurrentSettlement != null)
            {
                if(challenger.CurrentSettlement.IsTown)
                {
                    arena = challenger.CurrentSettlement.LocationComplex.GetLocationWithId("lordshall").GetSceneName(1);
                }
            }
            else
            {  
                MapPatchData dat = Campaign.Current.MapSceneWrapper.GetMapPatchAtPosition(MobileParty.MainParty.Position2D);
                SingleplayerBattleSceneData spb = GameSceneDataManager.Instance.SingleplayerBattleScenes.Where((SingleplayerBattleSceneData scene) => scene.MapIndices.Contains(dat.sceneIndex)).First();
                arena = spb.SceneID;
            }

            MissionInitializerRecord mir = new(arena);
            mir.DamageToPlayerMultiplier = Campaign.Current.Models.DifficultyModel.GetDamageToPlayerMultiplier();
            mir.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
            mir.PlayingInCampaignMode = Campaign.Current.GameMode == CampaignGameMode.Campaign;
            mir.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.GetLogicalPosition());
            mir.SceneLevels = "";
            mir.DoNotUseLoadingScreen = true;

            Mission mission = MissionState.OpenNew("DuelMission", mir, (a) => new MissionBehavior[]
            {
                new MissionCampaignView(),
                new CampaignMissionComponent(),
                new MissionOptionsComponent(),
                new FightMission(challenger),
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

            Agent player = mission.SpawnAgent(new AgentBuildData(Hero.MainHero.CharacterObject));
            Agent npc = mission.SpawnAgent(new AgentBuildData(challenger.CharacterObject));

            List<Agent> fighter = new();
            List<Agent> opponent = new();
            fighter.Add(player);
            opponent.Add(npc);

            mission.GetMissionBehavior<MissionFightHandler>().StartCustomFight(fighter, opponent, true, true, OnFightEnded);
        }

        internal static void NPCFight(Hero hero, Hero target)
        {

        }

        internal static void OnFightEnded(bool playerWon)
        {
            int i = 0;
            fightMission = null;
        }

        internal class FightMission : MissionLogic
        {
            private Hero Opponent;
            public FightMission(Hero opponent)
            {
                Opponent = opponent;
            }
            public override void AfterStart()
            {

            }
        }
    }
}
