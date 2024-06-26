using Dramalord.Data;
using HarmonyLib;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Abstractions.Base.PerCampaign;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TaleWorlds.CampaignSystem;
using static TaleWorlds.Engine.MeshBuilder;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using System;

namespace Dramalord
{
    internal sealed class DramalordMCM : AttributeGlobalSettings<DramalordMCM>
    {
        internal static Hero? SelectedHero = null;

        [AllowNull]
        internal static DramalordMCM Get => AttributeGlobalSettings<DramalordMCM>.Instance;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord431}Only Player Clan Logs", HintText = "{=Dramalord432}Show only logs related to the player clan", Order = 1, RequireRestart = false)]
        public bool OnlyPlayerClanOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord151}Flirt Logs", HintText = "{=Dramalord152}Show flirt events in logs", Order = 1, RequireRestart = false)]
        public bool FlirtOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord153}Affair Logs", HintText = "{=Dramalord154}Show affair events in logs", Order = 2, RequireRestart = false)]
        public bool AffairOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord239}Kingdom Changes Logs", HintText = "{=Dramalord240}Show kingdom related events in logs", Order = 3, RequireRestart = false)]
        public bool KingdomOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord155}Clan Changes Logs", HintText = "{=Dramalord156}Show clan related events in logs", Order = 3, RequireRestart = false)]
        public bool ClanOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord157}Marriage Logs", HintText = "{=Dramalord158}Show marriage events in logs", Order = 4, RequireRestart = false)]
        public bool MarriageOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord159}Death Logs", HintText = "{=Dramalord160}Show death (details) events in logs", Order = 5, RequireRestart = false)]
        public bool DeathOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord161}Birth Logs", HintText = "{=Dramalord162}Show birth/orphan events in logs", Order = 6, RequireRestart = false)]
        public bool BirthOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord343}Confrontation Logs", HintText = "{=Dramalord344}Show confrontation events in logs", Order = 7, RequireRestart = false)]
        public bool ConfrontationOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord345}Gossip Logs", HintText = "{=Dramalord346}Show gossip events in logs", Order = 8, RequireRestart = false)]
        public bool GossipOutput { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyFloatingInteger("{=Dramalord163}Trait-Score Multiplier", 1, 10, HintText = "{=Dramalord164}Trait score multiplier for AI emotion grow (higher = faster)", Order = 9, RequireRestart = false)]
        public int TraitScoreMultiplyer { get; set; } = 1;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord165}Protect Family", HintText = "{=Dramalord166}No AI interaction with family members (incest)", Order = 10, RequireRestart = false)]
        public bool ProtectFamily { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord241}Wanderer AI interaction", HintText = "{=Dramalord242}Allow Wanderers to interact with lords and other wanderers", Order = 9, RequireRestart = false)]
        public bool AllowWandererAI { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord243}Player Clan AI interaction", HintText = "{=Dramalord244}Allow Members of your clan to participate in AI interactions", Order = 10, RequireRestart = false)]
        public bool AllowPlayerClanAI { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord247}Heroes in Army AI interaction", HintText = "{=Dramalord248}Allow Members of an army to interact with each other", Order = 11, RequireRestart = false)]
        public bool AllowArmyInteractionAI { get; set; } = true;
        /*
        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord249}Allow Clan banishment", HintText = "{=Dramalord250}Allow Kingdom leaders to banish clans if they're angry on their leader", Order = 12, RequireRestart = false)]
        public bool AllowClanBanishment { get; set; } = true;
        */
        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyFloatingInteger("{=Dramalord251}NPC Approach Player Chance", 0, 100, HintText = "{=Dramalord252}Chance that heroes approach the player for intimate conversations", Order = 13, RequireRestart = false)]
        public int ChanceNPCApproachPlayer { get; set; } = 20;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord259}Approach Player In Settlements", HintText = "{=Dramalord260}Allow NPCs to approach you while being in the same settlement", Order = 14, RequireRestart = false)]
        public bool AllowApproachInSettlement { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord261}Approach Player In Party", HintText = "{=Dramalord262}Allow NPCs to approach you while being in your party", Order = 15, RequireRestart = false)]
        public bool AllowApproachInParty { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord284}Approach Player In Army", HintText = "{=Dramalord285}Allow NPCs to approach you while being in the same army", Order = 15, RequireRestart = false)]
        public bool AllowApproachInArmy { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord274}Link Emotion Changes to Relation", HintText = "{=Dramalord275}Allow changes to emotion being reflected to relation", Order = 16, RequireRestart = false)]
        public bool LinkEmotionToRelation { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord276}Allow Marriages", HintText = "{=Dramalord277}Allow marriages triggered by Dramalord (the original marriage system still works)", Order = 17, RequireRestart = false)]
        public bool AllowMarriages { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord278}Allow Divorces", HintText = "{=Dramalord279}Allow marriages being divorced by Dramalord events", Order = 18, RequireRestart = false)]
        public bool AllowDivorces { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord280}Allow Rage Kills", HintText = "{=Dramalord281}Allow enraged heroes to kill if being cheated on", Order = 19, RequireRestart = false)]
        public bool AllowRageKills { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord282}Allow Clan Changes", HintText = "{=Dramalord283}Allow heroes being kicked out of clans or leaving voluntarily", Order = 20, RequireRestart = false)]
        public bool AllowClanChanges { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord424}Allow AI Adoption", HintText = "{=Dramalord425}Allow heroes adopting children from the orphanage", Order = 20, RequireRestart = false)]
        public bool AllowAIAdoptions { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyFloatingInteger("{=Dramalord286}NPC Visit Quest Chance", 0, 100, HintText = "{=Dramalord287}Chance that heroes request the absent player to visit them due to urgent needs", Order = 21, RequireRestart = false)]
        public int ChanceNPCQuestVisitPlayer { get; set; } = 20;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord360}Enable Default Pregnancies", HintText = "{=Dramalord361}Allow default Bannerlord pregnancies to happen", Order = 22, RequireRestart = false)]
        public bool AllowDefaultPregnancies { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord362}Enable Default Marriages", HintText = "{=Dramalord363}Allow default Bannerlord marriages to happen", Order = 23, RequireRestart = false)]
        public bool AllowDefaultMarriages { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord364}Show lovers in Encyclopedia", HintText = "{=Dramalord365}Show lovers in Encyclopedia (Family section)", Order = 24, RequireRestart = false)]
        public bool ShowLoversEncyclopedia { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord366}Show friends with benefits in Encyclopedia", HintText = "{=Dramalord367}Show friends with benefits in Encyclopedia (Family section)", Order = 24, RequireRestart = false)]
        public bool ShowFWBEncyclopedia { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyFloatingInteger("{=Dramalord368}Date Memory Duration", 1, 100, HintText = "{=Dramalord369}Time in days heroes remember date events", Order = 25, RequireRestart = false)]
        public int DateMemoryDuration { get; set; } = 5;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyFloatingInteger("{=Dramalord370}Intercourse Memory Duration", 1, 100, HintText = "{=Dramalord371}Time in days heroes remember intercouse events", Order = 26, RequireRestart = false)]
        public int IntercourseMemoryDuration { get; set; } = 7;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyFloatingInteger("{=Dramalord372}Birth Memory Duration", 1, 100, HintText = "{=Dramalord373}Time in days heroes remember birth events", Order = 27, RequireRestart = false)]
        public int BirthMemoryDuration { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyFloatingInteger("{=Dramalord374}Marriage Memory Duration", 1, 100, HintText = "{=Dramalord375}Time in days heroes remember marriage events", Order = 28, RequireRestart = false)]
        public int MarriageMemoryDuration { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord429}Allow AI Same Sex Marriage", HintText = "{=Dramalord430}Allow AI to marry heroes of the same sex (immersion option)", Order = 29, RequireRestart = false)]
        public bool AllowAISameSexMarriage { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord167}Other Sex Attraction Modifier", -50, 50, Order = 1, HintText = "{=Dramalord168}AI attraction modifier for the other sex (negative = own sex, positive = opposite sex, 0 = neutral)", RequireRestart = false)]
        public int OtherSexAttractionModifier { get; set; } = 0;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord169}Min. Attraction Score", 0, 100, Order = 2, HintText = "{=Dramalord170}AI minimum attraction score to to start interacting with each other", RequireRestart = false)]
        public int MinAttractionForFlirting { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord171}Min. Emotion For Affairs", 0, 100, Order = 4, HintText = "{=Dramalord172}Minimum emotion value for AI to consider starting an affair", RequireRestart = false)]
        public int MinEmotionForDating { get; set; } = 30;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord173}Days Between Discreet Meetings", 0, 100, Order = 5, HintText = "{=Dramalord174}Days AI will wait before having a discreet meeting with the same hero again", RequireRestart = false)]
        public int DaysBetweenDates { get; set; } = 2;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord175}Min. Emotion For Marriage", 0, 100, Order = 6, HintText = "{=Dramalord176}Minimum emotion value for AI to consider marriage with a hero", RequireRestart = false)]
        public int MinEmotionForMarriage { get; set; } = 80;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord177}Min. Emotion For Divorce", 0, 100, Order = 7, HintText = "{=Dramalord178}Emotion value for AI to consider divorce", RequireRestart = false)]
        public int MinEmotionBeforeDivorce { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord179}Min. Horny Value For Intercouse", 0, 100, Order = 8, HintText = "{=Dramalord180}Minimum horniness of AI for interest in intercourse", RequireRestart = false)]
        public int MinHornyForIntercourse { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord181}Intercourse Horniness Loss", 0, 100, Order = 9, HintText = "{=Dramalord182}Value of horniness lost by intercourse", RequireRestart = false)]
        public int HornyLossIntercourse { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord340}Intercourse Tension Loss", 0, 100, Order = 10, HintText = "{=Dramalord341}Value of tension lost by intercourse", RequireRestart = false)]
        public int TensionLossIntercourse { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord183}Chance Getting Caught", 0, 100, Order = 11, HintText = "{=Dramalord184}Chance of getting caught by by partners while interacting wth other heroes", RequireRestart = false)]
        public int ChanceGettingCaught { get; set; } = 10;
        /*
                [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
                [SettingPropertyFloatingInteger("{=Dramalord185}Emotional Loss Flirting Witnessed", 0, 100, Order = 12, HintText = "{=Dramalord186}Loss of emotion of heroes witnessing their partners flirtingwith others", RequireRestart = false)]
                public int EmotionalLossCaughtFlirting { get; set; } = 5;

                [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
                [SettingPropertyFloatingInteger("{=Dramalord187}Emotional Loss Meeting Witnessed", 0, 100, Order = 13, HintText = "{=Dramalord188}Loss of emotion of heroes witnessing their partners meeting in secret others", RequireRestart = false)]
                public int EmotionalLossCaughtDate { get; set; } = 20;

                [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
                [SettingPropertyFloatingInteger("{=Dramalord189}Emotional Loss Intercourse Witnessed", 0, 100, Order = 14, HintText = "{=Dramalord190}Loss of emotion of heroes witnessing their partner having intercourse with others", RequireRestart = false)]
                public int EmotionalLossCaughtIntercourse { get; set; } = 50;

                [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
                [SettingPropertyFloatingInteger("{=Dramalord191}Emotional Loss Marry Other", 0, 100, Order = 15, HintText = "{=Dramalord192}Loss of emotion of hero if their partner marries someone else", RequireRestart = false)]
                public int EmotionalLossMarryOther { get; set; } = 60;
                */
        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord193}Emotional Loss Breakup", 0, 100, Order = 16, HintText = "{=Dramalord194}Loss of emotion of hero if their partner breaks up with them (affairs)", RequireRestart = false)]
        public int EmotionalLossBreakup { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord195}Emotional Loss Divorce", 0, 100, Order = 17, HintText = "{=Dramalord196}Loss of emotion of hero if their partner ends their marriage", RequireRestart = false)]
        public int EmotionalLossDivorce { get; set; } = 80;
        /*
        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord197}Emotional Loss Pregnancy", 0, 100, Order = 14, HintText = "{=Dramalord198}Loss of emotion of heroes if their partners are visibly pregnant from someone else", RequireRestart = false)]
        public int EmotionalLossPregnancy { get; set; } = 80;
        
        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord199}Emotional Loss Bastard", 0, 100, Order = 15, HintText = "{=Dramalord200}Loss of emotion of partners if heros give birth to children of someone else", RequireRestart = false)]
        public int EmotionalLossBastard { get; set; } = 100;
        */
        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord199}Emotional Loss Gossip", 0, 100, Order = 15, HintText = "{=Dramalord200}Loss of emotion of heroes if they hear gossip about their partners", RequireRestart = false)]
        public int EmotionalLossGossip { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord205}Pregnancy Chance", 0, 100, Order = 18, HintText = "{=Dramalord206}Chance of getting pregnant during intercourse", RequireRestart = false)]
        public int PregnancyChance { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord207}Pregnancy Duration", 1, 100, Order = 19, HintText = "{=Dramalord208}How many days a hero is pregnant before giving birth", RequireRestart = false)]
        public int PregnancyDuration { get; set; } = 21;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord209}Days Until Pregnancy Visible", 0, 100, Order = 20, HintText = "{=Dramalord210}How many days a hero is pregnant before the pregnancy is visible to other heroes", RequireRestart = false)]
        public int DaysUntilPregnancyVisible { get; set; } = 15;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord211}Wait Between Adopting", 1, 100, Order = 21, HintText = "{=Dramalord212}How many days to wait between adptions (same sex or old couples)", RequireRestart = false)]
        public int WaitBetweenAdopting { get; set; } = 84;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord213}Max. Fertility Age", 18, 130, Order = 22, HintText = "{=Dramalord214}Maximum age of being fertile", RequireRestart = false)]
        public int MaxFertilityAge { get; set; } = 45;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord215}Period Duration", 1, 21, Order = 23, HintText = "{=Dramalord216}Duration of period in days for female heroes per season (can't get pregnant)", RequireRestart = false)]
        public int PeriodDuration { get; set; } = 5;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord217}Toy Break Chance", 0, 100, Order = 24, HintText = "{=Dramalord218}Chance a toy will break when being used by a hero", RequireRestart = false)]
        public int ToyBreakChance { get; set; } = 5;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord376}Minimum Relation Level", 0, 100, HintText = "{=Dramalord377}Minimum amount of relation is required making a hero trust another", Order = 25, RequireRestart = false)]
        public int MinimumRelationLevel { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord379}Anger Duration Flirt", 0, 100, HintText = "{=Dramalord380}How long a hero is angry in days if witnessing their partner flirting with someone else", Order = 26, RequireRestart = false)]
        public int AngerDaysFlirt { get; set; } = 1;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord381}Anger Duration Date", 0, 100, HintText = "{=Dramalord382}How long a hero is angry in days if witnessing their partner on a date with someone else", Order = 27, RequireRestart = false)]
        public int AngerDaysDate { get; set; } = 3;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord383}Anger Duration Intercourse", 0, 100, HintText = "{=Dramalord384}How long a hero is angry in days if witnessing their partner having intercourse with someone else", Order = 28, RequireRestart = false)]
        public int AngerDaysIntercourse { get; set; } = 8;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord385}Anger Duration Pregnancy", 0, 100, HintText = "{=Dramalord386}How long a hero is angry in days if witnessing their partner being pregnant by someone else", Order = 29, RequireRestart = false)]
        public int AngerDaysPregnancy { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord387}Anger Duration Bastard", 0, 100, HintText = "{=Dramalord388}How long a hero is angry in days if witnessing their partner birthing a child by someone else", Order = 30, RequireRestart = false)]
        public int AngerDaysBastard { get; set; } = 21;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord389}Anger Duration Marriage", 0, 100, HintText = "{=Dramalord390}How long a hero is angry in days if witnessing their partner marry someone else", Order = 31, RequireRestart = false)]
        public int AngerDaysMarriage { get; set; } = 21;

        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyFloatingInteger("{=Dramalord219}Min. Emotion For Conversation", 0, 100, Order = 1, HintText = "{=Dramalord220}Minimum emotion value of AI to accept an intimate conversation with the player", RequireRestart = false)]
        public int MinEmotionForConversation { get; set; } = 0;

        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyFloatingInteger("{=Dramalord438}Bonus Player Attraction", 0, 100, HintText = "{=Dramalord439}Bonus value for player's attraction to other heroes", Order = 2, RequireRestart = false)]
        public int PlayerBaseAttraction { get; set; } = 10;
        /*
        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyFloatingInteger("{=Dramalord440}Bonus Player Emotion", 0, 100, HintText = "{=Dramalord441}Bonus value for heroes emotion they have for the player", Order = 3, RequireRestart = false)]
        public int PlayerBaseEmotion { get; set; } = 10;
        */
        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyBool("{=Dramalord253}Individual Relations", Order = 4, HintText = "{=Dramalord254}Use/Show individual relation Lords/Ladies instead of their clan leader relation", RequireRestart = false)]
        public bool IndividualRelation { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyBool("{=Dramalord298}Interact Being Witness", Order = 5, HintText = "{=Dramalord299}Interact when witnessing your partner doing stuff with others (otherwise you will just get a notification)", RequireRestart = false)]
        public bool InteractOnBeingWitness { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord150}Extra QOL")]
        [SettingPropertyBool("{=Dramalord225}No Captivity Messages", Order = 1, HintText = "{=Dramalord226}Disable spam of captured or freed heroes (english only!)", RequireRestart = false)]
        public bool NoCaptivityMessages { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord150}Extra QOL")]
        [SettingPropertyBool("{=Dramalord227}No Relation Change Notification", Order = 2, HintText = "{=Dramalord228}Disable spam of relation increase with notables", RequireRestart = false)]
        public bool NoRelationNotification { get; set; } = true;

        [SettingPropertyGroup("{=Dramalord150}Extra QOL")]
        [SettingPropertyFloatingInteger("Attraction Generation Weight", -50, 50, HintText = "Negative values generate more homosexual interest, positive values generate more heterosexual interest", Order = 3, RequireRestart = false)]
        public int TraitGenerationWeight { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord150}Extra QOL")]
        [SettingPropertyButton("Regenerate Traits", Order = 4, Content = "Click to generate", HintText = "Set a weight above and press this button to regenerate dramalord traits (only works with running campaign)", RequireRestart = false)]
        public Action RegenerateTraits { get; set; } = (() =>
        {
            if (Campaign.Current != null)
            {
                Hero.AllAliveHeroes.Where(hero => hero.IsDramalordLegit()).Do(hero =>
                {
                    HeroTraits.ApplyToHero(hero, true);
                });
            }
        });

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyText("Selected Hero:", Order = 1, RequireRestart = false)]
        public string HeroName
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.Name.ToString() : "None";
            }
            set { }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("Openness", -100, 100, HintText = "Represents how willing a person is to try new things", Order = 2, RequireRestart = false)]
        public int Openness
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().Openness : 0;
            }
            set
            {
                if(SelectedHero != null) SelectedHero.GetDramalordTraits().Openness = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("Conscientiousness", -100, 100, HintText = "Refers to an individual's desire to be careful and diligent", Order = 3, RequireRestart = false)]
        public int Conscientiousness
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().Conscientiousness : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().Conscientiousness = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("Extroversion", -100, 100, HintText = "Measures how energetic, outgoing and confident a person is", Order = 4, RequireRestart = false)]
        public int Extroversion
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().Extroversion : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().Extroversion = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("Agreeableness", -100, 100, HintText = "Refers to how an individual interacts with others", Order = 5, RequireRestart = false)]
        public int Agreeableness
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().Agreeableness : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().Agreeableness = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("Neuroticism", -100, 100, HintText = "Represents how much someone is inclined to experience negative emotions", Order = 6, RequireRestart = false)]
        public int Neuroticism
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().Neuroticism : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().Neuroticism = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("AttractionMen", 0, 100, HintText = "Defines whether an individual finds male persons attractive or not", Order = 7, RequireRestart = false)]
        public int AttractionMen
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().AttractionMen : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().AttractionMen = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("AttractionWomen", 0, 100, HintText = "Defines whether an individual finds female persons attractive or not", Order = 8, RequireRestart = false)]
        public int AttractionWomen
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().AttractionWomen : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().AttractionWomen = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("AttractionWeight", 0, 100, HintText = "Defines whether an individual has interest in chubby or thin heroes", Order = 9, RequireRestart = false)]
        public int AttractionWeight
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().AttractionWeight : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().AttractionWeight = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("AttractionBuild", 0, 100, HintText = "Defines whether an individual has interest in muscular or weak heroes", Order = 10, RequireRestart = false)]
        public int AttractionBuild
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().AttractionBuild : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().AttractionBuild = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("AttractionAgeDiff", -20, 20, HintText = "Defines whether an individual has interest in older or younger heroes in year difference", Order = 11, RequireRestart = false)]
        public int AttractionAgeDiff
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().AttractionAgeDiff : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().AttractionAgeDiff = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("Libido", 0, 10, HintText = "Defines whether an individual generally develops interest in intercourse or not", Order = 12, RequireRestart = false)]
        public int Libido
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().Libido : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().Libido = value;
            }
        }

        [SettingPropertyGroup("Trait Editor")]
        [SettingPropertyFloatingInteger("Horny", 0, 100, HintText = "Represents how willing a hero currently is for intercourse due to hormons", Order = 13, RequireRestart = false)]
        public int Horny
        {
            get
            {
                return (SelectedHero != null) ? SelectedHero.GetDramalordTraits().Horny : 0;
            }
            set
            {
                if (SelectedHero != null) SelectedHero.GetDramalordTraits().Horny = value;
            }
        }

        public override string Id => DramalordSubModule.ModuleName;

        public override string DisplayName => DramalordSubModule.ModuleName;

        public override string FolderName => DramalordSubModule.ModuleFolder;

        public override string FormatType => "json";
    }
}
