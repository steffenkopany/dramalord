using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;

namespace Dramalord.Actions
{
    internal static class LeaveClanAction
    {
        internal static void Apply(Hero hero)
        {
            Kingdom? kingdom = hero.MapFaction as Kingdom;
            if (kingdom != null && kingdom.RulingClan != null && kingdom.RulingClan.Leader == hero)
            {
                Campaign.Current.KingdomManager.AbdicateTheThrone(kingdom);
            }

            if (hero.Clan != null && hero.Clan.Leader == hero)
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
                hero.CompanionOf = null;
            }

            hero.Clan = null;
        }
    }
}
