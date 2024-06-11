using Dramalord.Actions;
using Dramalord.Data;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Dramalord
{
    public class DramalordEventCallbacks
    {
        public static List<Action<Hero, Hero>> OnHeroesMarriedActions = new();
        public static List<Action<Hero, Hero>> OnHeroesDivorcedActions = new();
        public static List<Action<Hero, Hero>> OnHeroesAffairMeetingActions = new();
        public static List<Action<Hero, Hero>> OnHeroesBreakupActions = new();
        public static List<Action<Hero, Hero>> OnHeroesIntercourseActions = new();
        public static List<Action<Hero, Hero>> OnHeroesConceiveActions = new();
        public static List<Action<Hero, Hero>> OnHeroesFlirtActions = new();
        public static List<Action<Hero, Hero, Hero>> OnHeroesAdoptedActions = new();
        public static List<Action<Hero, Hero, Hero>> OnHeroesBornActions = new();
        public static List<Action<Hero, Clan, Hero>> OnHeroesLeaveClanActions = new();
        public static List<Action<Hero, Clan>> OnHeroesJoinClanActions = new();
        public static List<Action<Hero, Hero>> OnHeroesPutToOrphanageActions = new();
        public static List<Action<Hero, Hero, Hero, EventType>> OnHeroesWitnessActions = new();
        public static List<Action<Hero, Hero, Hero, EventType>> OnHeroesKilledActions = new();
        public static List<Action<Hero, bool>> OnHeroesUsedToyActions = new();
        public static List<Action<Clan, Kingdom, bool>> OnClanLeftKingdomActions = new();
        public static List<Action<Clan, Kingdom>> OnClanJoinedKingdomActions = new();
        public static List<Action<Hero, Hero, Hero, HeroEvent>> OnHeroesConfrontationActions = new();

        internal static void OnHeroesMarried(Hero hero, Hero target)
        {
            OnHeroesMarriedActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesDivorced(Hero hero, Hero target)
        {
            OnHeroesDivorcedActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesAffairMeeting(Hero hero, Hero target)
        {
            OnHeroesAffairMeetingActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesIntercourse(Hero hero, Hero target)
        {
            OnHeroesIntercourseActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesConceive(Hero hero, Hero target)
        {
            OnHeroesConceiveActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesBreakup(Hero hero, Hero target)
        {
            OnHeroesBreakupActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesFlirt(Hero hero, Hero target)
        {
            OnHeroesFlirtActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesAdopted(Hero hero, Hero target, Hero child)
        {
            OnHeroesAdoptedActions.ForEach(item => item(hero, target, child));
        }

        internal static void OnHeroesBorn(Hero hero, Hero target, Hero child)
        {
            OnHeroesBornActions.ForEach(item => item(hero, target, child));
        }

        internal static void OnHeroesLeaveClan(Hero hero, Clan clan, Hero cause)
        {
            OnHeroesLeaveClanActions.ForEach(item => item(hero, clan, cause));
        }

        internal static void OnHeroesJoinClan(Hero hero, Clan clan)
        {
            OnHeroesJoinClanActions.ForEach(item => item(hero, clan));
        }

        internal static void OnHeroesPutToOrphanage(Hero hero, Hero child)
        {
            OnHeroesPutToOrphanageActions.ForEach(item => item(hero, child));
        }

        internal static void OnHeroesWitness(Hero hero, Hero target, Hero witness, EventType type)
        {
            OnHeroesWitnessActions.ForEach(item => item(hero, target, witness, type));
        }

        internal static void OnHeroesKilled(Hero killer, Hero victim, Hero reason, EventType type)
        {
            OnHeroesKilledActions.ForEach(item => item(killer, victim, reason, type));
        }

        internal static void OnHeroesUsedToy(Hero hero, bool broke)
        {
            OnHeroesUsedToyActions.ForEach(item => item(hero, broke));
        }

        internal static void OnClanLeftKingdom(Clan clan, Kingdom kingdom, bool forced)
        {
            OnClanLeftKingdomActions.ForEach(item => item(clan, kingdom, forced));
        }

        internal static void OnClanJoinedKingdom(Clan clan, Kingdom kingdom)
        {
            OnClanJoinedKingdomActions.ForEach(item => item(clan, kingdom));
        }

        internal static void OnHeroesConfrontation(Hero hero1, Hero hero2, Hero hero3, HeroEvent hEvent)
        {
            OnHeroesConfrontationActions.ForEach(item => item(hero1, hero2, hero3, hEvent));
        }
    }
}
