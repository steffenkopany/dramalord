using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Localization;

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

    [HarmonyPatch(typeof(RomanceCampaignBehavior), "conversation_player_can_open_courtship_on_condition")]
    internal static class conversation_player_can_open_courtship_on_condition_Patch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool conversation_player_can_open_courtship_on_condition(ref bool __result)
        {
            __result = false;
            if (Hero.OneToOneConversationHero == null || (Hero.OneToOneConversationHero.MapFaction?.IsMinorFaction ?? false) || Hero.OneToOneConversationHero.IsPrisoner)
            {
                __result = false;
            }
            else if (Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(Hero.MainHero, Hero.OneToOneConversationHero) && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction) && Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.Untested)
            {
                if (!Hero.OneToOneConversationHero.IsFemale)
                {
                    MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=bjJs0eeB}My lord, I note that you have not yet taken a wife.");
                }
                else
                {
                    MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=v1hC6Aem}My lady, I wish to profess myself your most ardent admirer.");
                }

                __result = true;
            }
            else if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInCompatibility || Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInPracticalities)
            {
                if (!Hero.OneToOneConversationHero.IsFemale)
                {
                    MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=2WnhUBMM}My lord, may you give me another chance to prove myself?");
                }
                else
                {
                    MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=4iTaEZKg}My lady, may you give me another chance to prove myself?");
                }

                __result = true;
            }

            return false;
        }
    }

}
