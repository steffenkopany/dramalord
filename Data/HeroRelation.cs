using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal class HeroFeelings
    {
        private int _emotion;
        private int _tension;

        internal int Emotion
        {
            get => _emotion;
            set => _emotion = MBMath.ClampInt(value, 0, 100);
        }

        internal int Tension
        {
            get => _tension;
            set => _tension = MBMath.ClampInt(value, 0, 100);
        }

        internal uint LastInteractionDay { get; set; }

        internal HeroFeelings(int emotion, int tension, uint lastInteractionDay)
        {
            Emotion = MBMath.ClampInt(emotion,0,100);
            Tension = MBMath.ClampInt(tension, 0, 100);
            LastInteractionDay = lastInteractionDay;
        }

        internal HeroFeelings(HeroFeelings other)
        {
            Emotion = other.Emotion;
            Tension = other.Tension;
            LastInteractionDay = other.LastInteractionDay;
        }
    }

    internal class HeroPartners
    {
        internal List<CharacterObject> Spouses { get; set; } = new();

        internal List<CharacterObject> Lovers { get; set; } = new();

        internal List<CharacterObject> FriendsWithBenefits { get; set; } = new();

        internal Dictionary<CharacterObject, HeroFeelings> Feelings { get; set; } = new();
    }

    public static class DramalordRelations
    {
        internal static Dictionary<CharacterObject, HeroPartners> Partners { get; set; } = new();

        internal static HeroFeelings GetHeroFeelings(Hero from, Hero to)
        {
            if (Partners.ContainsKey(from.CharacterObject) && Partners[from.CharacterObject].Feelings.ContainsKey(to.CharacterObject))
            {
                return Partners[from.CharacterObject].Feelings[to.CharacterObject];
            }
            else if(Partners.ContainsKey(from.CharacterObject))
            {
                HeroFeelings relation = new(0, 0, 0);
                Partners[from.CharacterObject].Feelings.Add(to.CharacterObject, relation);
                return relation;
            }
            else
            {
                HeroFeelings feelings = new HeroFeelings(0, 0, 0);
                HeroPartners partners = new();
                partners.Feelings.Add(to.CharacterObject, feelings);
                Partners.Add(from.CharacterObject, partners);
                return feelings;
            }
        }

        internal static List<CharacterObject> GetHeroSpouses(Hero hero)
        {
            if(Partners.ContainsKey(hero.CharacterObject))
            {
                return Partners[hero.CharacterObject].Spouses;
            }
            else
            {
                HeroPartners partners = new();
                Partners.Add(hero.CharacterObject, partners);
                return partners.Spouses;
            }
        }

        internal static List<CharacterObject> GetHeroLovers(Hero hero)
        {
            if (Partners.ContainsKey(hero.CharacterObject))
            {
                return Partners[hero.CharacterObject].Lovers;
            }
            else
            {
                HeroPartners partners = new();
                Partners.Add(hero.CharacterObject, partners);
                return partners.Lovers;
            }
        }

        internal static List<CharacterObject> GetHeroFriendsWithBenefits(Hero hero)
        {
            if (Partners.ContainsKey(hero.CharacterObject))
            {
                return Partners[hero.CharacterObject].FriendsWithBenefits;
            }
            else
            {
                HeroPartners partners = new();
                Partners.Add(hero.CharacterObject, partners);
                return partners.FriendsWithBenefits;
            }
        }
    }

    internal sealed class HeroFeelingsSave
    {
        [SaveableProperty(1)]
        internal int Emotion { get; set; }

        [SaveableProperty(2)]
        internal int Tension { get; set; }
        /*
        [SaveableProperty(3)]
        internal int Trust { get; set; }
        */
        [SaveableProperty(4)]
        internal uint LastInteractionDay { get; set; }

        internal HeroFeelingsSave(HeroFeelings feelings)
        {
            Emotion = feelings.Emotion;
            Tension = feelings.Tension;
            LastInteractionDay = feelings.LastInteractionDay;
        }

        internal HeroFeelings Create()
        {
            return new HeroFeelings(Emotion, Tension, LastInteractionDay); 
        }
    }
}
