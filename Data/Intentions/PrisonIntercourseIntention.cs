using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Notifications.Logs;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class PrisonIntercourseIntention : Intention
    {
        private static bool _accepted = false;

        public PrisonIntercourseIntention(Hero target, Hero intentionHero, CampaignTime validUntil, bool alwaysExecute = false) : base(intentionHero, target, validUntil)
        {
            _accepted = alwaysExecute;
        }

        public override bool Action()
        {
            _accepted = false;

            List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
            if (Target == Hero.MainHero && closeHeroes.Contains(Hero.MainHero) && ConversationTools.StartConversation(this, true))
            {
                return true;
            }
            else if (Target != Hero.MainHero && closeHeroes.Contains(Target))
            {
                _accepted = true;
                OnConversationEnded();
                return true;
            }

            return false;
        }

        public override void OnConversationEnded()
        {
            IntentionHero.GetRelationTo(Target).LastInteraction = CampaignTime.Now;

            if (_accepted)
            {
                IntercourseAction.Apply(IntentionHero, Target, out int loveGain);

                if (Target == Hero.MainHero || IntentionHero == Hero.MainHero)
                {
                    if (IntercourseIntention.HotButterFound)
                    {
                        MBInformationManager.ShowSceneNotification(new HotButterNotification(IntentionHero, Target, IntentionHero.CurrentSettlement));
                    }
                    else
                    {
                        Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                        TextObject banner = new TextObject("{=Dramalord072}You were intimate with {HERO.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 1000, otherHero.CharacterObject, "event:/ui/notification/relation");
                    }
                }

                if (DramalordMCM.Instance.IntimateLogs)
                {
                    LogEntry.AddLogEntry(new PrisonIntercourseLog(IntentionHero, Target));
                }


                RelationshipLossAction.Apply(IntentionHero, Target, out int loveDamage, out int trustDamage, 50, 50);
                new ChangeOpinionIntention(IntentionHero, Target, loveDamage, trustDamage, CampaignTime.Now).Action();     

                if (IntentionHero.IsFemale != Target.IsFemale && MBRandom.RandomInt(1, 100) < DramalordMCM.Instance.PregnancyChance)
                {
                    Hero female = (IntentionHero.IsFemale) ? IntentionHero : Target;
                    Hero male = (IntentionHero.IsFemale) ? Target : IntentionHero;

                    if (female.IsFertile())
                    {
                        ConceiveAction.Apply(female, male);

                        if (female == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord075}You got pregnant from {HERO.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", male.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, male.CharacterObject, "event:/ui/notification/relation");
                        }
                        else if (male == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord076}{HERO.LINK} got pregnant from you.");
                            StringHelpers.SetCharacterProperties("HERO", female.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, female.CharacterObject, "event:/ui/notification/relation");
                        }

                        if (female.Clan == Clan.PlayerClan || male.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions)
                        {
                            LogEntry.AddLogEntry(new ConceiveChildLog(female, male));
                        }
                    }
                }

                EndCaptivityAction.ApplyByRansom(Target, IntentionHero);
            }

            _accepted = false;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine("{player_prisoner_start}[ib:confident][if:convo_mocking_teasing]")
                .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as PrisonIntercourseIntention != null)
                .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                    .NpcLine("{player_wants_prisonfun}[ib:aggressive][if:convo_excited]")
                        .BeginPlayerOptions()
                            .PlayerOption("{npc_prisonfun_reaction_yes}")
                                .Consequence(() => { _accepted = true; ConversationTools.EndConversation(); })
                                .CloseDialog()
                            .PlayerOption("{npc_prisonfun_reaction_no}")
                                .Consequence(() => { _accepted = false; ConversationTools.EndConversation(); })
                                .CloseDialog()
                        .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
        }

        public override void OnConversationStart()
        {
            ConversationLines.player_prisoner_start.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord273}Prisoner, I need to have a word with you.");
            ConversationLines.player_wants_prisonfun.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord276}I would consider letting you go for... some special service in my bedroom.");
            ConversationLines.npc_prisonfun_reaction_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord278}You got yourself a deal! I think I will even enjoy it.");
            ConversationLines.npc_prisonfun_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord279}Never! You will not taint my honor with such offers!");
        }
    }
}
