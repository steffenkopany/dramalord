using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Deprecated
{
    internal class HeroOffspringData
    {
        [SaveableField(1)]
        internal Hero Father;

        [SaveableField(2)]
        internal double Conceived;

        [SaveableField(3)]
        internal bool ByForce;

        internal HeroOffspringData(Hero father, Hero mother, bool byForce)
        {
            Father = father;
            Conceived = CampaignTime.Now.ToDays;
            ByForce = byForce;
        }

        internal HeroOffspringData(HeroOffspringData other)
        {
            Father = other.Father;
            Conceived = other.Conceived;
            ByForce = other.ByForce;
        }
    }
}
