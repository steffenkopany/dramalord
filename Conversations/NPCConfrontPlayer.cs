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

        internal static void Start(Hero hero, HeroEvent @event)
        {
            ConfrontingHero = hero;
            Event = @event;
            _randomChance = MBRandom.RandomInt(1, 100);
            bool civilian = hero.CurrentSettlement != null;
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(hero.CharacterObject, isCivilianEquipmentRequiredForLeader: civilian, noBodyguards: true, noHorse: true, noWeapon: true));
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_starts_confrontation_unknown", "start", "npc_declares_confrontation", "{=Dramalord166}Greetings, {TITLE}, you don't know me. I am {HERO} of the {CLAN}.", ConditionConfrontationStartUnknown, ConsequenceConfrontationStart, 120);
            starter.AddDialogLine("npc_starts_confrontation_known", "start", "npc_declares_confrontation", "{=Dramalord167}Good day, {TITLE}. Good that I catch you.", ConditionConfrontationStartKnown, null, 120);

            starter.AddDialogLine("npc_confrontation_date", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord168}It looks like you and {HERO} were enjoying some romantic time. Are you aware that their are my {STATUS}?", ConditionConfrontationDate, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_date_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord176}Apparently you and {HERO} are meeting privatly sometimes. Have you fogotten that you are {STATUS}?", ConditionConfrontationDatePlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_intercourse", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord169}Huh, you and {HERO} were pounding each other in bed. You know they are {STATUS}, do you?", ConditionConfrontationIntercourse, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_intercourse", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord177}So {HERO} was giving you a good time in bed, eh? You are {STATUS}, do you remember?", ConditionConfrontationIntercoursePlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_birth", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord178}{HERO} has given birth to {CHILD.LINK} and you are apparently the father. I thought you are {STATUS}?", ConditionConfrontationBirth, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_birth_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord170}So you have given birth to {CHILD.LINK}. The child is apparently from {HERO} and not from me. You are {STATUS}, right?", ConditionConfrontationBirthPlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_marriage", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord235}I've heard that you married {HERO}. Did they tell you that they are actually {STATUS}?", ConditionConfrontationMarriage, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_marriage_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord171}You married {HERO} behind my back. Did you even think about telling me? You are {STATUS}!", ConditionConfrontationMarriagePlayer, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_engagement", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord236}Congratulations to you and {HERO} on your engagement. I just want to mention that they are {STATUS}.", ConditionConfrontationEngagement, ConsequenceConfrontations);
            starter.AddDialogLine("npc_confrontation_engagement_player", "npc_declares_confrontation", "npc_summarize_confrontations", "{=Dramalord172}I heard that you and {HERO} are now engaged. Did you think I won't hear about it, {STATUS}?", ConditionConfrontationEngagementPlayer, ConsequenceConfrontations);

            starter.AddDialogLine("npc_confrontation_result_ok", "npc_summarize_confrontations", "close_window", "{=Dramalord180}I forgive you, {TITLE}. But I will not forget it.", ConditionConfrontationResultOk, ConsequenceConfrontationResultOk);
            starter.AddDialogLine("npc_confrontation_result_break", "npc_summarize_confrontations", "close_window", "{=Dramalord181}I will not accept this, {TITLE}. I will end this relationship.", ConditionConfrontationResultBreak, ConsequenceConfrontationResultBreak);
            starter.AddDialogLine("npc_confrontation_result_leave", "npc_summarize_confrontations", "close_window", "{=Dramalord232}I.. I don't know what to say. This is shocking me. I have to think if I can stay around you...", ConditionConfrontationResultLeave, ConsequenceConfrontationResultLeave);
            starter.AddDialogLine("npc_confrontation_result_other", "npc_summarize_confrontations", "close_window", "{=Dramalord182}This won't be forgotton, {TITLE}. Farewell.", ConditionConfrontationResultOther, ConsequenceConfrontationResultOther);
        }

        private static TextObject RelationName(Hero hero, Hero partner)
        {
            RelationshipType relation = hero.GetRelationTo(partner).Relationship;
            if (relation == RelationshipType.Friend) return new TextObject("{=Dramalord174}my friend");
            if (relation == RelationshipType.FriendWithBenefits) return new TextObject("{=Dramalord026}my special friend");
            if (relation == RelationshipType.Lover) return new TextObject("{=Dramalord023}my lover");
            if (relation == RelationshipType.Betrothed) return new TextObject("{=Dramalord025}my betrothed");
            if (relation == RelationshipType.Spouse || hero.Spouse == partner) return new TextObject("{=Dramalord172}my spouse");
            return new TextObject("{=Dramalord175}my acquaintance");
        }

        private static bool ConditionConfrontationStartUnknown()
        {
            if (Hero.OneToOneConversationHero != Hero.MainHero && Hero.OneToOneConversationHero.IsDramalordLegit() && ConfrontingHero == Hero.OneToOneConversationHero && !Hero.OneToOneConversationHero.HasMet)
            {
                ConfrontingHero = null;

                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                MBTextManager.SetTextVariable("HERO", Hero.OneToOneConversationHero.Name.ToString());
                MBTextManager.SetTextVariable("CLAN", Hero.OneToOneConversationHero.Clan?.Name.ToString());
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
                    MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                    MBTextManager.SetTextVariable("HERO", Hero.OneToOneConversationHero.Name.ToString());
                    MBTextManager.SetTextVariable("CLAN", Hero.OneToOneConversationHero.Clan?.Name.ToString());
                    Event = heroEvent;
                    DramalordIntentions.Instance.RemoveIntention(Hero.OneToOneConversationHero, Hero.MainHero, IntentionType.Confrontation, intention.EventId);
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
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
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
                    MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
                    Event = heroEvent;
                    DramalordIntentions.Instance.RemoveIntention(Hero.OneToOneConversationHero, Hero.MainHero, IntentionType.Confrontation, intention.EventId);
                    return true;
                }
            }
            return false;
        }

        private static bool ConditionConfrontationDate()
        {
            if(Event.Type == EventType.Date && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, other));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationDatePlayer()
        {
            if (Event.Type == EventType.Date && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationIntercourse()
        {
            if (Event.Type == EventType.Intercourse && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, other));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationIntercoursePlayer()
        {
            if (Event.Type == EventType.Intercourse && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationBirth()
        {
            if (Event.Type == EventType.Birth && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Event.Actors.Hero2.Father == Hero.MainHero)
            {
                MBTextManager.SetTextVariable("HERO", Event.Actors.Hero1.Name);
                MBTextManager.SetTextVariable("CHILD", Event.Actors.Hero2.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationBirthPlayer()
        {
            if (Event.Type == EventType.Birth && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Event.Actors.Hero1 == Hero.MainHero)
            {
                MBTextManager.SetTextVariable("HERO", Event.Actors.Hero2.Father.Name);
                MBTextManager.SetTextVariable("CHILD", Event.Actors.Hero2.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationMarriage()
        {
            if (Event.Type == EventType.Marriage && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, other));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationMarriagePlayer()
        {
            if (Event.Type == EventType.Marriage && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationEngagement()
        {
            if (Event.Type == EventType.Betrothed && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, other));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationEngagementPlayer()
        {
            if (Event.Type == EventType.Betrothed && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
            {
                Hero other = (Event.Actors.Hero1 == Hero.MainHero) ? Event.Actors.Hero2 : Event.Actors.Hero1;
                MBTextManager.SetTextVariable("HERO", other.Name);
                MBTextManager.SetTextVariable("STATUS", RelationName(Hero.OneToOneConversationHero, Hero.MainHero));
                return true;
            }
            return false;
        }

        private static bool ConditionConfrontationResultOk()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love > 0;
        }

        private static bool ConditionConfrontationResultBreak()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < 0 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism < _randomChance;
        }

        private static bool ConditionConfrontationResultLeave()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < 0 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism >= _randomChance;
        }

        private static bool ConditionConfrontationResultOther()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.PlayerTitle(false));
            return !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero);
        }

        private static void ConsequenceConfrontationStart()
        {
            Hero.OneToOneConversationHero?.SetHasMet();
        }

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

            relation.Trust += (int)trustDamage;
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
            DramalordIntentions.Instance.AddIntention(Hero.OneToOneConversationHero, Hero.MainHero, IntentionType.LeaveClan, -1);
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
