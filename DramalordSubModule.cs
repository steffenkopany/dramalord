using Dramalord.Behaviors;
using Dramalord.UI;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
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
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if(!Patched && game.GameType is Campaign)
            {
                HarmonyPatch();
                Patched = true;
            }

            ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
            ItemObject leek = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_leek");
            ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
            Campaign.Current.DefaultVillageTypes.ConsumableRawItems.Add(wurst);
            Campaign.Current.DefaultVillageTypes.ConsumableRawItems.Add(leek);
            Campaign.Current.DefaultVillageTypes.ConsumableRawItems.Add(pie);
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            Notification.PrintText("Dramalord loaded");
        }
        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            base.OnGameStart(game, starter);

            CampaignGameStarter campaignGameStarter = (CampaignGameStarter)starter;
            campaignGameStarter.AddBehavior((CampaignBehaviorBase) new DramalordCampaignBehavior(campaignGameStarter));
        }

        private void HarmonyPatch()
        {
            try
            {
                (new Harmony(ModuleName)).PatchAll();
            }
            catch (HarmonyException e)
            {
                Notification.PrintText("[Dramalord] Could not apply Harmony patches " + e.Source);
            }
            
        }
    }
}