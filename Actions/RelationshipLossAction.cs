using Dramalord.Data;
using Dramalord.Extensions;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal class RelationshipLossAction
    {
        internal static void Apply(Hero hero, Hero target, out int loveLoss, out int trustLoss, int loveFactor, int trustFactor)
        {
            HeroPersonality personality = hero.GetPersonality();
            HeroRelation relation = hero.GetRelationTo(target);

            float agreeFactor = ((float)personality.Agreeableness) / 100f;
            float openFactor = ((float)personality.Openness) / 100f;
            float neuroFactor = ((float)personality.Neuroticism) / 100f;
            float conscFactor = ((float)personality.Conscientiousness) / 100f;

            float understanding = (agreeFactor + openFactor) * -1f;
            float braindriven = (neuroFactor + conscFactor);

            float lovers = (float)hero.GetAllRelations().Where(r => r.Key != target && (r.Value.Relationship == RelationshipType.Lover || r.Value.Relationship == RelationshipType.Betrothed || r.Value.Relationship == RelationshipType.Spouse)).Count();

            float lLoss = (loveFactor + lovers) * understanding;
            float tLoss = (trustFactor - lovers) * braindriven;

            loveLoss = Math.Min(Math.Max(loveFactor + (int)lLoss, 0), 100) * -1;
            trustLoss = Math.Min(Math.Max(trustFactor + (int)tLoss, 0), 100) * -1;
        }
    }
}
