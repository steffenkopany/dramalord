using Dramalord.Actions;
using Dramalord.Data;
using SandBox.GauntletUI.Encyclopedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia;

namespace Dramalord.UI
{
    internal static class Conversations
    {
        internal static readonly string CloseConversation = "close_window";
        internal static readonly string PlayerMainOptions = "hero_main_options"; 
        internal static readonly string NPCStartReplyGroup = "dl_npc_intro_reply";
        internal static readonly string PlayerConversationMain = "dl_player_conversation_main";
        internal static readonly string NPCConversationExit = "dl_player_conversation_exit";
        internal static readonly string NPCAttractionRating = "dl_npc_attraction_player";
        internal static readonly string NPCEmotionRating = "dl_npc_emotion_player";
        internal static readonly string NPCQuestionReply = "dl_npc_question_reply";
        internal static readonly string PlayerConversationQuestion = "dl_player_conversation_question";
        internal static readonly string NPCActionReply = "dl_npc_action_reply";
        internal static readonly string PlayerConversationAction = "dl_player_conversation_action";
        internal static readonly string NPCFlirtReply = "dl_npc_interaction_reply";
        internal static readonly string NPCDateReply = "dl_npc_date_reply";
        internal static readonly string NPCJoinReply = "dl_npc_join_reply";
        internal static readonly string NPCDivorceHusbandReply = "dl_npc_divorce_reply";
        internal static readonly string NPCMarryReply = "dl_npc_marry_reply";
        internal static readonly string NPCPresentReply = "dl_npc_present_reply";
        internal static readonly string NPCBreakupReply = "dl_npc_breakup_reply";
        internal static readonly string PlayerFlirtReply = "dl_player_flirt_reply";
        internal static readonly string PlayerDateReply = "dl_player_date_reply";
        internal static readonly string PlayerJoinReply = "dl_player_join_reply";
        internal static readonly string PlayerDivorceReply = "dl_player_divorce_reply";
        internal static readonly string NPCDivorceReply = "dl_npc_divorce_reply";
        internal static readonly string PlayerMarryReply = "dl_player_marry_reply";
        internal static readonly string NPCActionResult = "dl_npc_action_result";
        internal static readonly string NPCViolatedReply = "dl_npc_violated_result";

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            // main conversation
            starter.AddPlayerLine("Dramalord001", PlayerMainOptions,        NPCStartReplyGroup,         "{=Dramalord001}May I ask you something personal?", Conditions.PlayerCanAskForTalk, null);
            starter.AddDialogLine("Dramalord002", NPCStartReplyGroup,       PlayerConversationMain,     "{=Dramalord002}Sure. What do you want to know?", Conditions.NpcWantsToTalk, null);
            starter.AddDialogLine("Dramalord003", NPCStartReplyGroup,       PlayerMainOptions,          "{=Dramalord003}I do not feel comfortable enough with you for this kind of conversation.", Conditions.NpcDeclinesToTalk, null);
            starter.AddPlayerLine("Dramalord004", PlayerConversationMain,   NPCConversationExit,        "{=Dramalord004}Let's talk about something else...", null, null, 1);
            starter.AddDialogLine("Dramalord005", NPCConversationExit,      PlayerMainOptions,          "{=Dramalord005}As you wish", null, null);

            starter.AddPlayerLine("Dramalord006", PlayerConversationMain,   NPCAttractionRating,        "{=Dramalord006}Do you like what you see?", Conditions.PlayerCanAskForTalk, null);
            starter.AddDialogLine("Dramalord008", NPCAttractionRating,      PlayerConversationMain,     "{=Dramalord008}I think women are {RATING_WOMEN} and men are {RATING_MEN}. I like {RATING_WEIGHT} people of {RATING_BUILD} build. Best around the age of {RATING_AGE}. Hmm... I think you look {RATING_TOTAL}!", SetConversationTextVariables, Notification.PrintTopFlirtTargets);

            starter.AddPlayerLine("Dramalord030", PlayerConversationMain,   NPCEmotionRating,           "{=Dramalord030}How do you feel about me?", Conditions.PlayerCanAskForTalk, Notification.PrintNpcStats);
            starter.AddDialogLine("Dramalord031", NPCEmotionRating,         PlayerConversationMain,     "{=Dramalord031}I {EMOTION_SCORE} you!", SetConversationTextVariables, null);


