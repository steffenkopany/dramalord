using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using static Dramalord.Data.HeroMemory;

namespace Dramalord.Data
{
    public enum MemoryType
    {
        Participant,
        Witness,
        Confession,
        Gossip
    }

    internal sealed class HeroMemory
    {
        internal int EventId { get; set; }

        internal MemoryType Type { get; set; }

        internal CharacterObject Source { get; set; }

        internal HeroEvent? Event { get; set; }

        internal bool Active { get; set; }

        public HeroMemory(int eventID, MemoryType memoryType, CharacterObject source, HeroEvent? @event, bool active)
        {
            EventId = eventID;
            Type = memoryType;
            Source = source;
            Event = @event;
            Active = active;
        }
    }

    internal static class DramalordMemories
    {
        internal static Dictionary<CharacterObject, List<HeroMemory>> Memories { get; set; } = new();

        internal static List<HeroMemory> GetHeroMemory(Hero hero)
        {
            if(Memories.ContainsKey(hero.CharacterObject))
            {
                Memories[hero.CharacterObject].ToList().ForEach(item =>
                {
                    HeroEvent? ev = DramalordEvents.GetHeroEvent(item.EventId);
                    if(ev == null)
                    {
                        Memories[hero.CharacterObject].Remove(item);
                    }
                });
            }
            else
            {
                List<HeroMemory> newMem = new();
                Memories.Add(hero.CharacterObject, newMem);
            }
            return Memories[hero.CharacterObject];
        }

        internal static bool HasHeroMemory(Hero hero, int eventId)
        {
            if(Memories.ContainsKey(hero.CharacterObject))
            {
                return Memories[hero.CharacterObject].Any(item => item.EventId == eventId);
            }
            return false;
        }

        internal static void AddHeroMemory(Hero hero, int eventId, MemoryType memoryType, CharacterObject source, bool active)
        {
            if (Memories.ContainsKey(hero.CharacterObject))
            {
                if (!Memories[hero.CharacterObject].Any(item => item.EventId == eventId))
                {
                    Memories[hero.CharacterObject].Add(new HeroMemory(eventId, memoryType, source, DramalordEvents.GetHeroEvent(eventId), active));
                }
            }
            else
            {
                List<HeroMemory> newMem = new();
                newMem.Add(new HeroMemory(eventId, memoryType, source, DramalordEvents.GetHeroEvent(eventId), active));
                Memories.Add(hero.CharacterObject, newMem);
            }
        }

        internal static void SetHeroMemoryActive(Hero hero, int eventId, bool active)
        {
            if(Memories.ContainsKey(hero.CharacterObject))
            {
                Memories[hero.CharacterObject].Where(item => item.EventId == eventId).Do( item => item.Active = active);
            }
        }
    }

    internal sealed class HeroMemorySave
    {
        [SaveableProperty(1)]
        internal int EventId { get; set; }

        [SaveableProperty(2)]
        internal int Type { get; set; }

        [SaveableProperty(3)]
        internal string Source { get; set; }

        [SaveableProperty(4)]
        internal bool Active { get; set; }

        internal HeroMemorySave(HeroMemory memory)
        {
            EventId = memory.EventId;
            Type = (int)memory.Type;
            Source = memory.Source.StringId;
            Active = memory.Active;
        }

        internal HeroMemory Create()
        {
            return new HeroMemory(EventId, (MemoryType)Type, CharacterObject.Find(Source), DramalordEvents.GetHeroEvent(EventId), Active);
        }
    }
}
