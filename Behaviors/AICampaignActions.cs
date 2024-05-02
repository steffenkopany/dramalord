using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.UI;
using Helpers;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

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
                            HeroBirthAction.Apply(hero, offspring); //TaleWorlds.CampaignSystem.Actions.AddCompanionAction

                            Hero? child = hero.Children.FirstOrDefault(item => item.BirthDay.ToDays == CampaignTime.Now.ToDays);
                            if (child != null && hero.Spouse != null && child.Father != hero.Spouse)
                            {
                                Hero spouse = hero.Spouse;
                                if (spouse.CurrentSettlement == hero.CurrentSettlement && Info.ValidateHeroMemory(hero, spouse))
                                {
                                    HeroWitnessAction.Apply(hero, child, spouse, WitnessType.Bastard);
                                    HeroPutInOrphanageAction.Apply(spouse, child);

                                    if (spouse != Hero.MainHero && spouse.GetHeroTraits().Calculating < 0 && spouse.GetHeroTraits().Mercy < 0 && DramalordMCM.Get.AllowRageKills)
                                    {
                                        HeroKillAction.Apply(spouse, hero, child, KillReason.Bastard);
                                        return;
                                    }
                                    else if(spouse != Hero.MainHero && DramalordMCM.Get.AllowDivorces)
                                    {
                                        HeroDivorceAction.Apply(spouse, hero);
                                        if (spouse.GetHeroTraits().Mercy < 0 && spouse.Clan != null && spouse.Clan == hero.Clan && spouse.Clan.Leader == spouse && DramalordMCM.Get.AllowClanChanges)
                                        {
                                            HeroLeaveClanAction.Apply(hero, spouse);
                                        }
                                        return;
                                    }
                                }
                                else
                                {
                                    if (child.Father != Hero.MainHero && Info.ValidateHeroMemory(hero, child.Father) && Info.ValidateHeroMemory(hero, spouse))
                                    {
                                        
                                        if (child.Father.Clan != null && hero.Clan != null && child.Father.Clan != hero.Clan && Info.IsCoupleWithHero(hero, child.Father) && Info.GetEmotionToHero(hero, spouse) < DramalordMCM.Get.MinEmotionForDating && Info.GetEmotionToHero(hero, child.Father) > DramalordMCM.Get.MinEmotionForDating && DramalordMCM.Get.AllowClanChanges && DramalordMCM.Get.AllowDivorces)
                                        {
                                            HeroDivorceAction.Apply(hero, spouse);
                                            HeroLeaveClanAction.Apply(hero, hero);
                                            HeroLeaveClanAction.Apply(child, child);
                                            HeroJoinClanAction.Apply(hero, child.Father.Clan);
                                            return;
                                        }
                                        else if (hero.GetHeroTraits().Valor > 0 && Info.GetEmotionToHero(hero, spouse) < DramalordMCM.Get.MinEmotionForDating && DramalordMCM.Get.AllowDivorces && DramalordMCM.Get.AllowClanChanges)
                                        {
                                            HeroDivorceAction.Apply(hero, spouse);
                                            HeroLeaveClanAction.Apply(hero, hero);
                                            HeroLeaveClanAction.Apply(child, child);
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
                //TaleWorlds.CampaignSystem.Hero.SetHeroEncyclopediaTextAndLinks
                bool isSameParty = hero.PartyBelongedTo != null && hero.PartyBelongedTo == Hero.MainHero.PartyBelongedTo && DramalordMCM.Get.AllowApproachInParty;
                bool isSameSettlement = hero.CurrentSettlement != null && hero.CurrentSettlement == Hero.MainHero.CurrentSettlement && DramalordMCM.Get.AllowApproachInSettlement;
                bool isSameArmy = hero.PartyBelongedTo != null && hero.PartyBelongedTo.Army != null && Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.Army != null && hero.PartyBelongedTo.Army == Hero.MainHero.PartyBelongedTo.Army;
                if (!hero.IsPrisoner && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.ChanceNPCApproachPlayer && (isSameParty || isSameSettlement || isSameArmy))
                {
                    float emotion = Info.GetEmotionToHero(hero, Hero.MainHero);
                    int attraction = Info.GetAttractionToHero(hero, Hero.MainHero);
                    bool isCouple = Info.IsCoupleWithHero(hero, Hero.MainHero);

                    bool wantsFlirt = attraction >= DramalordMCM.Get.MinAttractionForFlirting && !isCouple && emotion > DramalordMCM.Get.MinEmotionForConversation && emotion < DramalordMCM.Get.MinEmotionForDating && CampaignTime.Now.ToDays - Info.GetLastDaySeen(hero, Hero.MainHero) > 1;
                    bool wantsDate = (isCouple || emotion >= DramalordMCM.Get.MinEmotionForDating) && emotion >= DramalordMCM.Get.MinEmotionBeforeDivorce && CampaignTime.Now.ToDays - Info.GetLastDate(hero, Hero.MainHero) >= DramalordMCM.Get.DaysBetweenDates && (emotion < DramalordMCM.Get.MinEmotionForMarriage || isSameArmy);
                    bool wantsToMarry = isCouple && hero.Spouse != Hero.MainHero && emotion >= DramalordMCM.Get.MinEmotionForMarriage && !isSameArmy && DramalordMCM.Get.AllowMarriages;
                    bool wantsToDivorce = hero.Spouse == Hero.MainHero && emotion < DramalordMCM.Get.MinEmotionBeforeDivorce && DramalordMCM.Get.AllowDivorces;
                    bool wantsToBreakUp = hero.Spouse != Hero.MainHero && emotion < DramalordMCM.Get.MinEmotionBeforeDivorce && isCouple;

                    if (wantsFlirt || wantsDate || wantsToMarry || wantsToBreakUp || wantsToDivorce)
                    {
                        Campaign.Current.SetTimeSpeed(0);
                        Conditions.ApproachingHero = hero;
                        CampaignMapConversation.OpenConversation(new ConversationCharacterData(Hero.MainHero.CharacterObject), new ConversationCharacterData(hero.CharacterObject));
                        return;
                    }
                }

                if (Info.GetHeroHasToy(hero))
                {
                    HeroToyAction.Apply(hero);
                    return;
                }

                if (!hero.IsPrisoner && (hero.Clan != Clan.PlayerClan || DramalordMCM.Get.AllowPlayerClanAI))
                {
                    List<Hero> flirts = new();
                    List<Hero> partners = new();
                    List<Hero> prisoners = new();

                    ScopeSurroundings(hero, ref flirts, ref partners, ref prisoners, false);

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

                        if (hero.Spouse == partner && heroEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce && DramalordMCM.Get.AllowDivorces)
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
                        CharacterTraits traits = hero.GetHeroTraits();
                        if (hero != Hero.MainHero && traits.Calculating < 0 && traits.Mercy < 0 && DramalordMCM.Get.AllowRageKills)
                        {
                            HeroKillAction.Apply(hero, target, target, KillReason.Pregnancy);
                            return;
                        }
                        else if(traits.Calculating < 0 && hero.IsKingdomLeader && target.IsClanLeader && hero.Clan != null && target.Clan != null && hero.Clan != target.Clan && target.Clan.Kingdom == hero.Clan.Kingdom && DramalordMCM.Get.AllowClanBanishment)
                        {
                            ClanLeaveKingdomAction.Apply(target.Clan, true);
                            return;
                        }
                        else if (hero.Spouse == target && heroEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce && DramalordMCM.Get.AllowDivorces)
                        {
                            HeroDivorceAction.Apply(hero, target);
                            if (hero.GetHeroTraits().Mercy < 0 && hero.Clan != null && target.Clan == hero.Clan && hero.Clan.Leader == hero && DramalordMCM.Get.AllowClanChanges)
                            {
                                HeroLeaveClanAction.Apply(target, hero);
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
                else if (hero.IsFemale && hero.IsPregnant)
                {
                    HeroOffspringData? offspring = Info.GetHeroOffspring(target);
                    if (offspring != null && offspring.Father != target && CampaignTime.Now.ToDays - offspring.Conceived >= DramalordMCM.Get.DaysUntilPregnancyVisible)
                    {
                        HeroWitnessAction.Apply(hero, hero, target, WitnessType.Pregnancy);

                        float targetEmotion = Info.GetEmotionToHero(target, hero);
                        CharacterTraits traits = target.GetHeroTraits();
                        if (target != Hero.MainHero && traits.Calculating < 0 && traits.Mercy < 0 && DramalordMCM.Get.AllowRageKills)
                        {
                            HeroKillAction.Apply(target, hero, hero, KillReason.Pregnancy);
                            return;
                        }
                        else if (traits.Calculating < 0 && target.IsKingdomLeader && hero.IsClanLeader && target.Clan != null && hero.Clan != null && target.Clan != hero.Clan && hero.Clan.Kingdom == target.Clan.Kingdom && DramalordMCM.Get.AllowClanBanishment)
                        {
                            ClanLeaveKingdomAction.Apply(hero.Clan, true);
                            return;
                        }
                        else if (target.Spouse == hero && targetEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce && DramalordMCM.Get.AllowDivorces)
                        {
                            HeroDivorceAction.Apply(target, hero);
                            if (target.GetHeroTraits().Mercy < 0 && target.Clan != null && target.Clan == hero.Clan && target.Clan.Leader == target && DramalordMCM.Get.AllowClanChanges)
                            {
                                HeroLeaveClanAction.Apply(hero, target);
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
                
                if ((Info.GetHeroHorny(hero) >= DramalordMCM.Get.MinHornyForIntercourse || hero == Hero.MainHero) && (Info.GetHeroHorny(target) >= DramalordMCM.Get.MinHornyForIntercourse || target == Hero.MainHero))
                {
                    if(hero != Hero.MainHero && target != Hero.MainHero)
                    {
                        HandleDateIntercourse(hero, target, heroWitness);
                    }
                    else
                    {
                        Hero main = (hero == Hero.MainHero) ? hero : target;
                        Hero other = (hero == Hero.MainHero) ? target : hero;

                        TextObject title = new TextObject("{=Dramalord245}Intimate Opportunity");
                        TextObject text = new TextObject("{=Dramalord246}{HERO.LINK} has a special spark in their eyes today and you have a feeling they want to go further. Will you let it happen?");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, text);

                        Notification.DrawMessageBox(
                                title,
                                text,
                                true,
                                () => {

                                    HandleDateIntercourse(hero, target, heroWitness);
                                },
                                () => {

                                }
                            );
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

        internal static void HandleDateIntercourse(Hero hero, Hero target, Hero? heroWitness)
        {
            HeroIntercourseAction.Apply(hero, target, false);

            Hero mother = hero.IsFemale ? hero : target;
            Hero father = target.IsFemale ? hero : target;

            if (mother != father && !mother.IsPregnant && Info.CanGetPregnant(mother) && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
            {
                HeroConceiveAction.Apply(hero, target, false);
            }

            if (heroWitness != null)
            {
                if (target != heroWitness && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
                {
                    HeroWitnessAction.Apply(hero, target, heroWitness, WitnessType.Intercourse);

                    float witnessEmotion = Info.GetEmotionToHero(heroWitness, hero);
                    CharacterTraits traits = heroWitness.GetHeroTraits();

                    if (heroWitness != Hero.MainHero && traits.Calculating < 0 && traits.Mercy < 0 && DramalordMCM.Get.AllowRageKills)
                    {
                        HeroKillAction.Apply(heroWitness, hero, target, KillReason.Intercourse);
                        if (traits.Calculating < 0 && heroWitness.IsKingdomLeader && target.IsClanLeader && heroWitness.Clan != null && target.Clan != null && heroWitness.Clan != target.Clan && target.Clan.Kingdom == heroWitness.Clan.Kingdom && DramalordMCM.Get.AllowClanBanishment)
                        {
                            ClanLeaveKingdomAction.Apply(target.Clan, true);
                        }
                        return;
                    }
                    else if (heroWitness.Spouse == hero && witnessEmotion < DramalordMCM.Get.MinEmotionBeforeDivorce && DramalordMCM.Get.AllowDivorces)
                    {
                        HeroDivorceAction.Apply(heroWitness, hero);
                        if (heroWitness.GetHeroTraits().Mercy < 0 && heroWitness.Clan != null && heroWitness.Clan == hero.Clan && heroWitness.Clan.Leader == heroWitness && DramalordMCM.Get.AllowClanChanges)
                        {
                            HeroLeaveClanAction.Apply(hero, heroWitness);
                        }
                        if (traits.Calculating < 0 && heroWitness.IsKingdomLeader && target.IsClanLeader && heroWitness.Clan != null && target.Clan != null && heroWitness.Clan != target.Clan && target.Clan.Kingdom == heroWitness.Clan.Kingdom && DramalordMCM.Get.AllowClanBanishment)
                        {
                            ClanLeaveKingdomAction.Apply(target.Clan, true);
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

        internal static void OnNewCompanionAdded(Hero companion)
        {
            if(companion.Clan == Clan.PlayerClan)
            {
                foreach (Hero child in companion.Children)
                {
                    if (child.IsChild && child.Clan == null)
                    {
                        child.Clan = companion.Clan;
                        child.UpdateHomeSettlement();
                        child.SetNewOccupation(companion.Occupation);
                        child.ChangeState(Hero.CharacterStates.Active);
                    }
                }
            }
        }

        internal static void ScopeSurroundings(Hero hero, ref List<Hero> flirts, ref List<Hero> partners, ref List<Hero> prisoners, bool lookupTroops)
        {
            if(Info.ValidateHeroInfo(hero))
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
                            bool clanCheck = h.Clan != Clan.PlayerClan || DramalordMCM.Get.AllowPlayerClanAI;
                            bool wandererCheck = h.Occupation != Occupation.Wanderer || DramalordMCM.Get.AllowWandererAI;

                            if ((!isRelative || !noFamily) && clanCheck && wandererCheck)
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
                                bool clanCheck = h.Clan != Clan.PlayerClan || DramalordMCM.Get.AllowPlayerClanAI;
                                bool wandererCheck = h.Occupation != Occupation.Wanderer || DramalordMCM.Get.AllowWandererAI;

                                if ((!isRelative || !noFamily) && clanCheck && wandererCheck)
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
                if (hero.PartyBelongedTo != null)
                {
                    if (hero.PartyBelongedTo.Army != null && DramalordMCM.Get.AllowArmyInteractionAI)
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
                                    bool clanCheck = h.Clan != Clan.PlayerClan || DramalordMCM.Get.AllowPlayerClanAI;
                                    bool wandererCheck = h.Occupation != Occupation.Wanderer || DramalordMCM.Get.AllowWandererAI;

                                    if ((!isRelative || !noFamily) && clanCheck && wandererCheck)
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

                    if (hero.PartyBelongedTo.PrisonRoster != null && hero.PartyBelongedTo.PrisonRoster.TotalHeroes > 0)
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

                    if (lookupTroops && hero.PartyBelongedTo.MemberRoster != null)
                    {
                        if (hero.PartyBelongedTo.MemberRoster.TotalHeroes > 1)
                        {
                            foreach (TroopRosterElement tre in hero.PartyBelongedTo.MemberRoster.GetTroopRoster())
                            {
                                if (tre.Character.IsHero)
                                {
                                    Hero h = tre.Character.HeroObject;
                                    if (h != null && h != hero && Info.ValidateHeroMemory(hero, h))
                                    {
                                        bool isCouple = Info.IsCoupleWithHero(hero, h);

                                        if (isCouple)
                                        {
                                            partners.Add(h);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } 
        }
    }
}
