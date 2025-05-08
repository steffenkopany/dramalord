﻿using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class GossipMarriageIntention : Intention
    {
        [SaveableField(4)]
        public readonly MarriageIntention EventIntention;

        [SaveableField(5)]
        public readonly List<Hero> Targets;

        [SaveableField(6)]
        public readonly bool IsWitness;

        public GossipMarriageIntention(MarriageIntention intention, bool isWitness, List<Hero> targets, Hero intentionHero, CampaignTime validUntil) : base(intentionHero, intentionHero, validUntil)
        {
            EventIntention = intention;
            Targets = targets;
            Targets.Add(intentionHero);
            IsWitness = isWitness;
            if (IsWitness && MBRandom.RandomInt(1, 100) > DramalordMCM.Instance.ChanceGossipingPlayer && !Targets.Contains(Hero.MainHero))
            {
                Targets.Add(Hero.MainHero);
            }
        }

        public override bool Action()
        {
            Hero target = IntentionHero.GetCloseHeroes().GetRandomElementWithPredicate(h => !Targets.Contains(h));

            if (target == Hero.MainHero)
            {
                if (IntentionHero.HasMet && (EventIntention.IntentionHero.HasMet || EventIntention.Target.HasMet) && IntentionHero.GetRelationTo(Hero.MainHero).Love > DramalordMCM.Instance.MaxTrustEnemies && ConversationTools.StartConversation(this, IntentionHero.CurrentSettlement != null))
                {
                    Targets.Add(Hero.MainHero);
                }
                return false;
            }
            else if(target != null)
            {
                if (target.CanBeJealousAboutRomance(EventIntention.IntentionHero, EventIntention.Target) || target.CanBeJealousAboutRomance(EventIntention.Target, EventIntention.IntentionHero))
                {
                    Targets.Add(target);
                    DramalordIntentions.Instance.GetIntentions().Add(new ConfrontMarriageIntention(EventIntention.IntentionHero, EventIntention.Target, target, CampaignTime.DaysFromNow(7), false));
                    DramalordIntentions.Instance.GetIntentions().Add(new ConfrontMarriageIntention(EventIntention.Target, EventIntention.IntentionHero, target, CampaignTime.DaysFromNow(7), false));
                }
                else
                {
                    DramalordIntentions.Instance.GetIntentions().Add(new GossipMarriageIntention(EventIntention, false, Targets, target, ValidUntil));
                }
                return false;
            }

            return !IsWitness;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow npcFlow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine("{npc_starts_confrontation_known}[ib:aggressive][if:convo_undecided_open]")
                    .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as GossipMarriageIntention != null)
                    .GotoDialogState("gossip_start_marriage");

            DialogFlow gossipFlow = DialogFlow.CreateDialogFlow("gossip_start_marriage")
                .BeginNpcOptions()
                    .NpcLine("{npc_gossip_married}[ib:aggressive][if:convo_excited]")
                        .Consequence(() =>
                        {
                            MarriageIntention gossip = (ConversationTools.ConversationIntention as GossipMarriageIntention).EventIntention;
                            HeroRelation relation = gossip.IntentionHero.GetRelationTo(gossip.Target);
                            if (!relation.IsKnownToPlayer)
                            {
                                relation.IsKnownToPlayer = true;
                                TextObject banner2 = new TextObject("{=Dramalord540}You have learned that {HERO} and {TARGET} are {RELATION}.");
                                banner2.SetTextVariable("HERO", gossip.IntentionHero.Name);
                                banner2.SetTextVariable("TARGET", gossip.Target.Name);
                                TextObject rel = new TextObject("{=Dramalord014}married");
                                banner2.SetTextVariable("RELATION", rel);//"{=Dramalord011}friends with benefits" "{=Dramalord012}lovers" "{=Dramalord014}married"
                                MBInformationManager.AddQuickInformation(banner2, 0, gossip.IntentionHero.CharacterObject, "event:/ui/notification/relation");
                            }

                            if (Hero.MainHero.IsEmotionalWith(gossip.IntentionHero) || Hero.MainHero.IsEmotionalWith(gossip.Target))
                            {
                                TextObject title = new TextObject("{=Dramalord557}React to gossip");
                                TextObject text = new TextObject("{=Dramalord558}You have heard some disturbing gossip about {HERO1} and {HERO2}. How will you react?");
                                text.SetTextVariable("HERO1", gossip.IntentionHero.Name);
                                text.SetTextVariable("HERO2", gossip.Target.Name);
                                InformationManager.ShowInquiry(
                                        new InquiryData(
                                            title.ToString(),
                                            text.ToString(),
                                            true,
                                            true,
                                            new TextObject("{=Dramalord559}Confront them!").ToString(),
                                            new TextObject("{=Dramalord560}Ignore it").ToString(),
                                            () => {
                                                if (Hero.MainHero.IsEmotionalWith(gossip.IntentionHero))
                                                {
                                                    ConfrontHeroQuest quest = new ConfrontHeroQuest(gossip.IntentionHero, gossip, CampaignTime.DaysFromNow(7));
                                                    quest.StartQuest();
                                                }

                                                if (Hero.MainHero.IsEmotionalWith(gossip.Target))
                                                {
                                                    ConfrontHeroQuest quest = new ConfrontHeroQuest(gossip.Target, gossip, CampaignTime.DaysFromNow(7));
                                                    quest.StartQuest();
                                                }
                                            },
                                            () => {
                                            }), true);
                            }
                        })
                        .BeginPlayerOptions()
                            .PlayerOption("{player_request_more_gossip}")
                                .GotoDialogState("npc_shares_gossip")
                            .PlayerOption("{player_interaction_abort}")
                                .GotoDialogState("hero_main_options")
                            .PlayerOption("{npc_challenge_summarize_end}..")
                                .Consequence(() => ConversationTools.EndConversation())
                                .CloseDialog()
                            .PlayerOption("{player_stop_bothering}")
                                .Consequence(() => { new ChangeOpinionIntention(Hero.OneToOneConversationHero, Hero.MainHero, (Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love > DramalordMCM.Instance.MaxTrustEnemies) ? DramalordMCM.Instance.MaxTrustEnemies - Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love : 0, 0, CampaignTime.Now).Action(); ConversationTools.EndConversation(); })
                                .CloseDialog()
                        .EndPlayerOptions();
                 
            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(gossipFlow);
        }

        public override void OnConversationEnded()
        {
            
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_gossip_married.SetTextVariable("HERO", EventIntention.IntentionHero.Name);
            ConversationLines.npc_gossip_married.SetTextVariable("OTHER", EventIntention.Target.Name);
            ConversationLines.npc_starts_confrontation_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Hero.MainHero, false));
            ConversationLines.player_interaction_abort.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, IntentionHero, false));
            ConversationLines.npc_challenge_summarize_end.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, IntentionHero, false));
        }
    }
}
