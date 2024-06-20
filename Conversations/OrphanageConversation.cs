using Dramalord.Actions;
using Dramalord.Data;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class OrphanageConversation
    {
        private static Hero? SelectedOrphan = null;

        internal static void start(Settlement settlement)
        {
            if(DramalordOrphanage.OrphanManager == null || DramalordOrphanage.OrphanManager.HeroObject.IsDead || DramalordOrphanage.OrphanManager.HeroObject.IsDisabled)
            {
                //DramalordOrphanage.OrphanManager = HeroCreator.CreateSpecialHero(Hero.MainHero.Mother.CharacterObject, settlement, null, null, 40).CharacterObject;
                DramalordOrphanage.OrphanManager = Hero.MainHero.Mother.CharacterObject;
                DramalordOrphanage.OrphanManager.HeroObject.SetHasMet();
            }
            Campaign.Current.SetTimeSpeed(0);
            ConversationHelper.ConversationIntention = ConversationType.OrphanConversation;
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(DramalordOrphanage.OrphanManager, isCivilianEquipmentRequiredForLeader: true));
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("orphanage_greeting", "start", "orphanage_player_start", "{=Dramalord410}Good day, {TITLE}. How can I help you?", ConditionIsOrphanCharacter, null, 100);

            starter.AddPlayerLine("orphanage_player_start_adopt", "orphanage_player_start", "orphanage_reply", "{=Dramalord411}I would like to extend my family and adopt an orphan.", null, null);
            starter.AddPlayerLine("orphanage_player_start_adopt", "orphanage_player_start", "close_window", "{=Dramalord072}Nevermind.", null, null);

            starter.AddDialogLine("orphanage_reply_yes", "orphanage_reply", "orphanage_player_select_gender", "{=Dramalord412}Oh that is great {TITLE}. We have currently {BOYS} boys and {GIRLS} girls in our orphanage.", ConditionCanAdoptOrphan, null);
            starter.AddDialogLine("orphanage_reply_married", "orphanage_reply", "orphanage_player_goodbye", "{=Dramalord413}I am truly sorry {TITLE}, but you have to be married in order to adopt a child.", ConditionCanNotAdoptMarriage, null);
            starter.AddDialogLine("orphanage_reply_empty", "orphanage_reply", "orphanage_player_goodbye", "{=Dramalord414}I am truly sorry {TITLE}, but there are currently no children in our orphanage.", ConditionCanNotAdoptOrphanageEmpty, null);
            starter.AddDialogLine("orphanage_reply_empty", "orphanage_reply", "orphanage_player_goodbye", "{=Dramalord415}I am truly sorry {TITLE}, but you adopted recently. You have to wait a while before you can adopt again.", ConditionCanNotAdoptLastAdoption, null);

            starter.AddPlayerLine("orphanage_player_select_boy", "orphanage_player_select_gender", "orphanage_child_selection", "{=Dramalord416}I would like to adopt a boy.", ConditionPlayerSelectBoys, ConsequencePlayerSelectBoys);
            starter.AddPlayerLine("orphanage_player_select_girl", "orphanage_player_select_gender", "orphanage_child_selection", "{=Dramalord417}I would like to adopt a girl.", ConditionPlayerSelectGirls, ConsequencePlayerSelectGirls);
            starter.AddPlayerLine("orphanage_player_select_none", "orphanage_player_select_gender", "close_window", "{=Dramalord072}Nevermind..", null, null);

            starter.AddDialogLine("orphanage_select_orphan", "orphanage_child_selection", "orphanage_player_select_child", "{=Dramalord418}Who would you like to adopt?", null, null);

            starter.AddRepeatablePlayerLine("orphanage_player_select_child", "orphanage_player_select_child", "orphanage_confirm_adopt", "{=Dramalord419}{ORPHAN.NAME}, {ORPHANAGE} years old.", "{=Dramalord420}I am thinking of a different orphan.", "orphanage_child_selection", ConditionListAvailableOrphans, ConsequenceOrphanSelected);

            starter.AddDialogLine("orphanage_confirm_adopt", "orphanage_confirm_adopt", "orphanage_player_confirm", "{=Dramalord421}Very well. Are you absolutely sure you want to adopt {ORPHAN.NAME}?", ConditionConfirmAdopt, null);

            starter.AddPlayerLine("orphanage_confirm_yes", "orphanage_player_confirm", "orphanage_do_adopt", "{=str_yes}Yes.", null, null);
            starter.AddPlayerLine("orphanage_confirm_no", "orphanage_player_confirm", "orphanage_dont_adopt", "{=str_no}No.", null, null);

            starter.AddDialogLine("orphanage_do_adopt", "orphanage_do_adopt", "orphanage_player_goodbye", "{=Dramalord422}Congratulations! I'm sure {ORPHAN.NAME} will be in good hands.", ConditionDoAdopt, ConsequenceDoAdopt);
            starter.AddDialogLine("orphanage_dont_adopt", "orphanage_dont_adopt", "orphanage_player_goodbye", "{=Dramalord423}Oh well, that's sad. {ORPHAN.NAME} will be very disappointed.", ConditionDontAdopt, null);

            starter.AddPlayerLine("orphanage_player_goodbye", "orphanage_player_goodbye", "close_window", "{=GcCfYKDl}Farewell, then.", null, null);
        }

        internal static bool ConditionIsOrphanCharacter()
        {
            if (DramalordOrphanage.OrphanManager != null && Hero.OneToOneConversationHero == DramalordOrphanage.OrphanManager.HeroObject && ConversationHelper.ConversationIntention == ConversationType.OrphanConversation)
            {
                ConversationHelper.ConversationIntention = ConversationType.PlayerInteraction;
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(DramalordOrphanage.OrphanManager.HeroObject, Hero.MainHero, false));
                return true;
            }
            return false;
        }

        internal static bool ConditionCanAdoptOrphan()
        {
            // check last adoption
            if( Hero.MainHero.Spouse != null && DramalordOrphanage.Orphans.Count() > 0 && ((uint)CampaignTime.Now.ToDays) - DramalordOrphanage.GetLastAdoptionDay(Hero.MainHero) > DramalordMCM.Get.WaitBetweenAdopting)
            {
                MBTextManager.SetTextVariable("BOYS", DramalordOrphanage.Orphans.Where(item => !item.Character.HeroObject.IsFemale).Count());
                MBTextManager.SetTextVariable("GIRLS", DramalordOrphanage.Orphans.Where(item => item.Character.HeroObject.IsFemale).Count()); string yes = GameTexts.FindText("str_yes").ToString();
                string no = GameTexts.FindText("str_no").ToString();
                return true;
            }
            return false;
        }

        internal static bool ConditionCanNotAdoptMarriage()
        {
            return Hero.MainHero.Spouse == null;
        }

        internal static bool ConditionCanNotAdoptOrphanageEmpty()
        {
            return DramalordOrphanage.Orphans.Count() == 0;
        }

        internal static bool ConditionPlayerSelectBoys()
        {
            return DramalordOrphanage.Orphans.Where(item => !item.Character.HeroObject.IsFemale).Any();
        }

        internal static bool ConditionPlayerSelectGirls()
        {
            return DramalordOrphanage.Orphans.Where(item => item.Character.HeroObject.IsFemale).Any();
        }

        internal static bool ConditionCanNotAdoptLastAdoption()
        {
            return ((uint)CampaignTime.Now.ToDays) - DramalordOrphanage.GetLastAdoptionDay(Hero.MainHero) <= DramalordMCM.Get.WaitBetweenAdopting;
        }

        internal static bool ConditionListAvailableOrphans()
        {
            CharacterObject characterObject = ConversationSentence.CurrentProcessedRepeatObject as CharacterObject;
            if (characterObject != null)
            {
                StringHelpers.SetRepeatableCharacterProperties("ORPHAN", characterObject);
                MBTextManager.SetTextVariable("ORPHANAGE", (int)characterObject.Age);
                return true;
            }

            return false;
        }

        internal static bool ConditionConfirmAdopt()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", SelectedOrphan.CharacterObject);
            return true;
        }

        internal static bool ConditionDoAdopt()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", SelectedOrphan.CharacterObject);
            return true;
        }

        internal static bool ConditionDontAdopt()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", SelectedOrphan.CharacterObject);
            return true;
        }

        internal static void ConsequencePlayerSelectBoys()
        {
            ConversationSentence.SetObjectsToRepeatOver(DramalordOrphanage.Orphans.Where(item => !item.Character.HeroObject.IsFemale).Select(item => item.Character).ToList());
        }

        internal static void ConsequencePlayerSelectGirls()
        {
            ConversationSentence.SetObjectsToRepeatOver(DramalordOrphanage.Orphans.Where(item => item.Character.HeroObject.IsFemale).Select(item => item.Character).ToList());
        }

        internal static void ConsequenceOrphanSelected()
        {
            CharacterObject characterObject = ConversationSentence.SelectedRepeatObject as CharacterObject;
            SelectedOrphan = characterObject.HeroObject;
        }

        internal static void ConsequenceDoAdopt()
        {
            HeroAdoptAction.Apply(Hero.MainHero, Hero.MainHero.Spouse, SelectedOrphan);
            SelectedOrphan = null;
        }
    }
}
