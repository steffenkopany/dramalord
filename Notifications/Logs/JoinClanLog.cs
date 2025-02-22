using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Notifications.Logs
{
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
