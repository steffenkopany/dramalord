using Dramalord.Data;
using Dramalord.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    public enum WitnessType
    {
        Flirting,
        Dating,
        Intercourse,
        Pregnancy,
        Bastard
    }

    internal static class HeroWitnessAction
    {
        internal static void Apply(Hero hero, Hero target, Hero witness, WitnessType type)
        {
            if (Info.ValidateHeroMemory(target, witness) && Info.ValidateHeroMemory(hero, witness))
            {
                if(type == WitnessType.Flirting)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossCaughtFlirting);
                    LogEntry.AddLogEntry(new LogWitnessFlirt(hero, target, witness));
                }
                else if(type == WitnessType.Dating)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossCaughtDate);
                    LogEntry.AddLogEntry(new LogWitnessDate(hero, target, witness));
                }
                else if (type == WitnessType.Intercourse)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossCaughtIntercourse);
                    LogEntry.AddLogEntry(new LogWitnessIntercourse(hero, target, witness));
                }
                else if (type == WitnessType.Pregnancy)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossPregnancy);
                    LogEntry.AddLogEntry(new LogWitnessPregnancy(hero, witness));
                }
                else if (type == WitnessType.Bastard)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossBastard);
                    LogEntry.AddLogEntry(new LogWitnessBastard(hero, target, witness));
                }

                DramalordEvents.OnHeroesWitness(hero, target, witness, type);
            }
        }
    }
}
