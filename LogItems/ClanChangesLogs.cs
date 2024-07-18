using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.LogItems
{

    public class LeaveClanLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero;

        [SaveableField(2)]
        public readonly Clan OldClan;

        [SaveableField(3)]
        public readonly Hero CausedBy;

        public LeaveClanLog(Hero hero, Clan oldClan, Hero causedBy)
        {
            Hero = hero;
            OldClan = oldClan;
            CausedBy = causedBy;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ClanChangeLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject();
            if (CausedBy == Hero)
            {
                textObject = new TextObject("{=Dramalord085}{HERO.LINK} has left {OLDCLAN}");
            }
            else
            {
                textObject = new TextObject("{=Dramalord086}{HERO.LINK} was thrown out of {OLDCLAN} by {CAUSEDBY.LINK}");
            }
            StringHelpers.SetCharacterProperties("HERO", Hero.CharacterObject, textObject);
            textObject.SetTextVariable("OLDCLAN", OldClan.EncyclopediaLinkWithName);
            StringHelpers.SetCharacterProperties("CAUSEDBY", CausedBy.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == OldClan || obj == CausedBy;
        }
    }

    public class JoinClanLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Hero Hero;

        [SaveableField(2)]
        public readonly Clan Clan;

        public JoinClanLog(Hero hero, Clan clan)
        {
            Hero = hero;
            Clan = clan;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ClanChangeLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord087}{HERO.LINK} joined {CLAN}.");
            StringHelpers.SetCharacterProperties("HERO", Hero.CharacterObject, textObject);
            textObject.SetTextVariable("CLAN", Clan.EncyclopediaLinkWithName);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == Clan;
        }
    }
}
