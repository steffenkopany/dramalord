using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal enum RelationshipType
    {
        None,
        Friend,
        FriendWithBenefits,
        Lover,
        Betrothed,
        Spouse
    }

    internal sealed class HeroTuple
    {
        internal readonly Hero Hero1;
        internal readonly Hero Hero2;

        internal HeroTuple(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        internal HeroTuple(string saveString)
        {
            string[] tuple = saveString.Split(':');
            if(tuple.Length == 2)
            {
                Hero1 = Hero.AllAliveHeroes.Where(hero => hero.StringId == tuple[0]).FirstOrDefault();
                Hero2 = Hero.AllAliveHeroes.Where(hero => hero.StringId == tuple[1]).FirstOrDefault();
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hero1.StringId, Hero2.StringId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HeroTuple);
        }

        internal bool Equals(HeroTuple? other)
        {
            return other != null && Contains(other.Hero1, other.Hero2);
        }

        internal bool Contains(Hero hero1, Hero hero2)
        {
            return (Hero1.StringId == hero1.StringId && Hero2.StringId == hero2.StringId) || (Hero1.StringId == hero2.StringId && Hero2.StringId == hero1.StringId);
        }

        internal bool Contains(Hero hero)
        {
            return Hero1.StringId == hero.StringId || Hero2.StringId == hero.StringId;
        }

        internal string SaveString
        {
            get => Hero1.StringId + ":" + Hero2.StringId;
        }

        internal bool IsNotNull()
        {
            return Hero1 != null && Hero2 != null;
        }
    }

    internal sealed class HeroRelation
    {
        private int _trust;
        private int _love;
        //private int _tension;
        private double _lastInteraction;
        private RelationshipType _relationship;

        [SaveableProperty(1)]
        internal int Trust
        {
            get => _trust;
            set => _trust = MBMath.ClampInt(value, -100, 100);
        }

        [SaveableProperty(2)]
        internal int Love
        {
            get => _love;
            set => _love = MBMath.ClampInt(value, -100, 100);
        }

        /*
        [SaveableProperty(3)]
        internal int Tension
        {
            get => _tension;
            set => _tension = MBMath.ClampInt(value, -100, 100);
        }
        */

        [SaveableProperty(4)]
        internal double LastInteraction { get => _lastInteraction; set => _lastInteraction = value; }

        internal RelationshipType Relationship { get => _relationship; set => _relationship = value; }

        [SaveableProperty(5)]
        internal int RelationshipAsInt { get => (int)_relationship; set => _relationship = (RelationshipType) value; }

        internal HeroRelation(int friendship, int love, /*int tension,*/ RelationshipType relationship)
        {
            Trust = friendship;
            Love = love;
            //Tension = tension;
            LastInteraction = 0;
            Relationship = relationship;
        }
    }

    internal class DramalordRelations : DramalordDataHandler
    {
        private static DramalordRelations? _instance;

        internal static DramalordRelations Instance
        {
            get
            {
                _instance ??= new DramalordRelations();
                return _instance;
            }
        }

        private readonly Dictionary<HeroTuple, HeroRelation> _relations;

        private DramalordRelations() : base("DramalordRelations")
        {
            _relations = new();
        }

        internal HeroRelation GetRelation(Hero hero1, Hero hero2)
        {
            if(_relations.Where(item => item.Key.Contains(hero1, hero2)).Count() == 0)
            {
                _relations.Add(new HeroTuple(hero1, hero2), 
                    new HeroRelation((hero1.IsFriend(hero2) ? MBRandom.RandomInt(20, 50) : 0),
                    (hero1.Spouse == hero2) ? MBRandom.RandomInt(50, 100) : 0,
                     //0,
                     hero1.IsFriend(hero2) ? RelationshipType.Friend : (hero1.Spouse == hero2) ? RelationshipType.Spouse : RelationshipType.None));
            }
            return _relations.Where(item => item.Key.Contains(hero1, hero2)).First().Value;
        }

        internal IEnumerable<KeyValuePair<HeroTuple, HeroRelation>> GetAllRelations(Hero hero)
        {
            return _relations.Where(keypair => keypair.Key.Contains(hero) && keypair.Value.Relationship != RelationshipType.None);
        }

        public override void LoadData(IDataStore dataStore)
        {
            _relations.Clear();
            Dictionary<string, HeroRelation> data = new();
            dataStore.SyncData(SaveIdentifier, ref data);

            data.Do(keypair =>
            {
                HeroTuple heroTuple = new(keypair.Key);
                if(heroTuple.IsNotNull() && !_relations.ContainsKey(heroTuple))
                {
                    _relations.Add(heroTuple, keypair.Value);
                }
            });
        }

        public override void SaveData(IDataStore dataStore)
        {
            Dictionary<string, HeroRelation> data = new();

            _relations.Do(keypair =>
            {
                if (!data.ContainsKey(keypair.Key.SaveString))
                {
                    data.Add(keypair.Key.SaveString, keypair.Value);
                }
            });

            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            // nothing to do
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            // nothing to do
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _relations.Where(item => item.Key.Contains(victim)).Select(item => item.Key).ToList().ForEach(tuple =>
            {
                _relations.Remove(tuple);
            });
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _relations.Where(item => item.Key.Contains(hero)).Select(item => item.Key).ToList().ForEach(tuple =>
            {
                _relations.Remove(tuple);
            });
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _relations.Clear();
        }
    }
}
