using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Notifications.Logs
{
    public class BirthChildLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero1;

        [SaveableField(2)]
        public readonly Hero Hero2;

        [SaveableField(3)]
        public readonly Hero Child;

        public BirthChildLog(Hero hero1, Hero hero2, Hero child)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Child = child;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ChildrenEventLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord165}{CHILD.LINK} was born to {HERO1.LINK} and {HERO2.LINK}");

            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Child.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2 || obj == Child;
        }
    }
}
