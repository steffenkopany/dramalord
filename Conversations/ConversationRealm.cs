using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Dramalord.Conversations
{
    internal static class ConversationRealm
    {
        private static Intention? GossipIntention = null;

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow leaveClanFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_kick_clan")
                .NpcLine("{player_reaction_breakup_accept}")
                .Condition(() => SetupLines())
                .Consequence(() => { ConversationTools.ConversationIntention = new LeaveClanToJoinOtherIntention(Hero.OneToOneConversationHero, CampaignTime.Now); ConversationTools.EndConversation(); })
                .CloseDialog();
 
            Campaign.Current.ConversationManager.AddDialogFlow(leaveClanFlow);

            DialogFlow gossipFlow = DialogFlow.CreateDialogFlow("hero_main_options")
                .PlayerLine("{player_request_gossip}")
                    .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && SetupLines())
                    .GotoDialogState("npc_shares_gossip");

            DialogFlow gossipFlow2 = DialogFlow.CreateDialogFlow("npc_shares_gossip")
                .BeginNpcOptions()
                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () =>
                    {
                        GossipBethrothedIntention? intention = DramalordIntentions.Instance.GetIntentions().FirstOrDefault(i  =>
                        {
                            GossipBethrothedIntention? gbi = i as GossipBethrothedIntention;

                            if (gbi != null && gbi.EventIntention.IntentionHero != Hero.MainHero && gbi.EventIntention.Target != Hero.MainHero && gbi.EventIntention.IntentionHero != Hero.OneToOneConversationHero && gbi.EventIntention.Target != Hero.OneToOneConversationHero && gbi.Targets.Contains(Hero.OneToOneConversationHero) && !gbi.Targets.Contains(Hero.MainHero))
                            {
                                return true;
                            }

                            return false;
                        }) as GossipBethrothedIntention;

                        if(intention != null)
                        {
                            intention.Targets.Add(Hero.MainHero);
                            intention.OnConversationStart();
                            ConversationTools.ConversationIntention = intention;
                            return true;
                        }
                        return false;
                    })
                        .GotoDialogState("gossip_start_bethrothed")
                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () =>
                    {
                        GossipBirthIntention? intention = DramalordIntentions.Instance.GetIntentions().FirstOrDefault(i =>
                        {
                            GossipBirthIntention? gbi = i as GossipBirthIntention;

                            if (gbi != null && gbi.EventIntention.IntentionHero != Hero.MainHero && gbi.EventIntention.Target != Hero.MainHero && gbi.EventIntention.IntentionHero != Hero.OneToOneConversationHero && gbi.EventIntention.Target != Hero.OneToOneConversationHero && gbi.Targets.Contains(Hero.OneToOneConversationHero) && !gbi.Targets.Contains(Hero.MainHero))
                            {
                                return true;
                            }

                            return false;
                        }) as GossipBirthIntention;

                        if (intention != null)
                        {
                            intention.Targets.Add(Hero.MainHero);
                            intention.OnConversationStart();
                            ConversationTools.ConversationIntention = intention;
                            return true;
                        }
                        return false;
                    })
                        .GotoDialogState("gossip_start_birth")

                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () =>
                    {
                        GossipDateIntention? intention = DramalordIntentions.Instance.GetIntentions().FirstOrDefault(i =>
                        {
                            GossipDateIntention? gbi = i as GossipDateIntention;

                            if (gbi != null && gbi.EventIntention.IntentionHero != Hero.MainHero && gbi.EventIntention.Target != Hero.MainHero && gbi.EventIntention.IntentionHero != Hero.OneToOneConversationHero && gbi.EventIntention.Target != Hero.OneToOneConversationHero && gbi.Targets.Contains(Hero.OneToOneConversationHero) && !gbi.Targets.Contains(Hero.MainHero))
                            {
                                return true;
                            }

                            return false;
                        }) as GossipDateIntention;

                        if (intention != null)
                        {
                            intention.Targets.Add(Hero.MainHero);
                            intention.OnConversationStart();
                            ConversationTools.ConversationIntention = intention;
                            return true;
                        }
                        return false;
                    })
                        .GotoDialogState("gossip_start_date")

                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () =>
                    {
                        GossipIntercourseIntention? intention = DramalordIntentions.Instance.GetIntentions().FirstOrDefault(i =>
                        {
                            GossipIntercourseIntention? gbi = i as GossipIntercourseIntention;

                            if (gbi != null && gbi.EventIntention.IntentionHero != Hero.MainHero && gbi.EventIntention.Target != Hero.MainHero && gbi.EventIntention.IntentionHero != Hero.OneToOneConversationHero && gbi.EventIntention.Target != Hero.OneToOneConversationHero && gbi.Targets.Contains(Hero.OneToOneConversationHero) && !gbi.Targets.Contains(Hero.MainHero))
                            {
                                return true;
                            }

                            return false;
                        }) as GossipIntercourseIntention;

                        if (intention != null)
                        {
                            intention.Targets.Add(Hero.MainHero);
                            intention.OnConversationStart();
                            ConversationTools.ConversationIntention = intention;
                            return true;
                        }
                        return false;
                    })
                        .GotoDialogState("gossip_start_intercourse")

                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () =>
                    {
                        GossipMarriageIntention? intention = DramalordIntentions.Instance.GetIntentions().FirstOrDefault(i =>
                        {
                            GossipMarriageIntention? gbi = i as GossipMarriageIntention;

                            if (gbi != null && gbi.EventIntention.IntentionHero != Hero.MainHero && gbi.EventIntention.Target != Hero.MainHero && gbi.EventIntention.IntentionHero != Hero.OneToOneConversationHero && gbi.EventIntention.Target != Hero.OneToOneConversationHero && gbi.Targets.Contains(Hero.OneToOneConversationHero) && !gbi.Targets.Contains(Hero.MainHero))
                            {
                                return true;
                            }

                            return false;
                        }) as GossipMarriageIntention;

                        if (intention != null)
                        {
                            intention.Targets.Add(Hero.MainHero);
                            intention.OnConversationStart();
                            ConversationTools.ConversationIntention = intention;
                            return true;
                        }
                        return false;
                    })
                        .GotoDialogState("gossip_start_marriage")
                    .NpcOption("{npc_no_gossip}", () =>
                    {
                        return true;
                    })
                    .GotoDialogState("hero_main_options")
                .EndNpcOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(gossipFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(gossipFlow2);
        }

        internal static bool SetupLines()
        {
            ConversationLines.player_reaction_breakup_accept.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.player_interaction_abort.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord101}Let's talk about something else, {TITLE}.");
            ConversationLines.npc_challenge_summarize_end.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            return true;
        }
    }
}
