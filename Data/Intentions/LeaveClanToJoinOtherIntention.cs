using Dramalord.Actions;
using Dramalord.Extensions;
using Dramalord.Notifications.Logs;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class LeaveClanToJoinOtherIntention : Intention
    {
        public LeaveClanToJoinOtherIntention(Hero intentionHero, CampaignTime validUntil) : base(intentionHero, intentionHero, validUntil)
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

            if (oldClan == Clan.PlayerClan)
            {
                TextObject banner = new TextObject("{=Dramalord082}{HERO.LINK} has left your clan.");
                StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, IntentionHero.CharacterObject, "event:/ui/notification/relation");
            }

            SelectClan(IntentionHero, oldClan, true);

            return true;
        }

        private void SelectClan(Hero hero, Clan oldClan, bool playerClanOK)
        {
            Clan? newClan = null;
            if (IntentionHero.Father != null && IntentionHero.Father.Clan != null && IntentionHero.Father.Clan != oldClan && !IntentionHero.Father.Clan.IsEliminated && (playerClanOK ||IntentionHero.Father.Clan != Clan.PlayerClan))
            {
                newClan = IntentionHero.Father.Clan;
            }
            else if (IntentionHero.Mother != null && IntentionHero.Mother.Clan != null && IntentionHero.Mother.Clan != oldClan && !IntentionHero.Mother.Clan.IsEliminated && (playerClanOK || IntentionHero.Mother.Clan != Clan.PlayerClan))
            {
                newClan = IntentionHero.Mother.Clan;
            }

            if (newClan == null)
            {
                newClan = IntentionHero.Siblings.FirstOrDefault(s => s.Clan != null && s.Clan != oldClan && !s.Clan.IsEliminated && (playerClanOK || s.Clan != Clan.PlayerClan))?.Clan;
            }

            if (newClan == null)
            {
                newClan = IntentionHero.GetAllRelations().FirstOrDefault(r => r.Key.Clan != null && r.Key.Clan != oldClan && !r.Key.Clan.IsEliminated && (playerClanOK || r.Key.Clan != Clan.PlayerClan) && (r.Value.Relationship == RelationshipType.Lover || r.Value.Relationship == RelationshipType.Betrothed || r.Value.Relationship == RelationshipType.Spouse)).Key?.Clan;
            }

            newClan = newClan ?? Clan.All.GetRandomElementWithPredicate(c => c != oldClan && !c.IsEliminated && (playerClanOK || c != Clan.PlayerClan));

            if(newClan == Clan.PlayerClan)
            {
                int speed = (int)Campaign.Current.TimeControlMode;
                Campaign.Current.SetTimeSpeed(0);
                TextObject title = new TextObject("{=Dramalord587}{HERO} requests to join you clan");
                TextObject text = new TextObject("{=Dramalord588}{HERO} has left {OLDCLAN} and requests to join your clan. Will you accept them?");
                title.SetTextVariable("HERO", IntentionHero.Name);
                text.SetTextVariable("HERO", IntentionHero.Name);
                text.SetTextVariable("OLDCLAN", oldClan.Name);
                InformationManager.ShowInquiry(
                        new InquiryData(
                            title.ToString(),
                            text.ToString(),
                            true,
                            true,
                            GameTexts.FindText("str_yes").ToString(),
                            GameTexts.FindText("str_no").ToString(),
                            () => {
                                JoinClan(hero, oldClan, newClan);
                            },
                            () => {
                                SelectClan(hero, oldClan, false);
                            }), true);
            }
            else if(newClan != null)
            {
                JoinClan(hero, oldClan, newClan);
            }
            else
            {
                new LeaveClanToWanderIntention(IntentionHero, CampaignTime.Now).Action();
            }
        }

        private void JoinClan(Hero hero, Clan oldClan, Clan newClan)
        {
            JoinClanAction.Apply(IntentionHero, newClan);
            IntentionHero.UpdateHomeSettlement();
            LogEntry.AddLogEntry(new JoinClanLog(IntentionHero, newClan));

            if (oldClan != null)
            {
                CampaignEventDispatcher.Instance.OnHeroChangedClan(IntentionHero, oldClan);
                MakeHeroFugitiveAction.Apply(IntentionHero);
            }

            if (oldClan == Clan.PlayerClan)
            {
                TextObject banner = new TextObject("{=Dramalord087}{HERO.LINK} joined {CLAN}.");
                StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                banner.SetTextVariable("CLAN", IntentionHero.Clan?.Name);
                MBInformationManager.AddQuickInformation(banner, 1000, IntentionHero.CharacterObject, "event:/ui/notification/relation");
            }
        }

        public override void OnConversationEnded()
        {
            Action();
        }

        public override void OnConversationStart()
        {
            
        }
    }
}
