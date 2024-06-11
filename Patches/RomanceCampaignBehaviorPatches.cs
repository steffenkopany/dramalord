using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(RomanceCampaignBehavior), "conversation_finalize_courtship_for_hero_on_condition")]
    internal static class conversation_finalize_courtship_for_hero_on_condition_Patch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool conversation_finalize_courtship_for_hero_on_condition(ref bool __result)
        {
            __result = false;
            if (Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(Hero.MainHero, Hero.OneToOneConversationHero))
            {
                bool result = !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction);

                if (result && Hero.OneToOneConversationHero.Clan?.Leader == Hero.OneToOneConversationHero && Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.CoupleAgreedOnMarriage)
                {
                    __result = true;
                }
            }

            return false;
        }
    }
}
