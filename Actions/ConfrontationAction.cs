using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Actions
{
    internal static class ConfrontationAction
    {
        internal static void Apply(Hero hero, HeroIntention intention)
        {
            HeroEvent? @event = DramalordEvents.Instance.GetEvent(intention.EventId);
            if(@event != null)
            {
                HeroPersonality personality = hero.GetPersonality();
                HeroRelation relation = hero.GetRelationTo(intention.Target);

                float loveDamage = -100;
                float trustDamage = -100;

                if (@event.Type == EventType.Date)
                {
                    loveDamage /= 5;
                    trustDamage /= 3;
                }
                else if (@event.Type == EventType.Intercourse)
                {
                    loveDamage /= 4;
                    trustDamage /= 2;
                }
                else if (@event.Type == EventType.Betrothed)
                {
                    loveDamage *= 1;
                    trustDamage *= 1;
                }
                else if (@event.Type == EventType.Marriage)
                {
                    loveDamage *= 1.5f;
                    trustDamage *= 1f;
                }
                else if (@event.Type == EventType.Birth)
                {
                    loveDamage /= 3;
                    trustDamage *= 1.5f;
                }

                float agreeFactor = ((float)personality.Agreeableness) / 100f;
                float openFactor = ((float)personality.Openness) / 100f;
                float extroFactor = ((float)personality.Extroversion) / 100f;
                float neuroFactor = ((float)personality.Neuroticism) / 100f;
                float conscFactor = ((float)personality.Conscientiousness) / 100f;

                loveDamage -= loveDamage * agreeFactor;
                loveDamage -= loveDamage * openFactor;

                trustDamage += trustDamage * neuroFactor;
                trustDamage += trustDamage * conscFactor;

                hero.GetRelationTo(intention.Target).UpdateLove();
                hero.GetRelationTo(intention.Target).Trust += (int)trustDamage;
                hero.GetRelationTo(intention.Target).Love += (int)loveDamage;
            }
        }
    }
}
