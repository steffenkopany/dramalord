using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Dramalord.Actions
{
    internal class DateAction
    {
        internal static void Apply(Hero hero, Hero target, out int loveGain, out int trustGain, int modifier)
        {
            HeroDesires heroDesires = hero.GetDesires();
            HeroDesires targetDesires = target.GetDesires();

            int sympathy = hero.GetSympathyTo(target);
            int heroAttraction = hero.GetAttractionTo(target);
            int tagetAttraction = target.GetAttractionTo(hero);

            heroDesires.Horny += heroAttraction / 10;
            targetDesires.Horny += tagetAttraction / 10;

            int attractionBonus = ((heroAttraction / 20) + (tagetAttraction / 20)) / 2;

            loveGain = MBMath.ClampInt((sympathy + attractionBonus), 0, 100) * modifier; 
            trustGain = MBMath.ClampInt(sympathy, 0, 100) * modifier;
        }
    }
}
