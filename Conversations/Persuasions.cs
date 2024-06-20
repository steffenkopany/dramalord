using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class Persuasions
    {
        internal static bool Success = false;
        internal static PersuasionTask CurrentTask = new PersuasionTask(3);

        internal static readonly string PlayerDateArgument = "dl_player_date_argument";
        internal static readonly string PlayerJoinArgument = "dl_player_join_argument";
        internal static readonly string PlayerDivorceArgument = "dl_player_divorce_argument";
        internal static readonly string PlayerMarryArgument = "dl_player_marry_argument";
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("date_argument_1", PlayerDateArgument, "npc_replies_to_date", "{=!}{PERSUADE_ATTEMPT_1}", PlayerOption1, PlayerConsequence1, 100, persuasionOptionDelegate: SetupOption1, clickableConditionDelegate: ClickableOnCondition1);
            starter.AddPlayerLine("date_argument_2", PlayerDateArgument, "npc_replies_to_date", "{=!}{PERSUADE_ATTEMPT_2}", PlayerOption2, PlayerConsequence2, 100, persuasionOptionDelegate: SetupOption2, clickableConditionDelegate: ClickableOnCondition2);
            starter.AddPlayerLine("date_argument_3", PlayerDateArgument, "npc_replies_to_date", "{=!}{PERSUADE_ATTEMPT_3}", PlayerOption3, PlayerConsequence3, 100, persuasionOptionDelegate: SetupOption3, clickableConditionDelegate: ClickableOnCondition3);
            starter.AddPlayerLine("date_argument_4", PlayerDateArgument, "npc_replies_to_date", "{=!}{PERSUADE_ATTEMPT_4}", PlayerOption4, PlayerConsequence4, 100, persuasionOptionDelegate: SetupOption4, clickableConditionDelegate: ClickableOnCondition4);
            starter.AddPlayerLine("date_argument_5", PlayerDateArgument, "npc_replies_to_date", "{=!}{PERSUADE_ATTEMPT_5}", PlayerOption5, PlayerConsequence5, 100, persuasionOptionDelegate: SetupOption5, clickableConditionDelegate: ClickableOnCondition5);

            starter.AddPlayerLine("divorce_argument_1", PlayerDivorceArgument, "npc_replies_to_divorcing_husband", "{=!}{PERSUADE_ATTEMPT_1}", PlayerOption1, PlayerConsequence1, 100, persuasionOptionDelegate: SetupOption1, clickableConditionDelegate: ClickableOnCondition1);
            starter.AddPlayerLine("divorce_argument_2", PlayerDivorceArgument, "npc_replies_to_divorcing_husband", "{=!}{PERSUADE_ATTEMPT_2}", PlayerOption2, PlayerConsequence2, 100, persuasionOptionDelegate: SetupOption2, clickableConditionDelegate: ClickableOnCondition2);
            starter.AddPlayerLine("divorce_argument_3", PlayerDivorceArgument, "npc_replies_to_divorcing_husband", "{=!}{PERSUADE_ATTEMPT_3}", PlayerOption3, PlayerConsequence3, 100, persuasionOptionDelegate: SetupOption3, clickableConditionDelegate: ClickableOnCondition3);
            starter.AddPlayerLine("divorce_argument_4", PlayerDivorceArgument, "npc_replies_to_divorcing_husband", "{=!}{PERSUADE_ATTEMPT_4}", PlayerOption4, PlayerConsequence4, 100, persuasionOptionDelegate: SetupOption4, clickableConditionDelegate: ClickableOnCondition4);
            starter.AddPlayerLine("divorce_argument_5", PlayerDivorceArgument, "npc_replies_to_divorcing_husband", "{=!}{PERSUADE_ATTEMPT_5}", PlayerOption5, PlayerConsequence5, 100, persuasionOptionDelegate: SetupOption5, clickableConditionDelegate: ClickableOnCondition5);

            starter.AddPlayerLine("marry_argument_1", PlayerMarryArgument, "npc_replies_to_marriage", "{=!}{PERSUADE_ATTEMPT_1}", PlayerOption1, PlayerConsequence1, 100, persuasionOptionDelegate: SetupOption1, clickableConditionDelegate: ClickableOnCondition1);
            starter.AddPlayerLine("marry_argument_2", PlayerMarryArgument, "npc_replies_to_marriage", "{=!}{PERSUADE_ATTEMPT_2}", PlayerOption2, PlayerConsequence2, 100, persuasionOptionDelegate: SetupOption2, clickableConditionDelegate: ClickableOnCondition2);
            starter.AddPlayerLine("marry_argument_3", PlayerMarryArgument, "npc_replies_to_marriage", "{=!}{PERSUADE_ATTEMPT_3}", PlayerOption3, PlayerConsequence3, 100, persuasionOptionDelegate: SetupOption3, clickableConditionDelegate: ClickableOnCondition3);
            starter.AddPlayerLine("marry_argument_4", PlayerMarryArgument, "npc_replies_to_marriage", "{=!}{PERSUADE_ATTEMPT_4}", PlayerOption4, PlayerConsequence4, 100, persuasionOptionDelegate: SetupOption4, clickableConditionDelegate: ClickableOnCondition4);
            starter.AddPlayerLine("marry_argument_5", PlayerMarryArgument, "npc_replies_to_marriage", "{=!}{PERSUADE_ATTEMPT_5}", PlayerOption5, PlayerConsequence5, 100, persuasionOptionDelegate: SetupOption5, clickableConditionDelegate: ClickableOnCondition5);
        }

        internal static bool PlayerOption1()
        {
            TextObject textObject = new TextObject("{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}");
            textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(CurrentTask.Options.ElementAt(0)));
            textObject.SetTextVariable("PERSUASION_OPTION_LINE", CurrentTask.Options.ElementAt(0).Line);
            MBTextManager.SetTextVariable("PERSUADE_ATTEMPT_1", textObject);
            return true;
        }

        internal static bool PlayerOption2()
        {
            TextObject textObject = new TextObject("{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}");
            textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(CurrentTask.Options.ElementAt(1)));
            textObject.SetTextVariable("PERSUASION_OPTION_LINE", CurrentTask.Options.ElementAt(1).Line);
            MBTextManager.SetTextVariable("PERSUADE_ATTEMPT_2", textObject);
            return true;
        }

        internal static bool PlayerOption3()
        {
            TextObject textObject = new TextObject("{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}");
            textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(CurrentTask.Options.ElementAt(2)));
            textObject.SetTextVariable("PERSUASION_OPTION_LINE", CurrentTask.Options.ElementAt(2).Line);
            MBTextManager.SetTextVariable("PERSUADE_ATTEMPT_3", textObject);
            return true;
        }

        internal static bool PlayerOption4()
        {
            TextObject textObject = new TextObject("{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}");
            textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(CurrentTask.Options.ElementAt(3)));
            textObject.SetTextVariable("PERSUASION_OPTION_LINE", CurrentTask.Options.ElementAt(3).Line);
            MBTextManager.SetTextVariable("PERSUADE_ATTEMPT_4", textObject);
            return true;
        }

        internal static bool PlayerOption5()
        {
            TextObject textObject = new TextObject("{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}");
            textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(CurrentTask.Options.ElementAt(4)));
            textObject.SetTextVariable("PERSUASION_OPTION_LINE", CurrentTask.Options.ElementAt(4).Line);
            MBTextManager.SetTextVariable("PERSUADE_ATTEMPT_5", textObject);
            return true;
        }

        internal static void PlayerConsequence1()
        {
            CurrentTask.Options.ElementAt(0).BlockTheOption(true);
            ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(0));
        }

        internal static void PlayerConsequence2()
        {
            CurrentTask.Options.ElementAt(1).BlockTheOption(true);
            ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(1));
        }

        internal static void PlayerConsequence3()
        {
            CurrentTask.Options.ElementAt(2).BlockTheOption(true);
            ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(2));
        }
        internal static void PlayerConsequence4()
        {
            CurrentTask.Options.ElementAt(3).BlockTheOption(true);
            ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(3));
        }

        internal static void PlayerConsequence5()
        {
            CurrentTask.Options.ElementAt(4).BlockTheOption(true);
            ConversationManager.PersuasionCommitProgress(CurrentTask.Options.ElementAt(4));
        }

        internal static PersuasionOptionArgs SetupOption1()
        {
            return CurrentTask.Options.ElementAt(0);
        }
        internal static PersuasionOptionArgs SetupOption2()
        {
            return CurrentTask.Options.ElementAt(1);
        }
        internal static PersuasionOptionArgs SetupOption3()
        {
            return CurrentTask.Options.ElementAt(2);
        }
        internal static PersuasionOptionArgs SetupOption4()
        {
            return CurrentTask.Options.ElementAt(3);
        }
        internal static PersuasionOptionArgs SetupOption5()
        {
            return CurrentTask.Options.ElementAt(4);
        }

        internal static bool ClickableOnCondition1(out TextObject hintText)
        {
            hintText = new TextObject();
            if (CurrentTask.Options.Count > 0)
            {
                return !CurrentTask.Options.ElementAt(0).IsBlocked;
            }

            hintText = new TextObject("{=9ACJsI6S}Blocked");
            return false;
        }

        internal static bool ClickableOnCondition2(out TextObject hintText)
        {
            hintText = new TextObject();
            if (CurrentTask.Options.Count > 1)
            {
                return !CurrentTask.Options.ElementAt(1).IsBlocked;
            }

            hintText = new TextObject("{=9ACJsI6S}Blocked");
            return false;
        }

        internal static bool ClickableOnCondition3(out TextObject hintText)
        {
            hintText = new TextObject();
            if (CurrentTask.Options.Count > 2)
            {
                return !CurrentTask.Options.ElementAt(2).IsBlocked;
            }

            hintText = new TextObject("{=9ACJsI6S}Blocked");
            return false;
        }

        internal static bool ClickableOnCondition4(out TextObject hintText)
        {
            hintText = new TextObject();
            if (CurrentTask.Options.Count > 3)
            {
                return !CurrentTask.Options.ElementAt(3).IsBlocked;
            }

            hintText = new TextObject("{=9ACJsI6S}Blocked");
            return false;
        }

        internal static bool ClickableOnCondition5(out TextObject hintText)
        {
            hintText = new TextObject();
            if (CurrentTask.Options.Count > 4)
            {
                return !CurrentTask.Options.ElementAt(4).IsBlocked;
            }

            hintText = new TextObject("{=9ACJsI6S}Blocked");
            return false;
        }

        internal static void CreatePersuasionTaskForDate()
        {
            ConversationManager.EndPersuasion();
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject("{=Dramalord088}Why should I spend time with you?");
            persuasionTask.FinalFailLine = new TextObject("{=Dramalord089}You are boring me.");
            persuasionTask.TryLaterLine = new TextObject("{=Dramalord055}I feel tired. Let's talk about this a other time.");
            PersuasionArgumentStrength persuasionArgumentStrength = PersuasionArgumentStrength.Normal;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord090}I have some exciting stories to tell!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord091}Wouldn't you like some entertainment?"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord092}I will buy you a drink in the local tavern."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord093}I would be honored to share some time with you."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord094}I beg you! I am so bored!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
            ConversationManager.StartPersuasion(1, 1f, 0f, 2f, 2f, 0);
        }

        internal static void CreatePersuasionTaskForJoining()
        {
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject("{=Dramalord053}Why should I ride with you?");
            persuasionTask.FinalFailLine = new TextObject("{=Dramalord054}I think I will leave my horse in the stable.");
            persuasionTask.TryLaterLine = new TextObject("{=Dramalord055}I feel tired. Let's talk about this a other time.");
            PersuasionArgumentStrength persuasionArgumentStrength = PersuasionArgumentStrength.Normal;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord056}We will ride fast as the wind!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord057}Sitting in a castle all day is boring."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord058}I will drown you with my attention!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord059}I would feel honored by your presence."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord060}The poor horses do nothing but stand all day."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
            ConversationManager.StartPersuasion(1, 1f, 0f, 2f, 2f, 0);
        }

        internal static void CreatePersuasionTaskForDivorce()
        {
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject("{=Dramalord061}Why should I give up my marriage?");
            persuasionTask.FinalFailLine = new TextObject("{=Dramalord068}I will not give up my marriage for you!");
            persuasionTask.TryLaterLine = new TextObject("{=Dramalord055}I feel tired. Let's talk about this a other time.");
            PersuasionArgumentStrength persuasionArgumentStrength = PersuasionArgumentStrength.Normal;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord056}We will ride fast as the wind!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord078}On the long this is the better choice."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord079}All your investments in this marriage are for nothing!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord080}You are being dishonored by this marriage."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord069}I can't take it seeing you caught in this marriage."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
            ConversationManager.StartPersuasion(1, 1f, 0f, 2f, 2f, 0);
        }

        internal static void CreatePersuasionTaskForMarriage()
        {
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject("{=Dramalord082}Why should I marry you");
            persuasionTask.FinalFailLine = new TextObject("{=Dramalord081}I will not marry someone like you");
            persuasionTask.TryLaterLine = new TextObject("{=Dramalord055}I feel tired. Let's talk about this a other time.");
            PersuasionArgumentStrength persuasionArgumentStrength = PersuasionArgumentStrength.Normal;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord083}Together we will experience new adventures!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord084}Imagine how we can build our future together."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord085}We would share everything with each other."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord086}It's a honorful thing to do in our situation."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord087}I can't live without you on my side!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
            ConversationManager.StartPersuasion(1, 1f, 0f, 2f, 2f, 0);
        }

        internal static void ClearCurrentPersuasion()
        {
            ConversationManager.EndPersuasion();
        }
    }
}
