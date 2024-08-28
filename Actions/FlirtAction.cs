using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class FlirtAction
    {
        internal static bool Apply(Hero hero, Hero target, int changeValue = -1000)
        {
            HeroRelation heroRelation = hero.GetRelationTo(target);

            int heroAttraction = hero.GetAttractionTo(target);
            int tagetAttraction = target.GetAttractionTo(hero);

            hero.GetDesires().Horny += heroAttraction / 10;
            target.GetDesires().Horny += tagetAttraction / 10;

            int loveGain = 0;
            if((heroAttraction >= DramalordMCM.Instance.MinAttraction && tagetAttraction >= DramalordMCM.Instance.MinAttraction) || hero == Hero.MainHero || target == Hero.MainHero)
            {
                loveGain = (changeValue == -1000) ? hero.GetSympathyTo(target) * DramalordMCM.Instance.LoveGainMultiplier : changeValue * DramalordMCM.Instance.LoveGainMultiplier;
                heroRelation.Love += loveGain;
            }
            
            heroRelation.LastInteraction = CampaignTime.Now.ToDays;

            if(hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord019}You flirted with {HERO.LINK}. (Love {NUM})");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber(loveGain));
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            return true;
        }
    }
}
