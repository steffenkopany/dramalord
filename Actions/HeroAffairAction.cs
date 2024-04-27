using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroAffairAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target))
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

                if (target == Hero.MainHero || hero == Hero.MainHero)
                {
                    Hero otherHero = (hero == Hero.MainHero) ? target : hero;

                    TextObject banner = new TextObject("{=Dramalord256}You met in private with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
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
