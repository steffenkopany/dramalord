using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using HarmonyLib;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class BreakupAction
    {
        internal static void Apply(Hero hero, Hero target)
        {
            HeroRelation relation = hero.GetRelationTo(target);

            relation.UpdateLove();
            RelationshipType oldRelationship = relation.Relationship;
            relation.Relationship = RelationshipType.None;
            relation.Love = (relation.CurrentLove > 0) ? 0 : relation.CurrentLove;
            relation.Trust = (relation.Trust > 0) ? 0 : relation.Trust;

            if(hero.Spouse == target)
            {
                foreach (Romance.RomanticState romanticState in Romance.RomanticStateList.ToList())
                {
                    if ((romanticState.Person1 == target && romanticState.Person2 == hero) || (romanticState.Person1 == hero && romanticState.Person2 == target))
                    {
                        romanticState.Level = Romance.RomanceLevelEnum.FailedInPracticalities;
                    }
                }
                hero.Spouse = null;
                target.Spouse = null;
            }

            List<HeroIntention> heroIntentions = hero.GetIntentions().ToList();
            heroIntentions.Where(intention => intention.Target == target).Do(intention => DramalordIntentions.Instance.RemoveIntention(hero, intention.Target, intention.Type, intention.EventId));

            List<HeroIntention> targetIntentions = target.GetIntentions().ToList();
            targetIntentions.Where(intention => intention.Target == hero).Do(intention => DramalordIntentions.Instance.RemoveIntention(target, intention.Target, intention.Type, intention.EventId));

            if (hero == Hero.MainHero)
            {
                if(oldRelationship == RelationshipType.Friend || oldRelationship == RelationshipType.FriendWithBenefits)
                {
                    TextObject textObject = new TextObject("{=Dramalord203}You ended your friendship with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", target.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, target.CharacterObject, "event:/ui/notification/relation");
                }
                else
                {
                    TextObject textObject = new TextObject("{=Dramalord088}You ended your relationship with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", target.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, target.CharacterObject, "event:/ui/notification/relation");
                }
            }
            else if (target == Hero.MainHero)
            {
                if (oldRelationship == RelationshipType.Friend || oldRelationship == RelationshipType.FriendWithBenefits)
                {
                    TextObject textObject = new TextObject("{=Dramalord204}{HERO.LINK} ended their friendship with you.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, hero.CharacterObject, "event:/ui/notification/relation");
                }
                else
                {
                    TextObject textObject = new TextObject("{=Dramalord089}{HERO.LINK} ended their relationship with you.");
                    StringHelpers.SetCharacterProperties("HERO", hero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, hero.CharacterObject, "event:/ui/notification/relation");
                }
            }

            if ((hero.Clan == Clan.PlayerClan || target.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new EndRelationshipLog(hero, target, oldRelationship));
            }
        }
    }
}
