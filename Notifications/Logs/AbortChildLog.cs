using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Notifications.Logs
{
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
