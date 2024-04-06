using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
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

    public class EncyclopediaLogMarriage : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26004)]
        public readonly Hero Hero1;

        [SaveableField(26005)]
        public readonly Hero Hero2;

        public EncyclopediaLogMarriage(Hero hero1, Hero hero2)
        {
            Hero1 = hero1;
            Hero2 = hero2;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.MarriageOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord113}{HERO1.LINK} married {HERO2.LINK}");
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
            TextObject textObject = new TextObject("{=Dramalord068}{KILLER.LINK} killed {VICTIM.LINK} after noticing she is pregnant from someone else.");
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

        public LogIntercourse(Hero hero1, Hero hero2, bool byForce)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            ByForce = byForce;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => Hero1 == Hero.MainHero || Hero2 == Hero.MainHero ? ChatNotificationType.PlayerClanPositive : ChatNotificationType.Civilian;

        public TextObject GetNotificationText()
        {
            TextObject textObject = TextObject.Empty;
            if (ByForce)
            {
                textObject = new TextObject("{=Dramalord116}{HERO1.LINK} forced themself on {HERO2.LINK}");
            }
            else
            {
                textObject = new TextObject("{=Dramalord114}{HERO1.LINK} had intercourse with {HERO2.LINK}");
            }

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

        public EncyclopediaLogConceived(Hero hero1, Hero hero2, bool byForce)
        {
            Hero1 = hero1;
            Hero2 = hero2;
            ByForce = byForce;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.AffairOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = TextObject.Empty;
            if (ByForce)
            {
                textObject = new TextObject("{=Dramalord117}{HERO1.LINK} was forcefully impregnated by {HERO2.LINK}");
            }
            else
            {
                textObject = new TextObject("{=Dramalord115}{HERO1.LINK} was impregnated by {HERO2.LINK}");
            }

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
            TextObject textObject = TextObject.Empty;
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
            TextObject textObject = new TextObject("{=Dramalord129}{HERO.LINK} played with their toy.");
            StringHelpers.SetCharacterProperties("HERO", Hero1.CharacterObject, textObject);
            return textObject;
        }
    }

    public class EncyclopediaLogKilledSuizide : LogEntry, IEncyclopediaLog, IChatNotification
    {
        [SaveableField(26054)]
        public readonly Hero Hero;

        public EncyclopediaLogKilledSuizide(Hero victim)
        {
            Hero = victim;
        }

        public bool IsVisibleNotification => DramalordMCM.Get.DeathOutput;
        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerFactionIndirectNegative;

        public TextObject GetEncyclopediaText()
        {
            TextObject textObject = new TextObject("{=Dramalord146}{HERO.LINK} commited suizide.");
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
}
