using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class ToyAction
    {
        internal static void Apply(Hero hero)
        {
            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance.ToyBreakChance)
            {
                hero.GetDesires().HasToy = false;
                TextObject textObject = new TextObject("{=Dramalord297}{HERO.LINK}s toy broke!");
                StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 0, hero.CharacterObject, "event:/ui/notification/relation");
            }
            else
            {
                HeroRelation relation = hero.GetRelationTo(Hero.MainHero);
                //relation.UpdateLove();
                relation.Love += 1;
                hero.GetDesires().Horny += 1;
            }
        }
    }
}
