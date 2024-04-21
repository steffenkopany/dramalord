using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class HeroAffairAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target) /*&& Info.ValidateHeroMemory(target, hero)*/)
            {
                Info.SetLastPrivateMeeting(hero, target, CampaignTime.Now.ToDays);
                Info.SetLastDaySeen(hero, target, CampaignTime.Now.ToDays);

                int score = Info.GetTraitscoreToHero(hero, target);
                if (score != 0 && Info.ValidateHeroInfo(hero) && Info.ValidateHeroInfo(target))
                {
                    Info.ChangeHeroHornyBy(hero, (score > 0) ? score : 0);
                    Info.ChangeHeroHornyBy(target, (score > 0) ? score : 0);
                    Info.ChangeEmotionToHeroBy(hero, target, score);
                }

                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new LogAffairMeeting(hero, target));
                }

                DramalordEvents.OnHeroesAffairMeeting(hero, target);
            }
        }
    }
}
