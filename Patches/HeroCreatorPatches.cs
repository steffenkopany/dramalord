using Dramalord.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(HeroCreator), "DeliverOffSpring")]
    internal static class DeliverOffSpringPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static void DeliverOffSpring(ref Hero mother, ref Hero father, ref bool isOffspringFemale)
        {
            if(father == null)
            {
                father = mother;
                /*
                if(mother.ExSpouses?.Count() > 0)
                {
                    father = mother.ExSpouses.GetRandomElement();
                }
                else
                {
                    father = Hero.AllAliveHeroes.GetRandomElementWithPredicate(h => !h.IsFemale && h.IsDramalordLegit() && h.Clan != Clan.PlayerClan);
                }
                */
            }
            if(DramalordMCM.Instance.NativeBirthDebug)
            {
                string sex = isOffspringFemale ? "female" : "male";
                InformationManager.DisplayMessage(new InformationMessage($"[Native birth] Mother:{mother.Name}, Father:{father.Name}, Child will be {sex}", new Color(1f, 0.08f, 0.58f)));
            }
        }
    }
}
