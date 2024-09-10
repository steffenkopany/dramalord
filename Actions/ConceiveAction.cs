using Dramalord.Data;
using Dramalord.LogItems;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class ConceiveAction
    {
        internal static bool Apply(Hero mother, Hero father)
        {
            if(mother.IsPregnant)
            {
                return false;
            }

            int eventId = DramalordEvents.Instance.AddEvent(mother, father, EventType.Pregnancy, 1000);
            DramalordPregancies.Instance.AddPregnancy(mother, father, CampaignTime.Now, eventId);

            if (mother == Hero.MainHero)
            {
                TextObject banner = new TextObject("{=Dramalord075}You got pregnant from {HERO.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", father.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, father.CharacterObject, "event:/ui/notification/relation");
            }
            else if (father == Hero.MainHero)
            {
                TextObject banner = new TextObject("{=Dramalord076}{HERO.LINK} got pregnant from you.");
                StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, mother.CharacterObject, "event:/ui/notification/relation");
            }

            mother.IsPregnant = true;

            if ((mother.Clan == Clan.PlayerClan || father.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new ConceiveChildLog(mother, father));
            }

            return true;
        }
    }
}
