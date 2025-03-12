using Dramalord.Data;
using Dramalord.Extensions;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Dramalord.Actions
{
    internal static class StartRelationshipAction
    {
        internal static void Apply(Hero hero, Hero target, HeroRelation relation, RelationshipType relationType)
        {
            if (relation.Relationship == relationType)
            {
                return;
            }

            //RelationshipType oldRelation = relation.Relationship;
            EndRelationshipAction.Apply(hero, target, relation);
            relation.Relationship = relationType;

            if(relationType == RelationshipType.Spouse)
            {
                // Only the main hero keeps their old spouses - npcs are divorced!
                if(hero != Hero.MainHero && hero.Spouse != null)
                {
                    EndRelationshipAction.Apply(hero, hero.Spouse, hero.GetRelationTo(hero.Spouse));
                }

                // Only the main hero keeps their old spouses - npcs are divorced!
                if (target != Hero.MainHero && target.Spouse != null)
                {
                    EndRelationshipAction.Apply(target, target.Spouse, target.GetRelationTo(target.Spouse));
                }

                ChangeRomanticStateAction.Apply(hero, target, Romance.RomanceLevelEnum.Marriage);
                //if (hero.Spouse != null && !hero.ExSpouses.Contains(hero.Spouse)) hero.ExSpouses.Add(hero.Spouse);
                //if (target.Spouse != null && !target.ExSpouses.Contains(target.Spouse)) target.ExSpouses.Add(hero.Spouse);
                hero.ExSpouses.Remove(target);
                target.ExSpouses.Remove(hero);
/*
                hero.Spouse = target;
                target.Spouse = hero;
                var ex1 = hero.ExSpouses.Distinct().Where(h => h != null).ToList();
                hero.ExSpouses.Clear();
                hero.ExSpouses.AddRange(ex1);
                var ex2 = target.ExSpouses.Distinct().Where(h => h != null).ToList();
                target.ExSpouses.Clear();
                target.ExSpouses.AddRange(ex2);
*/
            }

            if(hero == Hero.MainHero || target == Hero.MainHero || relationType == RelationshipType.Spouse)
            {
                relation.IsKnownToPlayer = true;
            }
        }
    }
}
