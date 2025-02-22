using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(PartyCharacterVM), "get_CanTalk")]
    public static class PartyCharacterVMPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void get_CanTalk(ref PartyCharacterVM __instance, ref bool __result)
        {
            if(__instance.Troop.Character.IsHero && !__instance.Troop.Character.HeroObject.IsPrisoner)
            {
                bool flag = __instance.Side == PartyScreenLogic.PartyRosterSide.Right;
                bool num = __instance.Troop.Character != CharacterObject.PlayerCharacter;
                bool isHero = __instance.Troop.Character.IsHero;
                bool flag2 = CampaignMission.Current == null;
                bool flag4 = MobileParty.MainParty.MapEvent == null;
                __result = num && flag && isHero && flag2 && flag4;
            }
        }
    }
}
