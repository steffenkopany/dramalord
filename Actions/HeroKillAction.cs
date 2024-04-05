using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;

namespace Dramalord.Actions
{
    public enum KillReason
    {
        Intercourse,
        Pregnancy,
        Bastard,
        Suicide
    }

    internal static class HeroKillAction
    {
        internal static void Apply(Hero killer, Hero victim, Hero reason, KillReason type)
        {
            if (Info.ValidateHeroMemory(killer, victim))
            {
                if (type == KillReason.Intercourse)
                {
                    KillCharacterAction.ApplyByMurder(victim, killer, false);
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenCaught(victim, killer, reason));
                }
                else if (type == KillReason.Pregnancy)
                {
                    KillCharacterAction.ApplyByMurder(victim, killer, false);
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenPregnant(victim, killer));
                }
                else if (type == KillReason.Bastard)
                {
                    KillCharacterAction.ApplyByMurder(victim, killer, false);
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenBornBastard(victim, killer, reason));
                }
                else if (type == KillReason.Suicide)
                {
                    KillCharacterAction.ApplyByMurder(victim, victim, false);
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledSuizide(victim));
                }

                DramalordEvents.OnHeroesKilled(killer, victim, reason, type);
            }
        }
    }
}
