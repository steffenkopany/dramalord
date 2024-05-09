using Dramalord.Actions;
using Dramalord.Behaviors;
using Dramalord.Data;
using Dramalord.Quests;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Dramalord.UI
{
    internal static class Consequences
    {
        static Hero? Player = null;
        static Hero? Npc = null;
        static Hero? PlayerSpouse = null;
        static Hero? NpcSpouse = null;

        // FLIRT
        internal static void NpcAcceptedFlirt()
        {
            SetRoles();

            HeroFlirtAction.Apply(Npc, Player);

            List<Hero> flirts = new();
            List<Hero> partners = new();
            List<Hero> prisoners = new();

            AICampaignActions.ScopeSurroundings(Npc, ref flirts, ref partners, ref prisoners, true);

            partners.Remove(Player);
            Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

            if (partner != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
            {
                HeroWitnessAction.Apply(Npc, Player, partner, WitnessType.Flirting);
            }

            partners.Clear();

            AICampaignActions.ScopeSurroundings(Player, ref flirts, ref partners, ref prisoners, true);

            partners.Remove(Npc);
            Hero? partner2 = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

            if (partner2 != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
            {
                HeroWitnessAction.Apply(Player, Npc, partner2, WitnessType.Flirting);
            }

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcDeclinedFlirt()
        {
            SetRoles();
            Info.SetLastDaySeen(Npc, Player, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Npc, Player, DramalordMCM.Get.EmotionalLossCaughtFlirting * -1);
        }

        //DATE

        internal static void NpcAcceptedDate()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            if (!Info.IsCoupleWithHero(Player, Npc))
            {
                Info.SetIsCoupleWithHero(Player, Npc, true);
                TextObject banner = new TextObject("{=Dramalord258}You and {HERO.LINK} are now a couple.");
                StringHelpers.SetCharacterProperties("HERO", Npc.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, Npc.CharacterObject, "event:/ui/notification/relation");
            }

            List<Hero> flirts = new();
            List<Hero> partners = new();
            List<Hero> prisoners = new();

            AICampaignActions.ScopeSurroundings(Player, ref flirts, ref partners, ref prisoners, true);
            partners.Remove(Npc);
            Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;
            AICampaignActions.CompleteDateActions(Player, Npc, partner);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasConsideringDate()
        {
            SetRoles();
            Persuasions.CreatePersuasionTaskForDate();
        }

        internal static void NpcDeclinedDate()
        {
            SetRoles();
            Info.SetLastPrivateMeeting(Npc, Player, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Npc, Player, DramalordMCM.Get.EmotionalLossCaughtDate * -1);
            Persuasions.ClearCurrentPersuasion();
        }


        //DIVORCE

        internal static void NpcAcceptedDivorce()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();

            HeroDivorceAction.Apply(Npc, NpcSpouse);
            Info.SetLastPrivateMeeting(Npc, Player, CampaignTime.Now.ToDays);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasConsideringDivorce()
        {
            SetRoles();
            Persuasions.CreatePersuasionTaskForDivorce();
        }

        internal static void NpcDeclinedDivorce()
        {
            SetRoles();
            Persuasions.ClearCurrentPersuasion();
            Info.SetLastPrivateMeeting(Npc, Player, CampaignTime.Now.ToDays);
        }

        //MARRIAGE

        internal static void NpcAcceptedMarriage()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            Info.SetLastPrivateMeeting(Npc, Player, CampaignTime.Now.ToDays);
            HeroMarriageAction.Apply(Player, Npc);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasConsideringMarriage()
        {
            SetRoles();
            Persuasions.CreatePersuasionTaskForMarriage();
        }

        internal static void NpcDeclinedMarriage()
        {
            SetRoles();
            Persuasions.ClearCurrentPersuasion();
            Info.SetLastPrivateMeeting(Npc, Player, CampaignTime.Now.ToDays);
            Info.ChangeEmotionToHeroBy(Npc, Player, DramalordMCM.Get.EmotionalLossMarryOther * -1);
        }

        internal static void NpcGotPresentFromPlayer()
        {
            SetRoles();

            TextObject toy = TextObject.Empty;
            if (Npc.IsFemale)
            {
                ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
                Player.PartyBelongedTo.ItemRoster.AddToCounts(wurst, -1);
                Info.SetHeroHasToy(Npc, true);
                toy = new TextObject("{=Dramalord052}sausage");
            }
            else
            {
                ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
                Player.PartyBelongedTo.ItemRoster.AddToCounts(pie, -1);
                Info.SetHeroHasToy(Npc, true);
                toy = new TextObject("{=Dramalord101}Pie");
            }

            TextObject banner = new TextObject("{=Dramalord258}You gave {HERO.LINK} a {TOY}.");
            StringHelpers.SetCharacterProperties("HERO", Npc.CharacterObject, banner);
            banner.SetTextVariable("TOY", toy);
            MBInformationManager.AddQuickInformation(banner, 1000, Npc.CharacterObject, "event:/ui/notification/relation");
        }

        internal static void NpcDidntCareAboutBreakup()
        {
            SetRoles();
            Info.SetIsCoupleWithHero(Npc, Player, false);
        }

        internal static void NpcWasSurprisedByBreakup()
        {
            SetRoles();
            HeroBreakupAction.Apply(Player, Npc);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasHeartBrokenByBreakup()
        {
            SetRoles();
            HeroBreakupAction.Apply(Player, Npc);

            if (Npc.Clan != null && Npc.Clan == Player.Clan && DramalordMCM.Get.AllowClanChanges)
            {
                HeroLeaveClanAction.Apply(Npc, Npc);
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcGotSuicidalByBreakup()
        {
            SetRoles();
            HeroBreakupAction.Apply(Player, Npc);
            if(DramalordMCM.Get.AllowRageKills)
            {
                HeroKillAction.Apply(Npc, Npc, Player, KillReason.Suicide);
            }
            
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcDidntCareAboutDivorce()
        {
            SetRoles();
            HeroDivorceAction.Apply(Player, Npc);
        }

        internal static void NpcWasSurprisedByDivorce()
        {
            SetRoles();
            HeroDivorceAction.Apply(Player, Npc);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasHeartBrokenByDivorce()
        {
            SetRoles();
            HeroDivorceAction.Apply(Player, Npc);

            if (Npc.Clan != null && Npc.Clan == Player.Clan && DramalordMCM.Get.AllowClanChanges)
            {
                HeroLeaveClanAction.Apply(Npc, Npc);
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcGotSuicidalByDivorce()
        {
            SetRoles();
            HeroDivorceAction.Apply(Player, Npc);
            if (DramalordMCM.Get.AllowRageKills)
            {
                HeroKillAction.Apply(Npc, Npc, Player, KillReason.Suicide);
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void PlayerViolatedNpc()
        {
            SetRoles();
            if (Info.ValidateHeroMemory(Player, Npc))
            {
                HeroIntercourseAction.Apply(Player, Npc, true);

                Hero mother = Player.IsFemale ? Player : Npc;
                Hero father = Npc.IsFemale ? Player : Npc;

                if (mother != father && !mother.IsPregnant && Info.CanGetPregnant(mother) && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
                {
                    HeroConceiveAction.Apply(Player, Npc, true);
                }
            }
        }

        internal static void PlayerAcceptedFlirt()
        {
            SetRoles();

            HeroFlirtAction.Apply(Player, Npc);

            List<Hero> flirts = new();
            List<Hero> partners = new();
            List<Hero> prisoners = new();

            AICampaignActions.ScopeSurroundings(Player, ref flirts, ref partners, ref prisoners, true);

            partners.Remove(Npc);
            Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

            if (partner != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
            {
                HeroWitnessAction.Apply(Player, Npc, partner, WitnessType.Flirting);
            }

            partners.Clear();

            AICampaignActions.ScopeSurroundings(Npc, ref flirts, ref partners, ref prisoners, true);

            partners.Remove(Player);
            Hero? partner2 = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

            if (partner2 != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
            {
                HeroWitnessAction.Apply(Npc, Player, partner2, WitnessType.Flirting);
            }

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void PlayerAcceptedDate()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            if (!Info.IsCoupleWithHero(Player, Npc))
            {
                Info.SetIsCoupleWithHero(Player, Npc, true);
                TextObject banner = new TextObject("{=Dramalord258}You and {HERO.LINK} are now a couple.");
                StringHelpers.SetCharacterProperties("HERO", Npc.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, Npc.CharacterObject, "event:/ui/notification/relation");
            }

            List<Hero> flirts = new();
            List<Hero> partners = new();
            List<Hero> prisoners = new();

            AICampaignActions.ScopeSurroundings(Player, ref flirts, ref partners, ref prisoners, true);
            partners.Remove(Npc);
            Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;
            AICampaignActions.CompleteDateActions(Player, Npc, partner);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcBrokeUpWithPlayer()
        {
            SetRoles();
            HeroBreakupAction.Apply(Npc, Player);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcDivorcedPlayer()
        {
            SetRoles();
            HeroDivorceAction.Apply(Npc, Player);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        //ACCUSATIONS
        internal static void PlayerKillsNpc()
        {
            SetRoles();
            KillReason reason = KillReason.Intercourse;
            if(Conditions.WitnessOf == WitnessType.Pregnancy)
            {
                reason = KillReason.Pregnancy;
            }
            else if (Conditions.WitnessOf == WitnessType.Bastard)
            {
                reason = KillReason.Bastard;
            }
            HeroKillAction.Apply(Player, Npc, Conditions.LoverOrChild, reason);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void PlayerKicksNpcOut()
        {
            SetRoles();
            HeroLeaveClanAction.Apply(Npc, Player);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void PlayerBreaksUpWithNpc()
        {
            SetRoles();
            if(Npc.Spouse == Player)
            {
                HeroDivorceAction.Apply(Player, Npc);
            }
            else
            {
                HeroBreakupAction.Apply(Player, Npc);
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        //QUEST
        internal static void VisitQuestFail()
        {
            SetRoles();
            if(VisitLoverQuest.HeroList.ContainsKey(Npc))
            {
                VisitLoverQuest.HeroList[Npc].QuestFail();
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void VisitQuestSuccess()
        {
            SetRoles();
            if (VisitLoverQuest.HeroList.ContainsKey(Npc))
            {
                VisitLoverQuest.HeroList[Npc].QuestSuccess();
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void SetRoles()
        {
            Player = Hero.MainHero;
            Npc = Hero.OneToOneConversationHero;
            PlayerSpouse = Hero.MainHero.Spouse;
            NpcSpouse = Hero.OneToOneConversationHero.Spouse;
        }
    }
}
