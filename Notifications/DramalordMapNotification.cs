using Dramalord.Data.Events.Interfaces;
using Dramalord.Quests;
using Dramalord.UI;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Notifications
{
    public class DramalordEventNotification : InformationData
    {
        private static bool _registered = false;

        public override TextObject TitleText => Event as IEncyclopediaLog != null ? (Event as IEncyclopediaLog).GetEncyclopediaText() : TextObject.GetEmpty();

        public override string SoundEventPath => "";

        [SaveableProperty(1)]
        public IDramalordEvent Event { get; private set; }

        public DramalordEventNotification(IDramalordEvent dramalordEvent, TextObject description)
            : base(description)
        {
            Event = dramalordEvent;
            RegisterIfNeeded();
        }

        private static void RegisterIfNeeded()
        {
            if (_registered) return;

            MapScreen.Instance?.MapNotificationView?.RegisterMapNotificationType(
                typeof(DramalordEventNotification),
                typeof(DramalordEventNotificationItemVM)
            );

            _registered = true;
        }
    }

    public class DramalordQuestNotification : InformationData
    {
        private static bool _registered = false;

        public override TextObject TitleText => Quest.Title;

        public override string SoundEventPath => "";

        [SaveableProperty(1)]
        public DramalordQuest Quest { get; private set; }

        public DramalordQuestNotification(DramalordQuest quest)
            : base(quest.Description)
        {
            Quest = quest;
            RegisterIfNeeded();
        }

        private static void RegisterIfNeeded()
        {
            if (_registered) return;

            MapScreen.Instance?.MapNotificationView?.RegisterMapNotificationType(
                typeof(DramalordQuestNotification),
                typeof(DramalordQuestNotificationItemVM)
            );

            _registered = true;
        }
    }
}
