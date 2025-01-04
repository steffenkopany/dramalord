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

    public class JoinKingdomLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Clan Clan;

        [SaveableField(2)]
        public readonly Kingdom Kingdom;

        public JoinKingdomLog(Clan clan, Kingdom kingdom)
        {
            Clan = clan;
            Kingdom = kingdom;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ClanChangeLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord492}{CLAN} joined {KINGDOM}.");
            textObject.SetTextVariable("CLAN", Clan.EncyclopediaLinkWithName);
            textObject.SetTextVariable("KINGDOM", Kingdom.EncyclopediaLinkWithName);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Kingdom || obj == Clan;
        }
    }

    public class LeaveKingdomLog : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(1)]
        public readonly Clan Clan;

        [SaveableField(2)]
        public readonly Kingdom Kingdom;

        public LeaveKingdomLog(Clan clan, Kingdom kingdom)
        {
            Clan = clan;
            Kingdom = kingdom;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance?.ClanChangeLogs ?? true;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord493}{CLAN} has left {KINGDOM}.");
            textObject.SetTextVariable("CLAN", Clan.EncyclopediaLinkWithName);
            textObject.SetTextVariable("KINGDOM", Kingdom.EncyclopediaLinkWithName);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Kingdom || obj == Clan;
        }
    }
}
