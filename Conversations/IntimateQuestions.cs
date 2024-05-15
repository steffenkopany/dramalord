using Dramalord.Behaviors;
using Dramalord.Data;
using Dramalord.UI;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.CampaignSystem.Actions.KillCharacterAction;

namespace Dramalord.Conversations
{
    internal static class IntimateQuestions 
    {
        internal static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("player_asks_intimate_questions", "hero_main_options", "npc_intimate_questions_reply", "{=Dramalord001}May I ask you something personal?", ConditionPlayerCanAskIntimateQuestions, null);

            starter.AddDialogLine("npc_accepts_intimate_questions", "npc_intimate_questions_reply", "player_intimate_questions_list", "{=Dramalord002}Sure. What do you want to know?", ConditionNpcAcceptsIntimateQuestions, null);
            starter.AddDialogLine("npc_declines_intimate_questions", "npc_intimate_questions_reply", "hero_main_options", "{=Dramalord003}I do not feel comfortable enough with you for this kind of conversation.", ConditionNpcDeclinesIntimateQuestions, null);

            starter.AddPlayerLine("player_asks_about_looks", "player_intimate_questions_list", "npc_replies_about_looks", "{=Dramalord007}What looks do you prefer?", null, null);
            starter.AddPlayerLine("player_asks_about_player_looks", "player_intimate_questions_list", "npc_replies_about_player_looks", "{=Dramalord322}How do you like the way I look?", null, null);
            starter.AddPlayerLine("player_asks_about_feelings", "player_intimate_questions_list", "npc_replies_about_feelings", "{=Dramalord030}How do you feel about me?", null, null);
            starter.AddPlayerLine("player_asks_about_fertility", "player_intimate_questions_list", "npc_replies_about_fertility", "{=Dramalord320}Can you currently get pregnant?", ConditionPlayerAskFertility, null);
            starter.AddPlayerLine("player_asks_about_horny", "player_intimate_questions_list", "npc_replies_about_horny", "{=Dramalord321}Do you feel this... special itch?", ConditionPlayerAskHorny, null);
            starter.AddPlayerLine("player_stops_asking_questions", "player_intimate_questions_list", "npc_replies_to_ending_conversation", "{=Dramalord004}Let's talk about something else...", null, null);

            starter.AddDialogLine("npc_replies_about_looks", "npc_replies_about_looks", "player_intimate_questions_list", "{=Dramalord008}I think women are {RATING_WOMEN} and men are {RATING_MEN}. I like {RATING_WEIGHT} people of {RATING_BUILD} build. Best around the age of {RATING_AGE}.", ConditionNpcRepliesAboutLooks, null);
            starter.AddDialogLine("npc_replies_about_player_looks", "npc_replies_about_player_looks", "player_intimate_questions_list", "{=Dramalord323}Hmm... I think you look {RATING_TOTAL}!", ConditionNpcRepliesAboutPlayerLooks, null);
            starter.AddDialogLine("npc_replies_about_feelings", "npc_replies_about_feelings", "player_intimate_questions_list", "{=Dramalord031}I {EMOTION_SCORE} you!", ConditionNpcRepliesAboutFeelings, null);
            starter.AddDialogLine("npc_replies_about_fertility_positive", "npc_replies_about_fertility", "player_intimate_questions_list", "{=Dramalord324}I am currently fertile, my period starts in {PERIOD_DAYS} days.", ConditionNpcRepliesAboutFertilityPositive, null);
            starter.AddDialogLine("npc_replies_about_fertility_period", "npc_replies_about_fertility", "player_intimate_questions_list", "{=Dramalord325}I have my period, so I am not fertile for the next {PERIOD_DAYS} days.", ConditionNpcRepliesAboutFertilityPeriod, null);
            starter.AddDialogLine("npc_replies_about_fertility_age", "npc_replies_about_fertility", "player_intimate_questions_list", "{=Dramalord326}I'm afraid I'm too old to be fertile.", ConditionNpcRepliesAboutFertilityAge, null);
            starter.AddDialogLine("npc_replies_about_fertility_male", "npc_replies_about_fertility", "player_intimate_questions_list", "{=Dramalord327}What a silly question... I am a man.", ConditionNpcRepliesAboutFertilityMale, null);
            starter.AddDialogLine("npc_replies_about_fertility_pregnant_player", "npc_replies_about_fertility", "player_intimate_questions_list", "{=Dramalord333}I'm already pregnant, silly!", ConditionNpcRepliesAboutFertilityPregnantPlayer, null);
            starter.AddDialogLine("npc_replies_about_fertility_pregnant_other", "npc_replies_about_fertility", "npc_accusation_reaction_list", "{=Dramalord333}I'm already pregnant, silly!", ConditionNpcRepliesAboutFertilityPregnantOther, null);
            starter.AddDialogLine("npc_replies_about_horny_positive", "npc_replies_about_horny", "player_intimate_questions_list", "{=Dramalord328}Hmm... yes... I'd love it if someone would scratch my itch...", ConditionNpcRepliesAboutHornyPositive, null);
            starter.AddDialogLine("npc_replies_about_horny_negative", "npc_replies_about_horny", "player_intimate_questions_list", "{=Dramalord329}No, not really. I'm currently not in the mood.", ConditionNpcRepliesAboutHornyNegative, null);
            starter.AddDialogLine("npc_replies_to_ending_conversation", "npc_replies_to_ending_conversation", "hero_main_options", "{=Dramalord005}As you wish", null, null);

        }

