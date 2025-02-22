using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class ConfrontMarriageIntention : Intention
    {
        [SaveableField(4)]
        public readonly Hero Other;

        [SaveableField(5)]
        public readonly bool IsWitness;

        private static int _random = 0;

        private static int _answer = 0;
        private static bool _hasDenied = false;

        private static ConfrontMarriageIntention? ConversationInstance() => ConversationTools.ConversationIntention as ConfrontMarriageIntention;

        public ConfrontMarriageIntention(Hero target, Hero other, Hero intentionHero, CampaignTime validUntil, bool isWitness) : base(intentionHero, target, validUntil)
        {
            Other = other;
            IsWitness = isWitness;
        }

        public override bool Action()
        {
            List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
            _random = MBRandom.RandomInt(1, 100);
            _answer = 0;
            _hasDenied = false;

            if (Target == Hero.MainHero && closeHeroes.Contains(Hero.MainHero) && ConversationTools.StartConversation(this, IntentionHero.CurrentSettlement != null))
            {
                return true;
            }
            else if (Target != Hero.MainHero && closeHeroes.Contains(Target))
            {
                ConsequenceConfrontations(IntentionHero, Target);
                OnConversationEnded();
                return true;
            }
            return false;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow npcFlow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_starts_confrontation_unknown}[ib:aggressive][if:convo_undecided_open]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as ConfrontMarriageIntention != null && ConversationTools.ConversationIntention.IntentionHero != Hero.MainHero && !Hero.OneToOneConversationHero.HasMet)
                        .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                        .GotoDialogState("confront_marriage")
                    .NpcOption("{npc_starts_confrontation_known}[ib:aggressive][if:convo_undecided_open]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as ConfrontMarriageIntention != null && ConversationTools.ConversationIntention.IntentionHero != Hero.MainHero && Hero.OneToOneConversationHero.HasMet)
                        .GotoDialogState("confront_marriage")
                .EndNpcOptions();

            DialogFlow npcFlow2 = DialogFlow.CreateDialogFlow("confront_marriage")
                .BeginNpcOptions()
                    .NpcOption("{npc_confrontation_marriage}[ib:warrior][if:convo_grave]", () => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                        .GotoDialogState("confront_marriage2")
                    .NpcOption("{npc_confrontation_marriage_player}[ib:warrior][if:convo_grave]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                        .GotoDialogState("confront_marriage2")
                .EndNpcOptions();

            DialogFlow npcFlow3 = DialogFlow.CreateDialogFlow("confront_marriage2")
                .BeginPlayerOptions()
                    .PlayerOption("{npc_reply_confrontation_love}")
                        .Consequence(() => { _hasDenied = true; ConsequenceConfrontations(Hero.OneToOneConversationHero, Hero.MainHero); })
                        .GotoDialogState("confront_marriage3")
                    .PlayerOption("{npc_reply_confrontation_nocare}")
                        .Consequence(() => { _hasDenied = false; ConsequenceConfrontations(Hero.OneToOneConversationHero, Hero.MainHero); })
                        .GotoDialogState("confront_marriage3")
                    .PlayerOption("{npc_confrontation_reply_gossip}")
                        .Condition(() => !ConversationInstance().IsWitness)
                        .Consequence(() => { _hasDenied = true; ConsequenceConfrontations(Hero.OneToOneConversationHero, Hero.MainHero); })
                        .GotoDialogState("confront_marriage3")
                .EndPlayerOptions();

            DialogFlow npcFlow4 = DialogFlow.CreateDialogFlow("confront_marriage3")
                .BeginNpcOptions()
                    .NpcOption("{npc_confrontation_result_ok}[ib:nervous2][if:convo_confused_normal]", () => _answer == 0)
                        .Consequence(() => { ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_confrontation_result_break}[ib:nervous][if:convo_shocked]", () => _answer == 1 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism < _random)
                        .Consequence(() => { ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_confrontation_result_leave}[ib:aggressive][if:convo_furious]", () => _answer == 1 && Hero.OneToOneConversationHero.GetPersonality().Neuroticism >= _random)
                        .Consequence(() => { ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_confrontation_result_other}[ib:weary2][if:convo_nervous]", () => _answer == 2)
                        .Consequence(() => { ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_confrontation_gossip_accept}[ib:nervous2][if:convo_confused_normal]", () => _answer == 3)
                        .Consequence(() => ConversationTools.EndConversation())
                        .CloseDialog()
                .EndNpcOptions();



            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow2);
            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow3);
            Campaign.Current.ConversationManager.AddDialogFlow(npcFlow4);
        }

        private static bool IsStupid(Hero hero) => hero.GetHeroTraits().Calculating < 0 && hero.GetPersonality().Conscientiousness < _random && hero.GetPersonality().Agreeableness > _random;

        private static bool IsNaive(Hero hero) => hero.GetPersonality().Neuroticism < 0 && hero.GetPersonality().Agreeableness > 0 && hero.GetPersonality().Conscientiousness < 0;

        private static void ConsequenceConfrontations(Hero hero, Hero target)
        {
            if (_hasDenied)
            {
                if (ConversationInstance().IsWitness && IsStupid(hero))
                {
                    return;
                }
                if (!ConversationInstance().IsWitness && IsNaive(hero))
                {
                    _answer = 3;
                    return;
                }
            }

            RelationshipType current = hero.GetRelationTo(target).Relationship;

            RelationshipLossAction.Apply(hero, target, out int loveDamage, out int trustDamage, 150, 100);
            new ChangeOpinionIntention(hero, target, loveDamage, trustDamage, CampaignTime.Now).Action();

            if(target == Hero.MainHero && current == RelationshipType.None)
            {
                _answer = 2;
            }
            else if(target == Hero.MainHero && current != hero.GetRelationTo(target).Relationship)
            {
                _answer = 1;
            }
        }

        public override void OnConversationEnded()
        {
            IntentionHero.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_confrontation_unknown.SetTextVariable("HERO", ConversationTools.GetHeroGreeting(IntentionHero, Hero.MainHero, false));
            ConversationLines.npc_starts_confrontation_unknown.SetTextVariable("HERO", IntentionHero.Name);
            ConversationLines.npc_starts_confrontation_unknown.SetTextVariable("CLAN", IntentionHero.Clan != null ? IntentionHero.Clan.Name.ToString() : IntentionHero.Occupation.ToString());
            ConversationLines.npc_starts_confrontation_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Hero.MainHero, false));
            ConversationLines.npc_confrontation_marriage.SetTextVariable("HERO", Other.Name);
            ConversationLines.npc_confrontation_marriage.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(IntentionHero, Other));
            ConversationLines.npc_confrontation_marriage_player.SetTextVariable("HERO", Other.Name);
            ConversationLines.npc_confrontation_marriage_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(IntentionHero, Hero.MainHero)); 
            ConversationLines.npc_reply_confrontation_love.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));//"{=Dramalord184}Oh {TITLE}, I love you! I beg you, don't do anything you might regret later...");
            ConversationLines.npc_reply_confrontation_nocare.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));//"{=Dramalord185}Well, oh well, {TITLE}. So what now?");
            ConversationLines.npc_confrontation_reply_gossip.SetTextVariable("TITLE", ConversationTools.GetHeroRelation(Target, IntentionHero)); //"{=Dramalord538}Pay no mind to the chatter {TITLE}. It's just gossip and will be forgotten soon."
            ConversationLines.npc_confrontation_result_ok.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Hero.MainHero, false));
            ConversationLines.npc_confrontation_result_break.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Hero.MainHero, false));
            ConversationLines.npc_confrontation_result_leave.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Hero.MainHero, false));
            ConversationLines.npc_confrontation_gossip_accept.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Hero.MainHero, false));
        }
    }
}
