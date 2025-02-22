using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace Dramalord.Actions
{
    internal static class OrphanizeAction
    {
        internal static void Apply(Hero child)
        {
            Hero father = child.Father;
            Hero mother = child.Mother;

            father.Children.Remove(child);
            mother.Children.Remove(child);
            child.Clan = null;

            if (child.BornSettlement == null)
            {
                child.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }
            child.SetNewOccupation(Occupation.NotAssigned);
            child.UpdateHomeSettlement();

            DramalordOrphans.Instance.AddOrphan(child);
        }
    }
}
