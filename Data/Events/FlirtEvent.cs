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
    internal class FlirtEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private int LoveGain;

        public FlirtEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(Actor);
            IsKnownTo.Add(Target);
        }

        public void Action(int modifier = 1)
        {
            HeroDesires heroDesires = Actor.GetDesires();
            HeroDesires targetDesires = Target.GetDesires();

            //int sympathy = Actor.GetSympathyTo(Target);
            int heroAttraction = Actor.GetAttractionTo(Target);
            int tagetAttraction = Target.GetAttractionTo(Actor);

            heroDesires.Horny += heroAttraction / 10;
            targetDesires.Horny += tagetAttraction / 10;

            if (Actor == Hero.MainHero || Target == Hero.MainHero)
            {
                int attractionBonus = tagetAttraction / 10;
                LoveGain = MBMath.ClampInt(attractionBonus, 0, 100) * modifier;
            }
            else
            {
                int attractionBonus = ((heroAttraction / 10) + (tagetAttraction / 10)) / 2;
                LoveGain = MBMath.ClampInt(attractionBonus, -100, 100) * modifier;
            }
            

            Actor.ChangeRelationTo(Target, 0, LoveGain);
        }

        public void AfterDialog()
        {
            AddLogEntry(this);
            (new RelationshipEvent(Actor, Target)).Action();
            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf(MBObjectBase obj)
        {
            return IsVisibleNotification &&
                   (Actor == obj || Target == obj);
        }

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(LoveGain >= 0 ? new(DramalordTexts.LOG_FLIRT_GOOD) : new(DramalordTexts.LOG_FLIRT_BAD), Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public DialogFlow? GetInitiationDialog()
        {
            return DialogFlow.CreateDialogFlow("start_intention")
                .NpcLineWithVariation(DramalordTexts.INTENTION_FLIRT_1 + "[ib:nervous][if:convo_mocking_teasing]")
                            .Variation(DramalordTexts.INTENTION_FLIRT_2 + "[ib:nervous][if:convo_mocking_teasing]")
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.INTENTION_FLIRT_REACT_OK)
                                .Condition(() => ConversationTools.SetConversationHero(Actor))
                                .Consequence(() => ConversationQuestions.SetupQuestions(this, true))
                                .GotoDialogState("start_challenge")
                            .PlayerOption(DramalordTexts.INTENTION_REACT_NO_INTEREST)
                                .Condition(() => ConversationTools.SetConversationHero(Actor))
                                .CloseDialog()
                        .EndPlayerOptions();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
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
        public bool IsVisibleNotification => DramalordMCM.Instance.FlirtingLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Neutral;
    }
}
