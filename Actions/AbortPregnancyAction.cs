using Dramalord.Data;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal static class AbortPregnancyAction
    {
        internal static void Apply(Hero mother)
        {
            DramalordPregnancies.Instance.RemovePregnancy(mother);
            mother.IsPregnant = false;
        }
    }
}
