using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Quests;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class MarriageEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        private bool DidMarry;

        public MarriageEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            IsKnownTo.Add(Actor);
            IsKnownTo.Add(Target);
            DidMarry = false;
        }

        public void Action(int modifier = 0)
        {
            if((Actor.Clan != null && Actor.Clan == Clan.PlayerClan || Target.Clan != null && Target.Clan == Clan.PlayerClan) && Actor != Hero.MainHero && Target != Hero.MainHero)
            {
                DramalordInquiry.CreateYesNoInquiry(Actor, Target, new TextObject(DramalordTexts.INQUIRY_MARRIAGE_TITLE), new TextObject(DramalordTexts.INQUIRY_MARRIAGE_ALLOW), () => { DoMarry(); AfterDialog(); }, () => { Actor.GetRelationTo(Target).SetBlockedUntil(CampaignTime.DaysFromNow(7)); });
            }
            else
            {
                DoMarry();
            }
        }

        private void DoMarry()
        {
            DidMarry = true;

            if (Actor != Hero.MainHero && Actor.Spouse != null)
            {
                Actor.ChangeRelationTo(Actor.Spouse, -100, -100);
                (new RelationshipEvent(Actor, Actor.Spouse)).Action();
            }

            // Only the main hero keeps their old spouses - npcs are divorced!
            if (Target != Hero.MainHero && Target.Spouse != null)
            {
                Target.ChangeRelationTo(Target.Spouse, -100, -100);
                (new RelationshipEvent(Target, Target.Spouse)).Action();
            }

            ChangeRomanticStateAction.Apply(Actor, Target, Romance.RomanceLevelEnum.Marriage);
            Actor.ExSpouses.Remove(Target);
            Target.ExSpouses.Remove(Actor);

            Actor.Spouse = Target;
            Target.Spouse = Actor;
        }

        public void AfterDialog()
        {
            if(!DidMarry)
            {
                return;
            }

            if (Actor == Hero.MainHero || Target == Hero.MainHero)
            {
                Hero other = Actor == Hero.MainHero ? Target : Actor;
                if (DramalordQuests.Instance.GetQuest(other) is MarriagePermissionQuest quest)
                {
                    if (quest.HasAsked)
                    {
                        quest.QuestSuccess(Hero.MainHero);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            AddLogEntry(this);

            Hero groom = Actor.IsFemale ? Target : Actor;
            Hero bride = groom == Actor ? Target : Actor;
            bool showScene = Actor.Clan == Clan.PlayerClan || Target.Clan == Clan.PlayerClan;

            if (groom.Clan == Clan.PlayerClan && bride.Clan != Clan.PlayerClan) 
            {
                DramalordInquiry.CreateYesNoInquiry(bride, new TextObject(DramalordTexts.INQUIRY_MARRIAGE_TITLE), new TextObject(DramalordTexts.INQUIRY_MARRIAGE_JOIN_CLAN), () => {
                    DramalordEvents.Instance.StartIntention(new LeaveClanEvent(bride));
                    DramalordEvents.Instance.StartIntention(new JoinClanEvent(bride, Clan.PlayerClan));
                    bride.SetNewOccupation(groom.Occupation);
                }, () => { });
                
            }
            else if(groom.Clan != Clan.PlayerClan && bride.Clan == Clan.PlayerClan)
            {
                DramalordInquiry.CreateYesNoInquiry(groom, new TextObject(DramalordTexts.INQUIRY_MARRIAGE_TITLE), new TextObject(DramalordTexts.INQUIRY_MARRIAGE_JOIN_CLAN), () => {
                    DramalordEvents.Instance.StartIntention(new LeaveClanEvent(groom));
                    DramalordEvents.Instance.StartIntention(new JoinClanEvent(groom, Clan.PlayerClan));
                    groom.SetNewOccupation(bride.Occupation);
                }, () => { });
            }
            else if((groom == Hero.MainHero && bride.IsPlayerCompanion) || (bride == Hero.MainHero && groom.IsPlayerCompanion))
            {
                if (groom.IsPlayerCompanion)
                {
                    groom.SetNewOccupation(Occupation.Lord);
                }
                else
                {
                    bride.SetNewOccupation(Occupation.Lord);
                }
            }
            else if (groom.Clan != bride.Clan)
            {
                if (groom.Clan != null && bride.Clan != null)
                {
                    if (bride.Clan.Leader == bride)
                    {
                        DramalordEvents.Instance.StartIntention(new LeaveClanEvent(groom));
                        DramalordEvents.Instance.StartIntention(new JoinClanEvent(groom, bride.Clan));
                        groom.SetNewOccupation(Occupation.Lord);
                    }
                    else
                    {
                        DramalordEvents.Instance.StartIntention(new LeaveClanEvent(bride));
                        DramalordEvents.Instance.StartIntention(new JoinClanEvent(bride, groom.Clan));
                        bride.SetNewOccupation(Occupation.Lord);
                    }
                }
                else if (groom.Clan != null)
                {
                    DramalordEvents.Instance.StartIntention(new LeaveClanEvent(bride));
                    DramalordEvents.Instance.StartIntention(new JoinClanEvent(bride, groom.Clan));
                    bride.SetNewOccupation(Occupation.Lord);
                }
                else if (bride.Clan != null)
                {
                    DramalordEvents.Instance.StartIntention(new LeaveClanEvent(groom));
                    DramalordEvents.Instance.StartIntention(new JoinClanEvent(groom, bride.Clan));
                    groom.SetNewOccupation(Occupation.Lord);
                }
            }

            

            if(showScene)
            {
                if(DramalordMCM.Instance.ShowDramaVideos)
                {
                    DramalordVideoNotification.ShowDramalordVideoNotification(groom, bride, null,DramalordVideoNotification.VideoContext.Wedding);
                }
                else
                {
                    TextObject textObject = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_MARRIAGE), Actor, Target);
                    MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(groom, bride, CampaignTime.Now));
                    MBInformationManager.AddNotice(new MarriageMapNotification(groom, bride, textObject, CampaignTime.Now));
                }
            }

            Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_MARRIAGE), Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public DialogFlow GetInitiationDialog()
        {
            return DialogFlow.CreateDialogFlow("start_intention")
                .NpcLineWithVariation(DramalordTexts.INTENTION_MARRY_1 + "[ib:nervous][if:convo_merry]")
                    .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                    .Variation(DramalordTexts.INTENTION_MARRY_2 + "[ib:nervous][if:convo_merry]")
                        .BeginPlayerOptions()
                            .PlayerOption(DramalordTexts.INTENTION_MARRY_REACT_OK)
                                .Condition(() => ConversationTools.SetConversationHero(Actor))
                                .BeginNpcOptions()
                                    .NpcOption(DramalordTexts.INTENTION_MARRY_REACT_NOW + "[ib:aggressive][if:convo_delighted]", () => (Hero.OneToOneConversationHero.Clan == null || Hero.OneToOneConversationHero.Clan == Clan.PlayerClan || Hero.OneToOneConversationHero.Clan?.Leader == Hero.OneToOneConversationHero) && ConversationTools.SetConversationHero(Hero.MainHero))
                                        .Consequence(() => Action())
                                        .CloseDialog()
                                    .NpcOption(DramalordTexts.NPC_INTERACTION_ENGAGE_OK + "[ib:aggressive][if:convo_delighted]", () => (Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan != Clan.PlayerClan && Hero.OneToOneConversationHero.Clan?.Leader != Hero.OneToOneConversationHero) && ConversationTools.SetConversationHero(Hero.OneToOneConversationHero.Clan.Leader))
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
                                                .CloseDialog()
                                        .EndPlayerOptions()
                                .EndNpcOptions()
                            .PlayerOption(DramalordTexts.INTENTION_REACT_NO_INTEREST)
                                .Condition(() => ConversationTools.SetConversationHero(Actor))
                                .CloseDialog()
                        .EndPlayerOptions();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            if (hero == Hero.MainHero)
            {
                IsKnownTo.Add(Hero.MainHero);
                if (hero.IsEmotionalWith(Actor) || hero.IsEmotionalWith(Target))
                {
                    Hero cheater = hero.IsEmotionalWith(Actor) ? Actor : Target;
                    Hero other = cheater == Actor ? Target : Actor;
                    DramalordInquiry.CreateYesNoInquiry(Hero.MainHero, DramalordTexts.INQUIRY_CONFRONT_TITLE, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_TEXT), cheater, other).ToString(), () =>
                    {
                        if (DramalordQuests.Instance.GetQuest(cheater) == null)
                        {
                            ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                            DramalordQuests.Instance.AddQuest(cheater, quest);
                            quest.StartQuest();
                        }
                    }, () => { });
                }

                return null;
            }
            else
            {
                if (hero.IsEmotionalWith(Actor) && hero.GetPersonality().Jealousy > 0)
                {
                    return new ConfrontEvent(hero, Actor, this);
                }
                if (hero.IsEmotionalWith(Target) && hero.GetPersonality().Jealousy > 0)
                {
                    return new ConfrontEvent(hero, Target, this);
                }

                Hero h = hero.GetCloseHeroes().GetRandomElementWithPredicate(h => !IsKnownTo.Contains(h)); ;
                if (h != null && Actor.Spouse != Target)
                {
                    return new GossiptEvent(hero, h, this);
                }
            }
            
            return null;
        }


        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            return DialogFlow.CreateDialogFlow("start_reaction")
            .BeginNpcOptions()
                .NpcOption(DramalordTexts.CONFRONTATION_MARRIAGE_OTHER + "[ib:warrior][if:convo_grave]", () => !Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Actor == Hero.MainHero ? Target : Actor)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : Actor))
                    .Consequence(() => {
                        ReactionResult(speaker, Hero.MainHero, out int trust, out int love);
                        speaker.ChangeRelationTo(Hero.MainHero, trust, love);
                    })
                    .NpcLine(DramalordTexts.CONFRONTATION_RESULT_NO_RELATION)
                        .Condition(() => ConversationTools.SetConversationHero(Hero.MainHero))
                        .CloseDialog()
                .NpcOption(DramalordTexts.CONFRONTATION_MARRIAGE_PLAYER + "[ib:warrior][if:convo_grave]", () => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Hero.MainHero)) && ConversationTools.SetConversationHero(Actor == Hero.MainHero ? Target : Actor))
                    .Consequence(() => {
                        ReactionResult(speaker, Hero.MainHero, out int trust, out int love);
                        speaker.ChangeRelationTo(Hero.MainHero, trust, love);
                    })
                    .BeginNpcOptions()
                        .NpcOption(DramalordTexts.CONFRONTATION_RESULT_BREAKUP + "[ib:warrior][if:convo_furious]", () => RelationshipEvent.CheckRelationship(speaker, Hero.MainHero) != speaker.GetRelationTo(Hero.MainHero).Relationship && ConversationTools.SetConversationHero(Hero.MainHero))
                            .CloseDialog()
                        .NpcOption(DramalordTexts.CONFRONTATION_RESULT_OK, () => RelationshipEvent.CheckRelationship(speaker, Hero.MainHero) == speaker.GetRelationTo(Hero.MainHero).Relationship && ConversationTools.SetConversationHero(Hero.MainHero))
                            .CloseDialog()
                    .EndNpcOptions()
            .EndNpcOptions();
        }

        public DialogFlow? GetGossipDialog(Hero speaker)
        {
            return DialogFlow.CreateDialogFlow("start_reaction")
                    .NpcLine(ConversationTools.SetCharacterObjects(new(DramalordTexts.GOSSIP_MARRIAGE), Actor, Target))
                    .Consequence(() =>
                    {
                        if ((Hero.MainHero.IsEmotionalWith(Actor) || Hero.MainHero.IsEmotionalWith(Target)))
                        {
                            Hero cheater = Hero.MainHero.IsEmotionalWith(Actor) ? Actor : Target;
                            Hero other = cheater == Actor ? Target : Actor;
                            if (DramalordQuests.Instance.GetQuest(cheater) == null)
                            {
                                DramalordInquiry.CreateYesNoInquiry(Hero.MainHero, DramalordTexts.INQUIRY_CONFRONT_TITLE, ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.INQUIRY_CONFRONT_TEXT), cheater, other).ToString(), () =>
                                {
                                    ConfrontHeroQuest quest = new ConfrontHeroQuest(cheater, this, CampaignTime.DaysFromNow(3));
                                    DramalordQuests.Instance.AddQuest(cheater, quest);
                                    quest.StartQuest();

                                }, () => { });
                            }
                        }
                    })
                    .BeginPlayerOptions()
                        .PlayerOption(DramalordTexts.PLAYER_INTERACTION_GOSSIP_MORE)
                            .Consequence(() => { IsKnownTo.Add(Hero.MainHero); DramalordEvents.Instance.SetNextGossipLine(Hero.OneToOneConversationHero); })
                            .GotoDialogState("start_reaction")
                        .PlayerOption(DramalordTexts.QUESTION_END)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                            .Consequence(() => IsKnownTo.Add(Hero.MainHero))
                            .CloseDialog()
                    .EndPlayerOptions();
        }

        public void ReactionResult(Hero speaker, Hero listener, out int trust, out int love)
        {
            trust = speaker.GetPersonality().Empathy -100;
            love = speaker.GetPersonality().Jealousy > 1 ? (int)(speaker.GetPersonality().Jealousy * -1) : -1;
        }
        public bool IsVisibleNotification => true;

        public override ChatNotificationType NotificationType => ChatNotificationType.PlayerClanPolitical;
    }
}
