using Dramalord.Data;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal static class ConceiveAction
    {
        internal static void Apply(Hero mother, Hero father)
        {
            if (mother.IsPregnant || DramalordPregnancies.Instance.GetPregnancy(mother) != null)
            {
                return;
            }

            DramalordPregnancies.Instance.AddPregnancy(mother, father, CampaignTime.Now);
            mother.IsPregnant = true;
        }
    }
}
