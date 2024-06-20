using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    public enum EventType
    {
        Flirt,
        Date,
        Intercourse,
        Marriage,
        Pregnancy,
        Birth,
        Kill,
        Divorce,
        BreakUp,
        Anger,
        Adoption
    }
    public sealed class HeroEvent
    {
        internal CharacterObject Hero1 { get; set; }

        internal CharacterObject Hero2 { get; set; }

        internal EventType Type { get; set; }

        internal uint CampaignDay { get; set; }

        internal uint DaysAlive { get; set; }

        internal HeroEvent(CharacterObject hero1, CharacterObject hero2, EventType type, uint campaignDay, uint daysAlive)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Type = type;
            CampaignDay = campaignDay;
            DaysAlive = daysAlive;
        }

        internal HeroEvent(HeroEvent other)
        {
            Hero1 = other.Hero1;
            Hero2 =(other.Hero2);
            Type = other.Type;
            CampaignDay = other.CampaignDay;
            DaysAlive = other.DaysAlive;
        }
    }

    internal static class DramalordEvents
    {
        internal static Dictionary<int, HeroEvent> Events { get; set; } = new();

        internal static int LastId = 0;

        internal static void GroomEvents()
        {
            Events.Where(item => CampaignTime.Now.ToDays - item.Value.CampaignDay > item.Value.DaysAlive).ToList().ForEach(item => Events.Remove(item.Key));
        }

        internal static HeroEvent? GetHeroEvent(int eventID)
        {
            if(Events.ContainsKey(eventID))
            {
                return Events[eventID];
            }
            return null;
        }

        internal static int AddHeroEvent(Hero hero1, Hero hero2, EventType type, int daysAlive)
        {
            Events.Add(++LastId, new HeroEvent(hero1.CharacterObject, hero2.CharacterObject, type, (uint)CampaignTime.Now.ToDays, (uint)daysAlive));
            return LastId;
        }

        internal static void RemoveHeroEvent(int eventID)
        {
            Events.Remove(eventID);
        }
    }

    internal sealed class HeroEventSave
    {
        [SaveableProperty(1)]
        internal string Hero1 { get; set; }

        [SaveableProperty(2)]
        internal string Hero2 { get; set; }

        [SaveableProperty(3)]
        internal int Type { get; set; }

        [SaveableProperty(4)]
        internal uint CampaignDay { get; set; }

        [SaveableProperty(5)]
        internal uint DaysAlive { get; set; }

        internal HeroEventSave(HeroEvent obj)
        {
            Hero1 = obj.Hero1.StringId;
            Hero2 = obj.Hero2.StringId;
            Type = (int)obj.Type;
            CampaignDay = obj.CampaignDay;
            DaysAlive = obj.DaysAlive;
        }

        internal HeroEvent Create()
        {
            return new HeroEvent(CharacterObject.Find(Hero1), CharacterObject.Find(Hero2), (EventType)Type, CampaignDay, DaysAlive);
        }
    }
}
