using Dramalord.Actions;
using HarmonyLib;
using Helpers;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(MarriageAction), "HandleClanChangeAfterMarriageForHero")]
    public static class HandleClanChangeAfterMarriageForHeroPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool HandleClanChangeAfterMarriageForHero(ref Hero hero, ref Clan clanAfterMarriage)
        {
            if (!HeroMarriageAction.IsDramalordMarriage)
            {
                Clan clan = hero.Clan;
                if (hero.GovernorOf != null)
                {
                    ChangeGovernorAction.RemoveGovernorOf(hero);
                }

                if (hero.PartyBelongedTo != null)
                {
                    if (clan.Kingdom != clanAfterMarriage.Kingdom)
                    {
                        if (hero.PartyBelongedTo.Army != null)
                        {
                            if (hero.PartyBelongedTo.Army.LeaderParty == hero.PartyBelongedTo)
                            {
                                DisbandArmyAction.ApplyByUnknownReason(hero.PartyBelongedTo.Army);
                            }
                            else
                            {
                                hero.PartyBelongedTo.Army = null;
                            }
                        }

                        IFaction kingdom = clanAfterMarriage.Kingdom;
                        FactionHelper.FinishAllRelatedHostileActionsOfNobleToFaction(hero, kingdom ?? clanAfterMarriage);
                    }

                    MakeHeroFugitiveAction.Apply(hero);
                }

                hero.Clan = clanAfterMarriage;
                foreach (Hero hero2 in clan.Heroes)
                {
                    hero2.UpdateHomeSettlement();
                }

                foreach (Hero hero3 in clanAfterMarriage.Heroes)
                {
                    hero3.UpdateHomeSettlement();
                }
            }
            return false;
        }
    }
}
