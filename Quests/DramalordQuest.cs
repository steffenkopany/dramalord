using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace Dramalord.Quests
{
    public abstract class DramalordQuest : QuestBase
    {
        public DramalordQuest(string questId, Hero questGiver, CampaignTime duration) : base(questId, questGiver, duration, 0)
        {
            if (!IsTracked(questGiver))
            {
                AddTrackedObject(questGiver);
            }
        }

        public override TextObject Title => GetTitle();

        public abstract TextObject Description { get; }

        protected override void OnTimedOut() => QuestTimeout();

        public override string SpecialQuestType => "DramalordQuest";

        public override bool IsRemainingTimeHidden => false;

        public abstract TextObject GetTitle(); 
        
        protected override void OnStartQuest() => QuestStartInit();

        public abstract void QuestStartInit();

        public abstract void QuestSuccess(Hero reason);

        public abstract void QuestFail(Hero reason);

        public abstract void QuestTimeout();
    }
}
