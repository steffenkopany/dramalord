using Dramalord.Data;
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
            if(Info.ValidateHeroMemory(witness, hero))
            {
                if (type == WitnessType.Flirting)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossCaughtFlirting);
                    if (DramalordMCM.Get.FlirtOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessFlirt(hero, target, witness));
                    }
                }
                else if (type == WitnessType.Dating)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossCaughtDate);
                    if (DramalordMCM.Get.AffairOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessDate(hero, target, witness));
                    }
                }
                else if (type == WitnessType.Intercourse)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossCaughtIntercourse);
                    if (DramalordMCM.Get.AffairOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessIntercourse(hero, target, witness));
                    }
                }
                else if (type == WitnessType.Pregnancy)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossPregnancy);
                    if (DramalordMCM.Get.AffairOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessPregnancy(hero, witness));
                    }
                }
                else if (type == WitnessType.Bastard)
                {
                    Info.ChangeEmotionToHeroBy(witness, hero, -DramalordMCM.Get.EmotionalLossBastard);
                    if (DramalordMCM.Get.BirthOutput)
                    {
                        LogEntry.AddLogEntry(new LogWitnessBastard(hero, target, witness));
                    }
                }
            }
            
            DramalordEvents.OnHeroesWitness(hero, target, witness, type);  
        }
    }
}
