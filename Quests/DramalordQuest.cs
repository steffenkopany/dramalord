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

        protected override void OnTimedOut() => QuestTimeout();

        public override bool IsSpecialQuest => true;

        public override bool IsRemainingTimeHidden => false;

        public abstract TextObject GetTitle(); 
        
        protected override void OnStartQuest() => QuestStartInit();

        public abstract void QuestStartInit();

        public abstract void QuestSuccess(Hero reason);

        public abstract void QuestFail(Hero reason);

        public abstract void QuestTimeout();
    }
}
