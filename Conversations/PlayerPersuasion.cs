using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.GameComponents;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem.Conversation;
using System.Reflection;
using TaleWorlds.Library;

namespace Dramalord.Conversations
{
    /*
    
            private void ExecuteOpenConversation()
        {
            if (CurrentCharacter.Side == PartyScreenLogic.PartyRosterSide.Right && CurrentCharacter.Character != CharacterObject.PlayerCharacter)
            {
                if (Settlement.CurrentSettlement == null)
                {
                    CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty), new ConversationCharacterData(CurrentCharacter.Character, PartyBase.MainParty, noHorse: false, noWeapon: false, spawnAfterFight: false, CurrentCharacter.IsPrisoner));
                }
                else
                {
                    PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationOfCharacter(LocationComplex.Current.GetFirstLocationCharacterOfCharacter(CurrentCharacter.Character)), null, CurrentCharacter.Character);
                }

                IsInConversation = true;
            }
        }
    */
    internal static class Persuasions
    {
        internal static bool Success = false;
        internal static PersuasionTask CurrentTask = new PersuasionTask(3);

        private static TextObject Date = new TextObject("date");
        private static TextObject Engage = new TextObject("engage");
        private static TextObject FWB = new TextObject("fwb");

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_persuasion_challenge", "npc_persuasion_challenge", "player_persuasion_argument", "{PERSUADE_CHALLENGE}", ConditionPersuasionChallenge, null);

            starter.AddPlayerLine("player_persuasion_argument_1", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_1}", ConditionPersuasionLine1, ConsequencePersuasionLine1, 100, persuasionOptionDelegate: SetupOption1);
            starter.AddPlayerLine("player_persuasion_argument_2", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_2}", ConditionPersuasionLine2, ConsequencePersuasionLine2, 100, persuasionOptionDelegate: SetupOption2);
            starter.AddPlayerLine("player_persuasion_argument_3", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_3}", ConditionPersuasionLine3, ConsequencePersuasionLine3, 100, persuasionOptionDelegate: SetupOption3);
            starter.AddPlayerLine("player_persuasion_argument_4", "player_persuasion_argument", "npc_persuasion_reaction", "{=!}{PERSUADE_ATTEMPT_4}", ConditionPersuasionLine4, ConsequencePersuasionLine4, 100, persuasionOptionDelegate: SetupOption4);
            starter.AddPlayerLine("player_persuasion_abort", "player_persuasion_argument", "npc_persuasion_reaction_abort", "{=Dramalord255}Nevermind.", null, ConsequencePersuasionAbort);

            starter.AddDialogLine("npc_persuasion_reaction_date", "npc_persuasion_reaction", "npc_interaction_reply_date", "{=0UPds9x3}Very well, then...", ConditionPeruasionDate, ConsequencePeruasionDate);
            starter.AddDialogLine("npc_persuasion_reaction_engage", "npc_persuasion_reaction", "npc_interaction_reply_engage", "{=0UPds9x3}Very well, then...", ConditionPeruasionEngage, ConsequencePeruasionEngage);
            starter.AddDialogLine("npc_persuasion_reaction_sex", "npc_persuasion_reaction", "npc_interaction_reply_sex", "{=0UPds9x3}Very well, then...", ConditionPeruasionFWB, ConsequencePeruasionFWB);
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
            MBTextManager.SetTextVariable("PERSUADE_ATTEMPT_" + (index+1), textObject);
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

        private static bool ConditionPeruasionDate() => CurrentTask.TryLaterLine == Date;
        private static bool ConditionPeruasionEngage() => CurrentTask.TryLaterLine == Engage; 
        private static bool ConditionPeruasionFWB() => CurrentTask.TryLaterLine == FWB;

        private static void ConsequencePersuasionAbort()
        {
            ConversationManager.EndPersuasion();
        }

        private static void ConsequencePeruasionDate()
        {
            if (ConversationManager.GetPersuasionProgressSatisfied())
            {
                TextObject banner = new TextObject("{=Dramalord439}You successfully convinced {HERO.LINK}. (Love {LOVE}, Trust {TRUST})");
                StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                int loveGain = DramalordMCM.Instance.MinDatingLove - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove;
                MBTextManager.SetTextVariable("LOVE", ConversationHelper.FormatNumber(loveGain));
                MBTextManager.SetTextVariable("TRUST", ConversationHelper.FormatNumber(0));
                MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love = DramalordMCM.Instance.MinDatingLove;
            }
            else
            {
                TextObject banner = new TextObject("{=Dramalord440}You failed to convince {HERO.LINK}. (Love {LOVE}, Trust {TRUST})");
                StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                int loveGain = Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove * -1;
                MBTextManager.SetTextVariable("LOVE", ConversationHelper.FormatNumber(loveGain));
                MBTextManager.SetTextVariable("TRUST", ConversationHelper.FormatNumber(0));
                MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love = 0;
            }
            ConversationManager.EndPersuasion();
        }

