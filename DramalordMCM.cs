using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.PerCampaign;
using System.Diagnostics.CodeAnalysis;

namespace Dramalord
{
    internal sealed class DramalordMCM : AttributePerCampaignSettings<DramalordMCM>
    {
        [AllowNull]
        internal static DramalordMCM Get => PerCampaignSettings<DramalordMCM>.Instance;

        [SettingPropertyGroup("General")]
        [SettingPropertyBool("Debug Output", HintText = "Print Debug info (lots of spam)", Order = 1, RequireRestart = false)]
        public bool DebugOutput { get; set; } = false;

        [SettingPropertyGroup("General")]
        [SettingPropertyBool("Minor Info Output", HintText = "Print not so important info (like affairs)", Order = 2, RequireRestart = false)]
        public bool MinorOutput { get; set; } = false;

        [SettingPropertyGroup("General")]
        [SettingPropertyFloatingInteger("Trait-Score Multiplyer", 1, 10, HintText = "Trait score multiplier for AI behavior", Order = 4, RequireRestart = false)]
        public int TraitScoreMultiplyer { get; set; } = 1;

        [SettingPropertyGroup("General")]
        [SettingPropertyBool("Protect Family", HintText = "No AI interaction with family members", Order = 5, RequireRestart = false)]
        public bool ProtectFamily { get; set; } = true;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Other Sex Attraction Modifier", -100, 100, Order = 1, HintText = "AI attraction modifier the other sex (negative = own sex, positive = opposite sex, 0 = neutral)", RequireRestart = false)]
        public int OtherSexAttractionModifier { get; set; } = 0;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Min. Attraction For Flirting", 0, 100, Order = 2, HintText = "AI minimum attraction to consider flirting with a hero if hero is not pretty enough", RequireRestart = false)]
        public int MinAttractionForFlirting { get; set; } = 50;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Flirts Per Day", 1, 100, Order = 3, HintText = "With how many other heroes AI heroes flirt per day", RequireRestart = false)]
        public int FlirtsPerDay { get; set; } = 3;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Min. Emotion For Discreet Meetings", 0, 100, Order = 4, HintText = "Minimum emotion for AI to consider meeting someone discreetly (dating)", RequireRestart = false)]
        public int MinEmotionForDating { get; set; } = 30;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Days Between Discreet Meetings", 0, 100, Order = 5, HintText = "Days a hero will wait before having a discreet meeting with the same person again", RequireRestart = false)]
        public int DaysBetweenDates { get; set; } = 2;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Min. Emotion For Marriage", 0, 100, Order = 6, HintText = "Minimum emotion for AI to consider marriage", RequireRestart = false)]
        public int MinEmotionForMarriage { get; set; } = 80;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Min. Emotion Before Divorce", 0, 100, Order = 7, HintText = "Minimum emotion for AI to consider divorce", RequireRestart = false)]
        public int MinEmotionBeforeDivorce { get; set; } = 10;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Min. Horny For Intercouse", 0, 100, Order = 8, HintText = "Minimum horniness of AI for intercourse interest", RequireRestart = false)]
        public int MinHornyForIntercouse { get; set; } = 50;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Hornyness Loss Intercourse", 0, 100, Order = 9, HintText = "Value of horniness lost by intercourse", RequireRestart = false)]
        public int HornyLossIntercourse { get; set; } = 50;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Days Apart Start Forget", 0, 100, Order = 10, HintText = "Days AI has no conversation with other heroes before forgetting emotional value", RequireRestart = false)]
        public int DaysApartStartForget { get; set; } = 21;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Amount Emotion Forget", 0, 100, Order = 10, HintText = "Emotional value AI forgets about others after passing the days apart border", RequireRestart = false)]
        public int AmountEmotionForget { get; set; } = 1;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Days Apart Completely Forgotten", 0, 100, Order = 10, HintText = "Days AI has no conversation with other heroes before forgetting them completely", RequireRestart = false)]
        public int DaysApartCompleteForget { get; set; } = 84;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Chance Getting Caught", 0, 100, Order = 11, HintText = "Chance of getting caught by by partners while interacting wth other heroes", RequireRestart = false)]
        public int ChanceGettingCaught { get; set; } = 10;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Caught Flirting", 0, 100, Order = 12, HintText = "Loss of emotion of partners when getting caught flirting", RequireRestart = false)]
        public int EmotionalLossCaughtFlirting { get; set; } = 5;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Caught Meeting", 0, 100, Order = 13, HintText = "Loss of emotion of partners when getting caught meeting someone discreetly", RequireRestart = false)]
        public int EmotionalLossCaughtDate { get; set; } = 20;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Caught Intercourse", 0, 100, Order = 14, HintText = "Loss of emotion of partners when getting caught during intercourse with others", RequireRestart = false)]
        public int EmotionalLossCaughtIntercourse { get; set; } = 50;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Marry Other", 0, 100, Order = 15, HintText = "Loss of emotion of partners if hero of interest marries someone else", RequireRestart = false)]
        public int EmotionalLossMarryOther { get; set; } = 60;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Breakup", 0, 100, Order = 16, HintText = "Loss of emotion of partners if hero breaks up the relationship (discreet meetings)", RequireRestart = false)]
        public int EmotionalLossBreakup { get; set; } = 50;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Divorce", 0, 100, Order = 17, HintText = "Loss of emotion of partners if heros divorce", RequireRestart = false)]
        public int EmotionalLossDivorce { get; set; } = 80;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Pregnancy", 0, 100, Order = 14, HintText = "Loss of emotion of partners if heros are visibly pregnant from someone else", RequireRestart = false)]
        public int EmotionalLossPregnancy { get; set; } = 80;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Loss Bastard", 0, 100, Order = 15, HintText = "Loss of emotion of partners if heros give birth to children of someone else", RequireRestart = false)]
        public int EmotionalLossBastard { get; set; } = 100;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Win Affair", 0, 100, Order = 16, HintText = "Win of emotion of partners if heroes enter an affair", RequireRestart = false)]
        public int EmotionalWinAffair { get; set; } = 20;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Emotional Win Marriage", 0, 100, Order = 17, HintText = "Win of emotion of partners if heroes are married", RequireRestart = false)]
        public int EmotionalWinMarriage { get; set; } = 30;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Pregnancy Chance", 0, 100, Order = 18, HintText = "Chance of getting pregnant in percent", RequireRestart = false)]
        public int PregnancyChance { get; set; } = 10;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Pregnancy Duration", 1, 100, Order = 19, HintText = "How many days a hero is pregnant before giving birth", RequireRestart = false)]
        public int PregnancyDuration { get; set; } = 21;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Days Until Pregnancy Visible", 0, 100, Order = 20, HintText = "How many days a hero is pregnant before AI will notice it", RequireRestart = false)]
        public int DaysUntilPregnancyVisible { get; set; } = 15;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Wait Between Adopting", 1, 100, Order = 21, HintText = "How many days to wait between adptions (same sex or old couples)", RequireRestart = false)]
        public int WaitBetweenAdopting { get; set; } = 84;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Max. Fertility Age", 18, 130, Order = 22, HintText = "Max age of being fertile", RequireRestart = false)]
        public int MaxFertilityAge { get; set; } = 45;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Period Duration", 1, 21, Order = 23, HintText = "Duration of period in days per season (can't get pregnant)", RequireRestart = false)]
        public int PeriodDuration { get; set; } = 5;

        [SettingPropertyGroup("Hero Setup")]
        [SettingPropertyFloatingInteger("Toy Break Chance", 0, 100, Order = 24, HintText = "Chance a toy will break when being used", RequireRestart = false)]
        public int ToyBreakChance { get; set; } = 5;

        [SettingPropertyGroup("Player Options")]
        [SettingPropertyFloatingInteger("Min. Emotion For Conversation", 0, 100, Order = 1, HintText = "Minimum emotion value of AI to accept intimate talks with the Player", RequireRestart = false)]
        public int MinEmotionForConversation { get; set; } = 10;

        [SettingPropertyGroup("Player Options")]
        [SettingPropertyBool("Player Always Attractive", Order = 2, HintText = "Player always 100% attractive (cheat)", RequireRestart = false)]
        public bool PlayerAlwaysAttractive { get; set; } = false;

        [SettingPropertyGroup("Player Options")]
        [SettingPropertyBool("Player Always Loved", Order = 3, HintText = "NPCs are always in love with player (cheat)", RequireRestart = false)]
        public bool PlayerAlwaysLoved { get; set; } = false;

        [SettingPropertyGroup("Extra QOL")]
        [SettingPropertyBool("No Captivity Messages", Order = 1, HintText = "Disable spam of captured or freed heroes (english only!)", RequireRestart = false)]
        public bool NoCaptivityMessages { get; set; } = true;

        [SettingPropertyGroup("Extra QOL")]
        [SettingPropertyBool("No Relation Notification", Order = 2, HintText = "Disable spam of relation increase with notables", RequireRestart = false)]
        public bool NoRelationNotification { get; set; } = true;

        public override string Id => DramalordSubModule.ModuleName;

        public override string DisplayName => DramalordSubModule.ModuleName;

        public override string FolderName => DramalordSubModule.ModuleFolder;
    }
}
