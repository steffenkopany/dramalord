using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;

namespace Dramalord.Data
{
    internal class HeroPersonality
    {
        private DramalordTraits _dltraits;
        private CharacterTraits _blTraits;
        private Hero _hero;

        internal HeroPersonality(Hero hero)
        {
            _dltraits = hero.GetDramalordTraits();
            _blTraits = hero.GetHeroTraits();
            _hero = hero;
        }

        internal bool IsCheating
        {
            get 
            {
                return _dltraits.Openness > 0 && _dltraits.Conscientiousness < 0 && _dltraits.Extroversion > 0 && _dltraits.Agreeableness < 0 && _dltraits.Neuroticism < 0 && _blTraits.Honor <= 0 && _blTraits.Valor >= 0;
            }
        }

        internal bool IsHotTempered
        {
            get
            {
                return _dltraits.Conscientiousness < -50 && _dltraits.Extroversion > 0 && _dltraits.Agreeableness < -50 && _dltraits.Neuroticism > 50 && _blTraits.Calculating < 0;
            }
        }

        internal bool IsInstable
        {
            get
            {
                return _dltraits.Conscientiousness < -80 && _dltraits.Extroversion > 50 && _dltraits.Agreeableness < -50 && _dltraits.Neuroticism > 80 && _blTraits.Calculating < 0;
            }
        }

        internal bool AcceptsGossip
        {
            get
            {
                return _dltraits.Openness > 0 && _dltraits.Agreeableness > 0 && _dltraits.Neuroticism < 0;
            }
        }

        internal bool AcceptsOtherMarriages
        {
            get
            {
                return _dltraits.Openness > 90 && _dltraits.Extroversion > 80 && _dltraits.Agreeableness > 90 && _dltraits.Neuroticism < -90 && _blTraits.Mercy > 0 && _blTraits.Generosity > 0;
            }
        }

        internal bool AcceptsOtherRelationships
        {
            get
            {
                return _dltraits.Openness > 80 &&  _dltraits.Agreeableness > 75 && _dltraits.Neuroticism < -80 && _blTraits.Mercy > 0 && _blTraits.Generosity > 0;
            }
        }

        internal bool AcceptsOtherIntercourse
        {
            get
            {
                return _dltraits.Openness > 80 && _dltraits.Agreeableness > 75 && _dltraits.Neuroticism < -80 && _blTraits.Mercy > 0 && _blTraits.Generosity > 0 && _dltraits.Libido > 5;
            }
        }

        internal bool AcceptsOtherFlirts
        {
            get
            {
                return _dltraits.Openness > 30 && _dltraits.Conscientiousness < 0 && _dltraits.Agreeableness > 30 && _dltraits.Neuroticism < 0;
            }
        }

        internal bool AcceptsOtherPregnancies
        {
            get
            {
                return _dltraits.Openness > 80 && _dltraits.Conscientiousness < -50 && _dltraits.Agreeableness > 80 && _dltraits.Neuroticism < -80 && _blTraits.Generosity > 0;
            }
        }

        internal bool AcceptsOtherChildren
        {
            get
            {
                return _dltraits.Openness > 80 && _dltraits.Conscientiousness < 0 && _dltraits.Agreeableness > 90 && _dltraits.Neuroticism < -90 && _blTraits.Generosity > 0 && _blTraits.Mercy > 0;
            }
        }

        internal bool IsHeteroSexual
        {
            get
            {
                return (_hero.IsFemale) ? (_dltraits.AttractionWomen < DramalordMCM.Get.MinAttractionForFlirting && _dltraits.AttractionMen >= DramalordMCM.Get.MinAttractionForFlirting) : (_dltraits.AttractionWomen >= DramalordMCM.Get.MinAttractionForFlirting && _dltraits.AttractionMen < DramalordMCM.Get.MinAttractionForFlirting);
            }
        }

        internal bool IsHomoSexual
        {
            get
            {
                return (_hero.IsFemale) ? (_dltraits.AttractionWomen >= DramalordMCM.Get.MinAttractionForFlirting && _dltraits.AttractionMen < DramalordMCM.Get.MinAttractionForFlirting) : (_dltraits.AttractionWomen < DramalordMCM.Get.MinAttractionForFlirting && _dltraits.AttractionMen >= DramalordMCM.Get.MinAttractionForFlirting);
            }
        }

        internal bool IsBiSexual
        {
            get
            {
                return (_dltraits.AttractionWomen >= DramalordMCM.Get.MinAttractionForFlirting && _dltraits.AttractionMen >= DramalordMCM.Get.MinAttractionForFlirting);
            }
        }

        internal bool IsASexual
        {
            get
            {
                return (_dltraits.AttractionWomen < DramalordMCM.Get.MinAttractionForFlirting && _dltraits.AttractionMen < DramalordMCM.Get.MinAttractionForFlirting);
            }
        }

        internal int GetEmotionalChange(EventType eventType)
        {
            if (eventType == EventType.Marriage)
            {
                int result = (_dltraits.Openness - 90) + (_dltraits.Extroversion - 80) + (_dltraits.Agreeableness - 90) + (_dltraits.Neuroticism * -1 - 80) + (_blTraits.Generosity * 10) + (_blTraits.Mercy * 10);
                return MBMath.ClampInt(result, -100, 0);
            }

            if (eventType == EventType.Date)
            {
                int result = (_dltraits.Openness - 80) + (_dltraits.Agreeableness - 75) + (_dltraits.Neuroticism * -1 - 80) + (_blTraits.Generosity *10) + (_blTraits.Mercy * 10);
                return MBMath.ClampInt(result, -50, 0);
            }

            if (eventType == EventType.Flirt)
            {
                int result = (_dltraits.Openness - 30) + (_dltraits.Conscientiousness * -1) + (_dltraits.Agreeableness - 30) + (_dltraits.Neuroticism * -1);
                return MBMath.ClampInt(result, -10, 0);
            }

            if (eventType == EventType.Intercourse)
            {
                int result = (_dltraits.Openness - 80) + (_dltraits.Agreeableness - 75) + (_dltraits.Neuroticism * -1 - 80) + (_blTraits.Generosity * 10) + (_blTraits.Mercy * 10) + (_dltraits.Libido - 5);
                return MBMath.ClampInt(result, -50, 0);
            }

            if (eventType == EventType.Pregnancy)
            {
                int result = (_dltraits.Openness - 80) + (_dltraits.Conscientiousness * -1 - 50) + (_dltraits.Agreeableness - 80) + (_dltraits.Neuroticism * -1 - 80) + (_blTraits.Generosity * 10);
                return MBMath.ClampInt(result, -100, 0);
            }

            if (eventType == EventType.Birth)
            {
                int result = (_dltraits.Openness - 80) + (_dltraits.Conscientiousness * -1) + (_dltraits.Agreeableness - 90) + (_dltraits.Neuroticism * -1 - 90) + (_blTraits.Generosity * 10) + (_blTraits.Mercy * 10);
                return MBMath.ClampInt(result, -100, 0);
            }

            return 0;
        }
    }
}
