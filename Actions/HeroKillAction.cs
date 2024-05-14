using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

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
            if (type == KillReason.Intercourse)
            {
                KillCharacterAction.ApplyByMurder(victim, killer, false);
                if (DramalordMCM.Get.DeathOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenCaught(victim, killer, reason));
                }
            }
            else if (type == KillReason.Pregnancy)
            {
                KillCharacterAction.ApplyByMurder(victim, killer, false);
                if (DramalordMCM.Get.DeathOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenPregnant(victim, killer));
                }
            }
            else if (type == KillReason.Bastard)
            {
                KillCharacterAction.ApplyByMurder(victim, killer, false);
                if (DramalordMCM.Get.DeathOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenBornBastard(victim, killer, reason));
                }
            }
            else if (type == KillReason.Suicide)
            {
                KillCharacterAction.ApplyByMurder(victim, victim, false);
                if (DramalordMCM.Get.DeathOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledSuicide(victim));
                }
            }

            if(killer == Hero.MainHero && type != KillReason.Suicide)
            {
                TextObject banner = new TextObject("{=Dramalord316}You killed {HERO.LINK}");
                StringHelpers.SetCharacterProperties("HERO", victim.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, victim.CharacterObject, "event:/ui/notification/relation");
            }

            DramalordEvents.OnHeroesKilled(killer, victim, reason, type);
        }
    }
}
