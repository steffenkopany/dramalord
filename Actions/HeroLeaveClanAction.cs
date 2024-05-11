using Dramalord.Data;
using Helpers;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class HeroLeaveClanAction
    {
        internal static void Apply(Hero hero, Hero causedBy)
        {
            Clan oldClan = hero.Clan;
            if(oldClan == null)
            {
                return;
            }

            Kingdom? kingdom = hero.MapFaction as Kingdom;
            if (kingdom != null && kingdom.RulingClan != null && kingdom.RulingClan.Leader == hero)
            {
                Campaign.Current.KingdomManager.AbdicateTheThrone(kingdom);
            }

            if (hero.Clan.Leader == hero)
            {
                ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(hero.Clan);
            }

            if (hero.GovernorOf != null)
            {
                ChangeGovernorAction.RemoveGovernorOf(hero);
            }

            if (hero.PartyBelongedTo != null)
            {
                MobileParty party = hero.PartyBelongedTo;
                if (party.Army != null && party.Army.LeaderParty == party)
                {
                    DisbandArmyAction.ApplyByUnknownReason(party.Army);
                }
                party.Army = null;

                if (party.Party.IsActive && party.Party.LeaderHero == hero)
                {
                    DisbandPartyAction.StartDisband(party);
                    party.Party.SetCustomOwner(null);
                    DestroyPartyAction.Apply(null, party); // test
                }
            }

            hero.Clan = null;
            if(hero.BornSettlement == null)
            {
                hero.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }

            hero.CompanionOf = null;
            hero.SetNewOccupation(Occupation.Wanderer);
            hero.UpdateHomeSettlement();
            CampaignEventDispatcher.Instance.OnHeroChangedClan(hero, oldClan);

            if(hero != causedBy && causedBy == Hero.MainHero)
            {
                TextObject textObject = new TextObject("{=Dramalord319}You banished {HERO.LINK} from your clan.");
                StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
            }

            if (DramalordMCM.Get.ClanOutput)
            {
                LogEntry.AddLogEntry(new EncyclopediaLogLeaveClan(hero, oldClan, causedBy));
            }
                
            DramalordEvents.OnHeroesLeaveClan(hero, oldClan, causedBy);
        }
    }
}
