using Dramalord.Actions;
using Dramalord.Behaviors;
using Dramalord.Data;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
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

            HeroFlirtAction.Apply(Player, Npc);
            
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcDeclinedFlirt()
        {
            SetRoles();
            Info.SetLastDaySeen(Npc, Player, CampaignTime.Now.ToDays);
        }

        //DATE

        internal static void NpcAcceptedDate()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            if (!Info.IsCoupleWithHero(Player, Npc))
            {
                Info.SetIsCoupleWithHero(Player, Npc, true);
                //Info.ChangeEmotionToHeroBy(Npc, Player, DramalordMCM.Get.EmotionalWinAffair);
                Notification.DrawBanner("You and " + Npc + " are now a couple");
            }

            List<Hero> flirts = new();
            List<Hero> partners = new();
            List<Hero> prisoners = new();

            AICampaignActions.ScopeSurroundings(Npc, ref flirts, ref partners, ref prisoners);
            partners.Remove(Hero.MainHero);
            Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;
            AICampaignActions.CompleteDateActions(Npc, Player, partner);

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
        }

        internal static void NpcGotPresentFromPlayer()
        {
            SetRoles();
            
            if (Npc.IsFemale)
            {
                ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
                Player.PartyBelongedTo.ItemRoster.AddToCounts(wurst, -1);
                Info.SetHeroHasToy(Npc, true);

                Notification.DrawBanner("You gave " + Npc + " a " + wurst.Name);
            }
            else
            {
                ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
                Player.PartyBelongedTo.ItemRoster.AddToCounts(pie, -1);
                Info.SetHeroHasToy(Npc, true);

                Notification.DrawBanner("You gave " + Npc + " a " + pie.Name);
            }
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

            if (Npc.Clan != null && Npc.Clan == Player.Clan)
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
            HeroKillAction.Apply(Npc, Npc, Player, KillReason.Suicide);
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

            if (Npc.Clan != null && Npc.Clan == Player.Clan)
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
            HeroKillAction.Apply(Npc, Npc, Player, KillReason.Suicide);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void PlayerViolatedNpc()
        {
            SetRoles();
            if (Info.GetHeroHorny(Player) >= DramalordMCM.Get.MinHornyForIntercourse)
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

        private static void SetRoles()
        {
            Player = Hero.MainHero;
            Npc = Hero.OneToOneConversationHero;
            PlayerSpouse = Hero.MainHero.Spouse;
            NpcSpouse = Hero.OneToOneConversationHero.Spouse;
        }
    }
}
