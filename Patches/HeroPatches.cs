using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(Hero), "GetRelation")]
    public static class HeroGetRelationPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void GetRelation(ref Hero otherHero, ref Hero __instance, ref int __result)
        {
            if(DramalordMCM.Instance.ShowRealrelation && otherHero != null && __instance != null && otherHero != __instance)
            {
                __result = Campaign.Current.Models.DiplomacyModel.GetBaseRelation(__instance, otherHero);
            }
        }
    }

    [HarmonyPatch(typeof(Hero), "IsEnemy")]
    public static class HeroIsEnemyPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void IsEnemy(ref Hero otherHero, ref Hero __instance, ref bool __result)
        {
            __result = CharacterRelationManager.GetHeroRelation(__instance, otherHero) <= DramalordMCM.Instance.MaxTrustEnemies;
        }
    }

    [HarmonyPatch(typeof(Hero), "IsFriend")]
    public static class HeroIsFriendPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void IsFriend(ref Hero otherHero, ref Hero __instance, ref bool __result)
        {
            __result = CharacterRelationManager.GetHeroRelation(__instance, otherHero) >= DramalordMCM.Instance.MinTrustFriends;
        }
    }
/*
    [HarmonyPatch(typeof(Hero), "CanMoveToSettlement")]
    public static class CanMoveToSettlementPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void CanMoveToSettlement(ref Hero __instance, ref bool __result)
        {
            __result = __instance.PartyBelongedTo == MobileParty.MainParty && DramalordQuests.Instance.GetQuest(__instance) as JoinPlayerQuest != null ? true : __result;
        }
    }
*/
}
