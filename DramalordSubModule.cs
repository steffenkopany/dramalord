using Dramalord.Actions;
using Dramalord.Behavior;
using Dramalord.Behaviours;
using Dramalord.Data.Intentions;
using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Dramalord
{
    public class DramalordSubModule : MBSubModuleBase
    {
        internal static string ModuleName = Assembly.GetExecutingAssembly().GetName().Name;
        internal static string ModuleVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        internal static bool Patched = false;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            base.OnGameStart(game, starter);

            CampaignGameStarter? campaignGameStarter = starter as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
                campaignGameStarter.AddBehavior(new DramalordCampaignBehavior(campaignGameStarter));
                campaignGameStarter.AddBehavior(new NpcCampaignBehavior(campaignGameStarter));
                //campaignGameStarter.AddBehavior(new PlayerCampaignBehavior(campaignGameStarter));

                if (!Patched && game.GameType is Campaign)
                {
                    Harmony harmony = new Harmony(ModuleName);
                    Type[] typesFromAssembly = AccessTools.GetTypesFromAssembly(typeof(DramalordSubModule).Assembly);
                    foreach (Type type in typesFromAssembly)
                    {
                        try
                        {
                            new PatchClassProcessor(harmony, type).Patch();
                        }
                        catch (HarmonyException)
                        {
                            InformationManager.DisplayMessage(new InformationMessage($"{ModuleName} could not apply patch {type.Name}", new Color(1f, 0f, 0f)));
                        }
                    }
                    Patched = true;
                }
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
           
            InformationManager.DisplayMessage(new InformationMessage($"{ModuleName} {ModuleVersion} loaded", new Color(1f, 0.08f, 0.58f)));
           
            Type? pompaType = AccessTools.TypeByName("PompaSceneNotificationItem");
            if(pompaType != null)
            {
                IntercourseIntention.HotButterFound = true;
                InformationManager.DisplayMessage(new InformationMessage($"{ModuleName}: HotButter detected", new Color(1f, 0.08f, 0.58f)));
            }

            Type? AMType = AccessTools.TypeByName("MAMarriageAction");
            if (AMType != null)
            {
                BetrothIntention.OtherMarriageModFound = true;
                InformationManager.DisplayMessage(new InformationMessage($"{ModuleName}: MarryAnyone detected (Disabling Marriage)", new Color(1f, 0.08f, 0.58f)));
            }

            Type? SEType = AccessTools.TypeByName("SpousesExpandedUtil");
            if (SEType != null)
            {
                BetrothIntention.OtherMarriageModFound = true;
                InformationManager.DisplayMessage(new InformationMessage($"{ModuleName}: Spouses Expanded detected (Disabling Marriage)", new Color(1f, 0.08f, 0.58f)));
            }

            Type? BKType = AccessTools.TypeByName("BannerKingsSettings");
            if (BKType != null)
            {
                BetrothIntention.OtherMarriageModFound = true;
                InformationManager.DisplayMessage(new InformationMessage($"{ModuleName}: Banner Kings detected (Disabling Marriage)", new Color(1f, 0.08f, 0.58f)));
            }

            Type? BastardType = AccessTools.TypeByName("BastardCampaignEvents");
            if (BastardType != null)
            {
                IntercourseIntention.OtherPregnancyModFound = true;
                InformationManager.DisplayMessage(new InformationMessage($"{ModuleName}: Bastard Children detected (Disabling Pregnancy)", new Color(1f, 0.08f, 0.58f)));
            }
            /*
            Type? CheyronCheatsType = AccessTools.TypeByName("CheyronSubModule");
            if (CheyronCheatsType != null)
            {
                IntercourseIntention.OtherPregnancyModFound = true;
                InformationManager.DisplayMessage(new InformationMessage($"{ModuleName}: Bannerlord Trainer Plus detected (Disabling Pregnancy)", new Color(1f, 0.08f, 0.58f)));
            }
            */
            Type? MoreSpousesType = AccessTools.TypeByName("MoreSpouseSetting");
            if (MoreSpousesType != null)
            {
                BetrothIntention.OtherMarriageModFound = true;
                InformationManager.DisplayMessage(new InformationMessage($"{ModuleName}: MoreSpouses Pro detected (Disabling Marriage)", new Color(1f, 0.08f, 0.58f)));
            }
        }
    }
}