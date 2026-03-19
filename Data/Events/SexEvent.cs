using Dramalord.Behaviors;
using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
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

        public override CampaignTime KeepInHistoryTime => CampaignTime.Days(14f);

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

            LoveGain = modifier;

            heroDesires.Horny = 0;
            targetDesires.Horny = 0;
        }

        public void AfterDialog()
        {
            if(LoveGain > 0)
            {
                AddLogEntry(this);

                if (Actor == Hero.MainHero || Target == Hero.MainHero)
                {
                    if (DramalordCampaignBehavior.HotScenesFound && DramalordMCM.Instance.ShowHotScenes)
                    {
                        MBInformationManager.ShowSceneNotification(new HotScenesNotificationData(Actor, Target));
                    }
                    else if (DramalordCampaignBehavior.HotButterFound && DramalordMCM.Instance.ShowHotButter)
                    {
                        MBInformationManager.ShowSceneNotification(new HotButterNotification(Actor, Target, Actor.CurrentSettlement));
                    }
                    else if (DramalordMCM.Instance.ShowDramaVideos)
                    {
                        if (LoveGain == 10 && Actor.IsFemale && Target == Hero.MainHero && MBRandom.RandomInt(1, 100) <= 10)
                        {
                            TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_SEX_GOOD), Actor, Target);
                            DramalordVideoNotification.ShowDramalordVideoNotification(Actor, Target, txt, DramalordVideoNotification.VideoContext.Itch, DramalordVideoNotification.VideoSound.RomanticChime);
                            MBInformationManager.AddNotice(new DramalordEventNotification(this, GetEncyclopediaText()));
                        }
                        else
                        {
                            TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_SEX_GOOD), Actor, Target);
                            DramalordVideoNotification.ShowDramalordVideoNotification(Actor, Target, txt, DramalordVideoNotification.VideoContext.Intercourse, DramalordVideoNotification.VideoSound.RomanticChime);
                            MBInformationManager.AddNotice(new DramalordEventNotification(this, GetEncyclopediaText()));
                        }
                    }
                    else
                    {
                        DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, DramalordTexts.BANNER_SEX, true);
                    }
                    
                }

                int chance = (LoveGain < 10) ? DramalordMCM.Instance.PregnancyChance * LoveGain : DramalordMCM.Instance.PregnancyChance;
                Hero female = Actor.IsFemale ? Actor : Target;
                Hero male = female == Actor ? Target : Actor;
                if (!CampaignOptions.IsLifeDeathCycleDisabled && female.IsFemale != male.IsFemale && female.IsFertile() && male.IsFertile() && MBRandom.RandomInt(1, 100) <= chance && DramalordPregnancies.Instance.GetPregnancy(female) == null)
                {
                    DramalordEvents.Instance.StartIntention(new ConceiveEvent(female, male));
                }
            }
            else
            {
                int loveLoss = (Actor.GetRelationTo(Target).Love / 4) * -1;
                if (DramalordMCM.Instance.ShowDramaImages)
                {
                    DramalordImageNotification.ShowDramalordImageNotification(Actor == Hero.MainHero ? Target : Actor, Hero.MainHero, null, DramalordImageNotification.ImageContext.Masturbation);
                }
                else
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, ConversationTools.SetConversationText(new TextObject(DramalordTexts.BANNER_MASTURBATION), ConversationTools.FormatNumber(loveLoss)).ToString(), true);
                    Actor.ChangeRelationTo(Target, 0, loveLoss);
                }
                (Actor == Hero.MainHero ? Target : Actor).GetDesires().Horny = 0;
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
            if(LoveGain == 0)
            {
                return null;
            }
            if (Actor.IsPlayerSpouse() && Target.IsPlayerSpouse())
            {
                return null;
            }
            else if (hero.IsPlayerSpouse())
            {
                if ((Actor == Hero.MainHero && Target.IsPlayerSpouse()) || (Actor.IsPlayerSpouse() && Target == Hero.MainHero))
                {
                    return null;
                }
            }

            if (hero == Hero.MainHero)
            {
                IsKnownTo.Add(Hero.MainHero);
                if(hero.IsEmotionalWith(Actor) || hero.IsEmotionalWith(Target))
                {
                    Hero cheater = hero.IsEmotionalWith(Actor) ? Actor : Target;
                    Hero other = (cheater == Actor) ? Target : Actor;
                    if (DramalordQuests.Instance.GetQuest(cheater) == null)
                    {
                        if (DramalordMCM.Instance.ShowDramaImages)
                        {
                            DramalordInquiry.CreateYesNoImageInquiry(cheater, other, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_SEX_TEXT), cheater, other), () =>
                            {
                                if (DramalordQuests.Instance.GetQuest(cheater) == null)
                                {
                                    ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                    DramalordQuests.Instance.AddQuest(cheater, quest);
                                    quest.StartQuest();
                                }
                            },
                                () => { },
                                InquiryContext.ConfrontSex
                             );
                        }
                        else
                        {
                            DramalordInquiry.CreateYesNoInquiry(cheater, DramalordTexts.INQUIRY_CONFRONT_TITLE, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_SEX_TEXT), cheater, other).ToString(), () =>
                            {
                                if (DramalordQuests.Instance.GetQuest(cheater) == null)
                                {
                                    ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                    DramalordQuests.Instance.AddQuest(cheater, quest);
                                    quest.StartQuest();
                                    MBInformationManager.AddNotice(new DramalordQuestNotification(quest));
                                }
                            },
                                () => { }
                            );
                        }
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

                if (hero.GetTraitLevel(DefaultTraits.Generosity) < 0 && hero.GetTraitLevel(DefaultTraits.Honor) < 0 && Hero.MainHero.Spouse != null && ((Actor == Hero.MainHero && !Target.IsPlayerSpouse()) || (!Actor.IsPlayerSpouse() && Target == Hero.MainHero)))
                {
                    if (DramalordQuests.Instance.GetQuest(hero) == null)
                    {
                        return new BlackmailEvent(hero, Actor == Hero.MainHero ? Actor : Target, this);
                    }
                }

                Hero h = hero.GetCloseHeroes().GetRandomElementWithPredicate(h => !IsKnownTo.Contains(h)); ;
                if (h != null && Actor.Spouse != Target && ((Actor == Hero.MainHero && Target.IsPlayerSpouse()) || (Actor.IsPlayerSpouse() && Target == Hero.MainHero)))
                {
                    return new GossiptEvent(hero, h, this);
                }
            }
            
            return null;
        }

        public DialogFlow? GetInitiationDialog()
        {
            return DialogFlow.CreateDialogFlow("start_intention")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.INTENTION_SEX_1 + "[ib:nervous][if:convo_mocking_teasing]", () => ConversationTools.SetConversationHero(Hero.MainHero))
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.INTENTION_SEX_REACT_OK)
                                .NpcLine(DramalordTexts.NPC_INTERACTION_SEX_OK)
                                    .Consequence(() => Action(10))
                                    .CloseDialog()
                            .PlayerOption(DramalordTexts.INTENTION_SEX_REACT_NO)
                                .NpcLine(DramalordTexts.NPC_INTERACTION_ASYOUWISH + "[ib:closed][if:convo_confused_annoyed]")
                                    .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                                    .CloseDialog()
                        .EndPlayerOptions()
                .EndNpcOptions();
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
                            Hero other = (cheater == Actor) ? Target : Actor;
                            if (DramalordQuests.Instance.GetQuest(cheater) == null)
                            {
                                if (DramalordMCM.Instance.ShowDramaImages)
                                {
                                    DramalordInquiry.CreateYesNoImageInquiry(cheater, other, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_SEX_TEXT), cheater, other), () =>
                                    {
                                        if (DramalordQuests.Instance.GetQuest(cheater) == null)
                                        {
                                            ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                            DramalordQuests.Instance.AddQuest(cheater, quest);
                                            quest.StartQuest();
                                            MBInformationManager.AddNotice(new DramalordQuestNotification(quest));
                                        }
                                    },
                                        () => { },
                                        InquiryContext.ConfrontSex
                                     );
                                }
                                else
                                {
                                    DramalordInquiry.CreateYesNoInquiry(cheater, DramalordTexts.INQUIRY_CONFRONT_TITLE, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_SEX_TEXT), cheater, other).ToString(), () =>
                                    {
                                        if (DramalordQuests.Instance.GetQuest(cheater) == null)
                                        {
                                            ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                            DramalordQuests.Instance.AddQuest(cheater, quest);
                                            quest.StartQuest();
                                            MBInformationManager.AddNotice(new DramalordQuestNotification(quest));
                                        }
                                    },
                                () => { }
                            );
                                }
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
                        .PlayerOption(DramalordTexts.INTENTION_REACT_STOP)
                            .Condition(() => ConversationTools.SetConversationHero(speaker))
                            .Consequence(() =>
                            {
                                IsKnownTo.Add(Hero.MainHero);
                                Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).SetBlockedUntil(CampaignTime.DaysFromNow(1000));
                                DramalordBanner.CreateBanner(Hero.OneToOneConversationHero, DramalordTexts.BANNER_APPROACH_STOP);
                            })
                            .CloseDialog()
                    .EndPlayerOptions();
        }

        public DialogFlow? GetBlackmailDialog(Hero speaker)
        {
            Hero other = Actor == Hero.MainHero ? Target : Actor;
            return DialogFlow.CreateDialogFlow("start_reaction")
                .NpcLine(ConversationTools.SetTextVariables(ConversationTools.SetCharacterObjects(new(DramalordTexts.INTENTION_BLACKMAIL_SEX), other, Hero.MainHero.Spouse), (Hero.MainHero.GetRelationTo(Hero.MainHero.Spouse).Love * 100).ToString()))
                .BeginNpcOptions()
                    .NpcOption(ConversationTools.SetTextVariables(ConversationTools.SetCharacterObjects(new(DramalordTexts.INTENTION_BLACKMAIL_DAYS), Hero.MainHero), 3.ToString()), () => DramalordQuests.Instance.GetQuest(speaker) == null)
                        .Consequence(() =>
                        {
                            BlackmailQuest quest = new BlackmailQuest(speaker, this, Hero.MainHero.GetRelationTo(Hero.MainHero.Spouse).Love * 1000, CampaignTime.DaysFromNow(3));
                            DramalordQuests.Instance.AddQuest(speaker, quest);
                            quest.StartQuest();
                            MBInformationManager.AddNotice(new DramalordQuestNotification(quest));
                        })
                        .CloseDialog()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_UHWELL, () => DramalordQuests.Instance.GetQuest(speaker) != null)
                        .CloseDialog()
                .EndNpcOptions();
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
