using Dramalord.Actions;
using Dramalord.Data;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class OrphanageConversation
    {
        private static bool _isOrphanageConversation = false;
        private static Hero? _selectedOrphan = null;
        internal static void Start()
        {
            _isOrphanageConversation = true;
            CharacterObject? character = Settlement.CurrentSettlement?.Notables.FirstOrDefault(notable => notable.Occupation == Occupation.GangLeader)?.CharacterObject;
            if(character != null)
            {
                character.HeroObject?.SetHasMet();
                CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(character, isCivilianEquipmentRequiredForLeader: true, noBodyguards: true, noHorse: true, noWeapon: true));
            }
            else
            {
                _isOrphanageConversation = false;
            }
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("orphanage_greeting", "start", "orphanage_player_start", "{=Dramalord252}Good day, {TITLE}. How can I help you?", ConditionOrphanageConversation, null, 120);

            starter.AddPlayerLine("orphanage_player_start_adopt", "orphanage_player_start", "adopt_reply", "{=Dramalord253}I would like to extend my family and adopt an orphan.", null, null);
            starter.AddPlayerLine("orphanage_player_start_orphanize", "orphanage_player_start", "orphanize_reply", "{=Dramalord254}I want to get rid off a child in my clan.", null, null);
            starter.AddPlayerLine("orphanage_player_start_abort", "orphanage_player_start", "close_window", "{=Dramalord255}Nevermind.", null, null);

            starter.AddDialogLine("adopt_reply_yes", "adopt_reply", "orphanage_player_select_gender", "{=Dramalord256}Oh that is great {TITLE}. We have currently {BOYS} boys and {GIRLS} girls in our orphanage.", ConditionCanAdoptOrphan, null);
            starter.AddDialogLine("adopt_reply_married", "adopt_reply", "orphanage_player_goodbye", "{=Dramalord257}I am truly sorry {TITLE}, but you have to be married in order to adopt a child.", ConditionCanNotAdoptMarriage, null);
            starter.AddDialogLine("adopt_reply_empty", "adopt_reply", "orphanage_player_goodbye", "{=Dramalord258}My apologies {TITLE}, but there are currently no children in our orphanage.", ConditionCanNotAdoptOrphanageEmpty, null);

            starter.AddPlayerLine("orphanage_player_select_boy", "orphanage_player_select_gender", "orphanage_child_selection", "{=Dramalord259}I would like to adopt a boy.", ConditionPlayerSelectBoys, ConsequencePlayerSelectBoys);
            starter.AddPlayerLine("orphanage_player_select_girl", "orphanage_player_select_gender", "orphanage_child_selection", "{=Dramalord260}I would like to adopt a girl.", ConditionPlayerSelectGirls, ConsequencePlayerSelectGirls);
            starter.AddPlayerLine("orphanage_player_select_none", "orphanage_player_select_gender", "close_window", "{=Dramalord255}Nevermind.", null, null);

            starter.AddDialogLine("orphanage_select_orphan", "orphanage_child_selection", "orphanage_player_select_child", "{=Dramalord261}Who would you like to adopt?", null, null);

            starter.AddRepeatablePlayerLine("orphanage_player_select_child", "orphanage_player_select_child", "orphanage_confirm_adopt", "{=Dramalord262}{ORPHAN.NAME}, {ORPHANAGE} years old.", "{=Dramalord263}I am thinking of a different child.", "orphanage_child_selection", ConditionListAvailableOrphans, ConsequenceOrphanSelected);

            starter.AddDialogLine("orphanage_confirm_adopt", "orphanage_confirm_adopt", "orphanage_player_confirm", "{=Dramalord264}Very well. Are you absolutely sure you want to adopt {ORPHAN.NAME}?", ConditionConfirmAdopt, null);

            starter.AddPlayerLine("orphanage_confirm_yes", "orphanage_player_confirm", "orphanage_do_adopt", "{=str_yes}Yes.", null, null);
            starter.AddPlayerLine("orphanage_confirm_no", "orphanage_player_confirm", "orphanage_dont_adopt", "{=str_no}No.", null, null);

            starter.AddDialogLine("orphanage_do_adopt", "orphanage_do_adopt", "orphanage_player_goodbye", "{=Dramalord265}Congratulations! I'm sure {ORPHAN.NAME} will be in good hands.", ConditionDoAdopt, ConsequenceDoAdopt);
            starter.AddDialogLine("orphanage_dont_adopt", "orphanage_dont_adopt", "orphanage_player_goodbye", "{=Dramalord266}Oh well, that's sad. {ORPHAN.NAME} will be very disappointed.", ConditionDontAdopt, null);

            starter.AddPlayerLine("orphanage_player_goodbye", "orphanage_player_goodbye", "close_window", "{=GcCfYKDl}Farewell, then.", null, null);


            starter.AddDialogLine("orphanize_reply_yes", "orphanize_reply", "orphanage_player_ownchild_selection", "{=Dramalord267}Of course {TITLE}, we can arrange that.", ConditionCanOrphanizeOrphan, ConsequencePlayerSelectChild);
            starter.AddDialogLine("orphanize_reply_empty", "orphanize_reply", "orphanage_player_goodbye", "{=Dramalord268}I appears {TITLE}, that there are no children in your clan.", ConditionCanNotOrphanizeOrphan, null);

            starter.AddDialogLine("orphanage_player_ownchild", "orphanage_player_ownchild_selection", "orphanage_player_select_ownchild", "{=Dramalord269}Who would be the unfortunate child we should take into custody?", null, null);

            starter.AddRepeatablePlayerLine("orphanage_player_select_ownchild", "orphanage_player_select_ownchild", "orphanage_confirm_orphanize", "{=Dramalord262}{ORPHAN.NAME}, {ORPHANAGE} years old.", "{=Dramalord263}I am thinking of a different child.", "orphanage_player_ownchild_selection", ConditionListAvailableChildren, ConsequenceChildSelected);

            starter.AddDialogLine("orphanage_confirm_adopt", "orphanage_confirm_orphanize", "orphanage_player_confirm_orphanize", "{=Dramalord270}Very well. Are you absolutely sure you want to give away {ORPHAN.NAME}?", ConditionConfirmOrphanize, null);

            starter.AddPlayerLine("orphanage_player_confirm_orphanize_yes", "orphanage_player_confirm_orphanize", "orphanage_do_orphanize", "{=str_yes}Yes.", null, null);
            starter.AddPlayerLine("orphanage_player_confirm_orphanize_no", "orphanage_player_confirm_orphanize", "orphanage_dont_orphanize", "{=str_no}No.", null, null);

            starter.AddDialogLine("orphanage_do_orphanize", "orphanage_do_orphanize", "orphanage_player_goodbye", "{=Dramalord271}Thus, it is decided! We will take care of {ORPHAN.NAME} and find a new home for the child.", ConditionDoOrphanize, ConsequenceDoOrphanize);
            starter.AddDialogLine("orphanage_dont_orphanize", "orphanage_dont_orphanize", "orphanage_player_goodbye", "{=Dramalord272}Very well, {TITLE}. {ORPHAN.NAME} will stay in your clan.", ConditionDontOrphanize, null);
        }

        private static bool ConditionOrphanageConversation()
        {
            if(_isOrphanageConversation)
            {
                _isOrphanageConversation = false;
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                return true;
            }
            
            return false;
        }

        private static bool ConditionCanAdoptOrphan()
        {
            int boys = DramalordOrphans.Instance.CountOrphans(false);
            int girls = DramalordOrphans.Instance.CountOrphans(true);
            if (Hero.MainHero.Spouse != null && boys + girls > 0)
            {
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                MBTextManager.SetTextVariable("BOYS", boys);
                MBTextManager.SetTextVariable("GIRLS", girls);
                return true;
            }
            return false;
        }

        private static bool ConditionCanNotAdoptMarriage()
        {
            return Hero.MainHero.Spouse == null;
        }

        private static bool ConditionCanNotAdoptOrphanageEmpty()
        {
            return DramalordOrphans.Instance.CountOrphans(false) + DramalordOrphans.Instance.CountOrphans(true) == 0;
        }

        private static bool ConditionPlayerSelectBoys()
        {
            return DramalordOrphans.Instance.CountOrphans(false) > 0;
        }

        private static bool ConditionPlayerSelectGirls()
        {
            return DramalordOrphans.Instance.CountOrphans(true) > 0;
        }

        private static bool ConditionListAvailableOrphans()
        {
            Hero? orphan = ConversationSentence.CurrentProcessedRepeatObject as Hero;
            if (orphan != null)
            {
                StringHelpers.SetRepeatableCharacterProperties("ORPHAN", orphan.CharacterObject);
                MBTextManager.SetTextVariable("ORPHANAGE", (int)orphan.Age);
                return true;
            }

            return false;
        }

        private static bool ConditionConfirmAdopt()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", _selectedOrphan?.CharacterObject);
            return true;
        }

        private static bool ConditionDoAdopt()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", _selectedOrphan?.CharacterObject);
            return true;
        }

        private static bool ConditionDontAdopt()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", _selectedOrphan?.CharacterObject);
            return true;
        }

        private static bool ConditionCanOrphanizeOrphan()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Clan.PlayerClan.Lords.Where(lord => lord.IsChild).Count() > 0;
        }

        private static bool ConditionCanNotOrphanizeOrphan()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Clan.PlayerClan.Lords.Where(lord => lord.IsChild).Count() == 0;
        }

        private static bool ConditionListAvailableChildren()
        {
            Hero? child = ConversationSentence.CurrentProcessedRepeatObject as Hero;
            if (child != null)
            {
                StringHelpers.SetRepeatableCharacterProperties("ORPHAN", child.CharacterObject);
                MBTextManager.SetTextVariable("ORPHANAGE", (int)child.Age);
                return true;
            }

            return false;
        }

        private static bool ConditionConfirmOrphanize()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", _selectedOrphan?.CharacterObject);
            return true;
        }

        private static bool ConditionDoOrphanize()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", _selectedOrphan?.CharacterObject);
            return true;
        }

        private static bool ConditionDontOrphanize()
        {
            StringHelpers.SetCharacterProperties("ORPHAN", _selectedOrphan?.CharacterObject);
            return true;
        }



        private static void ConsequencePlayerSelectBoys()
        {
            ConversationSentence.SetObjectsToRepeatOver(DramalordOrphans.Instance.GetOrphans(false));
        }

        private static void ConsequencePlayerSelectGirls()
        {
            ConversationSentence.SetObjectsToRepeatOver(DramalordOrphans.Instance.GetOrphans(true));
        }

        private static void ConsequenceOrphanSelected()
        {
            _selectedOrphan = ConversationSentence.SelectedRepeatObject as Hero;
        }

        internal static void ConsequenceDoAdopt()
        {
            AdoptAction.Apply(Hero.MainHero, Hero.MainHero.Spouse, _selectedOrphan);
            _selectedOrphan = null;
        }

        private static void ConsequenceChildSelected()
        {
            _selectedOrphan = ConversationSentence.SelectedRepeatObject as Hero;
        }

        private static void ConsequencePlayerSelectChild()
        {
            ConversationSentence.SetObjectsToRepeatOver(Clan.PlayerClan.Lords.Where(lord => lord.IsChild).ToList());
        }

        internal static void ConsequenceDoOrphanize()
        {
            OrphanizeAction.Apply(Hero.MainHero, _selectedOrphan);
            _selectedOrphan = null;
        }
    }
}
