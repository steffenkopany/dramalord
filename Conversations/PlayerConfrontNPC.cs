using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class PlayerConfrontNPC
    {
        private static Hero? TargetHero = null;
        private static HeroEvent? Event = null;

        internal static void Start(Hero hero, HeroEvent @event)
        {
            TargetHero = hero;
            Event = @event;
            bool civilian = hero.CurrentSettlement != null;
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(hero.CharacterObject, isCivilianEquipmentRequiredForLeader: civilian, noBodyguards: true, noHorse: true, noWeapon: true));
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_confrontation", "start", "player_declares_confrontation", "{=Dramalord166}Oh, hello {TITLE}. I... didn't expect you...", ConditionConfrontationStart, null, 120);

            starter.AddPlayerLine("player_confrontation_date_player", "player_declares_confrontation", "npc_reply_confrontation", "{=Dramalord176}Apparently you and {HERO} are meeting privatly sometimes. Have you forgotten that you are {STATUS}?", ConditionConfrontationDatePlayer, null);
            starter.AddPlayerLine("player_confrontation_intercourse", "player_declares_confrontation", "npc_reply_confrontation", "{=Dramalord177}So {HERO} was giving you a good time in bed, eh? You are {STATUS}, do you remember?", ConditionConfrontationIntercoursePlayer, null);
            starter.AddPlayerLine("player_confrontation_birth", "player_declares_confrontation", "npc_reply_confrontation", "{=Dramalord170}So you have given birth to {CHILD}. The child is apparently from {FATHER} and not from me. You are {STATUS}, right?", ConditionConfrontationBirthPlayer, null);
            starter.AddPlayerLine("player_confrontation_marriage", "player_declares_confrontation", "npc_reply_confrontation", "{=Dramalord171}You married {HERO} behind my back. Did you even think about telling me? You are {STATUS}!", ConditionConfrontationMarriage, null);
            starter.AddPlayerLine("player_confrontation_engagement", "player_declares_confrontation", "npc_reply_confrontation", "{=Dramalord172}I heard that you and {HERO} are now engaged. Did you think I won't hear about it, {STATUS}?", ConditionConfrontationEngagement, null);
            starter.AddPlayerLine("player_confrontation_abort", "player_declares_confrontation", "close_window", "{=Dramalord183}Nevermind, {TITLE}. Farewell.", ConditionExitConversation, ConsequenceEndConversation);

            starter.AddDialogLine("npc_reply_confrontation_love", "npc_reply_confrontation", "player_summarize_confrontations", "{=Dramalord184}Oh {TITLE}, I love you! I beg you, don't do anything you might regret later...", ConditionReplyLove, null);
            starter.AddDialogLine("npc_reply_confrontation_nocare", "npc_reply_confrontation", "player_summarize_confrontations", "{=Dramalord185}Well, oh well, {TITLE}. So what now?", ConditionReplyNoCare, null);

            starter.AddPlayerLine("npc_confrontation_result_ok", "player_summarize_confrontations", "close_window", "{=Dramalord180}I forgive you, {TITLE}. But I will not forget it.", ConditionExitConversation, ConsequenceConfrontationResultOk);
            starter.AddPlayerLine("npc_confrontation_result_break", "player_summarize_confrontations", "close_window", "{=Dramalord181}I will not accept this, {TITLE}. I will end this relationship.", ConditionExitConversation, ConsequenceConfrontationResultBreak);
            starter.AddPlayerLine("npc_confrontation_result_break_other", "player_summarize_confrontations", "close_window", "{=Dramalord234}I give you one more chance, {TITLE}, but you will end it with {HERO}.", ConditionExitConversationLover, ConsequenceConfrontationResultBreakOther);
            starter.AddPlayerLine("npc_confrontation_result_leave", "player_summarize_confrontations", "close_window", "{=Dramalord233}Get out of my sight, {NAME}. Pack your stuff and leave.", ConditionExitConversationLeave, ConsequenceConfrontationResultLeave);
        }

        private static TextObject RelationName(Hero hero, Hero partner)
        {
            RelationshipType relation = hero.GetRelationTo(partner).Relationship;
            if (relation == RelationshipType.Friend) return new TextObject("{=Dramalord174}my friend");
            if (relation == RelationshipType.FriendWithBenefits) return new TextObject("{=Dramalord026}my special friend");
            if (relation == RelationshipType.Lover) return new TextObject("{=Dramalord023}my lover");
            if (relation == RelationshipType.Betrothed) return new TextObject("{=Dramalord025}my betrothed");
            if (relation == RelationshipType.Spouse) return new TextObject("{=Dramalord172}my spouse");
            return new TextObject("{=Dramalord175}my acquaintance");
        }

        private static bool ConditionConfrontationStart()
        {
            if (Hero.OneToOneConversationHero.IsDramalordLegit() && TargetHero == Hero.OneToOneConversationHero && Event != null)
            {
                TargetHero = null;
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationDatePlayer()
        {
            if (Event.Type == EventType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.OneToOneConversationHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.MainHero, Hero.OneToOneConversationHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationIntercoursePlayer()
        {
            if (Event.Type == EventType.Intercourse && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.OneToOneConversationHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.MainHero, Hero.OneToOneConversationHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationBirthPlayer()
        {
            if (Event.Type == EventType.Birth && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Event.Actors.Hero1 == Hero.OneToOneConversationHero)
            {
                MBTextManager.SetTextVariable("FATHER", Event.Actors.Hero2.Father.Name);
                MBTextManager.SetTextVariable("CHILD", Event.Actors.Hero2.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.MainHero, Hero.OneToOneConversationHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationMarriage()
        {
            if (Event.Type == EventType.Marriage && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.OneToOneConversationHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.MainHero, Hero.OneToOneConversationHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationEngagement()
        {
            if (Event.Type == EventType.Betrothed && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.OneToOneConversationHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.MainHero, Hero.OneToOneConversationHero));
                return true;
            }
            return false;
        }

        private static bool ConditionExitConversation()
        {
            MBTextManager.SetTextVariable("TITLE", RelationName(Hero.MainHero, Hero.OneToOneConversationHero)); 
            Hero other = (Event.Actors.Hero1 == Hero.OneToOneConversationHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
            return true;
        }

        private static bool ConditionExitConversationLover()
        {
            if(Event.Type == EventType.Intercourse || Event.Type == EventType.Date || Event.Type == EventType.Betrothed || Event.Type == EventType.Marriage)
            {
                MBTextManager.SetTextVariable("TITLE", RelationName(Hero.MainHero, Hero.OneToOneConversationHero));
                Hero other = (Event.Actors.Hero1 == Hero.OneToOneConversationHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                return true;
            }
            return false;
        }

        private static bool ConditionExitConversationLeave()
        {
            MBTextManager.SetTextVariable("NAME", Hero.OneToOneConversationHero.Name);
            return Hero.OneToOneConversationHero.Clan == Hero.MainHero.Clan;
        }

        private static bool ConditionReplyLove()
        {
            MBTextManager.SetTextVariable("TITLE", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove;
        }

        private static bool ConditionReplyNoCare()
        {
            MBTextManager.SetTextVariable("TITLE", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
            return Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove;
        }

        private static void ConsequenceEndConversation()
        {
            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultOk()
        {
            TextObject banner = new TextObject("{=Dramalord179}You forgave {HERO.LINK}. (Love {NUM}, Trust {NUM2})");
            StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
            MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber(5));
            MBTextManager.SetTextVariable("NUM2", ConversationHelper.FormatNumber(10));
            MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");

            Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Trust += 10;
            Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love += 5;

            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultBreak()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.BreakUp, Hero.OneToOneConversationHero, -1);

            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultBreakOther()
        {
            Hero other = (Event.Actors.Hero1 == Hero.OneToOneConversationHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
            DramalordIntentions.Instance.AddIntention(Hero.OneToOneConversationHero, other, IntentionType.BreakUp, -1);
            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultLeave()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.BreakUp, Hero.OneToOneConversationHero, -1);
            DramalordIntentions.Instance.AddIntention(Hero.OneToOneConversationHero, Hero.MainHero, IntentionType.LeaveClan, -1);
            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
