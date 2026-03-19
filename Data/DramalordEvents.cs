using Dramalord.Conversations;
using Dramalord.Data.Events;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Data
{
    internal class DramalordEvents : DramalordData
    {
        private static DramalordEvents? _instance;

        internal static DramalordEvents Instance
        {
            get
            {
                _instance ??= new DramalordEvents();
                return _instance;
            }
        }

        private readonly List<IDramalordEvent> _events;

        private DramalordEvents() : base("DramalordEvents")
        {
            _events = new();
        }

        private IDramalordEvent? DialogEvent = null;

        internal bool StartIntention(IDramalordEvent dramaEvent, bool isReaction = false)
        {
            //NPC approaches Player
            if (DialogEvent == null && dramaEvent.Target == Hero.MainHero && dramaEvent.GetInitiationDialog() is DialogFlow flow && flow != null)
            {

                DialogEvent = dramaEvent;

                Campaign.Current.ConversationManager.AddDialogFlow(isReaction ? ReactionStartFlow(dramaEvent.Actor) : IntentionStartFlow(dramaEvent.Actor), dramaEvent);
                Campaign.Current.ConversationManager.AddDialogFlow(flow, dramaEvent);

                if (MobileParty.MainParty != null && MobileParty.MainParty.IsCurrentlyAtSea)
                {
                    CampaignMission.OpenConversationMission(
                        new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty), 
                        new ConversationCharacterData(dramaEvent.Actor.CharacterObject, PartyBase.MainParty, noBodyguards: true, isCivilianEquipmentRequiredForLeader: dramaEvent.Actor.CurrentSettlement != null, noHorse: true, noWeapon: true)

                        );
                }
                else
                {
                    CampaignMapConversation.OpenConversation(
                        new ConversationCharacterData(Hero.MainHero.CharacterObject),
                        new ConversationCharacterData(dramaEvent.Actor.CharacterObject, isCivilianEquipmentRequiredForLeader: dramaEvent.Actor.CurrentSettlement != null, noBodyguards: true, noHorse: true, noWeapon: true)
                        );
                }

                return true;
            }
            // Player starts event during conversation
            else if(DialogEvent == null && Campaign.Current.ConversationManager.IsConversationInProgress && dramaEvent.Actor == Hero.MainHero)
            {
                DialogEvent = dramaEvent;
                dramaEvent.Action(1);
                return true;
            }
            //NPC approaches NPC
            else if (dramaEvent.Target != Hero.MainHero || dramaEvent.GetInitiationDialog() == null)
            {
                dramaEvent.Action(1);
                dramaEvent.AfterDialog();
                HandleWitness(dramaEvent);
                return true;
            }

            return false;
        }

        public IDramalordEvent? GetReaction(Hero actor, Hero target, bool remove = false)
        {
            IDramalordEvent? ev = _events.FirstOrDefault(ev => ev.Actor == actor && ev.Target == target && ev is not GossiptEvent);
            if(ev != null && remove)
            {
                _events.Remove(ev);
            }
            return ev;
        }

        public void AddReaction(IDramalordEvent dramaEvent)
        {
            _events.Add(dramaEvent);
        }

        internal void OnHourlyTick()
        {
            List<Hero> heroList = new();
            List<IDramalordEvent> garbage = new();

            List<IDramalordEvent> events = _events.ToList();
            events.ForEach(ev =>
            {
                List<Hero> closeHeroes = ev.Actor.GetCloseHeroes();
                if (!heroList.Contains(ev.Actor) && (closeHeroes.Contains(ev.Target) || (ev.Target.IsChild) || ev.Actor == ev.Target))
                {
                    if(ev.Target != Hero.MainHero || ev is not GossiptEvent || (!ev.Actor.HasMetRecently(ev.Target) && !ev.Actor.IsBlockedBy(ev.Target)))
                    {
                        if (Instance.StartIntention(ev, isReaction: true))
                        {
                            heroList.Add(ev.Actor);

                            if (ev is not GossiptEvent)
                            {
                                garbage.Add(ev);
                            }
                        }
                    }
                }
                
                if(ev is LogEntry logEntry && logEntry.GameTime + logEntry.KeepInHistoryTime < CampaignTime.Now)
                {
                    garbage.Add(ev);
                }
                else if(ev is GossiptEvent gossipEvent)
                {
                    Hero? newTarget = closeHeroes.FirstOrDefault(ch => !gossipEvent.GetGossipEvent().IsKnownTo.Contains(ch));
                    if (newTarget != null)
                    {
                        _events.Add(new GossiptEvent(ev.Actor, newTarget, gossipEvent.GetGossipEvent()));
                    }
                    garbage.Add(ev);
                }
            });

            garbage.ForEach(g => _events.Remove(g));
        }

        internal void OnConversationStart(IAgent agent)
        {
            if (agent.Character != CharacterObject.PlayerCharacter)
            {
                // ConversationRelationship.GetRelationshipDialogs(instance);
            }
        }

        internal void OnConversationEnded(IEnumerable<CharacterObject> characters)
        {
            Campaign.Current.ConversationManager.RemoveRelatedLines(Instance);
            if (DialogEvent != null)
            {
                DialogEvent.AfterDialog();
                HandleWitness(DialogEvent);
                Campaign.Current.ConversationManager.RemoveRelatedLines(DialogEvent);
                Campaign.Current.ConversationManager.RemoveRelatedLines(this);
                DialogEvent = null;

                if (PlayerEncounter.Current != null)
                {
                    PlayerEncounter.LeaveEncounter = true;
                }
            }
        }

        public TextObject GetIntentionGreeting(Hero speaker) => speaker.HasMet ? ConversationTools.SetCharacterObjects(new(DramalordTexts.INTENTION_GREETING_KNOWN), Hero.MainHero) : ConversationTools.SetCharacterObjects(new(DramalordTexts.INTENTION_GREETING_UNKNOWN), Hero.MainHero);

        public TextObject GetConfrontationGreeting(Hero speaker) => speaker.HasMet ? ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_GREETING_KNOWN), Hero.MainHero) : ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_GREETING_UNKNOWN), Hero.MainHero, speaker);


        public DialogFlow IntentionStartFlow(Hero speaker) => DialogFlow.CreateDialogFlow("start", 200)
            .NpcLine(GetIntentionGreeting(speaker) + "[ib:nervous][if:convo_nervous]")
                .BeginPlayerOptions()
                    .PlayerOption(DramalordTexts.INTENTION_REACT_YES)
                        .Condition(() => ConversationTools.SetConversationHero(speaker))
                        .GotoDialogState("start_intention")
                    .PlayerOption(DramalordTexts.INTENTION_REACT_NO_TIME)
                        .Condition(() => ConversationTools.SetConversationHero(speaker))
                        .CloseDialog()
                    .PlayerOption(DramalordTexts.INTENTION_REACT_STOP)
                        .Condition(() => ConversationTools.SetConversationHero(speaker))
                        .Consequence(() => 
                        {
                            Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).SetBlockedUntil(CampaignTime.DaysFromNow(1000));
                            DramalordBanner.CreateBanner(Hero.OneToOneConversationHero, DramalordTexts.BANNER_APPROACH_STOP);
                        })
                        .CloseDialog()
                .EndPlayerOptions();

        public DialogFlow ReactionStartFlow(Hero speaker) => DialogFlow.CreateDialogFlow("start", 500)
            .NpcLine(ConversationTools.SetTextVariables(GetConfrontationGreeting(speaker), speaker.Clan != null ? speaker.Clan.EncyclopediaLinkWithName : TextObject.GetEmpty()) + "[ib:aggressive][if:convo_undecided_open]") //TODO: Get clanless name
                .GotoDialogState("start_reaction");


        public void SetNextGossipLine(Hero knownto)
        {
            if(Campaign.Current.ConversationManager.IsConversationInProgress)
            {
                Campaign.Current.ConversationManager.RemoveRelatedLines(Instance);
                if(DialogEvent != null)
                {
                    Campaign.Current.ConversationManager.RemoveRelatedLines(DialogEvent);
                }
                IDramalordEvent? ev = Campaign.Current.LogEntryHistory.GetGameActionLogs<LogEntry>(drama => drama is IDramalordEvent dr
                    && dr.Actor != knownto
                    && dr.Target != knownto
                    && dr.IsKnownTo.Contains(knownto)
                    && !dr.IsKnownTo.Contains(Hero.MainHero)
                    && DramalordQuests.Instance.GetQuest(dr.Actor) == null
                    && DramalordQuests.Instance.GetQuest(dr.Target) == null
                    && dr.GetGossipDialog(knownto) != null
                    ).Cast<IDramalordEvent>().FirstOrDefault();
                if(ev != null)
                {
                    DialogFlow? flow = ev.GetGossipDialog(knownto);
                    if(flow != null)
                    {
                        ev.IsKnownTo.Add(Hero.MainHero);
                        Campaign.Current.ConversationManager.AddDialogFlow(flow, Instance);
                        return;
                    }
                }
                DialogFlow? flow2 = DialogFlow.CreateDialogFlow("start_reaction")
                    .NpcLine(DramalordTexts.NPC_INTERACTION_GOSSIP_NONE)
                    .BeginPlayerOptions()
                        .PlayerOption(DramalordTexts.QUESTION_END)
                        .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .Consequence(() =>
                        {
                            if (PlayerEncounter.Current != null)
                            {
                                PlayerEncounter.LeaveEncounter = true;
                            }
                        })
                        .CloseDialog()
                        .PlayerOption(DramalordTexts.PLAYER_INTERACTION_END)
                        .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .NpcLine(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                            .GotoDialogState("hero_main_options")
                    .EndPlayerOptions();
                Campaign.Current.ConversationManager.AddDialogFlow(flow2, Instance);
            }
        }

        public void HandleWitness(IDramalordEvent dramaEvent)
        {
            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
            {
                List<Hero> closeHeroes = dramaEvent.Actor.GetCloseHeroes();
                Hero? witness = closeHeroes.GetRandomElementWithPredicate(h => !dramaEvent.IsKnownTo.Contains(h));
                if (witness != null)
                {
                    if (dramaEvent.CreateReaction(witness) is IDramalordEvent drama && drama != null)
                    {
                        _events.Add(drama);
                    }
                }
            }
        }

        internal override void LoadData(IDataStore dataStore)
        {
            _events.Clear();
            if(!IsOldData)
            {
                List<IDramalordEvent> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);
                _events.AddRange(data);
            }
        }

        internal override void SaveData(IDataStore dataStore)
        {
            List<IDramalordEvent> data = new();
            data.AddRange(_events);
            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _events.RemoveAll( ev => ev.Target == victim || ev.Actor == victim);
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _events.RemoveAll(ev => ev.Target == hero || ev.Actor == hero);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
           
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _events.Clear();
        }
    }
}
