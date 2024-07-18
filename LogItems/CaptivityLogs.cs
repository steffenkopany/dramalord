using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.LogItems
{
    public class DramalordTakePrisonerLogEntry : LogEntry, IEncyclopediaLog, IWarLog
    {
        [SaveableField(1)]
        public readonly IFaction CapturerPartyMapFaction;

        [SaveableField(2)]
        public readonly Hero Prisoner;

        [SaveableField(3)]
        public readonly Settlement CapturerSettlement;

        [SaveableField(4)]
        public readonly Hero? CapturerMobilePartyLeader;

        [SaveableField(5)]
        public readonly Hero CapturerHero;

        public override CampaignTime KeepInHistoryTime => CampaignTime.Weeks(12f);


        public DramalordTakePrisonerLogEntry(PartyBase capturerParty, Hero prisoner)
        {
            CapturerPartyMapFaction = capturerParty.MapFaction;
            CapturerHero = capturerParty.LeaderHero;
            CapturerMobilePartyLeader = capturerParty.MobileParty?.LeaderHero;
            CapturerSettlement = capturerParty.Settlement;
            Prisoner = prisoner;
        }

        public bool IsRelatedToWar(StanceLink stance, out IFaction effector, out IFaction effected)
        {
            IFaction faction = stance.Faction1;
            IFaction faction2 = stance.Faction2;
            effector = CapturerPartyMapFaction.MapFaction;
            effected = Prisoner.MapFaction;
            if (CapturerPartyMapFaction != faction || Prisoner.MapFaction != faction2)
            {
                if (CapturerPartyMapFaction == faction2)
                {
                    return Prisoner.MapFaction == faction;
                }

                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return GetNotificationText().ToString();
        }

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=QRJQ9Wgv}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has been taken prisoner by the {CAPTOR_FACTION}.");
            if (CapturerHero != null)
            {
                textObject = new TextObject("{=Ebb7aH3T}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has been taken prisoner by {CAPTURER_LORD.LINK}{?CAPTURER_LORD_HAS_FACTION_LINK} of the {CAPTURER_LORD_FACTION_LINK}{?}{\\?}.");
                StringHelpers.SetCharacterProperties("CAPTURER_LORD", CapturerHero.CharacterObject, textObject);
                Clan clan = CapturerHero.Clan;
                if (clan != null && !clan.IsMinorFaction)
                {
                    textObject.SetTextVariable("CAPTURER_LORD_FACTION_LINK", CapturerHero.MapFaction.EncyclopediaLinkWithName);
                    textObject.SetTextVariable("CAPTURER_LORD_HAS_FACTION_LINK", 1);
                }
            }

            textObject.SetTextVariable("CAPTOR_FACTION", CapturerPartyMapFaction.InformalName);
            StringHelpers.SetCharacterProperties("PRISONER_LORD", Prisoner.CharacterObject, textObject);
            Clan clan2 = Prisoner.Clan;
            if (clan2 != null && !clan2.IsMinorFaction)
            {
                textObject.SetTextVariable("PRISONER_LORD_FACTION_LINK", Prisoner.MapFaction.EncyclopediaLinkWithName);
                textObject.SetTextVariable("PRISONER_LORD_HAS_FACTION_LINK", 1);
            }

            return textObject;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            if (obj != Prisoner && (CapturerSettlement == null || obj != CapturerSettlement))
            {
                if (CapturerMobilePartyLeader != null)
                {
                    return obj == CapturerMobilePartyLeader;
                }

                return false;
            }

            return true;
        }

        public TextObject GetEncyclopediaText()
        {
            return GetNotificationText();
        }
    }

    public class DramalordEndCaptivityLogEntry : LogEntry, IEncyclopediaLog
    {
        [SaveableField(1)]
        public readonly IFaction CapturerMapFaction;

        [SaveableField(2)]
        public readonly Hero Prisoner;

        [SaveableProperty(3)]
        public EndCaptivityDetail Detail { get; private set; }

        public bool IsVisibleNotification => true;

        public DramalordEndCaptivityLogEntry(Hero prisoner, IFaction capturerMapFaction, EndCaptivityDetail detail)
        {
            CapturerMapFaction = capturerMapFaction;
            Prisoner = prisoner;
            Detail = detail;
        }

        public override string ToString()
        {
            return GetEncyclopediaText().ToString();
        }

        public TextObject GetEncyclopediaText()
        {
            return GetNotificationText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            if (obj != Prisoner)
            {
                return obj == Prisoner.Clan;
            }

            return true;
        }

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=6u3t174w}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} is now free.");
            switch (Detail)
            {
                case EndCaptivityDetail.Death:
                    textObject = new TextObject("{=XbQFAKUz}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has died while being held captive by the {CAPTURER_FACTION}.");
                    break;
                case EndCaptivityDetail.Ransom:
                    textObject = new TextObject("{=pX0MgdZA}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has been ransomed from the {CAPTURER_FACTION}.");
                    break;
                case EndCaptivityDetail.ReleasedAfterBattle:
                    textObject = new TextObject("{=hp4jLl3M}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has been released after battle.");
                    break;
                case EndCaptivityDetail.ReleasedAfterEscape:
                    textObject = new TextObject("{=krTrNonp}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has escaped from captivity.");
                    break;
                case EndCaptivityDetail.ReleasedAfterPeace:
                    textObject = new TextObject("{=wlhJGG0q}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has been freed because of a peace declaration.");
                    break;
                case EndCaptivityDetail.ReleasedByCompensation:
                    textObject = new TextObject("{=krTrNonp}{PRISONER_LORD.LINK}{?PRISONER_LORD_HAS_FACTION_LINK} of the {PRISONER_LORD_FACTION_LINK}{?}{\\?} has escaped from captivity.");
                    break;
            }

            Clan clan = Prisoner.Clan;
            if (clan != null && !clan.IsMinorFaction)
            {
                textObject.SetTextVariable("PRISONER_LORD_FACTION_LINK", Prisoner.MapFaction.EncyclopediaLinkWithName);
                textObject.SetTextVariable("PRISONER_LORD_HAS_FACTION_LINK", 1);
            }

            StringHelpers.SetCharacterProperties("PRISONER_LORD", Prisoner.CharacterObject, textObject);
            if (CapturerMapFaction != null)
            {
                textObject.SetTextVariable("CAPTURER_FACTION", CapturerMapFaction.InformalName);
            }

            return textObject;
        }

        public override void GetConversationScoreAndComment(Hero talkTroop, bool findString, out string comment, out ImportanceEnum score)
        {
            score = ImportanceEnum.Zero;
            comment = "";
            if (talkTroop == Prisoner && talkTroop.IsPlayerCompanion)
            {
                if (Detail == EndCaptivityDetail.Ransom)
                {
                    score = ImportanceEnum.VeryImportant;
                    comment = "str_comment_captivity_release_companion_ransom";
                }
                else
                {
                    score = ImportanceEnum.VeryImportant;
                    comment = "str_comment_captivity_release_companion";
                }
            }
        }
    }
}