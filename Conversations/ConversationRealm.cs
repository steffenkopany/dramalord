using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

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
                .BeginPlayerOptions()
                .PlayerOption("{player_request_gossip}")
                    .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && SetupLines())
                    .GotoDialogState("npc_shares_gossip")
                .EndPlayerOptions();

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

            DialogFlow prisonFlow = DialogFlow.CreateDialogFlow("hero_main_options", 200)
               .BeginPlayerOptions()
                   .PlayerOption("{player_prisoner_start}")
                   .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.MainHero.GetClosePrisoners().Contains(Hero.OneToOneConversationHero) && SetupLines())
                   .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                    .BeginNpcOptions()
                        .NpcOption("{npc_prisoner_reply_yes}[ib:closed][if:convo_bored]", () => Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) > DramalordMCM.Instance.MaxTrustEnemies)
                            .BeginPlayerOptions()
                                .PlayerOption("{player_wants_prisonfun}")
                                    .BeginNpcOptions()
                                        .NpcOption("{npc_prisonfun_reaction_yes}[ib:normal2][if:convo_confused_annoyed]", () => Hero.OneToOneConversationHero.GetHeroTraits().Honor <= 0 && Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) >= DramalordMCM.Instance.MinAttraction)
                                            .Consequence(() => ConversationTools.ConversationIntention = new PrisonIntercourseIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, true))
                                            .CloseDialog()
                                        .NpcOption("{npc_prisonfun_reaction_no}[ib:nervous][if:convo_shocked]", () => Hero.OneToOneConversationHero.GetHeroTraits().Honor > 0 || Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) < DramalordMCM.Instance.MinAttraction)
                                            .GotoDialogState("hero_main_options")
                                    .EndNpcOptions()
                                .PlayerOption("{player_wants_kill}")
                                    .BeginNpcOptions()
                                        .NpcOption("{npc_kill_reaction_no}", () => Hero.OneToOneConversationHero.GetHeroTraits().Valor <= 0 && Hero.OneToOneConversationHero.GetHeroTraits().Honor > 0)
                                            .Consequence(() => MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForPlayerExecutingHero(Hero.OneToOneConversationHero, null)))
                                            .CloseDialog()
                                        .NpcOption("{npc_kill_reaction_yes}", () => Hero.OneToOneConversationHero.GetHeroTraits().Valor > 0 && Hero.OneToOneConversationHero.GetHeroTraits().Honor > 0)
                                            .Consequence(() => MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForPlayerExecutingHero(Hero.OneToOneConversationHero, null)))
                                            .CloseDialog()
                                        .NpcOption("{npc_kill_reaction_offer}", () => Hero.OneToOneConversationHero.GetHeroTraits().Valor <= 0 && Hero.OneToOneConversationHero.GetHeroTraits().Honor <= 0)
                                            .BeginPlayerOptions()
                                                .PlayerOption("{player_choose_pleasure_yes}")
                                                    .Consequence(() => ConversationTools.ConversationIntention = new PrisonIntercourseIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, true))
                                                    .CloseDialog()
                                                .PlayerOption("{player_choose_pleasure_no}")
                                                    .Consequence(() => MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForPlayerExecutingHero(Hero.OneToOneConversationHero, null)))
                                                    .CloseDialog()
                                            .EndPlayerOptions()
                                    .EndNpcOptions()
                            .EndNpcOptions()
                        .NpcOption("{npc_prisoner_reply_no}[ib:aggressive][if:convo_grave]", () => Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) <= DramalordMCM.Instance.MaxTrustEnemies)
                            .GotoDialogState("hero_main_options")
                    .EndNpcOptions()
                .EndPlayerOptions();


            Campaign.Current.ConversationManager.AddDialogFlow(prisonFlow);
        }

        internal static bool SetupLines()
        {
            ConversationLines.player_reaction_breakup_accept.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.player_interaction_abort.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord101}Let's talk about something else, {TITLE}.");
            ConversationLines.npc_challenge_summarize_end.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            ConversationLines.player_prisoner_start.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord273}Prisoner, I need to have a word with you.");
            ConversationLines.npc_prisoner_reply_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord274}It's not like I have much of a choice {TITLE}.");
            ConversationLines.npc_prisoner_reply_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false)); // new("{=Dramalord275}I refuse to converse with you.");
            ConversationLines.player_wants_prisonfun.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord276}I would consider letting you go for... some special service in my bedroom.");
            ConversationLines.player_wants_kill.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord277}It's time to end this. Your existence is bothering me.");
            ConversationLines.npc_prisonfun_reaction_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord278}You got yourself a deal! I think I will even enjoy it.");
            ConversationLines.npc_prisonfun_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord279}Never! You will not taint my honor with such offers!");
            ConversationLines.npc_kill_reaction_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//new("{=Dramalord280}Do what you must, you honorless swine!");
            ConversationLines.npc_kill_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//new("{=Dramalord281}Oh god! No please... I beg you!");
            ConversationLines.npc_kill_reaction_offer.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//new("{=Dramalord282}Wait {TITLE}! Why choose death if there's also pleasure?");
            ConversationLines.player_choose_pleasure_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//new("{=Dramalord283}Hmm... Alright. I accept. I spare you this time if you perform well.");
            ConversationLines.player_choose_pleasure_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//new("{=Dramalord284}Well, your death is my sweetest pleasure.");
            return true;
        }
    }
}