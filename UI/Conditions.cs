using Dramalord.Actions;
using Dramalord.Data;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace Dramalord.UI
{
    internal static class Conditions
    {
        static Hero? Player = null;
        static Hero? Npc = null;
        static Hero? PlayerSpouse = null;
        static Hero? NpcSpouse = null;

        internal static Hero? ApproachingHero = null;

        internal static Hero? CheatingHero = null;
        internal static WitnessType WitnessOf = WitnessType.Flirting;
        internal static Hero? LoverOrChild = null;

        internal static bool PlayerCanAskForTalk()
        {
            SetRoles();
            return Info.ValidateHeroInfo(Npc) && Info.ValidateHeroMemory(Player,Npc);
        }

        internal static bool NpcWantsToTalk()
        {
            SetRoles();
            return IsCoupleWithPlayer() || EmotionForTalking();
        }

        internal static bool NpcDeclinesToTalk()
        {
            SetRoles();
            return !NpcWantsToTalk();
        }

        internal static bool PlayerCanAskForAction()
        {
            SetRoles();
            return CampaignTime.Now.ToDays - Info.GetLastDate(Npc, Player) > 0;
        }

        internal static bool PlayerCanAskForFlirt()
        {
            return true;
        }

        internal static bool NpcWantsToFlirt()
        {
            SetRoles();
            return (IsCoupleWithPlayer() || FindsPlayerAttractive()) && CampaignTime.Now.ToDays - Info.GetLastDaySeen(Npc, Player) > 0;
        }

        internal static bool NpcDeclinesToFlirt()
        {
            SetRoles();
            return !NpcWantsToFlirt();
        }

        // DATES

        internal static bool PlayerCanAskForDate()
        {
            SetRoles();
            return IsCoupleWithPlayer() || EnoughEmotionForDate() || NpcIsMarriedToPlayer();
        }

        internal static bool NpcWantsToDate()
        {
            SetRoles();
            return (((IsCoupleWithPlayer() && EnoughEmotionForDate()) || NpcIsMarriedToPlayer()) && LastDateInPastEnough()) || PersuasionSuccess();
        }

        internal static bool NpcConsidersToDate()
        {
            SetRoles();
            return !IsCoupleWithPlayer() && !NpcIsMarriedToPlayer() && (FindsPlayerAttractive() || EnoughEmotionForDate()) && LastDateInPastEnough();
        }

        internal static bool NpcDeclinesToDate()
        {
            SetRoles();
            return !NpcWantsToDate();
        }


        // JOIN

        internal static bool PlayerCanAskForJoin()
        {
            SetRoles();
            return IsCoupleWithPlayer() || EnoughEmotionForMarriage() || NpcIsMarriedToPlayer();
        }

        internal static bool NpcWantsToJoin()
        {
            SetRoles();
            return ((IsCoupleWithPlayer() || EnoughEmotionForMarriage() || NpcIsMarriedToPlayer()) && LastDateInPastEnough()) || PersuasionSuccess();
        }

        internal static bool NpcConsidersToJoin()
        {
            SetRoles();
            return IsCoupleWithPlayer() && !NpcIsMarriedToPlayer() && (FindsPlayerAttractive() || EnoughEmotionForMarriage()) && LastDateInPastEnough();
        }

        internal static bool NpcDeclinesToJoin()
        {
            SetRoles();
            return !NpcWantsToJoin();
        }

        // DIVORCE
        internal static bool PlayerCanAskForDivorceHusband()
        {
            SetRoles();
            return NpcIsMarried() && !NpcIsMarriedToPlayer() && IsCoupleWithPlayer();
        }

        internal static bool NpcWantsToDivorceHusband()
        {
            SetRoles();
            return (NpcWouldDivorceHusband() && !NpcLovesHusband() && !NpcIsMarriedToPlayer()) || PersuasionSuccess();
        }

        internal static bool NpcConsidersToDivorceHusband()
        {
            SetRoles();
            return !NpcWouldDivorceHusband() && !NpcLovesHusband() && !NpcIsMarriedToPlayer();
        }

        internal static bool NpcDeclinesToDivorceHusband()
        {
            SetRoles();
            return NpcLovesHusband() && !NpcIsMarriedToPlayer();
        }

        //MARRIAGE
        internal static bool PlayerCanAskForMarriage()
        {
            SetRoles();
            return !NpcIsMarried() && Player.Spouse == null && IsCoupleWithPlayer();
        }

        internal static bool NpcWantsToMarry()
        {
            SetRoles();
            return (EnoughEmotionForMarriage() && IsCoupleWithPlayer()) || PersuasionSuccess();
        }

        internal static bool NpcConsidersToMarry()
        {
            SetRoles();
            return !EnoughEmotionForMarriage() && EnoughEmotionForDate() && IsCoupleWithPlayer();
        }

        internal static bool NpcDeclinesToMarry()
        {
            SetRoles();
            return (!EnoughEmotionForMarriage() && EnoughEmotionForDate()) || !IsCoupleWithPlayer();
        }

        internal static bool PlayerCanGivePresent()
        {
            SetRoles();
            //ItemObject leek = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_leek");
            ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
            ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
            if(Npc.IsFemale)
            {
                return !Info.GetHeroHasToy(Npc) && Player.PartyBelongedTo.ItemRoster.FindIndexOfItem(wurst) >= 0;
            }
            else
            {
                return !Info.GetHeroHasToy(Npc) && Player.PartyBelongedTo.ItemRoster.FindIndexOfItem(pie) >= 0;
            }
        }

        internal static bool PlayerCanAskForBreakup()
        {
            SetRoles();
            return Info.IsCoupleWithHero(Player, Npc) && Player.Spouse != Npc;
        }

        internal static bool NpcDoesntMindBreakup()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) <= DramalordMCM.Get.MinEmotionBeforeDivorce;
        }

        internal static bool NpcSurprisedByBreakup()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) <= DramalordMCM.Get.MinEmotionForDating && Info.GetEmotionToHero(Npc, Player) > DramalordMCM.Get.MinEmotionBeforeDivorce;
        }

        internal static bool NpcHeartbrokenByBreakup()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) <= DramalordMCM.Get.MinEmotionForMarriage && Info.GetEmotionToHero(Npc, Player) > DramalordMCM.Get.MinEmotionForDating;
        }

        internal static bool NpcSuicidalByBreakup()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) > DramalordMCM.Get.MinEmotionForMarriage;
        }

        internal static bool PlayerCanAskForDivorce()
        {
            SetRoles();
            return  PlayerSpouse == Npc;
        }

        internal static bool PlayerCanViolateNpc()
        {
            SetRoles();
            return (Player.PartyBelongedTo != null && Player.PartyBelongedTo.PrisonRoster != null && Player.PartyBelongedTo.PrisonRoster.Contains(Npc.CharacterObject)) || (Npc.IsPrisoner && Npc.CurrentSettlement != null && Npc.CurrentSettlement.OwnerClan == Player.Clan);
        }

        // APPROACHING
        internal static bool NPCCanApproachPlayer()
        {
            if(Hero.OneToOneConversationHero != null && (Hero.OneToOneConversationHero.IsLord || Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer))
            {
                SetRoles();
                bool result = ApproachingHero == Npc;
                ApproachingHero = null;
                if (result)
                {
                    string text = string.Empty;
                    if (Npc.Spouse == Player)
                    {
                        text = Hero.MainHero.IsFemale ? new TextObject("{=Dramalord049}wife").ToString() : new TextObject("{=Dramalord048}husband").ToString();
                    }
                    else if (Info.IsCoupleWithHero(Npc, Player))
                    {
                        text = Hero.MainHero.IsFemale ? new TextObject("{=Dramalord097}My love").ToString() : new TextObject("{=Dramalord096}My lover").ToString();
                    }
                    else
                    {
                        text = Hero.MainHero.IsFemale ? GameTexts.FindText("str_my_lady").ToString() : GameTexts.FindText("str_my_lord").ToString();
                    }

                    char[] array = text.ToCharArray();
                    text = array[0].ToString().ToUpper();
                    for (int i = 1; i < array.Length; i++)
                    {
                        text += array[i];
                    }

                    MBTextManager.SetTextVariable("TITLE", text);
                }
                return result;
            }
            return false;  
        }

        internal static bool NPCAcceptsViolation()
        {
            SetRoles();
            return Npc.GetHeroTraits().Honor < 1;
        }

        internal static bool NPCDeclinesViolation()
        {
            SetRoles();
            return Npc.GetHeroTraits().Honor >= 1;
        }

        internal static bool NPCAsksPlayerSomething()
        {
            return NPCCanAskForFlirt() || NPCCanAskForDate() || NPCCanAskForMarriage();
        }

        internal static bool NPCTellsPlayerSomething()
        {
            return NPCCanAskForDivorce() || NPCCanAskForBreakup();
        }

        internal static bool NPCCanAskForFlirt()
        {
            SetRoles();
            bool wantsFlirt;
            Info.GetInteractionVariables(Npc, Player, out wantsFlirt, out _, out _, out _, out _);
            return wantsFlirt;
        }

        internal static bool NPCCanAskForDate()
        {
            SetRoles();
            bool wantsDate;
            Info.GetInteractionVariables(Npc, Player, out _, out wantsDate, out _, out _, out _);
            return wantsDate;
        }

        internal static bool NPCCanAskForMarriage()
        {
            SetRoles();
            bool wantsToMarry;
            Info.GetInteractionVariables(Npc, Player, out _, out _, out wantsToMarry, out _, out _);
            return wantsToMarry;
        }

        internal static bool NPCCanAskForDivorce()
        {
            SetRoles();
            bool wantsToDivorce;
            Info.GetInteractionVariables(Npc, Player, out _, out _, out _, out _, out wantsToDivorce);
            return wantsToDivorce;
        }

        internal static bool NPCCanAskForBreakup()
        {
            SetRoles();
            bool wantsToBreakUp;
            Info.GetInteractionVariables(Npc, Player, out _, out _, out _, out wantsToBreakUp, out _);
            return wantsToBreakUp;
        }

        // CONFRONTATION
        internal static bool PlayerCanConfrontNpc()
        {
            if (Hero.OneToOneConversationHero != null && (Hero.OneToOneConversationHero.IsLord || Hero.OneToOneConversationHero.Occupation == Occupation.Wanderer))
            {
                SetRoles();
                bool result = CheatingHero == Npc;
                CheatingHero = null;
                if (result)
                {
                    string text = string.Empty;
                    if (Npc.Spouse == Player)
                    {
                        text = Hero.MainHero.IsFemale ? new TextObject("{=Dramalord049}wife").ToString() : new TextObject("{=Dramalord048}husband").ToString();
                    }
                    else if (Info.IsCoupleWithHero(Npc, Player))
                    {
                        text = Hero.MainHero.IsFemale ? new TextObject("{=Dramalord097}My love").ToString() : new TextObject("{=Dramalord096}My lover").ToString();
                    }
                    else
                    {
                        text = Hero.MainHero.IsFemale ? GameTexts.FindText("str_my_lady").ToString() : GameTexts.FindText("str_my_lord").ToString();
                    }

                    char[] array = text.ToCharArray();
                    text = array[0].ToString().ToUpper();
                    for (int i = 1; i < array.Length; i++)
                    {
                        text += array[i];
                    }

                    MBTextManager.SetTextVariable("TITLE", text);
                }
                return result;
            }
            return false;
        }

        internal static bool PlayerSeesPregnancy()
        {
            return WitnessOf == WitnessType.Pregnancy;
        }

        internal static bool PlayerSeesDate()
        {
            return WitnessOf == WitnessType.Dating;
        }

        internal static bool PlayerSeesIntercourse()
        {
            return WitnessOf == WitnessType.Intercourse;
        }

        internal static bool PlayerSeesBastard()
        {
            return WitnessOf == WitnessType.Bastard;
        }

        internal static bool NpcAccusedDoesntCare()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) < DramalordMCM.Get.MinEmotionBeforeDivorce;
        }

        internal static bool NpcAccusedPlaysInnocent()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) < DramalordMCM.Get.MinEmotionForMarriage;
        }

        internal static bool NpcAccusedBegsForgiveness()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) >= DramalordMCM.Get.MinEmotionForMarriage;
        }

        internal static bool PlayerCanKillNpc()
        {
            return WitnessOf == WitnessType.Pregnancy || WitnessOf == WitnessType.Bastard || WitnessOf == WitnessType.Intercourse;
        }

        internal static bool PlayerCanKickNpcOut()
        {
            SetRoles();
            return Npc.Clan == Clan.PlayerClan;
        }

        internal static bool PlayerCanBreakUpOrDivorce()
        {
            SetRoles();
            return Player.Spouse == Npc || Info.IsCoupleWithHero(Player, Npc);
        }

        // private stuff
        private static bool FindsPlayerAttractive()
        {
            SetRoles();
            return Info.GetAttractionToHero(Npc, Player) >= DramalordMCM.Get.MinAttractionForFlirting;
        }

        private static bool IsCoupleWithPlayer()
        {
            SetRoles();
            return Info.IsCoupleWithHero(Npc, Player);
        }

        private static bool NpcIsMarriedToPlayer()
        {
            SetRoles();
            return NpcSpouse == Player;
        }

        private static bool NpcIsMarried()
        {
            SetRoles();
            return NpcSpouse != null;
        }

        private static bool NpcLovesHusband()
        {
            SetRoles();
            return NpcSpouse != null && Info.GetEmotionToHero(Npc, NpcSpouse) >= DramalordMCM.Get.MinEmotionForMarriage;
        }

        private static bool NpcWouldDivorceHusband()
        {
            SetRoles();
            return NpcSpouse != null && Info.GetEmotionToHero(Npc, NpcSpouse) < DramalordMCM.Get.MinEmotionBeforeDivorce;
        }

        private static bool EmotionForTalking()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) >= DramalordMCM.Get.MinEmotionForConversation;
        }

        private static bool EnoughEmotionForDate()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) >= DramalordMCM.Get.MinEmotionForDating;
        }

        private static bool EnoughEmotionForMarriage()
        {
            SetRoles();
            return Info.GetEmotionToHero(Npc, Player) >= DramalordMCM.Get.MinEmotionForMarriage;
        }

        private static bool LastDateInPastEnough()
        {
            SetRoles();
            return CampaignTime.Now.ToDays - Info.GetLastDate(Npc, Player) > DramalordMCM.Get.DaysBetweenDates;
        }

        private static bool PersuasionSuccess()
        {
            SetRoles();
            return ConversationManager.GetPersuasionIsActive() && ConversationManager.GetPersuasionProgressSatisfied();
        }

        private static bool PersuasionFail()
        {
            SetRoles();
            return ConversationManager.GetPersuasionIsActive() && !ConversationManager.GetPersuasionProgressSatisfied();
        }

        private static bool PersuasionActive()
        {
            SetRoles();
            return ConversationManager.GetPersuasionIsActive();
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
