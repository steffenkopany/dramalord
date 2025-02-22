using Dramalord.Actions;
using Dramalord.Notifications.Logs;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class LeaveClanToWanderIntention : Intention
    {
        public LeaveClanToWanderIntention(Hero intentionHero, CampaignTime validUntil) : base(intentionHero, intentionHero, validUntil)
        {
        }

        public override bool Action()
        {
            Clan oldClan = IntentionHero.Clan;

            if (IntentionHero.Clan != null)
            {    
                LeaveClanAction.Apply(IntentionHero);
                LogEntry.AddLogEntry(new LeaveClanLog(IntentionHero, oldClan, IntentionHero));
            }

            if (IntentionHero.BornSettlement == null)
            {
                IntentionHero.BornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }

            IntentionHero.SetNewOccupation(Occupation.Wanderer);
            TextObject newName = new TextObject("{=28tWEFNi}{FIRSTNAME} the Wanderer");
            newName.SetTextVariable("FIRSTNAME", IntentionHero.FirstName);
            IntentionHero.SetName(newName, IntentionHero.FirstName);
            IntentionHero.UpdateHomeSettlement();

            if(oldClan != null)
            {
                CampaignEventDispatcher.Instance.OnHeroChangedClan(IntentionHero, oldClan);
                MakeHeroFugitiveAction.Apply(IntentionHero);
            }

            if(oldClan == Clan.PlayerClan)
            {
                TextObject banner = new TextObject("{=Dramalord082}{HERO.LINK} has left your clan.");
                StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, IntentionHero.CharacterObject, "event:/ui/notification/relation");
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
