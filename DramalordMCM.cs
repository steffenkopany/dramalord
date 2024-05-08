using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Abstractions.Base.PerCampaign;
using System.Diagnostics.CodeAnalysis;

namespace Dramalord
{
    internal sealed class DramalordMCM : AttributeGlobalSettings<DramalordMCM>
    {
        [AllowNull]
        internal static DramalordMCM Get => AttributeGlobalSettings<DramalordMCM>.Instance;

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
        [SettingPropertyFloatingInteger("{=Dramalord163}Trait-Score Multiplier", 1, 10, HintText = "{=Dramalord164}Trait score multiplier for AI emotion grow (higher = faster)", Order = 7, RequireRestart = false)]
        public int TraitScoreMultiplyer { get; set; } = 1;

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord165}Protect Family", HintText = "{=Dramalord166}No AI interaction with family members", Order = 8, RequireRestart = false)]
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

        [SettingPropertyGroup("{=Dramalord147}General")]
        [SettingPropertyBool("{=Dramalord249}Allow Clan banishment", HintText = "{=Dramalord250}Allow Kingdom leaders to banish clans if they're angry on their leader", Order = 12, RequireRestart = false)]
        public bool AllowClanBanishment { get; set; } = true;

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
        [SettingPropertyFloatingInteger("{=Dramalord286}NPC Visit Quest Chance", 0, 100, HintText = "{=Dramalord287}Chance that heroes request the absent player to visit them due to urgent needs", Order = 21, RequireRestart = false)]
        public int ChanceNPCQuestVisitPlayer { get; set; } = 20;

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
        /*
        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("Days Apart Start Forget", 0, 100, Order = 10, HintText = "Days AI has no conversation with other heroes before forgetting emotional value", RequireRestart = false)]
        public int DaysApartStartForget { get; set; } = 21;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("Amount Emotion Forget", 0, 100, Order = 10, HintText = "Emotional value AI forgets about others after passing the days apart border", RequireRestart = false)]
        public int AmountEmotionForget { get; set; } = 1;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("Days Apart Completely Forgotten", 0, 100, Order = 10, HintText = "Days AI has no conversation with other heroes before forgetting them completely", RequireRestart = false)]
        public int DaysApartCompleteForget { get; set; } = 84;
        */
        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord183}Chance Getting Caught", 0, 100, Order = 11, HintText = "{=Dramalord184}Chance of getting caught by by partners while interacting wth other heroes", RequireRestart = false)]
        public int ChanceGettingCaught { get; set; } = 10;

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

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord193}Emotional Loss Breakup", 0, 100, Order = 16, HintText = "{=Dramalord194}Loss of emotion of hero if their partner breaks up with them (affairs)", RequireRestart = false)]
        public int EmotionalLossBreakup { get; set; } = 50;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord195}Emotional Loss Divorce", 0, 100, Order = 17, HintText = "{=Dramalord196}Loss of emotion of hero if their partner ends their marriage", RequireRestart = false)]
        public int EmotionalLossDivorce { get; set; } = 80;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord197}Emotional Loss Pregnancy", 0, 100, Order = 14, HintText = "{=Dramalord198}Loss of emotion of heroes if their partners are visibly pregnant from someone else", RequireRestart = false)]
        public int EmotionalLossPregnancy { get; set; } = 80;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord199}Emotional Loss Bastard", 0, 100, Order = 15, HintText = "{=Dramalord200}Loss of emotion of partners if heros give birth to children of someone else", RequireRestart = false)]
        public int EmotionalLossBastard { get; set; } = 100;
        /*
        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord201}Emotional Win Affair", 0, 100, Order = 16, HintText = "{=Dramalord202}Emotional gain of heroes who start an affair", RequireRestart = false)]
        public int EmotionalWinAffair { get; set; } = 10;

        [SettingPropertyGroup("{=Dramalord148}Hero Setup")]
        [SettingPropertyFloatingInteger("{=Dramalord203}Emotional Win Marriage", 0, 100, Order = 17, HintText = "{=Dramalord204}Emotional gain of heroes who are getting married", RequireRestart = false)]
        public int EmotionalWinMarriage { get; set; } = 20;
        */
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

        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyFloatingInteger("{=Dramalord219}Min. Emotion For Conversation", 0, 100, Order = 1, HintText = "{=Dramalord220}Minimum emotion value of AI to accept an intimate conversation with the player", RequireRestart = false)]
        public int MinEmotionForConversation { get; set; } = 0;

        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyBool("{=Dramalord221}Player Always Attractive", Order = 2, HintText = "{=Dramalord222}Player is always 100% attractive to other heroes (cheat)", RequireRestart = false)]
        public bool PlayerAlwaysAttractive { get; set; } = false;

        [SettingPropertyGroup("{=Dramalord149}Player Options")]
        [SettingPropertyBool("{=Dramalord223}Player Always Loved", Order = 3, HintText = "{=Dramalord224}Player is always loved by other heroes (cheat)", RequireRestart = false)]
        public bool PlayerAlwaysLoved { get; set; } = false;

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

        public override string Id => DramalordSubModule.ModuleName;

        public override string DisplayName => DramalordSubModule.ModuleName;

        public override string FolderName => DramalordSubModule.ModuleFolder;
    }
}
