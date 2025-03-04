using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class ConfrontHeroQuest : DramalordQuest
    {
        [SaveableField(1)]
        public readonly Intention ConfrontIntention;

        internal static Hero? OtherHero;

        public ConfrontHeroQuest(Hero questTarget, Intention questIntention, CampaignTime duration) : base("DramalordConfrontHeroQuest", questTarget, duration)
        {
            ConfrontIntention = questIntention;
        }

        protected override void SetDialogs()
        {
            OtherHero = ConfrontIntention.IntentionHero == QuestGiver ? ConfrontIntention.Target : ConfrontIntention.IntentionHero;

            ConversationLines.npc_starts_confrontation_surprised.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.npc_starts_confrontation_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));

            ConversationLines.npc_reply_confrontation_love.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.npc_reply_confrontation_nocare.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.npc_confrontation_reply_gossip.SetTextVariable("TITLE", ConversationTools.GetHeroRelation(Hero.OneToOneConversationHero, Hero.MainHero));

            ConversationLines.npc_confrontation_result_ok.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            ConversationLines.npc_confrontation_result_break.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            ConversationLines.npc_confrontation_result_break_other.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            ConversationLines.npc_confrontation_result_break_other.SetTextVariable("HERO", OtherHero.Name);

            
            if (ConfrontIntention as BetrothIntention != null)
            {
                ConversationLines.npc_confrontation_engagement_player.SetTextVariable("HERO", OtherHero.Name);
                ConversationLines.npc_confrontation_engagement_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Hero.OneToOneConversationHero));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_engagement_player);
            }
            else if (ConfrontIntention as GiveBirthIntention != null)
            {
                ConfrontBirthIntention i = ConfrontIntention as ConfrontBirthIntention;
                ConversationLines.npc_confrontation_birth_player.SetTextVariable("CHILD", i.Child.Name);
                ConversationLines.npc_confrontation_birth_player.SetTextVariable("HERO", OtherHero.Name);
                ConversationLines.npc_confrontation_birth_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Hero.OneToOneConversationHero));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_birth_player);
            }
            else if (ConfrontIntention as DateIntention != null)
            {
                ConversationLines.npc_confrontation_date_player.SetTextVariable("HERO", OtherHero.Name);
                ConversationLines.npc_confrontation_date_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Hero.OneToOneConversationHero));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_date_player);
            }
            else if (ConfrontIntention as IntercourseIntention != null)
            {
                ConversationLines.npc_confrontation_intercourse_player.SetTextVariable("HERO", OtherHero.Name);
                ConversationLines.npc_confrontation_intercourse_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Hero.OneToOneConversationHero));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_intercourse_player);
            }
            else if (ConfrontIntention as MarriageIntention != null)
            {
                ConversationLines.npc_confrontation_marriage_player.SetTextVariable("HERO", OtherHero.Name);
                ConversationLines.npc_confrontation_marriage_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Hero.OneToOneConversationHero));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_marriage_player);
            }
            
        }

        public override TextObject GetTitle()
        {
            TextObject txt = new TextObject("{=Dramalord573}Confront {HERO} with rumours");
            txt.SetTextVariable("HERO", QuestGiver.Name);
            return txt;
        }

        public override void QuestFail(Hero reason)
        {
            CompleteQuestWithFail();
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
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
            TextObject txt = new TextObject("{=Dramalord561}Find {HERO.LINK} and confront them with the disturbing rumours you have heard.");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, txt);
            AddLog(txt);
            InitializeQuestOnGameLoad();
        }

        protected override void InitializeQuestOnGameLoad()
        {

            DialogFlow playerFlow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine("{npc_starts_confrontation_surprised}[ib:nervous][if:convo_nervous]")
                    .Condition(() => 
                    { 
                        if (Hero.OneToOneConversationHero == QuestGiver) 
                        { 
                            SetDialogs(); 
                            return true; 
                        } 
                        return false; 
                    })
                    .PlayerLine("{CONFRONTATION_LINE}")
                        .GotoDialogState("npc_confrontation_reaction");

            DialogFlow npcFlow = DialogFlow.CreateDialogFlow("npc_confrontation_reaction")
                .BeginNpcOptions()
                    .NpcOption("{npc_reply_confrontation_love}[ib:nervous][if:convo_shocked]", () => Hero.OneToOneConversationHero.GetHeroTraits().Honor >= 0 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove)
                        .GotoDialogState("player_confrontation_resolution")
                    .NpcOption("{npc_reply_confrontation_nocare}[ib:normal2][if:convo_mocking_teasing]", () => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove || Hero.OneToOneConversationHero == OtherHero)
                        .GotoDialogState("player_confrontation_resolution")
                    .NpcOption("{npc_confrontation_reply_gossip}[ib:weary2][if:convo_nervous]", () => Hero.OneToOneConversationHero.GetHeroTraits().Honor < 0 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove)
                        .GotoDialogState("player_confrontation_resolution")
                .EndNpcOptions();

            DialogFlow resFlow = DialogFlow.CreateDialogFlow("player_confrontation_resolution")
                .BeginPlayerOptions()
                    .PlayerOption("{npc_confrontation_result_ok}")
                        .Consequence(() => { QuestFail(Hero.MainHero); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .PlayerOption("{npc_confrontation_result_break}")
                        .Consequence(() => 
                        {
                            ConversationTools.ConversationIntention = new ChangeOpinionIntention(Hero.MainHero, Hero.OneToOneConversationHero, MathF.Max(0, Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love) * -1, MathF.Max(0, Hero.MainHero.GetTrust(Hero.OneToOneConversationHero)) * -1, CampaignTime.Now);
                            TextObject txt = new TextObject("{=Dramalord088}You ended your relationship with {HERO.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", ConfrontIntention.IntentionHero.CharacterObject, txt);
                            AddLog(txt);
                            QuestSuccess(Hero.MainHero);
                            ConversationTools.EndConversation();
                        })
                        .CloseDialog()
                    .PlayerOption("{npc_confrontation_result_break_other}")
                        .Consequence(() =>
                        {
                            ConversationTools.ConversationIntention = new ChangeOpinionIntention(Hero.OneToOneConversationHero, OtherHero, MathF.Max(0, OtherHero.GetRelationTo(Hero.OneToOneConversationHero).Love) * -1, MathF.Max(0, OtherHero.GetTrust(Hero.OneToOneConversationHero)) * -1, CampaignTime.Now);
                            TextObject txt = new TextObject("{=Dramalord571}You told {HERO1.LINK} to break up with {HERO2.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO1", ConfrontIntention.IntentionHero.CharacterObject, txt);
                            StringHelpers.SetCharacterProperties("HERO2", ConfrontIntention.Target.CharacterObject, txt);
                            AddLog(txt);

                            QuestSuccess(Hero.MainHero);
                            ConversationTools.EndConversation();
                        })
                        .CloseDialog()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(playerFlow, this);
            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow, this);
            Campaign.Current.ConversationManager.AddDialogFlow(resFlow, this);
        }

        protected override void HourlyTick()
        {
            if(ConfrontIntention.IntentionHero.GetRelationTo(ConfrontIntention.Target).Relationship == RelationshipType.None)
                CompleteQuestWithCancel();
        }
    }
}
