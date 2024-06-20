using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal class HeroOrphan
    {
        internal CharacterObject Character;

        internal HeroOrphan(CharacterObject character)
        {
            Character = character;
        }

        internal HeroOrphan(HeroOrphan other)
        {
            Character = other.Character;
        }
        public override int GetHashCode()
        {
            return Character.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HeroOrphan);
        }

        public bool Equals(HeroOrphan? orphan)
        {
            return orphan != null && orphan.Character == Character;
        }
    }
    internal static class DramalordOrphanage
    {
        internal static List<HeroOrphan> Orphans { get; set; } = new();

        internal static CharacterObject? OrphanManager = null;

        internal static Dictionary<Settlement, CharacterObject> OrphanCharacters { get; set; } = new ();

        internal static Dictionary<CharacterObject, uint> LastAdoptionDays { get; set; } = new();

        internal static uint GetLastAdoptionDay(Hero hero)
        {
            if(LastAdoptionDays.ContainsKey(hero.CharacterObject))
            {
                return LastAdoptionDays[hero.CharacterObject]; 
            }
            return 0;
        }

        internal static void SetLastAdoptionDay(Hero hero, uint days)
        {
            if(LastAdoptionDays.ContainsKey(hero.CharacterObject))
            {
                LastAdoptionDays[hero.CharacterObject] = days;
            }
            else
            {
                LastAdoptionDays.Add(hero.CharacterObject, days);
            }
        }

        internal static CharacterObject? PullRandomOrphan()
        {
            HeroOrphan? orphan = Orphans.GetRandomElement();
            if(orphan != null)
            {
                Orphans.Remove(orphan);
                return orphan.Character;
            }
            return null;
        }

        internal static void AddOrphan(CharacterObject orphan)
        {
            HeroOrphan newOrphan = new(orphan);
            if(!Orphans.Contains(newOrphan))
            {
                Orphans.Add(newOrphan);
            }
        }

        internal static void RemoveOrphan(CharacterObject orphan)
        {
            HeroOrphan newOrphan = new(orphan);
            if (Orphans.Contains(newOrphan))
            {
                Orphans.Remove(newOrphan);
            }
        }
    }

    internal sealed class HeroOrphanSave
    {
        [SaveableProperty(1)]
        internal string Character { get; set; }

        internal HeroOrphanSave(HeroOrphan orphan)
        {
            Character = orphan.Character.StringId;
        }

        internal HeroOrphan? Create()
        {
            CharacterObject obj = CharacterObject.Find(Character);
            if(obj != null)
            {
                return new HeroOrphan(CharacterObject.Find(Character));
            }
            return null;
        }
    }
}
