using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Models
{
    public class DramalordRomanceModel : DefaultRomanceModel
    {
        public override int GetAttractionValuePercentage(Hero potentiallyInterestedCharacter, Hero heroOfInterest)
        {
            if (potentiallyInterestedCharacter.IsDramalordLegit() && heroOfInterest.IsDramalordLegit())
            {
                return potentiallyInterestedCharacter.GetDramalordAttractionTo(heroOfInterest);
            }
            return 0;
        }
    }
}
