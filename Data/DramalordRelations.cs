using Dramalord.Actions;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
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
    public enum RelationshipType
    {
        None,
        Friend,
        FriendWithBenefits,
        Lover,
        Betrothed,
        Spouse
    }

    public enum RelationshipRule
    {
        Faithful, //default
        Playful, //FWB are ok
        Poly,   // only spouse to spouse
        Open    // doesnt care at all
    }

    internal sealed class HeroRelation
    {
        [SaveableField(1)]
        private CampaignTime _lastInteraction;

        [SaveableField(2)]
        private int _love;

        [SaveableField(3)]
        private int _relationship;

        [SaveableField(4)]
        private bool _isKnownToPlayer;

        [SaveableField(5)]
        private CampaignTime _lastUpdate;

        [SaveableField(6)]
        private int _rules;

        internal CampaignTime LastInteraction 
        { 
            get => _lastInteraction; 
            set
            {
                if (_lastUpdate.ElapsedDaysUntilNow > DramalordMCM.Instance.LoveDecayStartDay && _love > 0)
                {
                    _love = MBMath.ClampInt(_love - (int)(_lastUpdate.ElapsedDaysUntilNow - DramalordMCM.Instance.LoveDecayStartDay), 0, 100);
                }
                else if (_lastUpdate.ElapsedDaysUntilNow > DramalordMCM.Instance.LoveDecayStartDay && _love < 0)
                {
                    _love = MBMath.ClampInt(_love + (int)(_lastUpdate.ElapsedDaysUntilNow - DramalordMCM.Instance.LoveDecayStartDay), -100, 0);
                }
                _lastInteraction = value;
                _lastUpdate = value;
            }
        }

        internal int Love
        {
            get
            {
                if (_lastUpdate.ElapsedDaysUntilNow > DramalordMCM.Instance.LoveDecayStartDay && _love > 0)
                {
                    _love = MBMath.ClampInt(_love - (int)(_lastUpdate.ElapsedDaysUntilNow - DramalordMCM.Instance.LoveDecayStartDay), 0, 100);
                    _lastUpdate = CampaignTime.Now;
                }
                return _love;
            }
            set => _love = MBMath.ClampInt(value, -100, 100);
        }

        internal RelationshipType Relationship { get => (RelationshipType)_relationship; set => _relationship = (int)value; }

        internal RelationshipRule Rules { get => (RelationshipRule)_rules; set => _rules = (int)value; }

        internal bool IsKnownToPlayer { get => _isKnownToPlayer; set => _isKnownToPlayer = value; }

        internal HeroRelation(int love, RelationshipType relationship)
        {
            _love = love;
            _lastInteraction = CampaignTime.Now;
            _relationship = (int)relationship;
            _isKnownToPlayer = false;
            _lastUpdate = CampaignTime.Now;
            _rules = (int)RelationshipRule.Faithful;
        }
    }

    internal class DramalordRelations : DramalordData
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
                    (hero1.Spouse == hero2) ? MBRandom.RandomInt(50, 100) : 0,
                     (hero1.Spouse == hero2) ? RelationshipType.Spouse : hero1.IsFriend(hero2) ? RelationshipType.Friend :  RelationshipType.None)
                    );
                _relations[hero1][hero2].IsKnownToPlayer = hero1.Spouse == hero2 ? true : false;
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

        internal override void LoadData(IDataStore dataStore)
        {
            _relations.Clear();

            if (!IsOldData)
            {
                Dictionary<Hero, Dictionary<Hero, HeroRelation>> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);

                data.Do(firstpair =>
                {
                    if (!_relations.ContainsKey(firstpair.Key))
                    {
                        _relations.Add(firstpair.Key, new());
                    }

                    firstpair.Value.Do(secondpair =>
                    {
                        if (secondpair.Key != firstpair.Key)
                        {
                            if (!_relations[firstpair.Key].ContainsKey(secondpair.Key))
                            {
                                if (_relations.ContainsKey(secondpair.Key) && _relations[secondpair.Key].ContainsKey(firstpair.Key))
                                {
                                    _relations[firstpair.Key].Add(secondpair.Key, _relations[secondpair.Key][firstpair.Key]);
                                }
                                else
                                {
                                    _relations[firstpair.Key].Add(secondpair.Key, secondpair.Value);
                                }
                            }
                        }
                    });
                });
            }
            else
            {
                LegacySave.LoadLegacyRelations(_relations, dataStore);
            }

            // also this is actually not necessary
            if (!BetrothIntention.OtherMarriageModFound)
            {
                // When not in compatibility mode, ensure that if the vanilla spouse is set, the Dramalord relation is correct.
                Hero.AllAliveHeroes.Where(h => h.Spouse != null && _relations.ContainsKey(h)).Do(h =>
                {
                    if (h.GetRelationTo(h.Spouse).Relationship != RelationshipType.Spouse)
                    {
                        StartRelationshipAction.Apply(h, h.Spouse, h.GetRelationTo(h.Spouse), RelationshipType.Spouse);
                    }
                    else if(h != Hero.MainHero && h.Spouse != Hero.MainHero)
                    {
                        RelationshipRule rule1 = h.GetDefaultRelationshipRule();
                        RelationshipRule rule2 = h.Spouse.GetDefaultRelationshipRule();
                        h.GetRelationTo(h.Spouse).Rules = (rule1 < rule2) ? rule1 : rule2;
                    }
                });
            }
        }


        internal override void SaveData(IDataStore dataStore)
        {
            Dictionary<Hero,Dictionary<Hero, HeroRelation>> data = new();

            _relations.Do(keypair =>
            {
                if (!data.ContainsKey(keypair.Key))
                {
                    data.Add(keypair.Key, new());
                }

                keypair.Value.Do(secondpair =>
                {
                    if (!data[keypair.Key].ContainsKey(secondpair.Key))
                    {
                        data[keypair.Key].Add(secondpair.Key, secondpair.Value);
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

        internal override void InitEvents()
        {
            base.InitEvents();
            CampaignEvents.RomanticStateChanged.AddNonSerializedListener(this, new Action<Hero, Hero, Romance.RomanceLevelEnum>(OnRomanticStateChanged));
            CampaignEvents.HeroesMarried.AddNonSerializedListener(this, new Action<Hero, Hero, bool>(OnHeroesMarried));
        }

        public void OnRomanticStateChanged(Hero hero1, Hero hero2, Romance.RomanceLevelEnum level)
        {
            if (level == Romance.RomanceLevelEnum.Marriage && !hero1.IsSpouseOf(hero2))
            {
                StartRelationshipAction.Apply(hero1, hero2, hero1.GetRelationTo(hero2), RelationshipType.Spouse);
            }
        }

        public void OnHeroesMarried(Hero hero1, Hero hero2, bool flag)
        {
            if (!hero1.IsSpouseOf(hero2))
            {
                StartRelationshipAction.Apply(hero1, hero2, hero1.GetRelationTo(hero2), RelationshipType.Spouse);
            }
        }
    }
}
