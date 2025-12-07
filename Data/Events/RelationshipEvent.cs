using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class RelationshipEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        [SaveableField(4)]
        private TextObject NotificationLine = TextObject.GetEmpty();

        private bool PlayerTriggered = false;

        public RelationshipEvent(Hero actor, Hero target, bool playerTriggered = false)
        {
            Actor = actor;
            Target = target;
            PlayerTriggered = playerTriggered;
            IsKnownTo.Add(Actor);
            IsKnownTo.Add(Target);
        }

        public void Action(int modifier = 0)
        {
            HeroRelation relation = Actor.GetRelationTo(Target);

            RelationshipType shouldRelation = CheckRelationship(Actor, Target);

            if(relation.Relationship == RelationshipType.None && shouldRelation == RelationshipType.Friend)
            {
                relation.Relationship = RelationshipType.Friend;
                NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_START), Actor, Target);
                ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(RelationshipType.Friend));

                if (Actor == Hero.MainHero || Target == Hero.MainHero)
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, new(DramalordTexts.BANNER_RELATIONSHIP_START), DramalordTexts.GetRelationshipStatusName(RelationshipType.Friend), true);
                }
                AddLogEntry(this);
            }
            else if (relation.Relationship == RelationshipType.Friend && shouldRelation == RelationshipType.None)
            {
                relation.SetBlockedUntil(CampaignTime.DaysFromNow(14));
                relation.Relationship = RelationshipType.None;
                NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), Actor, Target);
                ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(RelationshipType.Friend));

                if (Actor == Hero.MainHero || Target == Hero.MainHero)
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, new(DramalordTexts.BANNER_RELATIONSHIP_END), DramalordTexts.GetRelationshipStatusName(RelationshipType.Friend), true);
                }
                AddLogEntry(this);
            }
            else if (relation.Relationship == RelationshipType.Friend && shouldRelation == RelationshipType.Lover && PlayerTriggered)
            {
                relation.Relationship = RelationshipType.Lover;
                NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_START), Actor, Target);
                ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(RelationshipType.Lover));

                if ((Actor == Hero.MainHero || Target == Hero.MainHero))
                {
                    relation.Relationship = RelationshipType.Lover;
                    NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_START), Actor, Target);
                    ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(RelationshipType.Lover));

                    if (DramalordMCM.Instance.ShowDramaVideos)
                    {
                        DramalordVideoNotification.ShowDramalordVideoNotification(Actor, Target, null, DramalordVideoNotification.VideoContext.Lovers);
                    }
                    else
                    {
                        DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, new(DramalordTexts.BANNER_RELATIONSHIP_START), DramalordTexts.GetRelationshipStatusName(RelationshipType.Lover), true);
                    }
                    AddLogEntry(this);
                }
                else
                {
                    relation.Relationship = RelationshipType.Lover;
                    NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_START), Actor, Target);
                    ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(RelationshipType.Lover));
                    AddLogEntry(this);
                }      
            }
            else if (relation.Relationship == RelationshipType.Lover && shouldRelation == RelationshipType.Friend)
            {
                NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), Actor, Target);
                ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(relation.Relationship));

                if (Actor == Hero.MainHero || Target == Hero.MainHero)
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, new(DramalordTexts.BANNER_RELATIONSHIP_END), DramalordTexts.GetRelationshipStatusName(relation.Relationship), true);
                }
                relation.Relationship = RelationshipType.Friend;
                AddLogEntry(this);
            }
            else if (relation.Relationship == RelationshipType.Lover && shouldRelation == RelationshipType.None)
            {
                NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), Actor, Target);
                ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(relation.Relationship));

                if (Actor == Hero.MainHero || Target == Hero.MainHero)
                {
                    DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, new(DramalordTexts.BANNER_RELATIONSHIP_END), DramalordTexts.GetRelationshipStatusName(relation.Relationship), true);
                }
                relation.SetBlockedUntil(CampaignTime.DaysFromNow(14));
                relation.Relationship = RelationshipType.None;
                AddLogEntry(this);
            }
            else if (relation.Relationship == RelationshipType.Spouse && shouldRelation == RelationshipType.Friend)
            {
                NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), Actor, Target);
                ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(relation.Relationship));

                if (Actor == Hero.MainHero || Target == Hero.MainHero)
                {
                    if (DramalordMCM.Instance.ShowDramaVideos)
                    {
                        DramalordVideoNotification.ShowDramalordVideoNotification(Actor, Target, null, DramalordVideoNotification.VideoContext.Divorce);
                    }
                    else
                    {
                        DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, new(DramalordTexts.BANNER_RELATIONSHIP_END), DramalordTexts.GetRelationshipStatusName(relation.Relationship), true);
                    }
                }

                foreach (Romance.RomanticState romanticState in Romance.RomanticStateList.ToList())
                {
                    if ((romanticState.Person1 == Target && romanticState.Person2 == Actor) || (romanticState.Person1 == Actor && romanticState.Person2 == Target))
                    {
                        romanticState.Level = Romance.RomanceLevelEnum.FailedInPracticalities;
                    }
                }

                relation.Relationship = RelationshipType.Friend;

                if (Actor == Hero.MainHero && Actor.Spouse == Target)
                {
                    Actor.Spouse = Hero.MainHero.GetAllRelations().FirstOrDefault(r => r.Value.Relationship == RelationshipType.Spouse).Key ?? null;
                    Target.Spouse = null;
                }
                else if (Target == Hero.MainHero && Target.Spouse == Actor)
                {
                    Target.Spouse = Hero.MainHero.GetAllRelations().FirstOrDefault(r => r.Value.Relationship == RelationshipType.Spouse).Key ?? null;
                    Actor.Spouse = null;
                }
                else
                {
                    Actor.Spouse = null;
                    Target.Spouse = null;
                }
                
                AddLogEntry(this);
            }
            else if (relation.Relationship == RelationshipType.Spouse && shouldRelation == RelationshipType.None)
            {
                NotificationLine = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), Actor, Target);
                ConversationTools.SetTextVariables(NotificationLine, DramalordTexts.GetRelationshipStatusName(relation.Relationship));

                if (Actor == Hero.MainHero || Target == Hero.MainHero)
                {
                    if(DramalordMCM.Instance.ShowDramaVideos)
                    {
                        DramalordVideoNotification.ShowDramalordVideoNotification(Actor, Target, null, DramalordVideoNotification.VideoContext.Divorce);
                    }
                    else
                    {
                        DramalordBanner.CreateBanner(Actor == Hero.MainHero ? Target : Actor, new(DramalordTexts.BANNER_RELATIONSHIP_END), DramalordTexts.GetRelationshipStatusName(relation.Relationship), true);
                    }
                }

                foreach (Romance.RomanticState romanticState in Romance.RomanticStateList.ToList())
                {
                    if ((romanticState.Person1 == Target && romanticState.Person2 == Actor) || (romanticState.Person1 == Actor && romanticState.Person2 == Target))
                    {
                        romanticState.Level = Romance.RomanceLevelEnum.FailedInPracticalities;
                    }
                }

                relation.SetBlockedUntil(CampaignTime.DaysFromNow(14));
                relation.Relationship = RelationshipType.None;

                if (Actor == Hero.MainHero && Actor.Spouse == Target)
                {
                    Actor.Spouse = Hero.MainHero.GetAllRelations().FirstOrDefault(r => r.Value.Relationship == RelationshipType.Spouse).Key ?? null;
                    Target.Spouse = null;
                }
                else if (Target == Hero.MainHero && Target.Spouse == Actor)
                {
                    Target.Spouse = Hero.MainHero.GetAllRelations().FirstOrDefault(r => r.Value.Relationship == RelationshipType.Spouse).Key ?? null;
                    Actor.Spouse = null;
                }
                else
                {
                    Actor.Spouse = null;
                    Target.Spouse = null;
                }
                AddLogEntry(this);
            }
        }

        public static RelationshipType CheckRelationship(Hero hero1, Hero hero2)
        {
            HeroRelation relation = hero1.GetRelationTo(hero2);
            int currentTrust = hero1.GetTrust(hero2);
            int currentLove = relation.Love;

            if (relation.Relationship == RelationshipType.None && currentTrust >= DramalordMCM.Instance.MinTrustFriends && currentLove >= 0)
            {
                return RelationshipType.Friend;
            }
            else if (relation.Relationship == RelationshipType.Friend && currentTrust <= 0)
            {
                return RelationshipType.None;
            }
            else if (relation.Relationship == RelationshipType.Friend && currentTrust > 0 && currentLove >= DramalordMCM.Instance.MinDatingLove)
            {
                return RelationshipType.Lover;
            }
            else if ((relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Spouse) && currentTrust >= DramalordMCM.Instance.MinTrustFriends && currentLove <= 0)
            {
                return RelationshipType.Friend;
            }
            else if ((relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Spouse) && currentTrust < DramalordMCM.Instance.MinTrustFriends && currentLove <= 0)
            {
                return RelationshipType.None;
            }

            return relation.Relationship;
        }

        public void AfterDialog()
        {
            //nothing to do
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => IsVisibleNotification && (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return NotificationLine;
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
            return null;
        }

        public DialogFlow? GetConfrontationDialog(Hero speaker)
        {
            return null;
        }

        public DialogFlow? GetGossipDialog(Hero speaker)
        {
            return null;
        }

        public void ReactionResult(Hero speaker, Hero listener, out int trust, out int love)
        {
            trust = 0;
            love = 0;
        }

        public bool IsVisibleNotification => DramalordMCM.Instance.RelationshipLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Neutral;
    }
}
