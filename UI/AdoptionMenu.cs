using Dramalord.Conversations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Localization;

namespace Dramalord.UI
{
    internal static class AdoptionMenu
    {
        internal static void AddGameMenus(CampaignGameStarter gameStarter)
        {
            gameStarter.AddGameMenuOption("town_backstreet", "dl_orphanage_menu", "{=Dramalord286}Visit Orphanage", ConditionOrphanageAvailable, ConsequenceOrphanageAvailable, false, 2, false, null);
        }

        internal static bool ConditionOrphanageAvailable(MenuCallbackArgs args)
        {
            args.Tooltip = new TextObject("{=Dramalord287}Visit the orphanage to adopt a child or get rid off one of your clan", null);
            return true;
        }

        internal static void ConsequenceOrphanageAvailable(MenuCallbackArgs args)
        {
            OrphanageConversation.Start();
        }
    }
}
