using Dramalord.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation.Tags;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(PlayerIsSpouseTag), "IsApplicableTo")]
    internal static class IsApplicableToPatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void IsApplicableTo(ref CharacterObject character, ref bool __result)
        {
            if (character != null && character.IsHero && character.HeroObject.IsSpouseOf(Hero.MainHero))
            {
                __result = true; 
            }
        }
    }
}
