using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Data
{
    internal static class HeroTraits
    {
        internal static List<TraitObject> AllTraits = new();

        public static TraitObject Openness = new TraitObject("PersonalityOpenness");
        public static TraitObject Conscientiousness = new TraitObject("PersonalityConscientiousness");
        public static TraitObject Extroversion = new TraitObject("PersonalityExtroversion");
        public static TraitObject Agreeableness = new TraitObject("PersonalityAgreeableness");
        public static TraitObject Neuroticism = new TraitObject("PersonalityNeuroticism");

        public static TraitObject AttractionMen = new TraitObject("DesireAttractionMen");
        public static TraitObject AttractionWomen = new TraitObject("DesireAttractionWomen");
        public static TraitObject AttractionWeight = new TraitObject("DesireAttractionWeight");
        public static TraitObject AttractionBuild = new TraitObject("DesireAttractionBuild");
        public static TraitObject AttractionAgeDiff = new TraitObject("DesireAttractionAgeDiff");
        public static TraitObject Libido = new TraitObject("DesireLibido");
        public static TraitObject Horny = new TraitObject("DesireHorny");
        public static TraitObject PeriodDayOfSeason = new TraitObject("AttributePeriodDayOfSeason");
        public static TraitObject IntercourseSkill = new TraitObject("SkillIntercourseSkill");
        public static TraitObject HasToy = new TraitObject("AttributeHasToy");

        internal static void InitializeAll()
        {
            Openness.Initialize(new TextObject("Openness"), new TextObject("Represents how willing a person is to try new things"), false, -2, 2);
            if (!AllTraits.Contains(Openness)) AllTraits.Add(Openness);

            Conscientiousness.Initialize(new TextObject("Conscientiousness"), new TextObject("Refers to an individual's desire to be careful and diligent"), false, -2, 2);
            if (!AllTraits.Contains(Conscientiousness)) AllTraits.Add(Conscientiousness);

            Extroversion.Initialize(new TextObject("Extroversion"), new TextObject("Measures how energetic, outgoing and confident a person is"), false, -2, 2);
            if (!AllTraits.Contains(Extroversion)) AllTraits.Add(Extroversion);

            Agreeableness.Initialize(new TextObject("Agreeableness"), new TextObject("Refers to how an individual interacts with others"), false, -2, 2);
            if (!AllTraits.Contains(Agreeableness)) AllTraits.Add(Agreeableness);

            Neuroticism.Initialize(new TextObject("Neuroticism"), new TextObject("Represents how much someone is inclined to experience negative emotions"), false, -2, 2);
            if (!AllTraits.Contains(Neuroticism)) AllTraits.Add(Neuroticism);

            AttractionMen.Initialize(new TextObject("Male Attraction"), new TextObject("Defines whether an individual find male persons attractive or not"), false, 0, 100);
            if (!AllTraits.Contains(AttractionMen)) AllTraits.Add(AttractionMen);

            AttractionWomen.Initialize(new TextObject("Female Attraction"), new TextObject("Defines whether an individual find female persons attractive or not"), false, 0, 100);
            if (!AllTraits.Contains(AttractionWomen)) AllTraits.Add(AttractionWomen);

            AttractionWeight.Initialize(new TextObject("Weight Attraction"), new TextObject("Defines whether an individual has interest in chubby or thin heroes"), false, 0, 100);
            if (!AllTraits.Contains(AttractionWeight)) AllTraits.Add(AttractionWeight);

            AttractionBuild.Initialize(new TextObject("Build Attraction"), new TextObject("Defines whether an individual has interest in muscular or weak heroes"), false, 0, 100);
            if (!AllTraits.Contains(AttractionBuild)) AllTraits.Add(AttractionBuild);

            AttractionAgeDiff.Initialize(new TextObject("Age Attraction"), new TextObject("Defines whether an individual has interest in older or younger heroes"), false, -20, 20);
            if (!AllTraits.Contains(AttractionAgeDiff)) AllTraits.Add(AttractionAgeDiff);

            Libido.Initialize(new TextObject("Libido"), new TextObject("Defines whether an individual has generally interest in intercourse or not"), false, 0, 10);
            if (!AllTraits.Contains(Libido)) AllTraits.Add(Libido);

            Horny.Initialize(new TextObject("Horny"), new TextObject("Represents how willing a hero is for intercourse due to hormons"), false, 0, 100);
            if (!AllTraits.Contains(Horny)) AllTraits.Add(Horny);

            PeriodDayOfSeason.Initialize(new TextObject("Period Day Of Season"), new TextObject("Day in season when period of female character starts"), false, 0, CampaignTime.DaysInSeason);
            if (!AllTraits.Contains(PeriodDayOfSeason)) AllTraits.Add(PeriodDayOfSeason);

            IntercourseSkill.Initialize(new TextObject("Intercourse Skill"), new TextObject("How skilled someone is regarding intercourse"), false, 0, 100);
            if (!AllTraits.Contains(IntercourseSkill)) AllTraits.Add(IntercourseSkill);

            HasToy.Initialize(new TextObject("Has Toy"), new TextObject("Flag indicating whether a hero posses a toy or not"), false, 0, 1);
            if (!AllTraits.Contains(HasToy)) AllTraits.Add(HasToy);


            foreach (TraitObject trait in AllTraits)
            {
                if(!Game.Current.ObjectManager.ContainsObject<TraitObject>(trait.StringId))
                {
                    Game.Current.ObjectManager.RegisterObject<TraitObject>(trait);
                }
            }
        }

        internal static void AddToAllTraits()
        {
            foreach (TraitObject trait in AllTraits)
            {
                if (!TraitObject.All.Contains(trait))
                {
                    TraitObject.All.Add(trait);
                }
            }
        }

        internal static void ApplyToHero(Hero hero)
        {
            if (hero.IsLord || hero.IsWanderer)
            {
                foreach (TraitObject trait in AllTraits)
                {
                    if (!hero.GetHeroTraits().HasProperty(trait))
                    {
                        hero.GetHeroTraits().SetPropertyValue(trait, MBRandom.RandomInt(trait.MinValue, trait.MaxValue));
                    }
                }
            }
        }

        internal static TraitObject? GetTraitObject(string stringId)
        {
            return AllTraits.FirstOrDefault( item => item.StringId == stringId);
        }
    }

    public sealed class DramalordTraits
    {
        public int Openness
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.Openness);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.Openness, MBMath.ClampInt(value, HeroTraits.Openness.MinValue, HeroTraits.Openness.MaxValue));
        } 
        public int Conscientiousness
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.Conscientiousness);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.Conscientiousness, MBMath.ClampInt(value, HeroTraits.Conscientiousness.MinValue, HeroTraits.Conscientiousness.MaxValue));
        }
        public int Extroversion
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.Extroversion);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.Extroversion, MBMath.ClampInt(value, HeroTraits.Extroversion.MinValue, HeroTraits.Extroversion.MaxValue));
        }
        public int Agreeableness
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.Agreeableness);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.Agreeableness, MBMath.ClampInt(value, HeroTraits.Agreeableness.MinValue, HeroTraits.Agreeableness.MaxValue));
        }
        public int Neuroticism
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.Neuroticism);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.Neuroticism, MBMath.ClampInt(value, HeroTraits.Neuroticism.MinValue, HeroTraits.Neuroticism.MaxValue));
        }
        public int AttractionMen
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.AttractionMen);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionMen, MBMath.ClampInt(value, HeroTraits.AttractionMen.MinValue, HeroTraits.AttractionMen.MaxValue));
        }
        public int AttractionWomen
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.AttractionWomen);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionWomen, MBMath.ClampInt(value, HeroTraits.AttractionWomen.MinValue, HeroTraits.AttractionWomen.MaxValue));
        }
        public int AttractionWeight
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.AttractionWeight);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionWeight, MBMath.ClampInt(value, HeroTraits.AttractionWeight.MinValue, HeroTraits.AttractionWeight.MaxValue));
        }
        public int AttractionBuild
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.AttractionBuild);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionBuild, MBMath.ClampInt(value, HeroTraits.AttractionBuild.MinValue, HeroTraits.AttractionBuild.MaxValue));
        }
        public int AttractionAgeDiff
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.AttractionAgeDiff);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.AttractionAgeDiff, MBMath.ClampInt(value, HeroTraits.AttractionAgeDiff.MinValue, HeroTraits.AttractionAgeDiff.MaxValue));
        }
        public int Libido
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.Libido);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.Libido, MBMath.ClampInt(value, HeroTraits.Libido.MinValue, HeroTraits.Libido.MaxValue));
        }
        public int Horny
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.Horny);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.Horny, MBMath.ClampInt(value, HeroTraits.Horny.MinValue, HeroTraits.Horny.MaxValue));
        }
        public int PeriodDayOfSeason
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.PeriodDayOfSeason);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.PeriodDayOfSeason, MBMath.ClampInt(value, HeroTraits.PeriodDayOfSeason.MinValue, HeroTraits.PeriodDayOfSeason.MaxValue));
        }
        public int IntercourseSkill
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.IntercourseSkill);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.IntercourseSkill, MBMath.ClampInt(value, HeroTraits.IntercourseSkill.MinValue, HeroTraits.IntercourseSkill.MaxValue));
        }
        public int HasToy
        {
            get => _hero.GetHeroTraits().GetPropertyValue(HeroTraits.HasToy);
            set => _hero.GetHeroTraits().SetPropertyValue(HeroTraits.HasToy, MBMath.ClampInt(value, HeroTraits.HasToy.MinValue, HeroTraits.HasToy.MaxValue));
        }

        public bool IsSexuallyOpen => Openness >= 0 && Conscientiousness <= 0 && Extroversion >= 0 && Agreeableness >= 0 && Neuroticism <= 0;
        public bool IsEmotionallyOpen => Openness >= 2 && Conscientiousness <= 0 && Extroversion >= 0 && Agreeableness >= 1 && Neuroticism <= -1;
        public bool IsCheating => Openness >= 0 && Conscientiousness <= 0 && Extroversion >= 1 && Agreeableness <= 0 && Neuroticism <= 0;
        public bool IsBlackmailing => Openness >= 0 && Conscientiousness >= 1 && Extroversion >= 0 && Agreeableness <= 0 && Neuroticism >= 0;
        public bool IsHotTempered => Openness >= 0 && Conscientiousness <= 0 && Extroversion >= 0 && Agreeableness <= 0 && Neuroticism <= 0;
        public bool IsInstable => Openness >= 0 && Conscientiousness <= -1 && Extroversion >= 0 && Agreeableness <= -1 && Neuroticism <= -1;

        private readonly Hero _hero;

        public DramalordTraits(Hero hero)
        {
            _hero = hero;
        }
    }
}
