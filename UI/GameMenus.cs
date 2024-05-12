using Dramalord.Actions;
using Dramalord.Data;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.UI
{
    internal static class GameMenus
    {
        internal static void AddGameMenus(CampaignGameStarter gameStarter)
        {
            gameStarter.AddGameMenuOption("town_backstreet", "dl_orphanage_menu", "Visit Orphanage", ConditionOrphanageAvailable, ConsequenceOrphanageAvailable, false, 2, false, null);
        }

        internal static bool ConditionOrphanageAvailable(MenuCallbackArgs args)
        {
            args.IsEnabled = Hero.MainHero.Spouse != null && CampaignTime.Now.ToDays - Info.GetLastAdoption(Hero.MainHero, Hero.MainHero.Spouse) > DramalordMCM.Get.WaitBetweenAdopting && Info.GetOrphanCount() > 0;
            args.Tooltip = new TextObject("Visit the orphanage to adopt a child", null);
            if (Hero.MainHero.Spouse == null)
                return MenuHelper.SetOptionProperties(args, false, true, new TextObject("You have to be married in order to adopt a child"));
            if (Info.GetOrphanCount() == 0)
                return MenuHelper.SetOptionProperties(args, false, true, new TextObject("There are currently no children in the orphanage"));
            if (CampaignTime.Now.ToDays - Info.GetLastAdoption(Hero.MainHero, Hero.MainHero.Spouse) <= DramalordMCM.Get.WaitBetweenAdopting)
            {
                TextObject obj = new TextObject("You have to wait {DAYS} days between adoptions");
                obj.SetTextVariable("DAYS", DramalordMCM.Get.WaitBetweenAdopting);
                return MenuHelper.SetOptionProperties(args, false, true, obj);
            }
            

            return true;
            
        }

        internal static void ConsequenceOrphanageAvailable(MenuCallbackArgs args)
        {
            TextObject title = new TextObject("Orphanage Menu");
            TextObject text = new TextObject("There are currently {NUMBER} children in the orphanage. Would you like to adopt a child?");
            text.SetTextVariable("NUMBER", Info.GetOrphanCount());

            InformationManager.ShowInquiry(new InquiryData(title.ToString(), text.ToString(), true, true, GameTexts.FindText("str_ok").ToString(), GameTexts.FindText("str_no").ToString(), OrphanageBoyGirlPick, null, "event:/ui/notification/relation"));
        }

        internal static void OrphanageBoyGirlPick()
        {
            TextObject title = new TextObject("Orphanage Menu");
            TextObject text = new TextObject("Would you like to adopt a boy or a girl?");
            InformationManager.ShowInquiry(new InquiryData(title.ToString(), text.ToString(), true, true, new TextObject("boy").ToString(), new TextObject("girl").ToString(), PlayerAdoptBoy, PlayerAdoptGirl, "event:/ui/notification/relation"));
        }

        internal static void PlayerAdoptBoy()
        {
            Hero? orphan = Info.PullMaleOrphan();
            if(orphan != null)
            {
                orphan.Mother = (Hero.MainHero.IsFemale) ? Hero.MainHero : Hero.MainHero.Spouse;
                orphan.Father = (orphan.Mother == Hero.MainHero) ? Hero.MainHero.Spouse : Hero.MainHero;
                orphan.Clan = Clan.PlayerClan;
                orphan.SetNewOccupation(Occupation.Lord);
                orphan.SetName(orphan.FirstName, orphan.FirstName);


                orphan.UpdateHomeSettlement();
                TeleportHeroAction.ApplyDelayedTeleportToSettlement(orphan, orphan.HomeSettlement);
                Info.SetLastAdoption(Hero.MainHero, Hero.MainHero.Spouse, CampaignTime.Now.ToDays);

                TextObject title = new TextObject("Orphanage Menu");
                TextObject text = new TextObject("You have adopted {AGE} years old {HERO}");
                text.SetTextVariable("AGE", (int)orphan.Age);
                text.SetTextVariable("HERO", orphan.Name);
                InformationManager.ShowInquiry(new InquiryData(title.ToString(), text.ToString(), true, false, GameTexts.FindText("str_ok").ToString(), null, null, null, "event:/ui/notification/relation"));
            }
            else
            {
                TextObject title = new TextObject("Orphanage Menu");
                TextObject text = new TextObject("There are currently no boys in the orphanage");
                InformationManager.ShowInquiry(new InquiryData(title.ToString(), text.ToString(), true, false, GameTexts.FindText("str_ok").ToString(), null, null, null, "event:/ui/notification/relation"));
            }
        }

        internal static void PlayerAdoptGirl()
        {
            Hero? orphan = Info.PullFemaleOrphan();
            if (orphan != null)
            {
                orphan.Mother = (Hero.MainHero.IsFemale) ? Hero.MainHero : Hero.MainHero.Spouse;
                orphan.Father = (orphan.Mother == Hero.MainHero) ? Hero.MainHero.Spouse : Hero.MainHero;
                orphan.Clan = Clan.PlayerClan;
                orphan.SetNewOccupation(Occupation.Lord);
                orphan.SetName(orphan.FirstName, orphan.FirstName);


                orphan.UpdateHomeSettlement();
                TeleportHeroAction.ApplyDelayedTeleportToSettlement(orphan, orphan.HomeSettlement);
                Info.SetLastAdoption(Hero.MainHero, Hero.MainHero.Spouse, CampaignTime.Now.ToDays);

                TextObject title = new TextObject("Orphanage Menu");
                TextObject text = new TextObject("You have adopted {AGE} years old {HERO}");
                text.SetTextVariable("AGE", (int)orphan.Age);
                text.SetTextVariable("HERO", orphan.Name);
                InformationManager.ShowInquiry(new InquiryData(title.ToString(), text.ToString(), true, false, GameTexts.FindText("str_ok").ToString(), null, null, null, "event:/ui/notification/relation"));
            }
            else
            {
                TextObject title = new TextObject("Orphanage Menu");
                TextObject text = new TextObject("There are currently no girls in the orphanage");
                InformationManager.ShowInquiry(new InquiryData(title.ToString(), text.ToString(), true, false, GameTexts.FindText("str_ok").ToString(), null, null, null, "event:/ui/notification/relation"));
            }
        }
    }
}
