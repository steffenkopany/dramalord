using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class ConceiveEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        public ConceiveEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(actor);
            IsKnownTo.Add(target);
        }

        public void Action(int modifier = 0)
        {
            if(Actor.IsFemale == Target.IsFemale)
            {
                return;
            }

            Hero female = Actor.IsFemale ? Actor : Target;
            Hero male = Actor.IsFemale ? Target : Actor;

            if (!female.IsFertile() || DramalordPregnancies.Instance.GetPregnancy(female) != null)
            {
                return;
            }

            DramalordPregnancies.Instance.AddPregnancy(female, male, CampaignTime.Now);
            female.IsPregnant = true;
        }

        public void AfterDialog()
        {
            Hero female = Actor.IsFemale ? Actor : Target;
            if(female.IsPregnant)
            {
                AddLogEntry(this);
            }
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            Hero female = Actor.IsFemale ? Actor : Target;
            Hero male = Actor.IsFemale ? Target : Actor;
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_CONCEIVE), female, male);
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

        public DialogFlow? GetBlackmailDialog(Hero speaker)
        {
            return null;
        }

        public void ReactionResult(Hero speaker, Hero listener, out int trust, out int love)
        {
            trust = 0;
            love = 0;
        }
        public bool IsVisibleNotification => DramalordMCM.Instance.ConceiveLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Neutral;
    }
}
