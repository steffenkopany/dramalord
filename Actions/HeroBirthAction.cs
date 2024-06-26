﻿using Dramalord.Data;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Actions
{
    internal static class HeroBirthAction
    {
        internal static void Apply(Hero mother, HeroPregnancy pregnancy, List<Hero> closeHeroes)
        {
            PregnancyModel pregnancyModel = Campaign.Current.Models.PregnancyModel;
            Hero father = pregnancy.Father.HeroObject;

            /*
            if (!(MBRandom.RandomFloat > pregnancyModel.StillbirthProbability))
            {
                TextObject textObject = new TextObject("{=pw4cUPEn}{MOTHER.LINK} has delivered stillborn.");
                StringHelpers.SetCharacterProperties("MOTHER", mother.CharacterObject, textObject);
                InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
            }
            */
            Hero child = createBaby(mother, father);
            child.UpdateHomeSettlement();

            if (father == Hero.MainHero || mother == Hero.MainHero)
            {
                MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(father, mother, CampaignTime.Now)); //TaleWorlds.CampaignSystem.MapNotificationTypes.ChildBornMapNotification.
                MBInformationManager.AddNotice(new ChildBornMapNotification(child, new TextObject("A child was born"), CampaignTime.Now));
            }

            int eventID = DramalordEvents.AddHeroEvent(mother, child, EventType.Birth, DramalordMCM.Get.BirthMemoryDuration);
            mother.AddDramalordMemory(eventID, MemoryType.Participant, mother, true);

            DramalordEvents.RemoveHeroEvent(pregnancy.EventID);
            mother.ClearDramalordPregnancy();
            mother.IsPregnant = false;

            if (mother != Hero.MainHero && MBRandom.RandomFloat <= pregnancyModel.MaternalMortalityProbabilityInLabor && mother.Clan != Clan.PlayerClan)
            {
                KillCharacterAction.ApplyInLabor(mother);
            }

            if(DramalordMCM.Get.BirthOutput && (mother.Clan == Clan.PlayerClan || father.Clan == Clan.PlayerClan || !DramalordMCM.Get.OnlyPlayerClanOutput))
            {
                LogEntry.AddLogEntry(new EncyclopediaLogBirth(mother, father, child));
            }
            
            DramalordEventCallbacks.OnHeroesBorn(mother, father, child);

            if (MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
            {
                Hero? witness = closeHeroes.Where(item => item != father && (item.IsLover(mother) || item.IsSpouse(mother))).GetRandomElementInefficiently();
                if (witness != null)
                {
                    witness.AddDramalordMemory(eventID, MemoryType.Witness, witness, true);

                    if(DramalordMCM.Get.BirthOutput && (father.Clan == Clan.PlayerClan || mother.Clan == Clan.PlayerClan || !DramalordMCM.Get.OnlyPlayerClanOutput))
                    {
                        LogEntry.AddLogEntry(new LogWitnessBastard(mother, child, witness));
                    }
                    

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord272}You noticed that {TARGET.LINK} born by {HERO.LINK} is not your child");
                        StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", child.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, child.CharacterObject, "event:/ui/notification/relation");
                    }
                    else if (mother == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord273}{HERO.LINK} noticed your child {TARGET.LINK} is not theirs.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", child.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, witness.CharacterObject, "event:/ui/notification/relation");
                    }

                    if(!witness.GetDramalordPersonality().AcceptsOtherChildren)
                    {
                        int emotionChange = witness.GetDramalordPersonality().GetEmotionalChange(EventType.Birth);
                        HeroFeelings witnessFeelings = witness.GetDramalordFeelings(mother);
                        witnessFeelings.Emotion += emotionChange;
                        //witnessFeelings.Emotion -= DramalordMCM.Get.EmotionalLossBastard; 
                        if (DramalordMCM.Get.LinkEmotionToRelation)
                        {
                            witness.ChangeRelationTo(mother, (emotionChange / 2));
                            //witness.ChangeRelationTo(mother, (DramalordMCM.Get.EmotionalLossBastard / 2) * -1);
                        }

                        witness.MakeAngryWith(mother, DramalordMCM.Get.AngerDaysBastard);
                    }
                }
            }

            if(!mother.GetHeroSpouses().Contains(father.CharacterObject) && (mother.Clan != Clan.PlayerClan || father != Hero.MainHero))
            {
                HeroPutInOrphanageAction.Apply(mother, child);
            }
        }

        private static Hero createBaby(Hero mother, Hero father)
        {
            CharacterObject template = (MBRandom.RandomInt(1, 100) > 50) ? mother.CharacterObject : father.CharacterObject;
            Settlement bornSettlement = (mother.CurrentSettlement != null) ? mother.CurrentSettlement : father.HomeSettlement;
            if (bornSettlement == null)
            {
                bornSettlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
            }
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

            return child;
        }
    }
}
