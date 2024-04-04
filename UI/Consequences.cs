using Dramalord.Actions;
using Dramalord.Behaviors;
using Dramalord.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
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
            Info.IncreaseFlirtCountWithHero(Npc, Player);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcDeclinedFlirt()
        {
            Info.IncreaseFlirtCountWithHero(Npc, Player);
        }

        //DATE

        internal static void NpcAcceptedDate()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            Info.IncreaseFlirtCountWithHero(Npc, Player);
            AICampaignActions.CompleteDateActions(Npc, Player);
            if (!Info.GetIsCoupleWithHero(Player, Npc))
            {
                Info.SetIsCoupleWithHero(Player, Npc, true);
                Info.ChangeEmotionToHeroBy(Npc, Player, DramalordMCM.Get.MinEmotionForDating);
                Info.ChangeEmotionToHeroBy(Player, Npc, DramalordMCM.Get.MinEmotionForDating);
                Notification.DrawBanner("You and " + Npc + " are now a couple");
            }
            if(PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasConsideringDate()
        {
            Persuasions.CreatePersuasionTaskForDate();
        }

        internal static void NpcDeclinedDate()
        {
            Info.IncreaseFlirtCountWithHero(Npc, Player);
            Persuasions.ClearCurrentPersuasion();
        }

        //JOIN

        internal static void NpcAcceptedJoin()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            Info.IncreaseFlirtCountWithHero(Npc, Player);
            // TODO
        }

        internal static void NpcWasConsideringJoin()
        {
            Persuasions.CreatePersuasionTaskForJoining();
        }

        internal static void NpcDeclinedJoin()
        {
            Persuasions.ClearCurrentPersuasion();
            Info.IncreaseFlirtCountWithHero(Npc, Player);
        }

        //DIVORCE

        internal static void NpcAcceptedDivorce()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            Info.IncreaseFlirtCountWithHero(Npc, Player);

            HeroDivorceAction.Apply(Npc, NpcSpouse);
            
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasConsideringDivorce()
        {
            Persuasions.CreatePersuasionTaskForDivorce();
        }

        internal static void NpcDeclinedDivorce()
        {
            Persuasions.ClearCurrentPersuasion();
            Info.IncreaseFlirtCountWithHero(Npc, Player);
        }

        //MARRIAGE

        internal static void NpcAcceptedMarriage()
        {
            SetRoles();

            Persuasions.ClearCurrentPersuasion();
            Info.IncreaseFlirtCountWithHero(Npc, Player);
            HeroMarriageAction.Apply(Player, Npc);

            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasConsideringMarriage()
        {
            Persuasions.CreatePersuasionTaskForMarriage();
        }

        internal static void NpcDeclinedMarriage()
        {
            Persuasions.ClearCurrentPersuasion();
            Info.IncreaseFlirtCountWithHero(Npc, Player);
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
            Info.SetIsCoupleWithHero(Npc, Player, false);
        }

        internal static void NpcWasSurprisedByBreakup()
        {
            HeroBreakupAction.Apply(Player, Npc);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasHeartBrokenByBreakup()
        {
            HeroBreakupAction.Apply(Player, Npc);

            if (Npc.Clan != null && Npc.Clan == Player.Clan)
            {
                HeroLeaveClanAction.Apply(Npc, true, Npc);
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcGotSuicidalByBreakup()
        {
            HeroBreakupAction.Apply(Player, Npc);
            HeroKillAction.Apply(Npc, Npc, Player, KillReason.Suicide);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcDidntCareAboutDivorce()
        {
            HeroDivorceAction.Apply(Player, Npc);
        }

        internal static void NpcWasSurprisedByDivorce()
        {
            HeroDivorceAction.Apply(Player, Npc);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcWasHeartBrokenByDivorce()
        {
            HeroDivorceAction.Apply(Player, Npc);

            if (Npc.Clan != null && Npc.Clan == Player.Clan)
            {
                HeroLeaveClanAction.Apply(Npc, true, Npc);
            }
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void NpcGotSuicidalByDivorce()
        {
            HeroDivorceAction.Apply(Player, Npc);
            HeroKillAction.Apply(Npc, Npc, Player, KillReason.Suicide);
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        internal static void PlayerViolatedNpc()
        {
            if(AICampaignHelper.WillHaveIntercourse(Player, Npc, true))
            {
                HeroIntercourseAction.Apply(Player, Npc, true);
            
                if (AICampaignHelper.WillConceiveOffspring(Player, Npc))
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
