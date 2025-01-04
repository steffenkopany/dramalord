using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.LogItems
{
    public class ConceiveChildLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero1;

        [SaveableField(2)]
        public readonly Hero Hero2;

        public ConceiveChildLog(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ChildrenEventLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord164}{HERO1.LINK} was impregnated by {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.IsFemale ? Hero1.CharacterObject : Hero2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero1.IsFemale ? Hero2.CharacterObject : Hero1.CharacterObject, textObject);
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

    public class AdoptChildLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero1;

        [SaveableField(2)]
        public readonly Hero Child;

        public AdoptChildLog(Hero hero1,  Hero child)
        {
            Hero1 = hero1;
            Child = child;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ChildrenEventLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord251}{HERO1.LINK} adopted orphan {CHILD.LINK}");

            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Child.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Child;
        }
    }

    public class AbortChildLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero1;

        [SaveableField(2)]
        public readonly Hero Hero2;

        public AbortChildLog(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ChildrenEventLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord519}{HERO.LINK} aborted their unborn child of {HERO2.LINK}.");
            StringHelpers.SetCharacterProperties("HERO", Hero1.IsFemale ? Hero1.CharacterObject : Hero2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero1.IsFemale ? Hero2.CharacterObject : Hero1.CharacterObject, textObject);
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
