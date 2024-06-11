using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Models
{
    internal class DramalordMarriageModel : DefaultMarriageModel
    {
        public override bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero)
        {
            if(IsSuitableForMarriage(firstHero) && IsSuitableForMarriage(secondHero) && (firstHero.Clan != null || secondHero.Clan != null))
            {
                if(firstHero.Spouse == null && secondHero.Spouse == null)
                {
                    return true;
                }

                if(firstHero.IsDramalordRelativeTo(secondHero) && DramalordMCM.Get.ProtectFamily)
                {
                    return false;
                }

                if(firstHero == Hero.MainHero || secondHero == Hero.MainHero)
                {
                    return true;
                }

                DramalordTraits firstTraits = firstHero.GetDramalordTraits();
                DramalordTraits secondTraits = secondHero.GetDramalordTraits();

                bool firstPoly = firstTraits.Openness == 2 && firstTraits.Agreeableness == 2 && firstTraits.Extroversion > 0 && firstTraits.Neuroticism < 0;
                bool secondPoly = secondTraits.Openness == 2 && secondTraits.Agreeableness == 2 && secondTraits.Extroversion > 0 && secondTraits.Neuroticism < 0;

                return firstPoly && secondPoly;
            }
            return false;
        }

        public override bool IsClanSuitableForMarriage(Clan clan)
        {
            if(DramalordMCM.Get.AllowDefaultMarriages)
            {
                if (clan != null && !clan.IsBanditFaction)
                {
                    return !clan.IsRebelClan;
                }
            }

            return false;
        }

        public override bool IsSuitableForMarriage(Hero maidenOrSuitor)
        {
            return maidenOrSuitor.IsDramalordLegit() && maidenOrSuitor.Age > 18;
        }

        public override Clan GetClanAfterMarriage(Hero firstHero, Hero secondHero)
        {
            if (firstHero.IsHumanPlayerCharacter)
            {
                return firstHero.Clan;
            }

            if (secondHero.IsHumanPlayerCharacter)
            {
                return secondHero.Clan;
            }

            if (firstHero.Clan != null && firstHero.Clan.Leader == firstHero)
            {
                return firstHero.Clan;
            }

            if (secondHero.Clan != null && secondHero.Clan.Leader == secondHero)
            {
                return secondHero.Clan;
            }

            if(firstHero.Clan != null && secondHero.Clan == null)
            {
                return firstHero.Clan;
            }

            if(secondHero.Clan != null && firstHero.Clan == null)
            {
                return secondHero.Clan;
            }

            if (!firstHero.IsFemale)
            {
                return firstHero.Clan;
            }

            return secondHero.Clan;
        }
    }
}
