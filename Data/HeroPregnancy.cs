using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal sealed class HeroPregnancy
    {
        internal CharacterObject Father { get; set; }

        internal uint Conceived { get; set; }

        internal int EventID { get; set; }

        internal HeroPregnancy(CharacterObject father, uint campaignDays, int eventID)
        {
            Father = father;
            Conceived = campaignDays;
            EventID = eventID;
        }

        internal HeroPregnancy(HeroPregnancy other)
        {
            Father = other.Father;
            Conceived = other.Conceived;
            EventID = other.EventID;
        }
    }

    internal static class DramalordPregnancies
    {
        internal static Dictionary<CharacterObject, HeroPregnancy> Pregnancies { get; set; } = new();

        internal static HeroPregnancy? GetHeroPregnancy(Hero hero)
        {
            if (Pregnancies.ContainsKey(hero.CharacterObject))
            {
                return Pregnancies[hero.CharacterObject];
            }
            return null;
        }

        internal static void AddHeroPregnancy(Hero hero, Hero father, int eventID)
        {
            Pregnancies.Add(hero.CharacterObject, new(father.CharacterObject, (uint)CampaignTime.Now.ToDays, eventID));
        }

        internal static void RemovePregnancy(Hero hero)
        {
            Pregnancies.Remove(hero.CharacterObject);
        }
    }

    internal sealed class HeroPregnancySave
    {
        [SaveableProperty(1)]
        internal string Father { get; set; }

        [SaveableProperty(2)]
        internal uint Conceived { get; set; }

        [SaveableProperty(3)]
        internal int EventID { get; set; }

        internal HeroPregnancySave(HeroPregnancy pregnancy)
        {
            Father = pregnancy.Father.StringId;
            Conceived = pregnancy.Conceived;
            EventID = pregnancy.EventID;
        }

        internal HeroPregnancy? Create()
        {
            CharacterObject? father = CharacterObject.Find(Father);
            if(father != null)
            {
                return new HeroPregnancy(father, Conceived, EventID);
            }
            return null;
        }
    }
}
