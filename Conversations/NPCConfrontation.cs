using Dramalord.Actions;
using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal sealed class NPCConfrontation
    {
        private static Hero? ApproachingHero = null;
        private static HeroMemory? Memory = null;
        private static Hero? OtherHero = null;
        private static bool PlayerDenied = true;

        internal static void start(Hero hero, HeroMemory memory)
        {
            NPCConfrontation.ApproachingHero = hero;
            NPCConfrontation.Memory = memory;
            NPCConfrontation.OtherHero = memory.Event?.Hero1 != Hero.MainHero.CharacterObject ? memory.Event?.Hero1.HeroObject : memory.Event?.Hero2.HeroObject;
            ConversationHelper.ConversationIntention = ConversationType.NPCConfrontation;
            Campaign.Current.SetTimeSpeed(0);
            CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(NPCConfrontation.ApproachingHero.CharacterObject, isCivilianEquipmentRequiredForLeader: true));
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_start_accusation", "start", "npc_accusation_type", "{=Dramalord337}{TITLE}, I have to talk to you![if:convo_angry_voice]", ConditionNpcCanConfrontPlayer, null, 120);

            starter.AddDialogLine("npc_accusation_pregnancy", "player_accusation_list", "npc_accusation_source", "{=Dramalord305}You carry no child of mine, am I correct?", ConditionNpcSeesPregnancyPlayer, null);
            starter.AddDialogLine("npc_accusation_pregnancy_npc", "player_accusation_list", "npc_accusation_source", "{=Dramalord409}{HERO.LINK} is carrying your child, am I correct?", ConditionNpcSeesPregnancyNpc, null);
            starter.AddDialogLine("npc_accusation_bastard", "player_accusation_list", "npc_accusation_source", "{=Dramalord308}{TARGET} is clearly not my child, how dare you to give birth to a bastard?!", ConditionNpcSeesBastard, null);
            starter.AddDialogLine("npc_accusation_date", "npc_accusation_type", "npc_accusation_source", "{=Dramalord306}I guess you and {TARGET} were not talking about politics, eh?", ConditionNpcAccusationDate, null);
            starter.AddDialogLine("npc_accusation_intercourse", "npc_accusation_type", "npc_accusation_source", "{=Dramalord307}Looks like you're having a hard time keeping your underpants on when meeting {TARGET}.", ConditionNpcAccusationIntercourse, null);
            starter.AddDialogLine("npc_accusation_marriage", "npc_accusation_type", "npc_accusation_source", "{=Dramalord336}So you and {TARGET} married behind my back?", ConditionNpcAccusationMarriage, null);
            starter.AddDialogLine("npc_accusation_lostit", "npc_accusation_type", "close_window", "{=Dramalord378}Uh... I somehow forgot what I wanted to say. Excuse me.", ConditionNpcAccusationForgot, null);

            starter.AddDialogLine("npc_accusation_witnessed", "npc_accusation_source", "player_accusation_reaction", "{=Dramalord338}I saw you and {TARGET.LINK} with my own eyes!", ConditionNpcAccusationWitness, null);
            starter.AddDialogLine("npc_accusation_gossip", "npc_accusation_source", "player_accusation_reaction", "{=Dramalord339}I learned that from {SOURCE.LINK}!", ConditionNpcAccusationGossip, null);
            starter.AddDialogLine("npc_accusation_direct", "npc_accusation_source", "player_accusation_reaction", "{=Dramalord358}I can clearly see that!", ConditionNpcAccusationDirect, null);

            starter.AddPlayerLine("player_react_gossip_admit", "player_accusation_reaction", "npc_accusation_action", "{=Dramalord309}You are right. So what? I don't care.", ConditionPlayerGossipDoesntCare, ConsequencePlayerAdmit);
            starter.AddPlayerLine("player_react_gossip_deny", "player_accusation_reaction", "npc_accusation_action", "{=Dramalord310}What? I have no idea what you are talking about!", ConditionPlayerGossipPlaysInnocent, ConsequencePlayerDenied);
            starter.AddPlayerLine("player_react_witnessed", "player_accusation_reaction", "npc_accusation_action", "{=Dramalord311}{TITLE}, please! This is all a misunderstanding!", ConditionPlayerWitnessBegsForgiveness, ConsequencePlayerDenied);

            starter.AddDialogLine("Dramalord312", "npc_accusation_action", "close_window", "{=Dramalord312}This is the last breath you take!", ConditionNpcWantsKillPlayer, ConsequenceNpcWantsKillPlayer);
            starter.AddDialogLine("Dramalord313", "npc_accusation_action", "close_window", "{=Dramalord313}Get out of my sight! I never want to see you again!", ConditionNpcWantsLeaveClan, ConsequenceNpcWantsLeaveClan);
            starter.AddDialogLine("Dramalord314", "npc_accusation_action", "close_window", "{=Dramalord314}That's it! It's over!", ConditionNpcWantsBreakUpOrDivorce, ConsequenceNpcWantsBreakUpOrDivorce);
            starter.AddDialogLine("Dramalord356", "npc_accusation_action", "close_window", "{=Dramalord356}Well, next time invite me too. we could have fun together!", ConditionNpcWantsToJoin, ConsequenceWhatever);
            starter.AddDialogLine("Dramalord357", "npc_accusation_action", "close_window", "{=Dramalord357}I forgive you... But never do that again!", ConditionNpcForgives, ConsequenceWhatever);
            starter.AddDialogLine("Dramalord315", "npc_accusation_action", "close_window", "{=Dramalord315}Whatever...", ConditionNpcDoesntCare, ConsequenceWhatever);
        }

        //CONDITIONS
        internal static bool ConditionNpcCanConfrontPlayer()
        {
            if(NPCConfrontation.ApproachingHero != null && NPCConfrontation.Memory != null && ConversationHelper.ConversationIntention == ConversationType.NPCConfrontation)
            {
                ConversationHelper.ConversationIntention = ConversationType.PlayerInteraction;
                MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(NPCConfrontation.ApproachingHero, Hero.MainHero, true));
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcSeesPregnancyPlayer()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Pregnancy && NPCConfrontation.Memory.Event.Hero1 == Hero.MainHero.CharacterObject)
            {
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcSeesPregnancyNpc()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Pregnancy && NPCConfrontation.Memory.Event.Hero2 == Hero.MainHero.CharacterObject)
            {
                MBTextManager.SetTextVariable("HERO", OtherHero.Name);
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcSeesBastard()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Birth)
            {
                MBTextManager.SetTextVariable("TARGET", OtherHero.Name);
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcAccusationDate()
        {
            if(NPCConfrontation.Memory.Event.Type == EventType.Date)
            {
                MBTextManager.SetTextVariable("TARGET", OtherHero.Name);
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcAccusationIntercourse()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Intercourse)
            {
                MBTextManager.SetTextVariable("TARGET", OtherHero.Name);
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcAccusationMarriage()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Marriage)
            {
                MBTextManager.SetTextVariable("TARGET", OtherHero.Name);
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcAccusationForgot()
        {
            return !ConditionNpcSeesPregnancyPlayer() && !ConditionNpcSeesPregnancyNpc() && !ConditionNpcSeesBastard() && !ConditionNpcAccusationDate() && !ConditionNpcAccusationIntercourse() && !ConditionNpcAccusationMarriage();
        }

        internal static bool ConditionNpcAccusationWitness()
        {
            if(NPCConfrontation.Memory.Type == MemoryType.Witness && (NPCConfrontation.Memory.Event.Type != EventType.Pregnancy && NPCConfrontation.Memory.Event.Type != EventType.Birth))
            {
                StringHelpers.SetCharacterProperties("TARGET", OtherHero.CharacterObject);
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcAccusationGossip()
        {
            if (!ConditionNpcAccusationDirect() && !ConditionNpcAccusationWitness())
            {
                StringHelpers.SetCharacterProperties("SOURCE", NPCConfrontation.Memory.Source);
                return true;
            }
            return false;
        }

        internal static bool ConditionNpcAccusationDirect()
        {
            if (NPCConfrontation.Memory.Type == MemoryType.Witness && (NPCConfrontation.Memory.Event.Type == EventType.Pregnancy || NPCConfrontation.Memory.Event.Type == EventType.Birth))
            {
                return true;
            }
            return false;
        }

        internal static bool ConditionPlayerGossipDoesntCare()
        {
            return NPCConfrontation.Memory.Type != MemoryType.Participant;
        }

        internal static bool ConditionPlayerGossipPlaysInnocent()
        {
            return NPCConfrontation.Memory.Type != MemoryType.Witness;
        }

        internal static bool ConditionPlayerWitnessBegsForgiveness()
        {
            MBTextManager.SetTextVariable("TITLE", ConversationHelper.GetHeroGreeting(Hero.MainHero, NPCConfrontation.ApproachingHero, true));
            return NPCConfrontation.Memory.Type == MemoryType.Witness;
        }

        internal static bool ConditionNpcWantsKillPlayer()
        {
            return NPCConfrontation.ApproachingHero.GetDramalordFeelings(Hero.MainHero).Emotion < DramalordMCM.Get.MinEmotionBeforeDivorce && 
                NPCConfrontation.ApproachingHero.GetDramalordPersonality().IsInstable && DramalordMCM.Get.AllowRageKills;
        }

        internal static bool ConditionNpcWantsLeaveClan()
        {
            return NPCConfrontation.ApproachingHero.Clan == Hero.MainHero.Clan && NPCConfrontation.ApproachingHero.GetDramalordFeelings(Hero.MainHero).Emotion < DramalordMCM.Get.MinEmotionBeforeDivorce && 
                NPCConfrontation.ApproachingHero.GetRelationTo(Hero.MainHero) <= -50 && DramalordMCM.Get.AllowClanChanges;
        }

        internal static bool ConditionNpcWantsBreakUpOrDivorce()
        {
            return NPCConfrontation.ApproachingHero.GetDramalordFeelings(Hero.MainHero).Emotion < DramalordMCM.Get.MinEmotionBeforeDivorce &&
                NPCConfrontation.ApproachingHero.GetRelationTo(Hero.MainHero) > -50 &&
                !NPCConfrontation.ApproachingHero.GetDramalordPersonality().IsInstable;
        }

        internal static bool ConditionNpcWantsToJoin()
        {
            return NPCConfrontation.ApproachingHero.GetDramalordFeelings(Hero.MainHero).Emotion > DramalordMCM.Get.MinEmotionBeforeDivorce &&
                ((NPCConfrontation.ApproachingHero.GetDramalordPersonality().AcceptsOtherIntercourse && NPCConfrontation.Memory.Event.Type == EventType.Intercourse) ||
                (NPCConfrontation.ApproachingHero.GetDramalordPersonality().AcceptsOtherRelationships && NPCConfrontation.Memory.Event.Type == EventType.Date));
        }

        internal static bool ConditionNpcForgives()
        {
            return NPCConfrontation.ApproachingHero.GetDramalordFeelings(Hero.MainHero).Emotion > DramalordMCM.Get.MinEmotionBeforeDivorce &&
                !(NPCConfrontation.ApproachingHero.GetDramalordPersonality().AcceptsOtherIntercourse && NPCConfrontation.Memory.Event.Type == EventType.Intercourse) &&
                !(NPCConfrontation.ApproachingHero.GetDramalordPersonality().AcceptsOtherRelationships && NPCConfrontation.Memory.Event.Type == EventType.Date);
        }

        internal static bool ConditionNpcDoesntCare()
        {
            return !ConditionNpcWantsKillPlayer() && !ConditionNpcWantsLeaveClan() && !ConditionNpcWantsBreakUpOrDivorce() && !ConditionNpcWantsToJoin() && !ConditionNpcForgives();
        }


        // CONSEQUENCES
        internal static void ConsequencePlayerAdmit()
        {
            NPCConfrontation.PlayerDenied = false;
        }

        internal static void ConsequencePlayerDenied()
        {
            NPCConfrontation.PlayerDenied = true;
        }

        internal static void ConsequenceNpcWantsKillPlayer()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Birth && NPCConfrontation.OtherHero.Father != NPCConfrontation.ApproachingHero && !NPCConfrontation.OtherHero.IsOrphan())
            {
                HeroPutInOrphanageAction.Apply(NPCConfrontation.ApproachingHero, NPCConfrontation.OtherHero);
            }

            if (NPCConfrontation.ApproachingHero.IsLover(Hero.MainHero))
            {
                HeroBreakupAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            }
            else if (NPCConfrontation.ApproachingHero.IsSpouse(Hero.MainHero))
            {
                HeroDivorceAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            }

            //HeroFightAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            HeroKillAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero, NPCConfrontation.OtherHero, NPCConfrontation.Memory.Event.Type);

            NPCConfrontation.ApproachingHero = null;
            NPCConfrontation.Memory = null;
            NPCConfrontation.OtherHero = null;
        }

        internal static void ConsequenceNpcWantsLeaveClan()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Birth && NPCConfrontation.OtherHero.Father != NPCConfrontation.ApproachingHero && !NPCConfrontation.OtherHero.IsOrphan())
            {
                HeroPutInOrphanageAction.Apply(NPCConfrontation.ApproachingHero, NPCConfrontation.OtherHero);
            }

            if (NPCConfrontation.ApproachingHero.IsLover(Hero.MainHero))
            {
                HeroBreakupAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            }
            else if (NPCConfrontation.ApproachingHero.IsSpouse(Hero.MainHero))
            {
                HeroDivorceAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            }
            HeroLeaveClanAction.Apply(NPCConfrontation.ApproachingHero, NPCConfrontation.ApproachingHero);
            NPCConfrontation.ApproachingHero = null;
            NPCConfrontation.Memory = null;
            NPCConfrontation.OtherHero = null;
        }

        internal static void ConsequenceNpcWantsBreakUpOrDivorce()
        {
            if (NPCConfrontation.Memory.Event.Type == EventType.Birth && NPCConfrontation.OtherHero.Father != NPCConfrontation.ApproachingHero && !NPCConfrontation.OtherHero.IsOrphan())
            {
                HeroPutInOrphanageAction.Apply(NPCConfrontation.ApproachingHero, NPCConfrontation.OtherHero);
            }

            if (NPCConfrontation.ApproachingHero.IsLover(Hero.MainHero))
            {
                HeroBreakupAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            }
            else if(NPCConfrontation.ApproachingHero.IsSpouse(Hero.MainHero))
            {
                HeroDivorceAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            }
            //HeroFightAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            NPCConfrontation.ApproachingHero = null;
            NPCConfrontation.Memory = null;
            NPCConfrontation.OtherHero = null;
        }

        internal static void ConsequenceWhatever()
        {
            //HeroFightAction.Apply(NPCConfrontation.ApproachingHero, Hero.MainHero);
            NPCConfrontation.ApproachingHero = null;
            NPCConfrontation.Memory = null;
            NPCConfrontation.OtherHero = null;
        }
    }
}
