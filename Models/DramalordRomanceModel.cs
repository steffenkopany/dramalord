using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using Dramalord.Data;

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
