using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class BlackmailQuest : DramalordQuest
    {
        [SaveableField(1)]
        public readonly IDramalordEvent BlackmailEvent;

        [SaveableField(2)]
        public readonly int BlackmailGold;

        [SaveableField(3)]
        internal readonly Hero OtherHero;

        [SaveableField(4)]
        internal readonly Hero Spouse;

        public BlackmailQuest(Hero questTarget, IDramalordEvent questEvent, int gold, CampaignTime duration) : base("DramalordBlackmailQuest", questTarget, duration)
        {
            BlackmailEvent = questEvent;
            BlackmailGold = gold;
            OtherHero = BlackmailEvent.Actor == Hero.MainHero ? BlackmailEvent.Target : BlackmailEvent.Actor;
            Spouse = Hero.MainHero.Spouse;
        }

        protected override void SetDialogs()
        {
            
        }

        public override TextObject GetTitle()
        {
            return ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.QUEST_BLACKMAIL_TITLE), QuestGiver);
        }

        public override TextObject Description => ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_BLACKMAIL_INFO), QuestGiver, OtherHero, Spouse);

        public override void QuestFail(Hero reason)
        {
            if (Spouse != null)
            {
                if (Hero.MainHero.GetRelationTo(Spouse).Love > 0)
                {
                    Hero.MainHero.ChangeRelationTo(Spouse, 0, Hero.MainHero.GetRelationTo(Spouse).Love * -1);
                }

                AddLog(ConversationTools.SetCharacterObjects(ConversationTools.SetTextVariables(new(DramalordTexts.BANNER_RELATIONSHIP_END), DramalordTexts.GetRelationshipStatusName(RelationshipType.Spouse)), Spouse));

                (new RelationshipEvent(Hero.MainHero, Spouse)).Action();
                Hero.MainHero.GetRelationTo(Spouse).LastInteraction = CampaignTime.Now;
            }

            
            CompleteQuestWithFail();
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
        }

        public override void QuestSuccess(Hero reason)
        {
            if(!QuestGiver.IsAlive)
            {
                AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_BLACKMAIL_KILLED), QuestGiver));
            }
            else if(reason == QuestGiver)
            {
                AddLog(ConversationTools.SetTextVariables( ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_BLACKMAIL_PAID), QuestGiver), BlackmailGold.ToString()));
            }
            else
            {
                AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_BLACKMAIL_BEATEN), QuestGiver));
            }

            CompleteQuestWithSuccess();
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
        }

        public override void QuestTimeout()
        {
           QuestFail(Hero.MainHero);
        }

        public override void OnCanceled()
        {
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
        }

        public override void QuestStartInit()
        {
            AddLog(Description);
            InitializeQuestOnGameLoad();
        }

        protected override void InitializeQuestOnGameLoad()
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("hero_main_options")
               .BeginPlayerOptions()
                   .PlayerSpecialOption(ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INTENTION_BLACKMAIL_START), QuestGiver))
                   .Condition(() => Hero.OneToOneConversationHero == QuestGiver)
                   .BeginNpcOptions()
                       .NpcOption(DramalordTexts.INTENTION_BLACKMAIL_START_OK, () => Mission.Current?.GetMissionBehavior<MissionFightHandler>() != null)
                            .BeginPlayerOptions()
                                .PlayerOption(DramalordTexts.INTENTION_BLACKMAIL_PAY)
                                    .Condition(() => Hero.MainHero.Gold >= BlackmailGold && ConversationTools.SetConversationText(new TextObject(BlackmailGold.ToString())))
                                    .Consequence(() =>
                                    {
                                        Hero.MainHero.Gold -= BlackmailGold;
                                        Hero.OneToOneConversationHero.Gold += BlackmailGold;
                                        QuestSuccess(Hero.OneToOneConversationHero);
                                        if (PlayerEncounter.Current != null)
                                        {
                                            PlayerEncounter.LeaveEncounter = true;
                                        }
                                    })
                                    .CloseDialog()
                                .PlayerOption(DramalordTexts.INTENTION_BLACKMAIL_FIGHT)
                                    .Consequence(() =>
                                    {
                                        MissionFightHandler? fightHandler = Mission.Current?.GetMissionBehavior<MissionFightHandler>();
                                        if (fightHandler != null)
                                        {
                                            fightHandler.StartCustomFight(
                                                new List<Agent> { Agent.Main },
                                                new List<Agent> { (Agent)MissionConversationLogic.Current.ConversationManager.OneToOneConversationAgent },
                                                false,
                                                false,
                                                (b) => { if (b) QuestSuccess(Hero.MainHero); else QuestFail(Hero.MainHero); }
                                                );
                                        }
                                    })
                                    .CloseDialog()
                                .PlayerOption(DramalordTexts.NPC_INTERACTION_UHWELL)
                                    .GotoDialogState("hero_main_options")
                           .EndPlayerOptions()
                       .NpcOption(DramalordTexts.INTENTION_DIVORCE_START_NO, () => Mission.Current?.GetMissionBehavior<MissionFightHandler>() == null)
                       .GotoDialogState("hero_main_options")
                   .EndNpcOptions()
               .EndPlayerOptions();


            Campaign.Current.ConversationManager.AddDialogFlow(flow, this);
        }

        protected override void HourlyTick()
        {
            if(Spouse.Spouse != Hero.MainHero)
            {
                CompleteQuestWithCancel();
            }
        }
    }
}
