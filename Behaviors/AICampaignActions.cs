using Dramalord.Actions;
using Dramalord.Data;
using System.Collections.Generic;
using System.Reflection;
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
                                if (spouse.GetHeroTraits().Mercy < 0 && spouse.Clan != null && spouse.Clan == hero.Clan && spouse.Clan.Leader == spouse)
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

                if(!Info.GetHeroHasToy(hero) && !hero.IsPrisoner)
                {
                    Hero? flirt = AICampaignHelper.ScopeSurroundingsForFlirt(hero, Hero.MainHero);
                    Hero? partner = AICampaignHelper.ScopeSurroundingsForPartner(hero, Hero.MainHero, false);
                    Hero? witness = AICampaignHelper.ScopeSurroundingsForPartner(hero, partner, false);

                    if (flirt != null)
                    {
                        HeroFlirtAction.Apply(hero, flirt);
                        if(partner != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(hero, flirt, partner, WitnessType.Flirting);
                        }
                    }

                    if(partner != null)
                    {
                        if (AICampaignHelper.WantsToDivorceFrom(hero, partner))
                        {
                            HeroDivorceAction.Apply(hero, partner);
                        }
                        else if (AICampaignHelper.WantsToBreakUpWith(hero, partner))
                        {
                            HeroBreakupAction.Apply(hero, partner);
                        }

                        if (AICampaignHelper.CanAdoptFromOrphanage(hero, partner))
                        {
                            HeroAdoptAction.Apply(hero, partner);
                        }

                        if (hero.Spouse == null && partner.Spouse == null && (hero.Clan != null || partner.Clan != null) && Info.GetEmotionToHero(hero, partner) >= DramalordMCM.Get.MinEmotionForMarriage && Info.GetEmotionToHero(partner, hero) >= DramalordMCM.Get.MinEmotionForMarriage)
                        {
                            HeroMarriageAction.Apply(hero, partner);
                        }

                        CompleteDateActions(hero, partner, witness);
                    }
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

        internal static void CompleteDateActions(Hero hero, Hero target, Hero? heroWitness)
        {
            if (AICampaignHelper.WantsToDateWith(hero, target))
            {
                HeroAffairAction.Apply(hero, target);

                Hero? targetWitness = AICampaignHelper.ScopeSurroundingsForPartner(target, hero, false);

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

                    if(heroWitness != null)
                    {
                        if (target != heroWitness && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(hero, target, hero, WitnessType.Intercourse);

                            if (heroWitness.GetHeroTraits().Calculating < 0 && heroWitness.GetHeroTraits().Mercy < 0)
                            {
                                HeroKillAction.Apply(heroWitness, hero, hero, KillReason.Intercourse);
                                return;
                            }
                            else if (AICampaignHelper.WantsToDivorceFrom(heroWitness, hero))
                            {
                                HeroDivorceAction.Apply(heroWitness, hero);
                                if (heroWitness.GetHeroTraits().Mercy < 0 && heroWitness.Clan != null && heroWitness.Clan == hero.Clan && heroWitness.Clan.Leader == heroWitness)
                                {
                                    HeroLeaveClanAction.Apply(hero, false, heroWitness);
                                }
                                return;
                            }
                            else if (AICampaignHelper.WantsToBreakUpWith(heroWitness, hero))
                            {
                                HeroBreakupAction.Apply(heroWitness, hero);
                            }
                        }
                    }

                    if(targetWitness != null)
                    {
                        if (hero != targetWitness && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(target, hero, targetWitness, WitnessType.Intercourse);

                            if (targetWitness.GetHeroTraits().Calculating < 0 && targetWitness.GetHeroTraits().Mercy < 0)
                            {
                                HeroKillAction.Apply(targetWitness, target, hero, KillReason.Intercourse);
                                return;
                            }
                            else if (AICampaignHelper.WantsToDivorceFrom(targetWitness, target))
                            {
                                HeroDivorceAction.Apply(targetWitness, target);
                                if (targetWitness.GetHeroTraits().Mercy < 0 && targetWitness.Clan != null && targetWitness.Clan == target.Clan && targetWitness.Clan.Leader == targetWitness)
                                {
                                    HeroLeaveClanAction.Apply(target, false, targetWitness);
                                }
                                return;
                            }
                            else if (AICampaignHelper.WantsToBreakUpWith(targetWitness, target))
                            {
                                HeroBreakupAction.Apply(targetWitness, target);
                            }
                        }
                    }
                }
                else
                {
                    if(heroWitness != null)
                    {
                        if (target != heroWitness && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(hero, target, heroWitness, WitnessType.Dating);
                        }
                    }

                    if(targetWitness != null)
                    {
                        if (hero != targetWitness && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(target, hero, targetWitness, WitnessType.Dating);
                        }
                    }
                }
            }
        }
    }
}
