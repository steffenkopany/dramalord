using Dramalord.Behaviors;
using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class PrisonSexEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        public PrisonSexEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(actor);
            IsKnownTo.Add(target);
        }

        public void Action()
        {
            HeroDesires heroDesires = Actor.GetDesires();
            HeroDesires targetDesires = Target.GetDesires();

            int loveGain = -100 + targetDesires.Horny;
            int trustGain = -100 + heroDesires.Horny;

            heroDesires.Horny = 0;
            targetDesires.Horny = 0;

            Actor.ChangeRelationTo(Target, trustGain, loveGain);
        }

        public void AfterDialog()
        {
            AddLogEntry(this);
            DramalordEvents.Instance.StartIntention(new RelationshipEvent(Actor, Target));

            if (Actor == Hero.MainHero || Target == Hero.MainHero)
            {
                if (DramalordCampaignBehavior.HotButterFound)
                {
                    MBInformationManager.ShowSceneNotification(new HotButterNotification(Actor, Target, Actor.CurrentSettlement));
                }
                else
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, DramalordTexts.BANNER_SEX, true);
                }
            }

            if (!CampaignOptions.IsLifeDeathCycleDisabled && Actor.IsFemale != Target.IsFemale && MBRandom.RandomInt(1, 100) <= DramalordMCM.Instance.PregnancyChance)
            {
                DramalordEvents.Instance.StartIntention(new ConceiveEvent(Actor, Target));
            }

            EndCaptivityAction.ApplyByRansom(Target, Actor);

            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_SEX_PRISON), Actor, Target);
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
            return DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine(DramalordTexts.INTENTION_SEX_PRISON_1 + "[ib:confident][if:convo_mocking_teasing]")
                    .NpcLine(DramalordTexts.INTENTION_SEX_PRISON_2 + "[ib:aggressive][if:convo_excited]")
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.INTENTION_SEX_PRISON_REACT_OK)
                                .Consequence(() => { Action(); AfterDialog(); })
                                .CloseDialog()
                            .PlayerOption(DramalordTexts.INTENTION_SEX_PRISON_REACT_NO)
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

        public bool IsVisibleNotification => DramalordMCM.Instance.SexLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionNegative;
    }
}
