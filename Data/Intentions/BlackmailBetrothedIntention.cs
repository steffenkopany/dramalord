﻿using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class BlackmailBetrothedIntention : Intention
    {
        [SaveableField(4)]
        public readonly BetrothIntention EventIntention;

        [SaveableField(5)]
        public readonly int Gold;

        public BlackmailBetrothedIntention(BetrothIntention intention, Hero intentionHero, Hero target, CampaignTime validUntil) : base(intentionHero, target, validUntil)
        {
            EventIntention = intention;
            Gold = MathF.Max(10000 + (100 * intentionHero.GetPersonality().Agreeableness * -1) + (100 * intentionHero.GetPersonality().Conscientiousness * -1), 1000);
        }

        public override bool Action()
        {
            List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();

            if (Target == Hero.MainHero && closeHeroes.Contains(Hero.MainHero) && ConversationTools.StartConversation(this, IntentionHero.CurrentSettlement != null))
            {
                return true;
            }
            else if (Target != Hero.MainHero && closeHeroes.Contains(Target))
            {
                Target.Gold = (Target.Gold >= Gold) ? Target.Gold - Gold : 0;
                OnConversationEnded();
                return true;
            }
            return false;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow npcFlow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine("{npc_starts_confrontation_known}[ib:aggressive][if:convo_undecided_open]")
                        .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as BlackmailBetrothedIntention != null)
                .GotoDialogState("blackmail_start_betrothed");


            DialogFlow gossipFlow = DialogFlow.CreateDialogFlow("blackmail_start_betrothed")
                .BeginNpcOptions()
                    .NpcLine("{npc_blackmail_betrothed}[ib:aggressive][if:convo_excited]")
                        .BeginPlayerOptions()
                            .PlayerOption("{player_blackmail_pay}")
                                .Condition(() => Hero.MainHero.Gold >= (ConversationTools.ConversationIntention as BlackmailBetrothedIntention)?.Gold)
                                .Consequence(() => Hero.MainHero.Gold -= (ConversationTools.ConversationIntention as BlackmailBetrothedIntention != null) ? (ConversationTools.ConversationIntention as BlackmailBetrothedIntention).Gold : 0)
                                .NpcLine("{player_goods_select_pay}[ib:normal2][if:convo_mocking_teasing]")
                                    .CloseDialog()
                            .PlayerOption("{player_blackmail_quest}")
                                .Consequence(() => 
                                    { 
                                        // start quest here
                                    })
                                .NpcLine("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]")
                            .PlayerOption("{player_blackmail_nocare}")
                                .NpcLine("{npc_as_you_wish_reply}[ib:closed][if:convo_bored]")
                                .Consequence(() => 
                                    {
                                        BlackmailBetrothedIntention intention = ConversationTools.ConversationIntention as BlackmailBetrothedIntention;
                                        List<Hero> targets = new() { intention.IntentionHero, intention.Target, intention.EventIntention.IntentionHero, intention.EventIntention.Target };
                                        DramalordIntentions.Instance.GetIntentions().Add(new GossipBetrothedIntention(intention.EventIntention, true, targets, Hero.OneToOneConversationHero, CampaignTime.DaysFromNow(7)));
                                    })
                        .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(gossipFlow);
        }

        public override void OnConversationEnded()
        {
            RelationshipLossAction.Apply(Target, IntentionHero, out int loveDamage, out int trustDamage, 50, 50);
            new ChangeOpinionIntention(Target, IntentionHero, loveDamage, trustDamage, CampaignTime.Now).Action();
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_confrontation_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));
            Hero other = EventIntention.IntentionHero == Hero.MainHero ? EventIntention.Target : EventIntention.IntentionHero;
            ConversationLines.npc_blackmail_betrothed.SetTextVariable("HERO", other.Name);
            ConversationLines.npc_blackmail_betrothed.SetTextVariable("AMOUNT", Gold);
            ConversationLines.npc_blackmail_betrothed.SetTextVariable("SPOUSE", Hero.MainHero.Spouse.Name);

            ConversationLines.npc_as_you_wish_reply.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));
        }
    }
}
