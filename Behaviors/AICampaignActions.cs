using Dramalord.Actions;
using Dramalord.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;


namespace Dramalord.Behaviors
{
    internal static class AICampaignActions
    {
        internal static void DailyHeroUpdate(Hero hero)
        {
            if (hero != Hero.MainHero && Info.ValidateHeroInfo(hero) && !hero.IsFugitive)
            {
                Info.ValidateHeroMemory(hero, Hero.MainHero);

                if (hero.IsFemale)
                {
                    HeroOffspringData? offspring = Info.GetHeroOffspring(hero);
                    if (offspring != null)
                    {
                        if (CampaignTime.Now.ToDays - offspring.Conceived > DramalordMCM.Get.PregnancyDuration)
                        {
                            HeroBirthAction.Apply(hero, offspring); //DefaultDiplomacyModel.GetBaseRelation

                            Hero? child = hero.Children.FirstOrDefault(item => item.BirthDay.ToDays == CampaignTime.Now.ToDays);
                            if (child != null && hero.Spouse != null && child.Father != hero.Spouse)
                            {
                                Hero spouse = hero.Spouse;
                                if (spouse.CurrentSettlement == hero.CurrentSettlement && Info.ValidateHeroMemory(hero, spouse))
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
                                    if (child.Father != Hero.MainHero && Info.ValidateHeroMemory(hero, child.Father) && Info.ValidateHeroMemory(hero, spouse))
                                    {
                                        
                                        if (child.Father.Clan != null && hero.Clan != null && child.Father.Clan != hero.Clan && Info.IsCoupleWithHero(hero, child.Father) && Info.GetEmotionToHero(hero, spouse) < DramalordMCM.Get.MinEmotionForDating && Info.GetEmotionToHero(hero, child.Father) > DramalordMCM.Get.MinEmotionForDating)
                                        {
                                            HeroDivorceAction.Apply(hero, spouse);
                                            HeroLeaveClanAction.Apply(hero, true, hero);
                                            HeroJoinClanAction.Apply(hero, child.Father.Clan, true);
                                        }
                                        else if (hero.GetHeroTraits().Valor > 0 && Info.GetEmotionToHero(hero, spouse) < DramalordMCM.Get.MinEmotionForDating)
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

                if (Info.GetHeroHasToy(hero))
                {
                    HeroToyAction.Apply(hero);
                    return;
                }

                if (!hero.IsPrisoner)
                {
                    List<Hero> flirts = new();
                    List<Hero> partners = new();
                    List<Hero> prisoners = new();

                    ScopeSurroundings(hero, ref flirts, ref partners, ref prisoners);

                    // evil stuff first
                    if (hero.GetHeroTraits().Mercy < 0)
                    {
                        Hero? victim = (prisoners.Count > 0) ? prisoners[MBRandom.RandomInt(prisoners.Count)] : null;

                        if (victim != null && Info.GetHeroHorny(hero) >= DramalordMCM.Get.MinHornyForIntercourse)
                        {
                            HeroIntercourseAction.Apply(hero, victim, true);

                            Hero mother = hero.IsFemale ? hero : victim;
                            Hero father = victim.IsFemale ? hero : victim;

                            if (mother != father && !mother.IsPregnant && Info.CanGetPregnant(mother) && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
                            {
                                HeroConceiveAction.Apply(hero, victim, true);
                            }
                        }
                    }

                    //let keep it one each for now
                    flirts.Remove(Hero.MainHero);
                    Hero? flirt = (flirts.Count > 0) ? flirts[MBRandom.RandomInt(flirts.Count)] : null;

                    partners.Remove(Hero.MainHero);
                    Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

                    if(partner != null)
                    {
                        partners.Remove(partner);
                    }

                    Hero? witness = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

                    if (flirt != null)
                    {
                        HeroFlirtAction.Apply(hero, flirt);
                        if(partner != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(hero, flirt, partner, WitnessType.Flirting);
                        }
                    }

                    if(partner != null && Info.ValidateHeroMemory(hero, partner))
                    {
                        float heroEmotion = Info.GetEmotionToHero(hero, partner);

                        if (hero.Spouse == partner && heroEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                        {
                            HeroDivorceAction.Apply(hero, partner);
                        }
                        else if (hero.Spouse != partner && heroEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                        {
                            HeroBreakupAction.Apply(hero, partner);
                        }

                        if (hero.Spouse == partner && !Info.IsOrphanageEmpty())
                        {
                            if( (hero.IsFemale == partner.IsFemale) || (hero.IsFemale && hero.Age > DramalordMCM.Get.MaxFertilityAge) || (partner.IsFemale && partner.Age > DramalordMCM.Get.MaxFertilityAge))
                            {
                                if (CampaignTime.Now.ToDays - Info.GetLastAdoption(hero, partner) >= DramalordMCM.Get.WaitBetweenAdopting)
                                {
                                    HeroAdoptAction.Apply(hero, partner);
                                }
                            }
                        }

                        if (hero.Spouse == null && partner.Spouse == null && (hero.Clan != null || partner.Clan != null) && heroEmotion >= DramalordMCM.Get.MinEmotionForMarriage)
                        {
                            HeroMarriageAction.Apply(hero, partner);
                        }

                        CompleteDateActions(hero, partner, witness);
                    }
                }    
            }
        }

        internal static void CompleteDateActions(Hero hero, Hero target, Hero? heroWitness)
        {
            if (CampaignTime.Now.ToDays - Info.GetLastDate(hero, target) >= DramalordMCM.Get.DaysBetweenDates && Info.ValidateHeroInfo(target))
            {
                HeroAffairAction.Apply(hero, target);

                if (target.IsFemale && target.IsPregnant)
                {
                    HeroOffspringData? offspring = Info.GetHeroOffspring(target);
                    if (offspring != null && offspring.Father != hero && CampaignTime.Now.ToDays - offspring.Conceived >= DramalordMCM.Get.DaysUntilPregnancyVisible)
                    {
                        HeroWitnessAction.Apply(target, target, hero, WitnessType.Pregnancy);

                        float heroEmotion = Info.GetEmotionToHero(hero, target);

                        if (hero != Hero.MainHero && hero.GetHeroTraits().Calculating < 0 && hero.GetHeroTraits().Mercy < 0)
                        {
                            HeroKillAction.Apply(hero, target, target, KillReason.Pregnancy);
                            return;
                        }
                        else if (hero.Spouse == target && heroEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                        {
                            HeroDivorceAction.Apply(hero, target);
                            if (hero.GetHeroTraits().Mercy < 0 && hero.Clan != null && target.Clan == hero.Clan && hero.Clan.Leader == hero)
                            {
                                HeroLeaveClanAction.Apply(target, false, hero);
                            }
                            return;
                        }
                        else if (hero.Spouse != target && heroEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                        {
                            HeroBreakupAction.Apply(hero, target);
                            return;
                        }
                    }
                }

                if (hero.IsFemale && hero.IsPregnant)
                {
                    HeroOffspringData? offspring = Info.GetHeroOffspring(target);
                    if (offspring != null && offspring.Father != target && CampaignTime.Now.ToDays - offspring.Conceived >= DramalordMCM.Get.DaysUntilPregnancyVisible)
                    {
                        HeroWitnessAction.Apply(hero, hero, target, WitnessType.Pregnancy);

                        float targetEmotion = Info.GetEmotionToHero(target, hero);

                        if (target.GetHeroTraits().Calculating < 0 && target.GetHeroTraits().Mercy < 0)
                        {
                            HeroKillAction.Apply(target, hero, hero, KillReason.Pregnancy);
                            return;
                        }
                        else if (target.Spouse == hero && targetEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                        {
                            HeroDivorceAction.Apply(target, hero);
                            if (target.GetHeroTraits().Mercy < 0 && target.Clan != null && target.Clan == hero.Clan && target.Clan.Leader == target)
                            {
                                HeroLeaveClanAction.Apply(hero, false, target);
                            }
                            return;
                        }
                        else if (target.Spouse != hero && targetEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                        {
                            HeroBreakupAction.Apply(target, hero);
                            return;
                        }
                    }  
                }
                else if (Info.GetHeroHorny(hero) >= DramalordMCM.Get.MinHornyForIntercourse && Info.GetHeroHorny(target) >= DramalordMCM.Get.MinHornyForIntercourse)
                {
                    HeroIntercourseAction.Apply(hero, target, false);

                    Hero mother = hero.IsFemale ? hero : target;
                    Hero father = target.IsFemale ? hero : target;

                    if (mother != father && !mother.IsPregnant && Info.CanGetPregnant(mother) && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
                    {
                        HeroConceiveAction.Apply(hero, target, false);
                    }

                    if(heroWitness != null)
                    {
                        if (target != heroWitness && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                        {
                            HeroWitnessAction.Apply(hero, target, heroWitness, WitnessType.Intercourse);

                            float witnessEmotion = Info.GetEmotionToHero(heroWitness, hero);

                            if (heroWitness.GetHeroTraits().Calculating < 0 && heroWitness.GetHeroTraits().Mercy < 0)
                            {
                                HeroKillAction.Apply(heroWitness, hero, target, KillReason.Intercourse);
                                return;
                            }
                            else if (heroWitness.Spouse == hero && witnessEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                            {
                                HeroDivorceAction.Apply(heroWitness, hero);
                                if (heroWitness.GetHeroTraits().Mercy < 0 && heroWitness.Clan != null && heroWitness.Clan == hero.Clan && heroWitness.Clan.Leader == heroWitness)
                                {
                                    HeroLeaveClanAction.Apply(hero, false, heroWitness);
                                }
                                return;
                            }
                            else if (heroWitness.Spouse != hero && witnessEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce)
                            {
                                HeroBreakupAction.Apply(heroWitness, hero);
                                return;
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
                }
            }
        }

        internal static void ScopeSurroundings(Hero hero, ref List<Hero> flirts, ref List<Hero> partners, ref List<Hero> prisoners)
        {
            int flirtLimit = DramalordMCM.Get.MinAttractionForFlirting;
            bool noFamily = DramalordMCM.Get.ProtectFamily;

            if (hero.CurrentSettlement != null && (hero.CurrentSettlement.IsTown || hero.CurrentSettlement.IsCastle))
            {
                Settlement settlement = hero.CurrentSettlement;
                foreach (Hero h in settlement.HeroesWithoutParty)
                {
                    if (h != null && h != hero && Info.ValidateHeroMemory(hero, h))
                    {
                        bool isCouple = Info.IsCoupleWithHero(hero, h);
                        int attraction = Info.GetAttractionToHero(hero, h);
                        bool isRelative = Info.IsCloseRelativeTo(hero, h);

                        if (!isRelative || !noFamily)
                        {
                            if (!isCouple && attraction >= flirtLimit)
                            {
                                if (!h.IsPrisoner)
                                {
                                    flirts.Add(h);
                                }
                                else
                                {
                                    prisoners.Add(h);
                                }
                            }
                            else if (isCouple && !h.IsPrisoner)
                            {
                                partners.Add(h);
                            }
                        }
                    }
                }

                foreach (MobileParty mp in settlement.Parties)
                {
                    if (mp != null && mp.LeaderHero != null)
                    {
                        Hero h = mp.LeaderHero;
                        if (h != null && h != hero && Info.ValidateHeroMemory(hero, h))
                        {
                            bool isCouple = Info.IsCoupleWithHero(hero, h);
                            int attraction = Info.GetAttractionToHero(hero, h);
                            bool isRelative = Info.IsCloseRelativeTo(hero, h);

                            if (!isRelative || !noFamily)
                            {
                                if (!isCouple && attraction >= flirtLimit)
                                {
                                    if (!h.IsPrisoner)
                                    {
                                        flirts.Add(h);
                                    }
                                    else
                                    {
                                        prisoners.Add(h);
                                    }
                                }
                                else if (isCouple)
                                {
                                    partners.Add(h);
                                }
                            }
                        }
                    }
                }
            }
            else if (hero.PartyBelongedTo != null)
            {
                if (hero.PartyBelongedTo.Army != null)
                {
                    foreach (MobileParty mp in hero.PartyBelongedTo.Army.Parties)
                    {
                        if (mp != null && mp.LeaderHero != null)
                        {
                            Hero h = mp.LeaderHero;
                            if (h != null && h != hero && Info.ValidateHeroMemory(hero, h))
                            {
                                bool isCouple = Info.IsCoupleWithHero(hero, h);
                                int attraction = Info.GetAttractionToHero(hero, h);
                                bool isRelative = Info.IsCloseRelativeTo(hero, h);

                                if (!isRelative || !noFamily)
                                {
                                    if (!isCouple && attraction >= flirtLimit)
                                    {
                                        flirts.Add(h);
                                    }
                                    else if (isCouple)
                                    {
                                        partners.Add(h);
                                    }
                                }
                            }
                        }
                    }
                }
                if (hero.PartyBelongedTo.PrisonRoster != null && hero.PartyBelongedTo.PrisonRoster.TotalHeroes > 1)
                {
                    foreach (TroopRosterElement tre in hero.PartyBelongedTo.PrisonRoster.GetTroopRoster())
                    {
                        if (tre.Character.IsHero)
                        {
                            Hero h = tre.Character.HeroObject;
                            if (h != null && h != hero && Info.ValidateHeroMemory(hero, h))
                            {
                                bool isCouple = Info.IsCoupleWithHero(hero, h);
                                int attraction = Info.GetAttractionToHero(hero, h);
                                bool isRelative = Info.IsCloseRelativeTo(hero, h);

                                if (!isCouple && attraction >= flirtLimit && (!isRelative || !noFamily))
                                {
                                    prisoners.Add(h);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
