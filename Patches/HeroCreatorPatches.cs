using Dramalord.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

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
                if(mother.ExSpouses?.Count() > 0)
                {
                    father = mother.ExSpouses.GetRandomElement();
                }
                else
                {
                    father = Hero.AllAliveHeroes.GetRandomElementWithPredicate(h => !h.IsFemale && h.IsDramalordLegit() && h.Clan != Clan.PlayerClan);
                }
            }
        }
    }
}
