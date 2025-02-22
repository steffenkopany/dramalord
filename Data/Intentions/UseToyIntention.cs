using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class UseToyIntention : Intention
    {
        public UseToyIntention(Hero intentionHero, CampaignTime validUntil) : base(intentionHero, intentionHero, validUntil)
        {
        }

        public override bool Action()
        {
            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance.ToyBreakChance)
            {
                IntentionHero.GetDesires().HasToy = false;
                TextObject textObject = new TextObject("{=Dramalord297}{HERO.LINK}s toy broke!");
                StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
            }
            else
            {
                HeroRelation relation = IntentionHero.GetRelationTo(Hero.MainHero);
                HeroDesires desires = IntentionHero.GetDesires();

                relation.Love += 1;
                if (desires.Horny > DramalordMCM.Instance.MinTrustFWB / 2)
                {
                    desires.Horny = DramalordMCM.Instance.MinTrustFWB / 2;
                }
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
