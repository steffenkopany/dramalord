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

            _question = new TextObject("{=Dramalord352}Tell me, {TITLE}, as you travel wide and meet many, do you have an opinion of {POI.NAME}?");
            _question.SetTextVariable("TITLE", challenger.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationTools.GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, false));
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _question);

            if (poi?.HasMet ?? false)
            {
                _answers[0] = new TextObject("{=Dramalord353}Ah, {POI.NAME}! Yes, we have met. I am actually quite fond of them.");
                _answers[1] = new TextObject("{=Dramalord354}Unfortunately, yes. I have met {POI.NAME} and do not regard them kindly.");
                _answers[2] = new TextObject("{=Dramalord355}{POI.NAME}, you say? I harbor no feelings for them one way or the other.");
            }
            else
            {
                _answers[0] = new TextObject("{=Dramalord356}Oh, {POI.NAME}? We have yet to meet, but I hope to, and have heard good things.");
                _answers[1] = new TextObject("{=Dramalord357}From what I have heard, I have been spared a misfortune in not having met {POI.NAME}.");
                _answers[2] = new TextObject("{=Dramalord358}I hold no opinion of {POI.NAME}, as we have yet to exchange words.");
            }

            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _answers[0]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _answers[1]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _answers[2]);

            if (challenger.GetBaseHeroRelation(poi) > DramalordMCM.Instance.MinTrustFriends)
            {
                _weights[0] = 1;
                _weights[1] = -1;
                _weights[2] = 0;

                _reactions[0] = new TextObject("{=Dramalord359}That is good to hear! {POI.NAME} is a fine person to know.");
                _reactions[1] = new TextObject("{=Dramalord360}That is most unfortunate. {POI.NAME} and I get along quite well.");
                _reactions[2] = new TextObject("{=Dramalord361}Oh? That is a pity. {POI.NAME} is someone I have always thought well of.");
            }
            else if (challenger.GetBaseHeroRelation(poi) < DramalordMCM.Instance.MaxTrustEnemies)
            {
                _weights[0] = -1;
                _weights[1] = 1;
                _weights[2] = 0;

                _reactions[0] = new TextObject("{=Dramalord362}Hmph. You admire {POI.NAME}? How very disappointing.");
                _reactions[1] = new TextObject("{=Dramalord363}I completely agree! {POI.NAME} left a very poor impression on me.");
                _reactions[2] = new TextObject("{=Dramalord364}Well, I do not care for {POI.NAME} much at all.");
            }
            else
            {
                _weights[0] = 0;
                _weights[1] = 0;
                _weights[2] = 1;

                _reactions[0] = new TextObject("{=Dramalord365}You seem quick to form an opinion about {POI.NAME}, I have to say.");
                _reactions[1] = new TextObject("{=Dramalord365}You seem quick to form an opinion about {POI.NAME}, I have to say.");
                _reactions[2] = new TextObject("{=Dramalord366}I do not harbor an opinion about {POI.NAME}, either. I do not yet know them well enough.");
            }

            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _reactions[0]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _reactions[1]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, _reactions[2]);
        }

        internal static void StartTraitChallenge()
        {
            Hero challenger = Hero.OneToOneConversationHero;

            int random = (MBRandom.RandomInt(0, 100) % 5) + 1; //mercy valor honor generosity calculating

            TextObject good = new TextObject("{=Dramalord367}That is an excellent point of view! Agreed.");
            TextObject bad = new TextObject("{=Dramalord368}Well, I am afraid I must disagree with you on that.");
            TextObject neutral = new TextObject("{=Dramalord369}Hm. You seem to be quite opinionated.");

            if (random == 1)
            {
                _question = new TextObject("{=Dramalord370}I heard tale of a warrior that was executed for renouncing his oaths and turning to banditry, but it is said that he did it to feed the hungry. Did his ends justify his means? What do you think?");
                _answers[0] = new TextObject("{=Dramalord371}There is always another way, even if it presents great difficulty. One should not compromise their honor.");
                _answers[1] = new TextObject("{=Dramalord372}Having a code is almost as important as knowing when to break it.");
                _answers[2] = new TextObject("{=Dramalord373}People starve, people steal, and people die. It is just the way of things.");

                _weights[0] = (challenger.GetHeroTraits().Honor != 0) ? challenger.GetHeroTraits().Honor : -1;
                _weights[1] = (challenger.GetHeroTraits().Honor != 0) ? challenger.GetHeroTraits().Honor * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Honor == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Honor > 0) ? good : (challenger.GetHeroTraits().Honor < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Honor < 0) ? good : (challenger.GetHeroTraits().Honor > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Honor == 0) ? good : bad;
            }
            else if (random == 2)
            {
                _question = new TextObject("{=Dramalord374}I recently heard a story about a great commander who won a difficult victory against impossible odds. Would you still go to battle if you knew the odds were against you?");
                _answers[0] = new TextObject("{=Dramalord375}Yes, I would. Sometimes it is more important to fight than to be victorious. The world belongs to the brave");
                _answers[1] = new TextObject("{=Dramalord376}No. Why would I risk my life when I am certain to lose? That would be foolish.");
                _answers[2] = new TextObject("{=Dramalord377}Hm. I cannot say. I think that it would depend on the situation.");

                _weights[0] = (challenger.GetHeroTraits().Valor != 0) ? challenger.GetHeroTraits().Valor : -1;
                _weights[1] = (challenger.GetHeroTraits().Valor != 0) ? challenger.GetHeroTraits().Valor * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Valor == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Valor > 0) ? good : (challenger.GetHeroTraits().Valor < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Valor < 0) ? good : (challenger.GetHeroTraits().Valor > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Valor == 0) ? good : bad;
            }
            else if (random == 3)
            {
                _question = new TextObject("{=Dramalord378}I have heard that some lords allow defeated foes to leave the battlefield unharmed. Some think it admirable, others deem it a dangerous folly. What do you think?");
                _answers[0] = new TextObject("{=Dramalord379}They allowed them to go home to their families. I would do the same.");
                _answers[1] = new TextObject("{=Dramalord380}Off with their heads, I say! That would make it difficult for them to march to battle again.");
                _answers[2] = new TextObject("{=Dramalord381}It is the nature of war—some live, some do not. That is just the way of things.");

                _weights[0] = (challenger.GetHeroTraits().Mercy != 0) ? challenger.GetHeroTraits().Mercy : -1;
                _weights[1] = (challenger.GetHeroTraits().Mercy != 0) ? challenger.GetHeroTraits().Mercy * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Mercy == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Mercy > 0) ? good : (challenger.GetHeroTraits().Mercy < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Mercy < 0) ? good : (challenger.GetHeroTraits().Mercy > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Mercy == 0) ? good : bad;
            }
            else if (random == 4)
            {
                _question = new TextObject("{=Dramalord382}Recently, I heard tale told of an arena champion who gave all of his tournament winnings away to the poor.");
                _answers[0] = new TextObject("{=Dramalord383}That is quite the act of generosity. People such as that are a rarity, unfortunately.");
                _answers[1] = new TextObject("{=Dramalord384}Bah! What a waste. What is the point of gold and glory if not to keep it?");
                _answers[2] = new TextObject("{=Dramalord385}Well, I suppose that it is their money, thus it is their choice.");

                _weights[0] = (challenger.GetHeroTraits().Generosity != 0) ? challenger.GetHeroTraits().Generosity : -1;
                _weights[1] = (challenger.GetHeroTraits().Generosity != 0) ? challenger.GetHeroTraits().Generosity * -1 : -1;
                _weights[2] = (challenger.GetHeroTraits().Generosity == 0) ? 1 : -1;

                _reactions[0] = (challenger.GetHeroTraits().Generosity > 0) ? good : (challenger.GetHeroTraits().Generosity < 0) ? bad : neutral;
                _reactions[1] = (challenger.GetHeroTraits().Generosity < 0) ? good : (challenger.GetHeroTraits().Generosity > 0) ? bad : neutral;
                _reactions[2] = (challenger.GetHeroTraits().Generosity == 0) ? good : bad;
            }
            else if (random == 5)
            {
                _question = new TextObject("{=Dramalord386}I heard that a noble clan nearly turned against their own king after being denied a request. Some call it reckless, others see it as a bold gambit. What do you think?");
                _answers[0] = new TextObject("{=Dramalord387}That was a rash decision. One should not throw away power over a fleeting insult.");
                _answers[1] = new TextObject("{=Dramalord388}I can understand the urge. Sometimes you must act on your convictions, consequences be damned.");
                _answers[2] = new TextObject("{=Dramalord389}Difficult to say. Life is complicated, and I can hardly judge such things.");

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

            TextObject good = new TextObject("{=Dramalord390}Oh, you are making me blush! I am delighted to hear that.");
            TextObject bad = new TextObject("{=Dramalord391}Does that mean that you do not consider me to be attractive?");
            TextObject neutral = new TextObject("{=Dramalord392}You truly do not have any preference? Hm.");

            if (random == 1)
            {
                _question = new TextObject("{=Dramalord393}Do you prefer the company of those alike to yourself, or do you find charm in a more opposite sort?");
                _answers[0] = new TextObject("{=Dramalord321}I find myself primarily drawn to those of the opposite sex. As the saying goes, opposites attract.");
                _answers[1] = new TextObject("{=Dramalord322}I am most inclined toward those of my own sex. As they say, birds of a feather flock together.");
                _answers[2] = new TextObject("{=Dramalord394}I do not possess any particular preference.");

                _weights[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? 1 : -1;
                _weights[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? 1 : -1;
                _weights[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? 1 : 0;

                _reactions[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? good : bad;
                _reactions[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? good : bad;
                _reactions[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? good : neutral;
            }
            else if (random == 2)
            {
                _question = new TextObject("{=Dramalord395}I overheard a debate at the tavern—some were saying they enjoy a fuller figure, while others prefer a leaner form. What do you think?");
                _answers[0] = new TextObject("{=Dramalord327}I favor those of a fuller figure—more to hold, more to enjoy.");
                _answers[1] = new TextObject("{=Dramalord325}I find that slender figures possess a certain elegance.");
                _answers[2] = new TextObject("{=Dramalord326}I prefer a balance—not too thin, nor too large. A form that is just right.");

                _weights[0] = (challenger.Weight >= 0.7) ? 1 : -1;
                _weights[1] = (challenger.Weight <= 0.3) ? 1 : -1;
                _weights[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? 1 : -1;

                _reactions[0] = (challenger.Weight >= 0.7) ? good : bad;
                _reactions[1] = (challenger.Weight <= 0.3) ? good : bad;
                _reactions[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? good : bad;
            }
            else if (random == 3)
            {
                _question = new TextObject("{=Dramalord396}Many people admire a strong and powerful form. Others like the slender and agile. Where do your tastes lie?");
                _answers[0] = new TextObject("{=Dramalord330}I am quite fond of strong, muscular figures. The more imposing, the better.");
                _answers[1] = new TextObject("{=Dramalord328}I tend towards those who are lean. Sometimes less is more.");
                _answers[2] = new TextObject("{=Dramalord329}A well-toned body suits me best—Athletic, maybe, but nothing excessive. Somewhere in the middle.");

                _weights[0] = (challenger.Build >= 0.7) ? 1 : -1;
                _weights[1] = (challenger.Build <= 0.3) ? 1 : -1;
                _weights[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? 1 : -1;

                _reactions[0] = (challenger.Build >= 0.7) ? good : bad;
                _reactions[1] = (challenger.Build <= 0.3) ? good : bad;
                _reactions[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? good : bad;
            }
            else if (random == 4)
            {
                _question = new TextObject("{=Dramalord397}Some are known to rob the cradle, others are known to rob the grave. What about you?");
                _answers[0] = new TextObject("{=Dramalord398}I tend towards people older than myself. They say that wine grows better with age.");
                _answers[1] = new TextObject("{=Dramalord399}I enjoy the company of people younger than myself. I find their energy invigorating.");
                _answers[2] = new TextObject("{=Dramalord400}I usually enjoy being around people near to my own age.");

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
                _question = new TextObject("{=Dramalord401}I have heard it said that the world belongs to those willing to embrace change. What do you think?");
                _answers[0] = new TextObject("{=Dramalord402}Novel experiences bring life its color. I welcome change.");
                _answers[1] = new TextObject("{=Dramalord403}I see no need to meddle with what already works. A steady routine serves me well.");
                _answers[2] = new TextObject("{=Dramalord404}Change happens, whether we welcome it or not. I simply take things as they come.");

                _weights[0] = (challenger.GetPersonality().Openness > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Openness < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Openness <= 16 && challenger.GetPersonality().Openness >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Openness > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Openness < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Openness <= 16 && challenger.GetPersonality().Openness >= -16) ? good : neutral;
            }
            else if (random == 2)
            {
                _question = new TextObject("{=Dramalord405}Tell me, do you find comfort in careful planning, or do you prefer to take life as it comes?");
                _answers[0] = new TextObject("{=Dramalord406}Order and structure are the foundation of success. A good plan is everything.");
                _answers[1] = new TextObject("{=Dramalord407}Plans? No, not really. I usually follow the road where it takes me.");
                _answers[2] = new TextObject("{=Dramalord408}Sometimes I plan, sometimes I do not. It depends on the day.");

                _weights[0] = (challenger.GetPersonality().Conscientiousness > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Conscientiousness < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Conscientiousness <= 16 && challenger.GetPersonality().Conscientiousness >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Conscientiousness > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Conscientiousness < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Conscientiousness <= 16 && challenger.GetPersonality().Conscientiousness >= -16) ? good : neutral;
            }
            else if (random == 3)
            {
                _question = new TextObject("{=Dramalord409}Feasts bring together all sorts—lords, merchants, poets, and travelers. Some relish the company, while others prefer a quiet corner. What about you?");
                _answers[0] = new TextObject("{=Dramalord410}Oh, absolutely! I thrive in good company. The more people, the better!");
                _answers[1] = new TextObject("{=Dramalord411}I prefer to keep to myself. Large crowds can be exhausting.");
                _answers[2] = new TextObject("{=Dramalord412}Well, I suppose that depends. I enjoy company, but too much can be overwhelming.");

                _weights[0] = (challenger.GetPersonality().Extroversion > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Extroversion < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Extroversion <= 16 && challenger.GetPersonality().Extroversion >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Extroversion > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Extroversion < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Extroversion <= 16 && challenger.GetPersonality().Extroversion >= -16) ? good : neutral;
            }
            else if (random == 4)
            {
                _question = new TextObject("{=Dramalord413}When a friend confides in you about something that weighs heavily on their heart. Do you offer comfort, or is it best they handle it on their own?");
                _answers[0] = new TextObject("{=Dramalord414}Of course, I try to comfort them. If I can help, I will.");
                _answers[1] = new TextObject("{=Dramalord415}Truthfully, I am not always the best person for that. People must learn to handle their own troubles.");
                _answers[2] = new TextObject("{=Dramalord416}I suppose that it depends on my mood and the situation.");

                _weights[0] = (challenger.GetPersonality().Agreeableness > 16) ? 1 : -1;
                _weights[1] = (challenger.GetPersonality().Agreeableness < -16) ? 1 : -1;
                _weights[2] = (challenger.GetPersonality().Agreeableness <= 16 && challenger.GetPersonality().Agreeableness >= -16) ? 1 : 0;

                _reactions[0] = (challenger.GetPersonality().Agreeableness > 16) ? good : bad;
                _reactions[1] = (challenger.GetPersonality().Agreeableness < -16) ? good : bad;
                _reactions[2] = (challenger.GetPersonality().Agreeableness <= 16 && challenger.GetPersonality().Agreeableness >= -16) ? good : neutral;
            }
            else if (random == 5)
            {
                _question = new TextObject("{=Dramalord417}The burdens of leadership can weigh heavily on the mind. Some struggle under the pressure, while others thrive in it. How do you manage?");
                _answers[0] = new TextObject("{=Dramalord418}Oh, the stress can be unbearable at times. When it becomes too much, I shut myself away for days.");
                _answers[1] = new TextObject("{=Dramalord419}Stress? I hardly notice it. I find I rather enjoy the challenge.");
                _answers[2] = new TextObject("{=Dramalord420}I manage well enough, though I do take time to rest when needed.");

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
