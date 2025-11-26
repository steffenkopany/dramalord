using Dramalord.Notifications;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace Dramalord
{
    internal sealed class DramalordMCM : AttributeGlobalSettings<DramalordMCM>
    {
        [SettingPropertyGroup(DramalordTexts.MCM_AUTONOMY)]
        [SettingPropertyBool(DramalordTexts.MCM_AUTONOMY_WANDERERS, HintText = DramalordTexts.MCM_AUTONOMY_WANDERERS_INFO, Order = 1, RequireRestart = false)]
        public bool AllowWandererAutonomy { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_AUTONOMY)]
        [SettingPropertyBool(DramalordTexts.MCM_AUTONOMY_PLAYERCLAN, HintText = DramalordTexts.MCM_AUTONOMY_PLAYERCLAN_INFO, Order = 2, RequireRestart = false)]
        public bool AllowPlayerClanAutonomy { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_AUTONOMY)]
        [SettingPropertyBool(DramalordTexts.MCM_AUTONOMY_NOTABLES, Order = 3, HintText = DramalordTexts.MCM_AUTONOMY_NOTABLES_INFO, RequireRestart = false)]
        public bool AllowNotablesAutonomy { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_AUTONOMY)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_AUTONOMY_APPROACH_PLAYER, 0, 100, Order = 4, HintText = DramalordTexts.MCM_AUTONOMY_APPROACH_PLAYER_INFO, RequireRestart = false)]
        public int ChanceApproachingPlayer { get; set; } = 30;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_MIN_ATTRACTION, 0, 100, Order = 1, HintText = DramalordTexts.MCM_HERO_MIN_ATTRACTION_INFO, RequireRestart = false)]
        public int MinAttraction { get; set; } = 66;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_MIN_LOVE_DATE, 0, 100, Order = 2, HintText = DramalordTexts.MCM_HERO_MIN_LOVE_DATE_INFO, RequireRestart = false)]
        public int MinDatingLove { get; set; } = 50;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_MIN_LOVE_MARRIAGE, 0, 100, Order = 3, HintText = DramalordTexts.MCM_HERO_MIN_LOVE_MARRIAGE_INFO, RequireRestart = false)]
        public int MinMarriageLove { get; set; } = 75;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_MIN_TRUST_FRIEND, 10, 100, Order = 4, HintText = DramalordTexts.MCM_HERO_MIN_TRUST_FRIEND_INFO, RequireRestart = false)]
        public int MinTrustFriends { get; set; } = 40;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_MAX_TRUST_ENEMY, -100, -10, Order = 5, HintText = DramalordTexts.MCM_HERO_MAX_TRUST_ENEMY_INFO, RequireRestart = false)]
        public int MaxTrustEnemies { get; set; } = -30;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_PREGNANCY_DURATION, 1, 100, Order = 7, HintText = DramalordTexts.MCM_HERO_PREGNANCY_DURATION_INFO, RequireRestart = false)]
        public int PregnancyDuration { get; set; } = 21;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_PREGNANCY_CHANCE, 0, 100, Order = 8, HintText = DramalordTexts.MCM_HERO_PREGNANCY_CHANCE_INFO, RequireRestart = false)]
        public int PregnancyChance { get; set; } = 10;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_MAX_FERTILITY_AGE, 18, 100, Order = 9, HintText = DramalordTexts.MCM_HERO_MAX_FERTILITY_AGE_INFO, RequireRestart = false)]
        public int MaxFertilityAge { get; set; } = 45;

        [SettingPropertyGroup(DramalordTexts.MCM_HERO)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_HERO_LOVE_DECAY_START, 0, 100, Order = 10, HintText = DramalordTexts.MCM_HERO_LOVE_DECAY_START_INFO, RequireRestart = false)]
        public int LoveDecayStartDay { get; set; } = 10;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_RELATIONSHIP, Order = 1, HintText = DramalordTexts.MCM_LOGGING_RELATIONSHIP_INFO, RequireRestart = false)]
        public bool RelationshipLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_TALKING, Order = 2, HintText = DramalordTexts.MCM_LOGGING_TALKING_INFO, RequireRestart = false)]
        public bool TalkLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_GOSSIP, Order = 3, HintText = DramalordTexts.MCM_LOGGING_GOSSIP_INFO, RequireRestart = false)]
        public bool GossipLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_FLIRTING, Order = 4, HintText = DramalordTexts.MCM_LOGGING_FLIRTING_INFO, RequireRestart = false)]
        public bool FlirtingLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_DATING, Order = 5, HintText = DramalordTexts.MCM_LOGGING_DATING_INFO, RequireRestart = false)]
        public bool DatingLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_INTERCOURSE, Order = 6, HintText = DramalordTexts.MCM_LOGGING_INTERCOURSE_INFO, RequireRestart = false)]
        public bool SexLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_IMPREGNATION, Order = 7, HintText = DramalordTexts.MCM_LOGGING_IMPREGNATION_INFO, RequireRestart = false)]
        public bool ConceiveLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_LOGGING)]
        [SettingPropertyBool(DramalordTexts.MCM_LOGGING_CHILDREN, Order = 8, HintText = DramalordTexts.MCM_LOGGING_CHILDREN, RequireRestart = false)]
        public bool ChildrenEventLogs { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_GENERAL_CHANCE_CAUGHT, 0, 100, Order = 1, HintText = DramalordTexts.MCM_GENERAL_CHANCE_CAUGHT_INFO, RequireRestart = false)]
        public int ChanceGettingCaught { get; set; } = 30;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_GENERAL_PLAYER_ATTRACTION, 0, 100, HintText = DramalordTexts.MCM_GENERAL_PLAYER_ATTRACTION_INFO, Order = 2, RequireRestart = false)]
        public int PlayerBaseAttraction { get; set; } = 10;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_GENERAL_PLAYER_SYMPATHY, 0, 10, HintText = DramalordTexts.MCM_GENERAL_PLAYER_SYMPATHY_INFO, Order = 3, RequireRestart = false)]
        public int PlayerBaseSympathy { get; set; } = 5;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_GENERAL_INTERACTION_DELAY, 0, 100, HintText = DramalordTexts.MCM_GENERAL_INTERACTION_DELAY_INFO, Order = 4, RequireRestart = false)]
        public int DaysBetweenInteractions { get; set; } = 1;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyFloatingInteger(DramalordTexts.MCM_GENERAL_QUEST_CHANCE, 0, 100, Order = 5, HintText = DramalordTexts.MCM_GENERAL_QUEST_CHANCE_INFO, RequireRestart = false)]
        public int QuestChance { get; set; } = 30;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyBool(DramalordTexts.MCM_GENERAL_DEFAULT_PREGNANCIES, HintText = DramalordTexts.MCM_GENERAL_DEFAULT_PREGNANCIES_INFO, Order = 6, RequireRestart = false)]
        public bool AllowDefaultPregnancies { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyBool(DramalordTexts.MCM_GENERAL_DEFAULT_MARRIAGES, HintText = DramalordTexts.MCM_GENERAL_DEFAULT_MARRIAGES_INFO, Order = 7, RequireRestart = false)]
        public bool AllowDefaultMarriages { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyBool(DramalordTexts.MCM_GENERAL_ALLOW_INCEST, HintText = DramalordTexts.MCM_GENERAL_ALLOW_INCEST_INFO, Order = 8, RequireRestart = false)]
        public bool AllowIncest { get; set; } = false;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyBool(DramalordTexts.MCM_GENERAL_ALLOW_SAMESEX_MARRIAGE, HintText = DramalordTexts.MCM_GENERAL_ALLOW_SAMESEX_MARRIAGE_INFO, Order = 9, RequireRestart = false)]
        public bool AllowSameSexMarriage { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_GENERAL)]
        [SettingPropertyBool(DramalordTexts.MCM_GENERAL_ALLOW_CLASS_MIX, HintText = DramalordTexts.MCM_GENERAL_ALLOW_CLASS_MIX_INFO, Order = 10, RequireRestart = false)]
        public bool AllowSocialClassMix { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_OPTIONAL)]
        [SettingPropertyBool(DramalordTexts.MCM_OPTIONAL_SHOW_RELATION_CHANGES, Order = 1, HintText = DramalordTexts.MCM_OPTIONAL_SHOW_RELATION_CHANGES_INFO, RequireRestart = false)]
        public bool ShowRelationChanges { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_OPTIONAL)]
        [SettingPropertyBool(DramalordTexts.MCM_OPTIONAL_SHOW_REAL_RELATIONS, Order = 2, HintText = DramalordTexts.MCM_OPTIONAL_SHOW_REAL_RELATIONS_INFO, RequireRestart = false)]
        public bool ShowRealrelation { get; set; } = true;

        [SettingPropertyGroup(DramalordTexts.MCM_OPTIONAL)]
        [SettingPropertyBool("DEBUG: Show All Logs", Order = 3, HintText = "Debugmode: Show all Dramalord log output", RequireRestart = false)]
        public bool DEBUGLOG { get; set; } = true;


        public override string Id => DramalordSubModule.ModuleName;

        public override string DisplayName => DramalordSubModule.ModuleName;

        public override string FolderName => DramalordSubModule.ModuleName;

        public override string FormatType => "json";
    }
}
