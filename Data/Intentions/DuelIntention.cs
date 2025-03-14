﻿using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications.Logs;
using HarmonyLib;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.MissionLogics;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class DuelIntention : Intention
    {
        [SaveableField(4)]
        internal Hero Other;

        public DuelIntention(Hero intentionHero, Hero target, Hero other, CampaignTime validUntil) : base(intentionHero, target, validUntil)
        {
            Other = other;
        }

        public override bool Action()
        {
            if(IntentionHero == Hero.MainHero || Target == Hero.MainHero)
            {
                MissionFightHandler fightHandler = Mission.Current.GetMissionBehavior<MissionFightHandler>();
                if (fightHandler != null)
                {
                    fightHandler.StartCustomFight(
                        new List<Agent> { Agent.Main },
                        new List<Agent> { (Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents.First() },
                        false,
                        false,
                        (b) => OnFightEnd(b)
                        );
                }
            }
            else
            {
                OnFightEnd(false);
            }

            return true;
        }

        public override void OnConversationEnded()
        {
            MissionFightHandler fightHandler = Mission.Current.GetMissionBehavior<MissionFightHandler>();
            if(fightHandler != null)
            {
                fightHandler.StartCustomFight(
                    new List<Agent> { Agent.Main },
                    new List<Agent> { (Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents.First() },
                    false,
                    false,
                    (b) => OnFightEnd(b)
                    );
            }
        }

        private void OnFightEnd(bool playerWon)
        {
            if (playerWon)
            {
                Hero winner = IntentionHero == Hero.MainHero ? IntentionHero : Target;
                Hero looser = IntentionHero == Hero.MainHero ? Target : IntentionHero;

                if(Target.IsAlive)
                {
                    if(DramalordMCM.Instance.AlwaysDuelToDeath)
                    {
                        KillCharacterAction.ApplyByBattle(looser, winner, true);
                    }
                    else
                    {
                        RelationshipType oldRelationship = looser.GetRelationTo(Other).Relationship;
                        EndRelationshipAction.Apply(looser, Other, looser.GetRelationTo(Other));

                        TextObject textObject = new EndRelationshipLog(looser, Other, oldRelationship).GetEncyclopediaText();
                        MBInformationManager.AddQuickInformation(textObject, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
                    } 
                }
                MakeFamilyHate(looser, winner, Other);
            }
            else
            {
                Hero winner = (IntentionHero == Hero.MainHero || Target == Hero.MainHero) ? (IntentionHero == Hero.MainHero ? Target : IntentionHero) : (MBRandom.RandomInt(1,100) > 50 ? IntentionHero : Target);
                Hero looser = IntentionHero == winner ? Target : IntentionHero;

                if (IntentionHero.IsAlive)
                {
                    if (DramalordMCM.Instance.AlwaysDuelToDeath)
                    {
                        KillCharacterAction.ApplyByBattle(looser, winner, true);
                    }
                    else
                    {
                        RelationshipType oldRelationship = looser.GetRelationTo(Other).Relationship;
                        EndRelationshipAction.Apply(looser, Other, looser.GetRelationTo(Other));

                        TextObject textObject = new EndRelationshipLog(looser, Other, oldRelationship).GetEncyclopediaText();
                        MBInformationManager.AddQuickInformation(textObject, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");
                    }
                }
                MakeFamilyHate(looser, winner, Other);
            }
        }

        private void MakeFamilyHate(Hero victim, Hero winner, Hero love)
        {
            if(victim.Father != null)
            {
                MakeFamilyHate(victim.Father, winner, love);
            }

            if(victim.Mother != null)
            {
                MakeFamilyHate(victim.Mother, winner, love);
            }

            if(victim.IsDramalordLegit())
            {
                new ChangeOpinionIntention(victim, winner, MathF.Abs(-100 - victim.GetRelationTo(winner).Love) * -1, MathF.Abs(-100 - victim.GetTrust(winner)) * -1, CampaignTime.Now).Action();
            }

            victim.Siblings.Where(s => s != love && s != victim && s.IsDramalordLegit()).Do(s => new ChangeOpinionIntention(s, winner, MathF.Abs(-100 - s.GetRelationTo(winner).Love) * -1, MathF.Abs(-100 - s.GetTrust(winner)) * -1, CampaignTime.Now).Action());
            victim.Children.Where(s => s != love && s != victim && s.IsDramalordLegit()).Do(s => new ChangeOpinionIntention(s, winner, MathF.Abs(-100 - s.GetRelationTo(winner).Love) * -1, MathF.Abs(-100 - s.GetTrust(winner)) * -1, CampaignTime.Now).Action());
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow startFlow = DialogFlow.CreateDialogFlow("hero_main_options", 200)
                .BeginPlayerOptions()
                    .PlayerOption("{player_interaction_duel}")
                        .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && SetupLines() && ((Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse.IsEmotionalWith(Hero.MainHero)) ||Hero.MainHero.GetAllRelations().Where(r => r.Value.Relationship == RelationshipType.Spouse && r.Key.IsEmotionalWith(Hero.OneToOneConversationHero)).Any()))
                        .BeginNpcOptions()
                            .NpcOption("{npc_interaction_duel}", () => Mission.Current != null && Mission.Current.GetMissionBehavior<MissionFightHandler>() != null && Hero.OneToOneConversationHero.CurrentSettlement != null && Hero.OneToOneConversationHero.CurrentSettlement.Town != null)
                                .Consequence(() => ConversationTools.ConversationIntention = new DuelIntention(Hero.MainHero, Hero.OneToOneConversationHero, (Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse.IsEmotionalWith(Hero.MainHero)) ? Hero.OneToOneConversationHero.Spouse : Hero.MainHero.GetAllRelations().FirstOrDefault(r => r.Value.Relationship == RelationshipType.Spouse && r.Key.IsEmotionalWith(Hero.OneToOneConversationHero)).Key, CampaignTime.Now))
                                .CloseDialog()
                            .NpcOption("{npc_interaction_duel_visit}", () => Hero.OneToOneConversationHero.CurrentSettlement != null && Hero.OneToOneConversationHero.CurrentSettlement.Town != null && (Mission.Current == null || Mission.Current.GetMissionBehavior<MissionFightHandler>() == null))
                                .CloseDialog()
                            .NpcOption("{npc_interaction_duel_town}", () => Hero.OneToOneConversationHero.CurrentSettlement == null || Hero.OneToOneConversationHero.CurrentSettlement.Town == null)
                            .Consequence(() => ConversationTools.EndConversation())
                                .CloseDialog()
                        .EndNpcOptions()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(startFlow);
        }

        private static bool SetupLines()
        {
            ConversationLines.player_interaction_duel.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, true));
            if (Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse.IsEmotionalWith(Hero.MainHero))
            {
                ConversationLines.player_interaction_duel.SetTextVariable("HERO", Hero.OneToOneConversationHero.Spouse?.Name);
            }
            else
            {
                ConversationLines.player_interaction_duel.SetTextVariable("HERO",
                    Hero.MainHero.GetAllRelations().FirstOrDefault(r => r.Value.Relationship == RelationshipType.Spouse && r.Key.IsEmotionalWith(Hero.OneToOneConversationHero)).Key?.Name);
            }
            ConversationLines.npc_interaction_duel.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, true));
            ConversationLines.npc_interaction_duel_visit.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, true));
            ConversationLines.npc_interaction_duel_town.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, true));
            return true;
        }

        public override void OnConversationStart()
        {
            
        }
    }
}
