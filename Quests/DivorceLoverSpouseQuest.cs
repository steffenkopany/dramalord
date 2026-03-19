using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Extensions;
using Dramalord.Notifications;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class DivorceLoverSpouseQuest : DramalordQuest
    {
        [SaveableField(1)]
        internal Hero Spouse;

        [SaveableField(2)]
        internal bool Divorced;

        public DivorceLoverSpouseQuest(Hero questGiver, Hero spouse, CampaignTime duration) : base("DramalordDivorceLoverSpouseQuest", questGiver, duration)
        {
            Spouse = spouse;
            Divorced = false;
        }

        public override TextObject GetTitle()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_DIVORCE_TITLE), QuestGiver, Spouse);
        }

        public override TextObject Description => ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_DIVORCE_INFO), QuestGiver, Spouse);

        protected override void HourlyTick()
        {

        }

        protected override void RegisterEvents()
        {
            CampaignEvents.RomanticStateChanged.AddNonSerializedListener(this, new Action<Hero, Hero, Romance.RomanceLevelEnum>(OnRomanticStateChanged));
        }

        protected override void InitializeQuestOnGameLoad()
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("hero_main_options")
                .BeginPlayerOptions()
                    .PlayerSpecialOption(ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INTENTION_DIVORCE_START), Spouse, QuestGiver))
                    .Condition(() => Hero.OneToOneConversationHero == Spouse)
                    .BeginNpcOptions()
                        .NpcOption(DramalordTexts.INTENTION_DIVORCE_START_OK, () => Mission.Current?.GetMissionBehavior<MissionFightHandler>() != null)
                            .PlayerLine(DramalordTexts.INTENTION_DIVORCE_REQUEST)
                            .Condition(() => ConversationTools.SetConversationHero(QuestGiver))
                            .BeginNpcOptions()
                                .NpcOption(DramalordTexts.INTENTION_DIVORCE_REQUEST_PAY, () => ConversationTools.SetConversationHero(QuestGiver) && Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Generosity) < 0 && Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor) < 1)
                                    .BeginPlayerOptions()
                                        .PlayerOption(DramalordTexts.INTENTION_DIVORCE_PAY)
                                            .Condition(() => Hero.MainHero.Gold >= QuestGiver.Gold && ConversationTools.SetConversationText(new TextObject(QuestGiver.Gold.ToString())))
                                            .Consequence(() => 
                                            {
                                                Hero.MainHero.Gold -= QuestGiver.Gold;
                                                Hero.OneToOneConversationHero.Gold += QuestGiver.Gold;
                                                QuestSuccess(Hero.MainHero);
                                                if (PlayerEncounter.Current != null)
                                                {
                                                    PlayerEncounter.LeaveEncounter = true;
                                                }
                                            })
                                            .CloseDialog()
                                        .PlayerOption(DramalordTexts.NPC_INTERACTION_UHWELL)
                                        .GotoDialogState("hero_main_options")
                                    .EndPlayerOptions()
                                .NpcOption(DramalordTexts.INTENTION_DIVORCE_REQUEST_FIGHT, () => ConversationTools.SetConversationHero(QuestGiver) && (Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Generosity) >= 0 || Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor) >= 0))
                                    .BeginPlayerOptions()
                                        .PlayerOption(DramalordTexts.INTENTION_DIVORCE_DRAW_WEAPON)
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
                            .EndNpcOptions()
                        .NpcOption(DramalordTexts.INTENTION_DIVORCE_START_NO, () => Mission.Current?.GetMissionBehavior<MissionFightHandler>() == null)
                        .GotoDialogState("hero_main_options")
                    .EndNpcOptions()
                .EndPlayerOptions();
                

            Campaign.Current.ConversationManager.AddDialogFlow(flow, this);
        }

        public override void OnCanceled()
        {
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            CompleteQuestWithCancel();
        }


        public override void QuestSuccess(Hero reason)
        {
            Divorced = true;
            QuestGiver.ChangeRelationTo(Spouse, -100, -100);
            (new RelationshipEvent(QuestGiver, Spouse)).Action();

            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);

            TextObject logme = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), QuestGiver, Spouse);
            ConversationTools.SetTextVariables(logme, DramalordTexts.GetRelationshipStatusName(RelationshipType.Spouse));
            
            AddLog(logme);
            CompleteQuestWithSuccess();
        }

        public override void QuestFail(Hero reason)
        {
            QuestGiver.ChangeRelationTo(Hero.MainHero, -100 + QuestGiver.GetPersonality().Empathy, -100 + QuestGiver.GetPersonality().Empathy);

            (new RelationshipEvent(QuestGiver, Hero.MainHero)).Action();

            DramalordQuests.Instance.RemoveQuest(QuestGiver);
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
            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_DIVORCE_INFO), QuestGiver, Spouse));
            RemoveTrackedObject(QuestGiver);
            AddTrackedObject(Spouse);
            InitializeQuestOnGameLoad();
        }

        protected override void SetDialogs()
        {
            throw new System.NotImplementedException();
        }

        public void OnRomanticStateChanged(Hero hero1, Hero hero2, Romance.RomanceLevelEnum level)
        {
            if (level == Romance.RomanceLevelEnum.FailedInPracticalities)
            {
                if ((hero1 == QuestGiver && hero2 == Spouse) || (hero2 == QuestGiver && hero1 == Spouse))
                {
                    if(!Divorced)
                        OnCanceled();
                }
            }
        }
    }
}