            starter.AddPlayerLine("Dramalord042", PlayerConversationMain,   NPCQuestionReply, "{=Dramalord042}Would you like to...", Conditions.PlayerCanAskForAction, null);
            starter.AddDialogLine("Dramalord043", NPCQuestionReply,           PlayerConversationQuestion, "{=Dramalord043}Do what?", null, null);

            starter.AddPlayerLine("Dramalord070", PlayerConversationQuestion, NPCFlirtReply, "{=Dramalord070}...go for a walk with me?", Conditions.PlayerCanAskForFlirt, null);
            starter.AddPlayerLine("Dramalord044", PlayerConversationQuestion, NPCDateReply, "{=Dramalord044}...retreat to somewhere more silent?", Conditions.PlayerCanAskForDate, null);
            /*starter.AddPlayerLine("Dramalord047", PlayerConversationQuestion, NPCJoinReply, "{=Dramalord047}...come with me for a while?", Conditions.PlayerCanAskForDate, null);*/
            starter.AddPlayerLine("Dramalord050", PlayerConversationQuestion, NPCDivorceHusbandReply, "{=Dramalord050}...get divorced so we can focus on us?", Conditions.PlayerCanAskForDivorceHusband, null);
            starter.AddPlayerLine("Dramalord051", PlayerConversationQuestion, NPCMarryReply, "{=Dramalord051}...marry me?", Conditions.PlayerCanAskForMarriage, null);
            starter.AddPlayerLine("Dramalord072", PlayerConversationQuestion, NPCStartReplyGroup, "{=Dramalord072}Nevermind.", null, null);

            starter.AddDialogLine("Dramalord045", NPCFlirtReply, CloseConversation, "{=Dramalord045}Oh... well, I think I would like that.", Conditions.NpcWantsToFlirt, Consequences.NpcAcceptedFlirt);
            starter.AddDialogLine("Dramalord046", NPCFlirtReply, PlayerConversationMain, "{=Dramalord046}No I would not like to do that.", Conditions.NpcDeclinesToFlirt, Consequences.NpcDeclinedFlirt);

            starter.AddDialogLine("Dramalord945", NPCDateReply, CloseConversation, "{=Dramalord045}Oh... well, I think I would like that.", Conditions.NpcWantsToDate, Consequences.NpcAcceptedDate);
            starter.AddDialogLine("Dramalord646", NPCDateReply, Persuasions.PlayerDateArgument, "{=Dramalord088}Why should I spend time with you?", Conditions.NpcConsidersToDate, Consequences.NpcWasConsideringDate);
            starter.AddDialogLine("Dramalord946", NPCDateReply, PlayerConversationMain, "{=Dramalord046}No I would not like to do that.", Conditions.NpcDeclinesToDate, Consequences.NpcDeclinedDate);
            /*
            starter.AddDialogLine("Dramalord845", NPCJoinReply, CloseConversation, "{=Dramalord045}Oh... well, I think I would like that.", Conditions.NpcWantsToJoin, Consequences.NpcAcceptedJoin);
            starter.AddDialogLine("Dramalord053", NPCJoinReply, Persuasions.PlayerJoinArgument, "{=Dramalord053}Why should I ride with you?", Conditions.NpcConsidersToJoin, Consequences.NpcWasConsideringJoin);
            starter.AddDialogLine("Dramalord846", NPCJoinReply, PlayerConversationMain, "{=Dramalord054}I think I will leave my horse in the stable.", Conditions.NpcDeclinesToJoin, Consequences.NpcDeclinedJoin);
            */
            starter.AddDialogLine("Dramalord745", NPCDivorceHusbandReply, CloseConversation, "{=Dramalord045}Oh... well, I think I would like that.", Conditions.NpcWantsToDivorceHusband, Consequences.NpcAcceptedDivorce);
            starter.AddDialogLine("Dramalord748", NPCDivorceHusbandReply, Persuasions.PlayerDivorceArgument, "{=Dramalord061}Why should I give up my marriage?", Conditions.NpcConsidersToDivorceHusband, Consequences.NpcWasConsideringDivorce);
            starter.AddDialogLine("Dramalord746", NPCDivorceHusbandReply, PlayerConversationMain, "{=Dramalord068}I will not give up my marriage for you!", Conditions.NpcDeclinesToDivorceHusband, Consequences.NpcDeclinedDivorce);

