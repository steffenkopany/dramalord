using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.LogItems;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class BirthAction
    {
        internal static void Apply(Hero mother, HeroPregnancy pregnancy)
        {
            Hero father = pregnancy.Father;
            Hero child = CreateBaby(mother, father);
            child.UpdateHomeSettlement();

            if (father.Clan == Clan.PlayerClan || mother.Clan == Clan.PlayerClan)
            {
                TextObject textObject = new TextObject("{=Dramalord077}{HERO.LINK} was born");
                StringHelpers.SetCharacterProperties("HERO", child.CharacterObject, textObject);

                MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(father, mother, CampaignTime.Now)); //TaleWorlds.CampaignSystem.MapNotificationTypes.ChildBornMapNotification.
                MBInformationManager.AddNotice(new ChildBornMapNotification(child, textObject, CampaignTime.Now));
            }

            DramalordPregancies.Instance.RemovePregnancy(mother);
            DramalordEvents.Instance.RemoveEvent(pregnancy.EventId);

            int eventID = DramalordEvents.Instance.AddEvent(mother, child, EventType.Birth, 5);
            mother.GetCloseHeroes().ForEach(member => 
            { 
                if(member.IsEmotionalWith(mother) && member != child.Father)
                {
                    DramalordIntentions.Instance.AddIntention(member, mother, IntentionType.Confrontation, eventID);
                } 
            });

            if((mother.Clan == Clan.PlayerClan || father.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new BirthChildLog(mother, father, child));
            }
        }

        private static Hero CreateBaby(Hero mother, Hero father)
        {
            CharacterObject template = (MBRandom.RandomInt(1, 100) > 50) ? mother.CharacterObject : father.CharacterObject;
            Settlement bornSettlement = mother.CurrentSettlement ?? father.HomeSettlement ?? SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);

            Clan faction = mother.Clan;
            Hero child = HeroCreator.CreateSpecialHero(template, bornSettlement, faction, null, 0);
            child.Mother = mother;
            child.Father = father;
            child.HeroDeveloper.InitializeHeroDeveloper(isByNaturalGrowth: true);
            BodyProperties bodyProperties = mother.BodyProperties;
            BodyProperties bodyProperties2 = father.BodyProperties;
            int seed = MBRandom.RandomInt();
            string hairTags = (child.IsFemale ? mother.HairTags : father.HairTags);
            string tattooTags = (child.IsFemale ? mother.TattooTags : father.TattooTags);
            child.ModifyPlayersFamilyAppearance(BodyProperties.GetRandomBodyProperties(template.Race, child.IsFemale, bodyProperties, bodyProperties2, 1, seed, hairTags, father.BeardTags, tattooTags).StaticProperties);

            child.SetNewOccupation(mother.Occupation);
            if(mother.Occupation == Occupation.Lord)
            {
                child.SetName(child.FirstName, child.FirstName);
            }

            return child;
        }
    }
}
