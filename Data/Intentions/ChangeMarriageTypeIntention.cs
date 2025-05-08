using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Data.Intentions
{
    internal class ChangeMarriageTypeIntention : Intention
    {
        private RelationshipRule Rule;
        public ChangeMarriageTypeIntention(Hero intentionHero, Hero target, RelationshipRule rule, CampaignTime validUntil) : base(intentionHero, target, validUntil)
        {
            Rule = rule;
        }

        public override bool Action()
        {
            IntentionHero.GetRelationTo(Target).Rules = Rule;
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
