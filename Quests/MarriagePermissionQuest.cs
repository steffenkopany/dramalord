using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class MarriagePermissionQuest : DramalordQuest
    {
        [SaveableField(1)]
        internal Hero Permitter;

        public MarriagePermissionQuest(Hero questGiver, Hero permitter, CampaignTime duration) : base("DramalordMarriagePermissionQuest", questGiver, duration)
        {
            Permitter = permitter;
            InitializeQuestOnGameLoad();
        }

        public override TextObject GetTitle()
        {
            TextObject txt = new TextObject("{=Dramalord548}Ask {QUESTHERO} for their hand in marriage.");
            txt.SetTextVariable("QUESTHERO", QuestGiver.Name);
            return txt;
        }

        protected override void SetDialogs()
        {
            ConversationLines.player_quest_marriage_ask.SetTextVariable("HERO", QuestGiver.Name);
            ConversationLines.player_quest_marriage_agree.SetTextVariable("HERO", QuestGiver.Name);
            ConversationLines.player_quest_marriage_later.SetTextVariable("HERO", QuestGiver.Name);
            ConversationLines.player_quest_marriage_decline.SetTextVariable("HERO", QuestGiver.Name);
        }

        public override void OnCanceled()
        {
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
        }

        public override void QuestFail(Hero reason)
        {
            RelationshipLossAction.Apply(QuestGiver, Hero.MainHero, out int loveDamage, out int trustDamage, 50, 30);
            new ChangeOpinionIntention(QuestGiver, Hero.MainHero, loveDamage, trustDamage, CampaignTime.Now).Action();

            CompleteQuestWithFail();
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
        }

        public override void QuestSuccess(Hero reason)
        {
            new BetrothIntention(QuestGiver, Hero.MainHero, CampaignTime.Now, true).OnConversationEnded();
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
            RemoveTrackedObject(QuestGiver);
            AddTrackedObject(Permitter);

            TextObject txt = new TextObject("{=Dramalord549}{QUESTHERO.LINK} told you that you require the permission of {TARGET.LINK} in order to marry them.");
            StringHelpers.SetCharacterProperties("TARGET", Permitter?.CharacterObject, txt);
            StringHelpers.SetCharacterProperties("QUESTHERO", QuestGiver.CharacterObject, txt);
            AddLog(txt);
        }

        protected override void InitializeQuestOnGameLoad()
        {
            DialogFlow permitterFlow = DialogFlow.CreateDialogFlow("hero_main_options")
                .BeginPlayerOptions()
                .PlayerOption("{player_quest_marriage_ask}")
                .Condition(() => { SetDialogs(); return Permitter != null && Permitter == Hero.OneToOneConversationHero; })
                .BeginNpcOptions()
                    .NpcOption("{player_quest_marriage_agree}", () => Clan.PlayerClan.Tier >= 3 && Permitter?.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFriends)
                        .Consequence(() => { QuestSuccess(Hero.MainHero); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{player_quest_marriage_later}", () => Clan.PlayerClan.Tier < 3 || Permitter?.GetTrust(Hero.MainHero) < DramalordMCM.Instance.MinTrustFriends || Permitter?.GetTrust(Hero.MainHero) > 0)
                        .Consequence(() =>
                        {
                            TextObject banner = new TextObject("{=Dramalord554}Make sure your clan is tier 3+ and {HERO} likes you.");
                            MBTextManager.SetTextVariable("HERO", Permitter?.Name);
                            MBInformationManager.AddQuickInformation(banner, 0, Permitter?.CharacterObject, "event:/ui/notification/relation");
                            AddLog(banner);
                        })
                        .GotoDialogState("hero_main_options")
                    .NpcOption("{player_quest_marriage_decline}", () => Clan.PlayerClan.Tier < 3 && Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) < 0)
                        .Consequence(() => { QuestFail(Hero.MainHero); ConversationTools.EndConversation(); })
                        .CloseDialog()
                .EndNpcOptions()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(permitterFlow, this);
        }

        protected override void HourlyTick()
        {
            if(Permitter == null)
            {
                QuestSuccess(Hero.MainHero);
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
                    QuestSuccess(Hero.MainHero);
                }
            }
        }
    }
}
