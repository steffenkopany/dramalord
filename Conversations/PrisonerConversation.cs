using Dramalord.Data;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class PrisonerConversation
    {
        private static TextObject player_prisoner_start = new("{=Dramalord273}Prisoner, I need to have a word with you.");
        private static TextObject npc_prisoner_reply_yes = new("{=Dramalord274}It's not like I have much of a choice {TITLE}.");
        private static TextObject npc_prisoner_reply_no = new("{=Dramalord275}I refuse to converse with you.");
        private static TextObject player_wants_prisonfun = new("{=Dramalord276}I would consider letting you go for... some special service in my bedroom.");
        private static TextObject player_wants_kill = new("{=Dramalord277}It's time to end this. Your existence is bothering me.");
        private static TextObject player_wants_nothing = new("{=Dramalord255}Nevermind.");
        private static TextObject npc_end_conversation = new("{=Dramalord186}As you wish, {TITLE}.");
        private static TextObject npc_prisonfun_reaction_yes = new("{=Dramalord278}You got yourself a deal! I think I will even enjoy it.");
        private static TextObject npc_prisonfun_reaction_no = new("{=Dramalord279}Never! You will not taint my honor with such offers!");
        private static TextObject npc_kill_reaction_yes = new("{=Dramalord280}Do what you must, you honorless swine!");
        private static TextObject npc_kill_reaction_no = new("{=Dramalord281}Oh god! No please... I beg you!");
        private static TextObject npc_kill_reaction_offer = new("{=Dramalord282}Wait {TITLE}! Why choose death if there's also pleasure?");
        private static TextObject player_choose_pleasure_yes = new("{=Dramalord283}Hmm... Alright. I accept. I spare you this time if you perform well.");
        private static TextObject player_choose_pleasure_no = new("{=Dramalord284}Well, your death is my sweetest pleasure.");

        private static void SetupLines()
        {
            npc_prisoner_reply_yes.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_end_conversation.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            npc_kill_reaction_offer.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));

            MBTextManager.SetTextVariable("player_prisoner_start", player_prisoner_start);
            MBTextManager.SetTextVariable("npc_prisoner_reply_yes", npc_prisoner_reply_yes);
            MBTextManager.SetTextVariable("npc_prisoner_reply_no", npc_prisoner_reply_no);
            MBTextManager.SetTextVariable("player_wants_prisonfun", player_wants_prisonfun);
            MBTextManager.SetTextVariable("player_wants_kill", player_wants_kill);
            MBTextManager.SetTextVariable("player_wants_nothing", player_wants_nothing);
            MBTextManager.SetTextVariable("npc_end_conversation", npc_end_conversation);
            MBTextManager.SetTextVariable("npc_prisonfun_reaction_yes", npc_prisonfun_reaction_yes);
            MBTextManager.SetTextVariable("npc_prisonfun_reaction_no", npc_prisonfun_reaction_no);
            MBTextManager.SetTextVariable("npc_kill_reaction_yes", npc_kill_reaction_yes);
            MBTextManager.SetTextVariable("npc_kill_reaction_no", npc_kill_reaction_no);
            MBTextManager.SetTextVariable("npc_kill_reaction_offer", npc_kill_reaction_offer);
            MBTextManager.SetTextVariable("player_choose_pleasure_yes", player_choose_pleasure_yes);
            MBTextManager.SetTextVariable("player_choose_pleasure_no", player_choose_pleasure_no);
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_prisoner_start", "hero_main_options", "npc_prisoner_reply", "{player_prisoner_start}", ConditionPlayerCanApproach, null);

            starter.AddDialogLine("npc_prisoner_reply_yes", "npc_prisoner_reply", "player_prisoner_selection", "{npc_prisoner_reply_yes}[ib:nervous2][if:convo_confused_normal]", ConditionNpcAcceptsApproach, null);
            starter.AddDialogLine("npc_prisoner_reply_no", "npc_prisoner_reply", "close_window", "{npc_prisoner_reply_no}[ib:closed][if:convo_bored]", ConditionNpcDeclinesApproach, null);

            starter.AddPlayerLine("player_wants_prisonfun", "player_prisoner_selection", "npc_prisonfun_reaction", "{player_wants_prisonfun}", null, null);
            starter.AddPlayerLine("player_wants_kill", "player_prisoner_selection", "npc_kill_reaction", "{player_wants_kill}", null, null);
            starter.AddPlayerLine("player_wants_nothing", "player_prisoner_selection", "npc_end_conversation", "{player_wants_nothing}", null, null);

            starter.AddDialogLine("npc_end_conversation", "npc_end_conversation", "hero_main_options", "{npc_end_conversation}", ConditionEndConversation, null);

            starter.AddDialogLine("npc_prisonfun_reaction_yes", "npc_prisonfun_reaction", "close_window", "{npc_prisonfun_reaction_yes}ib:weary2][if:convo_focused_happy]", ConditionNpcAcceptsFun, ConsequenceNpcAcceptsFun);
            starter.AddDialogLine("npc_prisonfun_reaction_no", "npc_prisonfun_reaction", "player_prisoner_selection", "{npc_prisonfun_reaction_no}[ib:closed][if:convo_annoyed]", ConditionNpcDeclinesFun, null);

            starter.AddDialogLine("npc_kill_reaction_yes", "npc_kill_reaction", "close_window", "{npc_kill_reaction_yes}[ib:warrior][if:convo_grave]", ConditionNpcAcceptsKill, ConsequenceKillNpc);
            starter.AddDialogLine("npc_kill_reaction_no", "npc_kill_reaction", "close_window", "{npc_kill_reaction_no}[ib:nervous][if:convo_shocked]", ConditionNpcDeclinesKill, ConsequenceKillNpc);
            starter.AddDialogLine("npc_kill_reaction_offer", "npc_kill_reaction", "player_choose_pleasure", "{npc_kill_reaction_offer}[ib:aggressive2][if:convo_approving]", ConditionNpcOffersKillAlternative, null);

            starter.AddPlayerLine("player_choose_pleasure_yes", "player_choose_pleasure", "close_window", "{player_choose_pleasure_yes}", null, ConsequenceNpcAcceptsFun);
            starter.AddPlayerLine("player_choose_pleasure_no", "player_choose_pleasure", "close_window", "{player_choose_pleasure_no}", null, ConsequenceKillNpc);
        }

        private static bool ConditionPlayerCanApproach()
        {
            if(Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero.IsPrisoner)
            {
                if (Hero.OneToOneConversationHero.CurrentSettlement?.OwnerClan == Clan.PlayerClan || MobileParty.MainParty.PrisonRoster.Contains(Hero.OneToOneConversationHero.CharacterObject))
                {
                    Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).UpdateLove();
                    SetupLines();
                    return true;
                }
            }
            return false;
        }

        private static bool ConditionNpcAcceptsApproach()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationWithPlayer() > -30;
        }

        private static bool ConditionNpcDeclinesApproach()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.GetRelationWithPlayer() <= -30;
        }

        private static bool ConditionEndConversation()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return true;
        }

        private static bool ConditionNpcAcceptsFun()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor < 0 && Hero.OneToOneConversationHero.GetPersonality().Openness > 0;
        }

        private static bool ConditionNpcDeclinesFun()
        {
            return !ConditionNpcAcceptsFun();
        }

        private static bool ConditionNpcAcceptsKill()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Valor > 0;
        }

        private static bool ConditionNpcDeclinesKill()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor > 0 && Hero.OneToOneConversationHero.GetHeroTraits().Valor <= 0;
        }

        private static bool ConditionNpcOffersKillAlternative()
        {
            return Hero.OneToOneConversationHero.GetHeroTraits().Honor <= 0 && Hero.OneToOneConversationHero.GetHeroTraits().Valor <= 0;
        }

        private static void ConsequenceNpcAcceptsFun()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Intercourse, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
            EndCaptivityAction.ApplyByRansom(Hero.OneToOneConversationHero, Hero.MainHero);
        }

        private static void ConsequenceKillNpc()
        {
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Execute, Hero.OneToOneConversationHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
