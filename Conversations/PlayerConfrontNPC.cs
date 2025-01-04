using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class PlayerConfrontNPC
    {
        private static Hero? TargetHero = null;
        private static HeroEvent? Event = null;

        private static TextObject npc_starts_confrontation = new("{=Dramalord482}Oh, hello {TITLE}. I... didn't expect you...");
        private static TextObject player_confrontation_date_player = new("{=Dramalord176}Apparently you and {HERO} are meeting privatly sometimes. Have you forgotten that you are {STATUS}?");
        private static TextObject player_confrontation_intercourse = new("{=Dramalord177}So {HERO} was giving you a good time in bed, eh? You are {STATUS}, do you remember?");
        private static TextObject player_confrontation_birth = new("{=Dramalord170}So you have given birth to {CHILD}. The child is apparently from {FATHER} and not from me. You are {STATUS}, right?");
        private static TextObject player_confrontation_marriage = new("{=Dramalord171}You married {HERO} behind my back. Did you even think about telling me? You are {STATUS}!");
        private static TextObject player_confrontation_engagement = new("{=Dramalord172}I heard that you and {HERO} are now engaged. Did you think I won't hear about it, {STATUS}?");
        private static TextObject player_confrontation_abort = new("{=Dramalord183}Nevermind, {TITLE}. Farewell.");
        private static TextObject npc_reply_confrontation_love = new("{=Dramalord184}Oh {TITLE}, I love you! I beg you, don't do anything you might regret later...");
        private static TextObject npc_reply_confrontation_nocare = new("{=Dramalord185}Well, oh well, {TITLE}. So what now?");
        private static TextObject npc_confrontation_result_ok = new("{=Dramalord180}I forgive you, {TITLE}. But I will not forget it.");
        private static TextObject npc_confrontation_result_break = new("{=Dramalord181}I will not accept this, {TITLE}. I will end this relationship.");
        private static TextObject npc_confrontation_result_break_other = new("{=Dramalord234}I give you one more chance, {TITLE}, but you will end it with {HERO}.");
        private static TextObject npc_confrontation_result_leave = new("{=Dramalord233}Get out of my sight, {NAME}. Pack your stuff and leave.");

        internal static void Start(Hero hero, HeroEvent @event)
        {
            hero.GetRelationTo(Hero.MainHero).UpdateLove();
            TargetHero = hero;
            Event = @event;
            bool civilian = hero.CurrentSettlement != null;
            SetupLines();
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(hero.CharacterObject, isCivilianEquipmentRequiredForLeader: civilian, noBodyguards: true, noHorse: true, noWeapon: true));
        }

        private static void SetupLines()
        {
            npc_starts_confrontation.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, TargetHero));

            Hero other = (Event.Actors.Hero1 == TargetHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
            player_confrontation_date_player.SetTextVariable("HERO", other.Name);
            player_confrontation_date_player.SetTextVariable("STATUS", RelationName(Hero.MainHero, TargetHero));
            player_confrontation_intercourse.SetTextVariable("HERO", other.Name);
            player_confrontation_intercourse.SetTextVariable("STATUS", RelationName(Hero.MainHero, TargetHero));

            player_confrontation_birth.SetTextVariable("FATHER", Event.Actors.Hero2.Father?.Name ?? new TextObject("someone else"));
            player_confrontation_birth.SetTextVariable("CHILD", Event.Actors.Hero2.Name);
            player_confrontation_birth.SetTextVariable("STATUS", RelationName(Hero.MainHero, TargetHero));

            player_confrontation_marriage.SetTextVariable("HERO", other.Name);
            player_confrontation_marriage.SetTextVariable("STATUS", RelationName(Hero.MainHero, TargetHero));

            player_confrontation_engagement.SetTextVariable("HERO", other.Name);
            player_confrontation_engagement.SetTextVariable("STATUS", RelationName(Hero.MainHero, TargetHero));

            player_confrontation_abort.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false, TargetHero));

            npc_reply_confrontation_love.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, TargetHero));
            npc_reply_confrontation_nocare.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, TargetHero));

            npc_confrontation_result_ok.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false, TargetHero));
            npc_confrontation_result_break.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false, TargetHero));
            npc_confrontation_result_break_other.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false, TargetHero));
            npc_confrontation_result_break_other.SetTextVariable("HERO", other.Name);

            npc_confrontation_result_leave.SetTextVariable("TITLE", ConversationHelper.NpcTitle(false, TargetHero));

            MBTextManager.SetTextVariable("npc_starts_confrontation", npc_starts_confrontation);
            MBTextManager.SetTextVariable("player_confrontation_date_player", player_confrontation_date_player);
            MBTextManager.SetTextVariable("player_confrontation_intercourse", player_confrontation_intercourse);
            MBTextManager.SetTextVariable("player_confrontation_birth", player_confrontation_birth);
            MBTextManager.SetTextVariable("player_confrontation_marriage", player_confrontation_marriage);
            MBTextManager.SetTextVariable("player_confrontation_engagement", player_confrontation_engagement);
            MBTextManager.SetTextVariable("player_confrontation_abort", player_confrontation_abort);
            MBTextManager.SetTextVariable("npc_reply_confrontation_love", npc_reply_confrontation_love);
            MBTextManager.SetTextVariable("npc_reply_confrontation_nocare", npc_reply_confrontation_nocare);
            MBTextManager.SetTextVariable("npc_confrontation_result_ok", npc_confrontation_result_ok);
            MBTextManager.SetTextVariable("npc_confrontation_result_break", npc_confrontation_result_break);
            MBTextManager.SetTextVariable("npc_confrontation_result_break_other", npc_confrontation_result_break_other);
            MBTextManager.SetTextVariable("npc_confrontation_result_leave", npc_confrontation_result_leave);
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_confrontation", "start", "player_declares_confrontation", "{npc_starts_confrontation}[ib:nervous][if:convo_shocked]", ConditionConfrontationStart, null, 120);

            starter.AddPlayerLine("player_confrontation_date_player", "player_declares_confrontation", "npc_reply_confrontation", "{player_confrontation_date_player}", ConditionConfrontationDatePlayer, null);
            starter.AddPlayerLine("player_confrontation_intercourse", "player_declares_confrontation", "npc_reply_confrontation", "{player_confrontation_intercourse}", ConditionConfrontationIntercoursePlayer, null);
            starter.AddPlayerLine("player_confrontation_birth", "player_declares_confrontation", "npc_reply_confrontation", "{player_confrontation_birth}", ConditionConfrontationBirthPlayer, null);
            starter.AddPlayerLine("player_confrontation_marriage", "player_declares_confrontation", "npc_reply_confrontation", "{player_confrontation_marriage}", ConditionConfrontationMarriage, null);
            starter.AddPlayerLine("player_confrontation_engagement", "player_declares_confrontation", "npc_reply_confrontation", "{player_confrontation_engagement}", ConditionConfrontationEngagement, null);
            starter.AddPlayerLine("player_confrontation_abort", "player_declares_confrontation", "close_window", "{player_confrontation_abort}", ConditionExitConversation, ConsequenceEndConversation);

            starter.AddDialogLine("npc_reply_confrontation_love", "npc_reply_confrontation", "player_summarize_confrontations", "{npc_reply_confrontation_love}[ib:nervous][if:convo_shocked]", ConditionReplyLove, null);
            starter.AddDialogLine("npc_reply_confrontation_nocare", "npc_reply_confrontation", "player_summarize_confrontations", "{npc_reply_confrontation_nocare}[ib:normal2][if:convo_mocking_teasing]", ConditionReplyNoCare, null);

            starter.AddPlayerLine("npc_confrontation_result_ok", "player_summarize_confrontations", "close_window", "{npc_confrontation_result_ok}", ConditionExitConversation, ConsequenceConfrontationResultOk);
            starter.AddPlayerLine("npc_confrontation_result_break", "player_summarize_confrontations", "close_window", "{npc_confrontation_result_break}", ConditionExitConversation, ConsequenceConfrontationResultBreak);
            starter.AddPlayerLine("npc_confrontation_result_break_other", "player_summarize_confrontations", "close_window", "{npc_confrontation_result_break_other}", ConditionExitConversationLover, ConsequenceConfrontationResultBreakOther);
            starter.AddPlayerLine("npc_confrontation_result_leave", "player_summarize_confrontations", "close_window", "{npc_confrontation_result_leave}", ConditionExitConversationLeave, ConsequenceConfrontationResultLeave);
        }

        private static TextObject RelationName(Hero hero, Hero partner)
        {
            RelationshipType relation = hero.GetRelationTo(partner).Relationship;
            if (relation == RelationshipType.Friend) return new TextObject("{=Dramalord174}my friend");
            if (relation == RelationshipType.FriendWithBenefits) return new TextObject("{=Dramalord026}my special friend");
            if (relation == RelationshipType.Lover) return new TextObject("{=Dramalord023}my lover");
            if (relation == RelationshipType.Betrothed) return new TextObject("{=Dramalord025}my betrothed");
            if (relation == RelationshipType.Spouse) return new TextObject("{=Dramalord173}my spouse");
            return new TextObject("{=Dramalord175}my acquaintance");
        }

        private static bool ConditionConfrontationStart()
        {
            if (Hero.OneToOneConversationHero.IsDramalordLegit() && TargetHero == Hero.OneToOneConversationHero && Event != null)
            {
                TargetHero = null;
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationDatePlayer() => Event.Type == EventType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationIntercoursePlayer() => Event.Type == EventType.Intercourse && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationBirthPlayer() => Event.Type == EventType.Birth && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Event.Actors.Hero1 == Hero.OneToOneConversationHero;

        private static bool ConditionConfrontationMarriage() => Event.Type == EventType.Marriage && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationEngagement() => Event.Type == EventType.Betrothed && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionExitConversation()
        {
            return true;
        }

        private static bool ConditionExitConversationLover() => Event.Type == EventType.Intercourse || Event.Type == EventType.Date || Event.Type == EventType.Betrothed || Event.Type == EventType.Marriage;

        private static bool ConditionExitConversationLeave() => Hero.OneToOneConversationHero.Clan == Hero.MainHero.Clan;

        private static bool ConditionReplyLove() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove >= DramalordMCM.Instance.MinDatingLove;

        private static bool ConditionReplyNoCare() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove < DramalordMCM.Instance.MinDatingLove;

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

            Hero.OneToOneConversationHero.SetTrust(Hero.MainHero, Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) + 10);
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
            Hero.OneToOneConversationHero.AddIntention(other, IntentionType.BreakUp, -1);
            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultLeave()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.BreakUp, Hero.OneToOneConversationHero, -1);
            Hero.OneToOneConversationHero.AddIntention(Hero.MainHero, IntentionType.LeaveClan, -1);
            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
