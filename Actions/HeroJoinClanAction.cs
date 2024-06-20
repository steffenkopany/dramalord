using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroJoinClanAction
    {
        internal static void Apply(Hero hero, Clan clan)
        {
            if (clan == Clan.PlayerClan)
            {
                TextObject title = new TextObject("{=Dramalord426}Hero wants to join your clan");
                TextObject text = new TextObject("{=Dramalord427}{HERO.LINK} requests joining your clan. Will you accept?");
                TextObject banner = new TextObject("{=Dramalord428}You accepted {HERO.LINK} joining your clan.");
                StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, text);
                StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);

                Campaign.Current.SetTimeSpeed(0);
                Notification.DrawMessageBox(
                        title,
                        text,
                        true,
                        () => {

                            DoClanJoin(hero, clan);
                            MBInformationManager.AddQuickInformation(banner, 1000, hero.CharacterObject, "event:/ui/notification/relation");
                        },
                        () => {

                            hero.GetDramalordFeelings(Hero.MainHero).Emotion -= DramalordMCM.Get.EmotionalLossBreakup;
                            hero.ChangeRelationTo(Hero.MainHero, (DramalordMCM.Get.EmotionalLossBreakup / 2) * -1);
                        }
                    );
            }
            else
            {
                DoClanJoin(hero, clan);
            }
        }

        private static void DoClanJoin(Hero hero, Clan clan)
        {
            if (hero.Occupation == Occupation.Wanderer)
            {
                hero.SetName(hero.FirstName, hero.FirstName);
            }

            hero.Clan = clan;
            hero.UpdateHomeSettlement();
            hero.SetNewOccupation(Occupation.Lord);
            hero.ChangeState(Hero.CharacterStates.Active);

            foreach (Hero child in hero.Children)
            {
                if (child.IsChild && child.Clan == null)
                {
                    HeroPutInOrphanageAction.Apply(hero, child);
                }
            }

            if (DramalordMCM.Get.ClanOutput)
            {
                LogEntry.AddLogEntry(new EncyclopediaLogJoinClan(hero, clan));
            }

            DramalordEventCallbacks.OnHeroesJoinClan(hero, clan);
        }
    }
}
