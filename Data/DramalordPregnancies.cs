using Dramalord.Data.Intentions;
using HarmonyLib;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal sealed class HeroPregnancy
    {
        [SaveableProperty(1)]
        internal Hero Father { get; set; }

        [SaveableProperty(2)]
        internal CampaignTime Conceived { get; set; }

        internal HeroPregnancy(Hero father, CampaignTime conceived)
        {
            Father = father;
            Conceived = conceived;
        }
    }

    internal class DramalordPregnancies : DramalordData
    {
        private static DramalordPregnancies? _instance;

        internal static DramalordPregnancies Instance
        {
            get
            {
                _instance ??= new DramalordPregnancies();
                return _instance;
            }
        }

        private readonly Dictionary<Hero, HeroPregnancy> _pregnancies;

        private DramalordPregnancies() : base("DramalordPregnancies")
        {
            _pregnancies = new Dictionary<Hero, HeroPregnancy>();
        }

        internal HeroPregnancy? GetPregnancy(Hero mother)
        {
            if (_pregnancies.ContainsKey(mother))
            {
                return _pregnancies[mother];
            }
            return null;
        }

        internal void AddPregnancy(Hero mother, Hero father, CampaignTime conceived)
        {
            if (!_pregnancies.ContainsKey(mother))
            {
                _pregnancies.Add(mother, new HeroPregnancy(father, conceived));
            }
        }

        internal void RemovePregnancy(Hero mother)
        {
            _pregnancies.Remove(mother);
        }

        internal void OnHourlyTick()
        {
            _pregnancies.Where(keypair => CampaignTime.Days((float)keypair.Value.Conceived.ToDays + (float)DramalordMCM.Instance.PregnancyDuration).IsPast).ToList().ForEach(keypair =>
            {
                new GiveBirthIntention(keypair.Value, keypair.Key, CampaignTime.Now).Action();
                _pregnancies.Remove(keypair.Key);
            });
        }

        internal override void InitEvents()
        {
            base.InitEvents();
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(OnHourlyTick));
        }

        internal override void LoadData(IDataStore dataStore)
        {
            _pregnancies.Clear();

            if(!IsOldData)
            {
                Dictionary<Hero, HeroPregnancy> data = new();
                dataStore.SyncData(SaveIdentifier, ref data);
                data.Do(pair => _pregnancies.Add(pair.Key, pair.Value));
            }
            else
            {
                LegacySave.LoadLegacyPregnancies(_pregnancies, dataStore);
            }

            _pregnancies.Do(preg => preg.Key.IsPregnant = true);
        }

        internal override void SaveData(IDataStore dataStore)
        {
            Dictionary<Hero, HeroPregnancy> data = new();

            _pregnancies.Do(pair => data.Add(pair.Key, pair.Value));

            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            //nothing to do
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            // nothing to do
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _pregnancies.Remove(victim);
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _pregnancies.Remove(hero);
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _pregnancies.Clear();
        }
    }
}