        private static void ConsequencePeruasionEngage()
        {
            if (ConversationManager.GetPersuasionProgressSatisfied())
            {
                TextObject banner = new TextObject("{=Dramalord439}You successfully convinced {HERO.LINK}. (Love {LOVE}, Trust {TRUST})");
                StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                int loveGain = DramalordMCM.Instance.MinMarriageLove - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove;
                MBTextManager.SetTextVariable("LOVE", ConversationHelper.FormatNumber(loveGain));
                MBTextManager.SetTextVariable("TRUST", ConversationHelper.FormatNumber(0));
                MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love = DramalordMCM.Instance.MinMarriageLove;
            }
            else
            {
                TextObject banner = new TextObject("{=Dramalord440}You failed to convince {HERO.LINK}. (Love {LOVE}, Trust {TRUST})");
                StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                int loveGain = 24 - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove;
                MBTextManager.SetTextVariable("LOVE", ConversationHelper.FormatNumber(loveGain));
                MBTextManager.SetTextVariable("TRUST", ConversationHelper.FormatNumber(0));
                MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love = 24;
            }
            ConversationManager.EndPersuasion();
        }

        private static void ConsequencePeruasionFWB()
        {
            if (ConversationManager.GetPersuasionProgressSatisfied())
            {
                TextObject banner = new TextObject("{=Dramalord439}You successfully convinced {HERO.LINK}. (Love {LOVE}, Trust {TRUST})");
                StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                int trustGain = 75 - Hero.OneToOneConversationHero.GetTrust(Hero.MainHero);
                MBTextManager.SetTextVariable("LOVE", ConversationHelper.FormatNumber(0));
                MBTextManager.SetTextVariable("TRUST", ConversationHelper.FormatNumber(trustGain));
                MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                Hero.OneToOneConversationHero.GetDesires().Horny = 75;
                Hero.OneToOneConversationHero.SetTrust(Hero.MainHero, 75);
            }
            else
            {
                TextObject banner = new TextObject("{=Dramalord440}You failed to convince {HERO.LINK}. (Love {LOVE}, Trust {TRUST})");
                StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                int trustGain = 24 - Hero.OneToOneConversationHero.GetTrust(Hero.MainHero);
                MBTextManager.SetTextVariable("LOVE", ConversationHelper.FormatNumber(0));
                MBTextManager.SetTextVariable("TRUST", ConversationHelper.FormatNumber(trustGain));
                MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                Hero.OneToOneConversationHero.GetDesires().Horny = 24;
                Hero.OneToOneConversationHero.SetTrust(Hero.MainHero, 24);
            }
            ConversationManager.EndPersuasion();
        }

        internal static void CreatePersuasionTaskForDate()
        {
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject("{=Dramalord423}I... I am not sure about this. I like you, but do I like you enough?");
            persuasionTask.TryLaterLine = Date;
            int loveDiff = MBMath.ClampInt(((DramalordMCM.Instance.MinDatingLove - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove) * -1) / 10, -3, 3);
            PersuasionArgumentStrength persuasionArgumentStrength = (PersuasionArgumentStrength)loveDiff;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord424}If we don't try we never will find out if this could work."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord425}You think too much. Just let it happen."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Steward, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord426}I am convinced we can give each other much love."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord427}I will feel honored if you would consider it."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
        }

        internal static void CreatePersuasionTaskForEngage()
        {
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject("{=Dramalord428}This is overwhelming. I love you, {PLAYER.NAME}, but are we ready for this step?");
            persuasionTask.TryLaterLine = Engage;
            int loveDiff = MBMath.ClampInt(((DramalordMCM.Instance.MinMarriageLove - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove) * -1) / 10, -3, 3);
            PersuasionArgumentStrength persuasionArgumentStrength = (PersuasionArgumentStrength)loveDiff;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord429}Love is an adventure and this will be our next challenge!"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Steward, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord430}If you think about it, this is logically the next step."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord431}We finally could share everything with each other."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Tactics, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord432}It's a honorful thing to do in our situation."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
        }

        internal static void CreatePersuasionTaskForFWB()
        {
            PersuasionTask persuasionTask = new PersuasionTask(3);
            persuasionTask.SpokenLine = new TextObject("{=Dramalord433}This is a spicy proposal indeed. What if feelings are mixing in?");
            persuasionTask.TryLaterLine = FWB;
            int trustDiff = MBMath.ClampInt(((DramalordMCM.Instance.MinTrustFWB - Hero.OneToOneConversationHero.GetTrust(Hero.MainHero)) * -1) / 10, -3, 3);
            PersuasionArgumentStrength persuasionArgumentStrength = (PersuasionArgumentStrength)trustDiff;

            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord434}What is a steamy adventure without risks anyway?"), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord435}Then we re-evaluate the situation and draw our conclusions."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord436}Think about all the pleasure we could give each other."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            persuasionTask.AddOptionToTask(new PersuasionOptionArgs(DefaultSkills.OneHanded, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, givesCriticalSuccess: false, new TextObject("{=Dramalord437}Then we release each other and go back to manual work."), null, canBlockOtherOption: false, canMoveToTheNextReservation: true));
            CurrentTask = persuasionTask;
        }
    }
}
