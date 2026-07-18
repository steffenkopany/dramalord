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
    internal class BlackmailEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private IDramalordEvent DramaEvent;

        public BlackmailEvent(Hero actor, Hero target, IDramalordEvent dramaEvent)
        {
            Actor = actor;
            Target = target;
            DramaEvent = dramaEvent;
            IsKnownTo.Add(Actor);
            dramaEvent.IsKnownTo.Add(Actor);
        }

        public void Action(int modifier = 0)
        {

            AddLogEntry(this);
        }

        public void AfterDialog()
        {
            
        }

        public bool IsVisibleInEncyclopediaPageOf(MBObjectBase obj)
        {
            return IsVisibleNotification &&
                   (Actor == obj || Target == obj);
        }

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_BLACKMAIL_TITLE), Actor);
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
            return DramaEvent.GetBlackmailDialog(Actor);
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

        public bool IsVisibleNotification => DramalordMCM.Instance.RelationshipLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionNegative;
    }
}
