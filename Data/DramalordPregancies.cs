using Dramalord.Actions;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal sealed class HeroPregnancy
    {
        internal Hero Father { get; set; }

        internal CampaignTime Conceived { get; set; }

        internal int EventId { get; set; }

        internal HeroPregnancy(Hero father, CampaignTime conceived, int eventId)
        {
            Father = father;
            Conceived = conceived;
            EventId = eventId;
        }
    }

    internal sealed class HeroPregnancySave
    {
        [SaveableProperty(1)]
        internal string FatherId { get; set; }

        [SaveableProperty(2)]
        internal double Conceived { get; set; }

        [SaveableProperty(3)]
        internal int EventId { get; set; }

        internal HeroPregnancySave(string fatherid, double conceived, int eventid)
        {
            FatherId = fatherid;
            Conceived = conceived;
            EventId = eventid;
        }
    }

    internal class DramalordPregancies : DramalordDataHandler
    {
        private static DramalordPregancies? _instance;

        internal static DramalordPregancies Instance
        {
            get
            {
                _instance ??= new DramalordPregancies();
                return _instance;
            }
        }

        private readonly Dictionary<Hero, HeroPregnancy> _pregnancies;

        private DramalordPregancies() : base("DramalordPregnancies")
        {
            _pregnancies = new Dictionary<Hero, HeroPregnancy>();
        }

        internal HeroPregnancy? GetPregnancy(Hero mother)
        {
            if(_pregnancies.ContainsKey(mother))
            {
                return _pregnancies[mother];
            }
            return null;
        }

        internal void AddPregnancy(Hero mother, Hero father, CampaignTime conceived, int eventid)
        {
            if(!_pregnancies.ContainsKey(mother))
            {
                _pregnancies.Add(mother, new HeroPregnancy(father, conceived, eventid));
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
                BirthAction.Apply(keypair.Key, keypair.Value);
                _pregnancies.Remove(keypair.Key);
            });
        }

        public override void LoadData(IDataStore dataStore)
        {
            _pregnancies.Clear();
            Dictionary<string, HeroPregnancySave> data = new();
            dataStore.SyncData(SaveIdentifier, ref data);

            data.Do(keypair =>
            {
                Hero? mother = Hero.AllAliveHeroes.Where(hero => hero.StringId == keypair.Key).FirstOrDefault();
                HeroPregnancy pregnancy = new(
                    Hero.AllAliveHeroes.Where(hero => hero.StringId == keypair.Value.FatherId).FirstOrDefault(),
                    CampaignTime.Milliseconds((long)keypair.Value.Conceived),
                    keypair.Value.EventId
                    );
                if (mother != null && pregnancy.Father != null && !_pregnancies.ContainsKey(mother))
                {
                    _pregnancies.Add(mother, pregnancy);
                }
            });
        }

        public override void SaveData(IDataStore dataStore)
        {
            Dictionary<string, HeroPregnancySave> data = new();
            _pregnancies.Do(keypair =>
            {
                if(!data.ContainsKey(keypair.Key.StringId))
                {
                    data.Add(keypair.Key.StringId, new HeroPregnancySave(keypair.Value.Father.StringId, keypair.Value.Conceived.ToMilliseconds, keypair.Value.EventId));
                }
            });
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
    }
}
