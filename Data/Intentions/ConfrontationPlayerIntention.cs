using Dramalord.Conversations;
using Dramalord.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class ConfrontationPlayerIntention : Intention
    {
        [SaveableField(4)]
        public readonly Intention ConfrontationIntention;

        private static ConfrontationPlayerIntention? ConversationInstance() => ConversationTools.ConversationIntention as ConfrontationPlayerIntention;

        public ConfrontationPlayerIntention(Intention confrontationintention, Hero target, CampaignTime validUntil) : base(Hero.MainHero, target, validUntil)
        {
            ConfrontationIntention = confrontationintention;
        }

        public override bool Action()
        {
            ConversationTools.StartConversation(this, IntentionHero.CurrentSettlement != null);
            return true;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow playerFlow = DialogFlow.CreateDialogFlow("start", 200)
                .NpcLine("{npc_starts_confrontation_surprised}[ib:nervous][if:convo_shocked]")
                    .Condition(() => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationInstance() != null && ConversationInstance().IntentionHero != Hero.OneToOneConversationHero)
                    .PlayerLine("{CONFRONTATION_LINE}")
                        .NpcLine("{npc_interaction_reply_uhwell}[ib:nervous2][if:convo_confused_normal]")
                        .GotoDialogState("player_witness_resolution");


            DialogFlow resFlow = DialogFlow.CreateDialogFlow("player_witness_resolution")
                .BeginPlayerOptions()
                    .PlayerOption("{npc_confrontation_result_ok}")
                        .Consequence(() => ConversationTools.EndConversation())
                        .CloseDialog()
                    .PlayerOption("{npc_confrontation_result_break}")
                        .Consequence(() =>
                        {
                            new ChangeOpinionIntention(Hero.MainHero, Hero.OneToOneConversationHero, TaleWorlds.Library.MathF.Max(0, Hero.MainHero.GetRelationTo(Hero.OneToOneConversationHero).Love) * -1, TaleWorlds.Library.MathF.Max(0, Hero.MainHero.GetTrust(Hero.OneToOneConversationHero)) * -1, CampaignTime.Now).Action();
                            ConversationTools.EndConversation();
                        })
                        .CloseDialog()
                    .PlayerOption("{npc_confrontation_result_break_other}")
                        .Consequence(() =>
                        {
                            Hero otherHero = ConversationInstance().ConfrontationIntention.IntentionHero == ConversationInstance().IntentionHero ? ConversationInstance().ConfrontationIntention.Target : ConversationInstance().ConfrontationIntention.IntentionHero;
                            new ChangeOpinionIntention(Hero.OneToOneConversationHero, otherHero, TaleWorlds.Library.MathF.Max(0, otherHero.GetRelationTo(Hero.OneToOneConversationHero).Love) * -1, TaleWorlds.Library.MathF.Max(0, otherHero.GetTrust(Hero.OneToOneConversationHero)) * -1, CampaignTime.Now).Action();
                            ConversationTools.EndConversation();
                        })
                        .CloseDialog()
                .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(playerFlow);
            Campaign.Current.ConversationManager.AddDialogFlow(resFlow);
        }

        public override void OnConversationEnded()
        {
            IntentionHero.GetRelationTo(Target).LastInteraction = CampaignTime.Now;
        }

        public override void OnConversationStart()
        {
            ConfrontationPlayerIntention playerIntention = ConversationInstance();
            Intention confrontationIntention = playerIntention.ConfrontationIntention;

            Hero otherHero = confrontationIntention.IntentionHero == playerIntention.IntentionHero ? confrontationIntention.Target : confrontationIntention.IntentionHero;

            ConversationLines.npc_starts_confrontation_surprised.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, Hero.MainHero, false));

            ConversationLines.npc_confrontation_result_ok.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Target, false));
            ConversationLines.npc_confrontation_result_break.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Target, false));
            ConversationLines.npc_confrontation_result_break_other.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Target, false));
            ConversationLines.npc_confrontation_result_break_other.SetTextVariable("HERO", otherHero.Name);


            if (confrontationIntention as BetrothIntention != null)
            {
                ConversationLines.npc_confrontation_engagement_player.SetTextVariable("HERO", otherHero.Name);
                ConversationLines.npc_confrontation_engagement_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Target));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_engagement_player);
            }
            else if (confrontationIntention as GiveBirthIntention != null)
            {
                ConfrontBirthIntention i = confrontationIntention as ConfrontBirthIntention;
                ConversationLines.npc_confrontation_birth_player.SetTextVariable("CHILD", i.Child.Name);
                ConversationLines.npc_confrontation_birth_player.SetTextVariable("HERO", otherHero.Name);
                ConversationLines.npc_confrontation_birth_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Target));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_birth_player);
            }
            else if (confrontationIntention as DateIntention != null)
            {
                ConversationLines.npc_confrontation_date_player.SetTextVariable("HERO", otherHero.Name);
                ConversationLines.npc_confrontation_date_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Target));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_date_player);
            }
            else if (confrontationIntention as IntercourseIntention != null)
            {
                ConversationLines.npc_confrontation_intercourse_player.SetTextVariable("HERO", otherHero.Name);
                ConversationLines.npc_confrontation_intercourse_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Target));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_intercourse_player);
            }
            else if (confrontationIntention as MarriageIntention != null)
            {
                ConversationLines.npc_confrontation_marriage_player.SetTextVariable("HERO", otherHero.Name);
                ConversationLines.npc_confrontation_marriage_player.SetTextVariable("STATUS", ConversationTools.GetHeroRelation(Hero.MainHero, Target));
                MBTextManager.SetTextVariable("CONFRONTATION_LINE", ConversationLines.npc_confrontation_marriage_player);
            }
        }
    }
}
