using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Notifications;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class LeaveClanEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableProperty(4)]
        public Clan LeftClan { get; private set; }

        public LeaveClanEvent(Hero actor)
        {
            Actor = actor;
            Target = actor;
            LeftClan = actor.Clan;
            IsKnownTo.Add(actor);
        }

        public void Action(int modifier = 0)
        {
            if(Actor.Clan != null)
            {
                Kingdom? kingdom = Actor.MapFaction as Kingdom;
                if (kingdom != null && kingdom.RulingClan != null && kingdom.RulingClan.Leader == Actor)
                {
                    Campaign.Current.KingdomManager.AbdicateTheThrone(kingdom);
                }

                if (Actor.Clan != null && Actor.Clan.Leader == Actor)
                {
                    ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(Actor.Clan);
                }

                if (Actor.GovernorOf != null)
                {
                    ChangeGovernorAction.RemoveGovernorOf(Actor);
                }

                if (Actor.PartyBelongedTo != null)
                {
                    MobileParty party = Actor.PartyBelongedTo;
                    if (party.Army != null && party.Army.LeaderParty == party)
                    {
                        if (party.Party.LeaderHero == Actor)
                        {
                            DisbandArmyAction.ApplyByUnknownReason(party.Army);
                        }
                    }

                    if (party.Party.IsActive && party.Party.LeaderHero == Actor)
                    {
                        DisbandPartyAction.StartDisband(party);
                        party.Party.SetCustomOwner(null);
                        DestroyPartyAction.Apply(null, party); // test
                    }
                    else if (party.IsActive)
                    {
                        party.MemberRoster.RemoveTroop(Actor.CharacterObject);
                    }
                }

                if (Actor.IsPlayerCompanion)
                {
                    Actor.CompanionOf = null;
                }

                Actor.Clan = null;

                CampaignEventDispatcher.Instance.OnHeroChangedClan(Actor, LeftClan);
            }
        }

        public void AfterDialog()
        {
            if(LeftClan != null)
                AddLogEntry(this);
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetTextVariables(ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_LEAVE_CLAN), Actor), LeftClan.EncyclopediaLinkWithName);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            return null;
        }

        public DialogFlow? GetInitiationDialog()
        {
            return null;
        }


        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            return null;
        }

        public DialogFlow? GetGossipDialog(Hero speaker)
        {
            return null;
        }

        public DialogFlow? GetBlackmailDialog(Hero speaker)
        {
            return null;
        }

        public void ReactionResult(Hero speaker, Hero listener, out int trust, out int love)
        {
            trust = 0;
            love = 0;
        }

        public bool IsVisibleNotification => true;

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionPolitical;
    }
}
