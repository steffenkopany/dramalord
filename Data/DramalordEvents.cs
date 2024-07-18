using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal enum EventType
    {
        Date,
        Intercourse,
        Betrothed,
        Marriage,
        Pregnancy,
        Birth,
        Kill,
        Divorce,
        Breakup,
        Adoption
    }

    internal sealed class HeroEvent
    {
        internal EventType Type { get; private set; }

        internal HeroTuple Actors { get; private set; }

        internal CampaignTime Time { get; private set; }

        internal double DaysAlive { get; private set; }

        internal HeroEvent(EventType type, HeroTuple actors, CampaignTime time, double daysAlive)
        {
            Type = type;
            Actors = actors;
            Time = time;
            DaysAlive = daysAlive;
        }
    }

    internal sealed class HeroEventSave
    {
        [SaveableProperty(1)]
        internal int Type { get; set; }

        [SaveableProperty(2)]
        internal string TupleString { get; set; }

        [SaveableProperty(3)]
        internal double Time { get; set; }

        [SaveableProperty(4)]
        internal double DaysAlive { get; set; }

        internal HeroEventSave(int type, string tupleString, double time, double daysAlive)
        {
            Type = type;
            TupleString = tupleString;
            Time = time;
            DaysAlive = daysAlive;
        }
    }

    internal class DramalordEvents : DramalordDataHandler
    {
        private static DramalordEvents? _instance;

        internal static DramalordEvents Instance
        {
            get
            {
                _instance ??= new DramalordEvents();
                return _instance;
            }
        }

        private readonly Dictionary<int, HeroEvent> _events;
        private static int _lastId = 0;

        private DramalordEvents() : base("DramalordEvents")
        {
            _events = new();
        }

        internal HeroEvent? GetEvent(int eventID)
        {
            if(_events.ContainsKey(eventID))
            {
                return _events[eventID];
            }
            return null;
        }

        internal int AddEvent(Hero hero1, Hero hero2, EventType type, double daysAlive)
        {
            _events.Add(++_lastId, new HeroEvent(type, new HeroTuple(hero1, hero2), CampaignTime.Now, daysAlive));
            return _lastId;
        }

        internal int FindEvent(Hero hero1, Hero hero2, EventType type)
        {
            int rtn = -1;
            _events.Where(keypair => keypair.Value.Actors.Contains(hero1, hero2) && keypair.Value.Type == type).Do(keypair => rtn = keypair.Key);
            return rtn;
        }

        internal void RemoveEvent(int eventId)
        {
            _events.Remove(eventId);
        }

        private void GroomEvents()
        {
            _events.Where(item => CampaignTime.Now.ToDays - item.Value.Time.ToDays > item.Value.DaysAlive).ToList().ForEach(item => _events.Remove(item.Key));
        }

        internal override void InitEvents()
        {
            base.InitEvents();
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(GroomEvents));
        }

        public override void LoadData(IDataStore dataStore)
        {
            _events.Clear();
            _lastId = 0;
            Dictionary<int, HeroEventSave> data = new();
            dataStore.SyncData(SaveIdentifier, ref data);

            data.Do(keypair =>
            {
                _lastId = (_lastId < keypair.Key) ? keypair.Key : _lastId;
                HeroEvent @event = new HeroEvent(
                    (EventType)keypair.Value.Type,
                    new HeroTuple(keypair.Value.TupleString),
                    CampaignTime.Milliseconds((long)keypair.Value.Time),
                    keypair.Value.DaysAlive
                    );
                if (@event.Actors.IsNotNull() && !_events.ContainsKey(keypair.Key))
                {
                    _events.Add(keypair.Key, @event);
                }
            });
        }

        public override void SaveData(IDataStore dataStore)
        {
            Dictionary<int, HeroEventSave> data = new();

            _events.Do(keypair =>
            {
                data.Add(keypair.Key, new HeroEventSave((int)keypair.Value.Type, keypair.Value.Actors.SaveString, keypair.Value.Time.ToMilliseconds, keypair.Value.DaysAlive));
            });

            dataStore.SyncData(SaveIdentifier, ref data);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            // nothing todo
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            // nothing todo
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _events.Where(keypair => keypair.Value.Actors.Contains(victim)).Select(keypair => keypair.Key).ToList().ForEach(key => _events.Remove(key));
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _events.Where(keypair => keypair.Value.Actors.Contains(hero)).Select(keypair => keypair.Key).ToList().ForEach(key => _events.Remove(key));
        }
    }
}
