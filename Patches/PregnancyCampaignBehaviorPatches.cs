using Dramalord.Actions;
using Dramalord.Data;
using HarmonyLib;
using Helpers;
using JetBrains.Annotations;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Patches
{

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "RefreshSpouseVisit", new Type[] { typeof(Hero) })]
    public static class RefreshSpouseVisitPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool RefreshSpouseVisit(ref Hero hero)
        {
            if(!DramalordMCM.Get.AllowDefaultPregnancies)
            {
                return false;
            }

            Settlement? heroSettlement = hero.CurrentSettlement;
            MobileParty? heroParty = hero.PartyBelongedTo;

            Settlement? heroSettlement2 = hero.Spouse?.CurrentSettlement;
            MobileParty? heroParty2 = hero.Spouse?.PartyBelongedTo;

            if (heroParty?.AttachedTo != null)
            {
                heroParty = heroParty.AttachedTo;
            }

            if (heroParty2?.AttachedTo != null)
            {
                heroParty2 = heroParty2.AttachedTo;
            }

            if (heroSettlement == null)
            {
                heroSettlement = heroParty?.CurrentSettlement;
            }
            if (heroSettlement2 == null)
            {
                heroSettlement2 = heroParty2?.CurrentSettlement;
            }

            if (((heroSettlement != null && heroSettlement == heroSettlement2) || (heroParty != null && heroParty == heroParty2)) && MBRandom.RandomInt(1,100) <= DramalordMCM.Get.PregnancyChance)
            {
                ChildConceivedPatch.Father = hero.Spouse?.CharacterObject;
                //MakePregnantAction.Apply(hero);
                ChildConceivedPatch.ChildConceived(ref hero);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "CheckOffspringsToDeliver", new Type[] { typeof(Hero) })]
    public static class CheckOffspringsToDeliverPatch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static void CheckOffspringsToDeliver(ref Hero hero)
        {
            HeroPregnancy? pregnancy = hero.GetDramalordPregnancy();
            if(pregnancy == null)
            {
                hero.IsPregnant = false;
            }
            else if(CampaignTime.Now.ToDays - pregnancy.Conceived > DramalordMCM.Get.PregnancyDuration)
            {
                HeroBirthAction.Apply(hero, pregnancy, hero.GetCloseHeroes());
            }
        }
    }

    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "ChildConceived", new Type[] { typeof(Hero) })]
    public static class ChildConceivedPatch
    {
        internal static CharacterObject? Father;

        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool ChildConceived(ref Hero mother)
        {
            CharacterObject? fatherChar = Father ?? mother.Spouse?.CharacterObject;
            if(fatherChar != null && fatherChar.IsHero)
            {
                mother.MakeDramalordPregnant(fatherChar.HeroObject);
            
                if (mother == Hero.MainHero)
                {
                    TextObject banner = new TextObject("{=Dramalord143}You got pregnant from {HERO.LINK}");
                    StringHelpers.SetCharacterProperties("HERO", fatherChar, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, fatherChar, "event:/ui/notification/relation");
                }
                else if (fatherChar.HeroObject == Hero.MainHero)
                {
                    TextObject banner = new TextObject("{=Dramalord144}{HERO.LINK} got pregnant from you.");
                    StringHelpers.SetCharacterProperties("HERO", mother.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 1000, mother.CharacterObject, "event:/ui/notification/relation");
                }

                if (DramalordMCM.Get.AffairOutput)
                {
                    LogEntry.AddLogEntry(new EncyclopediaLogConceived(mother, fatherChar.HeroObject));
                }

                DramalordEventCallbacks.OnHeroesConceive(mother, fatherChar.HeroObject);
            }

            return false;
        }
    }   
}
