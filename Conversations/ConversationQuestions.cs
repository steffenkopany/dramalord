using Dramalord.Data;
using Dramalord.Data.Intentions;
using Dramalord.Extensions;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class ConversationQuestions
    {
        public enum Context
        {
            Chat,
            Flirt,
            Date
        }


        private static Context _currentContext = Context.Chat;

        private static TextObject _question = new();

        private static TextObject[] _answers = new TextObject[3] { new(), new(), new() };

        private static TextObject[] _reactions = new TextObject[3] { new(), new(), new() };

        private static int[] _weights = new int[3] { 0, 0, 0 };

        //private static int _answer = -1;

        private static int _count = 0;

        private static int _result = 0;

        public static void SetupQuestions(Context context, int count)
        {
            _currentContext = context;
            _count = count;
            ConversationLines.npc_challenge_summarize_end.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
        }

        internal static void AddDialogs(CampaignGameStarter starter)
        {
            DialogFlow flow = DialogFlow.CreateDialogFlow("start_challenge")
                .NpcLine("{CHALLENGE_QUESTION}[ib:aggressive][if:convo_undecided_open]")
                    .Condition(() => GenerateQuestion())
                    .Consequence(()=> ConversationLines.player_interaction_abort.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, false)))
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
                    .PlayerOption("{player_interaction_abort}")
                        .Consequence(() => AnswerAbort())
                        .GotoDialogState("evaluate_challenge")
                .EndPlayerOptions();

            DialogFlow eval = DialogFlow.CreateDialogFlow("evaluate_challenge")
                .NpcLine("{REACTION_LINE}{REACTION_EXPRESSION}")
                    .Consequence(() => ConversationLines.player_interaction_abort.SetTextVariable("TITLE", ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false)))
                .BeginNpcOptions()
                    .NpcOption("{player_interaction_abort}{SUMMARIZE_EXPRESSION}", () => _count > 0)
                        .GotoDialogState("start_challenge")
                    .NpcOption("{npc_challenge_summarize_end}{SUMMARIZE_EXPRESSION}", () => _count <= 0)
                        .Consequence(() => FinishChallenge())
                        .CloseDialog()
                .EndNpcOptions();

            Campaign.Current.ConversationManager.AddDialogFlow(flow);
            Campaign.Current.ConversationManager.AddDialogFlow(eval);
        }

        internal static bool GenerateQuestion()
        {
            if (_currentContext == Context.Date)
            {
                GenerateRandomDateChallenge();
            }
            else if (_currentContext == Context.Flirt)
            {
                GenerateRandomFlirtChallenge();
            }
            else if (_currentContext == Context.Chat)
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

            if(_currentContext == Context.Chat || _currentContext == Context.Date)
            {
                ChangeSympathy(_weights[0]);
            }

            if(_currentContext == Context.Flirt || _currentContext == Context.Date)
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

            if (_currentContext == Context.Chat || _currentContext == Context.Date)
            {
                ChangeSympathy(_weights[1]);
            }

            if (_currentContext == Context.Flirt || _currentContext == Context.Date)
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

            if (_currentContext == Context.Chat || _currentContext == Context.Date)
            {
                ChangeSympathy(_weights[2]);
            }

            if (_currentContext == Context.Flirt || _currentContext == Context.Date)
            {
                ChangeAttraction(_weights[2]);
            }
        }

        internal static void AnswerAbort()
        {
            MBTextManager.SetTextVariable("REACTION_LINE", ConversationLines.npc_as_you_wish_reply);
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
            if(_currentContext == Context.Chat)
            {
                ConversationTools.ConversationIntention = new TalkIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, _result);
            }
            else if(_currentContext == Context.Flirt)
            {
                ConversationTools.ConversationIntention = new FlirtIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, _result);
            }
            else if (_currentContext == Context.Date)
            {
                ConversationTools.ConversationIntention = new DateIntention(Hero.OneToOneConversationHero, Hero.MainHero, CampaignTime.Now, _result);
            }

            _result = 0;
            _count = 0;
            ConversationTools.EndConversation();
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

            Hero? poi = Hero.AllAliveHeroes.Where(h => h.IsLord && h != Hero.MainHero && h != challenger && (challenger.GetBaseHeroRelation(h) > 40 || challenger.GetBaseHeroRelation(h) < -30)).GetRandomElementInefficiently();
            if (poi == null) poi = Hero.AllAliveHeroes.Where(h => h.IsLord && h != Hero.MainHero && h != challenger && h.IsDramalordLegit()).GetRandomElementInefficiently();

            _question = new TextObject("{=Dramalord352}Tell me {TITLE}, as you travel a lot and meet people, what's your opinion about {POI.NAME}?");
            _question.SetTextVariable("TITLE", challenger.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _question);

            if (poi?.HasMet ?? false)
            {
                _answers[0] = new TextObject("{=Dramalord353}I have met {POI.NAME} and I consider them being a friend of mine.");
                _answers[1] = new TextObject("{=Dramalord354}Ugh, don't remind me. {POI.NAME} is annoying and I hope I won't meet them again.");
                _answers[2] = new TextObject("{=Dramalord355}{POI.NAME}, hmm. I actually don't have any particular opinion about them.");
            }
            else
            {
                _answers[0] = new TextObject("{=Dramalord356}Unfortunately I never hat the chance to meet {POI.NAME}, but they sound like a nice person?");
                _answers[1] = new TextObject("{=Dramalord357}Good thing I never met {POI.NAME}. I can image them being a horrible being.");
                _answers[2] = new TextObject("{=Dramalord358}I do not have any opinion about {POI.NAME}, as I never exchanged a word with them.");
            }

            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _answers[0]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _answers[1]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _answers[2]);

            if (challenger.GetBaseHeroRelation(poi) > DramalordMCM.Instance.MinTrustFriends)
            {
                _weights[0] = 1;
                _weights[1] = -1;
                _weights[2] = 0;

                _reactions[0] = new TextObject("{=Dramalord359}That's good to hear! {POI.NAME} is a good friend of mine as well!");
                _reactions[1] = new TextObject("{=Dramalord360}I'm disappointed to hear that. {POI.NAME} is actually a good friend of mine.");
                _reactions[2] = new TextObject("{=Dramalord361}Well you probably don't know {POI.NAME} well. They're my friend.");
            }
            else if (challenger.GetBaseHeroRelation(poi) < DramalordMCM.Instance.MaxTrustEnemies)
            {
                _weights[0] = -1;
                _weights[1] = 1;
                _weights[2] = 0;

                _reactions[0] = new TextObject("{=Dramalord362}Ugh, really? You like {POI.NAME}? That's very disappointing.");
                _reactions[1] = new TextObject("{=Dramalord363}I totally agree! {POI.NAME} is like dirt under my fingernails.");
                _reactions[2] = new TextObject("{=Dramalord364}Well you probably don't know {POI.NAME} well. I really don't like them.");
            }
            else
            {
                _weights[0] = 0;
                _weights[1] = 0;
                _weights[2] = 1;

                _reactions[0] = new TextObject("{=Dramalord365}You are fast in building up an opinion about {POI.NAME} I have to say.");
                _reactions[1] = new TextObject("{=Dramalord365}You are fast in building up an opinion about {POI.NAME} I have to say.");
                _reactions[2] = new TextObject("{=Dramalord366}I don't have an opinion about {POI.NAME} as well. I don't know them well enough.");
            }

            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _reactions[0]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _reactions[1]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _reactions[2]);
        }

        internal static void StartTraitChallenge()
        {
            Hero challenger = Hero.OneToOneConversationHero;

            int random = (MBRandom.RandomInt(0, 100) % 5) + 1; //mercy valor honor generosity calculating

            TextObject good = new TextObject("{=Dramalord367}That's an excellent point of view! Agreed.");
            TextObject bad = new TextObject("{=Dramalord368}Well, I actually have to diagree with you.");
            TextObject neutral = new TextObject("{=Dramalord369}You are quite opinionated, aren't you?");

            if (random == 1)
            {
                _question = new TextObject("{=Dramalord370}Last war our faction raided an enemy village because they ran out of food. It's sad, but I think it had to be done for the greater good. (Honor)");
                _answers[0] = new TextObject("{=Dramalord371}I disagree. Civilians shouldn't suffer because of politics.");
                _answers[1] = new TextObject("{=Dramalord372}Right. Those peasants should have given their goods volunarily.");
                _answers[2] = new TextObject("{=Dramalord373}I don't know. I guess thats just the way things are.");

                _weights[0] = (challenger.GetHeroTraits().Honor != 0) ? challenger.GetHeroTraits().Honor : -1;
                _weights[1] = (challenger.GetHeroTraits().Honor != 0) ? challenger.GetHeroTraits().Honor * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Honor == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Honor > 0) ? good : (challenger.GetHeroTraits().Honor < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Honor < 0) ? good : (challenger.GetHeroTraits().Honor > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Honor == 0) ? good : bad;
            }
            else if (random == 2)
            {
                _question = new TextObject("{=Dramalord374}Have you ever encountered a stronger foe but went to battle nevertheless? (Valor)");
                _answers[0] = new TextObject("{=Dramalord375}Yes, and we've won anyway. This world belongs to the brave!");
                _answers[1] = new TextObject("{=Dramalord376}No. Why would I risk my life when I know we can't win.");
                _answers[2] = new TextObject("{=Dramalord377}I can't say. I think that depends on the situation.");

                _weights[0] = (challenger.GetHeroTraits().Valor != 0) ? challenger.GetHeroTraits().Valor : -1;
                _weights[1] = (challenger.GetHeroTraits().Valor != 0) ? challenger.GetHeroTraits().Valor * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Valor == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Valor > 0) ? good : (challenger.GetHeroTraits().Valor < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Valor < 0) ? good : (challenger.GetHeroTraits().Valor > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Valor == 0) ? good : bad;
            }
            else if (random == 3)
            {
                _question = new TextObject("{=Dramalord378}I heard that some lords let captured troops simply leave the battlefield. Isn't that a risk? (Mercy)");
                _answers[0] = new TextObject("{=Dramalord379}They let them go home to their families. I would do the same.");
                _answers[1] = new TextObject("{=Dramalord380}Heads off I'd say! Makes it hard for them to go to battle again.");
                _answers[2] = new TextObject("{=Dramalord381}They are there to kill or be killed, so who cares about them.");

                _weights[0] = (challenger.GetHeroTraits().Mercy != 0) ? challenger.GetHeroTraits().Mercy : -1;
                _weights[1] = (challenger.GetHeroTraits().Mercy != 0) ? challenger.GetHeroTraits().Mercy * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Mercy == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Mercy > 0) ? good : (challenger.GetHeroTraits().Mercy < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Mercy < 0) ? good : (challenger.GetHeroTraits().Mercy > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Mercy == 0) ? good : bad;
            }
            else if (random == 4)
            {
                _question = new TextObject("{=Dramalord382}Recently the champion of a tournament gave all of his winnings to the poor. (Generosity)");
                _answers[0] = new TextObject("{=Dramalord383}That is a generous move indeed. This kind of people are rare, unfortunately.");
                _answers[1] = new TextObject("{=Dramalord384}Bah! What a waste of money. Why did they participate in the first place?");
                _answers[2] = new TextObject("{=Dramalord385}Well, I guess it's their money, thus it's their choice.");

                _weights[0] = (challenger.GetHeroTraits().Generosity != 0) ? challenger.GetHeroTraits().Generosity : -1;
                _weights[1] = (challenger.GetHeroTraits().Generosity != 0) ? challenger.GetHeroTraits().Generosity * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Generosity == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Generosity > 0) ? good : (challenger.GetHeroTraits().Generosity < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Generosity < 0) ? good : (challenger.GetHeroTraits().Generosity > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Generosity == 0) ? good : bad;
            }
            else if (random == 5)
            {
                _question = new TextObject("{=Dramalord386}One of the southern clans almost left their kingdom, after their leader had an argument with their king. (Calculating)");
                _answers[0] = new TextObject("{=Dramalord387}That's stupid. They would have lose their protection because of their pride.");
                _answers[1] = new TextObject("{=Dramalord388}I can understand how they feel. I also loose me temper every now and then.");
                _answers[2] = new TextObject("{=Dramalord389}I cant't judge that move, as long as I don't know what the argument was about.");

                _weights[0] = (challenger.GetHeroTraits().Calculating != 0) ? challenger.GetHeroTraits().Calculating : -1;
                _weights[1] = (challenger.GetHeroTraits().Calculating != 0) ? challenger.GetHeroTraits().Calculating * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Calculating == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Calculating > 0) ? good : (challenger.GetHeroTraits().Calculating < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Calculating < 0) ? good : (challenger.GetHeroTraits().Calculating > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Calculating == 0) ? good : bad;
            }
        }

        internal static void StartAttractionChallenge()
        {
            Hero challenger = Hero.OneToOneConversationHero;

            int random = (MBRandom.RandomInt(0, 100) % 4) + 1; //sex weight build age 

            TextObject good = new TextObject("{=Dramalord390}Ohh! You're making me blush! I'm happy to hear that.");
            TextObject bad = new TextObject("{=Dramalord391}So does that mean you don't consider me attractive?");
            TextObject neutral = new TextObject("{=Dramalord392}I'm pretty sure you have some kind of preference.");

            if (random == 1)
            {
                _question = new TextObject("{=Dramalord393}I've heard that some lords and ladies have a different taste in physical gender. How about you? (Orientation)");
                _answers[0] = new TextObject("{=Dramalord321}I feel drawn to the other sex and I don't have much interest in persons of my own.");
                _answers[1] = new TextObject("{=Dramalord322}I don't have much interest in the other sex, I rather prefer persons of my own.");
                _answers[2] = new TextObject("{=Dramalord394}I actually don't have any specific preference.");

                _weights[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? 1 : -1;
                _weights[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? 1 : -1;
                _weights[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? 1 : 0;

                _reactions[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? good : bad;
                _reactions[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? good : bad;
                _reactions[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? good : neutral;
            }
            else if (random == 2)
            {
                _question = new TextObject("{=Dramalord395}The owner of the local inn has gained some weight. Now the tavern is packed with gawking customers every night. (Weight)");
                _answers[0] = new TextObject("{=Dramalord327}I like people with more weight. There's more to grab for me.");
                _answers[1] = new TextObject("{=Dramalord325}I think slim people are more grazile then others.");
                _answers[2] = new TextObject("{=Dramalord326}I don't like thin or fat. The middle is just right.");

                _weights[0] = (challenger.Weight >= 0.7) ? 1 : -1;
                _weights[1] = (challenger.Weight <= 0.3) ? 1 : -1;
                _weights[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? 1 : -1;

                _reactions[0] = (challenger.Weight >= 0.7) ? good : bad;
                _reactions[1] = (challenger.Weight <= 0.3) ? good : bad;
                _reactions[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? good : bad;
            }
            else if (random == 3)
            {
                _question = new TextObject("{=Dramalord396}In the highlands lords and ladies are often bulky and powerful. Many people admire them for being strong. (Build)");
                _answers[0] = new TextObject("{=Dramalord330}I love powerful people. The bulkier the better.");
                _answers[1] = new TextObject("{=Dramalord328}Muscles are overrated. I like it skinny and want to see bones.");
                _answers[2] = new TextObject("{=Dramalord329}Medium muscles are just right for me. I don't need something special.");

                _weights[0] = (challenger.Build >= 0.7) ? 1 : -1;
                _weights[1] = (challenger.Build <= 0.3) ? 1 : -1;
                _weights[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? 1 : -1;

                _reactions[0] = (challenger.Build >= 0.7) ? good : bad;
                _reactions[1] = (challenger.Build <= 0.3) ? good : bad;
                _reactions[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? good : bad;
            }
            else if (random == 4)
            {
                _question = new TextObject("{=Dramalord397}There are people who prefer older people and others prefer younger people. What is your preference? (Age)");
                _answers[0] = new TextObject("{=Dramalord398}I prefer people older then me. Nothing is better then ripe fruit.");
                _answers[1] = new TextObject("{=Dramalord399}I enjoy the energy and stamina of younger people very much.");
                _answers[2] = new TextObject("{=Dramalord400}I think people around my own age are more attractive.");

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

            int random = (MBRandom.RandomInt(0, 100) % 5) + 1; //openness, conscientiousness, extroversion, agreeableness, neuroticism

            TextObject good = new TextObject("{=YcdQ1MWq}Well.. It seems we have a fair amount in common.");
            TextObject bad = new TextObject("{=dY2PzpIV}I'm not sure how much we have in common..");
            TextObject neutral = new TextObject("{=E9s2bjqw}I can only hope that some day you could change your mind.");

            if (random == 1)
            {
                _question = new TextObject("{=Dramalord401}Do you think trying out new things is sometimes exciting? (Openness)");
                _answers[0] = new TextObject("{=Dramalord402}I like trying new things. It's getting boring without change.");
                _answers[1] = new TextObject("{=Dramalord403}Nonsense. Things are good the way they are and don't need to change.");
                _answers[2] = new TextObject("{=Dramalord404}Maybe, I don't really care. Things are changing anyway all the time.");

                _weights[0] = (challenger.GetPersonality().Openness > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Openness < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Openness <= 16 && challenger.GetPersonality().Openness >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Openness > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Openness < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Openness <= 16 && challenger.GetPersonality().Openness >= -16) ? good : neutral;
            }
            else if (random == 2)
            {
                _question = new TextObject("{=Dramalord405}So, are you rather a person who plans out things, or are you more the sponateous type? (Conscientiousness)");
                _answers[0] = new TextObject("{=Dramalord406}Everything needs order and structure. Planning is everything.");
                _answers[1] = new TextObject("{=Dramalord407}Nah, not really. I usually stumble into my next adventure.");
                _answers[2] = new TextObject("{=Dramalord408}Eh, sometimes I plan, sometimes I don't.");

                _weights[0] = (challenger.GetPersonality().Conscientiousness > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Conscientiousness < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Conscientiousness <= 16 && challenger.GetPersonality().Conscientiousness >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Conscientiousness > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Conscientiousness < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Conscientiousness <= 16 && challenger.GetPersonality().Conscientiousness >= -16) ? good : neutral;
            }
            else if (random == 3)
            {
                _question = new TextObject("{=Dramalord409}When there's a tournament in town, are you excited to meet new people? (Extroversion)");
                _answers[0] = new TextObject("{=Dramalord410}Oh absolutely! I barely can't stop chatting when I'm around people.");
                _answers[1] = new TextObject("{=Dramalord411}Ugh. Nah, I'm rather to myself. Most people are bothering me.");
                _answers[2] = new TextObject("{=Dramalord412}Well, that depends. I'd meet a few probably, but not many.");

                _weights[0] = (challenger.GetPersonality().Extroversion > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Extroversion < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Extroversion <= 16 && challenger.GetPersonality().Extroversion >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Extroversion > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Extroversion < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Extroversion <= 16 && challenger.GetPersonality().Extroversion >= -16) ? good : neutral;
            }
            else if (random == 4)
            {
                _question = new TextObject("{=Dramalord413}If one of your friends tell you a sad story, how do you react? (Agreeableness)");
                _answers[0] = new TextObject("{=Dramalord414}I try to comfort them, or help if I can of course.");
                _answers[1] = new TextObject("{=Dramalord415}Uhm, my friends know not to bother me with their personal stuff.");
                _answers[2] = new TextObject("{=Dramalord416}That depends on how I feel myself, I guess.");

                _weights[0] = (challenger.GetPersonality().Agreeableness > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Agreeableness < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Agreeableness <= 16 && challenger.GetPersonality().Agreeableness >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Agreeableness > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Agreeableness < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Agreeableness <= 16 && challenger.GetPersonality().Agreeableness >= -16) ? good : neutral;
            }
            else if (random == 5)
            {
                _question = new TextObject("{=Dramalord417}Managing a party is a lot of stress sometimes. How do you deal with it? (Neuroticism)");
                _answers[0] = new TextObject("{=Dramalord418}Oh yes. For recovering I usually lock myself away for a few days.");
                _answers[1] = new TextObject("{=Dramalord419}Well, it's not stressful at all. I am actually rarely stressed.");
                _answers[2] = new TextObject("{=Dramalord420}It's ok I think. Sometimes I need a day off or two.");

                _weights[0] = (challenger.GetPersonality().Neuroticism > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Neuroticism < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Neuroticism <= 16 && challenger.GetPersonality().Neuroticism >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Neuroticism > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Neuroticism < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Neuroticism <= 16 && challenger.GetPersonality().Neuroticism >= -16) ? good : neutral;
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
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }
            else if (newAttraction != oldAttraction)
            {
                TextObject banner = new TextObject("{=Dramalord476}You are now less attractive to {HERO.LINK}. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationTools.FormatNumber(newAttraction - oldAttraction));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
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

            personality.Openness += (playerPersonality.Openness > personality.Openness) ? result : (playerPersonality.Openness < personality.Openness) ? -result : 0;
            personality.Extroversion += (playerPersonality.Extroversion > personality.Extroversion) ? result : (playerPersonality.Extroversion < personality.Extroversion) ? -result : 0;
            personality.Neuroticism += (playerPersonality.Neuroticism > personality.Neuroticism) ? result : (playerPersonality.Neuroticism < personality.Neuroticism) ? -result : 0;
            personality.Conscientiousness += (playerPersonality.Conscientiousness > personality.Conscientiousness) ? result : (playerPersonality.Conscientiousness < personality.Conscientiousness) ? -result : 0;
            personality.Agreeableness += (playerPersonality.Agreeableness > personality.Agreeableness) ? result : (playerPersonality.Agreeableness < personality.Agreeableness) ? -result : 0;

            int newSympathy = npc.GetSympathyTo(Hero.MainHero);

            if (_result > 0 && newSympathy != oldSympathy)
            {
                TextObject banner = new TextObject("{=Dramalord477}{HERO.LINK} has more sympathy for you. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationTools.FormatNumber(newSympathy - oldSympathy));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }
            else if (newSympathy != oldSympathy)
            {
                TextObject banner = new TextObject("{=Dramalord478}{HERO.LINK} has less sympathy for you. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationTools.FormatNumber(newSympathy - oldSympathy));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }
        }
    }
}
