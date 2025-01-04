using Dramalord.Data;
using Dramalord.LogItems;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class AbortionAction
    {
        internal static bool Apply(Hero mother)
        {
            HeroPregnancy? pregnancy = DramalordPregancies.Instance.GetPregnancy(mother);
            if (pregnancy == null)
            {
                return false;
            }

            if (mother == Hero.MainHero)
            {
                TextObject banner = new TextObject("{=Dramalord517}You aborted your unborn child of {HERO.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", pregnancy.Father.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, pregnancy.Father.CharacterObject, "event:/ui/notification/relation");
            }
            else if (pregnancy.Father == Hero.MainHero)
            {
                TextObject banner = new TextObject("{=Dramalord518}{HERO.LINK} aborted the unborn child of you.");
                StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, mother.CharacterObject, "event:/ui/notification/relation");
            }
            else if (mother.Clan == Clan.PlayerClan)
            {
                TextObject banner = new TextObject("{=Dramalord519}{HERO.LINK} aborted their unborn child of {HERO2.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                StringHelpers.SetCharacterProperties("HERO2", pregnancy.Father.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, mother.CharacterObject, "event:/ui/notification/relation");
            }

            mother.IsPregnant = false;

            if ((mother.Clan == Clan.PlayerClan || pregnancy.Father.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new AbortChildLog(mother, pregnancy.Father));
            }

            DramalordPregancies.Instance.RemovePregnancy(mother);

            return true;
        }
    }
}
