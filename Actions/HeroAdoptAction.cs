using Dramalord.Data;
using Helpers;
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
            Hero? adopted = Info.PullRandomOrphan();
            if (adopted != null && Info.ValidateHeroMemory(hero, target))
            {
                adopted.Mother = (hero.IsFemale) ? hero : target;
                adopted.Father = (hero.IsFemale) ? target : hero;
                adopted.Clan = hero.Clan;
                adopted.SetNewOccupation(hero.Occupation);

                if (hero.Occupation == Occupation.Lord)
                {
                    adopted.SetName(hero.FirstName, hero.FirstName);
                }

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

                if(DramalordMCM.Get.BirthOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogAdopted(hero, target, adopted));
                }
                        
                DramalordEvents.OnHeroesAdopted(hero, target, adopted);
            } 
        }
    }
}
