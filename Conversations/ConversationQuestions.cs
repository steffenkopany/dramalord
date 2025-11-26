using Dramalord.Data;
using Dramalord.Data.Events;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class ConversationQuestions
    {
        public enum QuestionType
        {
            Talk,
            Flirt,
            Date
        }

        private static QuestionType _questionType;

        private static string expr_undecided = "[if:convo_undecided_open]";

        private static TextObject _question = TextObject.GetEmpty();

        private static TextObject[] _answers = new TextObject[3] { TextObject.GetEmpty(), TextObject.GetEmpty(), TextObject.GetEmpty() };

        private static TextObject[] _reactions = new TextObject[3] { TextObject.GetEmpty(), TextObject.GetEmpty(), TextObject.GetEmpty() };

        private static int[] _weights = new int[3] { 0, 0, 0 };

        private static bool _exitConversation;

        private static int _count = 0;

        private static int _result = 0;

        public static void SetupQuestions(QuestionType qType, int count, bool exitConversation)
        {
            _questionType = qType;
            _count = count;
            _result = 0;
            _exitConversation = exitConversation;
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start_challenge")
                .NpcLine("{CHALLENGE_QUESTION}[ib:aggressive]")
                    .Condition(() => GenerateQuestion())
                    .BeginPlayerOptions()
                        .PlayerOption("{CHALLENGE_REPLY_1}")
                            .Consequence(() => Answer1())
                            .GotoDialogState("evaluate_challenge")
                        .PlayerOption("{CHALLENGE_REPLY_2}")
                            .Consequence(() => Answer2())
                            .GotoDialogState("evaluate_challenge")
                        .PlayerOption("{CHALLENGE_REPLY_3}")
                            .Consequence(() => Answer3())
                            .GotoDialogState("evaluate_challenge")
                        .PlayerOption(DramalordTexts.PLAYER_INTERACTION_END)
                            .Condition(() => ConversationTools.SetConversationHero(Hero.OneToOneConversationHero))
                            .Consequence(() => AnswerAbort())
                            .GotoDialogState("evaluate_challenge")
                    .EndPlayerOptions();

            DialogFlow eval = DialogFlow.CreateDialogFlow("evaluate_challenge")
                .NpcLine("{REACTION_LINE}{REACTION_EXPRESSION}")
                    .BeginNpcOptions()
                        .NpcOption(DramalordTexts.PLAYER_INTERACTION_END + "{SUMMARIZE_EXPRESSION}", () => _count > 0 && ConversationTools.SetConversationHero(Hero.MainHero))
                            .GotoDialogState("start_challenge")
                        .NpcOption(DramalordTexts.QUESTION_END + "{SUMMARIZE_EXPRESSION}", () => _count <= 0 && _exitConversation && ConversationTools.SetConversationHero(Hero.MainHero))
                            .Consequence(() => FinishChallenge())
                            .CloseDialog()
                        .NpcOption(DramalordTexts.QUESTION_END + "{SUMMARIZE_EXPRESSION}", () => _count <= 0 && !_exitConversation && ConversationTools.SetConversationHero(Hero.MainHero))
                            .Consequence(() => FinishChallenge())
                            .GotoDialogState("player_interaction_selection")
                    .EndNpcOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
            Campaign.Current.ConversationManager.AddDialogFlow(eval);
        }

        internal static bool GenerateQuestion()
        {
            if (_questionType == QuestionType.Date)
            {
                GenerateRandomDateChallenge();
            }
            else if (_questionType == QuestionType.Flirt)
            {
                GenerateRandomFlirtChallenge();
            }
            else if (_questionType == QuestionType.Talk)
            {
                GenerateRandomChatChallenge();
            }

            MBTextManager.SetTextVariable("CHALLENGE_QUESTION", _question);
            MBTextManager.SetTextVariable("CHALLENGE_REPLY_1", _answers[0]);
            MBTextManager.SetTextVariable("CHALLENGE_REPLY_2", _answers[1]);
            MBTextManager.SetTextVariable("CHALLENGE_REPLY_3", _answers[2]);

            _count--;
            return true;
        }

        internal static void Answer1()
        {
            _result += _weights[0];
            MBTextManager.SetTextVariable("REACTION_LINE", _reactions[0]);
            if (_weights[0] > 0)
            {
                MBTextManager.SetTextVariable("REACTION_EXPRESSION", "[ib:confident][if:convo_delighted]");
            }
            else
            {
                MBTextManager.SetTextVariable("REACTION_EXPRESSION", "[ib:nervous][if:convo_grave]");
            }

            if (_result > 0)
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:demure][if:convo_bemused]");
            }
            else
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:closed][if:convo_bored]");
            }

            if(_questionType == QuestionType.Talk || _questionType == QuestionType.Date)
            {
                ChangeSympathy(_weights[0]);
            }

            if(_questionType == QuestionType.Flirt || _questionType == QuestionType.Date)
            {
                ChangeAttraction(_weights[0]);
            }
        }

        internal static void Answer2()
        {
            _result += _weights[1];
            MBTextManager.SetTextVariable("REACTION_LINE", _reactions[1]);
            if (_weights[1] > 0)
            {
                MBTextManager.SetTextVariable("REACTION_EXPRESSION", "[ib:confident][if:convo_delighted]");
            }
            else
            {
                MBTextManager.SetTextVariable("REACTION_EXPRESSION", "[ib:nervous][if:convo_grave]");
            }

            if (_result > 0)
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:demure][if:convo_bemused]");
            }
            else
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:closed][if:convo_bored]");
            }

            if (_questionType == QuestionType.Talk || _questionType == QuestionType.Date)
            {
                ChangeSympathy(_weights[1]);
            }

            if (_questionType == QuestionType.Flirt || _questionType == QuestionType.Date)
            {
                ChangeAttraction(_weights[1]);
            }
        }

        internal static void Answer3()
        {
            _result += _weights[2];
            MBTextManager.SetTextVariable("REACTION_LINE", _reactions[2]);
            if (_weights[2] > 0)
            {
                MBTextManager.SetTextVariable("REACTION_EXPRESSION", "[ib:confident][if:convo_delighted]");
            }
            else
            {
                MBTextManager.SetTextVariable("REACTION_EXPRESSION", "[ib:nervous][if:convo_grave]");
            }

            if (_result > 0)
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:demure][if:convo_bemused]");
            }
            else
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:closed][if:convo_bored]");
            }

            if (_questionType == QuestionType.Talk || _questionType == QuestionType.Date)
            {
                ChangeSympathy(_weights[2]);
            }

            if (_questionType == QuestionType.Flirt || _questionType == QuestionType.Date)
            {
                ChangeAttraction(_weights[2]);
            }
        }

        internal static void AnswerAbort()
        {
            MBTextManager.SetTextVariable("REACTION_LINE", DramalordTexts.NPC_INTERACTION_ASYOUWISH);
            ConversationTools.SetConversationHero(Hero.MainHero);
            _count = 0;
            MBTextManager.SetTextVariable("REACTION_EXPRESSION", "[ib:closed][if:convo_bored]");

            if (_result > 0)
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:demure][if:convo_bemused]");
            }
            else
            {
                MBTextManager.SetTextVariable("SUMMARIZE_EXPRESSION", "[ib:closed][if:convo_bored]");
            }
        }

        internal static void FinishChallenge()
        {

            if (_questionType == QuestionType.Talk)
            {
                DramalordEvents.Instance.StartIntention(new TalkEvent(Hero.MainHero, Hero.OneToOneConversationHero, _result));
            }
            else if (_questionType == QuestionType.Flirt)
            {
                DramalordEvents.Instance.StartIntention(new FlirtEvent(Hero.MainHero, Hero.OneToOneConversationHero, _result));
            }
            if (_questionType == QuestionType.Date)
            {
                DramalordEvents.Instance.StartIntention(new DateEvent(Hero.MainHero, Hero.OneToOneConversationHero, _result));
            }

            _result = 0;
            _count = 0;
        }

        ///////////////////////////////////////
        internal static void GenerateRandomChatChallenge()
        {
            int rand = (MBRandom.RandomInt(0, 100) % 2) + 1;
            if (rand == 1) StartHeroOpinionChallenge();
            if (rand == 2) StartTraitChallenge();
        }

        internal static void GenerateRandomFlirtChallenge()
        {
            int rand = (MBRandom.RandomInt(0, 100) % 2) + 1;
            if (rand == 1) StartAttractionChallenge();
            if (rand == 2) StartPersonalityChallenge();
        }

        internal static void GenerateRandomDateChallenge()
        {
            int rand = (MBRandom.RandomInt(0, 100) % 4) + 1;
            if (rand == 1) StartHeroOpinionChallenge();
            if (rand == 2) StartTraitChallenge();
            if (rand == 3) StartAttractionChallenge();
            if (rand == 4) StartPersonalityChallenge();
        }

        internal static void StartHeroOpinionChallenge()
        {
            Hero challenger = Hero.OneToOneConversationHero;

            Hero? poi = Hero.AllAliveHeroes.Where(h => h.IsLord && h != Hero.MainHero && h != challenger && (challenger.GetBaseHeroRelation(h) >= DramalordMCM.Instance.MinTrustFriends || challenger.GetBaseHeroRelation(h) <= DramalordMCM.Instance.MaxTrustEnemies)).GetRandomElementInefficiently();
            if (poi == null) poi = Hero.AllAliveHeroes.Where(h => h.IsLord && h != Hero.MainHero && h != challenger && h.IsDramalordLegit()).GetRandomElementInefficiently();

            string expression = (challenger.GetBaseHeroRelation(poi) >= DramalordMCM.Instance.MinTrustFriends) ? "[if:convo_focused_happy]" : (challenger.GetBaseHeroRelation(poi) <= DramalordMCM.Instance.MaxTrustEnemies) ? "[if:convo_furious]" : "[if:convo_undecided_open]";
            _question = new TextObject(DramalordTexts.QUESTION_POI + expression);
            ConversationTools.SetCharacterObjects(_question, Hero.MainHero, poi);

            if (poi?.HasMet ?? false)
            {
                _answers[0] = new TextObject(DramalordTexts.QUESTION_POI_MET_LIKE);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_POI_MET_DISLIKE);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_POI_MET_NOCARE);
            }
            else
            {
                _answers[0] = new TextObject(DramalordTexts.QUESTION_POI_NOTMET_LIKE);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_POI_NOTMET_DISLIKE);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_POI_NOTMET_NOCARE);
            }

            ConversationTools.SetCharacterObjects(_answers[0], poi);
            ConversationTools.SetCharacterObjects(_answers[1], poi);
            ConversationTools.SetCharacterObjects(_answers[2], poi);

            if (challenger.GetBaseHeroRelation(poi) > DramalordMCM.Instance.MinTrustFriends)
            {
                _weights[0] = 1;
                _weights[1] = -1;
                _weights[2] = 0;

                _reactions[0] = new TextObject(DramalordTexts.QUESTION_POI_LIKE_SUCCESS);
                _reactions[1] = new TextObject(DramalordTexts.QUESTION_POI_LIKE_FAIL);
                _reactions[2] = new TextObject(DramalordTexts.QUESTION_POI_LIKE_NOCARE);
            }
            else if (challenger.GetBaseHeroRelation(poi) < DramalordMCM.Instance.MaxTrustEnemies)
            {
                _weights[0] = -1;
                _weights[1] = 1;
                _weights[2] = 0;

                _reactions[0] = new TextObject(DramalordTexts.QUESTION_POI_DISLIKE_FAIL);
                _reactions[1] = new TextObject(DramalordTexts.QUESTION_POI_DISLIKE_SUCCESS);
                _reactions[2] = new TextObject(DramalordTexts.QUESTION_POI_DISLIKE_NOCARE);
            }
            else
            {
                _weights[0] = 0;
                _weights[1] = 0;
                _weights[2] = 1;

                _reactions[0] = new TextObject(DramalordTexts.QUESTION_POI_NOCARE_FAIL);
                _reactions[1] = new TextObject(DramalordTexts.QUESTION_POI_NOCARE_FAIL);
                _reactions[2] = new TextObject(DramalordTexts.QUESTION_POI_NOCARE_SUCCESS);
            }

            ConversationTools.SetCharacterObjects(_reactions[0], poi);
            ConversationTools.SetCharacterObjects(_reactions[1], poi);
            ConversationTools.SetCharacterObjects(_reactions[2], poi);
        }

        internal static void StartTraitChallenge()
        {
            Hero challenger = Hero.OneToOneConversationHero;

            int random = (MBRandom.RandomInt(0, 100) % 5) + 1; //mercy valor honor generosity calculating

            TextObject good = new TextObject(DramalordTexts.QUESTION_TRAIT_AGREE);
            TextObject bad = new TextObject(DramalordTexts.QUESTION_TRAIT_DISAGREE);
            TextObject neutral = new TextObject(DramalordTexts.QUESTION_TRAIT_NOCARE);

            if (random == 1)
            {
                _question = new TextObject(DramalordTexts.QUESTION_TRAIT_HONOR + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_TRAIT_HONOR_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_TRAIT_HONOR_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_TRAIT_HONOR_NOCARE);

                _weights[0] = (challenger.GetTraitLevel(DefaultTraits.Honor) != 0) ? challenger.GetTraitLevel(DefaultTraits.Honor) : -1;
                _weights[1] = (challenger.GetTraitLevel(DefaultTraits.Honor) != 0) ? challenger.GetTraitLevel(DefaultTraits.Honor) * -1 : -1;
                _weights[2] = (challenger.GetTraitLevel(DefaultTraits.Honor) == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetTraitLevel(DefaultTraits.Honor) > 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Honor) < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetTraitLevel(DefaultTraits.Honor) < 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Honor) > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetTraitLevel(DefaultTraits.Honor) == 0) ? good : bad;
            }
            else if (random == 2)
            {
                _question = new TextObject(DramalordTexts.QUESTION_TRAIT_VALOR + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_TRAIT_VALOR_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_TRAIT_VALOR_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_TRAIT_VALOR_NOCARE);

                _weights[0] = (challenger.GetTraitLevel(DefaultTraits.Valor) != 0) ? challenger.GetTraitLevel(DefaultTraits.Valor) : -1;
                _weights[1] = (challenger.GetTraitLevel(DefaultTraits.Valor) != 0) ? challenger.GetTraitLevel(DefaultTraits.Valor) * -1 : -1;
                _weights[2] = (challenger.GetTraitLevel(DefaultTraits.Valor) == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetTraitLevel(DefaultTraits.Valor) > 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Valor) < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetTraitLevel(DefaultTraits.Valor) < 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Valor) > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetTraitLevel(DefaultTraits.Valor) == 0) ? good : bad;
            }
            else if (random == 3)
            {
                _question = new TextObject(DramalordTexts.QUESTION_TRAIT_MERCY + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_TRAIT_MERCY_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_TRAIT_MERCY_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_TRAIT_MERCY_NOCARE);

                _weights[0] = (challenger.GetTraitLevel(DefaultTraits.Mercy) != 0) ? challenger.GetTraitLevel(DefaultTraits.Mercy) : -1;
                _weights[1] = (challenger.GetTraitLevel(DefaultTraits.Mercy) != 0) ? challenger.GetTraitLevel(DefaultTraits.Mercy) * -1 : -1;
                _weights[2] = (challenger.GetTraitLevel(DefaultTraits.Mercy) == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetTraitLevel(DefaultTraits.Mercy) > 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Mercy) < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetTraitLevel(DefaultTraits.Mercy) < 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Mercy) > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetTraitLevel(DefaultTraits.Mercy) == 0) ? good : bad;
            }
            else if (random == 4)
            {
                _question = new TextObject(DramalordTexts.QUESTION_TRAIT_GENEROSITY + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_TRAIT_GENEROSITY_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_TRAIT_GENEROSITY_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_TRAIT_GENEROSITY_NOCARE);

                _weights[0] = (challenger.GetTraitLevel(DefaultTraits.Generosity) != 0) ? challenger.GetTraitLevel(DefaultTraits.Generosity) : -1;
                _weights[1] = (challenger.GetTraitLevel(DefaultTraits.Generosity) != 0) ? challenger.GetTraitLevel(DefaultTraits.Generosity) * -1 : -1;
                _weights[2] = (challenger.GetTraitLevel(DefaultTraits.Generosity) == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetTraitLevel(DefaultTraits.Generosity) > 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Generosity) < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetTraitLevel(DefaultTraits.Generosity) < 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Generosity) > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetTraitLevel(DefaultTraits.Generosity) == 0) ? good : bad;
            }
            else if (random == 5)
            {
                _question = new TextObject(DramalordTexts.QUESTION_TRAIT_CALCULATING + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_TRAIT_CALCULATING_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_TRAIT_CALCULATING_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_TRAIT_CALCULATING_NOCARE);

                _weights[0] = (challenger.GetTraitLevel(DefaultTraits.Calculating) != 0) ? challenger.GetTraitLevel(DefaultTraits.Calculating) : -1;
                _weights[1] = (challenger.GetTraitLevel(DefaultTraits.Calculating) != 0) ? challenger.GetTraitLevel(DefaultTraits.Calculating) * -1 : -1;
                _weights[2] = (challenger.GetTraitLevel(DefaultTraits.Calculating) == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetTraitLevel(DefaultTraits.Calculating) > 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Calculating) < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetTraitLevel(DefaultTraits.Calculating) < 0) ? good : (challenger.GetTraitLevel(DefaultTraits.Calculating) > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetTraitLevel(DefaultTraits.Calculating) == 0) ? good : bad;
            }
        }

        internal static void StartAttractionChallenge()
        {
            Hero challenger = Hero.OneToOneConversationHero;

            int random = (MBRandom.RandomInt(0, 100) % 4) + 1; //sex weight build age 

            TextObject good = new TextObject(DramalordTexts.QUESTION_PHYSICAL_GOOD);
            TextObject bad = new TextObject(DramalordTexts.QUESTION_PHYSICAL_BAD);
            TextObject neutral = new TextObject(DramalordTexts.QUESTION_PHYSICAL_NOCARE);

            if (random == 1)
            {
                _question = new TextObject(DramalordTexts.QUESTION_PHYSICAL_ORIENTATION + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_HETERO);
                _answers[1] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_HOMO);
                _answers[2] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_BI);

                _weights[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? 1 : -1;
                _weights[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? 1 : -1;
                _weights[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? 1 : 0;

                _reactions[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? good : bad;
                _reactions[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? good : bad;
                _reactions[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? good : neutral;
            }
            else if (random == 2)
            {
                _question = new TextObject(DramalordTexts.QUESTION_PHYSICAL_WEIGHT + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_WEIGHT_HIGH);
                _answers[1] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_WEIGHT_LOW);
                _answers[2] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_WEIGHT_MID);

                _weights[0] = (challenger.Weight >= 0.7) ? 1 : -1;
                _weights[1] = (challenger.Weight <= 0.3) ? 1 : -1;
                _weights[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? 1 : -1;

                _reactions[0] = (challenger.Weight >= 0.7) ? good : bad;
                _reactions[1] = (challenger.Weight <= 0.3) ? good : bad;
                _reactions[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? good : bad;
            }
            else if (random == 3)
            {
                _question = new TextObject(DramalordTexts.QUESTION_PHYSICAL_BUILD + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_BUILD_HIGH);
                _answers[1] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_BUILD_LOW);
                _answers[2] = new TextObject(DramalordTexts.NPC_INTERACTION_INFO_BUILD_MID);

                _weights[0] = (challenger.Build >= 0.7) ? 1 : -1;
                _weights[1] = (challenger.Build <= 0.3) ? 1 : -1;
                _weights[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? 1 : -1;

                _reactions[0] = (challenger.Build >= 0.7) ? good : bad;
                _reactions[1] = (challenger.Build <= 0.3) ? good : bad;
                _reactions[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? good : bad;
            }
            else if (random == 4)
            {
                _question = new TextObject(DramalordTexts.QUESTION_PHYSICAL_AGE + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_PHYSICAL_AGE_OLDER);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_PHYSICAL_AGE_YOUNGER);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_PHYSICAL_AGE_SAME);

                _weights[0] = (challenger.Age - Hero.MainHero.Age > 5) ? 1 : -1;
                _weights[1] = (challenger.Age - Hero.MainHero.Age < -5) ? 1 : -1;
                _weights[2] = (challenger.Age - Hero.MainHero.Age <= 5 && challenger.Age - Hero.MainHero.Age >= -5) ? 1 : -1;

                _reactions[0] = (challenger.Age - Hero.MainHero.Age > 5) ? good : bad;
                _reactions[1] = (challenger.Age - Hero.MainHero.Age < -5) ? good : bad;
                _reactions[2] = (challenger.Age - Hero.MainHero.Age <= 5 && challenger.Age - Hero.MainHero.Age >= -5) ? good : bad;
            }
        }

        internal static void StartPersonalityChallenge()
        {
            Hero challenger = Hero.OneToOneConversationHero;

            int random = (MBRandom.RandomInt(0, 100) % 3) + 1; //openness, conscientiousness, extroversion, agreeableness, neuroticism

            TextObject good = new TextObject("{=YcdQ1MWq}Well.. It seems we have a fair amount in common.");
            TextObject bad = new TextObject("{=dY2PzpIV}I'm not sure how much we have in common..");
            TextObject neutral = new TextObject("{=E9s2bjqw}I can only hope that some day you could change your mind.");

            if (random == 1)
            {
                _question = new TextObject(DramalordTexts.QUESTION_PERSONALITY_JEALOUSY + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_JEALOUSY_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_JEALOUSY_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_JEALOUSY_NOCARE);

                _weights[0] = (challenger.GetPersonality().Jealousy > 66) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Jealousy < 33) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Jealousy <= 66 && challenger.GetPersonality().Jealousy >= 33) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Jealousy > 66) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Jealousy < 33) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Jealousy <= 66 && challenger.GetPersonality().Jealousy >= 33) ? good : neutral;
            }
            else if (random == 2)
            {
                _question = new TextObject(DramalordTexts.QUESTION_PERSONALITY_SOCIABILITY + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_SOCIABILITY_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_SOCIABILITY_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_SOCIABILITY_NOCARE);

                _weights[0] = (challenger.GetPersonality().Sociability > 66) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Sociability < 33) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Sociability <= 66 && challenger.GetPersonality().Sociability >= 33) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Sociability > 66) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Sociability < 33) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Sociability <= 66 && challenger.GetPersonality().Sociability >= 33) ? good : neutral;
            }
            else if (random == 3)
            {
                _question = new TextObject(DramalordTexts.QUESTION_PERSONALITY_EMPATHY + expr_undecided);
                _answers[0] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_EMPATHY_GOOD);
                _answers[1] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_EMPATHY_BAD);
                _answers[2] = new TextObject(DramalordTexts.QUESTION_PERSONALITY_EMPATHY_NOCARE);

                _weights[0] = (challenger.GetPersonality().Empathy > 66) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Empathy < 33) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Empathy <= 66 && challenger.GetPersonality().Empathy >= 33) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Empathy > 66) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Empathy < 33) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Empathy <= 66 && challenger.GetPersonality().Empathy >= 33) ? good : neutral;
            }
        }

        private static void ChangeAttraction(int value)
        {
            if (value == 0)
            {
                return;
            }
            int result = value > 0 ? 1 : -1;
            Hero npc = Hero.OneToOneConversationHero;
            HeroDesires desires = npc.GetDesires();
            int oldAttraction = npc.GetAttractionTo(Hero.MainHero);

            if (Hero.MainHero.IsFemale) { desires.AttractionWomen += result; desires.AttractionMen -= result; }
            else if (!Hero.MainHero.IsFemale) { desires.AttractionWomen -= result; desires.AttractionMen += result; }

            int build = (int)(Hero.MainHero.Build * 100);
            if (build < desires.AttractionBuild) { desires.AttractionBuild -= result; }
            else if (build > desires.AttractionBuild) { desires.AttractionBuild += result; }

            int weight = (int)(Hero.MainHero.Weight * 100);
            if (weight < desires.AttractionWeight) { desires.AttractionWeight -= result; }
            else if (weight > desires.AttractionWeight) { desires.AttractionWeight += result; }

            int age = (int)Hero.MainHero.Age;
            int wantedAge = (int)npc.Age + desires.AttractionAgeDiff;
            if (age < wantedAge) { desires.AttractionAgeDiff -= result; }
            else if (age > wantedAge) { desires.AttractionAgeDiff += result; }

            int newAttraction = npc.GetAttractionTo(Hero.MainHero);

            if (result > 0 && newAttraction != oldAttraction)
            {
                TextObject banner = new TextObject("{=Dramalord475}You are now more attractive to {HERO.LINK}. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationTools.FormatNumber(newAttraction - oldAttraction));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, soundEventPath: "event:/ui/notification/relation");
            }
            else if (newAttraction != oldAttraction)
            {
                TextObject banner = new TextObject("{=Dramalord476}You are now less attractive to {HERO.LINK}. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationTools.FormatNumber(newAttraction - oldAttraction));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, soundEventPath: "event:/ui/notification/relation");
            }
        }

        private static void ChangeSympathy(int value)
        {
            if (value == 0)
            {
                return;
            }
            int result = value > 0 ? 1 : -1;
            Hero npc = Hero.OneToOneConversationHero;
            HeroPersonality personality = npc.GetPersonality();
            HeroPersonality playerPersonality = Hero.MainHero.GetPersonality();

            int oldSympathy = npc.GetSympathyTo(Hero.MainHero);

            personality.Jealousy += (playerPersonality.Jealousy > personality.Jealousy) ? result : (playerPersonality.Jealousy < personality.Jealousy) ? -result : 0;
            personality.Sociability += (playerPersonality.Sociability > personality.Sociability) ? result : (playerPersonality.Sociability < personality.Sociability) ? -result : 0;
            personality.Empathy += (playerPersonality.Empathy > personality.Empathy) ? result : (playerPersonality.Empathy < personality.Empathy) ? -result : 0;

            int newSympathy = npc.GetSympathyTo(Hero.MainHero);

            if (_result > 0 && newSympathy != oldSympathy)
            {
                TextObject banner = new TextObject("{=Dramalord477}{HERO.LINK} has more sympathy for you. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationTools.FormatNumber(newSympathy - oldSympathy));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, soundEventPath: "event:/ui/notification/relation");
            }
            else if (newSympathy != oldSympathy)
            {
                TextObject banner = new TextObject("{=Dramalord478}{HERO.LINK} has less sympathy for you. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationTools.FormatNumber(newSympathy - oldSympathy));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, soundEventPath: "event:/ui/notification/relation");
            }
        }
    }
}
