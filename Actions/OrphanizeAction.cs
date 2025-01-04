using Dramalord.Data;
using Dramalord.LogItems;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class OrphanizeAction
    {
        internal static void Apply(Hero hero, Hero child)
        {
            Hero father = child.Father;
            Hero mother = child.Mother;
            Clan oldClan = child.Clan;

            father.Children.Remove(child);
            mother.Children.Remove(child);
            child.Clan = null;

            if (child.BornSettlement == null)
            {
                child.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }
            child.SetNewOccupation(Occupation.Wanderer);
            child.UpdateHomeSettlement();

            DramalordOrphans.Instance.AddOrphan(child);

            if(oldClan == Clan.PlayerClan)
            {
                TextObject textObject = new TextObject("{=Dramalord250}{HERO1.LINK} put child {CHILD.LINK} into an orphanage.");
                StringHelpers.SetCharacterProperties("HERO1", hero.CharacterObject, textObject);
                StringHelpers.SetCharacterProperties("CHILD", child.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 0, hero.CharacterObject, "event:/ui/notification/relation");
            }

            if (oldClan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new OrphanizeChildLog(hero, child));
            }
        }
    }
}
