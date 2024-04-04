using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroAdoptAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            if (Info.ValidateHeroMemory(hero, target))
            {
                Hero? adopted = Info.PullRandomOrphan();
                if (adopted != null)
                {
                    adopted.Mother = (hero.IsFemale) ? hero : target;
                    adopted.Father = (hero.IsFemale) ? target : hero;
                    adopted.Clan = hero.Clan;
                    adopted.SetNewOccupation(hero.Occupation);
                    adopted.UpdateHomeSettlement();
                    TeleportHeroAction.ApplyDelayedTeleportToSettlement(adopted, adopted.HomeSettlement);
                    Info.SetLastAdoption(hero, target, CampaignTime.Now.ToDays);

                    if (target == Hero.MainHero)
                    {
                        TextObject textObject = new TextObject("{=Dramalord133}{HERO.LINK} adopted {CHILD.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                        StringHelpers.SetCharacterProperties("CHILD", adopted.CharacterObject, textObject);
                        MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                    }

                    LogEntry.AddLogEntry(new EncyclopediaLogAdopted(hero, target, adopted));
                    DramalordEvents.OnHeroesAdopted(hero, target, adopted);
                }  
            }
        }
    }
}
