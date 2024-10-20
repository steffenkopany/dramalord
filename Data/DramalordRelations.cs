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

        internal int CurrentLove
        {
            get => (_lastInteraction != 0 && CampaignTime.Now.ToDays - _lastInteraction > DramalordMCM.Instance.LoveDecayStartDay) ? MBMath.ClampInt(_love - (int)(CampaignTime.Now.ToDays - (_lastInteraction + DramalordMCM.Instance.LoveDecayStartDay)), 0, 100) : _love;
        }

        internal void UpdateLove()
        {
            if(_lastInteraction != 0 && CampaignTime.Now.ToDays - _lastInteraction > DramalordMCM.Instance.LoveDecayStartDay)
            {
                int result = Love - (int)(CampaignTime.Now.ToDays - (_lastInteraction + DramalordMCM.Instance.LoveDecayStartDay));
                Love = (Love > 0 && result >= 0) ? result : (Love > 0) ? 0 : Love;
            }
            _lastInteraction = CampaignTime.Now.ToDays;
        }

        [SaveableProperty(4)]
        internal double LastInteraction { get => _lastInteraction; set => _lastInteraction = value; }

        internal RelationshipType Relationship { get => _relationship; set => _relationship = value; }

        [SaveableProperty(5)]
        internal int RelationshipAsInt { get => (int)_relationship; set => _relationship = (RelationshipType) value; }

        internal HeroRelation(int friendship, int love, RelationshipType relationship)
        {
            Trust = friendship;
            Love = love;
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

        //private readonly Dictionary<HeroTuple, HeroRelation> _relations;
        private readonly Dictionary<Hero, Dictionary<Hero, HeroRelation>> _relations;

        private DramalordRelations() : base("DramalordRelations")
        {
            _relations = new();
        }

        internal HeroRelation GetRelation(Hero hero1, Hero hero2)
        {
            if(!_relations.ContainsKey(hero1))
            {
                _relations.Add(hero1, new());
            }

            if (!_relations.ContainsKey(hero2))
            {
                _relations.Add(hero2, new());
            }

            if (!_relations[hero1].ContainsKey(hero2))
            {
                _relations[hero1].Add(hero2, new HeroRelation(
                    hero1.IsFriend(hero2) ? MBRandom.RandomInt(20, 50) : 0,
                    (hero1.Spouse == hero2) ? MBRandom.RandomInt(50, 100) : 0,
                     hero1.IsFriend(hero2) ? RelationshipType.Friend : (hero1.Spouse == hero2) ? RelationshipType.Spouse : RelationshipType.None)
                    );
            }

            if (!_relations[hero2].ContainsKey(hero1))
            {
                _relations[hero2].Add(hero1, _relations[hero1][hero2]);
            }

            return _relations[hero1][hero2];
        }

        internal Dictionary<Hero, HeroRelation> GetAllRelations(Hero hero)
        {
            if(!_relations.ContainsKey(hero))
            {
                _relations.Add(hero, new());
            }
            return _relations[hero];
        }

        public override void LoadData(IDataStore dataStore)
        {
            _relations.Clear();

            bool newRelationStructure = false;
            dataStore.SyncData("NewRelationStructure", ref newRelationStructure);

            if(newRelationStructure)
            {
                Dictionary<string, Dictionary<string, HeroRelation>> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);

                data.Do(firstpair =>
                {
                    Hero? hero1 = Hero.AllAliveHeroes.Where(hero => hero.StringId == firstpair.Key).FirstOrDefault();
                    if(hero1 != null)
                    {
                        if (!_relations.ContainsKey(hero1))
                        {
                            _relations.Add(hero1, new());
                        }

                        firstpair.Value.Do(secondpair =>
                        {
                            Hero? hero2 = Hero.AllAliveHeroes.Where(hero => hero.StringId == secondpair.Key).FirstOrDefault();
                            if(hero2 != null && hero2 != hero1)
                            {
                                if (!_relations[hero1].ContainsKey(hero2))
                                {
                                    if(_relations.ContainsKey(hero2) && _relations[hero2].ContainsKey(hero1))
                                    {
                                        _relations[hero1].Add(hero2, _relations[hero2][hero1]);
                                    }
                                    else
                                    {
                                        _relations[hero1].Add(hero2, secondpair.Value);
                                    }
                                }
                            }
                        });
                    } 
                });
            }
            else
            {
                Dictionary<string, HeroRelation> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);

                data.Do(keypair =>
                {
                    HeroTuple heroTuple = new(keypair.Key);
                    if (heroTuple.IsNotNull())
                    {
                        if(!_relations.ContainsKey(heroTuple.Hero1))
                        {
                            _relations.Add(heroTuple.Hero1, new());
                        }
                        if (!_relations.ContainsKey(heroTuple.Hero2))
                        {
                            _relations.Add(heroTuple.Hero2, new());
                        }
                        if (heroTuple.Hero1 != heroTuple.Hero2 && !_relations[heroTuple.Hero1].ContainsKey(heroTuple.Hero2))
                        {
                            _relations[heroTuple.Hero1].Add(heroTuple.Hero2, keypair.Value);
                        }
                        if (heroTuple.Hero1 != heroTuple.Hero2 && !_relations[heroTuple.Hero2].ContainsKey(heroTuple.Hero1))
                        {
                            _relations[heroTuple.Hero2].Add(heroTuple.Hero1, keypair.Value);
                        }
                    }
                });
            }
        }

        public override void SaveData(IDataStore dataStore)
        {
            bool newRelationStructure = true;
            dataStore.SyncData("NewRelationStructure", ref newRelationStructure);

            Dictionary<string,Dictionary<string, HeroRelation>> data = new();

            _relations.Do(keypair =>
            {
                if (!data.ContainsKey(keypair.Key.StringId))
                {
                    data.Add(keypair.Key.StringId, new());
                }

                keypair.Value.Do(secondpair =>
                {
                    if (!data[keypair.Key.StringId].ContainsKey(secondpair.Key.StringId))
                    {
                        data[keypair.Key.StringId].Add(secondpair.Key.StringId, secondpair.Value);
                    }
                });
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
            _relations.Remove(victim);

            _relations.Where(item => item.Value.ContainsKey(victim)).Select(item => item.Value).Do(item =>
            {
                item.Remove(victim);
            });
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _relations.Remove(hero);

            _relations.Where(item => item.Value.ContainsKey(hero)).Select(item => item.Value).Do(item =>
            {
                item.Remove(hero);
            });
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _relations.Clear();
        }
    }
}
