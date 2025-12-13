using Dramalord.Conversations;
using Dramalord.Data.Events.Interfaces;
using Dramalord.Extensions;
using Dramalord.Notifications;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Events
{
    internal class DuelEvent : LogEntry, IDramalordEvent, IEncyclopediaLog, IChatNotification
    {
        [SaveableProperty(1)]
        public Hero Actor { get; private set; }

        [SaveableProperty(2)]
        public Hero Target { get; private set; }

        [SaveableProperty(3)]
        public List<Hero> IsKnownTo { get; private set; } = new();

        public bool RequiresLocation => true;

        private int ActorDamage;
        private int TargetDamage;
        private bool DuelDone;

        public DuelEvent(Hero actor, Hero target)
        {
            Actor = actor;
            Target = target;
            ActorDamage = 0;
            TargetDamage = 0;
            DuelDone = false;
            IsKnownTo.Add(Actor);
            IsKnownTo.Add(Target);
        }

        public void Action(int modifier = 1)
        {
            if(Actor != Hero.MainHero && Target != Hero.MainHero)
            {
                int actorHealth = Actor.HitPoints;
                int targetHealth = Target.HitPoints;

                ActorDamage = MBRandom.RandomInt(0, actorHealth);
                TargetDamage = MBRandom.RandomInt(0, targetHealth);

                DuelDone = true;
            }
        }

        public void AfterDialog()
        {
            if(DuelDone)
            {
                AddLogEntry(this);
                (new RelationshipEvent(Actor, Target)).Action();

                if (ActorDamage > TargetDamage)
                {
                    if (ActorDamage == 0)
                    {
                        KillCharacterAction.ApplyByWounds(Actor);
                    }
                }
                else if (ActorDamage < TargetDamage)
                {
                    if (TargetDamage == 0)
                    {
                        KillCharacterAction.ApplyByWounds(Target);
                    }
                }
                else
                {
                    if (ActorDamage == 0)
                    {
                        KillCharacterAction.ApplyByWounds(Actor);
                    }
                    if (TargetDamage == 0)
                    {
                        KillCharacterAction.ApplyByWounds(Target);
                    }
                }

                Actor.ChangeRelationTo(Target, -100, 0);

                Actor.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
            }
        }

        public bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase => (Actor == obj || Target == obj);

        public TextObject GetEncyclopediaText()
        {
            return ConversationTools.SetCharacterObjects(ActorDamage > TargetDamage ? new(DramalordTexts.LOG_DUEL_WON) : new(DramalordTexts.LOG_DUEL_LOST), Actor, Target);
        }

        public TextObject GetNotificationText()
        {
            return GetEncyclopediaText();
        }

        public IDramalordEvent? CreateReaction(Hero hero)
        {
            return null;
        }

        public DialogFlow GetInitiationDialog()
        {
            return DialogFlow.CreateDialogFlow("start_intention")
                .NpcLine(DramalordTexts.INTENTION_DIVORCE_DRAW_WEAPON + "[ib:warrior][if:convo_furious]")
                    .Consequence(() =>
                    {
                        MissionFightHandler fightHandler = Mission.Current.GetMissionBehavior<MissionFightHandler>();
                        if (fightHandler != null)
                        {
                            fightHandler.StartCustomFight(
                                new List<Agent> { Agent.Main },
                                new List<Agent> { (Agent)MissionConversationLogic.Current.ConversationManager.OneToOneConversationAgent },
                                false,
                                false,
                                (b) => OnFightEnd(b)
                                );
                        }
                    })
                    .CloseDialog();
        }

        public void OnFightEnd(bool b)
        {
            ActorDamage = (Actor == Hero.MainHero && b) ? Actor.HitPoints : 1;
            TargetDamage = (Target == Hero.MainHero && b) ? Target.HitPoints : 1;

            DuelDone = true;

            AfterDialog();

            Mission.Current.EndMission();
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

        public bool IsVisibleNotification => DramalordMCM.Instance.TalkLogs && (DramalordMCM.Instance.DEBUGLOG || Actor == Hero.MainHero || Target == Hero.MainHero);

        public override ChatNotificationType NotificationType => ChatNotificationType.Neutral;
    }
}
