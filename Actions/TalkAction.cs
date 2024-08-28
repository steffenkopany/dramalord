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
        internal static bool Apply(Hero hero, Hero target, int changeValue = -1000)
        {
            HeroRelation heroRelation = hero.GetRelationTo(target);

            int heroAttraction = hero.GetAttractionTo(target) / 20;
            int tagetAttraction = target.GetAttractionTo(hero) / 20;

            hero.GetDesires().Horny += heroAttraction;
            target.GetDesires().Horny += tagetAttraction;

            int trustGain = (changeValue == -1000) ? hero.GetSympathyTo(target) * DramalordMCM.Instance.TrustGainMultiplier : changeValue * DramalordMCM.Instance.TrustGainMultiplier;

            heroRelation.Trust += trustGain;
            heroRelation.LastInteraction = CampaignTime.Now.ToDays;

            if (hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord020}You had a casual conversation with {HERO.LINK}. (Trust {NUM})");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBTextManager.SetTextVariable("NUM", ConversationHelper.FormatNumber(trustGain));
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            return true;
        }
    }
}
