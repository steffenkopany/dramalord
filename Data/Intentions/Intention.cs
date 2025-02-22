using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    public abstract class Intention
    {
        [SaveableField(1)]
        public readonly CampaignTime ValidUntil;

        [SaveableField(2)]
        public readonly Hero IntentionHero;

        [SaveableField(3)]
        public readonly Hero Target;

        public Intention(Hero intentionHero, Hero target, CampaignTime validUntil)
        {
            IntentionHero = intentionHero;
            Target = target;
            ValidUntil = validUntil;
        }

        public abstract bool Action();

        public abstract void OnConversationStart();

        public abstract void OnConversationEnded();
    }
}
