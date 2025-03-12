using Dramalord.Actions;
using HarmonyLib;
using Helpers;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(HeroCreator), "DecideBornSettlement")]
    internal class DecideBornSettlementPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool DecideBornSettlement(ref Hero child, ref Settlement __result)
        {
            Settlement settlement;
            if (child.Mother.CurrentSettlement != null && (child.Mother.CurrentSettlement.IsTown || child.Mother.CurrentSettlement.IsVillage))
            {
                settlement = child.Mother.CurrentSettlement;
            }
            else if (child.Mother.PartyBelongedTo != null || child.Mother.PartyBelongedToAsPrisoner != null)
            {
                IMapPoint? toMapPoint;
                if (child.Mother.PartyBelongedToAsPrisoner != null)
                {
                    IMapPoint? mapPoint;
                    if (!child.Mother.PartyBelongedToAsPrisoner.IsMobile)
                    {
                        IMapPoint settlement2 = child.Mother.PartyBelongedToAsPrisoner.Settlement;
                        mapPoint = settlement2;
                    }
                    else
                    {
                        IMapPoint settlement2 = child.Mother.PartyBelongedToAsPrisoner.MobileParty;
                        mapPoint = settlement2;
                    }

                    toMapPoint = mapPoint;
                }
                else
                {
                    toMapPoint = child.Mother.PartyBelongedTo;
                }

                settlement = SettlementHelper.FindNearestTown(null, toMapPoint);
            }
            else
            {
                settlement = child.Mother.HomeSettlement;
            }

            if (settlement == null)
            {
                settlement = ((child.Mother.Clan?.Settlements.Count > 0) ? child.Mother.Clan.Settlements.GetRandomElement() : Town.AllTowns.GetRandomElement().Settlement);
            }

            __result =  settlement;
            return false;
        }
    }

    [HarmonyPatch(typeof(HeroCreator), "DeliverOffSpring")]
    internal class DeliverOffSpringPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool DeliverOffSpring(ref Hero mother, ref Hero father, ref bool isOffspringFemale, ref Hero __result)
        {
            if(mother.IsLord && father.IsLord)
            {
                return true;
            }

            try
            {
                __result = GiveBirthAction.CreateBaby(mother, father);
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
