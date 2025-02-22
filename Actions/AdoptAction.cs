using Dramalord.Data;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal static class AdoptAction
    {
        internal static void Apply(Hero mother, Hero father, Hero child)
        {
            child.Mother = mother;
            child.Father = father;

            DramalordOrphans.Instance.RemoveOrphan(child);
            child.UpdateHomeSettlement();
        }
    }
}
