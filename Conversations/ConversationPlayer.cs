using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Quests;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class ConversationPlayer
    {
        private static Hero? SelectedChild = null;

        private static bool Timeout() => Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).LastInteraction.ElapsedDaysUntilNow < DramalordMCM.Instance.DaysBetweenInteractions;

        private static bool SpouseAway() => Hero.OneToOneConversationHero.Spouse == null || Hero.OneToOneConversationHero.Spouse == Hero.MainHero || !Hero.OneToOneConversationHero.IsCloseTo(Hero.OneToOneConversationHero.Spouse);

        private static bool HasOtherSpouse() => Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse.IsAlive && !Hero.OneToOneConversationHero.IsSpouseOf(Hero.MainHero);

        private static bool FreeForMarriage() =>  Hero.OneToOneConversationHero.Clan == null || Hero.OneToOneConversationHero.Clan == Clan.PlayerClan || Hero.OneToOneConversationHero.Clan?.Leader == Hero.OneToOneConversationHero;

        internal static void AddDialogs(CampaignGameStarter starter)
        {

            DialogFlow startFlow = DialogFlow.CreateDialogFlow("hero_main_options")
                .BeginPlayerOptions()
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_START)
                        .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && !Hero.OneToOneConversationHero.IsPrisoner && !Hero.MainHero.IsPrisoner && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .BeginNpcOptions()
                            .NpcOption(DramalordTexts.INTENTION_REACT_YES + "[ib:normal2][if:convo_calm_friendly]", () => Hero.OneToOneConversationHero.GetRelationWithPlayer() > -30 && ConversationTools.SetConversationHero(Hero.MainHero))
                                .GotoDialogState("player_interaction_selection")
                            .NpcOption(DramalordTexts.INTENTION_REACT_NO_INTEREST + "[ib:closed][if:convo_bored]", () => Hero.OneToOneConversationHero.GetRelationWithPlayer() <= -30 && ConversationTools.SetConversationHero(Hero.MainHero))
                                .GotoDialogState("hero_main_options")
                        .EndNpcOptions()
                    .PlayerOption(DramalordTexts.INTENTION_PRISON)
                        .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && Hero.OneToOneConversationHero.IsPrisoner && !Hero.MainHero.IsPrisoner && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .BeginNpcOptions()
                            .NpcOption(DramalordTexts.INTENTION_PRISON_OK + "[ib:normal2][if:convo_calm_friendly]", () => Hero.OneToOneConversationHero.GetRelationWithPlayer() > -30 && ConversationTools.SetConversationHero(Hero.MainHero))
                                .BeginPlayerOptions()
                                    .PlayerOption(DramalordTexts.INTENTION_PRISON_OFFER)
                                        .BeginNpcOptions()
                                            .NpcOption(DramalordTexts.INTENTION_PRISON_OFFER_OK, () => Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor) < 1 && Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) >= DramalordMCM.Instance.MinAttraction)
                                                .Consequence(() => DramalordEvents.Instance.StartIntention(new PrisonSexEvent(Hero.MainHero, Hero.OneToOneConversationHero)))
                                                .CloseDialog()
                                            .NpcOption(DramalordTexts.INTENTION_PRISON_OFFER_NO, () => Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor) >= 1 || Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) < DramalordMCM.Instance.MinAttraction)
                                                .GoBackToDialogState("hero_main_options")
                                        .EndNpcOptions()
                                    .PlayerOption(DramalordTexts.INTENTION_PRISON_KILL)
                                        .NpcLine(DramalordTexts.INTENTION_PRISON_KILL_OK)
                                            .Consequence(() => MBInformationManager.ShowSceneNotification(HeroExecutionSceneNotificationData.CreateForPlayerExecutingHero(Hero.OneToOneConversationHero, null)))
                                            .CloseDialog()
                                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_END)
                                        .GotoDialogState("hero_main_options")
                                .EndPlayerOptions()
                            .NpcOption(DramalordTexts.INTENTION_PRISON_NO + "[ib:closed][if:convo_bored]", () => Hero.OneToOneConversationHero.GetRelationWithPlayer() <= -30 && ConversationTools.SetConversationHero(Hero.MainHero))
                                .GotoDialogState("hero_main_options")
                        .EndNpcOptions()
                    .PlayerOption(DramalordTexts.ORPHANAGE_ADOPT)
                        .Condition(() => Hero.OneToOneConversationHero.Occupation == Occupation.GangLeader)
                        .BeginNpcOptions()
                            .NpcOption(DramalordTexts.ORPHANAGE_ADOPT_BOY_GIRL, () =>
                            {
                                int boys = DramalordOrphans.Instance.CountOrphans(false);
                                int girls = DramalordOrphans.Instance.CountOrphans(true);
                                if (boys + girls > 0)
                                {
                                    ConversationTools.SetConversationHero(Hero.MainHero);
                                    ConversationTools.SetConversationText(new TextObject(boys), new TextObject(girls));
                                    return true;
                                }
                                return false;
                            })
                                .BeginPlayerOptions()
                                    .PlayerOption(DramalordTexts.ORPHANAGE_ADOPT_BOY)
                                        .Condition(() => DramalordOrphans.Instance.CountOrphans(false) > 0)
                                        .Consequence(() => { SelectedChild = null; ConversationSentence.SetObjectsToRepeatOver(DramalordOrphans.Instance.GetOrphans(false)); })
                                        .GotoDialogState("orphanage_child_selection")
                                    .PlayerOption(DramalordTexts.ORPHANAGE_ADOPT_GIRL)
                                        .Condition(() => DramalordOrphans.Instance.CountOrphans(true) > 0)
                                        .Consequence(() => { SelectedChild = null; ConversationSentence.SetObjectsToRepeatOver(DramalordOrphans.Instance.GetOrphans(true)); })
                                        .GotoDialogState("orphanage_child_selection")
                                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_END)
                                        .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                        .GotoDialogState("hero_main_options")
                                .EndPlayerOptions()
                            .NpcOption(DramalordTexts.ORPHANAGE_ADOPT_EMPTY, () => DramalordOrphans.Instance.CountOrphans(false) + DramalordOrphans.Instance.CountOrphans(true) == 0 && ConversationTools.SetConversationHero(Hero.MainHero))
                        .EndNpcOptions()
                    .PlayerOption(DramalordTexts.ORPHANAGE_ORPHANIZE)
                        .Condition(() => Hero.OneToOneConversationHero.Occupation == Occupation.GangLeader)
                        .BeginNpcOptions()
                            .NpcOption(DramalordTexts.ORPHANAGE_ORPHANIZE_OK, () => Hero.MainHero.Children.Where(c => c.Age < 18).ToList().Count > 0 && ConversationTools.SetConversationHero(Hero.MainHero))
                                .Consequence(() => { SelectedChild = null; ConversationSentence.SetObjectsToRepeatOver(Hero.MainHero.Children.Where(c => c.Age < 18).ToList()); })
                                .GotoDialogState("orphanage_player_ownchild_selection")
                            .NpcOption(DramalordTexts.ORPHANAGE_ORPHANIZE_EMPTY, () => Hero.MainHero.Children.Where(c => c.Age < 18).ToList().Count == 0 && ConversationTools.SetConversationHero(Hero.MainHero))
                                .GotoDialogState("hero_main_options")
                        .EndNpcOptions()
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_GOSSIP)
                        .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit())
                        .Consequence(() => DramalordEvents.Instance.SetNextGossipLine(Hero.OneToOneConversationHero))
                        .GotoDialogState("start_reaction")
                .EndPlayerOptions();

            DialogFlow playerSelectionFlow = DialogFlow.CreateDialogFlow("player_interaction_selection")
                .BeginPlayerOptions()
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_TALK)
                        .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_talk")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_FLIRT)
                        .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_flirt")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_DATING_START)
                        .Condition(() => Hero.MainHero.IsFriendOf(Hero.OneToOneConversationHero) && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_date")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_DATING)
                        .Condition(() => Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_date")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_SEX)
                        .Condition(() => Hero.MainHero.IsLoverOf(Hero.OneToOneConversationHero) && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_sex")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_DIVORCE_SPOUSE)
                        .Condition(() => Hero.MainHero.IsLoverOf(Hero.OneToOneConversationHero) && HasOtherSpouse() && DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) == null && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero.Spouse))
                        .GotoDialogState("npc_interaction_reply_divorce")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_MARRIAGE_START)
                        .Condition(() => Hero.MainHero.IsLoverOf(Hero.OneToOneConversationHero) && !HasOtherSpouse() && DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) == null && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_engage")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_MARRIAGE)
                        .Condition(() => Hero.MainHero.IsLoverOf(Hero.OneToOneConversationHero) && DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) is MarriagePermissionQuest && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_marriage")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_ASK_INFO)
                        .Condition(() => !Hero.OneToOneConversationHero.GetDesires().IsKnowToPlayer && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_ask")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_BREAK_UP)
                        .Condition(() => Hero.MainHero.IsEmotionalWith(Hero.OneToOneConversationHero) && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .GotoDialogState("npc_interaction_reply_breakup")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_APPROACH)
                        .Condition(() => Hero.OneToOneConversationHero.IsBlockedBy(Hero.MainHero) && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .NpcLine(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                            .Consequence(() =>
                            {
                                Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).SetBlockedUntil(CampaignTime.DaysFromNow(-14));
                                DramalordBanner.CreateBanner(Hero.OneToOneConversationHero, DramalordTexts.BANNER_APPROACH_START);
                            })
                            .GotoDialogState("player_interaction_selection")
                    .PlayerOption(DramalordTexts.PLAYER_INTERACTION_END)
                        .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                        .NpcLine(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                            .GotoDialogState("hero_main_options")
                .EndPlayerOptions();


            DialogFlow talkFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_talk")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_TALK_1 + "[ib:normal][if:convo_calm_friendly]", () => !Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.QuestionType.Talk, 1, true))
                        .GotoDialogState("start_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_TIMEOUT + "[ib:closed][if:convo_bored]", () => Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow flirtFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_flirt")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_FLIRT_1 + "[ib:normal2][if:convo_mocking_teasing]", () => (Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) >= DramalordMCM.Instance.MinAttraction) && !Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() => {
                            ConversationQuestions.SetupQuestions(ConversationQuestions.QuestionType.Flirt, 1, true); 
                        })
                        .GotoDialogState("start_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_TIMEOUT + "[ib:closed][if:convo_bored]", () => Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption(DramalordTexts.INTENTION_REACT_NO_INTEREST + "[ib:nervous][if:convo_shocked]", () => !Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero) < DramalordMCM.Instance.MinAttraction && !Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow dateFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_date")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_DATE_FIRST_1 + "[ib:confident3][if:convo_excited]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && (Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove || ConversationPersuasions.Success) && !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() => {ConversationQuestions.SetupQuestions(ConversationQuestions.QuestionType.Date, 3, true); ConversationPersuasions.Success = false; })
                        .GotoDialogState("start_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_DATE_1 + "[ib:demure2][if:convo_merry]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.QuestionType.Date, 3, true))
                        .GotoDialogState("start_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_DATE_1 + "[ib:demure2][if:convo_mocking_teasing]", () => SpouseAway() && HasOtherSpouse() && !Timeout() && Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationHero(Hero.MainHero, Hero.OneToOneConversationHero.Spouse))
                        .Consequence(() => ConversationQuestions.SetupQuestions(ConversationQuestions.QuestionType.Date, 3, true))
                        .GotoDialogState("start_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_UHWELL + "[ib:nervous2][if:convo_confused_normal]", () => !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love > 0 && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove && !ConversationPersuasions.Success)
                        .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForDate())
                        .GotoDialogState("npc_persuasion_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_SPOUSE_NEARBY + "[ib:nervous][if:convo_shocked]", () => !SpouseAway() && HasOtherSpouse() && !Timeout() && (Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) || Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove) && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_TIMEOUT + "[ib:closed][if:convo_bored]", () => Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption(DramalordTexts.INTENTION_REACT_NO_INTEREST + "[ib:nervous][if:convo_shocked]", () => !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love <= 0 && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow sexFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_sex")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_UHWELL + "[ib:nervous2][if:convo_confused_normal]", () => (SpouseAway() || !HasOtherSpouse()) && !Timeout() && Hero.OneToOneConversationHero.IsLoverOf(Hero.MainHero) && !ConversationPersuasions.Success && Hero.OneToOneConversationHero.GetDesires().Horny >= 20)
                        .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForFWB())
                        .GotoDialogState("npc_persuasion_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_SEX_OK+ "[ib:confident3][if:convo_excited]", () => !Timeout() && (Hero.OneToOneConversationHero.IsSpouseOf(Hero.MainHero) || ConversationPersuasions.Success) && Hero.OneToOneConversationHero.GetDesires().Horny >= 20)
                        .Consequence(() => {DramalordEvents.Instance.StartIntention(new SexEvent(Hero.MainHero, Hero.OneToOneConversationHero)); ConversationPersuasions.Success = false; })
                        .CloseDialog()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_SPOUSE_NEARBY + "[ib:nervous][if:convo_shocked]", () => !SpouseAway() && HasOtherSpouse() && !Timeout() && Hero.OneToOneConversationHero.IsLoverOf(Hero.MainHero) && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_TIMEOUT + "[ib:closed][if:convo_bored]", () => Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption(DramalordTexts.INTENTION_REACT_NO_INTEREST + "[ib:normal2][if:convo_confused_annoyed]", () => !Timeout() && Hero.OneToOneConversationHero.GetDesires().Horny < 20 && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow engageFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_engage")
                .BeginNpcOptions()  
                    .NpcOption(DramalordTexts.NPC_INTERACTION_MARRIAGE_OK + "[ib:aggressive][if:convo_delighted]", () => !Timeout() && FreeForMarriage() && (Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinMarriageLove || ConversationPersuasions.Success) && ConversationTools.SetConversationHero(Hero.MainHero))
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                                .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                .Consequence(() => { DramalordEvents.Instance.StartIntention(new MarriageEvent(Hero.MainHero, Hero.OneToOneConversationHero)); ConversationPersuasions.Success = false; })
                                .CloseDialog()
                            .PlayerOption(DramalordTexts.PLAYER_INTERACTION_END)
                                .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                .GotoDialogState("npc_interaction_abort")
                        .EndPlayerOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_ENGAGE_OK + "[ib:aggressive][if:convo_delighted]", () => !Timeout() && !FreeForMarriage() && (Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinMarriageLove || ConversationPersuasions.Success) && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero.Clan.Leader))
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.NPC_INTERACTION_ASYOUWISH)
                                .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                .Consequence(() =>
                                {
                                    ConversationPersuasions.Success = false;
                                    MarriagePermissionQuest quest = new MarriagePermissionQuest(Hero.OneToOneConversationHero,
                                        Hero.OneToOneConversationHero.Clan.Leader, 
                                        CampaignTime.DaysFromNow(21));
                                    quest.StartQuest();
                                    DramalordQuests.Instance.AddQuest(Hero.OneToOneConversationHero, quest);
                                })
                                .CloseDialog()
                            .PlayerOption(DramalordTexts.PLAYER_INTERACTION_END)
                                .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                                .GotoDialogState("npc_interaction_abort")
                        .EndPlayerOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_UHWELL + "[ib:nervous2][if:convo_confused_normal]", () => !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinMarriageLove && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love >= DramalordMCM.Instance.MinDatingLove)
                        .Consequence(() => ConversationPersuasions.CreatePersuasionTaskForEngage())
                        .GotoDialogState("npc_persuasion_challenge")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_ENGAGE_NO + "[ib:normal2][if:convo_confused_annoyed]", () => !Timeout() && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love < DramalordMCM.Instance.MinDatingLove && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_TIMEOUT + "[ib:closed][if:convo_bored]", () => Timeout() && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();

            DialogFlow marryFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_marriage")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_MARRIAGE_OK + "[ib:aggressive][if:convo_delighted]", () => DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) is MarriagePermissionQuest quest && quest.HasAsked && ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() => DramalordEvents.Instance.StartIntention(new MarriageEvent(Hero.MainHero, Hero.OneToOneConversationHero)))
                        .CloseDialog()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_ENGAGE_OK + "[ib:normal2][if:convo_confused_annoyed]", () => DramalordQuests.Instance.GetQuest(Hero.OneToOneConversationHero) is MarriagePermissionQuest quest && quest.HasAsked && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                .EndNpcOptions();


            DialogFlow breakupFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_breakup")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_BREAKUP_OK + "[ib:nervous][if:convo_shocked]", () => ConversationTools.SetConversationHero(Hero.MainHero))
                        .Consequence(() => { 
                            Hero.OneToOneConversationHero.ChangeRelationTo(Hero.MainHero, -100 + Hero.OneToOneConversationHero.GetPersonality().Empathy, Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Love * -1);
                            DramalordEvents.Instance.StartIntention(new RelationshipEvent(Hero.MainHero, Hero.OneToOneConversationHero));
                        })
                        .CloseDialog()
                .EndNpcOptions();

            DialogFlow askFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_ask")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_NO + "[ib:hip][if:convo_annoyed]", () => Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) < DramalordMCM.Instance.MinTrustFriends && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("player_interaction_selection")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_OK + "[ib:normal2][if:convo_calm_friendly]", () => Hero.OneToOneConversationHero.GetTrust(Hero.MainHero) >= DramalordMCM.Instance.MinTrustFriends && ConversationTools.SetConversationHero(Hero.MainHero))
                        .GotoDialogState("npc_interaction_reply_orientation")
                .EndNpcOptions();


            DialogFlow answerOrientationFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_orientation")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_HETERO + "[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return (o2o.IsFemale) ? desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen < DramalordMCM.Instance.MinAttraction
                            : desires.AttractionMen < DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_HOMO + "[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return (o2o.IsFemale) ? desires.AttractionMen < DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction
                            : desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen < DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_BI + "[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return desires.AttractionMen >= DramalordMCM.Instance.MinAttraction && desires.AttractionWomen >= DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_ASEX + "[ib:demure][if:convo_approving]", () =>
                    {
                        HeroDesires desires = Hero.OneToOneConversationHero.GetDesires();
                        Hero o2o = Hero.OneToOneConversationHero;
                        return desires.AttractionMen < DramalordMCM.Instance.MinAttraction && desires.AttractionWomen < DramalordMCM.Instance.MinAttraction;
                    })
                    .GotoDialogState("npc_interaction_reply_weight")
                .EndNpcOptions();

            DialogFlow answerWeightFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_weight")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_WEIGHT_LOW + "[ib:demure][if:happy]", () => Hero.OneToOneConversationHero.GetDesires().AttractionWeight <= 33)
                    .GotoDialogState("npc_interaction_reply_build")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_WEIGHT_HIGH + "[ib:demure][if:happy]", () => Hero.OneToOneConversationHero.GetDesires().AttractionWeight >= 66)
                    .GotoDialogState("npc_interaction_reply_build")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_WEIGHT_MID + "[ib:demure][if:happy]", () => Hero.OneToOneConversationHero.GetDesires().AttractionWeight > 33 && Hero.OneToOneConversationHero.GetDesires().AttractionWeight < 66)
                    .GotoDialogState("npc_interaction_reply_build")
                .EndNpcOptions();

            DialogFlow answerBuildFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_build")
                .BeginNpcOptions()
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_BUILD_LOW + "[ib:confident][if:convo_excited]", () => Hero.OneToOneConversationHero.GetDesires().AttractionBuild <= 33)
                    .GotoDialogState("npc_interaction_reply_age")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_BUILD_HIGH + "[ib:confident][if:convo_excited]", () => Hero.OneToOneConversationHero.GetDesires().AttractionBuild >= 66)
                    .GotoDialogState("npc_interaction_reply_age")
                    .NpcOption(DramalordTexts.NPC_INTERACTION_INFO_BUILD_MID + "[ib:confident][if:convo_excited]", () => Hero.OneToOneConversationHero.GetDesires().AttractionBuild > 33 && Hero.OneToOneConversationHero.GetDesires().AttractionBuild < 66)
                    .GotoDialogState("npc_interaction_reply_age")
                .EndNpcOptions();

            DialogFlow answerAgeFlow = DialogFlow.CreateDialogFlow("npc_interaction_reply_age")
                .NpcLine(DramalordTexts.NPC_INTERACTION_INFO_AGE_DIFF + "[ib:normal2][if:convo_calm_friendly]")
                .Condition(() => SetAgeInfo())
                .NpcLine(DramalordTexts.NPC_INTERACTION_INFO_RESULT + "[ib:demure][if:convo_bemused]")
                    .Condition(() => ConversationTools.SetConversationText(new TextObject(Hero.OneToOneConversationHero.GetAttractionTo(Hero.MainHero))))
                    .Consequence(() =>
                    {
                        Hero.OneToOneConversationHero.GetDesires().IsKnowToPlayer = true;
                        DramalordBanner.CreateBanner(Hero.OneToOneConversationHero, DramalordTexts.BANNER_INFO_LEARNED, true);
                    })
                .GotoDialogState("player_interaction_selection");

            starter.AddDialogLine("orphanage_select_orphan", "orphanage_child_selection", "orphanage_player_select_child", DramalordTexts.ORPHANAGE_ADOPT_WHO, null, null);
            starter.AddRepeatablePlayerLine("orphanage_player_select_child", "orphanage_player_select_child", "orphanage_confirm_adopt", DramalordTexts.ORPHANAGE_ADOPT_SELECT, DramalordTexts.ORPHANAGE_ADOPT_OTHER, "orphanage_child_selection",
                () =>
                {
                    Hero? child = ConversationSentence.CurrentProcessedRepeatObject as Hero;
                    if (child != null)
                    {
                        StringHelpers.SetRepeatableCharacterProperties("ACTOR1", child.CharacterObject);
                        MBTextManager.SetTextVariable("TEXT1", (int)child.Age);
                        return true;
                    }

                    return false;
                },
                null);

            starter.AddDialogLine("orphanage_player_ownchild", "orphanage_player_ownchild_selection", "orphanage_player_select_ownchild", DramalordTexts.ORPHANAGE_ORPHANIZE_WHO, null, null);
            starter.AddRepeatablePlayerLine("orphanage_player_select_ownchild", "orphanage_player_select_ownchild", "orphanage_confirm_orphanize", DramalordTexts.ORPHANAGE_ADOPT_SELECT, DramalordTexts.ORPHANAGE_ADOPT_OTHER, "orphanage_player_ownchild_selection",
                () =>
                {
                    Hero? child = ConversationSentence.CurrentProcessedRepeatObject as Hero;
                    if (child != null)
                    {
                        StringHelpers.SetRepeatableCharacterProperties("ACTOR1", child.CharacterObject);
                        MBTextManager.SetTextVariable("TEXT1", (int)child.Age);
                        return true;
                    }

                    return false;
                },
                null);

            DialogFlow doAdoptFlow = DialogFlow.CreateDialogFlow("orphanage_confirm_adopt")
                .NpcLine(DramalordTexts.ORPHANAGE_ADOPT_CONFIRM)
                    .Condition(() =>
                    {
                        SelectedChild = (ConversationSentence.SelectedRepeatObject as Hero);
                        StringHelpers.SetCharacterProperties("ACTOR1", SelectedChild?.CharacterObject);
                        return true;
                    })
                    .BeginPlayerOptions()
                        .PlayerOption("{=str_yes}Yes.")
                            .NpcLine(DramalordTexts.ORPHANAGE_ADOPT_FINISHED)
                            .Consequence(() =>
                            {
                                if (SelectedChild != null)
                                {
                                    DramalordEvents.Instance.StartIntention(new AdoptEvent(Hero.MainHero, SelectedChild));
                                    SelectedChild = null;
                                }
                            })
                        .PlayerOption("{=str_no}No.")
                            .NpcLine(DramalordTexts.ORPHANAGE_ADOPT_ABORT)
                            .GotoDialogState("hero_main_options")
                    .EndPlayerOptions();

            DialogFlow doOrphanizeFlow = DialogFlow.CreateDialogFlow("orphanage_confirm_orphanize")
                .NpcLine(DramalordTexts.ORPHANAGE_ORPHANIZE_CONFIRM)
                    .Condition(() =>
                    {
                        SelectedChild = (ConversationSentence.SelectedRepeatObject as Hero);
                        StringHelpers.SetCharacterProperties("ACTOR1", SelectedChild?.CharacterObject);
                        return true;
                    })
                    .BeginPlayerOptions()
                        .PlayerOption("{=str_yes}Yes.")
                            .NpcLine(DramalordTexts.ORPHANAGE_ORPHANIZE_FINISHED)
                            .Consequence(() =>
                            {
                                if (SelectedChild != null)
                                {
                                    DramalordEvents.Instance.StartIntention(new OrphanizeEvent(Hero.MainHero, SelectedChild));
                                    SelectedChild = null;
                                }
                            })
                        .PlayerOption("{=str_no}No.")
                            .NpcLine(DramalordTexts.ORPHANAGE_ORPHANIZE_ABORT)
                            .GotoDialogState("hero_main_options")
                    .EndPlayerOptions();

            starter.AddDialogFlow(startFlow);
            starter.AddDialogFlow(playerSelectionFlow);
            starter.AddDialogFlow(talkFlow);
            starter.AddDialogFlow(flirtFlow);
            starter.AddDialogFlow(dateFlow);
            starter.AddDialogFlow(sexFlow);
            starter.AddDialogFlow(engageFlow);
            starter.AddDialogFlow(marryFlow);
            starter.AddDialogFlow(breakupFlow);
            starter.AddDialogFlow(askFlow); 
            starter.AddDialogFlow(answerOrientationFlow);
            starter.AddDialogFlow(answerWeightFlow);
            starter.AddDialogFlow(answerBuildFlow);
            starter.AddDialogFlow(answerAgeFlow);
            starter.AddDialogFlow(doAdoptFlow);
            starter.AddDialogFlow(doOrphanizeFlow);
        }

        internal static bool SetAgeInfo()
        {
            int diff = Hero.OneToOneConversationHero.GetDesires().AttractionAgeDiff;
            TextObject ageDiff = new TextObject((diff < 5) ? DramalordTexts.NAME_YOUNGER : (diff > 5) ? DramalordTexts.NAME_OLDER : DramalordTexts.NAME_SAME_AGE);
            TextObject age = new TextObject(MBMath.ClampInt((int)Hero.OneToOneConversationHero.Age + diff, 18, 120));
            ConversationTools.SetConversationText(ageDiff, age);
            return true;
        }
    }
}
