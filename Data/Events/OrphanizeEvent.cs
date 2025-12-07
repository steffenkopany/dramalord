using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Notifications;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class OrphanizeEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        public OrphanizeEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(actor);
            IsKnownTo.Add(target);
        }

        public void Action(int modifier = 0)
        {
            Hero father = Target.Father;
            Hero mother = Target.Mother;

            father.Children.Remove(Target);
            mother.Children.Remove(Target);
            Target.Clan = null;

            if (Target.BornSettlement == null)
            {
                Target.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }

            Target.ChangeState(Hero.CharacterStates.Disabled);
            Target.SetNewOccupation(Occupation.NotAssigned);
            Target.UpdateHomeSettlement();

            DramalordOrphans.Instance.AddOrphan(Target);
        }

        public void AfterDialog()
        {
            AddLogEntry(this);
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_ORPHANIZE), Actor, Target);
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

        public bool IsVisibleNotification => DramalordMCM.Instance.ChildrenEventLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target?.Father == Hero.MainHero || Target?.Mother == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;
    }
}
