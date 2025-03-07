using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Dramalord.Quests;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Dramalord.Conversations
{
    internal static class ConversationRelationship
    {
        private static bool Timeout() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow < DramalordMCM.Instance.DaysBetweenInteractions;

        private static bool SpouseAway() => Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero || !Hero.OneToOneConversationHero.IsCloseTo(Hero.OneToOneConversationHero.Spouse);

        private static bool HasOtherSpouse() => Hero.OneToOneConversationHero.Spouse != null && !Hero.OneToOneConversationHero.IsSpouseOf(Hero.MainHero);

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow startFlow = DialogFlow.CreateDialogFlow("hero_main_options")
                .BeginPlayerOptions()
                    .PlayerOption("{player_approach_start}")
                        .Condition(() =>
                        {
                            if (Hero.OneToOneConversationHero.IsDramalordLegit() && !Hero.OneToOneConversationHero.IsPrisoner && !Hero.MainHero.IsPrisoner)
                            {
                                SetupLines();
                                return true;
                            }
                            return false;
                        })
                    .BeginNpcOptions()
                        .NpcOption("{player_interaction_start_react_yes}[ib:normal2][if:convo_calm_friendly]", () => Hero.OneToOneConversationHero.GetRelationWithPlayer() > -30)
                            .GotoDialogState("player_interaction_selection")
                        .NpcOption("{player_interaction_start_react_no}[ib:closed][if:convo_bored]", () => Hero.OneToOneConversationHero.GetRelationWithPlayer() <= -30)
                            .Consequence(() => ConversationTools.EndConversation())
                            .CloseDialog()
                    .EndNpcOptions()
                .EndPlayerOptions();

            DialogFlow playerSelectionFlow = DialogFlow.CreateDialogFlow("player_interaction_selection")
                .BeginPlayerOptions()
                    .PlayerOption("{player_interaction_talk}")
                        .GotoDialogState("npc_interaction_reply_talk")
                    .PlayerOption("{player_interaction_flirt}")
                        .GotoDialogState("npc_interaction_reply_flirt")
                    .PlayerOption("{player_interaction_date_first}")
                        .GotoDialogState("npc_interaction_reply_date")
                        .Condition(() => Hero.MainHero.IsFriendOf(Hero.OneToOneConversationHero) || Hero.MainHero.IsFriendWithBenefitsOf(Hero.OneToOneConversationHero))
                    .PlayerOption("{player_interaction_date}")
                        .GotoDialogState("npc_interaction_reply_date")
                        .Condition(() => Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero))
                    .PlayerOption("{player_interaction_sex_friend}")
                        .GotoDialogState("npc_interaction_reply_sex")
                        .Condition(() => Hero.MainHero.IsFriendOf(Hero.OneToOneConversationHero))
                    .PlayerOption("{player_interaction_sex_fwb}")
                        .GotoDialogState("npc_interaction_reply_sex")
                        .Condition(() => Hero.MainHero.IsFriendWithBenefitsOf(Hero.OneToOneConversationHero))
                    .PlayerOption("{player_interaction_sex}")
                        .GotoDialogState("npc_interaction_reply_sex")
                        .Condition(() => Hero.MainHero.IsSpouseOf(Hero.OneToOneConversationHero))
                    .PlayerOption("{player_quest_joinparty_start}")
                        .Condition(() => !Timeout() && Hero.OneToOneConversationHero.PartyBelongedTo == null && Hero.OneToOneConversationHero.CurrentSettlement != null && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) == null)
                        .BeginNpcOptions()
                            .NpcOption("{npc_interaction_reply_flirt_yes_1}", () => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove && Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFriends)
                                .Consequence(() =>
                                {
                                    JoinPlayerQuest quest = new JoinPlayerQuest(Hero.OneToOneConversationHero, CampaignTime.DaysFromNow(7));
                                    quest.StartQuest();
                                    DramalordQuests.Instance.AddQuest(Hero.OneToOneConversationHero, quest);
                                    ConversationTools.EndConversation();
                                })
                                .CloseDialog()
                            .NpcOption("{player_reaction_no}", () => Timeout() || DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) != null || Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove || Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) < DramalordMCM.Instance.MinTrustFriends)
                                .GotoDialogState("player_interaction_selection")
                        .EndNpcOptions()
                    .PlayerOption("{player_interaction_engage}")
                        .GotoDialogState("npc_interaction_reply_engage")
                        .Condition(() => !BetrothIntention.OtherMarriageModFound && Hero.MainHero.IsLoverOf(Hero.OneToOneConversationHero) && DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) == null)      
                    .PlayerOption("{player_interaction_marry}")
                        .GotoDialogState("npc_interaction_reply_marriage")
                        .Condition(() => !BetrothIntention.OtherMarriageModFound && Hero.MainHero.IsBetrothedOf(Hero.OneToOneConversationHero) && Hero.MainHero.CurrentSettlement != null && Hero.MainHero.CurrentSettlement == Hero.OneToOneConversationHero.CurrentSettlement)
                    .PlayerOption("{player_interaction_breakup}")
                        .GotoDialogState("npc_interaction_reply_breakup")
                        .Condition(() => Hero.MainHero.IsSexualWith(Hero.OneToOneConversationHero))
                    .PlayerOption("{player_interaction_gift}")
                        .GotoDialogState("npc_interaction_reply_gift")
                        .Condition(() =>
                        {
                            ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
                            ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
                            if (Hero.OneToOneConversationHero.IsFemale && Hero.MainHero.PartyBelongedTo != null)
                            {
                                if (Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && !Hero.OneToOneConversationHero.GetDesires().HasToy && Hero.MainHero.PartyBelongedTo.ItemRoster.FindIndexOfItem(wurst) >= 0)
                                {
                                    ConversationLines.player_interaction_gift.SetTextVariable("GIFT", new TextObject("{=Dramalord240}Sausage")); 
                                    return true;
                                }
                                return false;
                            }
                            else if (Hero.MainHero.PartyBelongedTo != null)
                            {
                                if (Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && !Hero.OneToOneConversationHero.GetDesires().HasToy && Hero.MainHero.PartyBelongedTo.ItemRoster.FindIndexOfItem(pie) >= 0)
                                {
                                    ConversationLines.player_interaction_gift.SetTextVariable("GIFT", new TextObject("{=Dramalord241}Pie"));
                                    return true;
                                }
                                return false;
                            }
                            return false;
                        })
                    /*.PlayerOption("{player_interaction_adopt}")
                        .GotoDialogState("npc_interaction_reply_adopt")
                        .Condition(() => Hero.OneToOneConversationHero.Children.FirstOrDefault(child => child.Clan == Clan.PlayerClan && child.Occupation == Occupation.Wanderer) != null)*/
                    .PlayerOption("{player_interaction_kick_clan}")
                        .GotoDialogState("npc_interaction_reply_kick_clan")
                        .Condition(() => Hero.OneToOneConversationHero.Clan == Hero.MainHero.Clan)
                    /*.PlayerOption("{player_interaction_kick_kingdom}")
                        .GotoDialogState("npc_interaction_reply_kick_kingdom")
                        .Condition(() => Hero.OneToOneConversationHero.Clan != null && Hero.MainHero.Clan.Kingdom != null && Hero.MainHero.IsKingdomLeader && Hero.OneToOneConversationHero.Clan.Kingdom == Hero.MainHero.Clan.Kingdom && Hero.OneToOneConversationHero.Clan != Hero.MainHero.Clan)*/
                    .PlayerOption("{player_info_ask}")
                        .GotoDialogState("npc_interaction_reply_ask")
                        .Condition(() => !Hero.OneToOneConversationHero.GetDesires().IsKnowToPlayer)
                    .PlayerOption("{player_interaction_abort}")
                        .GotoDialogState("npc_interaction_abort")
                .EndPlayerOptions();

            DialogFlow abortFlow = DialogFlow.CreateDialogFlow("npc_interaction_abort")
                .NpcLine("{npc_interaction_abort}")
                    .GotoDialogState("hero_main_options");

            DialogFlow talkFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_talk")
                .BeginNpcOptions()
                    .NpcOptionWithVariation("{npc_interaction_reply_talk_1}[ib:normal][if:convo_calm_friendly]", () => !Timeout())
                        .Variation("{npc_interaction_reply_talk_2}[ib:normal][if:convo_calm_friendly]")
                        .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Chat, 1))
                        .GotoDialogState("start_challenge")
                    .NpcOption("{npc_interaction_reply_timeout}[ib:closed][if:convo_bored]", () => Timeout())
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow flirtFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_flirt")
                .BeginNpcOptions()
                    .NpcOptionWithVariation("{npc_interaction_reply_flirt_yes_1}[ib:normal2][if:convo_mocking_teasing]", () => (Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) >= DramalordMCM.Instance.MinAttraction) && !Timeout())
                        .Variation("{npc_interaction_reply_flirt_yes_2}[ib:normal2][if:convo_mocking_teasing]")
                        .Consequence(() => {
                            ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Flirt, 1); 
                        })
                        .GotoDialogState("start_challenge")
                    .NpcOption("{npc_interaction_reply_timeout}[ib:closed][if:convo_bored]", () => Timeout())
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{player_reaction_no}[ib:nervous][if:convo_shocked]", () => !Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) < DramalordMCM.Instance.MinAttraction && !Timeout())
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow dateFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_date")
                .BeginNpcOptions()
                    .NpcOptionWithVariation("{npc_interaction_reply_date_first_yes_1}[ib:confident3][if:convo_excited]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                        .Variation("{npc_interaction_reply_date_first_yes_2}[ib:confident3][if:convo_excited]")
                        .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Date, 3))
                        .GotoDialogState("start_challenge")
                    .NpcOptionWithVariation("{npc_interaction_reply_date_yes_1}[ib:demure2][if:convo_merry]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                        .Variation("{npc_interaction_reply_date_yes_2}[ib:demure2][if:convo_merry]")
                        .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Date, 3))
                        .GotoDialogState("start_challenge")
                    .NpcOptionWithVariation("{npc_interaction_date_married_1}[ib:demure2][if:convo_mocking_teasing]", () => SpouseAway() && HasOtherSpouse() && !Timeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                        .Variation("{npc_interaction_date_married_2}[ib:demure2][if:convo_mocking_teasing]")
                        .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.Context.Date, 3))
                        .GotoDialogState("start_challenge")
                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () => !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love > 0 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove)
                        .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForDate())
                        .GotoDialogState("npc_persuasion_challenge")
                    .NpcOption("{npc_interaction_reply_husband}[ib:nervous][if:convo_shocked]", () => !SpouseAway() && HasOtherSpouse() && !Timeout() && (Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_interaction_reply_timeout}[ib:closed][if:convo_bored]", () => Timeout())
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{player_reaction_no}[ib:nervous][if:convo_shocked]", () => !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love <= 0)
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow sexFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_sex")
                .BeginNpcOptions()
                    .NpcOption("{player_reaction_sex_friend_yes}[ib:confident3][if:convo_excited]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= DramalordMCM.Instance.MinTrustFWB && Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFWB)
                        .Consequence(() => { ConversationTools.ConversationIntention = new IntercourseIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, true); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () => !Timeout() && Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFWB / 2 && Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) < DramalordMCM.Instance.MinTrustFWB)
                        .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForFWB())
                        .GotoDialogState("npc_persuasion_challenge")
                    .NpcOptionWithVariation("{npc_interaction_sex_friend_wb_1}[ib:confident2][if:convo_focused_happy]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= DramalordMCM.Instance.MinTrustFWB / 2)
                        .Variation("{npc_interaction_sex_friend_wb_2}[ib:confident2][if:convo_focused_happy]")
                        .Consequence(() => { ConversationTools.ConversationIntention = new IntercourseIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, true); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_interaction_reply_sex_fwb_no}[ib:normal2][if:convo_confused_annoyed]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny < DramalordMCM.Instance.MinTrustFWB / 2)
                        .GotoDialogState("player_interaction_selection")
                    .NpcOptionWithVariation("{npc_interaction_sex_else_1}[ib:confident3][if:convo_excited]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 20)
                        .Variation("{npc_interaction_sex_else_2}[ib:confident3][if:convo_excited]")
                        .Consequence(() => { ConversationTools.ConversationIntention = new IntercourseIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, true); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOptionWithVariation("{npc_interaction_sex_married_1}[ib:confident3][if:convo_excited]", () => SpouseAway() && HasOtherSpouse() && !Timeout() && Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny >= 20)
                        .Variation("{npc_interaction_sex_married_2}[ib:confident3][if:convo_excited]")
                        .Consequence(() => { ConversationTools.ConversationIntention = new IntercourseIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, true); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_interaction_reply_husband}[ib:nervous][if:convo_shocked]", () => !SpouseAway() && HasOtherSpouse() && !Timeout() && (Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero) || (Hero.OneToOneConversationHero.IsFriendOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFWB)))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_interaction_reply_timeout}[ib:closed][if:convo_bored]", () => Timeout())
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_interaction_reply_sex_fwb_no}[ib:normal2][if:convo_confused_annoyed]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsFriendWithBenefitsOf(Hero.MainHero) && Hero.OneToOneConversationHero.GetDesires().Horny < DramalordMCM.Instance.MinTrustFWB / 2)
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_interaction_reply_sex_no_interest}[ib:normal2][if:convo_confused_annoyed]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && (!Hero.OneToOneConversationHero.IsSexualWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetDesires().Horny < DramalordMCM.Instance.MinTrustFWB / 2))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow engageFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_engage")
                .BeginNpcOptions()  
                    .NpcOption("{player_reaction_engagement_yes}[ib:aggressive][if:convo_delighted]", () =>
                        Hero.OneToOneConversationHero.Spouse == null && (Hero.OneToOneConversationHero.Father == null || !Hero.OneToOneConversationHero.Father.IsAlive || Hero.OneToOneConversationHero.Father == Hero.MainHero) && (Hero.OneToOneConversationHero.Clan == null || Hero.OneToOneConversationHero.Clan == Clan.PlayerClan || Hero.OneToOneConversationHero.Clan?.Leader == Hero.OneToOneConversationHero))
                        .Consequence(() => { new BetrothIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, true).OnConversationEnded(); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{player_quest_marriage_start}[ib:aggressive][if:convo_delighted]", () => Hero.OneToOneConversationHero.Spouse == null &&
                        ((Hero.OneToOneConversationHero.Father != null && Hero.OneToOneConversationHero.Father.IsAlive && Hero.OneToOneConversationHero.Father != Hero.MainHero) || (Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan?.Leader != Hero.MainHero && Hero.OneToOneConversationHero.Clan?.Leader != Hero.OneToOneConversationHero)))
                        .BeginPlayerOptions()
                            .PlayerOption("{npc_as_you_wish_reply}")
                            .Condition(() => { ConversationLines.npc_as_you_wish_reply.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false)); return true; })
                                .Consequence(() =>
                                {
                                    MarriagePermissionQuest quest = new MarriagePermissionQuest(Hero.OneToOneConversationHero, CampaignTime.DaysFromNow(21));
                                    quest.StartQuest();
                                    DramalordQuests.Instance.AddQuest(Hero.OneToOneConversationHero, quest);
                                    ConversationTools.EndConversation();
                                })
                                .CloseDialog()
                            .PlayerOption("{player_interaction_abort}")
                                .GotoDialogState("npc_interaction_abort")
                        .EndPlayerOptions()
                    .NpcOption("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]", () =>
                            Hero.OneToOneConversationHero.Spouse == null && !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinMarriageLove &&
                            Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove)
                        .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForEngage())
                        .GotoDialogState("npc_persuasion_challenge")
                    .NpcOption("{npc_interaction_betrothed_married}[ib:nervous][if:convo_shocked]", () => Hero.OneToOneConversationHero.Spouse != null && !Timeout())
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_interaction_reply_engage_no}[ib:normal2][if:convo_confused_annoyed]", () => Hero.OneToOneConversationHero.Spouse == null && !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove)
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_interaction_reply_timeout}[ib:closed][if:convo_bored]", () => Timeout())
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow marryFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_marriage")
                .BeginNpcOptions()
                    .NpcOption("{player_reaction_marry_yes}[ib:aggressive][if:convo_delighted]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinMarriageLove)
                        .Consequence(() => { ConversationTools.ConversationIntention = new MarriageIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_interaction_reply_engage_no}[ib:normal2][if:convo_confused_annoyed]", () => SpouseAway() && !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinMarriageLove)
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_interaction_reply_timeout}[ib:closed][if:convo_bored]", () => Timeout())
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow breakupFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_breakup")
                .BeginNpcOptions()
                    .NpcOption("{npc_interaction_breakup_love}[ib:nervous][if:convo_shocked]", () => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove && Hero.OneToOneConversationHero.GetPersonality().Neuroticism < 50)
                        .Consequence(() => { new ChangeOpinionIntention(Hero.MainHero, Hero.OneToOneConversationHero, MathF.Max(0, Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love) * -1, MathF.Max(0, Hero.MainHero.GetTrust(Hero.OneToOneConversationHero)) * -1, CampaignTime.Now).Action(); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{player_reaction_breakup_accept}[ib:closed2][if:convo_bored2]", () => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove)
                        .Consequence(() => { new ChangeOpinionIntention(Hero.MainHero, Hero.OneToOneConversationHero, MathF.Max(0, Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love) * -1, MathF.Max(0, Hero.MainHero.GetTrust(Hero.OneToOneConversationHero)) * -1, CampaignTime.Now).Action(); ConversationTools.EndConversation(); })
                        .CloseDialog()
                    .NpcOption("{npc_confrontation_result_leave}[ib:warrior][if:convo_grave]", () => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove && Hero.OneToOneConversationHero.GetPersonality().Neuroticism >= 50)
                        .Consequence(() => { new ChangeOpinionIntention(Hero.MainHero, Hero.OneToOneConversationHero, MathF.Max(0, Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love) * -1, MathF.Max(0, Hero.MainHero.GetTrust(Hero.OneToOneConversationHero)) * -1, CampaignTime.Now).Action();ConversationTools.ConversationIntention = new LeaveClanToJoinOtherIntention(Hero.OneToOneConversationHero, CampaignTime.Now); ConversationTools.EndConversation(); })
                        .CloseDialog()
                .EndNpcOptions();

            DialogFlow askFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_ask")
                .BeginNpcOptions()
                    .NpcOption("{npc_info_reply_ask_deny}[ib:hip][if:convo_annoyed]", () => Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) < DramalordMCM.Instance.MinTrustFriends)
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption("{npc_info_reply_ask_accept}[ib:normal2][if:convo_calm_friendly]", () => Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFriends)
                        .GotoDialogState("npc_interaction_reply_orientation")
                .EndNpcOptions();

            DialogFlow answerOrientationFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_orientation")
                .BeginNpcOptions()
                    .NpcOption("{npc_info_reply_orientation_hetero}[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return (o2o.IsFemale) ? desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen < DramalordMCM.Instance.MinAttraction
                            : desires.AttractionMen < DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                    .NpcOption("{npc_info_reply_orientation_hetero_gay}[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return (o2o.IsFemale) ? desires.AttractionMen < DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction
                            : desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen < DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                    .NpcOption("{npc_info_reply_orientation_bi}[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                    .NpcOption("{npc_info_reply_orientation_none}[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return desires.AttractionMen < DramalordMCM.Instance.MinAttraction && desires.AttractionWomen < DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                .EndNpcOptions();

            DialogFlow answerWeightFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_weight")
                .BeginNpcOptions()
                    .NpcOption("{npc_info_reply_weight_thin}[ib:demure][if:happy]", () => Hero.OneToOneConversationHero.GetDesires().AttractionWeight <= 33)
                    .GotoDialogState("npc_interaction_reply_build")
                    .NpcOption("{npc_info_reply_weight_fat}[ib:demure][if:happy]", () => Hero.OneToOneConversationHero.GetDesires().AttractionWeight >= 66)
                    .GotoDialogState("npc_interaction_reply_build")
                    .NpcOption("{npc_info_reply_weight_normal}[ib:demure][if:happy]", () => Hero.OneToOneConversationHero.GetDesires().AttractionWeight > 33 && Hero.OneToOneConversationHero.GetDesires().AttractionWeight < 66)
                    .GotoDialogState("npc_interaction_reply_build")
                .EndNpcOptions();

            DialogFlow answerBuildFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_build")
                .BeginNpcOptions()
                    .NpcOption("{npc_info_reply_build_low}[ib:confident][if:convo_excited]", () => Hero.OneToOneConversationHero.GetDesires().AttractionBuild <= 33)
                    .GotoDialogState("npc_interaction_reply_age")
                    .NpcOption("{npc_info_reply_build_high}[ib:confident][if:convo_excited]", () => Hero.OneToOneConversationHero.GetDesires().AttractionBuild >= 66)
                    .GotoDialogState("npc_interaction_reply_age")
                    .NpcOption("{npc_info_reply_build_average}[ib:confident][if:convo_excited]", () => Hero.OneToOneConversationHero.GetDesires().AttractionBuild > 33 && Hero.OneToOneConversationHero.GetDesires().AttractionWeight < 66)
                    .GotoDialogState("npc_interaction_reply_age")
                .EndNpcOptions();

            DialogFlow answerAgeFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_age")
                .NpcLine("{npc_info_reply_age}[ib:normal2][if:convo_calm_friendly]")
                .NpcLine("{npc_info_reply_summary}[ib:demure][if:convo_bemused]")
                    .Consequence(() =>
                    {
                        Hero.OneToOneConversationHero.GetDesires().IsKnowToPlayer = true;
                        TextObject banner = new TextObject("{=Dramalord336}You learned the physical preferences of {HERO.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                    })
                .GotoDialogState("player_interaction_selection");


            DialogFlow giftFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_gift")
                .NpcLine("{npc_interaction_reply_gift}[ib:normal2][if:convo_mocking_teasing]")
                .Consequence(() =>
                {
                    TextObject toy = new TextObject();
                    if (Hero.OneToOneConversationHero.IsFemale)
                    {
                        ItemObject wurst = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_sausage");
                        Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(wurst, -1);
                        Hero.OneToOneConversationHero.GetDesires().HasToy = true;
                        toy = new TextObject("{=Dramalord240}Sausage");
                    }
                    else
                    {
                        ItemObject pie = MBObjectManager.Instance.GetObject<ItemObject>("dramalord_pie");
                        Hero.MainHero.PartyBelongedTo.ItemRoster.AddToCounts(pie, -1);
                        Hero.OneToOneConversationHero.GetDesires().HasToy = true;
                        toy = new TextObject("{=Dramalord241}Pie");
                    }

                    TextObject banner = new TextObject("{=Dramalord294}You gave {HERO.LINK} a {TOY}.");
                    StringHelpers.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, banner);
                    banner.SetTextVariable("TOY", toy);
                    MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                })
                .GotoDialogState("player_interaction_selection");

            

            Campaign.Current.ConversationManager.AddDialogFlow(startFlow); 
            Campaign.Current.ConversationManager.AddDialogFlow(playerSelectionFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(abortFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(talkFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(flirtFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(dateFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(sexFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(engageFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(marryFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(breakupFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(askFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(answerOrientationFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(answerWeightFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(answerBuildFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(answerAgeFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(giftFlow);
        }

        internal static void SetupLines()
        {
            ConversationLines.player_approach_start.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, true));// ""{=Dramalord100}{TITLE}, may I occupy a few minutes of your time?");
            ConversationLines.player_interaction_start_react_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord029}Of course {TITLE}. How can I be of service?");
            ConversationLines.player_interaction_start_react_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");

            ConversationLines.player_interaction_talk.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord339}I know you have good stories, tell me some of them! (Friendly chat)");
            ConversationLines.player_interaction_flirt.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord340}You look amazing, but I'm sure you already know that... (Flirting)");
            ConversationLines.player_interaction_date_first.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord341}I must confess I have feelings for you, {TITLE}! (Love Affair)");
            ConversationLines.player_interaction_date.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord342}There's nothing I'd like more than to be with you and only you... (Dating)");
            ConversationLines.player_interaction_sex_friend.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord343}Do you urge for pleaseure as well, {TITLE}? (Friend with benefits)");
            ConversationLines.player_interaction_sex_fwb.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord344}Would you care for some bed excercise, {TITLE}? (Intimacy)");
            ConversationLines.player_interaction_sex.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord345}I want you. I know you feel the same way. Meet me in my chambers. (Intimacy)");
            ConversationLines.player_interaction_engage.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord346}I love you very much, {TITLE}. Will you marry me? (Betrothed)");
            ConversationLines.player_interaction_marry.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord347}We are in a settlement, {TITLE}, let's  get married! (Marriage)");
            ConversationLines.player_interaction_breakup.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));// "{=Dramalord348}It's just not working between you and I. Let's end this farce before it festers. (Break up)");
            ConversationLines.player_interaction_abort.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//"{=Dramalord101}Let's talk about something else, {TITLE}.");
            ConversationLines.player_interaction_gift.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, true));//"{=Dramalord292}{TITLE}, let me give you this exceptional {GIFT} as a token of my affection.");
            ConversationLines.player_info_ask.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, true));//"{=Dramalord318}{TITLE} I always wondered what looks you prefer. (Information)");

            ConversationLines.npc_interaction_reply_timeout.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false)); // "{=Dramalord105}Again? Give me some rest, {TITLE}. Let's talk about it later."
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
            ConversationLines.npc_interaction_reply_uhwell.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord438}Uh... well...");
            ConversationLines.npc_interaction_reply_husband.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord102}Apologies, {TITLE}, but my spouse is around I we can not risk it.");

            ConversationLines.npc_interaction_reply_talk_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord031}I always like to hear new stories. Tell me about your latest exploits while traveling in the realm.");
            ConversationLines.npc_interaction_reply_talk_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord032}I would like to hear your opinion of a certain matter which is occupying my mind for a while.");
            ConversationLines.npc_interaction_reply_flirt_yes_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord033}I have to say I really like your smile. Would you mind telling me more about yourself?");
            ConversationLines.npc_interaction_reply_flirt_yes_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord034}Would you mind going for a walk with me? I want to cause jealousy with your outstanding appearance.");
            ConversationLines.npc_interaction_reply_date_first_yes_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord035}I must confess, I can not stop thinking of you. I clearly have feelings for you, {TITLE}, and would like to bring our relationship to the next level. What do you say?");
            ConversationLines.npc_interaction_reply_date_first_yes_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord036}Your presence makes me blush, {TITLE}. I would love to see you more frequently, just the two of us in private. What do you say, {TITLE}?");
            ConversationLines.npc_interaction_date_single_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord037}Oh, {TITLE}, I have been missing you! The servants prepared a meal im my private chambers. Would you care to join me?");
            ConversationLines.npc_interaction_reply_date_yes_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord038}I was looking forward to seeing you, {TITLE}. Would you like to retreat somewhere more silent, for a more private conversation?");
            ConversationLines.npc_interaction_date_married_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord039}We are in luck, {TITLE}. {SPOUSE} is currently not around and the chambermaid swore to remain silent. Will you come with me?");
            ConversationLines.npc_interaction_date_married_1.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);//"{=Dramalord039}We are in luck, {TITLE}. {SPOUSE} is currently not around and the chambermaid swore to remain silent. Will you come with me?");
            ConversationLines.npc_interaction_date_married_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord040}Ugh, {SPOUSE} is finally away. Now we have all the rooms for us! The servants will keep it for themselves, care to join me, {TITLE}?");
            ConversationLines.npc_interaction_date_married_2.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);//"{=Dramalord040}Ugh, {SPOUSE} is finally away. Now we have all the rooms for us! The servants will keep it for themselves, care to join me, {TITLE}?");
            ConversationLines.player_reaction_sex_friend_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord061}Interesting proposition, {TITLE}. I think I like the idea.");
            ConversationLines.npc_interaction_sex_friend_wb_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord045}I was looking forward to seeing you, {TITLE}. Would you care for a chat and maybe some bed excercise afterwards?");
            ConversationLines.npc_interaction_sex_friend_wb_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord046}Oh, {TITLE}. I was hoping you had some time for a conversation and some anatomical studies after. Are you interested?");
            ConversationLines.npc_interaction_reply_sex_fwb_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord239}Not today, {TITLE}. I'm not in the mood, ask me some other time.");
            ConversationLines.npc_interaction_sex_married_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord047}{SPOUSE} is not around I need to feel you, {TITLE}. Let's head to the bedchamber and enjoy ourselves. Would you like that?");
            ConversationLines.npc_interaction_sex_married_1.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);// "{=Dramalord047}{SPOUSE} is not around I need to feel you, {TITLE}. Let's head to the bedchamber and enjoy ourselves. Would you like that?");
            ConversationLines.npc_interaction_sex_married_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord048}Now that {SPOUSE} is not around, I was thinking that you and I could use the empty bed for ourselves, {TITLE}. Does this tempt you?");
            ConversationLines.npc_interaction_sex_married_2.SetTextVariable("SPOUSE", Hero.OneToOneConversationHero.Spouse?.Name);// "{=Dramalord048}Now that {SPOUSE} is not around, I was thinking that you and I could use the empty bed for ourselves, {TITLE}. Does this tempt you?");
            ConversationLines.npc_interaction_sex_else_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord049}Come here and kiss me, {TITLE}. I'm craving for your body and want to enjoy you with all the lust and pleasure there is!");
            ConversationLines.npc_interaction_sex_else_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord050}Let's get rid of that clothes of yours. I show you a different kind of battle in my bedchamber where both sides win!");
            ConversationLines.npc_interaction_reply_sex_no_interest.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord103}Apologies, {TITLE}, but I will not do that with you.");
            ConversationLines.player_reaction_marry_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord067}I agree, {TITLE}. Let's seal this bond of ours.");

            ConversationLines.player_quest_joinparty_start.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));//{=Dramalord545}Tell me, {TITLE}, would you like to join my party for a while?"
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");

            Hero? Permitter = (Hero.OneToOneConversationHero.Father != null && Hero.OneToOneConversationHero.Father.IsAlive) ? Hero.OneToOneConversationHero.Father :
            (Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan != Clan.PlayerClan && Hero.OneToOneConversationHero.Clan.Leader != Hero.OneToOneConversationHero) ? Hero.OneToOneConversationHero.Clan.Leader : null;
            ConversationLines.player_quest_marriage_start.SetTextVariable("HERO", Permitter?.Name);
            ConversationLines.player_quest_marriage_ask.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));
            ConversationLines.player_quest_marriage_agree.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.player_quest_marriage_later.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.player_quest_marriage_decline.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.npc_interaction_betrothed_married.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.player_reaction_engagement_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord051}I love you very much, {TITLE}. I think it's time for us to take the next step in our relationship. Will you marry me?");
            ConversationLines.npc_interaction_reply_engage_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord104}I am sorry {TITLE}, but I am not ready for this step just yet.");
            ConversationLines.npc_interaction_reply_uhwell.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord438}Uh... well...");
            ConversationLines.npc_as_you_wish_reply.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false));

            ConversationLines.npc_interaction_reply_gift.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord293}Thank you {TITLE}! I will keep it close to my... bed as a reminder of your... affection and enjoy it every day!");

            ConversationLines.npc_info_reply_ask_accept.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord320}Certainly, {TITLE}. If you really want know I can tell you...");
            ConversationLines.npc_info_reply_ask_deny.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));//"{=Dramalord319}Apologies, {TITLE}, but I don't trust you enough to tell you about my personal preferences.");
            ConversationLines.npc_info_reply_orientation_hetero.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord321}I feel drawn to the other sex and I don't have much interest in persons of my own.");
            ConversationLines.npc_info_reply_orientation_gay.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord322}I don't have much interest in the other sex, I rather prefer persons of my own.");
            ConversationLines.npc_info_reply_orientation_bi.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord323}I find people of the other sex attractive, but also feel drawn to those of my own.");
            ConversationLines.npc_info_reply_orientation_none.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord324}I don't have any affections for either sex. They don't interest me much.");
            ConversationLines.npc_info_reply_weight_thin.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord325}I think slim people are more grazile then others.");
            ConversationLines.npc_info_reply_weight_normal.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord326}I don't like thin or fat. The middle is just right.");
            ConversationLines.npc_info_reply_weight_fat.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord327}I like people with more weight. There's more to grab for me.");
            ConversationLines.npc_info_reply_build_low.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord328}Muscles are overrated. I like it skinny and want to see bones.");
            ConversationLines.npc_info_reply_build_average.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord329}Medium muscles are just right for me. I don't need something special.");
            ConversationLines.npc_info_reply_build_high.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));// "{=Dramalord330}I love powerful people. The bulkier the better.");
            int diff = Hero.OneToOneConversationHero.GetDesires().AttractionAgeDiff;
            TextObject ageDiff = (diff < 5) ? new TextObject("{=Dramalord333}younger") : (diff > 5) ? new TextObject("{=Dramalord334}older") : new TextObject("{=Dramalord335}not older and not younger");
            TextObject age = new TextObject(MBMath.ClampInt((int)Hero.OneToOneConversationHero.Age + diff, 18, 120));
            ConversationLines.npc_info_reply_age.SetTextVariable("AGEDIFF", ageDiff);// "{=Dramalord331}I like people who are {AGEDIFF} then me. Best around the age of {AGE}.");
            ConversationLines.npc_info_reply_age.SetTextVariable("AGE", age);// "{=Dramalord331}I like people who are {AGEDIFF} then me. Best around the age of {AGE}.");
            ConversationLines.npc_info_reply_summary.SetTextVariable("RATING", new TextObject(Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero)));// "{=Dramalord332}You {TITLE}, I think you are a {RATING} out of 100, I would say.");
            ConversationLines.npc_info_reply_summary.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));

            ConversationLines.npc_interaction_breakup_love.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false)); 
            ConversationLines.player_reaction_breakup_accept.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            ConversationLines.npc_confrontation_result_leave.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
        }
    }
}