            starter.AddDialogLine("Dramalord545", NPCMarryReply, CloseConversation, "{=Dramalord045}Oh... well, I think I would like that.", Conditions.NpcWantsToMarry, Consequences.NpcAcceptedMarriage);
            starter.AddDialogLine("Dramalord546", NPCMarryReply, Persuasions.PlayerMarryArgument, "{=Dramalord082}Why should I marry you", Conditions.NpcConsidersToMarry, Consequences.NpcWasConsideringMarriage);
            starter.AddDialogLine("Dramalord545", NPCMarryReply, PlayerConversationMain, "{=Dramalord046}No I would not like to do that.", Conditions.NpcDeclinesToMarry, Consequences.NpcDeclinedMarriage);


            starter.AddPlayerLine("Dramalord098", PlayerConversationMain, NPCActionReply, "{=Dramalord098}I would like to...", null, null);
            starter.AddDialogLine("Dramalord102", NPCActionReply, PlayerConversationAction, "{=Dramalord102}What do you want?", null, null);

            starter.AddPlayerLine("Dramalord103", PlayerConversationAction, NPCPresentReply, "{=Dramalord103}...give you something.", Conditions.PlayerCanGivePresent, null);
            starter.AddPlayerLine("Dramalord104", PlayerConversationAction, NPCBreakupReply, "{=Dramalord104}...end this love affair.", Conditions.PlayerCanAskForBreakup, null);
            starter.AddPlayerLine("Dramalord105", PlayerConversationAction, NPCDivorceReply, "{=Dramalord105}...end this marriage.", Conditions.PlayerCanAskForDivorce, null);
            starter.AddPlayerLine("Dramalord972", PlayerConversationAction, NPCStartReplyGroup, "{=Dramalord072}Nevermind.", null, null);

            starter.AddDialogLine("Dramalord106", NPCPresentReply, PlayerConversationMain, "{=Dramalord106}Oh! I will put it to good use!", null, Consequences.NpcGotPresentFromPlayer);

            starter.AddDialogLine("Dramalord107", NPCBreakupReply, PlayerConversationMain, "{=Dramalord107}Ugh. Finally this is over!", Conditions.NpcDoesntMindBreakup, Consequences.NpcDidntCareAboutBreakup);
            starter.AddDialogLine("Dramalord108", NPCBreakupReply, CloseConversation, "{=Dramalord108}Oh. This is a suprise. I.. I have to be alone now...", Conditions.NpcSurprisedByBreakup, Consequences.NpcWasSurprisedByBreakup);
            starter.AddDialogLine("Dramalord109", NPCBreakupReply, CloseConversation, "{=Dramalord109}What? You bastard! I never want to see you again!", Conditions.NpcHeartbrokenByBreakup, Consequences.NpcWasHeartBrokenByBreakup);
            starter.AddDialogLine("Dramalord110", NPCBreakupReply, CloseConversation, "{=Dramalord110}Oh god... my darkes nightmare has come true... I can't live without you...", Conditions.NpcSuicidalByBreakup, Consequences.NpcGotSuicidalByBreakup);

            starter.AddDialogLine("Dramalord907", NPCDivorceReply, PlayerConversationMain, "{=Dramalord107}Ugh. Finally this is over!", Conditions.NpcDoesntMindBreakup, Consequences.NpcDidntCareAboutDivorce);
            starter.AddDialogLine("Dramalord908", NPCDivorceReply, CloseConversation, "{=Dramalord108}Oh. This is a suprise. I.. I have to be alone now...", Conditions.NpcSurprisedByBreakup, Consequences.NpcWasSurprisedByDivorce);
            starter.AddDialogLine("Dramalord909", NPCDivorceReply, CloseConversation, "{=Dramalord109}What? You bastard! I never want to see you again!", Conditions.NpcHeartbrokenByBreakup, Consequences.NpcWasHeartBrokenByDivorce);
            starter.AddDialogLine("Dramalord910", NPCDivorceReply, CloseConversation, "{=Dramalord110}Oh god... my darkes nightmare has come true... I can't live without you...", Conditions.NpcSuicidalByBreakup, Consequences.NpcGotSuicidalByDivorce);

            starter.AddPlayerLine("Dramalord111", PlayerMainOptions, NPCViolatedReply, "{=Dramalord111}You and I... we're going to have some fun now...", Conditions.PlayerCanViolateNpc, null);
            starter.AddDialogLine("Dramalord112", NPCViolatedReply, CloseConversation, "{=Dramalord112}Oh god! Please, no! Stay away from me!", null, Consequences.PlayerViolatedNpc);
        }


