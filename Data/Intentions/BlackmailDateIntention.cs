using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class BlackmailDateIntention : Intention
    {
        [SaveableField(4)]
        public readonly DateIntention EventIntention;

        [SaveableField(5)]
        public readonly int Gold;

        public BlackmailDateIntention(DateIntention intention, Hero intentionHero, Hero target, CampaignTime validUntil) : base(intentionHero, target, validUntil)
        {
            EventIntention = intention;
            Gold = MathF.Max(5000 + (100 * intentionHero.GetPersonality().Agreeableness * -1) + (100 * intentionHero.GetPersonality().Conscientiousness * -1), 500);
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
                        .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as BlackmailDateIntention != null)
                .GotoDialogState("blackmail_start_date");


            DialogFlow gossipFlow = DialogFlow.CreateDialogFlow("blackmail_start_date")
                .BeginNpcOptions()
                    .NpcLine("{npc_blackmail_date}[ib:aggressive][if:convo_excited]")
                        .BeginPlayerOptions()
                            .PlayerOption("{player_blackmail_pay}")
                                .Condition(() => Hero.MainHero.Gold >= (ConversationTools.ConversationIntention as BlackmailDateIntention)?.Gold)
                                .Consequence(() => Hero.MainHero.Gold -= (ConversationTools.ConversationIntention as BlackmailDateIntention != null) ? (ConversationTools.ConversationIntention as BlackmailDateIntention).Gold : 0)
                                .NpcLine("{player_goods_select_pay}[ib:normal2][if:convo_mocking_teasing]")
                                    .CloseDialog()
                            .PlayerOption("{player_blackmail_quest}")
                                .Condition(() => DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) == null)
                                .Consequence(() =>
                                {
                                    BlackmailPlayerQuest quest = new BlackmailPlayerQuest(Hero.OneToOneConversationHero, (ConversationTools.ConversationIntention as BlackmailDateIntention).EventIntention, (ConversationTools.ConversationIntention as BlackmailDateIntention).Gold, CampaignTime.DaysFromNow(3));
                                    DramalordQuests.Instance.AddQuest(Hero.OneToOneConversationHero, quest);
                                    quest.StartQuest();
                                })
                                .NpcLine("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]")
                            .PlayerOption("{player_blackmail_nocare}")
                                .NpcLine("{npc_as_you_wish_reply}[ib:closed][if:convo_bored]")
                                .Consequence(() => 
                                    {
                                        BlackmailDateIntention intention = ConversationTools.ConversationIntention as BlackmailDateIntention;
                                        List<Hero> targets = new() { intention.IntentionHero, intention.Target, intention.EventIntention.IntentionHero, intention.EventIntention.Target };
                                        DramalordIntentions.Instance.GetIntentions().Add(new GossipDateIntention(intention.EventIntention, true, targets, Hero.OneToOneConversationHero, CampaignTime.DaysFromNow(7)));
                                    })
                        .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(gossipFlow);
        }

        public override void OnConversationEnded()
        {
            RelationshipLossAction.Apply(Target, IntentionHero, out int loveDamage, out int trustDamage, 25, 25);
            new ChangeOpinionIntention(Target, IntentionHero, loveDamage, trustDamage, CampaignTime.Now).Action();
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_confrontation_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));
            Hero other = EventIntention.IntentionHero == Hero.MainHero ? EventIntention.Target : EventIntention.IntentionHero;
            ConversationLines.npc_blackmail_date.SetTextVariable("HERO", other.Name);
            ConversationLines.npc_blackmail_date.SetTextVariable("AMOUNT", Gold);
            ConversationLines.npc_blackmail_date.SetTextVariable("SPOUSE", Hero.MainHero.Spouse.Name);

            ConversationLines.npc_as_you_wish_reply.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));
        }
    }
}
