using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data
{
    internal sealed class HeroPair
    {
        [SaveableProperty(1)]
        private string _stringId1 
        {
            get => Hero1.StringId;
            set => Hero1 = CharacterObject.Find(value);
        }

        [SaveableProperty(2)]
        private string _stringId2
        {
            get => Hero2.StringId;
            set => Hero2 = CharacterObject.Find(value);
        }

        internal CharacterObject Hero1 { get; set; }

        internal CharacterObject Hero2 { get; set; }

        internal HeroPair(CharacterObject hero1, CharacterObject hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_stringId1, _stringId2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HeroPair);
        }

        public bool Equals(HeroPair? tuple)
        {
            return tuple != null && ((_stringId1 == tuple._stringId1 && _stringId2 == tuple._stringId2) || (_stringId1 == tuple._stringId2 && _stringId2 == tuple._stringId1));
        }
    }
}
