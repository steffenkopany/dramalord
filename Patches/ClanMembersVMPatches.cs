using HarmonyLib;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(ClanMembersVM), "RefreshMembersList")]
    public static class RefreshMembersListPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void RefreshMembersList(ref ClanMembersVM __instance)
        {
            List<ClanLordItemVM> otherChildren = __instance.Family.Where(item => item.IsChild && item.GetHero().Occupation == Occupation.Wanderer).ToList();
            foreach(ClanLordItemVM child in otherChildren)
            {
                __instance.Family.Remove(child);
                __instance.Companions.Add(child);
            }
        }
    }
}
