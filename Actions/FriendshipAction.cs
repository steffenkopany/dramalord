using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class FriendshipAction
    {
        internal static bool Apply(Hero hero, Hero target)
        {
            hero.GetRelationTo(target).Relationship = RelationshipType.Friend;

            if (hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord079}You and {HERO.LINK} are now friends.");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            if ((hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new StartRelationshipLog(hero, target, RelationshipType.Friend));
            }

            return true;
        }
    }
}
