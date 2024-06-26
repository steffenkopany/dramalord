using Bannerlord.UIExtenderEx;
using Dramalord.Behaviors;
using Dramalord.Data;
using Dramalord.Models;
using Dramalord.UI;
using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Dramalord
{
    public class DramalordSubModule : MBSubModuleBase
    {
        internal static string ModuleName = "Dramalord";
        internal static string ModuleFolder = "Dramalord";
        internal static bool Patched = false;
        internal static bool GameLoaded = false;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            UIExtender extender = new UIExtender(ModuleName);
            extender.Register(typeof(DramalordSubModule).Assembly);
            extender.Enable();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
            ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
            Campaign.Current.DefaultVillageTypes.ConsumableRawItems.Add(wurst);
            Campaign.Current.DefaultVillageTypes.ConsumableRawItems.Add(pie);
            //HeroTraits.AddToAllTraits();
            Hero.AllAliveHeroes.ForEach(item => HeroTraits.ApplyToHero(item, !GameLoaded));
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            HeroTraits.AddToAllTraits();
            GameLoaded = true;
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            Hero.AllAliveHeroes.ForEach(item => HeroTraits.ApplyToHero(item, true));
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot(); 
            Notification.PrintText("Dramalord " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3) + " loaded");
        }

        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            base.OnGameStart(game, starter);

            CampaignGameStarter? campaignGameStarter = starter as CampaignGameStarter;
            if(campaignGameStarter != null)
            {
                campaignGameStarter.AddBehavior(new DramalordCampaignBehavior(campaignGameStarter));
                campaignGameStarter.AddBehavior(new AICampaignBehavior(campaignGameStarter));
                campaignGameStarter.AddBehavior(new PlayerCampaignBehavior(campaignGameStarter));

                //campaignGameStarter.AddModel(new DramalordPregnancyModel());
                campaignGameStarter.AddModel(new DramalordMarriageModel());
                campaignGameStarter.AddModel(new DramalordRomanceModel());
                

                if (!Patched && game.GameType is Campaign)
                {
                    HarmonyPatch();
                    Patched = true;
                }

                HeroTraits.InitializeAll();
                GameLoaded = false;
            }
        }

        private void HarmonyPatch()
        {
           // try
           // {
                Harmony harmony = new Harmony(ModuleName);
                Type[] typesFromAssembly = AccessTools.GetTypesFromAssembly(typeof(DramalordSubModule).Assembly);
                foreach (Type type in typesFromAssembly)
                {
                    try
                    {
                        new PatchClassProcessor(harmony, type).Patch();
                    }
                    catch(HarmonyException e)
                    {
                        Notification.PrintText("[Dramalord] Could not apply patch " + type.Name);
                    }
                }
                //(new Harmony(ModuleName)).PatchAll();
           /* }
            catch (HarmonyException e)
            {
                Notification.PrintText("[Dramalord] Could not apply Harmony patches " + e.Message);
            }
            */
        }
    }
}