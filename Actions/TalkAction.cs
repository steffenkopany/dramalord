using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class TalkAction
    {
        internal static bool Apply(Hero hero, Hero target)
        {
            HeroRelation heroRelation = hero.GetRelationTo(target);
            int sympathy = hero.GetSympathyTo(target);

            int heroAttraction = hero.GetAttractionTo(target) / 20;
            int tagetAttraction = target.GetAttractionTo(hero) / 20;

            hero.GetDesires().Horny += heroAttraction;
            target.GetDesires().Horny += tagetAttraction;

            //heroRelation.Tension += (heroAttraction + tagetAttraction) / 2;
            heroRelation.Trust += sympathy;
            heroRelation.LastInteraction = CampaignTime.Now.ToDays;

            if (hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord020}You had a casual conversation with {HERO.LINK}. (Trust {NUM})");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber(sympathy));
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            return true;
        }
    }
}
