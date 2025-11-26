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
    internal class AdoptEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        public AdoptEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(actor);
            IsKnownTo.Add(target);
        }

        public void Action()
        {
            if(Actor.IsFemale)
            {
                Target.Mother = Actor;
                Target.Father = Actor.Spouse ?? Target.Father;
            }
            else
            {
                Target.Father = Actor;
                Target.Mother = Actor.Spouse ?? Target.Mother;
            }

            Target.SetNewOccupation(Actor.Occupation);
            Target.Clan = Actor.Clan;

            DramalordOrphans.Instance.RemoveOrphan(Target);
            Target.UpdateHomeSettlement();
        }

        public void AfterDialog()
        {
            AddLogEntry(this);
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_ADOPT), Actor, Target);
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
        public bool IsVisibleNotification => DramalordMCM.Instance.ChildrenEventLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;
    }
}
