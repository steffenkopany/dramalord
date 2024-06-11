using Dramalord.Data;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;

namespace Dramalord.Models
{
    internal class DramalordPregnancyModel : DefaultPregnancyModel
    {
        public override float PregnancyDurationInDays => DramalordMCM.Get.PregnancyDuration;

        public override float MaternalMortalityProbabilityInLabor => 0.0f;

        public override float StillbirthProbability => 0.0f;

        public override float DeliveringFemaleOffspringProbability => 0.51f;

        public override float DeliveringTwinsProbability => 0.0f;

        public override float GetDailyChanceOfPregnancyForHero(Hero hero)
        {
            if(hero.Spouse == null || !DramalordMCM.Get.AllowDefaultPregnancies)
            {
                return 0;
            }

            int num = hero.Children.Count + 1;
            float num2 = 4 + 4 * hero.Clan.Tier;
            int num3 = hero.Clan.Lords.Count((Hero x) => x.IsAlive);
            float num4 = ((hero != Hero.MainHero && hero.Spouse != Hero.MainHero) ? Math.Min(1f, (2f * num2 - (float)num3) / num2) : 1f);
            float num5 = (1.2f - (hero.Age - 18f) * 0.04f) / (float)(num * num) * 0.12f * num4;
            float baseNumber = ((hero.Spouse != null && hero.GetDramalordIsFertile()) ? num5 : 0f);
            ExplainedNumber explainedNumber = new ExplainedNumber(baseNumber);
            if (hero.GetPerkValue(DefaultPerks.Charm.Virile) || hero.Spouse.GetPerkValue(DefaultPerks.Charm.Virile))
            {
                explainedNumber.AddFactor(DefaultPerks.Charm.Virile.PrimaryBonus, DefaultPerks.Charm.Virile.Name);
            }

            return explainedNumber.ResultNumber;
        }
    }
}
