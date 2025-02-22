using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Notifications.Logs
{
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
}
