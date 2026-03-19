using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Notifications;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class GossiptEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new ();

        [SaveableField(4)]
        private IDramalordEvent DramaEvent;

        public IDramalordEvent GetGossipEvent() => DramaEvent;

        public TextObject GetConfrontationGreeting(Hero speaker) => speaker.HasMet ? ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_GREETING_KNOWN), Hero.MainHero) : ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_GREETING_UNKNOWN), Hero.MainHero, speaker);

        public GossiptEvent(Hero actor, Hero target, IDramalordEvent dramaEvent)
        {
            Actor = actor;
            Target = target;
            DramaEvent = dramaEvent;
            dramaEvent.IsKnownTo.Add(Actor);
        }

        public void Action(int modifier = 0)
        {
            AddLogEntry(this);
            DramaEvent.IsKnownTo.Add(Target);
        }

        public void AfterDialog()
        {
            
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_GOSSIP), Actor, Target, DramaEvent.Actor, DramaEvent.Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            if(hero != Hero.MainHero && !DramaEvent.IsKnownTo.Contains(hero))
            {
                return DramaEvent.CreateReaction(hero);
            }
            return null;
        }

        public DialogFlow? GetInitiationDialog()
        {
            return DramaEvent.GetGossipDialog(Actor);
        }

        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            return null;
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

        public bool IsVisibleNotification => DramalordMCM.Instance.GossipLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Neutral;
    }
}
