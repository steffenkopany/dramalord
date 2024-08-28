using Dramalord.Data;
using Dramalord.LogItems;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class AdoptAction
    {
        internal static void Apply(Hero hero, Hero spouse, Hero child)
        {
            DramalordOrphans.Instance.RemoveOrphan(child);

            child.Father = (hero.IsFemale) ? spouse : hero;
            child.Mother = (child.Father == hero) ? spouse : hero;
            child.Clan = hero.Clan;
            child.SetNewOccupation(hero.Occupation);

            if (hero.Occupation == Occupation.Lord)
            {
                child.SetName(child.FirstName, child.FirstName);
            }

            child.UpdateHomeSettlement();

            if (hero.Clan == Clan.PlayerClan)
            {
                TextObject textObject = new TextObject("{=Dramalord251}{HERO1.LINK} adopted orphan {CHILD.LINK}");
                StringHelpers.SetCharacterProperties("HERO1", hero.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 0, hero.CharacterObject, "event:/ui/notification/relation");
            }

            if (hero.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new AdoptChildLog(hero, child));
            }
        }
    }
}
