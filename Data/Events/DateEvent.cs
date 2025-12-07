using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class DateEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private int LoveGain;

        [SaveableField(5)]
        private int TrustGain;

        private int Modifier = 0;

        public DateEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(Actor);
            IsKnownTo.Add(Target);
        }

        public void Action(int modifier = 0)
        {
            Modifier = modifier;
            HeroDesires heroDesires = Actor.GetDesires();
            HeroDesires targetDesires = Target.GetDesires();

            int sympathy = Actor.GetSympathyTo(Target);
            int heroAttraction = Actor.GetAttractionTo(Target);
            int tagetAttraction = Target.GetAttractionTo(Actor);

            heroDesires.Horny += heroAttraction / 10;
            targetDesires.Horny += tagetAttraction / 10;

            //int attractionBonus = ((heroAttraction / 10) + (tagetAttraction / 10)) / 2;
            int attractionBonus = tagetAttraction / 10;

            if (Actor == Hero.MainHero || Target == Hero.MainHero)
            {
                LoveGain = MBMath.ClampInt(attractionBonus, 0, 100) * modifier;
                TrustGain = MBMath.ClampInt(sympathy, 0, 100) * modifier;
            }
            else
            {
                LoveGain = MBMath.ClampInt(attractionBonus, -100, 100) * modifier;
                TrustGain = MBMath.ClampInt(sympathy, -100, 100) * modifier;
            }

            Actor.ChangeRelationTo(Target, TrustGain, LoveGain);
        }

        public void AfterDialog()
        {
            if(Modifier == 0)
            {
                return;
            }

            AddLogEntry(this);

            bool relationChanged = RelationshipEvent.CheckRelationship(Actor, Target) != Actor.GetRelationTo(Target).Relationship;
            (new RelationshipEvent(Actor, Target, true)).Action();

            if (Actor != Hero.MainHero && Target != Hero.MainHero && Actor.GetDesires().Horny > 50 && Target.GetDesires().Horny > 50)
            {
                DramalordEvents.Instance.StartIntention(new SexEvent(Actor, Target));
            }
            else if ((Actor == Hero.MainHero || Target == Hero.MainHero) && TrustGain > 0 && LoveGain > 0)
            {
                Hero otherHero = (Actor == Hero.MainHero) ? Target : Actor;
                if(otherHero.GetDesires().Horny > 50 && !relationChanged)
                {
                    DramalordInquiry.CreateYesNoImageInquiry(Hero.MainHero, otherHero, new TextObject(DramalordTexts.INQUIRY_SEX_TEXT), () =>
                    {
                        SexEvent sexEvent = new SexEvent(Actor, Target);
                        sexEvent.Action();
                        sexEvent.AfterDialog();
                    }, 
                    () => { },
                    InquiryContext.AcceptSex);
                    /*
                    DramalordInquiry.CreateYesNoInquiry(otherHero, DramalordTexts.INQUIRY_SEX_TITLE, DramalordTexts.INQUIRY_SEX_TEXT, () => {
                        SexEvent sexEvent = new SexEvent(Actor, Target);
                        sexEvent.Action();
                        sexEvent.AfterDialog();
                    }, () => { });
                    */
                }
            }

            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            TextObject rtn = new(LoveGain > 0 ? DramalordTexts.LOG_DATE_LOVE : TrustGain > 0 ? DramalordTexts.LOG_DATE_FRIEND : DramalordTexts.LOG_DATE_BAD);
            return ConversationTools.SetCharacterObjects(rtn, Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }


        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            return DialogFlow.CreateDialogFlow("start_reaction")
            .BeginNpcOptions()
                .NpcOption(DramalordTexts.CONFRONTATION_DATE_OTHER + "[ib:warrior][if:convo_grave]", () => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Actor == Hero.MainHero ? Target : Actor)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : Actor))
                    .Consequence(() => {
                        ReactionResult(speaker, Hero.MainHero, out int trust, out int love);
                        speaker.ChangeRelationTo(Hero.MainHero, trust, love);
                    })
                    .NpcLine(DramalordTexts.CONFRONTATION_RESULT_NO_RELATION)
                        .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                        .CloseDialog()
                .NpcOption(DramalordTexts.CONFRONTATION_DATE_PLAYER + "[ib:warrior][if:convo_grave]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Hero.MainHero)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : Actor))
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
                    .NpcLine(ConversationTools.SetCharacterObjects(new(DramalordTexts.GOSSIP_DATE), Actor, Target))
                    .Consequence(() =>
                    {
                        if ((Hero.MainHero.IsEmotionalWith(Actor) || Hero.MainHero.IsEmotionalWith(Target)))
                        {
                            Hero cheater = Hero.MainHero.IsEmotionalWith(Actor) ? Actor : Target;
                            Hero other = cheater == Actor ? Target : Actor;
                            if (DramalordQuests.Instance.GetQuest(cheater) == null)
                            {
                                DramalordInquiry.CreateYesNoInquiry(Hero.MainHero, DramalordTexts.INQUIRY_CONFRONT_TITLE, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_TEXT), cheater, other).ToString(), () =>
                                {
                                    ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                    DramalordQuests.Instance.AddQuest(cheater, quest);
                                    quest.StartQuest();

                                }, () => { });
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
            trust = speaker.GetPersonality().Empathy > 1 ? -100 + (int)(speaker.GetPersonality().Empathy) : -1;
            love = speaker.GetPersonality().Jealousy > 1 ? (int)(speaker.GetPersonality().Jealousy * -0.5) : -1;
        }

        public DialogFlow? GetInitiationDialog()
        {
            return DialogFlow.CreateDialogFlow("start_intention")
                .BeginNpcOptions()
                    .NpcOptionWithVariation(DramalordTexts.INTENTION_DATE_FIRST_1 + "[ib:nervous][if:convo_merry]", () => !Actor.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Variation(DramalordTexts.INTENTION_DATE_FIRST_2 + "[ib:nervous][if:convo_merry]")
                            .BeginPlayerOptions()
                                .PlayerOption(DramalordTexts.INTENTION_DATE_FIRST_REACT_OK)
                                    .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                    .Consequence(() => ConversationQuestions.SetupQuestions(this, true))
                                    .GotoDialogState("start_challenge")
                                .PlayerOption(DramalordTexts.INTENTION_REACT_NO_INTEREST)
                                    .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                    .CloseDialog()
                            .EndPlayerOptions()
                    .NpcOptionWithVariation(DramalordTexts.INTENTION_DATE_LOVER_1 + "[ib:confident2][if:convo_focused_happy]", () => Actor.IsEmotionalWith(Hero.MainHero) && (Actor.Spouse == Hero.MainHero || Actor.Spouse == null) && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Variation(DramalordTexts.INTENTION_DATE_LOVER_2 + "[ib:confident2][if:convo_focused_happy]")
                            .BeginPlayerOptions()
                                .PlayerOption(DramalordTexts.INTENTION_DATE_REACT_OK)
                                    .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                    .Consequence(() => ConversationQuestions.SetupQuestions(this, true))
                                    .GotoDialogState("start_challenge")
                                .PlayerOption(DramalordTexts.INTENTION_REACT_NO_INTEREST)
                                    .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                    .CloseDialog()
                            .EndPlayerOptions()
                    .NpcOptionWithVariation(DramalordTexts.INTENTION_DATE_CHEATING_1 + "[ib:confident2][if:convo_focused_happy]", () => Actor.IsEmotionalWith(Hero.MainHero) && Actor.Spouse != Hero.MainHero && Actor.Spouse != null && ConversationTools.SetConversationHero(Hero.MainHero, Actor.Spouse))
                        .Variation(DramalordTexts.INTENTION_DATE_CHEATING_2 + "[ib:confident2][if:convo_focused_happy]")
                            .BeginPlayerOptions()
                                .PlayerOption(DramalordTexts.INTENTION_DATE_REACT_OK)
                                    .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                    .Consequence(() => ConversationQuestions.SetupQuestions(this, true))
                                    .GotoDialogState("start_challenge")
                                .PlayerOption(DramalordTexts.INTENTION_REACT_NO_INTEREST)
                                    .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                    .CloseDialog()
                            .EndPlayerOptions()
                .EndNpcOptions();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            if (hero == Hero.MainHero)
            {
                IsKnownTo.Add(Hero.MainHero);
                if (hero.IsEmotionalWith(Actor) || hero.IsEmotionalWith(Target))
                {
                    Hero cheater = hero.IsEmotionalWith(Actor) ? Actor : Target;
                    Hero other = cheater == Actor ? Target : Actor;
                    DramalordInquiry.CreateYesNoInquiry(Hero.MainHero, DramalordTexts.INQUIRY_CONFRONT_TITLE, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_TEXT), cheater, other).ToString(), () =>
                    {
                        if (DramalordQuests.Instance.GetQuest(cheater) == null)
                        {
                            ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                            DramalordQuests.Instance.AddQuest(cheater, quest);
                            quest.StartQuest();
                        }
                    }, () => { });
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

        public bool IsVisibleNotification => DramalordMCM.Instance.DatingLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionCivilian;
    }
}
