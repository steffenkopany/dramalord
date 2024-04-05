using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class HeroAffairAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target) && Info.GetIsCoupleWithHero(hero, target))
            {
                Info.SetLastPrivateMeeting(hero, target, CampaignTime.Now.ToDays);
                Info.SetLastPrivateMeeting(target, hero, CampaignTime.Now.ToDays);
                Info.SetLastDaySeen(hero, target, CampaignTime.Now.ToDays);
                Info.SetLastDaySeen(target, hero, CampaignTime.Now.ToDays);

                int score = Info.GetTraitscoreToHero(hero, target);
                if (score != 0)
                {
                    Info.ChangeHeroHornyBy(hero, (score > 0) ? score * Info.GetHeroLibido(hero) : 0);
                    Info.ChangeHeroHornyBy(target, (score > 0) ? score * Info.GetHeroLibido(hero) : 0);
                    Info.ChangeEmotionToHeroBy(hero, target, score);
                    Info.ChangeEmotionToHeroBy(target, hero, score);
                }

                LogEntry.AddLogEntry(new LogAffairMeeting(hero, target));
                DramalordEvents.OnHeroesAffairMeeting(hero, target);
            }
        }
    }
}
