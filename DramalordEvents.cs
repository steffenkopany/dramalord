using Dramalord.Actions;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace Dramalord
{
    public class DramalordEvents
    {
        public static List<Action<Hero, Hero>> OnHeroesMarriedActions = new();
        public static List<Action<Hero, Hero>> OnHeroesDivorcedActions = new();
        public static List<Action<Hero, Hero>> OnHeroesAffairMeetingActions = new();
        public static List<Action<Hero, Hero>> OnHeroesBreakupActions = new();
        public static List<Action<Hero, Hero, bool>> OnHeroesIntercourseActions = new();
        public static List<Action<Hero, Hero, bool>> OnHeroesConceiveActions = new();
        public static List<Action<Hero, Hero, bool>> OnHeroesFlirtActions = new();
        public static List<Action<Hero, Hero, Hero>> OnHeroesAdoptedActions = new();
        public static List<Action<Hero, Hero, Hero>> OnHeroesBornActions = new();
        public static List<Action<Hero, Clan, Hero>> OnHeroesLeaveClanActions = new();
        public static List<Action<Hero, Clan>> OnHeroesJoinClanActions = new();
        public static List<Action<Hero, Hero>> OnHeroesPutToOrphanageActions = new();
        public static List<Action<Hero, Hero, Hero, WitnessType>> OnHeroesWitnessActions = new();
        public static List<Action<Hero, Hero, Hero, KillReason>> OnHeroesKilledActions = new();
        public static List<Action<Hero, bool>> OnHeroesUsedToyActions = new();

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

        internal static void OnHeroesIntercourse(Hero hero, Hero target, bool byForce)
        {
            OnHeroesIntercourseActions.ForEach(item => item(hero, target, byForce));
        }

        internal static void OnHeroesConceive(Hero hero, Hero target, bool byForce)
        {
            OnHeroesConceiveActions.ForEach(item => item(hero, target, byForce));
        }

        internal static void OnHeroesBreakup(Hero hero, Hero target)
        {
            OnHeroesBreakupActions.ForEach(item => item(hero, target));
        }

        internal static void OnHeroesFlirt(Hero hero, Hero target, bool startedAffair)
        {
            OnHeroesConceiveActions.ForEach(item => item(hero, target, startedAffair));
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

        internal static void OnHeroesWitness(Hero hero, Hero target, Hero witness, WitnessType type)
        {
            OnHeroesWitnessActions.ForEach(item => item(hero, target, witness, type));
        }

        internal static void OnHeroesKilled(Hero killer, Hero victim, Hero reason, KillReason type)
        {
            OnHeroesKilledActions.ForEach(item => item(killer, victim, reason, type));
        }

        internal static void OnHeroesUsedToy(Hero hero, bool broke)
        {
            OnHeroesUsedToyActions.ForEach(item => item(hero, broke));
        }
    }
}
