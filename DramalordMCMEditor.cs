﻿using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.PerSave;
using MCM.Common;
using System.Data;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Dramalord
{
    internal sealed class DramalordMCMEditor : AttributePerSaveSettings<DramalordMCMEditor>
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

        [SettingPropertyGroup("{=Dramalord107}1: Hero Selection")]
        [SettingPropertyDropdown("{=Dramalord108}Select Hero", Order = 1, RequireRestart = false, HintText = "{=Dramalord193}Select hero from list (only works with running campaign)")]
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

               
        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord110}Openness", -50, 50, HintText = "{=Dramalord111}Represents how willing a person is to try new things", Order = 1, RequireRestart = false)]
        public int Openness
        {
            get => _selected.GetPersonality().Openness;
            set { _selected.GetPersonality().Openness = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord112}Conscientiousness", -50, 50, HintText = "{=Dramalord113}Refers to an individual's desire to be careful and diligent", Order = 2, RequireRestart = false)]
        public int Conscientiousness
        {
            get => _selected.GetPersonality().Conscientiousness;
            set { _selected.GetPersonality().Conscientiousness = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord114}Extroversion", -50, 50, HintText = "{=Dramalord115}Measures how energetic, outgoing and confident a person is", Order = 3, RequireRestart = false)]
        public int Extroversion
        {
            get => _selected.GetPersonality().Extroversion;
            set { _selected.GetPersonality().Extroversion = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord116}Agreeableness", -50, 50, HintText = "{=Dramalord117}Refers to how an individual interacts with others", Order = 4, RequireRestart = false)]
        public int Agreeableness
        {
            get => _selected.GetPersonality().Agreeableness;
            set { _selected.GetPersonality().Agreeableness = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord118}Neuroticism", -50, 50, HintText = "{=Dramalord119}Represents how much someone is inclined to experience negative emotions", Order = 5, RequireRestart = false)]
        public int Neuroticism
        {
            get => _selected.GetPersonality().Neuroticism;
            set { _selected.GetPersonality().Neuroticism = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord121}Attraction To Men", 0, 100, HintText = "{=Dramalord122}Defines whether an individual finds male persons attractive or not", Order = 1, RequireRestart = false)]
        public int AttractionMen
        {
            get => _selected.GetDesires().AttractionMen;
            set { _selected.GetDesires().AttractionMen = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord123}Attraction To Women", 0, 100, HintText = "{=Dramalord124}Defines whether an individual finds female persons attractive or not", Order = 2, RequireRestart = false)]
        public int AttractionWomen
        {
            get => _selected.GetDesires().AttractionWomen;
            set { _selected.GetDesires().AttractionWomen = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord125}Attraction To Weight", 0, 100, HintText = "{=Dramalord126}Defines whether an individual has interest in chubby or thin heroes", Order = 3, RequireRestart = false)]
        public int AttractionWeight
        {
            get => _selected.GetDesires().AttractionWeight;
            set { _selected.GetDesires().AttractionWeight = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord127}Attraction To Build", 0, 100, HintText = "{=Dramalord128}Defines whether an individual has interest in muscular or weak heroes", Order = 4, RequireRestart = false)]
        public int AttractionBuild
        {
            get => _selected.GetDesires().AttractionBuild;
            set { _selected.GetDesires().AttractionBuild = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord129}Attraction To Age Difference", -20, 20, HintText = "{=Dramalord130}Defines whether an individual has interest in older or younger heroes in year difference", Order = 5, RequireRestart = false)]
        public int AttractionAgeDiff
        {
            get => _selected.GetDesires().AttractionAgeDiff;
            set { _selected.GetDesires().AttractionAgeDiff = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord131}Libido", 0, 10, HintText = "{=Dramalord132}Defines whether an individual generally develops interest in intercourse or not", Order = 6, RequireRestart = false)]
        public int Libido
        {
            get => _selected.GetDesires().Libido;
            set { _selected.GetDesires().Libido = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord133}Horny", 0, 100, HintText = "{=Dramalord134}Represents how willing a hero currently is for intercourse due to hormones", Order = 7, RequireRestart = false)]
        public int Horny
        {
            get => _selected.GetDesires().Horny;
            set { _selected.GetDesires().Horny = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingProperty("{=Dramalord612}Marriage type", Order = 8, RequireRestart = false, HintText = "{=Dramalord621}The type of marriage this hero would prefer.")]
        public string CurrentMarriageType
        {
            get => new TextObject(
                        (_selected.GetDefaultRelationshipRule() == RelationshipRule.Open) ? "{=Dramalord616}Open" :
                        (_selected.GetDefaultRelationshipRule() == RelationshipRule.Poly) ? "{=Dramalord615}Poly" :
                        (_selected.GetDefaultRelationshipRule() == RelationshipRule.Playful) ? "{=Dramalord614}Playful" : "{=Dramalord613}Faithful"
                        ).ToString();
            set => _dummy = value;
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyDropdown("{=Dramalord570}Select Target", Order = 1, RequireRestart = false, HintText = "{=Dramalord193}Select hero from list (only works with running campaign)")]
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


        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingProperty("{=Dramalord195}Current Attraction", Order = 2, RequireRestart = false, HintText = "{=Dramalord196}Current attraction value to player")]
        public string CurrentAttraction
        {
            get => (_selected == _target) ? 0.ToString() : _selected.GetAttractionTo(_target).ToString();
            set => _dummy = value;
        }


        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingProperty("{=Dramalord197}Current Sympathy", Order = 3, RequireRestart = false, HintText = "{=Dramalord198}Current sympathy value to player")]
        public string CurrentTraitScore
        {
            get => (_selected == _target) ? 0.ToString() : _selected.GetSympathyTo(_target).ToString();
            set => _dummy = value;
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyFloatingInteger("{=Dramalord136}Trust", -100, 100, HintText = "{=Dramalord137}Trust value to the player", Order = 4, RequireRestart = false)]
        public int Trust
        {
            get => (_selected == _target) ? 0 : _selected.GetTrust(_target);
            set { if (_selected != _target) _selected.SetTrust(_target, value); OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyFloatingInteger("{=Dramalord138}Love", -100, 100, HintText = "{=Dramalord139}Love value to the player", Order = 5, RequireRestart = false)]
        public int Love
        {
            get => (_selected == _target) ? 0 : _selected.GetRelationTo(_target).Love;
            set { if (_selected != _target) _selected.GetRelationTo(_target).Love = value; OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyBool("{=Dramalord142}No Relationship", Order = 6, HintText = "{=Dramalord143}No relationship with the player", RequireRestart = false)]
        public bool RelationshipNone
        {
            get => _selected.GetRelationTo(_target).Relationship == RelationshipType.None;
            set { if(_selected != Hero.MainHero) EndRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected)); OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyBool("{=Dramalord144}Friendship", Order = 7, HintText = "{=Dramalord145}Has friendship with the player", RequireRestart = false)]
        public bool RelationshipFriend
        {
            get => _selected.GetRelationTo(_target).Relationship == RelationshipType.Friend;
            set { if (_selected != _target) { if (_selected.Spouse == _target) { EndRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected)); } StartRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected), RelationshipType.Friend); OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyBool("{=Dramalord146}Friend with benefits", Order = 8, HintText = "{=Dramalord147}Has friendship with benefits with the player", RequireRestart = false)]
        public bool RelationshipFriendWithBenefits
        {
            get => _selected.GetRelationTo(_target).Relationship == RelationshipType.FriendWithBenefits;
            set { if (_selected != _target) { if (_selected.Spouse == _target) { EndRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected)); } StartRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected), RelationshipType.FriendWithBenefits); OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyBool("{=Dramalord148}Lover", Order = 9, HintText = "{=Dramalord149}Is a couple with the player", RequireRestart = false)]
        public bool RelationshipLove
        {
            get => _selected.GetRelationTo(_target).Relationship == RelationshipType.Lover;
            set { if (_selected != _target) { if (_selected.Spouse == _target) { EndRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected)); } StartRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected), RelationshipType.Lover); OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyBool("{=Dramalord150}Betrothed", Order = 10, HintText = "{=Dramalord151}Is engaged with the player", RequireRestart = false)]
        public bool RelationshipEngaged
        {
            get => _selected.GetRelationTo(_target).Relationship == RelationshipType.Betrothed;
            set { if (_selected != _target && !BetrothIntention.OtherMarriageModFound) { if (_selected.Spouse == _target) { EndRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected)); } StartRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected), RelationshipType.Betrothed); OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Target")]
        [SettingPropertyBool("{=Dramalord209}Married", Order = 11, HintText = "{=Dramalord210}Is married to the player", RequireRestart = false)]
        public bool RelationshipMarried
        {
            get => _selected.GetRelationTo(_target).Relationship == RelationshipType.Spouse || _selected.Spouse == _target;
            set { if (_selected != _target && !BetrothIntention.OtherMarriageModFound) { if (_selected.Spouse == _target) { EndRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected)); } StartRelationshipAction.Apply(_target, _selected, _target.GetRelationTo(_selected), RelationshipType.Spouse); OnPropertyChanged(); } }
        }

        public override string Id => "DramalordEditor";

        public override string DisplayName => "Dramalord Editor";

        public override string FolderName => DramalordSubModule.ModuleName;
    }
}
