using Dramalord.Actions;
using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.Notifications.Logs;
using Helpers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Data.Intentions
{
    internal class MarriageIntention : Intention
    {
        private static bool _accepted = false;

        public MarriageIntention(Hero target, Hero intentionHero, CampaignTime validUntil) : base(intentionHero, target, validUntil)
        {
            _accepted = true;
        }

        public override bool Action()
        {
            _accepted = false;

            List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
            if (Target == Hero.MainHero && closeHeroes.Contains(Hero.MainHero) && ConversationTools.StartConversation(this, true))
            {
                return true;
            }
            else if (Target != Hero.MainHero && closeHeroes.Contains(Target))
            {
                _accepted = true;
                OnConversationEnded();
                return true;
            }

            return false;
        }

        public override void OnConversationEnded()
        {
            IntentionHero.GetRelationTo(Target).LastInteraction = CampaignTime.Now;

            if (_accepted)
            {
                HeroRelation relation = IntentionHero.GetRelationTo(Target);
                StartRelationshipAction.Apply(IntentionHero, Target, relation, RelationshipType.Spouse);

                Hero groom = IntentionHero.IsFemale ? Target : IntentionHero;
                Hero bride = groom == IntentionHero ? Target : IntentionHero;

                if ((groom.Clan == Clan.PlayerClan && bride.Clan != Clan.PlayerClan) || (groom.Clan != Clan.PlayerClan && bride.Clan == Clan.PlayerClan))
                {
                    int speed = (int)Campaign.Current.TimeControlMode;
                    Campaign.Current.SetTimeSpeed(0);
                    TextObject title = new TextObject("{=Dramalord574}Join Clan On Marriage");
                    TextObject text = new TextObject("{=Dramalord578}{HERO1} and {HERO2} are ready for the altar. Will you welcome them in your clan after the ceremony?");
                    text.SetTextVariable("HERO1", groom.Name);
                    text.SetTextVariable("HERO2", bride.Name);
                    if(IntentionHero == Hero.MainHero || Target == Hero.MainHero)
                    {
                        Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                        text = new TextObject("{=Dramalord560}{HERO} is getting ready for the ceremony. Do you want to them to join your clan after getting married?");
                        text.SetTextVariable("HERO", otherHero.Name);
                    }
                    InformationManager.ShowInquiry(
                            new InquiryData(
                                title.ToString(),
                                text.ToString(),
                                true,
                                true,
                                GameTexts.FindText("str_yes").ToString(),
                                GameTexts.FindText("str_no").ToString(),
                                () => {
                                    Hero clanHero = (groom.Clan == Clan.PlayerClan) ? groom : bride;
                                    Hero otherHero = (groom.Clan == Clan.PlayerClan) ? bride : groom;
                                    LeaveClanAction.Apply(otherHero);
                                    JoinClanAction.Apply(otherHero, Clan.PlayerClan);
                                    ChangeOccupationAfterMarriage(otherHero, otherHero.Occupation != Occupation.Lord ? clanHero.Occupation : otherHero.Occupation);

                                    TextObject textObject = new TextObject("{=Dramalord080}{HERO.LINK} married {TARGET.LINK}.");
                                    StringHelpers.SetCharacterProperties("HERO", groom.CharacterObject, textObject);
                                    StringHelpers.SetCharacterProperties("TARGET", bride.CharacterObject, textObject);

                                    MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(groom, bride, CampaignTime.Now));
                                    MBInformationManager.AddNotice(new MarriageMapNotification(groom, bride, textObject, CampaignTime.Now));

                                    HandleWitness();

                                    if (DramalordMCM.Instance.RelationshipLogs)
                                    {
                                        LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Spouse));
                                    }

                                    Campaign.Current.SetTimeSpeed(speed);
                                },
                                () => {

                                    Hero clanHero = (groom.Clan == Clan.PlayerClan) ? groom : bride;
                                    Hero otherHero = (groom.Clan == Clan.PlayerClan) ? bride : groom;
                                    LeaveClanAction.Apply(clanHero);
                                    if (otherHero.Clan != null)
                                    {
                                        JoinClanAction.Apply(clanHero, otherHero.Clan);
                                        ChangeOccupationAfterMarriage(clanHero, Occupation.Lord);
                                    }
                                    else
                                    {
                                        ChangeOccupationAfterMarriage(clanHero, otherHero.Occupation);
                                    }

                                    TextObject textObject = new TextObject("{=Dramalord080}{HERO.LINK} married {TARGET.LINK}.");
                                    StringHelpers.SetCharacterProperties("HERO", groom.CharacterObject, textObject);
                                    StringHelpers.SetCharacterProperties("TARGET", bride.CharacterObject, textObject);

                                    MBInformationManager.AddQuickInformation(textObject, 1000, clanHero.CharacterObject, "event:/ui/notification/relation");

                                    HandleWitness();

                                    if (DramalordMCM.Instance.RelationshipLogs)
                                    {
                                        LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Spouse));
                                    }

                                    Campaign.Current.SetTimeSpeed(speed);
                                }), true);
                    
                    
                }
                else if(groom.Clan == Clan.PlayerClan && bride.Clan == Clan.PlayerClan)
                {
                    TextObject textObject = new TextObject("{=Dramalord080}{HERO.LINK} married {TARGET.LINK}.");
                    StringHelpers.SetCharacterProperties("HERO", groom.CharacterObject, textObject);
                    StringHelpers.SetCharacterProperties("TARGET", bride.CharacterObject, textObject);

                    MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(groom, bride, CampaignTime.Now));
                    MBInformationManager.AddNotice(new MarriageMapNotification(groom, bride, textObject, CampaignTime.Now));

                    HandleWitness();
                }
                else if(DramalordMCM.Instance.JoinClanOnMarriage)
                {
                    if (groom.Clan != bride.Clan)
                    {
                        if (groom.Clan != null && bride.Clan != null)
                        {
                            if (bride.Clan.Leader == bride)
                            {
                                LeaveClanAction.Apply(groom);
                                JoinClanAction.Apply(groom, bride.Clan);
                                ChangeOccupationAfterMarriage(groom, bride.Occupation);
                            }
                            else
                            {
                                LeaveClanAction.Apply(bride);
                                JoinClanAction.Apply(bride, groom.Clan);
                                ChangeOccupationAfterMarriage(bride, groom.Occupation);
                            }
                        }
                        else if (groom.Clan != null)
                        {
                            LeaveClanAction.Apply(bride);
                            JoinClanAction.Apply(bride, groom.Clan);
                            ChangeOccupationAfterMarriage(bride, groom.Occupation);
                        }
                        else if (bride.Clan != null)
                        {
                            LeaveClanAction.Apply(groom);
                            JoinClanAction.Apply(groom, bride.Clan);
                            ChangeOccupationAfterMarriage(groom, bride.Occupation);
                        }
                        else
                        {
                            //ChangeOccupationAfterMarriage(bride, groom.Occupation);
                        }
                    }

                    if (DramalordMCM.Instance.RelationshipLogs)
                    {
                        LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Spouse));
                    }
                }
                else if (DramalordMCM.Instance.RelationshipLogs)
                {
                    LogEntry.AddLogEntry(new StartRelationshipLog(IntentionHero, Target, RelationshipType.Spouse));
                }

                HandleWitness();
            }
            else if (!_accepted)
            {
                RelationshipLossAction.Apply(IntentionHero, Target, out int loveDamage, out int trustDamage, 10, 20);
                new ChangeOpinionIntention(IntentionHero, Target, loveDamage, trustDamage, CampaignTime.Now).Action();

                if ((IntentionHero == Hero.MainHero || Target == Hero.MainHero) && (loveDamage != 0 || trustDamage != 0))
                {
                    Hero other = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                    TextObject banner = new TextObject("{=Dramalord179}Relation loss with {HERO.LINK}. (Love {NUM}, Trust {NUM2})");
                    StringHelpers.SetCharacterProperties("HERO", other.CharacterObject, banner);
                    MBTextManager.SetTextVariable("NUM", ConversationTools.FormatNumber((int)loveDamage));
                    MBTextManager.SetTextVariable("NUM2", ConversationTools.FormatNumber((int)trustDamage));
                    MBInformationManager.AddQuickInformation(banner, 0, Hero.OneToOneConversationHero.CharacterObject, "event:/ui/notification/relation");
                }
            }

            _accepted = false;
        }

        internal void HandleWitness()
        {
            List<Hero> closeHeroes = IntentionHero.GetCloseHeroes();
            Hero? witness = DramalordMCM.Instance.PlayerAlwaysWitness && Target != Hero.MainHero && closeHeroes.Contains(Hero.MainHero) ? Hero.MainHero : closeHeroes.GetRandomElementWithPredicate(h => h != Target);
            if (witness != null)
            {
                if (witness != Target && (witness.IsEmotionalWith(IntentionHero) || witness.IsEmotionalWith(Target)))
                {
                    if (witness != Hero.MainHero)
                    {
                        DramalordIntentions.Instance.GetIntentions().Add(new ConfrontMarriageIntention(IntentionHero, Target, witness, CampaignTime.DaysFromNow(7), true));
                        DramalordIntentions.Instance.GetIntentions().Add(new ConfrontMarriageIntention(Target, IntentionHero, witness, CampaignTime.DaysFromNow(7), true));
                    }

                    if (witness == Hero.MainHero)
                    {
                        TextObject banner = new TextObject("{=Dramalord531}You witnessed {HERO.LINK} getting married to {OTHER.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", IntentionHero.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", Target.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, IntentionHero.CharacterObject, "event:/ui/notification/relation");

                        int speed = (int)Campaign.Current.TimeControlMode;
                        Campaign.Current.SetTimeSpeed(0);
                        TextObject title = new TextObject("{=Dramalord579}Catch {HERO1} and {HERO2} in the act");
                        TextObject text = new TextObject("{=Dramalord580}You caught {HERO1} and {HERO2} in the act. Do you wish to interrupt them?");
                        title.SetTextVariable("HERO1", IntentionHero.Name);
                        title.SetTextVariable("HERO2", Target.Name);
                        text.SetTextVariable("HERO1", IntentionHero.Name);
                        text.SetTextVariable("HERO2", Target.Name);
                        InformationManager.ShowInquiry(
                                new InquiryData(
                                    title.ToString(),
                                    text.ToString(),
                                    true,
                                    true,
                                    GameTexts.FindText("str_yes").ToString(),
                                    GameTexts.FindText("str_no").ToString(),
                                    () => { new ConfrontationPlayerIntention(this, Hero.MainHero.IsEmotionalWith(IntentionHero) ? IntentionHero : Target, CampaignTime.Now).Action(); },
                                    () => { Campaign.Current.SetTimeSpeed(speed); }), true);

                    }
                    else if (IntentionHero == Hero.MainHero || Target == Hero.MainHero)
                    {
                        Hero otherHero = (IntentionHero == Hero.MainHero) ? Target : IntentionHero;
                        TextObject banner = new TextObject("{=Dramalord530}{HERO.LINK} saw you getting married to {TARGET.LINK}.");
                        StringHelpers.SetCharacterProperties("HERO", witness.CharacterObject, banner);
                        StringHelpers.SetCharacterProperties("TARGET", otherHero.CharacterObject, banner);
                        MBInformationManager.AddQuickInformation(banner, 0, witness.CharacterObject, "event:/ui/notification/relation");
                    }
                }
                else if (witness != Target && witness != Hero.MainHero)
                {
                    List<Hero> targets = new() { IntentionHero, Target };
                    DramalordIntentions.Instance.GetIntentions().Add(new GossipMarriageIntention(this, true, targets, witness, CampaignTime.DaysFromNow(7)));
                }
            }
        }

        internal void ChangeOccupationAfterMarriage(Hero changeHero, Occupation newOccupation)
        {
            if(newOccupation == changeHero.Occupation)
            {
                return;
            }
            else if(newOccupation == Occupation.Wanderer && changeHero.Occupation != Occupation.Wanderer)
            {
                TextObject newName = new TextObject("{=28tWEFNi}{FIRSTNAME} the Wanderer");
                newName.SetTextVariable("FIRSTNAME", changeHero.FirstName);
                changeHero.SetName(newName, changeHero.FirstName);
            }
            else if(newOccupation == Occupation.Lord)
            {
                changeHero.SetName(changeHero.FirstName, changeHero.FirstName);
            }
            else if (newOccupation != Occupation.Wanderer && newOccupation != Occupation.Lord && (changeHero.Occupation == Occupation.Wanderer || changeHero.Occupation == Occupation.Lord))
            {
                TextObject newName = new TextObject("{=asePjBVy}{FIRSTNAME} the {?FEMALE}Freedwoman{?}Freedman{\\?}");
                newName.SetTextVariable("FIRSTNAME", changeHero.FirstName);
                changeHero.SetName(newName, changeHero.FirstName);
            }

            changeHero.SetNewOccupation(newOccupation);
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start", 200)
                .BeginNpcOptions()
                    .NpcOption("{npc_starts_interaction_unknown}[ib:nervous][if:convo_nervous]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as MarriageIntention != null && !Hero.OneToOneConversationHero.HasMet)
                        .Consequence(() => Hero.OneToOneConversationHero.SetHasMet())
                        .GotoDialogState("start_marriage")
                    .NpcOption("{npc_starts_interaction_known}[ib:nervous2][if:convo_confused_normal]", () => Hero.OneToOneConversationHero.IsDramalordLegit() && ConversationTools.ConversationIntention as MarriageIntention != null && Hero.OneToOneConversationHero.HasMet)
                        .GotoDialogState("start_marriage")
                .EndNpcOptions();

            DialogFlow flow2 = DialogFlow.CreateDialogFlow("start_marriage")
                .BeginPlayerOptions()
                    .PlayerOption("{player_interaction_start_react_yes}")
                        .GotoDialogState("start_marriage2")
                    .PlayerOption("{player_interaction_start_react_no}")
                        .Consequence(() => ConversationTools.EndConversation())
                        .CloseDialog()
                .EndPlayerOptions();

            DialogFlow flow3 = DialogFlow.CreateDialogFlow("start_marriage2")
                .NpcLineWithVariation("{npc_interaction_marriage_1}[ib:confident3][if:convo_excited]")
                    .Variation("{npc_interaction_marriage_2}[ib:confident3][if:convo_excited]")
                    .Condition(() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                    .BeginPlayerOptions()
                        .PlayerOption("{player_reaction_marry_yes}")
                            .Condition(() => Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero))
                            .Consequence(() => { _accepted = true; ConversationTools.EndConversation(); })
                            .CloseDialog()
                        .PlayerOption("{player_reaction_no}")
                            .Consequence(() => { _accepted = false; ConversationTools.EndConversation(); })
                            .CloseDialog()
                    .EndPlayerOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
            Campaign.Current.ConversationManager.AddDialogFlow(flow2);
            Campaign.Current.ConversationManager.AddDialogFlow(flow3);
        }

        public override void OnConversationStart()
        {
            ConversationLines.npc_starts_interaction_unknown.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));// "{=Dramalord027}Excuse me, {TITLE}, we have never met but I could not help myself asking you for a few minutes of your time."
            ConversationLines.npc_starts_interaction_known.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, true));// "{=Dramalord028}{TITLE}, it is good to see you! May I humbly request to occupy some of your time?");
            ConversationLines.player_interaction_start_react_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord029}Of course {TITLE}. How can I be of service?");
            ConversationLines.player_interaction_start_react_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord030}My apologies, {TITLE}, but I am short of time right now.");
            ConversationLines.npc_interaction_marriage_1.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord053}Now that we are in a settlement, {TITLE}, let's call a priest and finally get married! What do you say, {TITLE}?");
            ConversationLines.npc_interaction_marriage_2.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(IntentionHero, Target, false));//"{=Dramalord054}We could use the opportunity being in a settlement, {TITLE}. Let's head over to the church and seal this bond for life!");
            ConversationLines.player_reaction_marry_yes.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord067}I agree, {TITLE}. Let's seal this bond of ours.");
            ConversationLines.player_reaction_no.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Target, IntentionHero, false));// "{=Dramalord068}I am sorry {TITLE}, but I have no interest in that right now.");
        }
    }
}
