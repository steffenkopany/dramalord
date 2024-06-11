using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Deprecated
{
    internal class HeroTuple
    {
        [SaveableProperty(1)]
        internal Hero Actor { get; set; }

        [SaveableProperty(2)]
        internal Hero Target { get; set; }

        internal HeroTuple(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
        }

        public override int GetHashCode()
        {
            return 7 + (Actor.GetHashCode() + Target.GetHashCode()) * 17 * 79;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HeroTuple);
        }

        public bool Equals(HeroTuple? tuple)
        {
            return tuple != null && (Actor == tuple.Actor && Target == tuple.Target || Actor == tuple.Target && Target == tuple.Actor);
        }
    }
}
