using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class NPCConfrontPlayer
    {
        private static Hero? ConfrontingHero = null;
        private static HeroEvent? Event = null;
        private static int _randomChance = 0;

        private static TextObject npc_starts_confrontation_unknown = new("{=Dramalord166}Greetings, {TITLE}, you don't know me. I am {HERO} of the {CLAN}.");
        private static TextObject npc_starts_confrontation_known = new("{=Dramalord167}Good day, {TITLE}. Good that I catch you.");
        private static TextObject npc_confrontation_date = new("{=Dramalord168}It looks like you and {HERO} were enjoying some romantic time. Are you aware that their are my {STATUS}?");
        private static TextObject npc_confrontation_date_player = new("{=Dramalord176}Apparently you and {HERO} are meeting privatly sometimes. Have you fogotten that you are {STATUS}?");
        private static TextObject npc_confrontation_intercourse = new("{=Dramalord169}Huh, you and {HERO} were pounding each other in bed. You know they are {STATUS}, do you?");
        private static TextObject npc_confrontation_intercourse_player = new("{=Dramalord177}So {HERO} was giving you a good time in bed, eh? You are {STATUS}, do you remember?");
        private static TextObject npc_confrontation_birth = new("{=Dramalord178}{HERO} has given birth to {CHILD} and you are apparently the father. I thought you are {STATUS}?");
        private static TextObject npc_confrontation_birth_player = new("{=Dramalord170}So you have given birth to {CHILD}. The child is apparently from {HERO} and not from me. You are {STATUS}, right?");
        private static TextObject npc_confrontation_marriage = new("{=Dramalord235}I've heard that you married {HERO}. Did they tell you that they are actually {STATUS}?");
        private static TextObject npc_confrontation_marriage_player = new("{=Dramalord171}You married {HERO} behind my back. Did you even think about telling me? You are {STATUS}!");
        private static TextObject npc_confrontation_engagement = new("{=Dramalord236}Congratulations to you and {HERO} on your engagement. I just want to mention that they are {STATUS}.");
        private static TextObject npc_confrontation_engagement_player = new("{=Dramalord172}I heard that you and {HERO} are now engaged. Did you think I won't hear about it, {STATUS}?");
        private static TextObject npc_confrontation_result_ok = new("{=Dramalord180}I forgive you, {TITLE}. But I will not forget it.");
        private static TextObject npc_confrontation_result_break = new("{=Dramalord181}I will not accept this, {TITLE}. I will end this relationship.");
        private static TextObject npc_confrontation_result_leave = new("{=Dramalord232}I.. I don't know what to say. This is shocking me. I have to think if I can stay around you...");
        private static TextObject npc_confrontation_result_other = new("{=Dramalord182}This won't be forgotton, {TITLE}. Farewell.");

        internal static void Start(Hero hero, HeroEvent @event)
        {
            ConfrontingHero = hero;
            Event = @event;
            _randomChance = MBRandom.RandomInt(1, 100);
            bool civilian = hero.CurrentSettlement != null;
            hero.GetRelationTo(Hero.MainHero).UpdateLove();
            SetupLines();
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(hero.CharacterObject, isCivilianEquipmentRequiredForLeader: civilian, noBodyguards: true, noHorse: true, noWeapon: true));
        }

        private static void SetupLines()
        {
            npc_starts_confrontation_unknown.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, ConfrontingHero));
            npc_starts_confrontation_unknown.SetTextVariable("HERO", ConfrontingHero?.Name.ToString());
            npc_starts_confrontation_unknown.SetTextVariable("CLAN", ConfrontingHero?.Clan?.Name.ToString());
            npc_starts_confrontation_known.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, ConfrontingHero));

            Hero? other = (Event?.Actors.Hero1 == Hero.MainHero) ? Event?.Actors.Hero2 : Event?.Actors.Hero1;
            npc_confrontation_date.SetTextVariable("HERO", other?.Name);
            npc_confrontation_date.SetTextVariable("STATUS", RelationName(ConfrontingHero, other));
            npc_confrontation_date_player.SetTextVariable("HERO", other?.Name);
            npc_confrontation_date_player.SetTextVariable("STATUS", RelationName(ConfrontingHero, Hero.MainHero));

            npc_confrontation_intercourse.SetTextVariable("HERO", other?.Name);
            npc_confrontation_intercourse.SetTextVariable("STATUS", RelationName(ConfrontingHero, other));
            npc_confrontation_intercourse_player.SetTextVariable("HERO", other?.Name);
            npc_confrontation_intercourse_player.SetTextVariable("STATUS", RelationName(ConfrontingHero, Hero.MainHero));

            npc_confrontation_birth.SetTextVariable("HERO", Event?.Actors.Hero1.Name);
            npc_confrontation_birth.SetTextVariable("CHILD", Event.Actors.Hero2.Name);
            npc_confrontation_birth.SetTextVariable("STATUS", RelationName(ConfrontingHero, Hero.MainHero));

            npc_confrontation_birth_player.SetTextVariable("HERO", Event?.Actors.Hero1.Name);
            npc_confrontation_birth_player.SetTextVariable("CHILD", Event.Actors.Hero2.Name);
            npc_confrontation_birth_player.SetTextVariable("STATUS", RelationName(ConfrontingHero, Hero.MainHero));

            npc_confrontation_marriage.SetTextVariable("HERO", other?.Name);
            npc_confrontation_marriage.SetTextVariable("STATUS", RelationName(ConfrontingHero, other));
            npc_confrontation_marriage_player.SetTextVariable("HERO", other?.Name);
            npc_confrontation_marriage_player.SetTextVariable("STATUS", RelationName(ConfrontingHero, Hero.MainHero));

            npc_confrontation_engagement.SetTextVariable("HERO", other?.Name);
            npc_confrontation_engagement.SetTextVariable("STATUS", RelationName(ConfrontingHero, other));
            npc_confrontation_engagement_player.SetTextVariable("HERO", other?.Name);
            npc_confrontation_engagement_player.SetTextVariable("STATUS", RelationName(ConfrontingHero, Hero.MainHero));

            npc_confrontation_result_ok.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, ConfrontingHero));
            npc_confrontation_result_break.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, ConfrontingHero));
            npc_confrontation_result_other.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false, ConfrontingHero));


            MBTextManager.SetTextVariable("npc_starts_confrontation_unknown", npc_starts_confrontation_unknown);
            MBTextManager.SetTextVariable("npc_starts_confrontation_known", npc_starts_confrontation_known);
            MBTextManager.SetTextVariable("npc_confrontation_date", npc_confrontation_date);
            MBTextManager.SetTextVariable("npc_confrontation_date_player", npc_confrontation_date_player);
            MBTextManager.SetTextVariable("npc_confrontation_intercourse", npc_confrontation_intercourse);
            MBTextManager.SetTextVariable("npc_confrontation_intercourse_player", npc_confrontation_intercourse_player);
            MBTextManager.SetTextVariable("npc_confrontation_birth", npc_confrontation_birth);
            MBTextManager.SetTextVariable("npc_confrontation_birth_player", npc_confrontation_birth_player);
            MBTextManager.SetTextVariable("npc_confrontation_marriage", npc_confrontation_marriage);
            MBTextManager.SetTextVariable("npc_confrontation_marriage_player", npc_confrontation_marriage_player);
            MBTextManager.SetTextVariable("npc_confrontation_engagement", npc_confrontation_engagement);
            MBTextManager.SetTextVariable("npc_confrontation_engagement_player", npc_confrontation_engagement_player);
            MBTextManager.SetTextVariable("npc_confrontation_result_ok", npc_confrontation_result_ok);
            MBTextManager.SetTextVariable("npc_confrontation_result_break", npc_confrontation_result_break);
            MBTextManager.SetTextVariable("npc_confrontation_result_leave", npc_confrontation_result_leave);
            MBTextManager.SetTextVariable("npc_confrontation_result_other", npc_confrontation_result_other);
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_confrontation_unknown", "start", "npc_declares_confrontation", "{npc_starts_confrontation_unknown}[ib:aggressive][if:convo_undecided_open]", ConditionConfrontationStartUnknown, ConsequenceConfrontationStart, 120);
            starter.AddDialogLine("npc_starts_confrontation_known", "start", "npc_declares_confrontation", "{npc_starts_confrontation_known}[ib:aggressive][if:convo_undecided_open]", ConditionConfrontationStartKnown, null, 120);

            starter.AddDialogLine("npc_confrontation_date", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_date}[ib:warrior][if:convo_grave]", ConditionConfrontationDate, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_date_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_date_player}[ib:warrior][if:convo_grave]", ConditionConfrontationDatePlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_intercourse", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_intercourse}[ib:warrior][if:convo_grave]", ConditionConfrontationIntercourse, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_intercourse_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_intercourse_player}[ib:warrior][if:convo_grave]", ConditionConfrontationIntercoursePlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_birth", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_birth}[ib:warrior][if:convo_grave]", ConditionConfrontationBirth, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_birth_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_birth_player}[ib:warrior][if:convo_grave]", ConditionConfrontationBirthPlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_marriage", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_marriage}[ib:warrior][if:convo_grave]", ConditionConfrontationMarriage, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_marriage_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_marriage_player}[ib:warrior][if:convo_grave]", ConditionConfrontationMarriagePlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_engagement", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_engagement}[ib:warrior][if:convo_grave]", ConditionConfrontationEngagement, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_engagement_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{npc_confrontation_engagement_player}[ib:warrior][if:convo_grave]", ConditionConfrontationEngagementPlayer, ConsequenceConfrontations);

            starter.AddDialogLine("npc_confrontation_result_ok", "npc_summarize_confrontations", "close_window", "{npc_confrontation_result_ok}[ib:nervous2][if:convo_confused_normal]", ConditionConfrontationResultOk, ConsequenceConfrontationResultOk);
            starter.AddDialogLine("npc_confrontation_result_break", "npc_summarize_confrontations", "close_window", "{npc_confrontation_result_break}[ib:nervous][if:convo_shocked]", ConditionConfrontationResultBreak, ConsequenceConfrontationResultBreak);
            starter.AddDialogLine("npc_confrontation_result_leave", "npc_summarize_confrontations", "close_window", "{npc_confrontation_result_leave}[ib:aggressive][if:convo_furious]", ConditionConfrontationResultLeave, ConsequenceConfrontationResultLeave);
            starter.AddDialogLine("npc_confrontation_result_other", "npc_summarize_confrontations", "close_window", "{npc_confrontation_result_other}[ib:weary2][if:convo_nervous]", ConditionConfrontationResultOther, ConsequenceConfrontationResultOther);
        }

        private static TextObject RelationName(Hero hero, Hero partner)
        {
            RelationshipType relation = hero.GetRelationTo(partner).Relationship;
            if (relation == RelationshipType.Friend) return new TextObject("{=Dramalord174}my friend");
            if (relation == RelationshipType.FriendWithBenefits) return new TextObject("{=Dramalord026}my special friend");
            if (relation == RelationshipType.Lover) return new TextObject("{=Dramalord023}my lover");
            if (relation == RelationshipType.Betrothed) return new TextObject("{=Dramalord025}my betrothed");
            if (relation == RelationshipType.Spouse || hero.Spouse == partner) return new TextObject("{=Dramalord173}my spouse");
            return new TextObject("{=Dramalord175}my acquaintance");
        }

        private static bool ConditionConfrontationStartUnknown()
        {
            if (Hero.OneToOneConversationHero != Hero.MainHero && Hero.OneToOneConversationHero.IsDramalordLegit() && ConfrontingHero == Hero.OneToOneConversationHero && !Hero.OneToOneConversationHero.HasMet)
            {
                ConfrontingHero = null;
                return true;
            }
            else if (Hero.OneToOneConversationHero.IsDramalordLegit() && !Hero.OneToOneConversationHero.HasMet && DramalordIntentions.Instance.GetIntentionTowardsPlayer().Contains(Hero.OneToOneConversationHero))
            {
                HeroIntention? intention = Hero.OneToOneConversationHero.GetIntentions().FirstOrDefault(intention => intention.Target == Hero.MainHero && intention.Type == IntentionType.Confrontation && intention.EventId > -1);
                HeroEvent? heroEvent = null;
                if (intention != null)
                {
                    heroEvent = DramalordEvents.Instance.GetEvent(intention.EventId);
                }
                if(intention != null && heroEvent != null)
                {
                    Event = heroEvent;
                    Hero.OneToOneConversationHero.RemoveIntention(intention);
                    return true;
                }
            }
            return false;
        }

        private static bool ConditionConfrontationStartKnown()
        {
            if (Hero.OneToOneConversationHero.IsDramalordLegit() && ConfrontingHero == Hero.OneToOneConversationHero && Hero.OneToOneConversationHero.HasMet)
            {
                ConfrontingHero = null;
                return true;
            }
            else if (Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero.HasMet && DramalordIntentions.Instance.GetIntentionTowardsPlayer().Contains(Hero.OneToOneConversationHero))
            {
                HeroIntention? intention = Hero.OneToOneConversationHero.GetIntentions().FirstOrDefault(intention => intention.Target == Hero.MainHero && intention.Type == IntentionType.Confrontation && intention.EventId > -1);
                HeroEvent? heroEvent = null;
                if (intention != null)
                {
                    heroEvent = DramalordEvents.Instance.GetEvent(intention.EventId);
                }
                if (intention != null && heroEvent != null)
                {
                    Event = heroEvent;
                    Hero.OneToOneConversationHero.RemoveIntention(intention);
                    return true;
                }
            }
            return false;
        }

        private static bool ConditionConfrontationDate() => Event.Type == EventType.Date && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationDatePlayer() => Event.Type == EventType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationIntercourse() => Event.Type == EventType.Intercourse && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationIntercoursePlayer() => Event.Type == EventType.Intercourse && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationBirth() => Event.Type == EventType.Birth && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Event.Actors.Hero2.Father == Hero.MainHero;

        private static bool ConditionConfrontationBirthPlayer() => Event.Type == EventType.Birth && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Event.Actors.Hero1 == Hero.MainHero;

        private static bool ConditionConfrontationMarriage() => Event.Type == EventType.Marriage && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationMarriagePlayer() => Event.Type == EventType.Marriage && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationEngagement() => Event.Type == EventType.Betrothed && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationEngagementPlayer() => Event.Type == EventType.Betrothed && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static bool ConditionConfrontationResultOk() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove > 0;

        private static bool ConditionConfrontationResultBreak() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove < 0 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism < _randomChance;

        private static bool ConditionConfrontationResultLeave() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove < 0 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism >= _randomChance;

        private static bool ConditionConfrontationResultOther() => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);

        private static void ConsequenceConfrontationStart() => Hero.OneToOneConversationHero?.SetHasMet();

        private static void ConsequenceConfrontations()
        {
            Hero hero = Hero.OneToOneConversationHero;
            HeroPersonality personality = hero.GetPersonality();
            HeroRelation relation = hero.GetRelationTo(Hero.MainHero);
            HeroEvent @event = Event;

            float loveDamage = -100;
            float trustDamage = -100;

            if (@event.Type == EventType.Date)
            {
                loveDamage /= 5;
                trustDamage /= 3;
            }
            else if (@event.Type == EventType.Intercourse)
            {
                loveDamage /= 4;
                trustDamage /= 2;
            }
            else if (@event.Type == EventType.Betrothed)
            {
                loveDamage *= 1;
                trustDamage *= 1;
            }
            else if (@event.Type == EventType.Marriage)
            {
                loveDamage *= 1.5f;
                trustDamage *= 1f;
            }
            else if (@event.Type == EventType.Birth)
            {
                loveDamage /= 3;
                trustDamage *= 1.5f;
            }

            float agreeFactor = ((float)personality.Agreeableness) / 100f;
            float openFactor = ((float)personality.Openness) / 100f;
            float extroFactor = ((float)personality.Extroversion) / 100f;
            float neuroFactor = ((float)personality.Neuroticism) / 100f;
            float conscFactor = ((float)personality.Conscientiousness) / 100f;

            loveDamage -= loveDamage * agreeFactor;
            loveDamage -= loveDamage * openFactor;

            trustDamage += trustDamage * neuroFactor;
            trustDamage += trustDamage * conscFactor;

            //relation.Trust += (int)trustDamage;
            hero.SetTrust(Hero.MainHero, hero.GetTrust(Hero.MainHero) + (int)trustDamage);
            relation.Love += (int)loveDamage;

            TextObject banner = new TextObject("{=Dramalord179}Relation loss with {HERO.LINK}. (Love {NUM}, Trust {NUM2})");
            StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
            MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber((int)loveDamage));
            MBTextManager.SetTextVariable("NUM2", ConversationHelper.FormatNumber((int)trustDamage));
            MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
        }

        private static void ConsequenceConfrontationResultOk()
        {
            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultBreak()
        {
            Event = null;
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.BreakUp, Hero.MainHero, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultLeave()
        {
            Event = null;
            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.BreakUp, Hero.MainHero, -1);
            Hero.OneToOneConversationHero.AddIntention(Hero.MainHero, IntentionType.LeaveClan, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceConfrontationResultOther()
        {
            Event = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }
    }
}
