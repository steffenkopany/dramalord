using Dramalord.Extensions;
using Dramalord.Notifications;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.PerCampaign;
using MCM.Common;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Dramalord
{
    internal sealed class DramalordMCMEditor : AttributePerCampaignSettings<DramalordMCMEditor>
    {
        internal class HeroWrapper
        {
            internal Hero Hero { get; }
            public HeroWrapper(Hero hero)
            {
                Hero = hero;
            }
            public override int GetHashCode()
            {
                return Hero.GetHashCode();
            }

            public override string ToString()
            {
                return Hero.Name.ToString() + ((Hero.Clan != null) ? " of the " + Hero.Clan.Name.ToString() : "");
            }
        }

        private static Hero _selected = null;
        private static Hero _target = Hero.MainHero;
        private static Dropdown<HeroWrapper> _heroList = new(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList(), 0);
        private static Dropdown<HeroWrapper> _targetList = new(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList(), 0);
        private string _dummy;

        internal void SetSelected(Hero? hero)
        {
            if(hero != null && (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero)
            {
                _heroList.ForEach(item =>
                {
                    if(item.Hero == hero)
                    {
                        _heroList.SelectedIndex = _heroList.IndexOf(item);
                        _selected = hero;
                        return;
                    }
                }); 
            }
        }

        internal void SetTarget(Hero? hero)
        {
            if (hero != null && (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero)
            {
                _targetList.ForEach(item =>
                {
                    if (item.Hero == hero)
                    {
                        _targetList.SelectedIndex = _targetList.IndexOf(item);
                        _selected = hero;
                        return;
                    }
                });
            }
        }

        private void RefreshHeroLists()
        {
            _heroList = new(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList(), 0);
            _targetList = new(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList(), 0);
        }

        public DramalordMCMEditor()
        {
            _heroList = new(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList(), 0);
            _targetList = new(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList(), 0);
            if (_selected == null)
            {
                _selected = Hero.MainHero;
            }
            SetSelected(_selected);
            _target = Hero.MainHero;
            SetTarget(_target);
            _dummy = "";
            OnPropertyChanged();
        }
      

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_HERO)]
        [SettingPropertyDropdown(DramalordTexts.MCM_EDITOR_HERO_SELECT, Order = 1, RequireRestart = false)]
        public Dropdown<HeroWrapper> SelectedHero
        {
            get
            {
                if(_selected != _heroList.SelectedValue.Hero)
                {
                    _selected = _heroList.SelectedValue.Hero;
                    OnPropertyChanged();
                }

                return _heroList;
            }
        }

               
        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_PERSONALITY)]
        [SettingPropertyFloatingInteger(DramalordTexts.NAME_JEALOUSY, 0, 100, Order = 1, RequireRestart = false)]
        public int Openness
        {
            get => _selected.GetPersonality().Jealousy;
            set { _selected.GetPersonality().Jealousy = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_PERSONALITY)]
        [SettingPropertyFloatingInteger(DramalordTexts.NAME_EMPATHY, 0, 100, Order = 2, RequireRestart = false)]
        public int Conscientiousness
        {
            get => _selected.GetPersonality().Empathy;
            set { _selected.GetPersonality().Empathy = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_PERSONALITY)]
        [SettingPropertyFloatingInteger(DramalordTexts.NAME_SOCIABILITY, 0, 100, Order = 3, RequireRestart = false)]
        public int Extroversion
        {
            get => _selected.GetPersonality().Sociability;
            set { _selected.GetPersonality().Sociability = value; OnPropertyChanged(); }
        }


        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_DESIRE)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_EDITOR_DESIRE_MEN, 0, 100, Order = 1, RequireRestart = false)]
        public int AttractionMen
        {
            get => _selected.GetDesires().AttractionMen;
            set { _selected.GetDesires().AttractionMen = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_DESIRE)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_EDITOR_DESIRE_WOMEN, 0, 100, Order = 2, RequireRestart = false)]
        public int AttractionWomen
        {
            get => _selected.GetDesires().AttractionWomen;
            set { _selected.GetDesires().AttractionWomen = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_DESIRE)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_EDITOR_DESIRE_WEIGHT, 0, 100, Order = 3, RequireRestart = false)]
        public int AttractionWeight
        {
            get => _selected.GetDesires().AttractionWeight;
            set { _selected.GetDesires().AttractionWeight = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_DESIRE)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_EDITOR_DESIRE_BUILD, 0, 100, Order = 4, RequireRestart = false)]
        public int AttractionBuild
        {
            get => _selected.GetDesires().AttractionBuild;
            set { _selected.GetDesires().AttractionBuild = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_DESIRE)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_EDITOR_DESIRE_AGE, -20, 20, Order = 5, RequireRestart = false)]
        public int AttractionAgeDiff
        {
            get => _selected.GetDesires().AttractionAgeDiff;
            set { _selected.GetDesires().AttractionAgeDiff = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_DESIRE)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_EDITOR_DESIRE_LIBIDO, 0, 10, HintText = DramalordTexts.MCM_EDITOR_DESIRE_LIBIDO_INFO, Order = 6, RequireRestart = false)]
        public int Libido
        {
            get => _selected.GetDesires().Libido;
            set { _selected.GetDesires().Libido = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_DESIRE)]
        [SettingPropertyFloatingInteger(DramalordTexts.NAME_AROUSAL, 0, 100, HintText = DramalordTexts.MCM_EDITOR_DESIRE_AROUSAL_INFO, Order = 7, RequireRestart = false)]
        public int Horny
        {
            get => _selected.GetDesires().Horny;
            set { _selected.GetDesires().Horny = value; OnPropertyChanged(); }
        }


        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_RELATION)]
        [SettingPropertyDropdown(DramalordTexts.MCM_EDITOR_RELATION_TARGET, Order = 1, RequireRestart = false)]
        public Dropdown<HeroWrapper> SelectedTarget
        {
            get
            {
                if (_target != _targetList.SelectedValue.Hero)
                {
                    _target = _targetList.SelectedValue.Hero;
                    OnPropertyChanged();
                }

                return _targetList;
            }
        }


        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_RELATION)]
        [SettingProperty(DramalordTexts.MCM_EDITOR_RELATION_ATTRACTION, Order = 2, RequireRestart = false)]
        public string CurrentAttraction
        {
            get => (_selected == _target) ? 0.ToString() : _selected.GetAttractionTo(_target).ToString();
            set => _dummy = value;
        }


        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_RELATION)]
        [SettingProperty(DramalordTexts.MCM_EDITOR_RELATION_SYMPATHY, Order = 3, RequireRestart = false)]
        public string CurrentTraitScore
        {
            get => (_selected == _target) ? 0.ToString() : _selected.GetSympathyTo(_target).ToString();
            set => _dummy = value;
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_RELATION)]
        [SettingPropertyFloatingInteger(DramalordTexts.NAME_TRUST, -100, 100, Order = 4, RequireRestart = false)]
        public int Trust
        {
            get => (_selected == _target) ? 0 : _selected.GetTrust(_target);
            set { if (_selected != _target) _selected.SetTrust(_target, value); OnPropertyChanged(); }
        }

        [SettingPropertyGroup(DramalordTexts.MCM_EDITOR_RELATION)]
        [SettingPropertyFloatingInteger(DramalordTexts.NAME_LOVE, -100, 100, Order = 5, RequireRestart = false)]
        public int Love
        {
            get => (_selected == _target) ? 0 : _selected.GetRelationTo(_target).Love;
            set { if (_selected != _target) _selected.GetRelationTo(_target).Love = value; OnPropertyChanged(); }
        }


        public override string Id => "DramalordEditor";

        public override string DisplayName => "Dramalord Editor";

        public override string FolderName => DramalordSubModule.ModuleName;
    }
}
