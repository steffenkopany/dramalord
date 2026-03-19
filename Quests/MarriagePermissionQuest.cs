using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.Notifications;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class MarriagePermissionQuest : DramalordQuest
    {
        [SaveableField(1)]
        internal Hero Permitter;

        [SaveableField(2)]
        internal bool HasAsked;

        public MarriagePermissionQuest(Hero questGiver, Hero permitter, CampaignTime duration) : base("DramalordMarriagePermissionQuest", questGiver, duration)
        {
            Permitter = permitter;
            HasAsked = false;
            InitializeQuestOnGameLoad();
        }

        public override TextObject GetTitle()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_MARRIAGE_TITLE), QuestGiver);
        }

        public override TextObject Description => ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_MARRIAGE_INFO), QuestGiver, Permitter);

        protected override void SetDialogs()
        {
        }

        public override void OnCanceled()
        {
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
        }

        public override void QuestFail(Hero reason)
        {
            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_MARRIAGE_BLESSING), Permitter, QuestGiver));

            CompleteQuestWithFail();
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
        }

        public override void QuestSuccess(Hero reason)
        {
            AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_MARRIAGE), Hero.MainHero, QuestGiver));
            CompleteQuestWithSuccess();
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
        }

        public override void QuestTimeout()
        {
            QuestFail(Hero.MainHero);
        }

        protected override void RegisterEvents()
        {
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(OnHeroKilled));
        }

        public override void QuestStartInit()
        {
            if (HasAsked)
            {
                AddTrackedObject(QuestGiver);
                RemoveTrackedObject(Permitter);
            }
            else
            {
                AddTrackedObject(Permitter);
                RemoveTrackedObject(QuestGiver);
            }

            AddLog(Description);
        }

        internal void GetPermission()
        {
            if(!HasAsked)
            {
                HasAsked = true;
                AddLog(ConversationTools.SetCharacterObjects(new(DramalordTexts.QUEST_MARRIAGE_BLESSING), Permitter, QuestGiver));
                RemoveTrackedObject(Permitter);
            }
        }

        protected override void InitializeQuestOnGameLoad()
        {
            DialogFlow permitterFlow = DialogFlow.CreateDialogFlow("hero_main_options")
                .BeginPlayerOptions()
                .PlayerSpecialOption(ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INTENTION_MARRY_ENGAGE_ASK), Permitter, QuestGiver))
                .Condition(() => Permitter == Hero.OneToOneConversationHero && HasAsked == false)
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.INTENTION_MARRY_ENGAGE_OK+ "[ib:confident3][if:convo_excited]", () => Permitter?.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFriends && ConversationTools.SetConversationHero(QuestGiver))
                        .Consequence(() =>
                        {
                            GetPermission();
                            if (PlayerEncounter.Current != null)
                            {
                                PlayerEncounter.LeaveEncounter = true;
                            }
                        })
                        .CloseDialog()
                    .NpcOption(DramalordTexts.INTENTION_MARRY_ENGAGE_NO + "[ib:nervous][if:convo_shocked]", () => Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) < DramalordMCM.Instance.MinTrustFriends && ConversationTools.SetConversationHero(QuestGiver))
                        .Consequence(() =>
                        {
                            if (PlayerEncounter.Current != null)
                            {
                                PlayerEncounter.LeaveEncounter = true;
                            }
                        })
                        .CloseDialog()
                .EndNpcOptions()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(permitterFlow, this);

            if (HasAsked)
            {
                AddTrackedObject(QuestGiver);
                RemoveTrackedObject(Permitter);
            }
            else
            {
                AddTrackedObject(Permitter);
                RemoveTrackedObject(QuestGiver);
            }
        }

        protected override void HourlyTick()
        {
            if(Permitter == null)
            {
                GetPermission();
            }

            if(QuestGiver.Spouse != null)
            {
                QuestTimeout();
            }

            if (HasAsked)
            {
                AddTrackedObject(QuestGiver);
                RemoveTrackedObject(Permitter);
            }
            else
            {
                AddTrackedObject(Permitter);
                RemoveTrackedObject(QuestGiver);
            }
        }

        protected void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            if(victim == Permitter)
            {
                if(QuestGiver.Father != null && QuestGiver.Father.IsAlive && QuestGiver.Father != Hero.MainHero)
                {
                    Permitter = QuestGiver.Father;
                }
                else if (QuestGiver.Clan != null && QuestGiver.Clan.Leader != Hero.MainHero && QuestGiver.Clan.Leader != QuestGiver)
                {
                    Permitter = QuestGiver.Clan.Leader;
                }
                else
                {
                    GetPermission();
                }
            }

            if (HasAsked)
            {
                AddTrackedObject(QuestGiver);
                RemoveTrackedObject(Permitter);
            }
            else
            {
                AddTrackedObject(Permitter);
                RemoveTrackedObject(QuestGiver);
            }
        }
    }
}
