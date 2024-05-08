using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using StoryMode.StoryModeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class VisitLoverQuest : QuestBase
    {
        [SaveableField(1)]
        public Settlement? Settlement;

        [SaveableField(2)]
        public static List<Hero> HeroList = new();

        public VisitLoverQuest(Hero questGiver) : base("DramalordVisitLoverQuest", questGiver, CampaignTime.DaysFromNow(7), 0)
        {
            RelationshipChangeWithQuestGiver = Info.GetTraitscoreToHero(questGiver, Hero.MainHero) * 10;
            AddTrackedObject(questGiver);
            if(questGiver.CurrentSettlement != null)
            {
                Settlement = questGiver.CurrentSettlement;
                AddTrackedObject(questGiver.CurrentSettlement);
                SetDialogs();
                AddLog(new TextObject(questGiver.Name + " asks you to find them, as they have an urgent matter to discuss. Will you comply?"));
            }

            if(!HeroList.Contains(questGiver))
            {
                HeroList.Add(questGiver);
            }
        }

        public override TextObject Title => new TextObject(QuestGiver.Name + " requests your presence");

        public override bool IsRemainingTimeHidden => false;

        protected override void HourlyTick()
        {
            if(!QuestGiver.IsAlive)
            {
                CompleteQuestWithCancel();
            }
            else if(QuestGiver.CurrentSettlement != Settlement)
            {
                if(Settlement != null)
                {
                    RemoveTrackedObject(Settlement);
                }
                Settlement = QuestGiver.CurrentSettlement;
            }

            if(Settlement != null && !IsTracked(Settlement))
            {
                AddTrackedObject(Settlement);
            }
        }

        protected override void InitializeQuestOnGameLoad()
        {
            if (!HeroList.Contains(QuestGiver))
            {
                HeroList.Add(QuestGiver);
            }
        }

        protected override void SetDialogs()
        {
            Campaign.Current.ConversationManager.AddDialogFlow(TaskFlow());
        }

        private DialogFlow TaskFlow()
        {
            return DialogFlow.CreateDialogFlow("lord_start", 1100).NpcLine(new TextObject("{=Dramalord291}Finally you are here!")).Condition(() => Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == QuestGiver)
                .PlayerLine(new TextObject("{=Dramalord292}Here I am. What is the urgent matter you asked me to take care about?"))
                .BeginNpcOptions()
                .NpcOption(new TextObject("{=Dramalord293}The matter... was resolved by someone very skillful meanwhile."), () => Info.GetHeroHorny(Hero.OneToOneConversationHero) < DramalordMCM.Get.MinHornyForIntercourse).Consequence(() => QuestFail())
                .NpcOption(new TextObject("{=Dramalord294}Follow me. I would like to make use of your resourcefulness..."), () => Info.GetHeroHorny(Hero.OneToOneConversationHero) >= DramalordMCM.Get.MinHornyForIntercourse).Consequence(() => QuestSuccess())
                .EndNpcOptions()
                .CloseDialog();
        }
        protected void QuestSuccess()
        {
            Consequences.NpcAcceptedDate();
            HeroList.Remove(QuestGiver);
            CompleteQuestWithSuccess();
        }

        protected void QuestFail()
        {
            TextObject banner = new TextObject("{=Dramalord290}{HERO.LINK} is disappointed by your neglection of their matter.");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            Info.ChangeEmotionToHeroBy(QuestGiver, Hero.MainHero, -DramalordMCM.Get.EmotionalLossCaughtDate);
            MBInformationManager.AddQuickInformation(banner, 1000, QuestGiver.CharacterObject, "event:/ui/notification/relation");
            HeroList.Remove(QuestGiver);
            CompleteQuestWithFail();
        }
    }
}
