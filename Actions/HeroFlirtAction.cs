using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    internal static class HeroFlirtAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if(Info.ValidateHeroMemory(hero, target))
            {
                int score = Info.GetTraitscoreToHero(hero, target);
                if (score != 0 && Info.ValidateHeroInfo(hero) && Info.ValidateHeroInfo(target))
                {
                    /*
                    Info.ChangeHeroHornyBy(target, (score > 0) ? score : 0);
                    Info.ChangeHeroHornyBy(hero, (score > 0) ? score : 0);
                    */
                    Info.ChangeEmotionToHeroBy(hero, target, score);
                }

                Info.SetLastDaySeen(hero, target, CampaignTime.Now.ToDays);

                float emotionHero = Info.GetEmotionToHero(hero, target);

                if (hero != Hero.MainHero && !Info.IsCoupleWithHero(hero, target) && emotionHero >= DramalordMCM.Get.MinEmotionForDating && (!Info.IsCloseRelativeTo(hero, target) || !DramalordMCM.Get.ProtectFamily))
                {
                    Info.SetIsCoupleWithHero(hero, target, true);
                    //Info.ChangeEmotionToHeroBy(hero, target, DramalordMCM.Get.EmotionalWinAffair);
                    if (DramalordMCM.Get.AffairOutput)
                    {
                        LogEntry.AddLogEntry(new EncyclopediaLogStartAffair(hero, target));
                    }
                    
                    DramalordEvents.OnHeroesFlirt(hero, target, true);
                }
                else
                {
                    if (DramalordMCM.Get.FlirtOutput)
                    {
                        LogEntry.AddLogEntry(new LogFlirt(hero, target));
                    }
                        
                    DramalordEvents.OnHeroesFlirt(hero, target, false);
                }
            }
        }
    }  
}
