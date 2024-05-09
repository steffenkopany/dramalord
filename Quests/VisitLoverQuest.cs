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
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class VisitLoverQuest : QuestBase
    {
        [SaveableField(2)]
        public static Dictionary<Hero, VisitLoverQuest> HeroList = new();

        //public override bool IsSpecialQuest => true;

        public VisitLoverQuest(Hero questGiver) : base("DramalordVisitLoverQuest", questGiver, CampaignTime.DaysFromNow(7), 0)
        {
            RelationshipChangeWithQuestGiver = Info.GetTraitscoreToHero(questGiver, Hero.MainHero) * 10;
            if(!IsTracked(questGiver))
            {
                AddTrackedObject(questGiver);
            }
            if(questGiver.CurrentSettlement != null)
            {
                SetDialogs();
                AddLog(new TextObject(questGiver.Name + " asks you to find them, as they have an urgent matter to discuss. Will you comply?"));
            }

            if(!HeroList.ContainsKey(questGiver))
            {
                HeroList.Add(questGiver, this);
            }
        }

        public override TextObject Title => new TextObject(QuestGiver.Name + " requests your presence");

        public override bool IsRemainingTimeHidden => false;

        protected override void HourlyTick()
        {
            if(!QuestGiver.IsAlive)
            {
                CompleteQuestWithCancel();
                HeroList.Remove(QuestGiver);
            }
        }

        public override void OnCanceled()
        {

        }

        protected override void InitializeQuestOnGameLoad()
        {
            
            if (!HeroList.ContainsKey(QuestGiver))
            {
                HeroList.Add(QuestGiver, this);
                Notification.PrintText(QuestGiver.Name + " quest added");
            }
            
        }

        public override bool IsSpecialQuest => true;
        protected override void SetDialogs()
        {
           
        }

        internal void QuestSuccess()
        {
            CompleteQuestWithSuccess();
            Consequences.NpcAcceptedDate();
            HeroList.Remove(QuestGiver);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal void QuestFail()
        {
            CompleteQuestWithFail();
            TextObject banner = new TextObject("{=Dramalord290}{HERO.LINK} is disappointed by your neglection of their matter.");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            Info.ChangeEmotionToHeroBy(QuestGiver, Hero.MainHero, -DramalordMCM.Get.EmotionalLossCaughtDate);
            MBInformationManager.AddQuickInformation(banner, 1000, QuestGiver.CharacterObject, "event:/ui/notification/relation");
            HeroList.Remove(QuestGiver);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        protected override void OnTimedOut()
        {
            QuestFail();
        }
    }
}
