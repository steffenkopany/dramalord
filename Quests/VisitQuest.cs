using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Quests
{
    internal class VisitQuest : QuestBase
    {
        //public override bool IsSpecialQuest => true;

        public VisitQuest(Hero questGiver) : base("DramalordVisitQuest", questGiver, CampaignTime.DaysFromNow(7), 0)
        {
            if (!IsTracked(questGiver))
            {
                AddTrackedObject(questGiver);
            }

            TextObject txt = new TextObject("{=Dramalord303}{HERO.LINK} asks you to find them, as they have an urgent matter to discuss. Will you make it in time?");
            StringHelpers.SetCharacterProperties("HERO", questGiver.CharacterObject, txt);
            AddLog(txt);
        }

        public override TextObject Title
        {
            get 
            {
                TextObject txt = new TextObject("{=Dramalord304}{QUESTHERO} requests your presence.");
                MBTextManager.SetTextVariable("QUESTHERO", QuestGiver.Name);
                return txt;
            }
        }
        

        public override bool IsRemainingTimeHidden => false;

        protected override void HourlyTick()
        {
            //nothing to do
        }

        public override void OnCanceled()
        {
            DramalordQuests.Instance.RemoveLoverQuest(QuestGiver);
        }

        protected override void InitializeQuestOnGameLoad()
        {
            VisitQuest? quest = DramalordQuests.Instance.GetLoverQuest(QuestGiver);
            if (quest == null)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Could not find quest of {QuestGiver.Name}", new Color(1f, 0f, 0f)));
            }
        }

        public override bool IsSpecialQuest => true;
        protected override void SetDialogs()
        {

        }

        internal void QuestSuccess()
        {
            CompleteQuestWithSuccess();

            HeroRelation heroRelation = QuestGiver.GetRelationTo(Hero.MainHero);

            int sympathy = QuestGiver.GetSympathyTo(Hero.MainHero);
            int heroAttraction = QuestGiver.GetAttractionTo(Hero.MainHero) / 10;
            int tagetAttraction = Hero.MainHero.GetAttractionTo(QuestGiver) / 10;

            QuestGiver.GetDesires().Horny += heroAttraction;
            Hero.MainHero.GetDesires().Horny += tagetAttraction;

            heroRelation.UpdateLove();
            int loveGain = (sympathy + ((heroAttraction + tagetAttraction) / 20)) * DramalordMCM.Instance.LoveGainMultiplier;
            int trustGain = sympathy * DramalordMCM.Instance.TrustGainMultiplier;
            heroRelation.Love += loveGain;
            heroRelation.Trust += trustGain;

            TextObject banner = new TextObject("{=Dramalord305}{HERO.LINK} is very happy you fullfilled her request... (Love {NUM}, Trust {NUM2})");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber(loveGain));
            MBTextManager.SetTextVariable("NUM2", ConversationHelper.FormatNumber(trustGain));
            MBInformationManager.AddQuickInformation(banner, 0, QuestGiver.CharacterObject, "event:/ui/notification/relation");

            DramalordQuests.Instance.RemoveLoverQuest(QuestGiver);

            ConversationHelper.ConversationEndedIntention = new HeroIntention(IntentionType.Intercourse, QuestGiver, -1);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal void QuestFail()
        {
            CompleteQuestWithFail();

            HeroPersonality personality = QuestGiver.GetPersonality();

            float loveDamage = -100 / 5;
            float trustDamage = -100 / 3;

            float agreeFactor = ((float)personality.Agreeableness) / 100f;
            float openFactor = ((float)personality.Openness) / 100f;
            float extroFactor = ((float)personality.Extroversion) / 100f;
            float neuroFactor = ((float)personality.Neuroticism) / 100f;
            float conscFactor = ((float)personality.Conscientiousness) / 100f;

            loveDamage -= loveDamage * agreeFactor;
            loveDamage -= loveDamage * openFactor;

            trustDamage += trustDamage * neuroFactor;
            trustDamage += trustDamage * conscFactor;

            QuestGiver.GetRelationTo(Hero.MainHero).UpdateLove();
            QuestGiver.GetRelationTo(Hero.MainHero).Trust += (int)trustDamage;
            QuestGiver.GetRelationTo(Hero.MainHero).Love += (int)loveDamage;

            TextObject banner = new TextObject("{=Dramalord302}{HERO.LINK} is disappointed by your neglection of their matter.. (Love {NUM}, Trust {NUM2})");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber((int)loveDamage));
            MBTextManager.SetTextVariable("NUM2", ConversationHelper.FormatNumber((int)trustDamage));
            MBInformationManager.AddQuickInformation(banner, 0, QuestGiver.CharacterObject, "event:/ui/notification/relation");

            DramalordQuests.Instance.RemoveLoverQuest(QuestGiver);
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
