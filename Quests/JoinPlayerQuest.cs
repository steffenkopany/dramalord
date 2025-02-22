using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Quests
{
    internal class JoinPlayerQuest : DramalordQuest
    {
        [SaveableField(1)]
        internal Settlement StartLocation;

        //private bool Timeout() => QuestGiver.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow < DramalordMCM.Instance.DaysBetweenInteractions;

        public JoinPlayerQuest(Hero questGiver, CampaignTime duration) : base("DramalordJoinPlayerQuest", questGiver, duration)
        {
            StartLocation = questGiver.CurrentSettlement;
        }

        public override TextObject GetTitle()
        {
            TextObject txt = new TextObject("{=Dramalord541}{QUESTHERO} joins your party for a while.");
            MBTextManager.SetTextVariable("QUESTHERO", QuestGiver.Name);
            return txt;
        }

        protected override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, HourlyTick);
        }

        protected override void HourlyTick()
        {
            if(QuestGiver.PartyBelongedTo != MobileParty.MainParty)
            {
                QuestFail(Hero.MainHero);
            }
        }

        protected override void InitializeQuestOnGameLoad()
        {
            RemoveTrackedObject(QuestGiver);
        }

        protected override void SetDialogs()
        {
            
        }

        public override void OnCanceled()
        {
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
        }

        public override void QuestFail(Hero reason)
        {
            RelationshipLossAction.Apply(QuestGiver, Hero.MainHero, out int loveDamage, out int trustDamage, 30, 50);

            TextObject banner = new TextObject("{=Dramalord543}{HERO.LINK} is very disappointed to be left alone and went back to {TOWN.LINK}.");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            StringHelpers.SetSettlementProperties("TOWN", StartLocation, banner);
            MBInformationManager.AddQuickInformation(banner, 0, QuestGiver.CharacterObject, "event:/ui/notification/relation");

            new ChangeOpinionIntention(QuestGiver, Hero.MainHero, loveDamage, trustDamage, CampaignTime.Now).Action();
            DramalordQuests.Instance.RemoveQuest(QuestGiver);
            TeleportHeroAction.ApplyImmediateTeleportToSettlement(QuestGiver, StartLocation);

            AddLog(banner);
            CompleteQuestWithFail();
        }

        public override void QuestSuccess(Hero reason)
        {
            DateAction.Apply(QuestGiver, Hero.MainHero, out int loveGain, out int trustGain, 2);

            TextObject banner = new TextObject("{=Dramalord544}{HERO.LINK} enjoyed spending time with you and went back to {TOWN.LINK}.");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, banner);
            StringHelpers.SetSettlementProperties("TOWN", StartLocation, banner);
            MBInformationManager.AddQuickInformation(banner, 0, QuestGiver.CharacterObject, "event:/ui/notification/relation");

            new ChangeOpinionIntention(QuestGiver, Hero.MainHero, loveGain, trustGain, CampaignTime.Now).Action();

            DramalordQuests.Instance.RemoveQuest(QuestGiver);

            if(QuestGiver.CurrentSettlement != null)
            {
                LeaveSettlementAction.ApplyForCharacterOnly(QuestGiver);
            }
            
            TeleportHeroAction.ApplyImmediateTeleportToSettlement(QuestGiver, StartLocation);
            EnterSettlementAction.ApplyForCharacterOnly(QuestGiver, StartLocation);

            AddLog(banner);
            CompleteQuestWithSuccess();
        }

        public override void QuestTimeout()
        {
            new FinishJoinPartyQuestIntention(Hero.MainHero, QuestGiver, this, CampaignTime.Now).Action();
        }

        public override void QuestStartInit()
        {
            StartLocation = QuestGiver.CurrentSettlement;

            TextObject txt = new TextObject("{=Dramalord542}{HERO.LINK} wants to spend some quality time with you, and joins you on your journey for a while.");
            StringHelpers.SetCharacterProperties("HERO", QuestGiver.CharacterObject, txt);
            AddLog(txt);
            AddHeroToPartyAction.Apply(QuestGiver, MobileParty.MainParty);
            RemoveTrackedObject(QuestGiver);
        }
    }
}
