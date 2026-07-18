using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class BreakUpEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private bool BrokeUp;

        public BreakUpEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            BrokeUp = false;
            IsKnownTo.Add(Actor);
            IsKnownTo.Add(Target);
        }

        public void Action(int modifier = 1)
        {
            BrokeUp = true;
        }

        public void AfterDialog()
        {
            if(BrokeUp)
            {
                AddLogEntry(this);

                if(Actor.GetRelationTo(Target).Love > 0)
                {
                    Actor.ChangeRelationTo(Target, 0, Actor.GetRelationTo(Target).Love * -1);
                }

                (new RelationshipEvent(Actor, Target)).Action();
                Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
            }
            else
            {
                if (Actor.GetRelationTo(Target).Love <= 0)
                {
                    Actor.ChangeRelationTo(Target, 0, DramalordMCM.Instance.MinDatingLove - Actor.GetRelationTo(Target).Love);
                }

                foreach (var item in Actor.GetAllRelations())
                {
                    //cleanup
                    if (item.Key != Target && (item.Value.Relationship == RelationshipType.Lover || item.Value.Relationship == RelationshipType.Spouse))
                    {
                        Actor.ChangeRelationTo(item.Key, 0, DramalordMCM.Instance.MinDatingLove *-1 );
                    }
                }
            }
        }

        public bool IsVisibleInEncyclopediaPageOf(MBObjectBase obj)
        {
            return IsVisibleNotification &&
                   (Actor == obj || Target == obj);
        }

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_TALK_BAD), Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public DialogFlow? GetInitiationDialog()
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("resolve_breakup")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.PLAYER_INTERACTION_BREAK_UP_CONFIRM + "[ib:closed][if:convo_bored]", () => !ConversationPersuasions.Success)
                        .Consequence(() => BrokeUp = true)
                        .CloseDialog()
                    .NpcOption(DramalordTexts.PLAYER_INTERACTION_BREAK_UP_CONTINUE + "[ib:normal2][if:convo_calm_friendly]", () => ConversationPersuasions.Success && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() =>
                        {
                            BrokeUp = false;
                            ConversationPersuasions.Success = false;
                        })
                        .CloseDialog()
                .EndNpcOptions();
                
            Campaign.Current.ConversationManager.AddDialogFlow(flow, this);

            return DialogFlow.CreateDialogFlow("start_reaction")
                .NpcLine(DramalordTexts.PLAYER_INTERACTION_BREAK_UP + "[ib:warrior][if:convo_grave]")
                    .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.PERSUASION_DATE_2)
                                .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForBreakUp())
                                .GotoDialogState("npc_persuasion_challenge")
                            .PlayerOption(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                                .Condition(() => ConversationTools.SetConversationHero(Actor))
                                .GotoDialogState("resolve_breakup")
                        .EndPlayerOptions();

        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            return null;
        }

        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("resolve_breakup")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.PLAYER_INTERACTION_BREAK_UP_CONFIRM + "[ib:closed][if:convo_bored]", () => !ConversationPersuasions.Success)
                        .Consequence(() => BrokeUp = true)
                        .CloseDialog()
                    .NpcOption(DramalordTexts.PLAYER_INTERACTION_BREAK_UP_CONTINUE + "[ib:normal2][if:convo_calm_friendly]", () => ConversationPersuasions.Success && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() =>
                        {
                            BrokeUp = false;
                            ConversationPersuasions.Success = false;
                        })
                        .CloseDialog()
                .EndNpcOptions();
            Campaign.Current.ConversationManager.AddDialogFlow(flow, this);

            return DialogFlow.CreateDialogFlow("start_reaction")
               .NpcLine(DramalordTexts.PLAYER_INTERACTION_BREAK_UP + "[ib:warrior][if:convo_grave]")
                    .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                       .BeginPlayerOptions()
                           .PlayerOption(DramalordTexts.PERSUASION_DATE_2)
                               .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForBreakUp())
                               .GotoDialogState("npc_persuasion_challenge")
                           .PlayerOption(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                               .Condition(() => ConversationTools.SetConversationHero(speaker))
                                .GotoDialogState("resolve_breakup")
                       .EndPlayerOptions();
        }

        public DialogFlow? GetGossipDialog(Hero speaker)
        {
            return null;
        }

        public DialogFlow? GetBlackmailDialog(Hero speaker)
        {
            return null;
        }

        public void ReactionResult(Hero speaker, Hero listener, out int trust, out int love)
        {
            trust = 0;
            love = 0;
        }
        public bool IsVisibleNotification => DramalordMCM.Instance.RelationshipLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerClanNegative;
    }
}
