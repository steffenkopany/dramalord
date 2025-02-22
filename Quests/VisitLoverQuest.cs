using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
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
            TextObject txt = new TextObject("{=Dramalord304}{QUESTHERO} requests your presence.");
            MBTextManager.SetTextVariable("QUESTHERO", QuestGiver.Name);
            return txt;
        }


        protected override void HourlyTick()
        {

        }

        protected override void InitializeQuestOnGameLoad()
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_quest_visit_start_open}", () => { if (Hero.OneToOneConversationHero.IsDramalordLegit()) SetDialogs(); return Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero == QuestGiver && !HasBeenVisited; })
                        .PlayerLine("{player_quest_visit_reply}")
                            .NpcLine("{npc_quest_visit_success}")
                            .Consequence(() => { QuestSuccess(Hero.MainHero); ConversationTools.EndConversation(); })
                            .CloseDialog()
                    .NpcOption("{npc_quest_visit_start_fail}", () => Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero == QuestGiver && HasBeenVisited)
                        .PlayerLine("{player_quest_visit_reply}")
                            .NpcLine("{npc_quest_visit_fail}")
                            .Consequence(() => { QuestTimeout(); ConversationTools.EndConversation(); })
                            .CloseDialog()
            .EndNpcOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow, this);
        }

        protected override void SetDialogs()
        {
            ConversationLines.npc_quest_visit_start_open.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//{=Dramalord306}Oh {TITLE} you are finally here! I was desperately waiting for you!;
            ConversationLines.player_quest_visit_reply.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//{=Dramalord308}Here I am. What is the urgent matter you asked me to take care about?
            ConversationLines.npc_quest_visit_success.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//{=Dramalord309}Drop your clothes {TITLE} and follow me. I need to make use of your resourcefulness in a very delicate matter!
            ConversationLines.npc_quest_visit_start_fail.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//{=Dramalord307}Look who shows up. You weren't in a hurry {TITLE}, right?");
            ConversationLines.npc_quest_visit_fail.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//{=Dramalord310}Well {TITLE}, someone else was was taking care of my... matter. In a very pleasant way. Good bye.");

        }

        public override void OnCanceled()
        {
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
        }


        public override void QuestSuccess(Hero reason)
        {
            DateAction.Apply(QuestGiver, Hero.MainHero, out int loveGain, out int trustGain, 2);

            TextObject banner = new TextObject("{=Dramalord305}{HERO.LINK} is very happy you fullfilled their request...");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            MBTextManager.SetTextVariable("NUM", ConversationTools.FormatNumber(loveGain));
            MBTextManager.SetTextVariable("NUM2", ConversationTools.FormatNumber(trustGain));

            new ChangeOpinionIntention(QuestGiver, Hero.MainHero, loveGain, trustGain, CampaignTime.Now).Action();

            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);

            ConversationTools.ConversationIntention = new IntercourseIntention(QuestGiver, Hero.MainHero, CampaignTime.Now, true);
            ConversationTools.EndConversation();

            AddLog(banner);
            CompleteQuestWithSuccess();
        }

        public override void QuestFail(Hero reason)
        {
            RelationshipLossAction.Apply(QuestGiver, Hero.MainHero, out int loveDamage, out int trustDamage, 20, 33);

            TextObject banner = new TextObject("{=Dramalord537}In your absence {OTHER.LINK} took care of {HERO.LINK} and their issue...");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            StringHelpers.SetCharacterProperties("OTHER", reason.CharacterObject, banner);
            MBInformationManager.AddQuickInformation(banner, 0, QuestGiver.CharacterObject, "event:/ui/notification/relation");

            Campaign.Current.ConversationManager.RemoveRelatedLines(this);
            new ChangeOpinionIntention(QuestGiver, Hero.MainHero, loveDamage, trustDamage, CampaignTime.Now).Action();

            HasBeenVisited = true;
            AddLog(banner);
        }

        public override void QuestTimeout()
        {
            RelationshipLossAction.Apply(QuestGiver, Hero.MainHero, out int loveDamage, out int trustDamage, 20, 33);

            TextObject banner = new TextObject("{=Dramalord302}{HERO.LINK} is disappointed by your neglection of their matter...");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            MBInformationManager.AddQuickInformation(banner, 0, QuestGiver.CharacterObject, "event:/ui/notification/relation");

            new ChangeOpinionIntention(QuestGiver, Hero.MainHero, loveDamage, trustDamage, CampaignTime.Now).Action();

            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            Campaign.Current.ConversationManager.RemoveRelatedLines(this);

            AddLog(banner);
            CompleteQuestWithFail();
        }

        public override void QuestStartInit()
        {
            HasBeenVisited = false;
            TextObject txt = new TextObject("{=Dramalord303}{HERO.LINK} asks you to find them, as they have an urgent matter to discuss. Will you make it in time?");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, txt);
            AddLog(txt);
            InitializeQuestOnGameLoad();
        }
    }
}
