using Dramalord.Extensions;
using Dramalord.Notifications;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class ConversationPersuasions
    {
        internal static bool Success = false;
        internal static PersuasionTask CurrentTask = new PersuasionTask(3);

        private static TextObject Date = new TextObject("date");
        private static TextObject Engage = new TextObject("engage");
        private static TextObject FWB = new TextObject("fwb");
        private static TextObject Break = new TextObject("break");

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_persuasion_challenge", "npc_persuasion_challenge", "player_persuasion_argument", "{PERSUADE_CHALLENGE}", ConditionPersuasionChallenge, null);

            starter.AddPlayerLine("player_persuasion_argument_1", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_1}", ConditionPersuasionLine1, ConsequencePersuasionLine1, 100, persuasionOptionDelegate: SetupOption1);
            starter.AddPlayerLine("player_persuasion_argument_2", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_2}", ConditionPersuasionLine2, ConsequencePersuasionLine2, 100, persuasionOptionDelegate: SetupOption2);
            starter.AddPlayerLine("player_persuasion_argument_3", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_3}", ConditionPersuasionLine3, ConsequencePersuasionLine3, 100, persuasionOptionDelegate: SetupOption3);
            starter.AddPlayerLine("player_persuasion_argument_4", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_4}", ConditionPersuasionLine4, ConsequencePersuasionLine4, 100, persuasionOptionDelegate: SetupOption4);
            starter.AddPlayerLine("player_persuasion_abort", "player_persuasion_argument", "npc_persuasion_reaction_abort", "{=Dramalord255}Nevermind.", null, ConsequencePersuasionAbort);

            starter.AddDialogLine("npc_persuasion_reaction_date", "npc_persuasion_reaction", "npc_interaction_reply_date", "{=0UPds9x3}Very well, then...", ConditionPeruasionDateSuccess, ConsequencePeruasionSuccess);
            starter.AddDialogLine("npc_persuasion_reaction_date1", "npc_persuasion_reaction", "player_interaction_selection", "{=0UPds9x3}Very well, then...", ConditionPeruasionDateFail, ConsequencePeruasionFail);
            starter.AddDialogLine("npc_persuasion_reaction_engage", "npc_persuasion_reaction", "npc_interaction_reply_engage", "{=0UPds9x3}Very well, then...", ConditionPeruasionEngageSuccess, ConsequencePeruasionSuccess);
            starter.AddDialogLine("npc_persuasion_reaction_engage", "npc_persuasion_reaction", "player_interaction_selection", "{=0UPds9x3}Very well, then...", ConditionPeruasionEngageFail, ConsequencePeruasionFail);
            starter.AddDialogLine("npc_persuasion_reaction_sex", "npc_persuasion_reaction", "npc_interaction_reply_sex", "{=0UPds9x3}Very well, then...", ConditionPeruasionFWBSuccess, ConsequencePeruasionSuccess);
            starter.AddDialogLine("npc_persuasion_reaction_sex", "npc_persuasion_reaction", "player_interaction_selection", "{=0UPds9x3}Very well, then...", ConditionPeruasionFWBFail, ConsequencePeruasionFail);
            starter.AddDialogLine("npc_persuasion_reaction_break", "npc_persuasion_reaction", "resolve_breakup", "{=0UPds9x3}Very well, then...", ConditionPeruasionBreakupSuccess, ConsequencePeruasionSuccess);
            starter.AddDialogLine("npc_persuasion_reaction_break", "npc_persuasion_reaction", "resolve_breakup", "{=0UPds9x3}Very well, then...", ConditionPeruasionBreakupFail, ConsequencePeruasionFail);
            starter.AddDialogLine("npc_persuasion_reaction_abort", "npc_persuasion_reaction_abort", "player_interaction_selection", "{=0UPds9x3}Very well, then...", null, null);
        }

        private static bool ConditionPersuasionChallenge()
        {
            ConversationManager.StartPersuasion(1, 1, -1, 1, -1, 0, PersuasionDifficulty.Medium);
            MBTextManager.SetTextVariable("PERSUADE_CHALLENGE", CurrentTask.SpokenLine);
            return true;
        }

        private static bool ConditionPersuasionLine(int index)
        {
            TextObject textObject = new TextObject("{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}");
            textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(CurrentTask.Options.ElementAt(index)));
            textObject.SetTextVariable("PERSUASION_OPTION_LINE", CurrentTask.Options.ElementAt(index).Line);
            MBTextManager.SetTextVariable("PERSUADE_ATTEMPT_" + (index + 1), textObject);
            return true;
        }

        private static bool ConditionPersuasionLine1() => ConditionPersuasionLine(0);
        private static bool ConditionPersuasionLine2() => ConditionPersuasionLine(1);
        private static bool ConditionPersuasionLine3() => ConditionPersuasionLine(2);
        private static bool ConditionPersuasionLine4() => ConditionPersuasionLine(3);

        private static void ConsequencePersuasionLine1() => ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(0));
        private static void ConsequencePersuasionLine2() => ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(1));
        private static void ConsequencePersuasionLine3() => ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(2));
        private static void ConsequencePersuasionLine4() => ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(3));

        internal static PersuasionOptionArgs SetupOption1() => CurrentTask.Options.ElementAt(0);
        internal static PersuasionOptionArgs SetupOption2() => CurrentTask.Options.ElementAt(1);
        internal static PersuasionOptionArgs SetupOption3() => CurrentTask.Options.ElementAt(2);
        internal static PersuasionOptionArgs SetupOption4() => CurrentTask.Options.ElementAt(3);

        private static bool ConditionPeruasionDateSuccess() => CurrentTask.TryLaterLine == Date && ConversationManager.GetPersuasionProgressSatisfied();
        private static bool ConditionPeruasionDateFail() => CurrentTask.TryLaterLine == Date && !ConversationManager.GetPersuasionProgressSatisfied();
        private static bool ConditionPeruasionEngageSuccess() => CurrentTask.TryLaterLine == Engage && ConversationManager.GetPersuasionProgressSatisfied();
        private static bool ConditionPeruasionEngageFail() => CurrentTask.TryLaterLine == Engage && !ConversationManager.GetPersuasionProgressSatisfied();
        private static bool ConditionPeruasionFWBSuccess() => CurrentTask.TryLaterLine == FWB && ConversationManager.GetPersuasionProgressSatisfied();
        private static bool ConditionPeruasionFWBFail() => CurrentTask.TryLaterLine == FWB && !ConversationManager.GetPersuasionProgressSatisfied();
        private static bool ConditionPeruasionBreakupSuccess() => CurrentTask.TryLaterLine == Break && ConversationManager.GetPersuasionProgressSatisfied();
        private static bool ConditionPeruasionBreakupFail() => CurrentTask.TryLaterLine == Break && !ConversationManager.GetPersuasionProgressSatisfied();

        private static void ConsequencePersuasionAbort()
        {
            ConversationManager.EndPersuasion();
        }

        private static void ConsequencePeruasionSuccess()
        {
            Success = true;
            DramalordBanner.CreateBanner(Hero.OneToOneConversationHero, DramalordTexts.BANNER_INFO_PERSIUASION_SUCCESS, true);  
            ConversationManager.EndPersuasion();
        }

        private static void ConsequencePeruasionFail()
        {
            DramalordBanner.CreateBanner(Hero.OneToOneConversationHero, DramalordTexts.BANNER_INFO_PERSIUASION_FAIL, true);
            ConversationManager.EndPersuasion();
        }

        internal static void CreatePersuasionTaskForDate()
        {
            Success = false;
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject(DramalordTexts.PERSUASION_DATE_Q);
            persuasionTask.TryLaterLine = Date;
            int loveDiff = MBMath.ClampInt(((DramalordMCM.Instance.MinDatingLove - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love) * -1) / 10, -3, 3);
            PersuasionArgumentStrength persuasionArgumentStrength = (PersuasionArgumentStrength)loveDiff;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_DATE_1), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_DATE_2), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Steward, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_DATE_3), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_DATE_4), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
        }

        internal static void CreatePersuasionTaskForEngage()
        {
            Success = false;
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject(DramalordTexts.PERSUASION_MARRIAGE_Q);
            persuasionTask.TryLaterLine = Engage;
            int loveDiff = MBMath.ClampInt(((DramalordMCM.Instance.MinMarriageLove - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love) * -1) / 10, -3, 3);
            PersuasionArgumentStrength persuasionArgumentStrength = (PersuasionArgumentStrength)loveDiff;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_MARRIAGE_1), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Steward, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_MARRIAGE_2), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_MARRIAGE_3), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Tactics, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_MARRIAGE_4), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
        }

        internal static void CreatePersuasionTaskForFWB()
        {
            Success = false;
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject(DramalordTexts.PERSUASION_SEX_Q);
            persuasionTask.TryLaterLine = FWB;
            int trustDiff = MBMath.ClampInt(((100 - Hero.OneToOneConversationHero.GetTrust(Hero.MainHero)) * -1) / 10, -3, 3);
            PersuasionArgumentStrength persuasionArgumentStrength = (PersuasionArgumentStrength)trustDiff;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_SEX_1), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_SEX_2), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_SEX_3), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.OneHanded, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_SEX_4), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
        }

        internal static void CreatePersuasionTaskForBreakUp()
        {
            Success = false;
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject(DramalordTexts.PERSUASION_BREAKUP_Q);
            persuasionTask.TryLaterLine = Break;

            int trustDiff = MBMath.ClampInt(((100 - Hero.OneToOneConversationHero.GetTrust(Hero.MainHero)) * -1) / 10, -3, 3);
            PersuasionArgumentStrength persuasionArgumentStrength = (PersuasionArgumentStrength)trustDiff;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Trade, DefaultTraits.Generosity, TraitEffect.Negative, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_BREAKUP_1), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_BREAKUP_2), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_BREAKUP_3), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject(DramalordTexts.PERSUASION_BREAKUP_4), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
        }
    }
}
