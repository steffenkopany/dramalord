using Dramalord.Data.Events;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

/*
namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(HeroCreator), "DeliverOffSpring")]
    internal class DeliverOffSpringPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool DeliverOffSpring(ref Hero mother, ref Hero father, ref bool isOffspringFemale, ref Hero __result)
        {
            if(mother != null && mother.IsLord && father != null && father.IsLord)
            {
                return true;
            }

            try
            {
                __result = BirthEvent.CreateBaby(mother, father);
                mother.IsPregnant = false;
            }
            catch
            {
                __result = null;
            }
            return false;
        }
    }
}
*/
