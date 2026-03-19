using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Extensions;
using Dramalord.Notifications;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class VisitLoverQuest : DramalordQuest
    {
        [SaveableField(1)]
        internal bool HasBeenVisited = false;

        public VisitLoverQuest(Hero questGiver) : base("DramalordVisitLoverQuest", questGiver, CampaignTime.DaysFromNow(7))
        {
            
        }

        public override TextObject GetTitle()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_VISIT_TITLE), QuestGiver);
        }

        public override TextObject Description => ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_VISIT_INFO), QuestGiver);


        protected override void HourlyTick()
        {

        }

        protected override void InitializeQuestOnGameLoad()
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.INTENTION_VISIT_1_INTIME, () => Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero == QuestGiver && !HasBeenVisited && ConversationTools.SetConversationHero(Hero.MainHero))
                        .PlayerLine(DramalordTexts.INTENTION_VISIT_REACT_1)
                            .NpcLine(DramalordTexts.INTENTION_VISIT_2_INTIME)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                            .Consequence(() => 
                            { 
                                QuestSuccess(Hero.MainHero); 
                                DramalordEvents.Instance.StartIntention(new SexEvent(Hero.MainHero, Hero.OneToOneConversationHero));
                                if(PlayerEncounter.Current != null)
                                {
                                    PlayerEncounter.LeaveEncounter = true;
                                }
                            })
                            .CloseDialog()
                    .NpcOption(DramalordTexts.INTENTION_VISIT_1_LATE, () => Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero == QuestGiver && HasBeenVisited && ConversationTools.SetConversationHero(QuestGiver))
                        .PlayerLine(DramalordTexts.INTENTION_VISIT_REACT_1)
                            .NpcLine(DramalordTexts.INTENTION_VISIT_2_LATE)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                            .Consequence(() => 
                            { 
                                QuestTimeout(); 
                                if (PlayerEncounter.Current != null)
                                {
                                    PlayerEncounter.LeaveEncounter = true;
                                }
                            })
                            .CloseDialog()
            .EndNpcOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow, this);
        }

        public override void OnCanceled()
        {
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
        }


        public override void QuestSuccess(Hero reason)
        {
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);

            //DramalordEvents.Instance.StartIntention(new DateEvent(Hero.MainHero, QuestGiver));

            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_VISIT_SUCCESS), QuestGiver));
            CompleteQuestWithSuccess();
        }

        public override void QuestFail(Hero reason)
        {
            QuestGiver.ChangeRelationTo(Hero.MainHero, -100 + QuestGiver.GetPersonality().Empathy, QuestGiver.GetDesires().Horny * -1);

            (new RelationshipEvent(QuestGiver, Hero.MainHero)).Action();

            Campaign.Current.ConversationManager.RemoveRelatedLines(this);

            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_VISIT_FAILED), QuestGiver));

            CompleteQuestWithFail();
        }

        public override void QuestTimeout()
        {
            QuestFail(Hero.MainHero);
        }

        public override void QuestStartInit()
        {
            HasBeenVisited = false;
            AddLog(Description);
            InitializeQuestOnGameLoad();
        }

        protected override void SetDialogs()
        {
            throw new System.NotImplementedException();
        }
    }
}
