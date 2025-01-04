using Dramalord.LogItems;
using Helpers;
using Newtonsoft.Json.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class LeaveClanAction
    {
        internal static bool Apply(Hero hero, Hero cause, bool forMarriage)
        {
            Clan oldClan = hero.Clan;
            if (oldClan == null)
            {
                return false;
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
                else if (party.IsActive)
                {
                    party.MemberRoster.RemoveTroop(hero.CharacterObject);
                }
            }

            if (hero.IsPlayerCompanion)
            {
                Clan.PlayerClan.Companions.Remove(hero);
                hero.CompanionOf = null;
            }

            hero.Clan = null;
            if (hero.BornSettlement == null)
            {
                hero.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }

            hero.SetNewOccupation(Occupation.Wanderer);
            TextObject newName = new TextObject("{=28tWEFNi}{FIRSTNAME} the Wanderer");
            newName.SetTextVariable("FIRSTNAME", hero.FirstName);
            hero.SetName(newName, hero.FirstName);
            hero.UpdateHomeSettlement();
            CampaignEventDispatcher.Instance.OnHeroChangedClan(hero, oldClan);
            if(!forMarriage)
            {
                TeleportHeroAction.ApplyDelayedTeleportToSettlement(hero, hero.HomeSettlement);
            }

            if (oldClan == Clan.PlayerClan && hero != Hero.MainHero && cause == Hero.MainHero)
            {
                TextObject textObject = new TextObject("{=Dramalord081}You banished {HERO.LINK} from your clan.");
                StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
            }
            else if (oldClan == Clan.PlayerClan && hero != Hero.MainHero && cause == hero)
            {
                TextObject textObject = new TextObject("{=Dramalord082}{HERO.LINK} has left your clan.");
                StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 1000, hero.CharacterObject, "event:/ui/notification/relation");
            }

            if ((oldClan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new LeaveClanLog(hero, oldClan, cause));
            }

            return true;
        }
    }
}
