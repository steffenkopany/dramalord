using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Dramalord.Actions
{
    internal class TalkAction
    {
        internal static void Apply(Hero hero, Hero target, out int trustGain, int modifier)
        {
            int sympathy = hero.GetSympathyTo(target);
            trustGain = MBMath.ClampInt(sympathy, 0, 100) * modifier;
        }
    }
}
