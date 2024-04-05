using Dramalord.Actions;
using Dramalord.Data;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Dramalord.Behaviors
{
    internal static class AICampaignActions
    {
        internal static void DailyHeroUpdate(Hero hero)
        {
            if (hero != Hero.MainHero && Info.IsHeroLegit(hero) && !hero.IsFugitive)
            {
                Info.ResetFlirtCountWithHero(hero, Hero.MainHero);
                Info.ForgetHeroes(hero);

                if (Info.GetHeroHasToy(hero))
                {
                    HeroToyAction.Apply(hero);
                }
                List<Hero> partners = AICampaignHelper.ScopeSurroundingsForPartners(hero, true);
                foreach (Hero target in partners)
                {
                    if (AICampaignHelper.WantsToDivorceFrom(hero, target))
                    {
                        HeroDivorceAction.Apply(hero, target);
                    }
                    else if (AICampaignHelper.WantsToBreakUpWith(hero, target))
                    {
                        HeroBreakupAction.Apply(hero, target);
                    }

                    if (AICampaignHelper.CanAdoptFromOrphanage(hero, target))
                    {
                        HeroAdoptAction.Apply(hero, target);
                    }
                }

                Hero? candidate = AICampaignHelper.GetPotentialMarriagePartner(hero);
                if (candidate != null)
                {
                    HeroMarriageAction.Apply(hero, candidate);
                }

                HeroOffspringData? offspring = AICampaignHelper.CanGiveBirth(hero);
                if (offspring != null)
                {
                    HeroBirthAction.Apply(hero, offspring);

                    Hero? child = AICampaignHelper.GetChildBornToday(hero);
                    if (child != null && hero.Spouse != null && child.Father != hero.Spouse)
                    {
                        Hero spouse = hero.Spouse;
                        if (spouse.CurrentSettlement == hero.CurrentSettlement)
                        {
                            HeroWitnessAction.Apply(hero, child, spouse, WitnessType.Bastard);
                            HeroPutInOrphanageAction.Apply(spouse, child);

                            if (spouse.GetHeroTraits().Calculating < 0 && spouse.GetHeroTraits().Mercy < 0)
                            {
                                HeroKillAction.Apply(spouse, hero, child, KillReason.Bastard);
                                return;
                            }
                            else
                            {
                                HeroDivorceAction.Apply(spouse, hero);
                                if(spouse.GetHeroTraits().Mercy < 0 && spouse.Clan != null && spouse.Clan == hero.Clan && spouse.Clan.Leader == spouse)
                                {
                                    HeroLeaveClanAction.Apply(hero, false, spouse); 
                                }
                                return;
                            }
                        }
                        else
                        {
                            if (child.Father != Hero.MainHero)
                            {
                                if (child.Father.Clan != null && hero.Clan != null && child.Father.Clan != hero.Clan && Info.GetIsCoupleWithHero(hero, child.Father) && Info.GetEmotionToHero(hero, child.Father) > Info.GetEmotionToHero(hero, spouse))
                                {
                                    HeroDivorceAction.Apply(hero, spouse);
                                    HeroLeaveClanAction.Apply(hero, true, hero);
                                    HeroJoinClanAction.Apply(hero, child.Father.Clan, true);
                                }
                                else if (hero.GetHeroTraits().Valor > 0)
                                {
                                    HeroDivorceAction.Apply(hero, spouse);
                                    HeroLeaveClanAction.Apply(hero, true, hero);
                                    return;
                                }
                            }
                            else
                            {
                                HeroPutInOrphanageAction.Apply(hero, child);
                            }
                        }
                    }
                }
            }
        }

        internal static void DailyRomance(Hero hero)
        {
            if (hero == Hero.MainHero || !Info.IsHeroLegit(hero) || Info.GetHeroHasToy(hero) || hero.IsFugitive || hero.IsPrisoner)
            {
                return;
            }

            List<Hero> targets = AICampaignHelper.ScopeSurroundingsForFlirts(hero, false);
            List<Hero> partners = AICampaignHelper.ScopeSurroundingsForPartners(hero, true);
            foreach (Hero target in targets)
            {
                HeroFlirtAction.Apply(hero, target);

                foreach (Hero partner in partners)
                {
                    if (target != partner && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                    {
                        HeroWitnessAction.Apply(hero, target, partner, WitnessType.Flirting);
                    }
                }
            }

            foreach (Hero target in partners)
            {
                if(target != Hero.MainHero)
                {
                    CompleteDateActions(hero, target);
                }
            }
        }

        internal static void DailyViolence(Hero hero)
        {
            if (hero == Hero.MainHero || !Info.IsHeroLegit(hero) || Info.GetHeroHasToy(hero) || hero.IsFugitive || hero.IsPrisoner)
            {
                return;
            }

            Hero? victim = AICampaignHelper.ScopePrisonersForVictim(hero, true);
            if (victim != null && AICampaignHelper.WillHaveIntercourse(hero, victim, true))
            {
                HeroIntercourseAction.Apply(hero, victim, true);

                if (AICampaignHelper.WillConceiveOffspring(hero, victim))
                {
                    HeroConceiveAction.Apply(hero, victim, true);
                }
            }
        }

        internal static void CompleteDateActions(Hero hero, Hero target)
        {
            if (AICampaignHelper.WantsToDateWith(hero, target))
            {
                HeroAffairAction.Apply(hero, target);

                List<Hero> heroPartners = AICampaignHelper.ScopeSurroundingsForPartners(hero, true);
                List<Hero> targetPartners = AICampaignHelper.ScopeSurroundingsForPartners(target, true);

                if (AICampaignHelper.NoticeUnknownPregnancy(hero, target))
                {
                    HeroWitnessAction.Apply(target, target, hero, WitnessType.Pregnancy);

                    if (hero != Hero.MainHero && hero.GetHeroTraits().Calculating < 0 && hero.GetHeroTraits().Mercy < 0)
                    {
                        HeroKillAction.Apply(hero, target, target, KillReason.Pregnancy);
                        return;
                    }
                    else if (hero != Hero.MainHero && AICampaignHelper.WantsToDivorceFrom(hero, target))
                    {
                        HeroDivorceAction.Apply(hero, target);
                        if (hero.GetHeroTraits().Mercy < 0 && hero.Clan != null && target.Clan == hero.Clan && hero.Clan.Leader == hero)
                        {
                            HeroLeaveClanAction.Apply(target, false, hero);
                        }
                        return;
                    }
                    else if (hero != Hero.MainHero && AICampaignHelper.WantsToBreakUpWith(hero, target))
                    {
                        HeroBreakupAction.Apply(hero, target);
                    }
                }
                else if (AICampaignHelper.NoticeUnknownPregnancy(target, hero))
                {
                    HeroWitnessAction.Apply(hero, hero, target, WitnessType.Pregnancy);

                    if (target.GetHeroTraits().Calculating < 0 && target.GetHeroTraits().Mercy < 0)
                    {
                        HeroKillAction.Apply(target, hero, hero, KillReason.Pregnancy);
                        return;
                    }
                    else if (AICampaignHelper.WantsToDivorceFrom(target, hero))
                    {
                        HeroDivorceAction.Apply(target, hero);
                        if (target.GetHeroTraits().Mercy < 0 && target.Clan != null && target.Clan == hero.Clan && target.Clan.Leader == target)
                        {
                            HeroLeaveClanAction.Apply(hero, false, target);
                        }
                        return;
                    }
                    else if (AICampaignHelper.WantsToBreakUpWith(target, hero))
                    {
                        HeroBreakupAction.Apply(target, hero);
                    }
                }
                else if (AICampaignHelper.WillHaveIntercourse(hero, target, false))
                {
                    HeroIntercourseAction.Apply(hero, target, false);

                    if (AICampaignHelper.WillConceiveOffspring(hero, target))
                    {
                        HeroConceiveAction.Apply(hero, target, false);
                    }

                    foreach (Hero partner in heroPartners)
                    {
                        if (target != partner && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(hero, target, hero, WitnessType.Intercourse);

                            if (partner.GetHeroTraits().Calculating < 0 && partner.GetHeroTraits().Mercy < 0)
                            {
                                HeroKillAction.Apply(partner, hero, hero, KillReason.Intercourse);
                                return;
                            }
                            else if (AICampaignHelper.WantsToDivorceFrom(partner, hero))
                            {
                                HeroDivorceAction.Apply(partner, hero);
                                if (partner.GetHeroTraits().Mercy < 0 && partner.Clan != null && partner.Clan == hero.Clan && partner.Clan.Leader == partner)
                                {
                                    HeroLeaveClanAction.Apply(hero, false, partner);
                                }
                                return;
                            }
                            else if (AICampaignHelper.WantsToBreakUpWith(partner, hero))
                            {
                                HeroBreakupAction.Apply(partner, hero);
                            }
                        }
                    }

                    foreach (Hero partner in targetPartners)
                    {
                        if (hero != partner && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(target, hero, partner, WitnessType.Intercourse);

                            if (partner.GetHeroTraits().Calculating < 0 && partner.GetHeroTraits().Mercy < 0)
                            {
                                HeroKillAction.Apply(partner, target, hero, KillReason.Intercourse);
                                return;
                            }
                            else if (AICampaignHelper.WantsToDivorceFrom(partner, target))
                            {
                                HeroDivorceAction.Apply(partner, target);
                                if (partner.GetHeroTraits().Mercy < 0 && partner.Clan != null && partner.Clan == target.Clan && partner.Clan.Leader == partner)
                                {
                                    HeroLeaveClanAction.Apply(target, false, partner);
                                }
                                return;
                            }
                            else if (AICampaignHelper.WantsToBreakUpWith(partner, target))
                            {
                                HeroBreakupAction.Apply(partner, target);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Hero partner in heroPartners)
                    {
                        if (target != partner && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(hero, target, partner, WitnessType.Dating);
                        }
                    }

                    foreach (Hero partner in targetPartners)
                    {
                        if (hero != partner && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(target, hero, partner, WitnessType.Dating);
                        }
                    }
                }
            }
        }
    }
}
