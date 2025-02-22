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
    internal sealed class HeroDesires
    {
        [SaveableField(1)]
        private int _attractionMen;

        [SaveableField(2)]
        private int _attractionWomen;

        [SaveableField(3)]
        private int _attractionWeight;

        [SaveableField(4)]
        private int _attractionBuild;

        [SaveableField(5)]
        private int _attractionAgeDiff;

        [SaveableField(6)]
        private int _libido;

        [SaveableField(7)]
        private int _horny;

        [SaveableField(8)]
        private int _periodDayOfSeason;

        [SaveableField(9)]
        private int _intercourseSkill;

        [SaveableField(10)]
        private bool _hasToy;

        [SaveableField(11)]
        private bool _isKnownToPlayer;

        internal int AttractionMen
        {
            get => _attractionMen;
            set => _attractionMen = MBMath.ClampInt(value, 0, 100);
        }

        internal int AttractionWomen
        {
            get => _attractionWomen;
            set => _attractionWomen = MBMath.ClampInt(value, 0, 100);
        }

        internal int AttractionWeight
        {
            get => _attractionWeight;
            set => _attractionWeight = MBMath.ClampInt(value, 0, 100);
        }

        internal int AttractionBuild
        {
            get => _attractionBuild;
            set => _attractionBuild = MBMath.ClampInt(value, 0, 100);
        }

        internal int AttractionAgeDiff
        {
            get => _attractionAgeDiff;
            set => _attractionAgeDiff = MBMath.ClampInt(value, -20, 20);
        }

        internal int Libido
        {
            get => _libido;
            set => _libido = MBMath.ClampInt(value, 0, 10);
        }

        internal int Horny
        {
            get => _horny;
            set => _horny = MBMath.ClampInt(value, 0, 100);
        }

        internal int PeriodDayOfSeason
        {
            get => _periodDayOfSeason;
            set => _periodDayOfSeason = MBMath.ClampInt(value, 1, CampaignTime.DaysInSeason);
        }

        internal int IntercourseSkill
        {
            get => _intercourseSkill;
            set => _intercourseSkill = MBMath.ClampInt(value, 0, 100);
        }

        internal bool HasToy
        {
            get => _hasToy;
            set => _hasToy = value;
        }

        internal bool IsKnowToPlayer 
        {
            get => _isKnownToPlayer;
            set => _isKnownToPlayer = value; 
        }

        internal HeroDesires(int attractionMen, int attractionWomen, int attractionWeight, int attractionBuild, int attractionAgeDiff, int libido, int horny, int periodDayOfSeason, int intercourseSkill, bool hasToy, bool infoKnown)
        {
            AttractionMen = attractionMen;
            AttractionWomen = attractionWomen;
            AttractionWeight = attractionWeight;
            AttractionBuild = attractionBuild;
            AttractionAgeDiff = attractionAgeDiff;
            Libido = libido;
            Horny = horny;
            PeriodDayOfSeason = periodDayOfSeason;
            IntercourseSkill = intercourseSkill;
            HasToy = hasToy;
            IsKnowToPlayer = infoKnown;
        }
    }

    internal class DramalordDesires : DramalordData
    {
        private static float _heterosexualRate = 0.75f;
        private static float _homosexualRate = 0.1f;
        private static float _bisexualRate = 0.1f;
        private static float _asexualRate = 0.05f;

        internal static int Heterosexual { get; private set; } = 0;
        internal static int Homosexual { get; private set; } = 0;
        internal static int Bisexual { get; private set; } = 0;
        internal static int Asexual { get; private set; } = 0;

        private static DramalordDesires? _instance;

        internal static DramalordDesires Instance
        {
            get
            {
                _instance ??= new DramalordDesires();
                return _instance;
            }
        }

        private readonly Dictionary<Hero, HeroDesires> _desires;

        private DramalordDesires() : base("DramalordDesires")
        {
            _desires = new();
        }

        internal HeroDesires GetDesires(Hero hero)
        {
            if(!_desires.ContainsKey(hero))
            {
                _desires.Add(hero, Generate(hero));
            }
            return _desires[hero];
        }

        private HeroDesires Generate(Hero hero)
        {
            HeroDesires desires = new HeroDesires(
                MBRandom.RandomInt(0, 100),
                MBRandom.RandomInt(0, 100),
                Generate(25, 50, 0, 100),
                (hero.IsFemale) ? Generate(75, 100, 0, 100) : Generate(25, 50, 0, 100),
                Generate(0, 10, -20, 20),
                Generate(7, 10, 1, 10),
                Generate(50, 75, 0, 100), 
                MBRandom.RandomInt(1, CampaignTime.DaysInSeason),
                Generate(5, 7, 0, 10),
                false,
                false);

            float allHeroes = _desires.Count();
            float asRate = ((float)Asexual) / allHeroes;
            float biRate = ((float)Bisexual) / allHeroes;
            float hoRate = ((float)Homosexual) / allHeroes;

            if(asRate < _asexualRate)
            {
                desires.AttractionMen = Generate(0, 25, 0, 50);// MBRandom.RandomInt(0, 50);
                desires.AttractionWomen = Generate(0, 25, 0, 50);//MBRandom.RandomInt(0, 50);
                Asexual++;
            }
            else if(biRate < _bisexualRate)
            {
                desires.AttractionMen = Generate(100, 125, 50, 100);//MBRandom.RandomInt(50, 100);
                desires.AttractionWomen = Generate(100, 125, 50, 100);//MBRandom.RandomInt(50, 100);
                Bisexual++;
            }
            else if(hoRate < _homosexualRate)
            {
                desires.AttractionMen = (!hero.IsFemale) ? Generate(100, 125, 50, 100) : Generate(0, 25, 0, 50);//MBRandom.RandomInt(50, 100) : MBRandom.RandomInt(0, 50);
                desires.AttractionWomen = (hero.IsFemale) ? Generate(100, 125, 50, 100) : Generate(0, 25, 0, 50); //MBRandom.RandomInt(50, 100) : MBRandom.RandomInt(0, 50);
                Homosexual++;
            }
            else
            {
                desires.AttractionMen = (hero.IsFemale) ? Generate(100, 125, 50, 100) : Generate(0, 25, 0, 50);//MBRandom.RandomInt(50, 100) : MBRandom.RandomInt(0, 50);
                desires.AttractionWomen = (!hero.IsFemale) ? Generate(100, 125, 50, 100) : Generate(0, 25, 0, 50);//MBRandom.RandomInt(50, 100) : MBRandom.RandomInt(0, 50);
                Heterosexual++;
            }

            return desires;
        }

        internal override void LoadData(IDataStore dataStore)
        {
            _desires.Clear();
            Heterosexual = 0;
            Homosexual = 0;
            Bisexual = 0;
            Asexual = 0;

            if (!IsOldData)
            {
                Dictionary<Hero, HeroDesires> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);

                data.Do(keypair =>
                {
                    _desires.Add(keypair.Key, keypair.Value);
                });
            }
            else
            {
                LegacySave.LoadLegacyDesires(_desires, dataStore);
            }
            
            _desires.Do(keypair =>
            {
                if (keypair.Value.AttractionMen < 50 && keypair.Value.AttractionWomen < 50)
                {
                    Asexual++;
                }
                else if (keypair.Value.AttractionMen >= 50 && keypair.Value.AttractionWomen >= 50)
                {
                    Bisexual++;
                }
                else if(keypair.Value.AttractionMen >= 50 && keypair.Value.AttractionWomen < 50 && !keypair.Key.IsFemale)
                {
                    Homosexual++;
                }
                else if (keypair.Value.AttractionMen < 50 && keypair.Value.AttractionWomen >= 50 && keypair.Key.IsFemale)
                {
                    Homosexual++;
                }
                else if (keypair.Value.AttractionMen < 50 && keypair.Value.AttractionWomen >= 50 && !keypair.Key.IsFemale)
                {
                    Heterosexual++;
                }
                else if (keypair.Value.AttractionMen >= 50 && keypair.Value.AttractionWomen < 50 && keypair.Key.IsFemale)
                {
                    Heterosexual++;
                }
            });
        }

        internal override void SaveData(IDataStore dataStore)
        {
            Dictionary<Hero, HeroDesires> data = new();

            _desires.Do(keypair =>
            {
                data.Add(keypair.Key, keypair.Value);
            });

            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _desires.Remove(victim);
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _desires.Remove(hero);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            //nothing to do
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            //nothing to do
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _desires.Clear();
        }

        private int Generate(int peak, int normal, int min, int max)
        {
            float rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
            int result = (int)(peak + normal * rand_std_normal);
            while (result < min || result > max)
            {
                rand_std_normal = (float)Math.Sqrt(-2.0 * Math.Log(MBRandom.RandomFloat)) * (float)Math.Sin(2.0 * Math.PI * MBRandom.RandomFloat);
                result = (int)(peak + normal * rand_std_normal);
            }

            return result;
        }
    }
}
