using Dramalord.Data;
using Dramalord.Data.Deprecated;
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
        internal static void Apply(Hero hero, Hero target, Hero orphan)
        {
            if (orphan != null && hero.IsDramalordLegit() && target.IsDramalordLegit())
            {
                orphan.Mother = (hero.IsFemale) ? hero : target;
                orphan.Father = (hero.IsFemale) ? target : hero;
                orphan.Clan = hero.Clan;
                orphan.SetNewOccupation(hero.Occupation);

                if (hero.Occupation == Occupation.Lord)
                {
                    orphan.SetName(orphan.FirstName, orphan.FirstName);
                }

                orphan.UpdateHomeSettlement();
                TeleportHeroAction.ApplyImmediateTeleportToSettlement(orphan, orphan.HomeSettlement);
                DramalordOrphanage.SetLastAdoptionDay(hero, (uint)CampaignTime.Now.ToDays);
                DramalordOrphanage.SetLastAdoptionDay(target, (uint)CampaignTime.Now.ToDays);

                if (hero.Clan == Clan.PlayerClan)
                {
                    TextObject textObject = new TextObject("{=Dramalord133}{HERO.LINK} adopted {CHILD.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    StringHelpers.SetCharacterProperties("CHILD", orphan.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                }

                if(DramalordMCM.Get.BirthOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogAdopted(hero, target, orphan));
                }
                        
                DramalordEventCallbacks.OnHeroesAdopted(hero, target, orphan);
            } 
        }
    }
}
