using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal static class IntercourseAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            HeroRelation heroRelation = hero.GetRelationTo(target);
            HeroDesires heroDesires = hero.GetDesires();
            HeroDesires targetDesires = target.GetDesires();

            heroDesires.Horny = 0;
            targetDesires.Horny = 0;

            heroRelation.Love += (heroDesires.IntercourseSkill + targetDesires.IntercourseSkill) / 20;

            heroDesires.IntercourseSkill += (targetDesires.IntercourseSkill > heroDesires.IntercourseSkill) ? 2 : 1;
            targetDesires.IntercourseSkill += (targetDesires.IntercourseSkill < heroDesires.IntercourseSkill) ? 2 : 1;
        }
    }
}
