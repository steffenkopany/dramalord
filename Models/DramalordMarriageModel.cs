using Dramalord.Actions;
using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Models
{
    internal class DramalordMarriageModel : DefaultMarriageModel
    {
        public override bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero)
        {
            if(!HeroMarriageAction.IsDramalordMarriage)
            {
                return base.IsCoupleSuitableForMarriage(firstHero, secondHero);
            }

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

                return firstTraits.IsEmotionallyOpen && secondTraits.IsEmotionallyOpen;
            }
            return false;
        }

        public override bool IsClanSuitableForMarriage(Clan clan)
        {
            if (!HeroMarriageAction.IsDramalordMarriage)
            {
                return base.IsClanSuitableForMarriage(clan);
            }

            if (clan != null && !clan.IsBanditFaction)
            {
                return !clan.IsRebelClan;
            }
            return false;
        }

        public override bool IsSuitableForMarriage(Hero maidenOrSuitor)
        {
            if (!HeroMarriageAction.IsDramalordMarriage)
            {
                return base.IsSuitableForMarriage(maidenOrSuitor);
            }
            return maidenOrSuitor.IsDramalordLegit() && maidenOrSuitor.Age > 18 && ( maidenOrSuitor.Spouse == null || maidenOrSuitor == Hero.MainHero || maidenOrSuitor.GetDramalordTraits().IsEmotionallyOpen);
        }

        public override Clan GetClanAfterMarriage(Hero firstHero, Hero secondHero)
        {
            if (!HeroMarriageAction.IsDramalordMarriage)
            {
                return base.GetClanAfterMarriage(firstHero, secondHero);
            }

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
