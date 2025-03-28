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
                hero.ExSpouses.Remove(target);
                target.ExSpouses.Remove(hero);

                hero.Spouse = target;
                target.Spouse = hero;

            }

            if(hero == Hero.MainHero || target == Hero.MainHero || relationType == RelationshipType.Spouse)
            {
                relation.IsKnownToPlayer = true;
            }

            if (hero == Hero.MainHero || target == Hero.MainHero)
            {
                if(relationType == RelationshipType.FriendWithBenefits || relationType == RelationshipType.Lover)
                {
                    relation.Rules = RelationshipRule.Open;
                }
                else if (relationType == RelationshipType.Betrothed || relationType == RelationshipType.Spouse)
                {
                    Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                    relation.Rules = otherHero.GetDefaultRelationshipRule();
                }
            }
            else if (relationType == RelationshipType.Betrothed || relationType == RelationshipType.Spouse)
            {
                RelationshipRule rule1 = hero.GetDefaultRelationshipRule();
                RelationshipRule rule2 = target.GetDefaultRelationshipRule();
                relation.Rules = (rule1<rule2) ? rule1 : rule2;
            }
        }
    }
}
