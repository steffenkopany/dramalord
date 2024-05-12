using Dramalord.Actions;
using Dramalord.Data;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Behaviors
{
    internal static class PlayerCampaignActions
    {
        internal static Action<Hero>? PostConversationAction;

        internal static void OnConversationEnded(IEnumerable<CharacterObject> characters)
        {
            Hero? npc = null;
            foreach (CharacterObject character in characters)
            {
                if(character.HeroObject != Hero.MainHero)
                {
                    npc = character.HeroObject;
                    PostConversationAction?.Invoke(npc);
                    PostConversationAction = null;
                    return;
                }
            }
        }

        internal static void PlayerFlirtAction(Hero npc)
        {
            HeroFlirtAction.Apply(npc, Hero.MainHero);

            List<Hero> flirts = new();
            List<Hero> partners = new();
            List<Hero> prisoners = new();

            AICampaignActions.ScopeSurroundings(npc, ref flirts, ref partners, ref prisoners, true);

            partners.Remove(Hero.MainHero);
            Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

            if (partner != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
            {
                HeroWitnessAction.Apply(npc, Hero.MainHero, partner, WitnessType.Flirting);
            }

            partners.Clear();

            AICampaignActions.ScopeSurroundings(Hero.MainHero, ref flirts, ref partners, ref prisoners, true);

            partners.Remove(npc);
            Hero? partner2 = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;

            if (partner2 != null && MBRandom.RandomInt(1, 100) < DramalordMCM.Get.ChanceGettingCaught)
            {
                HeroWitnessAction.Apply(Hero.MainHero, npc, partner2, WitnessType.Flirting);
            }
        }

        internal static void PlayerDateAction(Hero npc)
        {
            if (!Info.IsCoupleWithHero(Hero.MainHero, npc))
            {
                Info.SetIsCoupleWithHero(Hero.MainHero, npc, true);
                TextObject banner = new TextObject("{=Dramalord258}You and {HERO.LINK} are now a couple.");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }

            List<Hero> flirts = new();
            List<Hero> partners = new();
            List<Hero> prisoners = new();

            AICampaignActions.ScopeSurroundings(Hero.MainHero, ref flirts, ref partners, ref prisoners, true);
            partners.Remove(npc);
            Hero? partner = (partners.Count > 0) ? partners[MBRandom.RandomInt(partners.Count)] : null;
            AICampaignActions.CompleteDateActions(Hero.MainHero, npc, partner);
        }

        internal static void PlayerConvincedDivorceAction(Hero npc)
        {
            if(npc.Spouse != null)
            {
                HeroDivorceAction.Apply(npc, npc.Spouse);
                Info.SetLastPrivateMeeting(npc, Hero.MainHero, CampaignTime.Now.ToDays);
            }
        }

        internal static void PlayerMarriageAction(Hero npc)
        {
            Info.SetLastPrivateMeeting(npc, Hero.MainHero, CampaignTime.Now.ToDays);
            HeroMarriageAction.Apply(Hero.MainHero, npc);
        }

        internal static void PlayerBrokeUpNpcLeaveClan(Hero npc)
        {
            if (npc.Clan != null && npc.Clan == Hero.MainHero.Clan && DramalordMCM.Get.AllowClanChanges)
            {
                HeroLeaveClanAction.Apply(npc, npc);
            }
        }

        internal static void PlayerBrokeUpNpcSuicides(Hero npc)
        {
            if (DramalordMCM.Get.AllowRageKills)
            {
                HeroKillAction.Apply(npc, npc, Hero.MainHero, KillReason.Suicide);
            }
        }

        internal static void PlayerPerformsPrisonerDeal(Hero npc)
        {
            if (Info.ValidateHeroMemory(Hero.MainHero, npc))
            {
                HeroIntercourseAction.Apply(Hero.MainHero, npc, true);

                Hero mother = Hero.MainHero.IsFemale ? Hero.MainHero : npc;
                Hero father = npc.IsFemale ? Hero.MainHero : npc;

                if (mother != father && !mother.IsPregnant && Info.CanGetPregnant(mother) && MBRandom.RandomInt(1, 100) <= DramalordMCM.Get.PregnancyChance)
                {
                    HeroConceiveAction.Apply(Hero.MainHero, npc, true);
                }
                EndCaptivityAction.ApplyByRansom(npc, npc);
            }
        }
    }
}
