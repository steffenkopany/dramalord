using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications.Logs;
using Dramalord.Quests;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace Dramalord.Data.Intentions
{
    internal class ChangeOpinionIntention : Intention
    {
        [SaveableField(4)]
        public int LoveChange;

        [SaveableField(5)]
        private int TrustChange;

        public ChangeOpinionIntention(Hero intentionHero, Hero target, int loveChange, int trustChange, CampaignTime validUntil) : base(intentionHero, target, validUntil)
        {
            LoveChange = loveChange;
            TrustChange = trustChange;
        }

        public override bool Action()
        {
            IntentionHero.SetTrust(Target, IntentionHero.GetTrust(Target) + TrustChange);
            HeroRelation relation = IntentionHero.GetRelationTo(Target);
            relation.Love += LoveChange;

            int currentTrust = IntentionHero.GetTrust(Target);
            int currentLove = relation.Love;
            bool playerinvolved = IntentionHero == Hero.MainHero || Target == Hero.MainHero;

            if (playerinvolved && (LoveChange != 0 || TrustChange != 0))
            {
                Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                TextObject banner = new TextObject("{=Dramalord556}Your relation to {HERO.LINK} has changed. (Love {LOVE}, Trust {TRUST})");
                StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner);
                banner.SetTextVariable("LOVE", ConversationTools.FormatNumber(LoveChange));
                banner.SetTextVariable("TRUST", ConversationTools.FormatNumber(TrustChange));
                MBInformationManager.AddQuickInformation(banner, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
            }

            if (relation.Relationship == RelationshipType.None && currentTrust >= DramalordMCM.Instance.MinTrustFriends && currentLove >= 0)
            {
                StartRelationshipAction.Apply(IntentionHero, Target, relation, RelationshipType.Friend);

                if (DramalordMCM.Instance.RelationshipLogs && (IntentionHero.Clan == Clan.PlayerClan || Target.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions))
                {
                    LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Friend));
                }

                if (playerinvolved)
                {
                    Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                    StartRelationshipAction.Apply(otherHero, Hero.MainHero, otherHero.GetRelationTo(Hero.MainHero), RelationshipType.Friend);
                    TextObject banner2 = new TextObject("{=Dramalord079}You and {HERO.LINK} are now friends.");
                    StringHelpers.SetCharacterProperties("HERO", otherHero.CharacterObject, banner2);
                    MBInformationManager.AddQuickInformation(banner2, 0, otherHero.CharacterObject, "event:/ui/notification/relation");
                }
            }
            else if ((relation.Relationship == RelationshipType.Friend || relation.Relationship == RelationshipType.FriendWithBenefits) && currentTrust <= 0)
            {
                RelationshipType oldRelationship = relation.Relationship;
                EndRelationshipAction.Apply(IntentionHero, Target, relation);

                if (DramalordMCM.Instance.RelationshipLogs && (IntentionHero.Clan == Clan.PlayerClan || Target.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions))
                {
                    LogEntry.AddLogEntry(new EndRelationshipLog(IntentionHero, Target, oldRelationship));
                }

                if (IntentionHero == Hero.MainHero)
                {
                    TextObject textObject = new TextObject("{=Dramalord203}You ended your friendship with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", Target.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, Target.CharacterObject, "event:/ui/notification/relation");

                    DramalordQuests.Instance.GetQuest(Target)?.QuestFail(Target);
                }
                else if (Target == Hero.MainHero)
                {
                    TextObject textObject = new TextObject("{=Dramalord204}{HERO.LINK} ended their friendship with you.");
                    StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");

                    DramalordQuests.Instance.GetQuest(Target)?.QuestFail(IntentionHero);
                }
            }
            else if (!playerinvolved && (relation.Relationship == RelationshipType.None || relation.Relationship == RelationshipType.FriendWithBenefits || relation.Relationship == RelationshipType.Friend) && currentLove >= DramalordMCM.Instance.MinDatingLove)
            {
                StartRelationshipAction.Apply(IntentionHero, Target, relation, RelationshipType.Lover);
                if (DramalordMCM.Instance.RelationshipLogs && (IntentionHero.Clan == Clan.PlayerClan || Target.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions))
                {
                    LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Lover));
                }
            }
            else if((relation.Relationship == RelationshipType.Lover || relation.Relationship == RelationshipType.Betrothed || relation.Relationship == RelationshipType.Spouse || IntentionHero.Spouse == Target) && currentLove <= 0)
            {
                RelationshipType oldRelationship = relation.Relationship;
                EndRelationshipAction.Apply(IntentionHero, Target, relation);

                if (IntentionHero == Hero.MainHero)
                {
                    TextObject textObject = new TextObject("{=Dramalord088}You ended your relationship with {HERO.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", Target.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, Target.CharacterObject, "event:/ui/notification/relation");

                    DramalordQuests.Instance.GetQuest(Target)?.QuestFail(Target);
                }
                else if (Target == Hero.MainHero)
                {
                    TextObject textObject = new TextObject("{=Dramalord089}{HERO.LINK} ended their relationship with you.");
                    StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, textObject);
                    MBInformationManager.AddQuickInformation(textObject, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");

                    DramalordQuests.Instance.GetQuest(Target)?.QuestFail(Target);
                }

                if (DramalordMCM.Instance.RelationshipLogs && (IntentionHero.Clan == Clan.PlayerClan || Target.Clan == Clan.PlayerClan || !DramalordMCM.Instance.ShowOnlyClanInteractions))
                {
                    LogEntry.AddLogEntry(new EndRelationshipLog(IntentionHero, Target, oldRelationship));
                }
            }

            return true;
        }

        public override void OnConversationEnded()
        {
            Action();
        }

        public override void OnConversationStart()
        {
            
        }
    }
}
