using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal class HeroInfoData
    {
        [SaveableProperty(1)]
        internal int AttractionMen { get; set; }

        [SaveableProperty(2)]
        internal int AttractionWomen { get; set; }

        [SaveableProperty(3)]
        internal float AttractionWeight { get; set; }

        [SaveableProperty(4)]
        internal float AttractionBuild { get; set; }

        [SaveableProperty(5)]
        internal int AttractionAgeDiff { get; set; }

        [SaveableProperty(6)]
        internal float Horny { get; set; }

        [SaveableProperty(7)]
        internal float Libido { get; set; }

        [SaveableProperty(8)]
        internal float PeriodDayOfSeason { get; set; }

        [SaveableProperty(9)]
        internal float IntercourseSkill { get; set; }

        [SaveableProperty(10)]
        internal bool HasToy { get; set; }

        internal HeroInfoData(int attractionMen, int attractionWomen, float attractionWeight, float attractionBuild, int attractionAgeDiff, float horny, float libido,  float periodDayOfSeason, float intercourseSkill)
        {
            AttractionMen = attractionMen;
            AttractionWomen = attractionWomen;
            AttractionWeight = attractionWeight;
            AttractionBuild = attractionBuild;
            AttractionAgeDiff = attractionAgeDiff;
            Horny = horny;
            Libido = libido;
            PeriodDayOfSeason = periodDayOfSeason;
            IntercourseSkill = intercourseSkill;
            HasToy = false;
        }

        internal HeroInfoData(HeroInfoData other)
        {
            AttractionMen = other.AttractionMen;
            AttractionWomen = other.AttractionWomen;
            AttractionWeight = other.AttractionWeight;
            AttractionBuild = other.AttractionBuild;
            AttractionAgeDiff = other.AttractionAgeDiff;
            Horny = other.Horny;
            Libido = other.Libido;
            PeriodDayOfSeason = other.PeriodDayOfSeason;
            IntercourseSkill = other.IntercourseSkill;
            HasToy = other.HasToy;
        }
    }
}
