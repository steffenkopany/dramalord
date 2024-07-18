using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(DefaultPregnancyModel), "GetDailyChanceOfPregnancyForHero")]
    public static class GetDailyChanceOfPregnancyForHeroPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool GetDailyChanceOfPregnancyForHero(ref Hero hero, ref float __result)
        {
            if(hero.Clan == null)
            {
                int num = hero.Children.Count + 1;
                float num2 = 4 + 4 * (hero.Clan?.Tier ?? 1);
                int num3 = hero.Clan?.Lords.Count((Hero x) => x.IsAlive) ?? 3;
                float num4 = ((hero != Hero.MainHero && hero.Spouse != Hero.MainHero) ? Math.Min(1f, (2f * num2 - (float)num3) / num2) : 1f);
                float num5 = (1.2f - (hero.Age - 18f) * 0.04f) / (float)(num * num) * 0.12f * num4;
                float baseNumber = ((hero.Spouse != null && hero.Age >= 18 && hero.Age < 45) ? num5 : 0f);
                ExplainedNumber explainedNumber = new ExplainedNumber(baseNumber);
                if (hero.GetPerkValue(DefaultPerks.Charm.Virile) || hero.Spouse.GetPerkValue(DefaultPerks.Charm.Virile))
                {
                    explainedNumber.AddFactor(DefaultPerks.Charm.Virile.PrimaryBonus, DefaultPerks.Charm.Virile.Name);
                }

                __result = explainedNumber.ResultNumber;
                return false;
            }
            return true;
        }
    } 
}
