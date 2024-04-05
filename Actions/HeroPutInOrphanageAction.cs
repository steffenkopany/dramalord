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
    internal static class HeroPutInOrphanageAction
    {
        internal static void Apply(Hero hero, Hero child)
        {
            if (Info.ValidateHeroInfo(hero))
            {
                if(child.Father == Hero.MainHero)
                {
                    TextObject title = new TextObject("{=Dramalord137}Take care of your child");
                    TextObject text = new TextObject("{=Dramalord138}{HERO.LINK} asks you to take {CHILD.LINK} into your care to protect them from {SPOUSE.LINK}.");
                    TextObject banner = new TextObject("{=Dramalord145}You took {HERO.LINK} into your care.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, text);
                    StringHelpers.SetCharacterProperties("CHILD", child.CharacterObject, text);
                    if(hero.Spouse != null)
                    {
                        StringHelpers.SetCharacterProperties("SPOUSE", hero.Spouse.CharacterObject, text);
                    }
                    else
                    {
                        text.SetTextVariable("SPOUSE.LINK", new TextObject("{=Dramalord146}danger"));
                    }
                    StringHelpers.SetCharacterProperties("HERO", child.CharacterObject, banner);

                    Notification.DrawMessageBox(
                            title,
                            text,
                            true,
                            () => {

                                child.Clan = Hero.MainHero.Clan; 
                                child.UpdateHomeSettlement();
                                MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                            },
                            () => {

                                Info.ChangeEmotionToHeroBy(hero, child.Father, -DramalordMCM.Get.EmotionalLossBreakup);
                                MakeOrphan(hero, child);
                            }
                        );
                }
                else
                {
                    MakeOrphan(hero, child);
                }
            }
        }

        private static void MakeOrphan(Hero hero, Hero child)
        {
            Hero father = child.Father;
            Hero mother = child.Mother;
            child.Father = null;
            child.Mother = null;
            father.Children.Remove(child);
            mother.Children.Remove(child);
            child.Clan = null;
            child.UpdateHomeSettlement();
            child.SetNewOccupation(Occupation.Wanderer);
            //MakeHeroFugitiveAction.Apply(child);

            Info.AddOrphan(child);

            LogEntry.AddLogEntry(new EncyclopediaLogPutChildToOrphanage(hero, child));
            DramalordEvents.OnHeroesPutToOrphanage(hero, child);
        }
    } 
}
