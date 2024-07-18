using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Extensions;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.PerCampaign;
using MCM.Common;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Dramalord
{
    internal sealed class DramalordMCMEditor : AttributePerCampaignSettings<DramalordMCMEditor>
    {
        private static Hero _selected = Hero.MainHero;
        private static Dropdown<HeroWrapper> _heroList = new(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList(), 0);
        private string _dummy;

        public class HeroWrapper
        {
            internal Hero Hero { get; }
            public HeroWrapper(Hero hero)
            {
                Hero = hero;
            }
            public override string ToString()
            {
                return Hero.Name.ToString() + ((Hero.Clan != null) ? " of the " + Hero.Clan.Name.ToString() : "");
            }
        }

        public DramalordMCMEditor()
        {
            _selected = Hero.MainHero;
        }

        [SettingPropertyGroup("{=Dramalord107}1: Hero Selection")]
        [SettingPropertyDropdown("{=Dramalord108}Select Hero", Order = 1, RequireRestart = false, HintText = "{=Dramalord193}Select hero from list and press 'Load Hero' (only works with running campaign)")]
        public Dropdown<HeroWrapper> SelectedHero
        {
            get
            {
                _heroList.Clear();
                _heroList.AddRange(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList());
                return _heroList;
                //Instance.OnPropertyChanged("SelectedHero");
            }
            set
            {
                _heroList = value;
                _selected = value.SelectedValue.Hero;
                Instance.OnPropertyChanged();
            }
        }


        [SettingPropertyGroup("{=Dramalord107}1: Hero Selection")]
        [SettingPropertyButton("{=Dramalord194}Load Hero", Order = 4, Content = "{=Dramalord194}Load Hero", HintText = "Press this button to load Dramalord data of selected hero (only works with running campaign)", RequireRestart = false)]
        public Action LoadHeroData { get; set; } = () =>
        {
            _selected = Instance.SelectedHero.SelectedValue.Hero;
            //_heroList.Clear();
            //_heroList.AddRange(Hero.AllAliveHeroes.Where(hero => (hero.IsDramalordLegit() && hero.HasMet) || hero == Hero.MainHero).Select(hero => new HeroWrapper(hero)).ToList());
            //_heroList.SelectedIndex = _heroList.FindIndex(wrapper => wrapper.Hero == _selected);
            Instance.OnPropertyChanged();
            InformationManager.DisplayMessage(new InformationMessage("Selected hero: " + Instance.SelectedHero.SelectedValue.ToString() + " at index " + Instance.SelectedHero.SelectedIndex, Color.White));
        };
          
        [SettingPropertyGroup("{=Dramalord107}1: Hero Selection")]
        [SettingProperty("{=Dramalord195}Current Attraction", Order = 2, RequireRestart = false, HintText = "{=Dramalord196}Current attraction value to player")]
        public string CurrentAttraction
        {
            get => _selected.GetAttractionTo(Hero.MainHero).ToString();
            set => _dummy = value;
        }


        [SettingPropertyGroup("{=Dramalord107}1: Hero Selection")]
        [SettingProperty("{=Dramalord197}Current Sympathy", Order = 3, RequireRestart = false, HintText = "{=Dramalord198}Current sympathy value to player")]
        public string CurrentTraitScore
        {
            get => _selected.GetSympathyTo(Hero.MainHero).ToString();
            set => _dummy = value;
        }
               
        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord110}Openness", -100, 100, HintText = "{=Dramalord111}Represents how willing a person is to try new things", Order = 1, RequireRestart = false)]
        public int Openness
        {
            get => _selected.GetPersonality().Openness;
            set => _selected.GetPersonality().Openness = value;
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord112}Conscientiousness", -100, 100, HintText = "{=Dramalord113}Refers to an individual's desire to be careful and diligent", Order = 2, RequireRestart = false)]
        public int Conscientiousness
        {
            get => _selected.GetPersonality().Conscientiousness;
            set => _selected.GetPersonality().Conscientiousness = value;
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord114}Extroversion", -100, 100, HintText = "{=Dramalord115}Measures how energetic, outgoing and confident a person is", Order = 3, RequireRestart = false)]
        public int Extroversion
        {
            get => _selected.GetPersonality().Extroversion;
            set => _selected.GetPersonality().Extroversion = value;
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord116}Agreeableness", -100, 100, HintText = "{=Dramalord117}Refers to how an individual interacts with others", Order = 4, RequireRestart = false)]
        public int Agreeableness
        {
            get => _selected.GetPersonality().Agreeableness;
            set => _selected.GetPersonality().Agreeableness = value;
        }

        [SettingPropertyGroup("{=Dramalord109}2: Personality")]
        [SettingPropertyFloatingInteger("{=Dramalord118}Neuroticism", -100, 100, HintText = "{=Dramalord119}Represents how much someone is inclined to experience negative emotions", Order = 5, RequireRestart = false)]
        public int Neuroticism
        {
            get => _selected.GetPersonality().Neuroticism;
            set => _selected.GetPersonality().Neuroticism = value;
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord121}Attraction To Men", 0, 100, HintText = "{=Dramalord122}Defines whether an individual finds male persons attractive or not", Order = 1, RequireRestart = false)]
        public int AttractionMen
        {
            get => _selected.GetDesires().AttractionMen;
            set => _selected.GetDesires().AttractionMen = value;
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord123}Attraction To Women", 0, 100, HintText = "{=Dramalord124}Defines whether an individual finds female persons attractive or not", Order = 2, RequireRestart = false)]
        public int AttractionWomen
        {
            get => _selected.GetDesires().AttractionWomen;
            set => _selected.GetDesires().AttractionWomen = value;
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord125}Attraction To Weight", 0, 100, HintText = "{=Dramalord126}Defines whether an individual has interest in chubby or thin heroes", Order = 3, RequireRestart = false)]
        public int AttractionWeight
        {
            get => _selected.GetDesires().AttractionWeight;
            set => _selected.GetDesires().AttractionWeight = value;
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord127}Attraction To Build", 0, 100, HintText = "{=Dramalord128}Defines whether an individual has interest in muscular or weak heroes", Order = 4, RequireRestart = false)]
        public int AttractionBuild
        {
            get => _selected.GetDesires().AttractionBuild;
            set => _selected.GetDesires().AttractionBuild = value;
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord129}Attraction To Age Difference", -20, 20, HintText = "{=Dramalord130}Defines whether an individual has interest in older or younger heroes in year difference", Order = 5, RequireRestart = false)]
        public int AttractionAgeDiff
        {
            get => _selected.GetDesires().AttractionAgeDiff;
            set => _selected.GetDesires().AttractionAgeDiff = value;
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord131}Libido", 0, 10, HintText = "{=Dramalord132}Defines whether an individual generally develops interest in intercourse or not", Order = 6, RequireRestart = false)]
        public int Libido
        {
            get => _selected.GetDesires().Libido;
            set => _selected.GetDesires().Libido = value;
        }

        [SettingPropertyGroup("{=Dramalord120}3: Desire")]
        [SettingPropertyFloatingInteger("{=Dramalord133}Horny", 0, 100, HintText = "{=Dramalord134}Represents how willing a hero currently is for intercourse due to hormons", Order = 7, RequireRestart = false)]
        public int Horny
        {
            get => _selected.GetDesires().Horny;
            set => _selected.GetDesires().Horny = value;
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyFloatingInteger("{=Dramalord136}Trust", -100, 100, HintText = "{=Dramalord137}Trust value to the player", Order = 1, RequireRestart = false)]
        public int Trust
        {
            get => (Hero.MainHero == _selected) ? 0 : _selected.GetRelationTo(Hero.MainHero).Trust;
            set { if (Hero.MainHero != _selected) _selected.GetRelationTo(Hero.MainHero).Trust = value; }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyFloatingInteger("{=Dramalord138}Love", -100, 100, HintText = "{=Dramalord139}Love value to the player", Order = 2, RequireRestart = false)]
        public int Love
        {
            get => (Hero.MainHero == _selected) ? 0 : _selected.GetRelationTo(Hero.MainHero).Love;
            set { if (Hero.MainHero != _selected) _selected.GetRelationTo(Hero.MainHero).Love = value; }
        }
        /*
        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyFloatingInteger("{=Dramalord140}Tension", -100, 100, HintText = "{=Dramalord141}Tension value to the player", Order = 3, RequireRestart = false)]
        public int Tension
        {
            get => (Hero.MainHero == _selected) ? 0 : _selected.GetRelationTo(Hero.MainHero).Tension;
            set { if (Hero.MainHero != _selected) _selected.GetRelationTo(Hero.MainHero).Tension = value; }
        }
        */
        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyBool("{=Dramalord142}No Relationship", Order = 4, HintText = "{=Dramalord143}No relationship with the player", RequireRestart = false)]
        public bool RelationshipNone
        {
            get => _selected.GetRelationTo(Hero.MainHero).Relationship == RelationshipType.None;
            set { if(_selected != Hero.MainHero) BreakupAction.Apply(Hero.MainHero, _selected); Instance.OnPropertyChanged(); }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyBool("{=Dramalord144}Friendship", Order = 5, HintText = "{=Dramalord145}Has friendship with the player", RequireRestart = false)]
        public bool RelationshipFriend
        {
            get => _selected.GetRelationTo(Hero.MainHero).Relationship == RelationshipType.Friend;
            set { if (_selected != Hero.MainHero) { if (_selected.Spouse == Hero.MainHero) { BreakupAction.Apply(Hero.MainHero, _selected); } FriendshipAction.Apply(Hero.MainHero, _selected); Instance.OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyBool("{=Dramalord146}Friend with benefits", Order = 6, HintText = "{=Dramalord147}Has friendship with benefits with the player", RequireRestart = false)]
        public bool RelationshipFriendWithBenefits
        {
            get => _selected.GetRelationTo(Hero.MainHero).Relationship == RelationshipType.FriendWithBenefits;
            set { if (_selected != Hero.MainHero) { if (_selected.Spouse == Hero.MainHero) { BreakupAction.Apply(Hero.MainHero, _selected); } FriendsWithBenefitsAction.Apply(Hero.MainHero, _selected); Instance.OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyBool("{=Dramalord148}Lover", Order = 7, HintText = "{=Dramalord149}Is a couple with the player", RequireRestart = false)]
        public bool RelationshipLove
        {
            get => _selected.GetRelationTo(Hero.MainHero).Relationship == RelationshipType.Lover;
            set { if (_selected != Hero.MainHero) { if (_selected.Spouse == Hero.MainHero) { BreakupAction.Apply(Hero.MainHero, _selected); } LoverAction.Apply(Hero.MainHero, _selected); Instance.OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyBool("{=Dramalord150}Betrothed", Order = 8, HintText = "{=Dramalord151}Is engaged with the player", RequireRestart = false)]
        public bool RelationshipEngaged
        {
            get => _selected.GetRelationTo(Hero.MainHero).Relationship == RelationshipType.Betrothed;
            set { if (_selected != Hero.MainHero) { if (_selected.Spouse == Hero.MainHero) { BreakupAction.Apply(Hero.MainHero, _selected); } EngagementAction.Apply(Hero.MainHero, _selected, _selected.GetCloseHeroes()); Instance.OnPropertyChanged(); } }
        }

        [SettingPropertyGroup("{=Dramalord135}4: Relation to Player")]
        [SettingPropertyBool("{=Dramalord209}Married", Order = 8, HintText = "{=Dramalord210}Is married to the player", RequireRestart = false)]
        public bool RelationshipMarried
        {
            get => _selected.GetRelationTo(Hero.MainHero).Relationship == RelationshipType.Spouse || _selected.Spouse == Hero.MainHero;
            set { if (_selected != Hero.MainHero) { if (_selected.Spouse == Hero.MainHero) { BreakupAction.Apply(Hero.MainHero, _selected); } MarriageAction.Apply(Hero.MainHero, _selected, _selected.GetCloseHeroes()); Instance.OnPropertyChanged(); } }
        }

        public override string Id => "DramalordEditor";

        public override string DisplayName => "Dramalord Editor";

        public override string FolderName => DramalordSubModule.ModuleName;
    }
}
