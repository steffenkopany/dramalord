using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Dramalord.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;
using TaleWorlds.Localization;
using static Dramalord.Notifications.DramalordVideoNotification;

namespace Dramalord.UI
{
    public class DramalordEventNotificationItemVM : MapNotificationItemBaseVM
    {
        public IDramalordEvent Event { get; private set; }

        public DramalordEventNotificationItemVM(DramalordEventNotification data) : base(data)
        {
            Event = data.Event;

            if(data.Event is MarriageEvent)
            {
                base.NotificationIdentifier = "icon_wedding";
                _onInspect = delegate
                {
                    TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_MARRIAGE), data.Event.Actor, data.Event.Target);
                    DramalordVideoNotification.ShowDramalordVideoNotification(data.Event.Actor, data.Event.Target, txt, DramalordVideoNotification.VideoContext.Wedding, DramalordVideoNotification.VideoSound.WeddingChime);
                };
            }
            else if (data.Event is SexEvent)
            {
                 base.NotificationIdentifier = "icon_sex";
                _onInspect = delegate
                {
                    TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_SEX_GOOD), data.Event.Actor, data.Event.Target);
                    DramalordVideoNotification.ShowDramalordVideoNotification(data.Event.Actor, data.Event.Target, txt, DramalordVideoNotification.VideoContext.Intercourse, DramalordVideoNotification.VideoSound.RomanticChime);
                };
            }
            else if (data.Event is ThreesomeEvent)
            {
                base.NotificationIdentifier = "icon_sex";
                _onInspect = delegate
                {
                    TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_THREESOME), data.Event.Actor, data.Event.Target, Hero.MainHero);
                    DramalordVideoNotification.ShowDramalordVideoNotification(data.Event.Actor, data.Event.Target, Hero.MainHero, txt, DramalordVideoNotification.VideoContext.Threesome, DramalordVideoNotification.VideoSound.RomanticChime);
                };
            }
            else if (data.Event is BirthEvent birthEvent)
            {
                 base.NotificationIdentifier = "icon_birth";
                _onInspect = delegate
                {
                    TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_BIRTH), birthEvent.Actor, birthEvent.Target, birthEvent.Offspring);
                    VideoLocation vLocation = birthEvent.Actor.CurrentSettlement != null ? VideoLocation.Castle : birthEvent.Actor.PartyBelongedTo != null && birthEvent.Actor.PartyBelongedTo.IsCurrentlyAtSea ? VideoLocation.Ship : VideoLocation.Tent;
                    DramalordVideoNotification.ShowDramalordVideoNotification(txt, DramalordVideoNotification.VideoContext.Birth, DramalordVideoNotification.VideoSound.WeddingChime, vLocation);
                };
            }
            else if (data.Event is PrisonSexEvent)
            {
                base.NotificationIdentifier = "icon_prisonsex";
                _onInspect = delegate
                {
                    TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_SEX_PRISON), data.Event.Actor, data.Event.Target);
                    DramalordVideoNotification.ShowDramalordVideoNotification(data.Event.Actor, data.Event.Target, txt, DramalordVideoNotification.VideoContext.PrisonSex, DramalordVideoNotification.VideoSound.DramaticChime);
                };
            }
            else if (data.Event is RelationshipEvent)
            {
                if (data.Event.Actor.GetRelationTo(data.Event.Target).Relationship == RelationshipType.Lover)
                {
                    base.NotificationIdentifier = "icon_lover";
                }
                else if (data.Event.Actor.GetRelationTo(data.Event.Target).Relationship == RelationshipType.Friend || data.Event.Actor.GetRelationTo(data.Event.Target).Relationship == RelationshipType.None)
                {
                    base.NotificationIdentifier = "icon_divorce";
                }
                _onInspect = delegate
                {
                    if(data.Event.Actor.GetRelationTo(data.Event.Target).Relationship == RelationshipType.Lover)
                    {
                        TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_RElATIONSHIP_START), data.Event.Actor, data.Event.Target);
                        ConversationTools.SetTextVariables(txt, DramalordTexts.RELATIONSHIP_LOVERS);
                        DramalordVideoNotification.ShowDramalordVideoNotification(data.Event.Actor, data.Event.Target, txt, DramalordVideoNotification.VideoContext.Lovers, DramalordVideoNotification.VideoSound.RomanticChime);
                    }
                    else if (data.Event.Actor.GetRelationTo(data.Event.Target).Relationship == RelationshipType.Friend || data.Event.Actor.GetRelationTo(data.Event.Target).Relationship == RelationshipType.None)
                    {
                        TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), data.Event.Actor, data.Event.Target);
                        ConversationTools.SetTextVariables(txt, DramalordTexts.GetRelationshipStatusName(RelationshipType.Spouse));
                        DramalordVideoNotification.ShowDramalordVideoNotification(data.Event.Actor, data.Event.Target, txt, DramalordVideoNotification.VideoContext.Divorce, DramalordVideoNotification.VideoSound.DramaticChime);
                    }
                };
            }
            else
            {
                base.NotificationIdentifier = "default";
            }
        }
    }

    public class DramalordQuestNotificationItemVM : MapNotificationItemBaseVM
    {
        public DramalordQuest Quest { get; private set; }

        public DramalordQuestNotificationItemVM(DramalordQuestNotification data) : base(data)
        {
            Quest = data.Quest;

            if(data.Quest is MarriagePermissionQuest)
            {
                base.NotificationIdentifier = "icon_weddingquest";
            }
            else if(data.Quest is ConfrontHeroQuest)
            {
                base.NotificationIdentifier = "icon_confrontquest";
            }
            else if (data.Quest is DivorceLoverSpouseQuest)
            {
                base.NotificationIdentifier = "icon_divorcequest";
            }
            else if (data.Quest is VisitLoverQuest)
            {
                base.NotificationIdentifier = "icon_visitquest";
            }
            else if (data.Quest is BlackmailQuest)
            {
                base.NotificationIdentifier = "icon_divorcequest";
            }
            else
            {
                base.NotificationIdentifier = "default";
            }

            _onInspect = delegate
            {
                if (Quest != null)
                {
                    NavigationHandler?.OpenQuests(Quest);
                }

                //ExecuteRemove();
            };
        }
    }
}
