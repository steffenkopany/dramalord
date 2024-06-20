using Dramalord.Data;
using Dramalord.Data.Deprecated;
using Dramalord.UI;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroPutInOrphanageAction
    {
        internal static void Apply(Hero hero, Hero child)
        {
            if(child.Father.IsDramalordLegit())
            {
                if (child.Father == Hero.MainHero)
                {
                    TextObject title = new TextObject("{=Dramalord137}Take care of your child");
                    TextObject text = new TextObject("{=Dramalord138}{HERO.LINK} asks you to take {CHILD.LINK} into your care to protect them from {SPOUSE.LINK}.");
                    TextObject banner = new TextObject("{=Dramalord145}You took {HERO.LINK} into your care.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, text);
                    StringHelpers.SetCharacterProperties("CHILD", child.CharacterObject, text);
                    if (hero.Spouse != null)
                    {
                        StringHelpers.SetCharacterProperties("SPOUSE", hero.Spouse.CharacterObject, text);
                    }
                    else
                    {
                        text.SetTextVariable("SPOUSE.LINK", new TextObject("{=Dramalord146}danger"));
                    }
                    StringHelpers.SetCharacterProperties("HERO", child.CharacterObject, banner);

                    Campaign.Current.SetTimeSpeed(0);
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

                                hero.GetDramalordFeelings(child.Father).Emotion -= DramalordMCM.Get.EmotionalLossBreakup;
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
            father.Children.Remove(child);
            mother.Children.Remove(child);
            Clan oldClan = child.Clan;
            child.Clan = null;
            if (child.BornSettlement == null)
            {
                child.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }
            child.SetNewOccupation(Occupation.Wanderer);
            child.UpdateHomeSettlement();
            if(oldClan != null)
            {
                CampaignEventDispatcher.Instance.OnHeroChangedClan(child, oldClan);
            }

            DramalordOrphanage.AddOrphan(child.CharacterObject);
            //child.SetBirthDay(CampaignTime.Days((float)CampaignTime.Now.ToDays - (CampaignTime.DaysInYear * 17 + (CampaignTime.DaysInYear-1)))); Testing onherocomesofage

            if (DramalordMCM.Get.BirthOutput)
            {
                LogEntry.AddLogEntry(new EncyclopediaLogPutChildToOrphanage(hero, child));
            }
                
            DramalordEventCallbacks.OnHeroesPutToOrphanage(hero, child);
        }
    } 
}
