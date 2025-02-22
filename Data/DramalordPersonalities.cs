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
        [SaveableField(1)]
        private int _openness;

        [SaveableField(2)]
        private int _conscientiousness;

        [SaveableField(3)]
        private int _extroversion;

        [SaveableField(4)]
        private int _agreeableness;

        [SaveableField(5)]
        private int _neuroticism;

        internal int Openness
        {
            get => _openness;
            set => _openness = MBMath.ClampInt(value, -50, 50);
        }

        internal int Conscientiousness
        {
            get => _conscientiousness;
            set => _conscientiousness = MBMath.ClampInt(value, -50, 50);
        }

        internal int Extroversion
        {
            get => _extroversion;
            set =>_extroversion = MBMath.ClampInt(value, -50, 50);
        }

        internal int Agreeableness
        {
            get => _agreeableness;
            set => _agreeableness = MBMath.ClampInt(value, -50, 50);
        }

        internal int Neuroticism
        {
            get => _neuroticism;
            set => _neuroticism = MBMath.ClampInt(value, -50, 50);
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

    internal class DramalordPersonalities : DramalordData
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

        internal override void LoadData(IDataStore dataStore)
        {
            _personalities.Clear();

            if(!IsOldData)
            {
                Dictionary<Hero, HeroPersonality> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);

                data.Do(keypair =>
                {
                    _personalities.Add(keypair.Key, keypair.Value);
                });
            }
            else
            {
                LegacySave.LoadLegacyPersonality(_personalities, dataStore);
            }
        }

        internal override void SaveData(IDataStore dataStore)
        {
            Dictionary<Hero, HeroPersonality> data = new();

            _personalities.Do(keypair =>
            {
                if(!data.ContainsKey(keypair.Key))
                {
                    data.Add(keypair.Key, keypair.Value);
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
            int result = (int)(25 * rand_std_normal);
            while(result < -50 || result > 50)
            {
                rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
                result = (int)(25 * rand_std_normal);
            }
            return result;
        }
    }
}