        // CONDITIONS
        internal static bool ConditionPlayerCanAskIntimateQuestions()
        {
            return Info.ValidateHeroInfo(Hero.OneToOneConversationHero) && Info.ValidateHeroMemory(Hero.MainHero, Hero.OneToOneConversationHero);
        }

        internal static bool ConditionNpcAcceptsIntimateQuestions()
        {
            return Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) >= DramalordMCM.Get.MinEmotionForConversation;
        }

        internal static bool ConditionNpcDeclinesIntimateQuestions()
        {
            return Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero) < DramalordMCM.Get.MinEmotionForConversation;
        }

        internal static bool ConditionPlayerAskFertility()
        {
            return Hero.MainHero.Spouse == Hero.OneToOneConversationHero || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero);
        }

        internal static bool ConditionPlayerAskHorny()
        {
            return Hero.MainHero.Spouse == Hero.OneToOneConversationHero || Info.IsCoupleWithHero(Hero.MainHero, Hero.OneToOneConversationHero);
        }

        internal static bool ConditionNpcRepliesAboutLooks()
        {
            HeroInfoData? data = Info.GetHeroInfoDataCopy(Hero.OneToOneConversationHero);
            if (data != null)
            {
                int attractionWomen = (Hero.OneToOneConversationHero.IsFemale) ? data.AttractionWomen - DramalordMCM.Get.OtherSexAttractionModifier : data.AttractionWomen + DramalordMCM.Get.OtherSexAttractionModifier;
                int attractionMen = (!Hero.OneToOneConversationHero.IsFemale) ? data.AttractionMen - DramalordMCM.Get.OtherSexAttractionModifier : data.AttractionMen + DramalordMCM.Get.OtherSexAttractionModifier;
                MBTextManager.SetTextVariable("RATING_WOMEN", new TextObject(attractionWomen > 75 ? "{=Dramalord010}sexy" : attractionWomen > 50 ? "{=Dramalord011}alright" : attractionWomen < 25 ? "{=Dramalord012}disgusting" : "{=Dramalord013}not my thing").ToString());
                MBTextManager.SetTextVariable("RATING_MEN", attractionMen > 75 ? "{=Dramalord010}sexy" : attractionMen > 50 ? "{=Dramalord011}alright" : attractionMen < 25 ? "{=Dramalord012}disgusting" : "{=Dramalord013}not my thing");
                MBTextManager.SetTextVariable("RATING_WEIGHT", data.AttractionWeight > 0.6 ? "{=Dramalord014}chubby" : data.AttractionWeight > 0.3 ? "{=Dramalord015}average" : "{=Dramalord016}slim");
                MBTextManager.SetTextVariable("RATING_BUILD", data.AttractionBuild > 0.6 ? "{=Dramalord017}muscular" : data.AttractionBuild > 0.3 ? "{=Dramalord018}normal" : "{=Dramalord019}low");
                MBTextManager.SetTextVariable("RATING_AGE", MBMath.ClampInt((int)Hero.OneToOneConversationHero.Age + data.AttractionAgeDiff, 18, 130));
            }

            return true;
        }

        internal static bool ConditionNpcRepliesAboutPlayerLooks()
        {
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

            return true;
        }

        internal static bool ConditionNpcRepliesAboutFeelings()
        {

            int emotion = (int)Info.GetEmotionToHero(Hero.MainHero, Hero.OneToOneConversationHero);

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

        internal static bool ConditionNpcRepliesAboutFertilityPositive()
        {
            if(Hero.OneToOneConversationHero.IsFemale && Info.IsHeroFertile(Hero.OneToOneConversationHero))
            {
                HeroInfoData? data = Info.GetHeroInfoDataCopy(Hero.OneToOneConversationHero);
                if(data != null)
                {
                    int today = (int)CampaignTime.Now.GetDayOfSeason;

                    if (today < data.PeriodDayOfSeason)
                    {
                        MBTextManager.SetTextVariable("PERIOD_DAYS", data.PeriodDayOfSeason - today);
                    }
                    else
                    {
                        MBTextManager.SetTextVariable("PERIOD_DAYS", (CampaignTime.DaysInSeason + data.PeriodDayOfSeason) - today);
                    }

                    return true;
                }
            }
            return false;
        }

        internal static bool ConditionNpcRepliesAboutFertilityPeriod()
        {
            if (Hero.OneToOneConversationHero.IsFemale && !Hero.OneToOneConversationHero.IsPregnant && Hero.OneToOneConversationHero.Age <= DramalordMCM.Get.MaxFertilityAge && !Info.IsHeroFertile(Hero.OneToOneConversationHero))
            {
                HeroInfoData? data = Info.GetHeroInfoDataCopy(Hero.OneToOneConversationHero);
                if (data != null)
                {
                    int today = (int)CampaignTime.Now.GetDayOfSeason;
                    int nextToday = today + CampaignTime.DaysInSeason;
                    int startPeriod = (int)data.PeriodDayOfSeason;
                    int endPeriod = startPeriod + DramalordMCM.Get.PeriodDuration;

                    bool inPeriod = (today >= startPeriod && today <= endPeriod) || (nextToday >= startPeriod && nextToday <= endPeriod);


                    if (today >= startPeriod)
                    {
                        MBTextManager.SetTextVariable("PERIOD_DAYS", endPeriod - today);
                    }
                    else
                    {
                        MBTextManager.SetTextVariable("PERIOD_DAYS", endPeriod - nextToday);
                    }

                    return true;
                }
            }
            return false;
        }

        internal static bool ConditionNpcRepliesAboutFertilityAge()
        {
            return Hero.OneToOneConversationHero.IsFemale && Hero.OneToOneConversationHero.Age > DramalordMCM.Get.MaxFertilityAge;  
        }

        internal static bool ConditionNpcRepliesAboutFertilityPregnantPlayer()
        {
            if(Hero.OneToOneConversationHero.IsPregnant)
            {
                HeroOffspringData? offspring = Info.GetHeroOffspring(Hero.OneToOneConversationHero);
                if(offspring != null)
                {
                    return offspring.Father == Hero.MainHero;
                }
            }
            return false;
        }

        internal static bool ConditionNpcRepliesAboutFertilityPregnantOther()
        {
            if (Hero.OneToOneConversationHero.IsPregnant)
            {
                HeroOffspringData? offspring = Info.GetHeroOffspring(Hero.OneToOneConversationHero);
                if (offspring != null && offspring.Father != Hero.MainHero)
                {
                    PlayerConfrontation.CheatingHero = Hero.OneToOneConversationHero;
                    PlayerConfrontation.WitnessOf = Actions.WitnessType.Pregnancy;
                    return true;
                }
            }
            return false;
        }

        internal static bool ConditionNpcRepliesAboutFertilityMale()
        {
            return !Hero.OneToOneConversationHero.IsFemale;
        }

        internal static bool ConditionNpcRepliesAboutHornyPositive()
        {
            return Info.GetHeroHorny(Hero.OneToOneConversationHero) >= DramalordMCM.Get.MinHornyForIntercourse;
        }

        internal static bool ConditionNpcRepliesAboutHornyNegative()
        {
            return Info.GetHeroHorny(Hero.OneToOneConversationHero) < DramalordMCM.Get.MinHornyForIntercourse;
        }
    }
}
