using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class EngagementAction
    {
        internal static bool Apply(Hero hero, Hero target, List<Hero> closeHeroes)
        {
            hero.GetRelationTo(target).Relationship = RelationshipType.Betrothed;

            if (hero == Hero.MainHero || target == Hero.MainHero)
            {
                Hero otherHero = (hero == Hero.MainHero) ? target : hero;
                TextObject banner = new TextObject("{=Dramalord078}You are engaged to {HERO.LINK}.");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            int eventID = DramalordEvents.Instance.AddEvent(hero, target, EventType.Betrothed, 5);
          

            hero.ExSpouses.Remove(target);
            target.ExSpouses.Remove(hero);
            var ex1 = hero.ExSpouses.Distinct().Where(h => h != null).ToList();
            hero.ExSpouses.Clear();
            hero.ExSpouses.AddRange(ex1);
            var ex2 = target.ExSpouses.Distinct().Where(h => h != null).ToList();
            target.ExSpouses.Clear();
            target.ExSpouses.AddRange(ex2);

            Hero? witness = null;
            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
            {
                witness = closeHeroes.GetRandomElementWithPredicate(h => h != hero && h != target);
                if (DramalordMCM.Instance.PlayerAlwaysWitness && hero != Hero.MainHero && target != Hero.MainHero && witness != Hero.MainHero && closeHeroes.Contains(Hero.MainHero))
                {
                    witness = Hero.MainHero;
                }
                if (witness != null)
                {
                    if (witness.IsEmotionalWith(hero) || witness.IsEmotionalWith(target))
                    {
                        DramalordIntentions.Instance.RemoveIntentionsTo(witness, hero);
                        DramalordIntentions.Instance.RemoveIntentionsTo(witness, target);
                        DramalordIntentions.Instance.AddIntention(witness, hero, IntentionType.Confrontation, eventID);
                        DramalordIntentions.Instance.AddIntention(witness, target, IntentionType.Confrontation, eventID);
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord215}You saw {HERO.LINK} proposing to {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, hero.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (hero == Hero.MainHero || target == Hero.MainHero)
                    {
                        Hero other = (hero == Hero.MainHero) ? target : hero;
                        TextObject banner = new TextObject("{=Dramalord216}{HERO.LINK} saw you proposing to {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", other.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
            }

            if ((hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new StartRelationshipLog(hero, target, RelationshipType.Betrothed));
            }

            return true;
        }
    }
}
