using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class LoverAction
    {
        internal static bool Apply(Hero hero, Hero target)
        {
            hero.GetRelationTo(target).Relationship = RelationshipType.Lover;

            if (hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord106}You are now the lover of {HERO.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            hero.ExSpouses.Remove(target);
            target.ExSpouses.Remove(hero);
            var ex1 = hero.ExSpouses.Distinct().Where(h => h != null).ToList();
            hero.ExSpouses.Clear();
            hero.ExSpouses.AddRange(ex1);
            var ex2 = target.ExSpouses.Distinct().Where(h => h != null).ToList();
            target.ExSpouses.Clear();
            target.ExSpouses.AddRange(ex2);

            if ((hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new StartRelationshipLog(hero, target, RelationshipType.Lover));
            }

            return true;
        }
    }
}
