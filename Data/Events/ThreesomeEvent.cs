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
    internal class ThreesomeEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableProperty(4)]
        public Hero Third { get; private set; }

        public override CampaignTime KeepInHistoryTime => CampaignTime.Days(14f);

        public ThreesomeEvent(Hero actor, Hero target, Hero third)
        {
            Actor = actor;
            Target = target;
            Third = third;
            IsKnownTo.Add(actor);
            IsKnownTo.Add(target);
            IsKnownTo.Add(third);
        }

        public void Action(int modifier = 0)
        {
            HeroDesires heroDesires = Actor.GetDesires();
            HeroDesires targetDesires = Target.GetDesires();
            HeroDesires thirdDesires = Third.GetDesires();


            heroDesires.Horny = 0;
            targetDesires.Horny = 0;
            thirdDesires.Horny = 0;
        }

        public void AfterDialog()
        {
            AddLogEntry(this);

            if (Actor == Hero.MainHero || Target == Hero.MainHero || Third == Hero.MainHero)
            {
                if (DramalordCampaignBehavior.HotScenesFound && DramalordMCM.Instance.ShowHotScenes)
                {
                    MBInformationManager.ShowSceneNotification(new HotScenesNotificationData(Actor, Target, Third));
                }
                else if (DramalordMCM.Instance.ShowDramaVideos)
                {
                    TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_THREESOME), Actor, Target, Third);
                    DramalordVideoNotification.ShowDramalordVideoNotification(Actor, Target, Third, txt, DramalordVideoNotification.VideoContext.Threesome, DramalordVideoNotification.VideoSound.RomanticChime);
                    MBInformationManager.AddNotice(new DramalordEventNotification(this, GetEncyclopediaText()));
                }
                else
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_THREESOME), Actor, Target, Third), true);
                }
            }

            int chance = DramalordMCM.Instance.PregnancyChance;
            if (!CampaignOptions.IsLifeDeathCycleDisabled && Actor.IsFemale != Target.IsFemale && Actor.IsFertile() && Target.IsFertile() && MBRandom.RandomInt(1, 100) <= chance && DramalordPregnancies.Instance.GetPregnancy(Actor.IsFemale ? Actor : Target) == null)
            {
                DramalordEvents.Instance.StartIntention(new ConceiveEvent(Actor, Target));
            }
            if (!CampaignOptions.IsLifeDeathCycleDisabled && Actor.IsFemale != Third.IsFemale && Actor.IsFertile() && Third.IsFertile() && MBRandom.RandomInt(1, 100) <= chance && DramalordPregnancies.Instance.GetPregnancy(Actor.IsFemale ? Actor : Third) == null)
            {
                DramalordEvents.Instance.StartIntention(new ConceiveEvent(Actor, Third));
            }
            if (!CampaignOptions.IsLifeDeathCycleDisabled && Third.IsFemale != Target.IsFemale && Third.IsFertile() && Target.IsFertile() && MBRandom.RandomInt(1, 100) <= chance && DramalordPregnancies.Instance.GetPregnancy(Third.IsFemale ? Third : Target) == null)
            {
                DramalordEvents.Instance.StartIntention(new ConceiveEvent(Third, Target));
            }
            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
            Actor.GetRelationTo(Third).LastInteraction = CampaignTime.Now;
            Third.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj || Third == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_THREESOME), Actor, Target, Third);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            if (hero.IsEmotionalWith(Actor) && hero.GetPersonality().Jealousy > 0)
            {
                return new ConfrontEvent(hero, Actor, this);
            }
            if (hero.IsEmotionalWith(Target) && hero.GetPersonality().Jealousy > 0)
            {
                return new ConfrontEvent(hero, Target, this);
            }
            if (hero.IsEmotionalWith(Third) && hero.GetPersonality().Jealousy > 0)
            {
                return new ConfrontEvent(hero, Third, this);
            }

            if (hero.GetTraitLevel(DefaultTraits.Generosity) < 0 && hero.GetTraitLevel(DefaultTraits.Honor) < 0 && (Actor == Hero.MainHero || Target == Hero.MainHero || Third == Hero.MainHero) && Hero.MainHero.Spouse != null && Hero.MainHero.Spouse != Actor && Hero.MainHero.Spouse != Target && Hero.MainHero.Spouse != Third)
            {
                if (DramalordQuests.Instance.GetQuest(hero) == null)
                {
                    return new BlackmailEvent(hero, Hero.MainHero, this);
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
                    .NpcOption(DramalordTexts.CONFRONTATION_SEX_OTHER + "[ib:warrior][if:convo_grave]", () => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Actor == Hero.MainHero ? Target : (Target == Hero.MainHero) ? Third : Actor)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : (Target == Hero.MainHero) ? Third : Actor))
                        .Consequence(() => {
                            ReactionResult(speaker, Hero.MainHero, out int trust, out int love);
                            speaker.ChangeRelationTo(Hero.MainHero, trust, love);
                        })
                        .NpcLine(DramalordTexts.CONFRONTATION_RESULT_NO_RELATION)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                            .CloseDialog()
                    .NpcOption(DramalordTexts.CONFRONTATION_SEX_PLAYER + "[ib:warrior][if:convo_grave]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Hero.MainHero)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : (Target == Hero.MainHero) ? Third : Actor))
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
            return null;
        }

        public DialogFlow? GetBlackmailDialog(Hero speaker)
        {
            Hero other = Actor == Hero.MainHero ? Target : (Target == Hero.MainHero) ? Third : Actor;
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

        public bool IsVisibleNotification => DramalordMCM.Instance.SexLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero || Third == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionCivilian;
    }
}
