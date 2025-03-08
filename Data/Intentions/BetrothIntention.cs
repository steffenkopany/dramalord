using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications.Logs;
using Dramalord.Quests;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class BetrothIntention : Intention
    {
        internal static bool OtherMarriageModFound = false;

        private static bool _accepted = false;

        public BetrothIntention(Hero target, Hero intentionHero, CampaignTime validUntil, bool alwaysExecute = false) : base(intentionHero, target, validUntil)
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
                if(IntentionHero != Hero.MainHero && (IntentionHero.Father == Hero.MainHero || IntentionHero.Mother == Hero.MainHero || IntentionHero.Clan == Clan.PlayerClan ||
                    Target.Father == Hero.MainHero || Target.Mother == Hero.MainHero || Target.Clan == Clan.PlayerClan))
                {
                    int speed = (int)Campaign.Current.TimeControlMode;
                    Campaign.Current.SetTimeSpeed(0);
                    TextObject title = new TextObject("{=Dramalord566}Marriage Request");
                    TextObject text = new TextObject("{=Dramalord567}{HERO1} and {HERO2} would like to marry. Do they have your blessing?");
                    text.SetTextVariable("HERO1", IntentionHero.Name);
                    text.SetTextVariable("HERO2", Target.Name);
                    InformationManager.ShowInquiry(
                            new InquiryData(
                                title.ToString(),
                                text.ToString(),
                                true,
                                true,
                                GameTexts.FindText("str_yes").ToString(),
                                GameTexts.FindText("str_no").ToString(),
                                () => {
                                    _accepted = true;
                                    OnConversationEnded();
                                },
                                () => { return; }), true);

                    return true;
                }
                else
                {
                    _accepted = true;
                    OnConversationEnded();
                    return true;
                }
            }

            return false;
        }

        public override void OnConversationEnded()
        {
            IntentionHero.GetRelationTo(Target).LastInteraction = CampaignTime.Now;

            if (_accepted)
            {
                HeroRelation relation = IntentionHero.GetRelationTo(Target);

                StartRelationshipAction.Apply(IntentionHero, Target, relation, RelationshipType.Betrothed);

                if (IntentionHero == Hero.MainHero || Target == Hero.MainHero)
                {
                    Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                    TextObject banner = new TextObject("{=Dramalord078}You are engaged to {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                    MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
                }

                if (DramalordMCM.Instance.RelationshipLogs)
                {
                    LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Betrothed));
                }

                if (MBRandom.RandomInt(1, 100) < DramalordMCM.Instance?.ChanceGettingCaught)
                {
                    List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
                    Hero? witness = DramalordMCM.Instance.PlayerAlwaysWitness && Target != Hero.MainHero && closeHeroes.Contains(Hero.MainHero) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h != Target);
                    if (witness != null)
                    {
                        if (witness != Hero.MainHero && (witness.IsEmotionalWith(IntentionHero) || witness.IsEmotionalWith(Target)))
                        {
                            DramalordIntentions.Instance.GetIntentions().Add(new ConfrontBetrothedIntention(IntentionHero, Target, witness, CampaignTime.DaysFromNow(7), true));
                            DramalordIntentions.Instance.GetIntentions().Add(new ConfrontBetrothedIntention(Target, IntentionHero, witness, CampaignTime.DaysFromNow(7), true));
                        }
                        else if (witness != Hero.MainHero)
                        {
                            if(witness.GetHeroTraits().Mercy < 0 && witness.GetRelation(IntentionHero) < DramalordMCM.Instance.MinTrustFriends && witness.GetRelation(Target) < DramalordMCM.Instance.MinTrustFriends)
                            {
                                DramalordIntentions.Instance.GetIntentions().Add(new BlackmailBetrothedIntention(this, witness, IntentionHero, CampaignTime.DaysFromNow(7)));
                                DramalordIntentions.Instance.GetIntentions().Add(new BlackmailBetrothedIntention(this, witness, Target, CampaignTime.DaysFromNow(7)));
                            }
                            else
                            {
                                List<Hero> targets = new() { IntentionHero, Target };
                                DramalordIntentions.Instance.GetIntentions().Add(new GossipBetrothedIntention(this, true, targets, witness, CampaignTime.DaysFromNow(7)));
                            }
                        }

                        if (witness == Hero.MainHero)
                        {
                            TextObject banner = new TextObject("{=Dramalord215}You saw {HERO.LINK} proposing to {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", Target.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");

                            if(!relation.IsKnownToPlayer)
                            {
                                relation.IsKnownToPlayer = true;
                                TextObject banner2 = new TextObject("{=Dramalord540}You have learned that {HERO} and {TARGET} are {RELATION}.");
                                banner2.SetTextVariable("HERO", IntentionHero.Name);
                                banner2.SetTextVariable("TARGET", Target.Name);
                                banner2.SetTextVariable("RELATION", new TextObject("{=Dramalord013}engaged"));//"{=Dramalord011}friends with benefits" "{=Dramalord012}lovers" "{=Dramalord014}married"
                                MBInformationManager.AddQuickInformation(banner2, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
                            }

                            if (Hero.MainHero.IsEmotionalWith(IntentionHero) || Hero.MainHero.IsEmotionalWith(Target))
                            {
                                int speed = (int)Campaign.Current.TimeControlMode;
                                Campaign.Current.SetTimeSpeed(0);
                                TextObject title = new TextObject("{=Dramalord579}Interrupt {HERO1} and {HERO2} in the act");
                                TextObject text = new TextObject("{=Dramalord580}You caught {HERO1} and {HERO2} in the act. Do you wish to interrupt them?");
                                title.SetTextVariable("HERO1", IntentionHero.Name);
                                title.SetTextVariable("HERO2", Target.Name);
                                text.SetTextVariable("HERO1", IntentionHero.Name);
                                text.SetTextVariable("HERO2", Target.Name);
                                InformationManager.ShowInquiry(
                                        new InquiryData(
                                            title.ToString(),
                                            text.ToString(),
                                            true,
                                            true,
                                            GameTexts.FindText("str_yes").ToString(),
                                            GameTexts.FindText("str_no").ToString(),
                                            () => { new ConfrontationPlayerIntention(this, Hero.MainHero.IsEmotionalWith(IntentionHero) ? IntentionHero : Target, CampaignTime.Now).Action(); },
                                            () => { Campaign.Current.SetTimeSpeed(speed); }), true);
                            }
                        }
                        else if (IntentionHero == Hero.MainHero || Target == Hero.MainHero)
                        {
                            Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                            TextObject banner = new TextObject("{=Dramalord216}{HERO.LINK} saw you proposing to {TARGET.LINK}.");
                            StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                            StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, banner);
                            MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                        }
                    }
                }
            }
            else
            {
                RelationshipLossAction.Apply(IntentionHero, Target, out int loveDamage, out int trustDamage, 5, 5);
                new ChangeOpinionIntention(IntentionHero, Target, loveDamage, trustDamage, CampaignTime.Now).Action();
            }

            _accepted = false;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_starts_interaction_unknown}[ib:nervous][if:convo_nervous]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as BetrothIntention != null && !Hero.OneToOneConversationHero.HasMet)
                        .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                        .BeginPlayerOptions()
                            .PlayerOption("{player_interaction_start_react_yes}")
                                .GotoDialogState("start_engagement")
                            .PlayerOption("{player_interaction_start_react_no}")
                                .Consequence(() => ConversationTools.EndConversation())
                                .CloseDialog()
                        .EndPlayerOptions()
                    .NpcOption("{npc_starts_interaction_known}[ib:nervous2][if:convo_confused_normal]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as BetrothIntention != null && Hero.OneToOneConversationHero.HasMet)
                        .BeginPlayerOptions()
                            .PlayerOption("{player_interaction_start_react_yes}")
                                .GotoDialogState("start_engagement")
                            .PlayerOption("{player_interaction_start_react_no}")
                                .Consequence(() => ConversationTools.EndConversation())
                                .CloseDialog()
                        .EndPlayerOptions()
                .EndNpcOptions();


            DialogFlow engageflow = DialogFlow.CreateDialogFlow("start_engagement")
                .NpcLineWithVariation("{npc_interaction_betrothed_1}[ib:confident3][if:convo_excited]")
                    .Variation("{npc_interaction_betrothed_2}[ib:confident3][if:convo_excited]")
                    .Condition(() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                    .BeginPlayerOptions()
                        .PlayerOption("{player_reaction_engagement_yes}")
                            .Condition(() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                            .Consequence(() => _accepted = true)
                            .BeginNpcOptions()
                                .NpcOption("{player_quest_marriage_start}[ib:aggressive][if:convo_delighted]", () => Hero.OneToOneConversationHero.Father != null || Hero.OneToOneConversationHero.Clan?.Leader != Hero.MainHero || Hero.OneToOneConversationHero.Clan?.Leader != Hero.OneToOneConversationHero)
                                    .BeginPlayerOptions()
                                        .PlayerOption("{npc_as_you_wish_reply}")
                                            .Consequence(() =>
                                            {
                                                MarriagePermissionQuest quest = new MarriagePermissionQuest(Hero.OneToOneConversationHero, CampaignTime.DaysFromNow(21));
                                                quest.StartQuest();
                                                DramalordQuests.Instance.AddQuest(Hero.OneToOneConversationHero, quest);
                                                ConversationTools.EndConversation();
                                            })
                                        .CloseDialog()
                                        .PlayerOption("{player_reaction_no}")
                                            .Consequence(() => { _accepted = false; ConversationTools.EndConversation(); })
                                            .CloseDialog()
                                    .EndPlayerOptions()
                                .NpcOption("{player_reaction_engagement_yes}[ib:aggressive][if:convo_delighted]", () => Hero.OneToOneConversationHero.Father == null && Hero.OneToOneConversationHero.Clan == null || Hero.OneToOneConversationHero.Clan == Clan.PlayerClan )
                                    .Consequence(() => { new BetrothIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now).Action(); ConversationTools.EndConversation(); })
                                    .CloseDialog()
                            .EndNpcOptions()
                        .PlayerOption("{player_reaction_no}")
                            .Consequence(() => { _accepted = false; ConversationTools.EndConversation(); })
                            .CloseDialog()
                    .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
            Campaign.Current.ConversationManager.AddDialogFlow(engageflow);
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_interaction_unknown.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time."
            ConversationLines.npc_starts_interaction_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, true));// "{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?");
            ConversationLines.player_interaction_start_react_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord029}Of course {TITLE}. How can I be of service?");
            ConversationLines.player_interaction_start_react_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
            ConversationLines.npc_interaction_betrothed_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord051}I love you very much, {TITLE}. I think it's time for us to take the next step in our relationship. Will you marry me?");
            ConversationLines.npc_interaction_betrothed_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord052}You know, {TITLE}, I love you deeply, and I know that you are the only one for me. Will you marry me?");
            ConversationLines.player_reaction_engagement_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord065}You make my dream come true, {TITLE}. Yes I would love to marry you!");
            ConversationLines.player_reaction_engagement_instant_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord066}We're at a settlement, {TITLE}. Let's marry right now!");
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
        }
    }
}
