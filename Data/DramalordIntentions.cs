using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal enum IntentionType
    {
        SmallTalk,
        Gossip,
        Confrontation,
        Flirt,
        Date,
        Intercourse,
        Engagement,
        Marriage,
        BreakUp,
        LeaveClan,
        Adopt,
        Orphanize,
        Execute,
        PrisonIntercourse,
        EndDate,
        Duel,
        LeaveKingdom,
        Abortion
    }

    internal sealed class HeroIntention
    {
        internal IntentionType Type { get; set; }

        internal Hero Target { get; set; }

        internal int EventId { get; set; }

        internal HeroIntention(IntentionType type, Hero target, int eventId)
        {
            Type = type;
            Target = target;
            EventId = eventId;
        }
    }

    internal sealed class HeroIntentionSave
    {
        [SaveableProperty(1)]
        internal int Type { get; set; }

        [SaveableProperty(2)]
        internal string Target { get; set; }

        [SaveableProperty(3)]
        internal int EventId { get; set; }

        internal HeroIntentionSave(int type, string target, int eventId)
        {
            Type = type;
            Target = target;
            EventId = eventId;
        }
    }

    internal class DramalordIntentions : DramalordDataHandler
    {
        private static DramalordIntentions? _instance;

        internal static DramalordIntentions Instance
        {
            get
            {
                _instance ??= new DramalordIntentions();
                return _instance;
            }
        }

        private readonly Dictionary<Hero, List<HeroIntention>> _intentions;
        private readonly List<Hero> _intentionsToPlayer;

        private DramalordIntentions() : base("DramalordIntentions")
        {
            _intentions = new();
            _intentionsToPlayer = new();
        }

        internal List<HeroIntention> GetIntentions(Hero hero)
        {
            if (_intentions.ContainsKey(hero))
            {
                _intentions[hero].ToList().ForEach(intention =>
                {
                    HeroEvent? @event = DramalordEvents.Instance.GetEvent(intention.EventId);
                    if (intention.EventId >= 0 && @event == null)
                    {
                        _intentions[hero].Remove(intention);
                    }
                });
            }
            else
            {
                _intentions.Add(hero, new List<HeroIntention>());
            }
            return _intentions[hero];
        }

        internal List<Hero> GetIntentionTowardsPlayer()
        {
            return _intentionsToPlayer;
        }
        /*
        internal void AddIntention(Hero hero, Hero target, IntentionType type, int eventID)
        {
            List<HeroIntention> intentions = GetIntentions(hero);
            intentions.Add(new HeroIntention(type, target, eventID));
            if(target == Hero.MainHero && !_intentionsToPlayer.Contains(hero))
            {
                _intentionsToPlayer.Add(hero);
            }
        }
        

        internal void RemoveIntention(Hero hero, Hero target, IntentionType type, int eventID)
        {
            List<HeroIntention> intentions = GetIntentions(hero);
            intentions.Where(item => item.Type == type && item.Target == target && item.EventId == eventID).ToList().ForEach(item => 
            {
                intentions.Remove(item);
            });
            if (target == Hero.MainHero)
            {
                _intentionsToPlayer.Remove(hero);
            }
        }
        
        internal void RemoveIntentionsTo(Hero hero, Hero target)
        {
            List<HeroIntention> intentions = GetIntentions(hero);
            intentions.Where(item => item.Target == target).ToList().ForEach(item =>
            {
                intentions.Remove(item);
            });
            if (target == Hero.MainHero)
            {
                _intentionsToPlayer.RemoveAll(item => item == hero);
            }
        }

        internal void RemoveAllIntentions(Hero hero)
        {
            List<HeroIntention> intentions = GetIntentions(hero);
            intentions.Clear();
            _intentionsToPlayer.Remove(hero);
        }
        */
        public override void LoadData(IDataStore dataStore)
        {
            _intentions.Clear();
            Dictionary<string, List<HeroIntentionSave>> data = new();
            dataStore.SyncData(SaveIdentifier, ref data);

            data.Do(keypair =>
            {
                Hero? hero = Hero.AllAliveHeroes.FirstOrDefault(item => item.StringId == keypair.Key);
                if (hero != null)
                {
                    List<HeroIntention> intentions = GetIntentions(hero);
                    keypair.Value.ForEach(save =>
                    {
                        Hero? target = Hero.AllAliveHeroes.FirstOrDefault(item => item.StringId == save.Target);
                        if (target != null)
                        {
                            intentions.Add(new HeroIntention((IntentionType)save.Type, target, save.EventId));
                        }
                    });
                }

            });

            _intentionsToPlayer.Clear();
            List<string> playerData = new();
            dataStore.SyncData(SaveIdentifier + "Player", ref playerData);
            playerData.ForEach(heroId =>
            {
                Hero? hero = Hero.AllAliveHeroes.FirstOrDefault(item => item.StringId == heroId);
                if(hero != null)
                {
                    _intentionsToPlayer.Add(hero);
                }
            });
        }

        public override void SaveData(IDataStore dataStore)
        {
            Dictionary<string, List<HeroIntentionSave>> data = new();

            _intentions.Do(keypair =>
            {
                if(!data.ContainsKey(keypair.Key.StringId))
                {
                    List<HeroIntentionSave> intentions = new();
                    keypair.Value.ForEach(item =>
                    {
                        intentions.Add(new HeroIntentionSave((int)item.Type, item.Target.StringId, item.EventId));
                    });
                    data.Add(keypair.Key.StringId, intentions);
                }
            });

            dataStore.SyncData(SaveIdentifier, ref data);

            List<string> playerData = new();
            _intentionsToPlayer.ForEach(hero =>
            {
                playerData.Add(hero.StringId);
            });

            dataStore.SyncData(SaveIdentifier + "Player", ref playerData);
        }

        protected override void OnHeroComesOfAge(Hero hero)
        {
            //nothing todo
        }

        protected override void OnHeroCreated(Hero hero, bool born)
        {
            // nothing todo
        }

        protected override void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail reason, bool showNotifications)
        {
            _intentions.Remove(victim);
            _intentionsToPlayer.Remove(victim);
        }

        protected override void OnHeroUnregistered(Hero hero)
        {
            _intentions.Remove(hero);
            _intentionsToPlayer.Remove(hero);
        }

        protected override void OnNewGameCreated(CampaignGameStarter starter)
        {
            _intentions.Clear();
            _intentionsToPlayer.Clear();
        }
    }
}
