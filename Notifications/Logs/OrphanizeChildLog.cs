using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Notifications.Logs
{
    public class OrphanizeChildLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero;

        [SaveableField(2)]
        public readonly Hero Child;

        public OrphanizeChildLog(Hero hero1, Hero child)
        {
            Hero = hero1;
            Child = child;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ChildrenEventLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectPositive;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord250}{HERO1.LINK} put child {CHILD.LINK} into an orphanage.");
            StringHelpers.SetCharacterProperties("HERO1", Hero.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Child.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == Child;
        }
    }
}
