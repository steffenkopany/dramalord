using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Quests;
using Helpers;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class BirthEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public Hero? Offspring { get; private set; }

        [SaveableProperty(4)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        public BirthEvent(Hero actor, Hero target)
        {
            Actor = actor.IsFemale ? actor : target;
            Target = target.IsFemale ? actor : target;
            IsKnownTo.Add(actor);
            IsKnownTo.Add(target);
        }

        public void Action()
        {
            try
            {
                Offspring = HeroCreator.DeliverOffSpring(Actor, Target, MBRandom.RandomInt(0, 100) < 50);
                Actor.IsPregnant = false;
                Offspring.Clan = Actor.Clan;
                return;
            }
            catch (Exception e)
            {
            }

            try
            {
                Offspring = CreateBaby(Actor, Target);
                Actor.IsPregnant = false;
            }
            catch
            {
                Offspring = null;
            }
        }

        public void AfterDialog()
        {
            AddLogEntry(this);

            if (Offspring != null && (Actor.Clan == Clan.PlayerClan || Target.Clan == Clan.PlayerClan))
            {
                MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(Target, Actor, CampaignTime.Now));
                MBInformationManager.AddNotice(new ChildBornMapNotification(Offspring, GetEncyclopediaText(), CampaignTime.Now));
            }

            if (!Actor.IsSpouseOf(Target))
            {
                DramalordEvents.Instance.StartIntention(new OrphanizeEvent(Actor, Offspring));
            }
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return (Offspring == null) ? ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_BIRTH_DEATH), Actor, Target) : ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_BIRTH), Actor, Target, Offspring);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public DialogFlow? GetInitiationDialog()
        {
            return null;
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
            if(Offspring == null)
            {
                return null;
            }

            return DialogFlow.CreateDialogFlow("start_reaction")
            .BeginNpcOptions()
                .NpcOption(DramalordTexts.CONFRONTATION_BIRTH_OTHER + "[ib:warrior][if:convo_grave]", () => Target == Hero.MainHero && speaker.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Hero.MainHero)) && ConversationTools.SetConversationHero(Offspring, Actor))
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
                .NpcOption(DramalordTexts.CONFRONTATION_BIRTH_PLAYER + "[ib:warrior][if:convo_grave]", () => Actor == Hero.MainHero && speaker.IsEmotionalWith(Hero.MainHero) && ConversationTools.SetConversationText(ConversationTools.GetHeroRelation(speaker, Hero.MainHero)) && ConversationTools.SetConversationHero(Offspring, Target))
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
                    .NpcLine(ConversationTools.SetCharacterObjects(new(DramalordTexts.GOSSIP_BIRTH), Actor, Target))
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
            trust = speaker.GetPersonality().Empathy > 1 ? -100 + (int)(speaker.GetPersonality().Empathy * 0.25) : -1;
            love = speaker.GetPersonality().Jealousy > 1 ? (int)(speaker.GetPersonality().Jealousy * -1) : -1;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero;

        public override ChatNotificationType NotificationType => ChatNotificationType.Civilian;

        public static Hero CreateBaby(Hero mother, Hero father)
        {
            CharacterObject? template = null;
            if (mother.IsLord && father.IsLord)
            {
                template = (MBRandom.RandomInt(1, 100) > 50) ? mother.CharacterObject : father.CharacterObject;
            }
            else
            {
                template = mother.IsLord ? mother.CharacterObject : father.IsLord ? father.CharacterObject : Hero.AllAliveHeroes.GetRandomElementWithPredicate(h => h.IsLord && h.Clan != Clan.PlayerClan).CharacterObject;
            }

            Settlement bornSettlement = mother.CurrentSettlement ?? father.HomeSettlement ?? SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);

            Clan? faction = mother.Clan;
            Hero child = HeroCreator.CreateSpecialHero(template, bornSettlement, faction, null, 0);
            child.Mother = mother;
            child.Father = father;
            child.HeroDeveloper.InitializeHeroDeveloper();
            BodyProperties bodyProperties = mother.BodyProperties;
            BodyProperties bodyProperties2 = father.BodyProperties;
            TaleWorlds.Core.FaceGen.GenerateParentKey(child.BodyProperties, MBRandom.RandomInt(0, 1) == 0 ? mother.CharacterObject.Race : mother.CharacterObject.Race, ref bodyProperties, ref bodyProperties2);
            child.SetNewOccupation(mother.Occupation);

            if (child.Occupation == Occupation.Lord)
            {
                child.SetName(child.FirstName, child.FirstName);
            }

            return child;
        }
    }
}
