using Dramalord.Actions;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Data.Intentions
{
    internal class AdoptIntention : Intention
    {
        public AdoptIntention(Hero intentionHero, Hero child, CampaignTime validUntil) : base(intentionHero, child, validUntil)
        {
        }

        public override bool Action()
        {
            if(IntentionHero.Spouse != null)
            {
                Hero father = IntentionHero.IsFemale ? IntentionHero.Spouse : IntentionHero;
                Hero mother = father == IntentionHero ? Target : IntentionHero;

                AdoptAction.Apply(mother, father, Target);
                JoinClanAction.Apply(Target, Clan.PlayerClan);
            }

            return true;
        }

        public override void OnConversationEnded()
        {

        }

        public override void OnConversationStart()
        {
            
        }
    }
}
