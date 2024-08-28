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
    internal sealed class HeroPersonality
    {
        private int _openness;
        private int _conscientiousness;
        private int _extroversion;
        private int _agreeableness;
        private int _neuroticism;

        [SaveableProperty(1)]
        internal int Openness
        {
            get => _openness;
            set => _openness = MBMath.ClampInt(value, -100, 100);
        }

        [SaveableProperty(2)]
        internal int Conscientiousness
        {
            get => _conscientiousness;
            set => _conscientiousness = MBMath.ClampInt(value, -100, 100);
        }

        [SaveableProperty(3)]
        internal int Extroversion
        {
            get => _extroversion;
            set =>_extroversion = MBMath.ClampInt(value, -100, 100);
        }

        [SaveableProperty(4)]
        internal int Agreeableness
        {
            get => _agreeableness;
            set => _agreeableness = MBMath.ClampInt(value, -100, 100);
        }

        [SaveableProperty(5)]
        internal int Neuroticism
        {
            get => _neuroticism;
            set => _neuroticism = MBMath.ClampInt(value, -100, 100);
        }

        internal HeroPersonality(int openness, int conscientiousness, int extroversion, int agreeableness, int neuroticism)
        {
            Openness = openness;
            Conscientiousness = conscientiousness;
            Extroversion = extroversion;
            Agreeableness = agreeableness;
            Neuroticism = neuroticism;
        }
    }

    internal class DramalordPersonalities : DramalordDataHandler
    {
        private static DramalordPersonalities? _instance; 

        internal static DramalordPersonalities Instance
        {
            get
            {
                _instance ??= new DramalordPersonalities();
                return _instance;
            }
        }

        private readonly Dictionary<Hero, HeroPersonality> _personalities;

        private DramalordPersonalities() : base("DramalordPersonalities")
        {
            _personalities = new();
        }

        internal HeroPersonality GetPersonality(Hero hero)
        {
            if(!_personalities.ContainsKey(hero))
            {
                _personalities.Add(hero, new HeroPersonality(
                    Generate(),
                    Generate(),
                    Generate(),
                    Generate(),
                    Generate()
                ));
            }
            return _personalities[hero];
        }

        public override void LoadData(IDataStore dataStore)
        {
            _personalities.Clear();
            Dictionary<string, HeroPersonality> data = new();
            dataStore.SyncData(SaveIdentifier, ref data);

            data.Do(keypair =>
            {
                Hero? hero = Hero.AllAliveHeroes.Where(item => item.StringId == keypair.Key).FirstOrDefault();
                if(hero != null && !_personalities.ContainsKey(hero))
                {
                    _personalities.Add(hero, keypair.Value);
                }
            });
        }

        public override void SaveData(IDataStore dataStore)
        {
            Dictionary<string, HeroPersonality> data = new();

            _personalities.Do(keypair =>
            {
                if(!data.ContainsKey(keypair.Key.StringId))
                {
                    data.Add(keypair.Key.StringId, keypair.Value);
                }
            });

            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _personalities.Remove(victim);
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _personalities.Remove(hero);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            //nothing to do
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            // nothing to do
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _personalities.Clear();
        }

        private int Generate()
        {
            float rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
            int result = (int)(50 * rand_std_normal);
            while(result < -100 || result > 100)
            {
                rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
                result = (int)(50 * rand_std_normal);
            }
            return result;
        }
    }
}
