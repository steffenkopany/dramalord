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
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class BirthAction
    {
        internal static void Apply(Hero mother, HeroPregnancy pregnancy)
        {
            Hero father = pregnancy.Father;

            if (DramalordMCM.Instance.BirthDebugOutput)
            {
                InformationManager.DisplayMessage(new InformationMessage($"[Dramalord birth] Mother:{mother.Name}, Father:{father.Name}", new Color(1f, 0.08f, 0.58f)));
            }

            Hero child = CreateBaby(mother, father);
            child.UpdateHomeSettlement();
            mother.IsPregnant = false;

            if (father.Clan == Clan.PlayerClan || mother.Clan == Clan.PlayerClan)
            {
                TextObject textObject = new TextObject("{=Dramalord077}{HERO.LINK} was born");
                StringHelpers.SetCharacterProperties("HERO", child.CharacterObject, textObject);

                MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(father, mother, CampaignTime.Now));
                MBInformationManager.AddNotice(new ChildBornMapNotification(child, textObject, CampaignTime.Now));
            }

            DramalordPregancies.Instance.RemovePregnancy(mother);
            DramalordEvents.Instance.RemoveEvent(pregnancy.EventId);

            int eventID = DramalordEvents.Instance.AddEvent(mother, child, EventType.Birth, 5);
            mother.GetCloseHeroes().ForEach(member => 
            { 
                if(member.IsEmotionalWith(mother) && member != child.Father)
                {
                    member.AddIntention(mother, IntentionType.Confrontation, eventID);
                } 
            });

            if((mother.Spouse == null || (mother.Spouse != father && mother.GetRelationTo(father).Relationship != RelationshipType.Spouse)) && (father == Hero.MainHero || mother.Clan == Clan.PlayerClan || father.Clan == Clan.PlayerClan))
            {
                TextObject title = new TextObject("{=Dramalord524}{CHILD} was born.");
                TextObject text = new TextObject("{=Dramalord525}{MOTHER} has given birth to {CHILD}. {FATHER} is the father. Would urge the mother to keep the child or get rid of them?");
                title.SetTextVariable("CHILD", child.Name);
                text.SetTextVariable("MOTHER", mother.Name);
                text.SetTextVariable("CHILD", child.Name);
                text.SetTextVariable("FATHER", father.Name);
                InformationManager.ShowInquiry(
                        new InquiryData(
                            title.ToString(),
                            text.ToString(),
                            true,
                            true,
                            new TextObject("{=Dramalord526}Get rid of the child").ToString(),
                            new TextObject("{=Dramalord527}Keep the child").ToString(),
                            () => { mother.AddIntention(child, IntentionType.Orphanize, eventID); },
                            () => { return; }), true);
            }
            else if((mother.Spouse == null || (mother.Spouse != father && mother.GetRelationTo(father).Relationship != RelationshipType.Spouse)) && ((mother.Clan != null && mother.Clan != Clan.PlayerClan && !DramalordMCM.Instance.KeepClanChildren) || (mother.Clan == null && !DramalordMCM.Instance.KeepNotableChildren)))
            {
                mother.AddIntention(child, IntentionType.Orphanize, eventID);
            }
            else if (child.Clan == Clan.PlayerClan && child.Occupation == Occupation.Wanderer)
            {
                if (father == Hero.MainHero || mother == Hero.MainHero)
                {
                    child.SetNewOccupation(Occupation.Lord);
                    Clan.PlayerClan.Companions.Remove(child);
                }
                else if (Clan.PlayerClan.Companions.Count >= Campaign.Current.Models.ClanTierModel.GetCompanionLimit(Clan.PlayerClan))
                {
                    child.SetNewOccupation(Occupation.Lord);
                    Clan.PlayerClan.Companions.Remove(child);
                    mother.AddIntention(child, IntentionType.Orphanize, eventID);
                }
            }

            if ((mother.Clan == Clan.PlayerClan || father.Clan == Clan.PlayerClan) || !DramalordMCM.Instance.ShowOnlyClanInteractions)
            {
                LogEntry.AddLogEntry(new BirthChildLog(mother, father, child));
            }
        }

        private static Hero CreateBaby(Hero mother, Hero father)
        {
            CharacterObject? template = null;
            if (mother.IsLord && father.IsLord)
            {
                template = (MBRandom.RandomInt(1, 100) > 50) ? mother.CharacterObject : father.CharacterObject;
            }
            else
            {
                template = mother.IsLord ? mother.CharacterObject : father.IsLord ? father.CharacterObject : Hero.AllAliveHeroes.GetRandomElementWithPredicate(h => h.IsLord && h.Clan != Clan.PlayerClan).CharacterObject;
            }

            Settlement bornSettlement = mother.CurrentSettlement ?? father.HomeSettlement ?? SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);

            Clan? faction = mother.Clan;
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

            if(child.Occupation == Occupation.Lord)
            {
                child.SetName(child.FirstName, child.FirstName);
            }

            return child;
        }
    }
}
