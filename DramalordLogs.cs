using Dramalord.Data;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord
{
    public class EncyclopediaLogStartAffair : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26000)]
        public readonly Hero Hero1;

        [SaveableField(26001)]
        public readonly Hero Hero2;

        public EncyclopediaLogStartAffair(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord062}{HERO1.LINK} and {HERO2.LINK} started an affair.");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2;
        }
    }

    public class EncyclopediaLogBreakup : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26002)]
        public readonly Hero Hero1;

        [SaveableField(26003)]
        public readonly Hero Hero2;

        public EncyclopediaLogBreakup(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectPositive;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord063}{HERO1.LINK} ended their affair with {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2;
        }
    }

    public class EncyclopediaLogDivorce : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26006)]
        public readonly Hero Hero1;

        [SaveableField(26007)]
        public readonly Hero Hero2;

        public EncyclopediaLogDivorce(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.MarriageOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectPositive;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord064}{HERO1.LINK} ended their marriage with {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2;
        }
    }

    public class EncyclopediaLogPutChildToOrphanage : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26008)]
        public readonly Hero Hero;

        [SaveableField(26009)]
        public readonly Hero Child; 

        public EncyclopediaLogPutChildToOrphanage(Hero hero1, Hero child)
        {
            Hero = hero1;
            Child = child;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.BirthOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord065}{HERO1.LINK} put child {CHILD.LINK} into an orphanage.");
            StringHelpers.SetCharacterProperties("HERO1", Hero.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Child.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == Child;
        }
    }

    public class EncyclopediaLogKilledWhenCaught : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26010)]
        public readonly Hero Hero;

        [SaveableField(26011)]
        public readonly Hero Killer;

        [SaveableField(26012)]
        public readonly Hero Lover;

        public EncyclopediaLogKilledWhenCaught(Hero victim, Hero killer, Hero lover)
        {
            Hero = victim;
            Killer = killer;
            Lover = lover;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.DeathOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord066}{KILLER.LINK} killed {VICTIM.LINK} after catching them with {LOVER.LINK}.");
            StringHelpers.SetCharacterProperties("KILLER", Killer.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("VICTIM", Hero.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("LOVER", Lover.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == Killer || obj == Lover;
        }
    }

    public class EncyclopediaLogJoinClan : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26013)]
        public readonly Hero Hero;

        [SaveableField(26014)]
        public readonly Clan Clan;

        public EncyclopediaLogJoinClan(Hero hero, Clan clan)
        {
            Hero = hero;
            Clan = clan;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.ClanOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord067}{HERO.LINK} joined {CLAN}.");
            StringHelpers.SetCharacterProperties("HERO", Hero.CharacterObject, textObject);
            textObject.SetTextVariable("CLAN", Clan.EncyclopediaLinkWithName);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == Clan;
        }
    }

    public class EncyclopediaLogKilledWhenPregnant : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26015)]
        public readonly Hero Hero;

        [SaveableField(26016)]
        public readonly Hero Killer;

        public EncyclopediaLogKilledWhenPregnant(Hero victim, Hero killer)
        {
            Hero = victim;
            Killer = killer;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.DeathOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord099}{KILLER.LINK} killed {VICTIM.LINK} after noticing she is pregnant from someone else.");
            StringHelpers.SetCharacterProperties("KILLER", Killer.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("VICTIM", Hero.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == Killer;
        }
    }

    public class LogIntercourse : LogEntry, IChatNotification
    {
        [SaveableField(26017)]
        public readonly Hero Hero1;

        [SaveableField(26018)]
        public readonly Hero Hero2;

        [SaveableField(26019)]
        public readonly bool ByForce;

        public LogIntercourse(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => Hero1 == Hero.MainHero || Hero2 == Hero.MainHero ? ChatNotificationType.PlayerClanPositive : ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord114}{HERO1.LINK} had intercourse with {HERO2.LINK}");

            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }
    }

    public class EncyclopediaLogConceived : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26020)]
        public readonly Hero Hero1;

        [SaveableField(26021)]
        public readonly Hero Hero2;

        [SaveableField(26022)]
        public readonly bool ByForce;

        public EncyclopediaLogConceived(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.BirthOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord115}{HERO1.LINK} was impregnated by {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.IsFemale ? Hero1.CharacterObject : Hero2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero1.IsFemale ? Hero2.CharacterObject : Hero1.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2;
        }
    }

    public class LogFlirt : LogEntry, IChatNotification
    {
        [SaveableField(26023)]
        public readonly Hero Hero1;

        [SaveableField(26024)]
        public readonly Hero Hero2;

        public LogFlirt(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.FlirtOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord116}{HERO1.LINK} flirted with {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }
    }

    public class LogAffairMeeting : LogEntry, IChatNotification
    {
        [SaveableField(26025)]
        public readonly Hero Hero1;

        [SaveableField(26026)]
        public readonly Hero Hero2;

        public LogAffairMeeting(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord119}{HERO1.LINK} was meeting in secret with {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }
    }

    public class EncyclopediaLogAdopted : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26027)]
        public readonly Hero Hero1;

        [SaveableField(26028)]
        public readonly Hero Hero2;

        [SaveableField(26029)]
        public readonly Hero Child;

        public EncyclopediaLogAdopted(Hero hero1, Hero hero2, Hero child)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Child = child;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.BirthOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectPositive;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord120}{HERO1.LINK} and {HERO2.LINK} adopted orphan {CHILD.LINK}");

            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Child.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2 || obj == Child;
        }
    }

    public class EncyclopediaLogBirth : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26030)]
        public readonly Hero Hero1;

        [SaveableField(26031)]
        public readonly Hero Hero2;

        [SaveableField(26032)]
        public readonly Hero Child;

        public EncyclopediaLogBirth(Hero hero1, Hero hero2, Hero child)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Child = child;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.BirthOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord121}{CHILD.LINK} was born to {HERO1.LINK} and {HERO2.LINK}");

            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Child.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2 || obj == Child;
        }
    }

    public class EncyclopediaLogLeaveClan : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26033)]
        public readonly Hero Hero;

        [SaveableField(26034)]
        public readonly Clan OldClan;

        [SaveableField(26035)]
        public readonly Hero CausedBy;

        public EncyclopediaLogLeaveClan(Hero hero, Clan oldClan, Hero causedBy)
        {
            Hero = hero;
            OldClan = oldClan;
            CausedBy = causedBy;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.ClanOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject();
            if (CausedBy == Hero)
            {
                textObject = new TextObject("{=Dramalord122}{HERO.LINK} has left {OLDCLAN}");
            }
            else
            {
                textObject = new TextObject("{=Dramalord123}{HERO.LINK} was thrown out of {OLDCLAN} by {CAUSEDBY.LINK}");
            }
            StringHelpers.SetCharacterProperties("HERO", Hero.CharacterObject, textObject);
            textObject.SetTextVariable("OLDCLAN", OldClan.EncyclopediaLinkWithName);
            StringHelpers.SetCharacterProperties("CAUSEDBY", CausedBy.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == OldClan || obj == CausedBy;
        }
    }

    public class LogWitnessFlirt : LogEntry, IChatNotification
    {
        [SaveableField(26036)]
        public readonly Hero Hero1;

        [SaveableField(26037)]
        public readonly Hero Hero2;

        [SaveableField(26038)]
        public readonly Hero Witness;

        public LogWitnessFlirt(Hero hero1, Hero hero2, Hero witness)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Witness = witness;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.FlirtOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord124}{WITNESS.LINK} witnessed {HERO1.LINK} flirting with {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("WITNESS", Witness.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }
    }

    public class LogWitnessDate : LogEntry, IChatNotification
    {
        [SaveableField(26039)]
        public readonly Hero Hero1;

        [SaveableField(26040)]
        public readonly Hero Hero2;

        [SaveableField(26041)]
        public readonly Hero Witness;

        public LogWitnessDate(Hero hero1, Hero hero2, Hero witness)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Witness = witness;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord125}{WITNESS.LINK} witnessed {HERO1.LINK} being on a date with {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("WITNESS", Witness.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }
    }

    public class LogWitnessIntercourse : LogEntry, IChatNotification
    {
        [SaveableField(26042)]
        public readonly Hero Hero1;

        [SaveableField(26043)]
        public readonly Hero Hero2;

        [SaveableField(26044)]
        public readonly Hero Witness;

        public LogWitnessIntercourse(Hero hero1, Hero hero2, Hero witness)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Witness = witness;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord126}{WITNESS.LINK} caught {HERO1.LINK} in bed with {HERO2.LINK}");
            StringHelpers.SetCharacterProperties("WITNESS", Witness.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }
    }

    public class LogWitnessPregnancy : LogEntry, IChatNotification
    {
        [SaveableField(26045)]
        public readonly Hero Hero1;

        [SaveableField(26046)]
        public readonly Hero Witness;

        public LogWitnessPregnancy(Hero hero1, Hero witness)
        {
            Hero1 = hero1;
            Witness = witness;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord126}{WITNESS.LINK} noticed {HERO1.LINK} is pregnant from someone else.");
            StringHelpers.SetCharacterProperties("WITNESS", Witness.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            return textObject;
        }
    }

    public class LogWitnessBastard : LogEntry, IChatNotification
    {
        [SaveableField(26047)]
        public readonly Hero Hero1;

        [SaveableField(26048)]
        public readonly Hero Bastard;

        [SaveableField(26049)]
        public readonly Hero Witness;

        public LogWitnessBastard(Hero hero, Hero bastard, Hero witness)
        {
            Hero1 = hero;
            Bastard = bastard;
            Witness = witness;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.BirthOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord127}{WITNESS.LINK} noticed {CHILD.LINK} birthed by {HERO1.LINK} was not theirs.");
            StringHelpers.SetCharacterProperties("WITNESS", Witness.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Bastard.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            return textObject;
        }
    }

    public class EncyclopediaLogKilledWhenBornBastard : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26050)]
        public readonly Hero Hero;

        [SaveableField(26051)]
        public readonly Hero Killer;

        [SaveableField(26052)]
        public readonly Hero Bastard;

        public EncyclopediaLogKilledWhenBornBastard(Hero victim, Hero killer, Hero bastard)
        {
            Hero = victim;
            Killer = killer;
            Bastard = bastard;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.DeathOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord128}{KILLER.LINK} killed {HERO.LINK} for giving birth to bastard {CHILD.LINK}");
            StringHelpers.SetCharacterProperties("KILLER", Killer.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO", Hero.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("CHILD", Bastard.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero || obj == Killer;
        }
    }

    public class LogUsedToy : LogEntry, IChatNotification
    {
        [SaveableField(26053)]
        public readonly Hero Hero1;

        public LogUsedToy(Hero hero)
        {
            Hero1 = hero;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord129}{HERO.LINK} played with their toy while thinking of you.");
            StringHelpers.SetCharacterProperties("HERO", Hero1.CharacterObject, textObject);
            return textObject;
        }
    }

    public class EncyclopediaLogKilledSuicide : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26054)]
        public readonly Hero Hero;

        public EncyclopediaLogKilledSuicide(Hero victim)
        {
            Hero = victim;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.DeathOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord238}{HERO.LINK} commited suicide.");
            StringHelpers.SetCharacterProperties("HERO", Hero.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero;
        }
    }

    public class EncyclopediaLogClanLeftKingdom : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26055)]
        public readonly Clan Clan;

        [SaveableField(26056)]
        public readonly Kingdom OldKingdom;

        [SaveableField(26057)]
        public readonly bool Forced;

        public EncyclopediaLogClanLeftKingdom(Clan clan, Kingdom oldKingdom, bool forced)
        {
            Clan = clan;
            OldKingdom = oldKingdom;
            Forced = forced;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.KingdomOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectPositive;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject();
            if (Forced && OldKingdom.Leader != null)
            {
                textObject = new TextObject("{=Dramalord236}{HERO.LINK} banished {CLAN} from {KINGDOM}.");
                StringHelpers.SetCharacterProperties("HERO", OldKingdom.Leader.CharacterObject, textObject);
                textObject.SetTextVariable("CLAN", Clan.EncyclopediaLinkWithName);
                textObject.SetTextVariable("KINGDOM", OldKingdom.EncyclopediaLinkWithName);
            }
            else
            {
                textObject = new TextObject("{=Dramalord235}{CLAN} left {KINGDOM}.");
                textObject.SetTextVariable("CLAN", Clan.EncyclopediaLinkWithName);
                textObject.SetTextVariable("KINGDOM", OldKingdom.EncyclopediaLinkWithName);
            }
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Clan || obj == OldKingdom;
        }
    }

    public class EncyclopediaLogClanJoinedKingdom : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26058)]
        public readonly Clan Clan;

        [SaveableField(26059)]
        public readonly Kingdom NewKingdom;

        public EncyclopediaLogClanJoinedKingdom(Clan clan, Kingdom oldKingdom)
        {
            Clan = clan;
            NewKingdom = oldKingdom;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.KingdomOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectPositive;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord237}{CLAN} joined {KINGDOM}.");
            textObject.SetTextVariable("CLAN", Clan.EncyclopediaLinkWithName);
            textObject.SetTextVariable("KINGDOM", NewKingdom.EncyclopediaLinkWithName);
         
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Clan || obj == NewKingdom;
        }
    }

    public class DramalordTakePrisonerLogEntry : LogEntry, IEncyclopediaLog, IWarLog
    {
        [SaveableField(26060)]
        public readonly IFaction CapturerPartyMapFaction;

        [SaveableField(26061)]
        public readonly Hero Prisoner;

        [SaveableField(26062)]
        public readonly Settlement CapturerSettlement;

        [SaveableField(26063)]
        public readonly Hero? CapturerMobilePartyLeader;

        [SaveableField(26064)]
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
        [SaveableField(26065)]
        public readonly IFaction CapturerMapFaction;

        [SaveableField(26066)]
        public readonly Hero Prisoner;

        [SaveableProperty(26067)]
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

    public class LogConfrontation : LogEntry, IChatNotification
    {
        [SaveableField(26068)]
        public readonly Hero Hero1;

        [SaveableField(26069)]
        public readonly Hero Hero2;

        [SaveableField(26070)]
        public readonly Hero Hero3;

        [SaveableField(26071)]
        public readonly int EType;

        public LogConfrontation(Hero hero1, Hero hero2, Hero hero3, HeroEvent @event)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Hero3 = hero3;
            EType = (int)@event.Type;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.ConfrontationOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            EventType eventType = (EventType)EType;
            TextObject tobj = new TextObject();
            if(eventType == EventType.Date)
            {
                tobj = new TextObject("{=Dramalord398}{HERO1.LINK} confronted {HERO2.LINK} about the date with {HERO3.LINK} they have witnessed.");
            }
            else if (eventType == EventType.Intercourse)
            {
                tobj = new TextObject("{=Dramalord399}{HERO1.LINK} confronted {HERO2.LINK} about the intercourse with {HERO3.LINK} they have witnessed.");
            }
            else if (eventType == EventType.Marriage)
            {
                tobj = new TextObject("{=Dramalord400}{HERO1.LINK} confronted {HERO2.LINK} about the marriage with {HERO3.LINK} they have witnessed.");
            }
            else if (eventType == EventType.Pregnancy)
            {
                tobj = new TextObject("{=Dramalord401}{HERO1.LINK} confronted {HERO2.LINK} about being pregnant from {HERO3.LINK}.");
            }
            else if (eventType == EventType.Birth)
            {
                tobj = new TextObject("{=Dramalord402}{HERO1.LINK} confronted {HERO2.LINK} about giving birth to {HERO3.LINK} who is not their child.");
            }

            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, tobj);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, tobj);
            StringHelpers.SetCharacterProperties("HERO3", Hero3.CharacterObject, tobj);
            return tobj;
        }
    }

    public class LogWitnessMarriage : LogEntry, IChatNotification
    {
        [SaveableField(26072)]
        public readonly Hero Hero1;

        [SaveableField(26073)]
        public readonly Hero Hero2;

        [SaveableField(26074)]
        public readonly Hero Witness;

        public LogWitnessMarriage(Hero hero1, Hero hero2, Hero witness)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Witness = witness;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.MarriageOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = new TextObject("{=Dramalord342}{WITNESS.LINK} saw {HERO1.LINK} and {HERO2.LINK} getting married.");
            StringHelpers.SetCharacterProperties("WITNESS", Witness.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }
    }

    public class LogGossipConversation : LogEntry, IChatNotification
    {
        [SaveableField(26075)]
        public readonly Hero Hero1;

        [SaveableField(26076)]
        public readonly Hero Hero2;

        [SaveableField(26077)]
        public readonly Hero Hero3;

        [SaveableField(26078)]
        public readonly Hero Hero4;

        [SaveableField(26079)]
        public readonly int EType;

        [SaveableField(26080)]
        public readonly int MType;

        public LogGossipConversation(Hero hero1, Hero hero2, Hero hero3, Hero hero4, EventType eType, MemoryType mType)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            Hero3 = hero3;
            Hero4 = hero4;
            EType = (int)eType;
            MType = (int)mType;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.ConfrontationOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            MemoryType memoryType = (MemoryType)MType;
            EventType eventType = (EventType)EType;
            TextObject tobj = new TextObject();
            if (memoryType == MemoryType.Confession)
            {
                if(eventType == EventType.Date)
                {
                    tobj = new TextObject("{=Dramalord347}{HERO1.LINK} confessed to {HERO2.LINK} that they had a date with {HERO3.LINK}");
                }
                else if(eventType == EventType.Intercourse)
                {
                    tobj = new TextObject("{=Dramalord348}{HERO1.LINK} confessed to {HERO2.LINK} that they had intercourse with {HERO3.LINK}");
                }
                else if (eventType == EventType.Marriage)
                {
                    tobj = new TextObject("{=Dramalord349}{HERO1.LINK} confessed to {HERO2.LINK} that they married {HERO3.LINK}");
                }
                else if (eventType == EventType.Pregnancy)
                {
                    tobj = new TextObject("{=Dramalord403}{HERO1.LINK} confessed to {HERO2.LINK} that they are pregnant from {HERO3.LINK}");
                }
                else if (eventType == EventType.Birth)
                {
                    tobj = new TextObject("{=Dramalord404}{HERO1.LINK} confessed to {HERO2.LINK} that their child {HERO3.LINK} is from {HERO4.LINK}");
                }
            }
            else if (memoryType == MemoryType.Gossip)
            {
                if (eventType == EventType.Date)
                {
                    tobj = new TextObject("{=Dramalord350}{HERO1.LINK} told gossip to {HERO2.LINK} about a date of {HERO2.LINK} and {HERO4.LINK}");
                }
                else if (eventType == EventType.Intercourse)
                {
                    tobj = new TextObject("{=Dramalord351}{HERO1.LINK} told gossip to {HERO2.LINK} about {HERO2.LINK} and {HERO4.LINK} having intercourse");
                }
                else if (eventType == EventType.Marriage)
                {
                    tobj = new TextObject("{=Dramalord352}{HERO1.LINK} told gossip to {HERO2.LINK} about the marriage of {HERO3.LINK} and {HERO4.LINK}");
                }
                else if (eventType == EventType.Pregnancy)
                {
                    tobj = new TextObject("{=Dramalord405}{HERO1.LINK} told gossip to {HERO2.LINK} about {HERO3.LINK} being pregnant from {HERO4.LINK}");
                }
                else if (eventType == EventType.Birth)
                {
                    tobj = new TextObject("{=Dramalord406}{HERO1.LINK} told gossip to {HERO2.LINK} that {HERO3.LINK}' child is from {HERO4.LINK}");
                }
            }
            else if (memoryType == MemoryType.Witness)
            {
                if (eventType == EventType.Date)
                {
                    tobj = new TextObject("{=Dramalord353}{HERO1.LINK} told {HERO2.LINK} that they witnessed a date of {HERO2.LINK} and {HERO4.LINK}");
                }
                else if (eventType == EventType.Intercourse)
                {
                    tobj = new TextObject("{=Dramalord354}{HERO1.LINK} told {HERO2.LINK} that they witnessed {HERO2.LINK} and {HERO4.LINK} having intercourse");
                }
                else if (eventType == EventType.Marriage)
                {
                    tobj = new TextObject("{=Dramalord355}{HERO1.LINK} told {HERO2.LINK} that they witnessed the marriage of {HERO2.LINK} and {HERO4.LINK}");
                }
                else if (eventType == EventType.Pregnancy)
                {
                    tobj = new TextObject("{=Dramalord407}{HERO1.LINK} told {HERO2.LINK} that they witnessed {HERO2.LINK} being pregnant from {HERO4.LINK}");
                }
                else if (eventType == EventType.Birth)
                {
                    tobj = new TextObject("{=Dramalord408}{HERO1.LINK} told {HERO2.LINK} that they witnessed that {HERO2.LINK}'s child is from {HERO4.LINK}");
                }
            }

            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, tobj);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, tobj);
            StringHelpers.SetCharacterProperties("HERO3", Hero3.CharacterObject, tobj);
            StringHelpers.SetCharacterProperties("HERO4", Hero3.CharacterObject, tobj);
            return tobj;
        }
    }

    public class EncyclopediaLogStartFWB : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26081)]
        public readonly Hero Hero1;

        [SaveableField(26082)]
        public readonly Hero Hero2;

        public EncyclopediaLogStartFWB(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord359}{HERO1.LINK} and {HERO2.LINK} started being friends with benefits.");
            StringHelpers.SetCharacterProperties("HERO1", Hero1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("HERO2", Hero2.CharacterObject, textObject);
            return textObject;
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase
        {
            return obj == Hero1 || obj == Hero2;
        }
    }
}