        internal static bool SetConversationTextVariables()
        {
            if (Hero.OneToOneConversationHero != null)
            {
                HeroInfoData? data = Info.GetHeroInfoDataCopy(Hero.OneToOneConversationHero);
                if(data != null)
                {
                    int attractionWomen = (Hero.OneToOneConversationHero.IsFemale) ? data.AttractionWomen - DramalordMCM.Get.OtherSexAttractionModifier : data.AttractionWomen + DramalordMCM.Get.OtherSexAttractionModifier;
                    int attractionMen = (!Hero.OneToOneConversationHero.IsFemale) ? data.AttractionMen - DramalordMCM.Get.OtherSexAttractionModifier : data.AttractionMen + DramalordMCM.Get.OtherSexAttractionModifier;
                    MBTextManager.SetTextVariable("RATING_WOMEN", new TextObject(attractionWomen > 75 ? "{=Dramalord010}sexy" : attractionWomen > 50 ? "{=Dramalord011}alright" : attractionWomen < 25 ? "{=Dramalord012}disgusting" : "{=Dramalord013}not my thing").ToString());
                    MBTextManager.SetTextVariable("RATING_MEN", attractionMen > 75 ? "{=Dramalord010}sexy" : attractionMen > 50 ? "{=Dramalord011}alright" : attractionMen < 25 ? "{=Dramalord012}disgusting" : "{=Dramalord013}not my thing");
                    MBTextManager.SetTextVariable("RATING_WEIGHT", data.AttractionWeight > 0.6 ? "{=Dramalord014}chubby" : data.AttractionWeight > 0.3 ? "{=Dramalord015}average" : "{=Dramalord016}slim");
                    MBTextManager.SetTextVariable("RATING_BUILD", data.AttractionBuild > 0.6 ? "{=Dramalord017}muscular" : data.AttractionBuild > 0.3 ? "{=Dramalord018}normal" : "{=Dramalord019}low");
                    MBTextManager.SetTextVariable("RATING_AGE", Info.Clamp<int>((int)Hero.OneToOneConversationHero.Age + data.AttractionAgeDiff, 18, 130));
                }

                int rating = Info.GetAttractionToHero(Hero.OneToOneConversationHero, Hero.MainHero);
                if (rating < 10)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord020}absolutely disgusting!"));
                }
                else if (rating < 20)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord021}disgusting."));
                }
                else if (rating < 30)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord022}very ugly."));
                }
                else if (rating < 40)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord023}ugly."));
                }
                else if (rating < 50)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord024}not pretty."));
                }
                else if (rating < 60)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord025}average."));
                }
                else if (rating < 70)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord026}nice."));
                }
                else if (rating < 80)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord027}pretty!"));
                }
                else if (rating < 90)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord028}stunning!"));
                }
                else
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord029}so fucking hot!"));
                }


                int emotion = (int)Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.MainHero);

                if (emotion < -80)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord032}absolutely despise"));
                }
                else if (emotion < -60)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord033}hate"));
                }
                else if (emotion < -40)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord034}can't stand"));
                }
                else if (emotion < -20)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord034}dislike"));
                }
                else if (emotion < 0)
                {
                    MBTextManager.SetTextVariable("RATING_TOTAL", new TextObject("{=Dramalord024}am not a fan of"));
                }
                else if (emotion < 20)
                {
                    MBTextManager.SetTextVariable("EMOTION_SCORE", new TextObject("{=Dramalord037}respect"));
                }
                else if (emotion < 40)
                {
                    MBTextManager.SetTextVariable("EMOTION_SCORE", new TextObject("{=Dramalord038}like"));
                }
                else if (emotion < 60)
                {
                    MBTextManager.SetTextVariable("EMOTION_SCORE", new TextObject("{=Dramalord039}think I'm falling for"));
                }
                else if (emotion < 80)
                {
                    MBTextManager.SetTextVariable("EMOTION_SCORE", new TextObject("{=Dramalord040}can't stop thinking about"));
                }
                else
                {
                    MBTextManager.SetTextVariable("EMOTION_SCORE", new TextObject("{=Dramalord041}am totally in love with"));
                }

                return true;
            }
            return false;
        }
    }
}
