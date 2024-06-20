using Dramalord.Data;
using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(ConversationManager), "IsTagApplicable")]
    internal static class IsTagApplicablePatch
    {
        [UsedImplicitly]
        [HarmonyPostfix]
        public static void IsTagApplicable(string tagId, CharacterObject character, ref bool __result)
        {
            if(character != null && character.IsHero && tagId == "PlayerIsSpouseTag" && character.HeroObject.IsSpouse(Hero.MainHero))
            {
                __result = true;
            }
        }
    }
}
