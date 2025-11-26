using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class ConfrontHeroQuest : DramalordQuest
    {
        [SaveableField(1)]
        public readonly IDramalordEvent ConfrontEvent;

        internal static Hero? OtherHero;

        public ConfrontHeroQuest(Hero questTarget, IDramalordEvent questEvent, CampaignTime duration) : base("DramalordConfrontHeroQuest", questTarget, duration)
        {
            ConfrontEvent = questEvent;
        }

        protected override void SetDialogs()
        {
            OtherHero = ConfrontEvent.Actor == QuestGiver ? ConfrontEvent.Target : ConfrontEvent.Actor;
        }

        public override TextObject GetTitle()
        {
            return new(DramalordTexts.INQUIRY_CONFRONT_TITLE);
        }

        public override void QuestFail(Hero reason)
        {
            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_CONFRONT_FAILED), ConfrontEvent.Actor));
            CompleteQuestWithFail();
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
        }

        public override void QuestSuccess(Hero reason)
        {
            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_CONFRONT_SUCCESS), ConfrontEvent.Actor));
            CompleteQuestWithSuccess();
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
        }

        public override void QuestTimeout()
        {
            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_CONFRONT_FAILED), ConfrontEvent.Actor));
            //QuestFail(Hero.MainHero);
        }

        public override void OnCanceled()
        {
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
        }

        public override void QuestStartInit()
        {
            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_CONFRONT_INFO), ConfrontEvent.Actor, ConfrontEvent.Target));
            InitializeQuestOnGameLoad();
        }

        protected override void InitializeQuestOnGameLoad()
        {
            OtherHero = ConfrontEvent.Actor == QuestGiver ? ConfrontEvent.Target : ConfrontEvent.Actor;
            DialogFlow playerFlow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine(ConversationTools.SetCharacterObjects(new(DramalordTexts.PLAYER_CONFRONT_NPC_INTRO), Hero.MainHero) + "[ib:nervous][if:convo_nervous]")
                    .Condition(() => Hero.OneToOneConversationHero == QuestGiver)
                    .PlayerLine(DramalordTexts.PLAYER_CONFRONT_ACCUSE)
                        .Condition(() => ConversationTools.SetConversationHero(OtherHero))
                            .NpcLine(ConversationTools.SetCharacterObjects(new(DramalordTexts.PLAYER_CONFRONT_NPC_PLAYING_INNOCENT), Hero.MainHero) + "[ib:nervous][if:convo_shocked]")
                                .BeginPlayerOptions()
                                    .PlayerOption(ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_RESULT_OK), QuestGiver))
                                        .NpcLine(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                                            .Consequence(() => CompleteQuestWithCancel())
                                            .CloseDialog()
                                    .PlayerOption(ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_RESULT_BREAKUP), QuestGiver))
                                        .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                                            .Consequence(() =>
                                            {
                                                Hero.MainHero.ChangeRelationTo(Hero.OneToOneConversationHero, Hero.MainHero.GetTrust(Hero.OneToOneConversationHero) * -1, Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love * -1);
                                                DramalordEvents.Instance.StartIntention(new RelationshipEvent(Hero.MainHero, Hero.OneToOneConversationHero));
                                                QuestSuccess(Hero.MainHero);
                                            })
                                            .NpcLine(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                                                .CloseDialog()
                                    .PlayerOption(ConversationTools.SetCharacterObjects(new(DramalordTexts.CONFRONTATION_RESULT_BREAKUP_OTHER), OtherHero))
                                        .NpcLine(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                                            .Consequence(() =>
                                            {
                                                OtherHero.ChangeRelationTo(Hero.OneToOneConversationHero, 0, OtherHero.GetRelationTo(Hero.OneToOneConversationHero).Love * -1);
                                                DramalordEvents.Instance.StartIntention(new RelationshipEvent(OtherHero, Hero.OneToOneConversationHero));
                                                QuestSuccess(Hero.MainHero);
                                            })
                                            .CloseDialog()
                                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(playerFlow, this);
        }

        protected override void HourlyTick()
        {
            
        }
    }
}
