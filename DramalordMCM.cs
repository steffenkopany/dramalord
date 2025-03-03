using Dramalord.Data;
using Dramalord.Data.Intentions;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace Dramalord
{
    internal sealed class DramalordMCM : AttributeGlobalSettings<DramalordMCM>
    {
        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord002}Wanderer Autonomy", HintText = "{=Dramalord003}Allow clanless Wanderers to interact with lords and other wanderers", Order = 1, RequireRestart = false)]
        public bool AllowWandererAutonomy { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord237}Player Clan Autonomy", HintText = "{=Dramalord238}Allow heroes of your clan to interact with lords and other wanderers", Order = 2, RequireRestart = false)]
        public bool AllowPlayerClanAutonomy { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord505}Allow Notables Autonomy", Order = 3, HintText = "{=Dramalord506}Include Notables like Gang leaders in Dramalord AI (experimental, performance impact)", RequireRestart = false)]
        public bool AllowNotablesAutonomy { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyFloatingInteger("{=Dramalord201}Chance Approaching Player", 0, 100, Order = 4, HintText = "{=Dramalord202}Chance that heroes approach the player for conversations", RequireRestart = false)]
        public int ChanceApproachingPlayer { get; set; } = 30;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyFloatingInteger("{=Dramalord598}Chance Gossiping Player", 0, 100, Order = 5, HintText = "{=Dramalord599}Chance that heroes approach the player for spreading gossip", RequireRestart = false)]
        public int ChanceGossipingPlayer { get; set; } = 30;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord298}Allow Default Pregnancies", HintText = "{=Dramalord299}Allow default pregnancies for all npcs", Order = 6, RequireRestart = false)]
        public bool AllowDefaultPregnancies { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord300}Allow Default Marriages", HintText = "{=Dramalord301}Allow default marriages for all npcs", Order = 7, RequireRestart = false)]
        public bool AllowDefaultMarriages { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord242}Allow Incest", HintText = "{=Dramalord243}Allow AI to approach family members for intimate and emotional events", Order = 8, RequireRestart = false)]
        public bool AllowIncest { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord246}Allow Same Sex Marriage", HintText = "{=Dramalord247}Allow AI to marry other heroes of the same sex (otherwise they are just lovers)", Order = 9, RequireRestart = false)]
        public bool AllowSameSexMarriage { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord576}Allow Social Class Mix", HintText = "{=Dramalord577}Allow AI interactions between nobles and commoners (Lords and Notables/Wanderers)", Order = 10, RequireRestart = false)]
        public bool AllowSocialClassMix { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord001}Autonomy")]
        [SettingPropertyBool("{=Dramalord574}Join Clan On Marriage", HintText = "{=Dramalord575}Allow AI to join the clan of their spouse (otherwise they're just married)", Order = 11, RequireRestart = false)]
        public bool JoinClanOnMarriage { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord005}Minimum Attraction", 0, 100, Order = 1, HintText = "{=Dramalord006}Attraction score required for NPCs to consider others as attractive", RequireRestart = false)]
        public int MinAttraction { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord191}Minimum Dating Love", 0, 100, Order = 2, HintText = "{=Dramalord192}Love points required to consider dating a hero", RequireRestart = false)]
        public int MinDatingLove { get; set; } = 30;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord205}Minimum Marriage Love", 0, 100, Order = 3, HintText = "{=Dramalord206}Love points required to consider to marry a hero", RequireRestart = false)]
        public int MinMarriageLove { get; set; } = 75;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord337}Minimum Trust Friends", 10, 100, Order = 4, HintText = "{=Dramalord338}Trust points required to consider a hero a friend", RequireRestart = false)]
        public int MinTrustFriends { get; set; } = 40;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord564}Maximum Trust Enemies", -100, -10, Order = 5, HintText = "{=Dramalord565}Negative trust points required to consider a hero an enemy", RequireRestart = false)]
        public int MaxTrustEnemies { get; set; } = -30;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord421}Minimum Trust FWB", 0, 100, Order = 6, HintText = "{=Dramalord422}Trust points required to consider a hero being a friend with benefits", RequireRestart = false)]
        public int MinTrustFWB { get; set; } = 75;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord187}Pregnancy Duration", 1, 100, Order = 7, HintText = "{=Dramalord188}How many days is a hero pregnant before giving birth", RequireRestart = false)]
        public int PregnancyDuration { get; set; } = 21;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord189}Pregnancy Chance", 0, 100, Order = 8, HintText = "{=Dramalord190}Chance a hero gets pregnant (only for Dramalord pregnancies)", RequireRestart = false)]
        public int PregnancyChance { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord520}Maximum Fertility Age", 18, 100, Order = 9, HintText = "{=Dramalord521}Maximum age a hero can get pregnant (only for Dramalord pregnancies)", RequireRestart = false)]
        public int MaxFertilityAge { get; set; } = 45;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord295}Toy Break Chance", 0, 100, Order = 11, HintText = "{=Dramalord296}Chance a toy can break while being used by a hero.", RequireRestart = false)]
        public int ToyBreakChance { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord480}Love Decay Start Day", 0, 100, Order = 12, HintText = "{=Dramalord481}Number of days lovers havent seen each others needed to make love decay.", RequireRestart = false)]
        public int LoveDecayStartDay { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord004}Hero Setup")]
        [SettingPropertyBool("{=Dramalord592}FWB Pregnancy", Order = 13, HintText = "{=Dramalord593}Friends with benefits can get pregnant", RequireRestart = false)]
        public bool FWBPregnancy { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord007}Logging Setup")]
        [SettingPropertyBool("{=Dramalord484}Intimate Logs", Order = 0, HintText = "{=Dramalord485}Show intimate events in the logs", RequireRestart = false)]
        public bool IntimateLogs { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord007}Logging Setup")]
        [SettingPropertyBool("{=Dramalord008}Relationship Logs", Order = 1, HintText = "{=Dramalord009}Show relationship changes in the logs", RequireRestart = false)]
        public bool RelationshipLogs { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord007}Logging Setup")]
        [SettingPropertyBool("{=Dramalord083}Clan Changes Logs", Order = 2, HintText = "{=Dramalord084}Show clan changes in the logs", RequireRestart = false)]
        public bool ClanChangeLogs { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord007}Logging Setup")]
        [SettingPropertyBool("{=Dramalord162}Children Logs", Order = 3, HintText = "{=Dramalord163}Show birth, conceptions or orphanage events in the logs", RequireRestart = false)]
        public bool ChildrenEventLogs { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord007}Logging Setup")]
        [SettingPropertyBool("{=Dramalord021}Show Only Clan Interaction", Order = 4, HintText = "{=Dramalord022}Show only interactions of your clan members in the chatlog", RequireRestart = false)]
        public bool ShowOnlyClanInteractions { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyFloatingInteger("{=Dramalord017}Chance Getting Caught", 0, 100, Order = 1, HintText = "{=Dramalord018}Chance of getting caught by partners while interacting with other heroes", RequireRestart = false)]
        public int ChanceGettingCaught { get; set; } = 30;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyFloatingInteger("{=Dramalord199}Bonus Player Attraction", 0, 100, HintText = "{=Dramalord200}Bonus value for player's attraction to other heroes", Order = 2, RequireRestart = false)]
        public int PlayerBaseAttraction { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyFloatingInteger("{=Dramalord217}Bonus Player Sympathy", 0, 10, HintText = "{=Dramalord218}Bonus value for player's sympathy to other heroes", Order = 3, RequireRestart = false)]
        public int PlayerBaseSympathy { get; set; } = 5;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyFloatingInteger("{=Dramalord207}Days Between Interactions", 0, 100, HintText = "{=Dramalord208}Number of days heroes can't interact with the same hero again", Order = 4, RequireRestart = false)]
        public int DaysBetweenInteractions { get; set; } = 1;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyBool("{=Dramalord213}Player Always Witness", Order = 6, HintText = "{=Dramalord214}The player will always be witness of events", RequireRestart = false)]
        public bool PlayerAlwaysWitness { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyBool("{=Dramalord494}Player Spouse Faithful", Order = 10, HintText = "{=Dramalord495}The spouse of the player will not interact with anyone else", RequireRestart = false)]
        public bool PlayerSpouseFaithful { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyFloatingInteger("{=Dramalord311}Quest Chance", 0, 100, Order = 11, HintText = "{=Dramalord312}Chance that a lover of the player starts a quest", RequireRestart = false)]
        public int QuestChance { get; set; } = 30;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyBool("{=Dramalord500}Keep Children in Clans", Order = 12, HintText = "{=Dramalord501}Non legitimate children born into clans will not end up in the orphanage", RequireRestart = false)]
        public bool KeepClanChildren { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyBool("{=Dramalord522}Keep Children of Notables", Order = 13, HintText = "{=Dramalord523}Non legitimate children born to notables will not end up in the orphanage", RequireRestart = false)]
        public bool KeepNotableChildren { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord016}General")]
        [SettingPropertyBool("{=Dramalord585}Always Duel To Death", Order = 14, HintText = "{=Dramalord586}Duels with other heroes are always to the death", RequireRestart = false)]
        public bool AlwaysDuelToDeath { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord313}Optional")]
        [SettingPropertyBool("{=Dramalord314}Show Relation Changes", Order = 1, HintText = "{=Dramalord315}Enable this if you want to see relation change notifications (required for the Small Talk mod)", RequireRestart = false)]
        public bool ShowRelationChanges { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord313}Optional")]
        [SettingPropertyBool("{=Dramalord562}Show Real Relation", Order = 2, HintText = "{=Dramalord563}Enable this if you want to see the real relation to a hero (turn off if you're using True Noble Opinion)", RequireRestart = false)]
        public bool ShowRealrelation { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord313}Optional")]
        [SettingPropertyBool("{=Dramalord594}Enable Dramalord Pregnancy", Order = 3, HintText = "{=Dramalord595}Enable Dramalords pregnancy system (Disabled automatically if incompatible mod is detected)", RequireRestart = false)]
        public bool EnableDramalordPregnancy
        {
            get => !IntercourseIntention.OtherPregnancyModFound;
            set => IntercourseIntention.OtherPregnancyModFound = !value;
        }

        [SettingPropertyGroup("{=Dramalord313}Optional")]
        [SettingPropertyBool("{=Dramalord596}Enable Dramalord Marriage", Order = 3, HintText = "{=Dramalord597}Enable Dramalords marriage system (Disabled automatically if incompatible mod is detected)", RequireRestart = false)]
        public bool EnableDramalordMarriage
        {
            get => !BethrothIntention.OtherMarriageModFound;
            set => BethrothIntention.OtherMarriageModFound = !value;
        }

        public override string Id => DramalordSubModule.ModuleName;

        public override string DisplayName => DramalordSubModule.ModuleName;

        public override string FolderName => DramalordSubModule.ModuleName;

        public override string FormatType => "json";
    }
}
