using Dramalord.Data;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroKillAction
    {
        internal static void Apply(Hero killer, Hero victim, Hero reason, EventType type)
        {
            if (type == EventType.Intercourse || type == EventType.Date)
            {
                KillCharacterAction.ApplyByMurder(victim, killer, false);
                if (DramalordMCM.Get.DeathOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenCaught(victim, killer, reason));
                }
            }
            else if (type == EventType.Pregnancy)
            {
                KillCharacterAction.ApplyByMurder(victim, killer, false);
                if (DramalordMCM.Get.DeathOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenPregnant(victim, killer));
                }
            }
            else if (type == EventType.Birth)
            {
                KillCharacterAction.ApplyByMurder(victim, killer, false);
                if (DramalordMCM.Get.DeathOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogKilledWhenBornBastard(victim, killer, reason));
                }
            }

            if(killer == Hero.MainHero)
            {
                TextObject banner = new TextObject("{=Dramalord316}You killed {HERO.LINK}");
                StringHelpers.SetCharacterProperties("HERO", victim.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, victim.CharacterObject, "event:/ui/notification/relation");
            }

            DramalordEventCallbacks.OnHeroesKilled(killer, victim, reason, type);
        }
    }
}
