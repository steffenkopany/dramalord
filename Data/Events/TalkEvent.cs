using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class TalkEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private int TrustGain;

        private readonly int Modifier;

        public TalkEvent(Hero actor, Hero target, int modifier = 1)
        {
            Actor = actor;
            Target = target;
            Modifier = modifier;
            IsKnownTo.Add(Actor);
            IsKnownTo.Add(Target);
        }

        public void Action()
        {
            int sympathy = Actor.GetSympathyTo(Target);

            TrustGain = MBMath.ClampInt(sympathy, 0, 100) * Modifier;
            Actor.ChangeRelationTo(Target, TrustGain, 0);
        }

        public void AfterDialog()
        {
            AddLogEntry(this);
            DramalordEvents.Instance.StartIntention(new RelationshipEvent(Actor, Target));
            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(TrustGain > 0 ? new(DramalordTexts.LOG_TALK_GOOD) : new(DramalordTexts.LOG_TALK_BAD), Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            return null;
        }

        public DialogFlow GetInitiationDialog()
        {
            return DialogFlow.CreateDialogFlow("start_intention")
                .NpcLineWithVariation(DramalordTexts.INTENTION_TALK_1 + "[ib:normal2][if:convo_calm_friendly]")
                            .Variation(DramalordTexts.INTENTION_TALK_2 + "[ib:normal2][if:convo_calm_friendly]")
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.INTENTION_TALK_REACT_OK)
                                .Condition(() => ConversationTools.SetConversationHero(Actor))
                                .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.QuestionType.Talk, 1, true))
                                .GotoDialogState("start_challenge")
                            .PlayerOption(DramalordTexts.INTENTION_REACT_NO_INTEREST)
                                .Condition(() => ConversationTools.SetConversationHero(Actor))
                                .CloseDialog()
                        .EndPlayerOptions();
        }

        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            return null;
        }

        public DialogFlow? GetGossipDialog(Hero speaker)
        {
            return null;
        }

        public void ReactionResult(Hero speaker, Hero listener, out int trust, out int love)
        {
            trust = 0;
            love = 0;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance.TalkLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Neutral;
    }
}
