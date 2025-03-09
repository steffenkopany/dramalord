using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class BlackmailPlayerQuest : DramalordQuest
    {
        [SaveableField(1)]
        public readonly Intention BlackmailIntention;

        [SaveableField(2)]
        public readonly int BlackmailGold;

        [SaveableField(3)]
        public readonly Hero PlayerSpouse;

        public BlackmailPlayerQuest(Hero questTarget, Intention questIntention, int blackmailGold, CampaignTime duration) : base("DramalordBlackmailPlayerQuest", questTarget, duration)
        {
            BlackmailIntention = questIntention;
            BlackmailGold = blackmailGold;
            PlayerSpouse = Hero.MainHero.Spouse;
        }

        protected override void SetDialogs()
        {
            ConversationLines.npc_starts_confrontation_surprised.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
        }

        public override TextObject GetTitle()
        {
            TextObject txt = new TextObject("{=Dramalord607}{HERO} is blackmailing you");
            txt.SetTextVariable("HERO", QuestGiver.Name);
            return txt;
        }

        public override void QuestFail(Hero reason)
        {
            CompleteQuestWithFail();
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);

            if(BlackmailIntention as DateIntention != null)
            {
                DramalordIntentions.Instance.GetIntentions().Add(new ConfrontDateIntention(BlackmailIntention.IntentionHero, BlackmailIntention.Target, PlayerSpouse, CampaignTime.DaysFromNow(7), true));
            }
            else if (BlackmailIntention as IntercourseIntention != null)
            {
                DramalordIntentions.Instance.GetIntentions().Add(new ConfrontIntercourseIntention(BlackmailIntention.IntentionHero, BlackmailIntention.Target, PlayerSpouse, CampaignTime.DaysFromNow(7), true));
            }
            else if (BlackmailIntention as BetrothIntention != null)
            {
                DramalordIntentions.Instance.GetIntentions().Add(new ConfrontBetrothedIntention(BlackmailIntention.IntentionHero, BlackmailIntention.Target, PlayerSpouse, CampaignTime.DaysFromNow(7), true));
            }
        }

        public override void QuestSuccess(Hero reason)
        {
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
            TextObject txt = new TextObject("{=Dramalord608}{HERO.LINK} saw you and {OTHER.LINK}. They threaten you to tell {SPOUSE.LINK} if you do not pay them {AMOUNT}{GOLD_ICON}. Visit them during night time and resolve that matter.");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, txt);
            StringHelpers.SetCharacterProperties("OTHER", BlackmailIntention.IntentionHero == Hero.MainHero ? BlackmailIntention.Target.CharacterObject : BlackmailIntention.IntentionHero.CharacterObject, txt);
            StringHelpers.SetCharacterProperties("SPOUSE", PlayerSpouse.CharacterObject, txt);
            txt.SetTextVariable("AMOUNT", BlackmailGold);
            AddLog(txt);
            InitializeQuestOnGameLoad();
        }

        protected override void InitializeQuestOnGameLoad()
        {
            DialogFlow playerFlow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine("{npc_starts_confrontation_surprised}[ib:nervous][if:convo_nervous]")
                    .Condition(() =>
                    {
                        if (Hero.OneToOneConversationHero == QuestGiver && CampaignTime.Now.IsNightTime && Mission.Current != null && Mission.Current.GetMissionBehavior<MissionFightHandler>() != null)
                        {
                            SetDialogs();
                            return true;
                        }
                        return false;
                    })
                    .BeginPlayerOptions()
                        .PlayerOption("{player_wants_kill}")
                            .Consequence(() =>
                            {
                                MissionFightHandler fightHandler = Mission.Current.GetMissionBehavior<MissionFightHandler>();
                                if (fightHandler != null)
                                {
                                    fightHandler.StartCustomFight(
                                        new List<Agent> { Agent.Main },
                                        new List<Agent> { (Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents.First() },
                                        false,
                                        false,
                                        (b) => OnFightEnd(b)
                                        );
                                }
                            })
                            .CloseDialog()
                        .PlayerOption("{player_blackmail_pay}")
                            .Condition(() => Hero.MainHero.Gold >= BlackmailGold)
                            .Consequence(() => { Hero.MainHero.Gold -= BlackmailGold; QuestSuccess(Hero.MainHero); })
                            .CloseDialog()
                    .EndPlayerOptions();


            Campaign.Current.ConversationManager.AddDialogFlow(playerFlow, this);
        }

        private void OnFightEnd(bool playerWon)
        {
            if (playerWon)
            {
                QuestSuccess(Hero.MainHero);
            }
            else
            {
                QuestFail(Hero.MainHero);
            }
        }

        protected override void HourlyTick()
        {
            if(!QuestGiver.IsAlive || PlayerSpouse != Hero.MainHero.Spouse)
                CompleteQuestWithCancel();
        }
    }
}
