using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Deprecated
{
    internal class HeroMemoryData
    {
        [SaveableProperty(1)]
        internal float Emotion { get; set; }

        [SaveableProperty(2)]
        internal double LastAdoption { get; set; }

        [SaveableProperty(3)]
        internal double LastDate { get; set; }

        [SaveableProperty(4)]
        internal bool IsCouple { get; set; }

        [SaveableProperty(5)]
        internal double LastMet { get; set; }

        [SaveableProperty(6)]
        internal int FlirtCount { get; set; }

        internal HeroMemoryData(float emotion, double lastAdoption, double lastDate, bool isCouple, double lastMet)
        {
            Emotion = emotion;
            LastAdoption = lastAdoption;
            LastDate = lastDate;
            IsCouple = isCouple;
            LastMet = lastMet;
            FlirtCount = 0;
        }

        internal HeroMemoryData(HeroMemoryData other)
        {
            Emotion = other.Emotion;
            LastAdoption = other.LastAdoption;
            LastDate = other.LastDate;
            IsCouple = other.IsCouple;
            LastMet = other.LastMet;
            FlirtCount = other.FlirtCount;
        }
    }
}
