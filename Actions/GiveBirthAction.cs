using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Dramalord.Actions
{
    internal static class GiveBirthAction
    {
        internal static void Apply(Hero mother, Hero father, out Hero child)
        {
            try
            {
                child = HeroCreator.DeliverOffSpring(mother, father, MBRandom.RandomInt(0, 100) < 50);
                mother.IsPregnant = false;
                return;
            }
            catch(Exception e)
            { 
            }

            try
            {
                child = CreateBaby(mother, father);
                mother.IsPregnant = false;
            }
            catch
            {
                child = null;
            }
        }

        internal static Hero CreateBaby(Hero mother, Hero father)
        {
            CharacterObject? template = null;
            if (mother.IsLord && father.IsLord)
            {
                template = (MBRandom.RandomInt(1, 100) > 50) ? mother.CharacterObject : father.CharacterObject;
            }
            else
            {
                template = mother.IsLord ? mother.CharacterObject : father.IsLord ? father.CharacterObject : Hero.AllAliveHeroes.GetRandomElementWithPredicate(h => h.IsLord && h.Clan != Clan.PlayerClan).CharacterObject;
            }

            Settlement bornSettlement = mother.CurrentSettlement ?? father.HomeSettlement ?? SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);

            Clan? faction = mother.Clan;
            Hero child = HeroCreator.CreateSpecialHero(template, bornSettlement, faction, null, 0);
            child.Mother = mother;
            child.Father = father;
            child.HeroDeveloper.InitializeHeroDeveloper(isByNaturalGrowth: true);
            BodyProperties bodyProperties = mother.BodyProperties;
            BodyProperties bodyProperties2 = father.BodyProperties;
            int seed = MBRandom.RandomInt();
            string hairTags = (child.IsFemale ? mother.HairTags : father.HairTags);
            string tattooTags = (child.IsFemale ? mother.TattooTags : father.TattooTags);
            child.ModifyPlayersFamilyAppearance(BodyProperties.GetRandomBodyProperties(template.Race, child.IsFemale, bodyProperties, bodyProperties2, 1, seed, hairTags, father.BeardTags, tattooTags).StaticProperties);
            child.SetNewOccupation(mother.Occupation);

            if (child.Occupation == Occupation.Lord)
            {
                child.SetName(child.FirstName, child.FirstName);
            }

            return child;
        }
    }
}
