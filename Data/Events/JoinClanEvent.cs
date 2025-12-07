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
    internal class JoinClanEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableProperty(4)]
        public Clan JoinClan { get; private set; }

        public JoinClanEvent(Hero actor, Clan clan)
        {
            Actor = actor;
            Target = actor;
            JoinClan = clan;
            IsKnownTo.Add(actor);
        }

        public void Action(int modifier = 0)
        {
            if(Actor.Clan != null) return;

            if (Actor.Occupation != Occupation.Lord)
            {
                Actor.SetName(Actor.FirstName, Actor.FirstName);
            }

            Actor.SetNewOccupation(Occupation.Lord);
            Actor.Clan = JoinClan;
            Actor.UpdateHomeSettlement();

            CampaignEventDispatcher.Instance.OnHeroChangedClan(Actor, null);
        }

        public void AfterDialog()
        {
            if(JoinClan != null)
                AddLogEntry(this);
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetTextVariables(ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_JOIN_CLAN), Actor), JoinClan.EncyclopediaLinkWithName);
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
            return null;
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

        public bool IsVisibleNotification => true;

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionPolitical;
    }
}
