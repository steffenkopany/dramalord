using Dramalord.Actions;
using Dramalord.Notifications.Logs;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class OrphanizeChildIntention : Intention
    {
        public OrphanizeChildIntention(Hero child, Hero intentionHero, CampaignTime validUntil) : base(intentionHero, child, validUntil)
        {

        }

        public override bool Action()
        {
            Clan oldClan = Target.Clan;

            OrphanizeAction.Apply(Target);

            if (oldClan == Clan.PlayerClan)
            {
                TextObject textObject = new TextObject("{=Dramalord250}{HERO1.LINK} put child {CHILD.LINK} into an orphanage.");
                StringHelpers.SetCharacterProperties("HERO1", IntentionHero.CharacterObject, textObject);
                StringHelpers.SetCharacterProperties("CHILD", Target.CharacterObject, textObject);
                MBInformationManager.AddQuickInformation(textObject, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
            }

            if (oldClan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new OrphanizeChildLog(IntentionHero, Target));
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
