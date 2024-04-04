using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroConceiveAction
    {
        internal static void Apply(Hero hero, Hero target, bool byForce)
        {
            if (Info.ValidateHeroMemory(hero, target))
            {
                Hero mother = (hero.IsFemale) ? hero : target;
                Hero father = (target.IsFemale) ? hero : target;

                if (mother == Hero.MainHero)
                {
                    TextObject banner = new TextObject("{=Dramalord143}You got pregnant from {HERO.LINK}");
                    StringHelpers.SetCharacterProperties("HERO", father.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                }
                else if(father == Hero.MainHero)
                {
                    TextObject banner = new TextObject("{=Dramalord144}{HERO.LINK} got pregnant from you.");
                    StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                }

                Info.AddHeroOffspring(mother, father, byForce);
                mother.IsPregnant = true;

                LogEntry.AddLogEntry(new EncyclopediaLogConceived(hero, target, byForce));
                DramalordEvents.OnHeroesConceive(hero, target, byForce);
            }
        }
    }
}
