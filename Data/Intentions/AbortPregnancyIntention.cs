using Dramalord.Actions;
using Dramalord.Notifications.Logs;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class AbortPregnancyIntention : Intention
    {
        [SaveableField(4)]
        public readonly HeroPregnancy Pregnancy;

        public AbortPregnancyIntention(HeroPregnancy pregnancy, Hero intentionHero, CampaignTime validUntil) : base(intentionHero, intentionHero, validUntil)
        {
            Pregnancy = pregnancy;
        }

        public override bool Action()
        {
            AbortPregnancyAction.Apply(IntentionHero);

            if (IntentionHero == Hero.MainHero)
            {
                TextObject banner = new TextObject("{=Dramalord517}You aborted your unborn child of {HERO.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", Pregnancy.Father.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, Pregnancy.Father.CharacterObject, "event:/ui/notification/relation");
            }
            else if (Pregnancy.Father == Hero.MainHero)
            {
                TextObject banner = new TextObject("{=Dramalord518}{HERO.LINK} aborted the unborn child of you.");
                StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
            }
            else if (IntentionHero.Clan == Clan.PlayerClan)
            {
                TextObject banner = new TextObject("{=Dramalord519}{HERO.LINK} aborted their unborn child of {HERO2.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                StringHelpers.SetCharacterProperties("HERO2", Pregnancy.Father.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
            }

            if ((IntentionHero.Clan == Clan.PlayerClan || Pregnancy.Father.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new AbortChildLog(IntentionHero, Pregnancy.Father));
            }

            return true;
        }

        public override void OnConversationEnded()
        {
         
        }

        public override void OnConversationStart()
        {
           
        }
    }
}
