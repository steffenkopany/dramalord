using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.LogItems
{
    internal class PrisonIntercourseLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero1;

        [SaveableField(2)]
        public readonly Hero Hero2;

        public PrisonIntercourseLog(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.RelationshipLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord285}{HERO1.LINK} violated their captive {HERO2.LINK}.");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2;
        }
    }
}
