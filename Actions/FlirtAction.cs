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
        internal static bool Apply(Hero hero, Hero target)
        {
            HeroRelation heroRelation = hero.GetRelationTo(target);

            int heroAttraction = hero.GetAttractionTo(target) / 10;
            int tagetAttraction = target.GetAttractionTo(hero) / 10;

            hero.GetDesires().Horny += heroAttraction;
            target.GetDesires().Horny += tagetAttraction;

            //heroRelation.Tension += (heroAttraction + tagetAttraction);
            heroRelation.Love += hero.GetSympathyTo(target);
            heroRelation.LastInteraction = CampaignTime.Now.ToDays;

            if(hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord019}You flirted with {HERO.LINK}. (Love {NUM})");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber(hero.GetSympathyTo(target)));
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            return true;
        }
    }
}
