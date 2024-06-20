using Dramalord.Data;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroMarriageAction
    {
        internal static bool IsDramalordMarriage = false;
        internal static void Apply(Hero firstHero, Hero secondHero, List<Hero> closeHeroes)
        {
            if (firstHero.Clan == null && secondHero.Clan == null)
            {
                return; // NOPE!
            }

            if (firstHero.Spouse != null && !firstHero.IsSpouse(firstHero.Spouse))
            {
                firstHero.SetSpouse(firstHero.Spouse);
            }

            if (secondHero.Spouse != null && !secondHero.IsSpouse(secondHero.Spouse))
            {
                secondHero.SetSpouse(secondHero.Spouse);
            }

            if(firstHero.Clan != secondHero.Clan)
            {
                if(firstHero.Clan == null)
                {
                    HeroJoinClanAction.Apply(firstHero, secondHero.Clan);
                }
                else if(secondHero.Clan == null)
                {
                    HeroJoinClanAction.Apply(secondHero, firstHero.Clan);
                }
                else if(firstHero.Clan == Clan.PlayerClan)
                {
                    HeroLeaveClanAction.Apply(secondHero, secondHero);
                    HeroJoinClanAction.Apply(secondHero, firstHero.Clan);
                }
                else if(secondHero.Clan == Clan.PlayerClan)
                {
                    HeroLeaveClanAction.Apply(firstHero, firstHero);
                    HeroJoinClanAction.Apply(firstHero, secondHero.Clan);
                }
                else if(firstHero.IsFemale != secondHero.IsFemale && secondHero.IsFemale)
                {
                    HeroLeaveClanAction.Apply(secondHero, secondHero);
                    HeroJoinClanAction.Apply(secondHero, firstHero.Clan);
                }
                else
                {
                    HeroLeaveClanAction.Apply(firstHero, firstHero);
                    HeroJoinClanAction.Apply(firstHero, secondHero.Clan);
                }
            }

            Hero? movingHero = null;
            Settlement? settlement = null;
            MobileParty? party = null;

            if(firstHero == Hero.MainHero)
            {
                movingHero = secondHero;
                settlement = firstHero.CurrentSettlement;
                party = secondHero.PartyBelongedTo;
            }
            else if(secondHero == Hero.MainHero)
            {
                movingHero = firstHero;
                settlement = secondHero.CurrentSettlement;
                party = firstHero.PartyBelongedTo;
            }

            IsDramalordMarriage = true;
            MarriageAction.Apply(firstHero, secondHero, false);
            IsDramalordMarriage = false;

            firstHero.SetSpouse(secondHero);

            if (firstHero.Occupation == Occupation.Wanderer)
            {
                firstHero.SetName(firstHero.FirstName, firstHero.FirstName);
                firstHero.SetNewOccupation(Occupation.Lord);
                if (secondHero.IsHumanPlayerCharacter)
                {
                    if (Clan.PlayerClan.Companions.Contains(firstHero))
                    {
                        Clan.PlayerClan.Companions.Remove(firstHero);
                    }

                    if (!Clan.PlayerClan.Lords.Contains(firstHero))
                    {
                        Clan.PlayerClan.Lords.Add(firstHero);
                    }

                }
            }

            if (secondHero.Occupation == Occupation.Wanderer)
            {
                secondHero.SetName(secondHero.FirstName, secondHero.FirstName);
                secondHero.SetNewOccupation(Occupation.Lord);
                
                if (firstHero.IsHumanPlayerCharacter)
                {
                    if (Clan.PlayerClan.Companions.Contains(secondHero))
                    {
                        Clan.PlayerClan.Companions.Remove(secondHero);
                    }

                    if (!Clan.PlayerClan.Lords.Contains(secondHero))
                    {
                        Clan.PlayerClan.Lords.Add(secondHero);
                    }

                }
            }

            if (movingHero != null)
            {
                if(party != null && party == MobileParty.MainParty)
                {
                    TeleportHeroAction.ApplyImmediateTeleportToParty(movingHero, MobileParty.MainParty);
                    movingHero.ChangeState(Hero.CharacterStates.Active);
                }
                else if(settlement != null)
                {
                    TeleportHeroAction.ApplyImmediateTeleportToSettlement(movingHero, settlement);
                    movingHero.ChangeState(Hero.CharacterStates.Active);
                }
            }

            foreach (Hero child in firstHero.Children)
            {
                if (child.IsChild && child.Clan == null)
                {
                    HeroPutInOrphanageAction.Apply(firstHero, child);
                }
            }

            foreach (Hero child in secondHero.Children)
            {
                if (child.IsChild && child.Clan == null)
                {
                    HeroPutInOrphanageAction.Apply(secondHero, child);
                }
            }

            int eventID = DramalordEvents.AddHeroEvent(firstHero, secondHero, EventType.Marriage, DramalordMCM.Get.MarriageMemoryDuration);
            firstHero.AddDramalordMemory(eventID, MemoryType.Participant, firstHero, true);
            secondHero.AddDramalordMemory(eventID, MemoryType.Participant, secondHero, true);

            closeHeroes.ForEach(item => {

                if(item != firstHero && item != secondHero)
                {
                    item.AddDramalordMemory(eventID, MemoryType.Witness, item, true);
                    LogEntry.AddLogEntry(new LogWitnessMarriage(firstHero, secondHero, item));

                    if (item == Hero.MainHero && (item.IsSpouse(firstHero) || item.IsLover(firstHero) || item.IsSpouse(secondHero) || item.IsLover(secondHero)))
                    {
                        TextObject banner = new TextObject("{=Dramalord393}You saw {HERO.LINK} marry {TARGET.LINK}");
                        StringHelpers.SetCharacterProperties("HERO", firstHero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", secondHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, firstHero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if ((firstHero == Hero.MainHero || secondHero == Hero.MainHero) && (item.IsSpouse(firstHero) || item.IsLover(firstHero) || item.IsSpouse(secondHero) || item.IsLover(secondHero)))
                    {
                        TextObject banner = new TextObject("{=Dramalord394}{HERO.LINK} saw you marry {TARGET.LINK}");
                        StringHelpers.SetCharacterProperties("HERO", item.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", (firstHero == Hero.MainHero) ? secondHero.CharacterObject : firstHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, item.CharacterObject, "event:/ui/notification/relation");
                    }

                    if (item.IsSpouse(firstHero) || item.IsLover(firstHero))
                    {
                        if (!item.GetDramalordTraits().IsEmotionallyOpen)
                        {
                            HeroFeelings witnessFeelings = item.GetDramalordFeelings(firstHero);
                            witnessFeelings.Emotion -= DramalordMCM.Get.EmotionalLossMarryOther;
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                item.ChangeRelationTo(firstHero, (DramalordMCM.Get.EmotionalLossMarryOther / 2) * -1);
                            }
                            item.MakeAngryWith(firstHero, DramalordMCM.Get.AngerDaysMarriage);
                        }
                            
                    }
                    else if (item.IsSpouse(secondHero) || item.IsLover(secondHero))
                    {
                        if (!item.GetDramalordTraits().IsEmotionallyOpen)
                        {
                            HeroFeelings witnessFeelings = item.GetDramalordFeelings(secondHero);
                            witnessFeelings.Emotion -= DramalordMCM.Get.EmotionalLossMarryOther;
                            if (DramalordMCM.Get.LinkEmotionToRelation)
                            {
                                item.ChangeRelationTo(secondHero, (DramalordMCM.Get.EmotionalLossMarryOther / 2) * -1);
                            }
                            item.MakeAngryWith(secondHero, DramalordMCM.Get.AngerDaysMarriage);
                        }
                    }
                }
            });

            DramalordEventCallbacks.OnHeroesMarried(firstHero, secondHero);
        }
    }
}
