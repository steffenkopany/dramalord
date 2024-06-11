using Dramalord.Data;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class HeroMarriageAction
    {
        internal static void Apply(Hero firstHero, Hero secondHero, List<Hero> closeHeroes)
        {
            if(firstHero.Spouse != null && !firstHero.IsSpouse(firstHero.Spouse))
            {
                firstHero.SetSpouse(firstHero.Spouse);
            }

            if (secondHero.Spouse != null && !secondHero.IsSpouse(secondHero.Spouse))
            {
                secondHero.SetSpouse(secondHero.Spouse);
            }

            MarriageAction.Apply(firstHero, secondHero, false);

            firstHero.SetSpouse(secondHero);

            Clan clan = (firstHero.Clan == null) ? secondHero.Clan : firstHero.Clan;

            foreach (Hero child in firstHero.Children)
            {
                if (child.IsChild && child.Clan == null)
                {
                    if (child.Occupation == Occupation.Wanderer)
                    {
                        child.SetName(child.FirstName, child.FirstName);
                    }
                    child.Clan = clan;
                    child.UpdateHomeSettlement();
                    child.SetNewOccupation(Occupation.Lord);
                    child.ChangeState(Hero.CharacterStates.Active);
                }
            }

            foreach (Hero child in secondHero.Children)
            {
                if (child.IsChild && child.Clan == null)
                {
                    if (child.Occupation == Occupation.Wanderer)
                    {
                        child.SetName(child.FirstName, child.FirstName);
                    }
                    child.Clan = clan;
                    child.UpdateHomeSettlement();
                    child.SetNewOccupation(Occupation.Lord);
                    child.ChangeState(Hero.CharacterStates.Active);
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

                    if (item.IsSpouse(firstHero) || item.IsLover(firstHero))
                    {
                        HeroFeelings witnessFeelings = item.GetDramalordFeelings(firstHero);
                        witnessFeelings.Emotion -= DramalordMCM.Get.EmotionalLossMarryOther;
                        witnessFeelings.Trust -= DramalordMCM.Get.EmotionalLossMarryOther;
                    }
                    else if (item.IsSpouse(secondHero) || item.IsLover(secondHero))
                    {
                        HeroFeelings witnessFeelings = item.GetDramalordFeelings(secondHero);
                        witnessFeelings.Emotion -= DramalordMCM.Get.EmotionalLossMarryOther;
                        witnessFeelings.Trust -= DramalordMCM.Get.EmotionalLossMarryOther;
                    }
                }
            });

            DramalordEventCallbacks.OnHeroesMarried(firstHero, secondHero);
        }
    }
}
