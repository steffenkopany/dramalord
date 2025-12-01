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
    internal class ConfrontEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private IDramalordEvent DramaEvent;

        public TextObject GetConfrontationGreeting(Hero speaker) => speaker.HasMet ? ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_GREETING_KNOWN), Hero.MainHero) : ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_GREETING_UNKNOWN), Hero.MainHero, speaker);

        public ConfrontEvent(Hero actor, Hero target, IDramalordEvent dramaEvent)
        {
            Actor = actor;
            Target = target;
            DramaEvent = dramaEvent;
            IsKnownTo.Add(Actor);
            dramaEvent.IsKnownTo.Add(Actor);
            AddLogEntry(this);
        }

        public void Action()
        {
            DramaEvent.ReactionResult(Actor, Target, out int trust, out int love);
            Actor.ChangeRelationTo(Target, trust, love);
        }

        public void AfterDialog()
        {
            (new RelationshipEvent(Actor, Target)).Action();// DramalordEvents.Instance.StartIntention(new RelationshipEvent(Actor, Target));
            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_CONFRONTATION), Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            return null;
        }

        public DialogFlow? GetInitiationDialog()
        {
            return DramaEvent.GetConfrontationDialog(Actor);
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

        public bool IsVisibleNotification => DramalordMCM.Instance.RelationshipLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerClanNegative;
    }
}
