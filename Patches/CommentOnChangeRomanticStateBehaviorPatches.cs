using HarmonyLib;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(CommentOnChangeRomanticStateBehavior), "OnRomanticStateChanged")]
    public static class OnRomanticStateChangedPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool OnRomanticStateChanged(ref Hero hero1, ref Hero hero2, ref Romance.RomanceLevelEnum level)
        {
            if (hero1 == Hero.MainHero || hero2 == Hero.MainHero || hero1.Clan?.Leader == hero1 || hero2.Clan?.Leader == hero2)
            {
                LogEntry.AddLogEntry(new ChangeRomanticStateLogEntry(hero1, hero2, level));
            }
            return false;
        }
    } 
}
