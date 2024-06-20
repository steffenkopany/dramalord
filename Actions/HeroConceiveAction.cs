using Dramalord.Data;
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
        internal static void Apply(Hero mother, Hero father)
        {
            if(mother.IsDramalordLegit() && father.IsDramalordLegit())
            {
                if(!mother.MakeDramalordPregnant(father))
                {
                    return;
                }

                if (mother == Hero.MainHero)
                {
                    TextObject banner = new TextObject("{=Dramalord143}You got pregnant from {HERO.LINK}");
                    StringHelpers.SetCharacterProperties("HERO", father.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, father.CharacterObject, "event:/ui/notification/relation");
                }
                else if (father == Hero.MainHero)
                {
                    TextObject banner = new TextObject("{=Dramalord144}{HERO.LINK} got pregnant from you.");
                    StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, mother.CharacterObject, "event:/ui/notification/relation");
                }

                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogConceived(mother, father));
                }

                DramalordEventCallbacks.OnHeroesConceive(mother, father);
            }
        }
    }
}
