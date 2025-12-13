using Dramalord.Behaviors;
using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class SexEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private int LoveGain;

        public SexEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(actor);
            IsKnownTo.Add(target);
        }

        public void Action(int modifier = 0)
        {
            HeroDesires heroDesires = Actor.GetDesires();
            HeroDesires targetDesires = Target.GetDesires();

            heroDesires.Horny = 0;
            targetDesires.Horny = 0;
        }

        public void AfterDialog()
        {
            AddLogEntry(this);

            if (Actor == Hero.MainHero || Target == Hero.MainHero)
            {
                if (DramalordCampaignBehavior.HotButterFound && DramalordMCM.Instance.ShowHotButter)
                {
                    MBInformationManager.ShowSceneNotification(new HotButterNotification(Actor, Target, Actor.CurrentSettlement));
                }
                else if (DramalordMCM.Instance.ShowDramaVideos)
                {
                    DramalordVideoNotification.ShowDramalordVideoNotification(Actor, Target, null, DramalordVideoNotification.VideoContext.Intercourse);
                }
                else
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, DramalordTexts.BANNER_SEX, true);
                }
            }

            if (!CampaignOptions.IsLifeDeathCycleDisabled && Actor.IsFemale != Target.IsFemale && MBRandom.RandomInt(1, 100) <= DramalordMCM.Instance.PregnancyChance && DramalordPregnancies.Instance.GetPregnancy(Actor) == null)
            {
                DramalordEvents.Instance.StartIntention(new ConceiveEvent(Actor, Target));
            }
            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(LoveGain > 0 ? new(DramalordTexts.LOG_SEX_GOOD) : new(DramalordTexts.LOG_SEX_BAD), Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            if (hero == Hero.MainHero)
            {
                IsKnownTo.Add(Hero.MainHero);
                if(hero.IsEmotionalWith(Actor) || hero.IsEmotionalWith(Target))
                {
                    Hero cheater = hero.IsEmotionalWith(Actor) ? Actor : Target;
                    Hero other = cheater == Actor ? Target : Actor;
                    if (DramalordQuests.Instance.GetQuest(cheater) == null)
                    {
                        DramalordInquiry.CreateYesNoImageInquiry(cheater, other, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_TEXT), cheater, other), () =>
                        {
                            if (DramalordQuests.Instance.GetQuest(cheater) == null)
                            {
                                ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                DramalordQuests.Instance.AddQuest(cheater, quest);
                                quest.StartQuest();
                            }
                        },
                            () => { },
                            InquiryContext.Confront
                         );
                    }
                }

                return null;
            }
            else
            {
                if (hero.IsEmotionalWith(Actor) && hero.GetPersonality().Jealousy > 0)
                {
                    return new ConfrontEvent(hero, Actor, this);
                }
                if (hero.IsEmotionalWith(Target) && hero.GetPersonality().Jealousy > 0)
                {
                    return new ConfrontEvent(hero, Target, this);
                }

                Hero h = hero.GetCloseHeroes().GetRandomElementWithPredicate(h => !IsKnownTo.Contains(h)); ;
                if (h != null && Actor.Spouse != Target)
                {
                    return new GossiptEvent(hero, h, this);
                }
            }
            
            return null;
        }

        public DialogFlow? GetInitiationDialog()
        {
            return null;
        }

        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            return DialogFlow.CreateDialogFlow("start_reaction")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.CONFRONTATION_SEX_OTHER + "[ib:warrior][if:convo_grave]", () => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Actor == Hero.MainHero ? Target : Actor)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : Actor))
                        .Consequence(() => {
                            ReactionResult(speaker, Hero.MainHero, out int trust, out int love);
                            speaker.ChangeRelationTo(Hero.MainHero, trust, love);
                        })
                        .NpcLine(DramalordTexts.CONFRONTATION_RESULT_NO_RELATION)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                            .CloseDialog()
                    .NpcOption(DramalordTexts.CONFRONTATION_SEX_PLAYER + "[ib:warrior][if:convo_grave]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Hero.MainHero)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : Actor))
                        .Consequence(() => {
                            ReactionResult(speaker, Hero.MainHero, out int trust, out int love);
                            speaker.ChangeRelationTo(Hero.MainHero, trust, love);
                        })
                        .BeginNpcOptions()
                            .NpcOption(DramalordTexts.CONFRONTATION_RESULT_BREAKUP + "[ib:warrior][if:convo_furious]", () => RelationshipEvent.CheckRelationship(speaker, Hero.MainHero) != speaker.GetRelationTo(Hero.MainHero).Relationship && ConversationTools.SetConversationHero(Hero.MainHero))
                                .CloseDialog()
                            .NpcOption(DramalordTexts.CONFRONTATION_RESULT_OK, () => RelationshipEvent.CheckRelationship(speaker, Hero.MainHero) == speaker.GetRelationTo(Hero.MainHero).Relationship && ConversationTools.SetConversationHero(Hero.MainHero))
                                .CloseDialog()
                        .EndNpcOptions()
                .EndNpcOptions();
        }

        public DialogFlow? GetGossipDialog(Hero speaker)
        {
            return DialogFlow.CreateDialogFlow("start_reaction")
                    .NpcLine(ConversationTools.SetCharacterObjects(new(DramalordTexts.GOSSIP_SEX), Actor, Target))
                    .Consequence(() =>
                    {
                        if ((Hero.MainHero.IsEmotionalWith(Actor) || Hero.MainHero.IsEmotionalWith(Target)))
                        {
                            Hero cheater = Hero.MainHero.IsEmotionalWith(Actor) ? Actor : Target;
                            Hero other = cheater == Actor ? Target : Actor;
                            if (DramalordQuests.Instance.GetQuest(cheater) == null)
                            {
                                DramalordInquiry.CreateYesNoImageInquiry(cheater, other, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_TEXT), cheater, other), () =>
                                {
                                    if (DramalordQuests.Instance.GetQuest(cheater) == null)
                                    {
                                        ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                        DramalordQuests.Instance.AddQuest(cheater, quest);
                                        quest.StartQuest();
                                    }
                                },
                                    () => { },
                                    InquiryContext.Confront
                                 );
                            }
                        }
                    })
                    .BeginPlayerOptions()
                        .PlayerOption(DramalordTexts.PLAYER_INTERACTION_GOSSIP_MORE)
                            .Consequence(() => { IsKnownTo.Add(Hero.MainHero); DramalordEvents.Instance.SetNextGossipLine(Hero.OneToOneConversationHero); })
                            .GotoDialogState("start_reaction")
                        .PlayerOption(DramalordTexts.QUESTION_END)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                            .Consequence(() => IsKnownTo.Add(Hero.MainHero))
                            .CloseDialog()
                    .EndPlayerOptions();
        }

        public void ReactionResult(Hero speaker, Hero listener, out int trust, out int love)
        {
            trust = speaker.GetPersonality().Empathy > 1 ? -100 + (int)(speaker.GetPersonality().Empathy * 0.50) : -1;
            love = speaker.GetPersonality().Jealousy > 1 ? (int)(speaker.GetPersonality().Jealousy * -0.75) : -1;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance.SexLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionCivilian;
    }
}
