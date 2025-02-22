using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Notifications.Logs
{
    internal class StartRelationshipLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero1;

        [SaveableField(2)]
        public readonly Hero Hero2;

        [SaveableField(3)]
        public readonly int RelationType;

        public StartRelationshipLog(Hero hero1, Hero hero2, RelationshipType type)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            RelationType = (int)type;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.RelationshipLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord015}{HERO1.LINK} and {HERO2.LINK} are now {RELATIONSHIP}");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            RelationshipType relationType = (RelationshipType)RelationType;
            if (relationType == RelationshipType.Friend) MBTextManager.SetTextVariable("RELATIONSHIP", new TextObject("{=Dramalord010}friends"));
            if (relationType == RelationshipType.FriendWithBenefits) MBTextManager.SetTextVariable("RELATIONSHIP", new TextObject("{=Dramalord011}friends with benefits"));
            if (relationType == RelationshipType.Lover) MBTextManager.SetTextVariable("RELATIONSHIP", new TextObject("{=Dramalord012}lovers"));
            if (relationType == RelationshipType.Betrothed) MBTextManager.SetTextVariable("RELATIONSHIP", new TextObject("{=Dramalord013}engaged"));
            if (relationType == RelationshipType.Spouse) MBTextManager.SetTextVariable("RELATIONSHIP", new TextObject("{=Dramalord014}married"));

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
