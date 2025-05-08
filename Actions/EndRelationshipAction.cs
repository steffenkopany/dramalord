using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal static class EndRelationshipAction
    {
        internal static void Apply(Hero hero, Hero target, HeroRelation relation)
        {
            if (!BetrothIntention.OtherMarriageModFound && hero.IsSpouseOf(target))
            {
                foreach (Romance.RomanticState romanticState in Romance.RomanticStateList.ToList())
                {
                    if ((romanticState.Person1 == target && romanticState.Person2 == hero) || (romanticState.Person1 == hero && romanticState.Person2 == target))
                    {
                        romanticState.Level = Romance.RomanceLevelEnum.FailedInPracticalities;
                    }
                }
                if(hero == Hero.MainHero || target == Hero.MainHero)
                {
                    Hero other = hero == Hero.MainHero ? target : hero;
                    other.Spouse = null;
                    Hero.MainHero.Spouse = Hero.MainHero.GetAllRelations().FirstOrDefault(r => r.Value.Relationship == RelationshipType.Spouse).Key ?? null;
                }
                else
                {
                    hero.Spouse = null;
                    target.Spouse = null;
                }

                if(!hero.ExSpouses.Contains(target))
                {
                    hero.ExSpouses.Add(target);
                }
                if (!target.ExSpouses.Contains(hero))
                {
                    target.ExSpouses.Add(hero);
                }
            }

            relation.Relationship = RelationshipType.None;
            relation.Rules = RelationshipRule.Faithful;
        }
    }
}
