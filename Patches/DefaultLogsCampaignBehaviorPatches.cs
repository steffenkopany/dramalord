﻿using HarmonyLib;
using SandBox.GauntletUI.AutoGenerated0;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine;
using TaleWorlds.Library;


namespace Dramalord.Patches
{
    [HarmonyPatch(typeof(DefaultLogsCampaignBehavior), "OnPrisonerTaken", new Type[] { typeof(PartyBase), typeof(Hero) })]
    public static class OnPrisonerTakenPatch
    {
        public static bool Prefix(ref PartyBase party, Hero hero)
        {
            if(DramalordMCM.Get.NoCaptivityMessages)
            {
                LogEntry.AddLogEntry(new DramalordTakePrisonerLogEntry(party, hero));
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DefaultLogsCampaignBehavior), "OnHeroPrisonerReleased", new Type[] { typeof(Hero), typeof(PartyBase), typeof(IFaction), typeof(EndCaptivityDetail) })]
    public static class OnHeroPrisonerReleasedPatch
    {
        public static bool Prefix(ref Hero hero, ref PartyBase party, ref IFaction captuererFaction, ref EndCaptivityDetail detail)
        {
            if (DramalordMCM.Get.NoCaptivityMessages)
            {
                LogEntry.AddLogEntry(new DramalordEndCaptivityLogEntry(hero, captuererFaction, detail));
                return false;
            }
            return true;
        }
    }

}
