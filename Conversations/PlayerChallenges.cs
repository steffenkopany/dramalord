using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Extensions;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{

    internal static class PlayerChallenges
    {
        public enum ChallengeContext
        {
            Chat,
            Flirt,
            Date
        }

        private class Challenge
        {
            public Challenge(ChallengeContext context)
            {
                Context = context;
            }
            public ChallengeContext Context { get; set; } = ChallengeContext.Chat;

            public TextObject Question { get; set; } = new TextObject();

            public TextObject[] Answers { get; set; } = new TextObject[3] { new TextObject(), new TextObject(), new TextObject() };

            public TextObject[] Reactions { get; set; } = new TextObject[3] { new TextObject(), new TextObject(), new TextObject() };

            public int[] ConsequenceWeights { get; set; } = new int[3] { 0, 0, 0 };

            public int AnswerIndex { get; set; } = -1;
        }

        private static Challenge? CurrentChallenge = null;
        internal static int ChallengeNumber = 0;
        internal static int ChallengeResult = 0;
        internal static bool ExitConversation = false;

        private static TextObject player_challenge_exit = new("{=Dramalord101}Let's talk about something else, {TITLE}.");
        private static TextObject npc_challenge_exit = new("{=Dramalord186}As you wish, {TITLE}.");
        private static TextObject npc_challenge_summarize_continue = new("{=Dramalord101}Let's talk about something else, {TITLE}.");
        private static TextObject npc_challenge_summarize_end = new("{=Dramalord351}Thank you for the conversation {TITLE}.");

        public static void AddDialogs(CampaignGameStarter starter)
        {
            starter.AddDialogLine("npc_challenge_start", "player_challenge_start", "player_challenge_reply", "{CHALLENGE_QUESTION}[ib:aggressive][if:convo_undecided_open]", ConditionNpcChallengeStart, null);

            starter.AddPlayerLine("player_challenge_reply1", "player_challenge_reply", "npc_challenge_reaction1", "{CHALLENGE_REPLY_1}", null, ConsequencePlayerAnswer1);
            starter.AddPlayerLine("player_challenge_reply2", "player_challenge_reply", "npc_challenge_reaction2", "{CHALLENGE_REPLY_2}", null, ConsequencePlayerAnswer2);
            starter.AddPlayerLine("player_challenge_reply3", "player_challenge_reply", "npc_challenge_reaction3", "{CHALLENGE_REPLY_3}", null, ConsequencePlayerAnswer3);
            starter.AddPlayerLine("player_challenge_exit", "player_challenge_reply", "npc_challenge_exit", "{player_challenge_exit}", null, null);

            starter.AddDialogLine("npc_challenge_exit", "npc_challenge_exit", "player_interaction_selection", "{npc_challenge_exit}", null, null);

            starter.AddDialogLine("npc_challenge_reaction1", "npc_challenge_reaction1", "npc_challenge_summarize", "{CHALLENGE_REACTION_1}{ANSWER_REACTION}", ConditionNpcReaction, ConsequenceNpcReaction);
            starter.AddDialogLine("npc_challenge_reaction2", "npc_challenge_reaction2", "npc_challenge_summarize", "{CHALLENGE_REACTION_2}{ANSWER_REACTION}", ConditionNpcReaction, ConsequenceNpcReaction);
            starter.AddDialogLine("npc_challenge_reaction3", "npc_challenge_reaction3", "npc_challenge_summarize", "{CHALLENGE_REACTION_3}{ANSWER_REACTION}", ConditionNpcReaction, ConsequenceNpcReaction);

            starter.AddDialogLine("npc_challenge_summarize_continue", "npc_challenge_summarize", "player_challenge_start", "{npc_challenge_summarize_continue}{SUMMARIZE_REACTION}", ConditionNpcChallengeContinue, ConsequenceNpcChallengeContinue);
            starter.AddDialogLine("npc_challenge_summarize_end", "npc_challenge_summarize", "player_interaction_selection", "{npc_challenge_summarize_end}{SUMMARIZE_REACTION}", ConditionNpcChallengeEnd, ConsequenceNpcChallengeEnd);
            starter.AddDialogLine("npc_challenge_summarize_exit", "npc_challenge_summarize", "close_window", "{npc_challenge_summarize_end}", ConditionNpcChallengeExit, ConsequenceNpcChallengeExit);
        }

        private static bool ConditionNpcChallengeStart()
        {
            MBTextManager.SetTextVariable("CHALLENGE_QUESTION", CurrentChallenge?.Question);
            MBTextManager.SetTextVariable("CHALLENGE_REPLY_1", CurrentChallenge?.Answers[0]);
            MBTextManager.SetTextVariable("CHALLENGE_REPLY_2", CurrentChallenge?.Answers[1]);
            MBTextManager.SetTextVariable("CHALLENGE_REPLY_3", CurrentChallenge?.Answers[2]);

            player_challenge_exit.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.OneToOneConversationHero.Name : ConversationHelper.NpcTitle(false));
            npc_challenge_exit.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_challenge_summarize_continue.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            npc_challenge_summarize_end.SetTextVariable("TITLE", Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));

            MBTextManager.SetTextVariable("player_challenge_exit", player_challenge_exit);
            MBTextManager.SetTextVariable("npc_challenge_exit", npc_challenge_exit);
            MBTextManager.SetTextVariable("npc_challenge_summarize_continue", npc_challenge_summarize_continue);
            MBTextManager.SetTextVariable("npc_challenge_summarize_end", npc_challenge_summarize_end);

            ChallengeNumber--;
            return true;
        }

        private static bool ConditionNpcReaction()
        {
            if (CurrentChallenge?.ConsequenceWeights[CurrentChallenge.AnswerIndex] > 0)
            {
                MBTextManager.SetTextVariable("ANSWER_REACTION", "[ib:confident][if:convo_delighted]");
            }
            else
            {
                MBTextManager.SetTextVariable("ANSWER_REACTION", "[ib:nervous][if:convo_grave]");
            }
            return true;
        }

        private static bool ConditionNpcChallengeExit()
        {
            if (ChallengeNumber <= 0 && ExitConversation)
            {
                if (ChallengeResult > 0)
                {
                    MBTextManager.SetTextVariable("SUMMARIZE_REACTION", "[ib:demure][if:convo_bemused]");
                }
                else
                {
                    MBTextManager.SetTextVariable("SUMMARIZE_REACTION", "[ib:closed][if:convo_bored]");
                }
                return true;
            }
            return false;
        }

        private static bool ConditionNpcChallengeContinue()
        {
            if (ChallengeNumber > 0)
            {
                if(CurrentChallenge?.ConsequenceWeights[CurrentChallenge.AnswerIndex] > 0)
                {
                    MBTextManager.SetTextVariable("SUMMARIZE_REACTION", "[ib:confident2][if:convo_focused_happy]");
                }
                else
                {
                    MBTextManager.SetTextVariable("SUMMARIZE_REACTION", "[ib:nervous][if:convo_shocked]");
                }
                return true;
            }
            return false;
        }

        private static bool ConditionNpcChallengeEnd()
        {
            if(ChallengeNumber <= 0 && !ExitConversation)
            {
                if(ChallengeResult > 0)
                {
                    MBTextManager.SetTextVariable("SUMMARIZE_REACTION", "[ib:demure][if:convo_bemused]");
                }
                else
                {
                    MBTextManager.SetTextVariable("SUMMARIZE_REACTION", "[ib:closed][if:convo_bored]");
                }
                return true;
            }
            return false;
        }

        private static void ConsequencePlayerAnswer1()
        {
            CurrentChallenge.AnswerIndex = 0;
            MBTextManager.SetTextVariable("CHALLENGE_REACTION_1", CurrentChallenge?.Reactions[0]);
        }
        private static void ConsequencePlayerAnswer2()
        {
            CurrentChallenge.AnswerIndex = 1;
            MBTextManager.SetTextVariable("CHALLENGE_REACTION_2", CurrentChallenge?.Reactions[1]);
        }
        private static void ConsequencePlayerAnswer3()
        {
            CurrentChallenge.AnswerIndex = 2;
            MBTextManager.SetTextVariable("CHALLENGE_REACTION_3", CurrentChallenge?.Reactions[2]);
        }

        private static void ConsequenceNpcReaction()
        {
            ChallengeResult += CurrentChallenge?.ConsequenceWeights[CurrentChallenge.AnswerIndex] ?? 0;
            if (ChallengeNumber <= 0)
            {
                int sympathy = Hero.OneToOneConversationHero.GetSympathyTo(Hero.MainHero);
                int changeValue = (sympathy <= 0) ? 1 + (Hero.MainHero.GetSkillValue(DefaultSkills.Charm) / 100) : sympathy + (Hero.MainHero.GetSkillValue(DefaultSkills.Charm) / 100);

                if (CurrentChallenge.Context == ChallengeContext.Chat)
                {
                    TalkAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, changeValue * ChallengeResult);
                    ChangeSympathy();
                    if (!Hero.OneToOneConversationHero.HasAnyRelationshipWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).Trust >= DramalordMCM.Instance.MinTrust)
                    {
                        FriendshipAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
                        PlayerApproachingNPC.SetupLines();
                    }
                }
                else if (CurrentChallenge.Context == ChallengeContext.Flirt)
                {
                    FlirtAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, changeValue * ChallengeResult);
                    ChangeAttraction();
                }
                else if (CurrentChallenge.Context == ChallengeContext.Date)
                {
                    DateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Hero.MainHero.GetCloseHeroes(), changeValue * ChallengeResult);
                    ChangeAttraction();
                    ChangeSympathy();
                    if (!Hero.OneToOneConversationHero.IsEmotionalWith(Hero.MainHero) && Hero.OneToOneConversationHero.GetRelationTo(Hero.MainHero).CurrentLove >= DramalordMCM.Instance.MinDatingLove)
                    {
                        LoverAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero);
                        PlayerApproachingNPC.SetupLines();
                    }
                }
            }
        }

        private static void ConsequenceNpcChallengeContinue()
        {
            if (CurrentChallenge.Context == ChallengeContext.Date)
            {
                GenerateRandomDateChallenge();
            }
            else if (CurrentChallenge.Context == ChallengeContext.Flirt)
            {
                GenerateRandomFlirtChallenge();
            }
            else if (CurrentChallenge.Context == ChallengeContext.Chat)
            {
                GenerateRandomChatChallenge();
            }
        }

        private static void ConsequenceNpcChallengeExit()
        {
            CurrentChallenge = null;
            if (PlayerEncounter.Current != null)
            {
                PlayerEncounter.LeaveEncounter = true;
            }
        }

        private static void ConsequenceNpcChallengeEnd()
        {
            CurrentChallenge = null;
        }

        internal static void GenerateRandomChatChallenge()
        {
            int rand = (MBRandom.RandomInt(0, 100) % 2) + 1;
            if (rand == 1) StartHeroOpinionChallenge(ChallengeContext.Chat);
            if (rand == 2) StartTraitChallenge(ChallengeContext.Chat);
        }

        internal static void GenerateRandomFlirtChallenge()
        {
            int rand = (MBRandom.RandomInt(0, 100) % 2) + 1;
            if (rand == 1) StartAttractionChallenge(ChallengeContext.Flirt);
            if (rand == 2) StartPersonalityChallenge(ChallengeContext.Flirt);
        }

        internal static void GenerateRandomDateChallenge()
        {
            int rand = (MBRandom.RandomInt(0, 100) % 4) + 1;
            if (rand == 1) StartHeroOpinionChallenge(ChallengeContext.Date);
            if (rand == 2) StartTraitChallenge(ChallengeContext.Date);
            if (rand == 3) StartAttractionChallenge(ChallengeContext.Date);
            if (rand == 4) StartPersonalityChallenge(ChallengeContext.Date);
        }

        internal static void StartHeroOpinionChallenge(ChallengeContext context)
        {
            Hero challenger = Hero.OneToOneConversationHero;

            CurrentChallenge = new(context);

            Hero? poi = Hero.AllAliveHeroes.Where(h => h.IsLord && h != Hero.MainHero && h != challenger && (challenger.GetBaseHeroRelation(h) > 40 || challenger.GetBaseHeroRelation(h) < -30)).GetRandomElementInefficiently();
            if (poi == null) poi = Hero.AllAliveHeroes.Where(h => h.IsLord && h != Hero.MainHero && h != challenger && h.IsDramalordLegit()).GetRandomElementInefficiently();

            CurrentChallenge.Question = new TextObject("{=Dramalord352}Tell me {TITLE}, as you travel a lot and meet people, what's your opinion about {POI.NAME}?");
            CurrentChallenge.Question.SetTextVariable("TITLE", challenger.HasAnyRelationshipWith(Hero.MainHero) ? Hero.MainHero.Name : ConversationHelper.PlayerTitle(false));
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, CurrentChallenge.Question);

            if(poi?.HasMet ?? false)
            {
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord353}I have met {POI.NAME} and I consider them being a friend of mine.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord354}Ugh, don't remind me. {POI.NAME} is annoying and I hope I won't meet them again.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord355}{POI.NAME}, hmm. I actually don't have any particular opinion about them.");
            }
            else
            {
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord356}Unfortunately I never hat the chance to meet {POI.NAME}, but they sound like a nice person?");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord357}Good thing I never met {POI.NAME}. I can image them being a horrible being.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord358}I do not have any opinion about {POI.NAME}, as I never exchanged a word with them.");
            }

            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, CurrentChallenge.Answers[0]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, CurrentChallenge.Answers[1]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, CurrentChallenge.Answers[2]);

            if (challenger.GetBaseHeroRelation(poi) > 40)
            {
                CurrentChallenge.ConsequenceWeights[0] = 1;
                CurrentChallenge.ConsequenceWeights[1] = -1;
                CurrentChallenge.ConsequenceWeights[2] = 0;

                CurrentChallenge.Reactions[0] = new TextObject("{=Dramalord359}That's good to hear! {POI.NAME} is a good friend of mine as well!");
                CurrentChallenge.Reactions[1] = new TextObject("{=Dramalord360}I'm disappointed to hear that. {POI.NAME} is actually a good friend of mine.");
                CurrentChallenge.Reactions[2] = new TextObject("{=Dramalord361}Well you probably don't know {POI.NAME} well. They're my friend.");
            }
            else if (challenger.GetBaseHeroRelation(poi) < -30)
            {
                CurrentChallenge.ConsequenceWeights[0] = -1;
                CurrentChallenge.ConsequenceWeights[1] = 1;
                CurrentChallenge.ConsequenceWeights[2] = 0;

                CurrentChallenge.Reactions[0] = new TextObject("{=Dramalord362}Ugh, really? You like {POI.NAME}? That's very disappointing.");
                CurrentChallenge.Reactions[1] = new TextObject("{=Dramalord363}I totally agree! {POI.NAME} is like dirt under my fingernails.");
                CurrentChallenge.Reactions[2] = new TextObject("{=Dramalord364}Well you probably don't know {POI.NAME} well. I really don't like them.");
            }
            else
            {
                CurrentChallenge.ConsequenceWeights[0] = 0;
                CurrentChallenge.ConsequenceWeights[1] = 0;
                CurrentChallenge.ConsequenceWeights[2] = 1;

                CurrentChallenge.Reactions[0] = new TextObject("{=Dramalord365}You are fast in building up an opinion about {POI.NAME} I have to say.");
                CurrentChallenge.Reactions[1] = new TextObject("{=Dramalord365}You are fast in building up an opinion about {POI.NAME} I have to say.");
                CurrentChallenge.Reactions[2] = new TextObject("{=Dramalord366}I don't have an opinion about {POI.NAME} as well. I don't know them well enough.");
            }

            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, CurrentChallenge.Reactions[0]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, CurrentChallenge.Reactions[1]);
            StringHelpers.SetCharacterProperties("POI", poi?.CharacterObject, CurrentChallenge.Reactions[2]);
        }

        internal static void StartTraitChallenge(ChallengeContext context)
        {
            Hero challenger = Hero.OneToOneConversationHero;
            CurrentChallenge = new(context);

            int random = (MBRandom.RandomInt(0, 100) % 5) + 1; //mercy valor honor generosity calculating

            TextObject good = new TextObject("{=Dramalord367}That's an excellent point of view! Agreed.");
            TextObject bad = new TextObject("{=Dramalord368}Well, I actually have to diagree with you.");
            TextObject neutral = new TextObject("{=Dramalord369}You are quite opinionated, aren't you?");

            if (random == 1)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord370}Last war our faction raided an enemy village because they ran out of food. It's sad, but I think it had to be done for the greater good. (Honor)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord371}I disagree. Civilians shouldn't suffer because of politics.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord372}Right. Those peasants should have given their goods volunarily.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord373}I don't know. I guess thats just the way things are.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetHeroTraits().Honor != 0) ? challenger.GetHeroTraits().Honor : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetHeroTraits().Honor != 0) ? challenger.GetHeroTraits().Honor * -1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetHeroTraits().Honor == 0) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.GetHeroTraits().Honor > 0) ? good : (challenger.GetHeroTraits().Honor < 0) ? bad : neutral;
                CurrentChallenge.Reactions[1] = (challenger.GetHeroTraits().Honor < 0) ? good : (challenger.GetHeroTraits().Honor > 0) ? bad : neutral;
                CurrentChallenge.Reactions[2] = (challenger.GetHeroTraits().Honor == 0) ? good : bad;
            }
            else if (random == 2)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord374}Have you ever encountered a stronger foe but went to battle nevertheless? (Valor)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord375}Yes, and we've won anyway. This world belongs to the brave!");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord376}No. Why would I risk my life when I know we can't win.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord377}I can't say. I think that depends on the situation.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetHeroTraits().Valor != 0) ? challenger.GetHeroTraits().Valor : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetHeroTraits().Valor != 0) ? challenger.GetHeroTraits().Valor * -1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetHeroTraits().Valor == 0) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.GetHeroTraits().Valor > 0) ? good : (challenger.GetHeroTraits().Valor < 0) ? bad : neutral;
                CurrentChallenge.Reactions[1] = (challenger.GetHeroTraits().Valor < 0) ? good : (challenger.GetHeroTraits().Valor > 0) ? bad : neutral;
                CurrentChallenge.Reactions[2] = (challenger.GetHeroTraits().Valor == 0) ? good : bad;
            }
            else if (random == 3)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord378}I heard that some lords let captured troops simply leave the battlefield. Isn't that a risk? (Mercy)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord379}They let them go home to their families. I would do the same.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord380}Heads off I'd say! Makes it hard for them to go to battle again.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord381}They are there to kill or be killed, so who cares about them.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetHeroTraits().Mercy != 0) ? challenger.GetHeroTraits().Mercy : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetHeroTraits().Mercy != 0) ? challenger.GetHeroTraits().Mercy * -1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetHeroTraits().Mercy == 0) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.GetHeroTraits().Mercy > 0) ? good : (challenger.GetHeroTraits().Mercy < 0) ? bad : neutral;
                CurrentChallenge.Reactions[1] = (challenger.GetHeroTraits().Mercy < 0) ? good : (challenger.GetHeroTraits().Mercy > 0) ? bad : neutral;
                CurrentChallenge.Reactions[2] = (challenger.GetHeroTraits().Mercy == 0) ? good : bad;
            }
            else if (random == 4)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord382}Recently the champion of a tournament gave all of his winnings to the poor. (Generosity)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord383}That is a generous move indeed. This kind of people are rare, unfortunately.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord384}Bah! What a waste of money. Why did they participate in the first place?");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord385}Well, I guess it's their money, thus it's their choice.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetHeroTraits().Generosity != 0) ? challenger.GetHeroTraits().Generosity : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetHeroTraits().Generosity != 0) ? challenger.GetHeroTraits().Generosity * -1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetHeroTraits().Generosity == 0) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.GetHeroTraits().Generosity > 0) ? good : (challenger.GetHeroTraits().Generosity < 0) ? bad : neutral;
                CurrentChallenge.Reactions[1] = (challenger.GetHeroTraits().Generosity < 0) ? good : (challenger.GetHeroTraits().Generosity > 0) ? bad : neutral;
                CurrentChallenge.Reactions[2] = (challenger.GetHeroTraits().Generosity == 0) ? good : bad;
            }
            else if (random == 5)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord386}One of the southern clans almost left their kingdom, after their leader had an argument with their king. (Calculating)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord387}That's stupid. They would have lose their protection because of their pride.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord388}I can understand how they feel. I also loose me temper every now and then.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord389}I cant't judge that move, as long as I don't know what the argument was about.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetHeroTraits().Calculating != 0) ? challenger.GetHeroTraits().Calculating : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetHeroTraits().Calculating != 0) ? challenger.GetHeroTraits().Calculating * -1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetHeroTraits().Calculating == 0) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.GetHeroTraits().Calculating > 0) ? good : (challenger.GetHeroTraits().Calculating < 0) ? bad : neutral;
                CurrentChallenge.Reactions[1] = (challenger.GetHeroTraits().Calculating < 0) ? good : (challenger.GetHeroTraits().Calculating > 0) ? bad : neutral;
                CurrentChallenge.Reactions[2] = (challenger.GetHeroTraits().Calculating == 0) ? good : bad;
            }
        }

        internal static void StartAttractionChallenge(ChallengeContext context)
        {
            Hero challenger = Hero.OneToOneConversationHero;
            CurrentChallenge = new(context);

            int random = (MBRandom.RandomInt(0, 100) % 4) + 1; //sex weight build age 

            TextObject good = new TextObject("{=Dramalord390}Ohh! You're making me blush! I'm happy to hear that.");
            TextObject bad = new TextObject("{=Dramalord391}So does that mean you don't consider me attractive?");
            TextObject neutral = new TextObject("{=Dramalord392}I'm pretty sure you have some kind of preference.");

            if (random == 1)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord393}I've heard that some lords and ladies have a different taste in physical gender. How about you? (Orientation)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord321}I feel drawn to the other sex and I don't have much interest in persons of my own.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord322}I don't have much interest in the other sex, I rather prefer persons of my own.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord394}I actually don't have any specific preference.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? 1 : 0;

                CurrentChallenge.Reactions[0] = (challenger.IsFemale != Hero.MainHero.IsFemale) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.IsFemale == Hero.MainHero.IsFemale) ? good : bad;
                CurrentChallenge.Reactions[2] = ((challenger.GetDesires().AttractionWomen >= DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen >= DramalordMCM.Instance.MinAttraction) || (challenger.GetDesires().AttractionWomen < DramalordMCM.Instance.MinAttraction && challenger.GetDesires().AttractionMen < DramalordMCM.Instance.MinAttraction)) ? good : neutral;
            }
            else if (random == 2)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord395}The owner of the local inn has gained some weight. Now the tavern is packed with gawking customers every night. (Weight)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord327}I like people with more weight. There's more to grab for me.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord325}I think slim people are more grazile then others.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord326}I don't like thin or fat. The middle is just right.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.Weight >= 0.7) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.Weight <= 0.3) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.Weight >= 0.7) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.Weight <= 0.3) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.Weight > 0.3 && challenger.Weight < 0.7) ? good : bad;
            }
            else if (random == 3)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord396}In the highlands lords and ladies are often bulky and powerful. Many people admire them for being strong. (Build)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord330}I love powerful people. The bulkier the better.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord328}Muscles are overrated. I like it skinny and want to see bones.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord329}Medium muscles are just right for me. I don't need something special.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.Build >= 0.7) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.Build <= 0.3) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.Build >= 0.7) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.Build <= 0.3) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.Build > 0.3 && challenger.Build < 0.7) ? good : bad;
            }
            else if (random == 4)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord397}There are people who prefer older people and others prefer younger people. What is your preference? (Age)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord398}I prefer people older then me. Nothing is better then ripe fruit.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord399}I enjoy the energy and stamina of younger people very much.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord400}I think people around my own age are more attractive.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.Age - Hero.MainHero.Age > 5) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.Age - Hero.MainHero.Age < -5) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.Age - Hero.MainHero.Age <= 5 && challenger.Age - Hero.MainHero.Age >= -5) ? 1 : -1;

                CurrentChallenge.Reactions[0] = (challenger.Age - Hero.MainHero.Age > 5) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.Age - Hero.MainHero.Age < -5) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.Age - Hero.MainHero.Age <= 5 && challenger.Age - Hero.MainHero.Age >= -5) ? good : bad;
            }
        }

        internal static void StartPersonalityChallenge(ChallengeContext context)
        {
            Hero challenger = Hero.OneToOneConversationHero;
            CurrentChallenge = new(context);

            int random = (MBRandom.RandomInt(0, 100) % 5) + 1; //openness, conscientiousness, extroversion, agreeableness, neuroticism

            TextObject good = new TextObject("{=YcdQ1MWq}Well.. It seems we have a fair amount in common.");
            TextObject bad = new TextObject("{=dY2PzpIV}I'm not sure how much we have in common..");
            TextObject neutral = new TextObject("{=E9s2bjqw}I can only hope that some day you could change your mind.");

            if (random == 1)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord401}Do you think trying out new things is sometimes exciting? (Openness)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord402}I like trying new things. It's getting boring without change.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord403}Nonsense. Things are good the way they are and don't need to change.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord404}Maybe, I don't really care. Things are changing anyway all the time.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetPersonality().Openness > 33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetPersonality().Openness < -33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetPersonality().Openness <= 33 && challenger.GetPersonality().Openness >= -33) ? 1 : 0;

                CurrentChallenge.Reactions[0] = (challenger.GetPersonality().Openness > 33) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.GetPersonality().Openness < -33) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.GetPersonality().Openness <= 33 && challenger.GetPersonality().Openness >= -33) ? good : neutral;
            }
            else if (random == 2)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord405}So, are you rather a person who plans out things, or are you more the sponateous type? (Conscientiousness)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord406}Everything needs order and structure. Planning is everything.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord407}Nah, not really. I usually stumble into my next adventure.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord408}Eh, sometimes I plan, sometimes I don't.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetPersonality().Conscientiousness > 33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetPersonality().Conscientiousness < -33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetPersonality().Conscientiousness <= 33 && challenger.GetPersonality().Conscientiousness >= -33) ? 1 : 0;

                CurrentChallenge.Reactions[0] = (challenger.GetPersonality().Conscientiousness > 33) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.GetPersonality().Conscientiousness < -33) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.GetPersonality().Conscientiousness <= 33 && challenger.GetPersonality().Conscientiousness >= -33) ? good : neutral;
            }
            else if (random == 3)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord409}When there's a tournament in town, are you excited to meet new people? (Extroversion)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord410}Oh absolutely! I barely can't stop chatting when I'm around people.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord411}Ugh. Nah, I'm rather to myself. Most people are bothering me.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord412}Well, that depends. I'd meet a few probably, but not many.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetPersonality().Extroversion > 33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetPersonality().Extroversion < -33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetPersonality().Extroversion <= 33 && challenger.GetPersonality().Extroversion >= -33) ? 1 : 0;

                CurrentChallenge.Reactions[0] = (challenger.GetPersonality().Extroversion > 33) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.GetPersonality().Extroversion < -33) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.GetPersonality().Extroversion <= 33 && challenger.GetPersonality().Extroversion >= -33) ? good : neutral;
            }
            else if (random == 4)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord413}If one of your friends tell you a sad story, how do you react? (Agreeableness)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord414}I try to comfort them, or help if I can of course.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord415}Uhm, my friends know not to bother me with their personal stuff.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord416}That depends on how I feel myself, I guess.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetPersonality().Agreeableness > 33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetPersonality().Agreeableness < -33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetPersonality().Agreeableness <= 33 && challenger.GetPersonality().Agreeableness >= -33) ? 1 : 0;

                CurrentChallenge.Reactions[0] = (challenger.GetPersonality().Agreeableness > 33) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.GetPersonality().Agreeableness < -33) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.GetPersonality().Agreeableness <= 33 && challenger.GetPersonality().Agreeableness >= -33) ? good : neutral;
            }
            else if (random == 5)
            {
                CurrentChallenge.Question = new TextObject("{=Dramalord417}Managing a party is a lot of stress sometimes. How do you deal with it? (Neuroticism)");
                CurrentChallenge.Answers[0] = new TextObject("{=Dramalord418}Oh yes. For recovering I usually lock myself away for a few days.");
                CurrentChallenge.Answers[1] = new TextObject("{=Dramalord419}Well, it's not stressful at all. I am actually rarely stressed.");
                CurrentChallenge.Answers[2] = new TextObject("{=Dramalord420}It's ok I think. Sometimes I need a day off or two.");

                CurrentChallenge.ConsequenceWeights[0] = (challenger.GetPersonality().Neuroticism > 33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[1] = (challenger.GetPersonality().Neuroticism < -33) ? 1 : -1;
                CurrentChallenge.ConsequenceWeights[2] = (challenger.GetPersonality().Neuroticism <= 33 && challenger.GetPersonality().Neuroticism >= -33) ? 1 : 0;

                CurrentChallenge.Reactions[0] = (challenger.GetPersonality().Neuroticism > 33) ? good : bad;
                CurrentChallenge.Reactions[1] = (challenger.GetPersonality().Neuroticism < -33) ? good : bad;
                CurrentChallenge.Reactions[2] = (challenger.GetPersonality().Neuroticism <= 33 && challenger.GetPersonality().Neuroticism >= -33) ? good : neutral;
            }
        }

        private static void ChangeAttraction()
        {
            int result = ChallengeResult;
            if(result == 0)
            {
                return;
            }

            Hero npc = Hero.OneToOneConversationHero;
            HeroDesires desires = npc.GetDesires();
            int oldAttraction = npc.GetAttractionTo(Hero.MainHero);

            if (Hero.MainHero.IsFemale) { desires.AttractionWomen += result; desires.AttractionMen -= result; }
            else if (!Hero.MainHero.IsFemale) { desires.AttractionWomen -= result; desires.AttractionMen += result; }

            int build = (int)(Hero.MainHero.Build * 100);
            if(build < desires.AttractionBuild) { desires.AttractionBuild -= result; }
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
                banner.SetTextVariable("NUMBER", ConversationHelper.FormatNumber(newAttraction - oldAttraction));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }
            else if(newAttraction != oldAttraction)
            {
                TextObject banner = new TextObject("{=Dramalord476}You are now less attractive to {HERO.LINK}. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationHelper.FormatNumber(newAttraction - oldAttraction));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }
        }

        private static void ChangeSympathy()
        {
            int result = ChallengeResult;
            if (result == 0)
            {
                return;
            }

            Hero npc = Hero.OneToOneConversationHero;
            HeroPersonality personality = npc.GetPersonality();
            HeroPersonality playerPersonality = Hero.MainHero.GetPersonality();

            int oldSympathy = npc.GetSympathyTo(Hero.MainHero);

            personality.Openness += (playerPersonality.Openness > personality.Openness) ? result : (playerPersonality.Openness < personality.Openness) ? - result : 0;
            personality.Extroversion += (playerPersonality.Extroversion > personality.Extroversion) ? result : (playerPersonality.Extroversion < personality.Extroversion) ? -result : 0;
            personality.Neuroticism += (playerPersonality.Neuroticism > personality.Neuroticism) ? result : (playerPersonality.Neuroticism < personality.Neuroticism) ? -result : 0;
            personality.Conscientiousness += (playerPersonality.Conscientiousness > personality.Conscientiousness) ? result : (playerPersonality.Conscientiousness < personality.Conscientiousness) ? -result : 0;
            personality.Agreeableness += (playerPersonality.Agreeableness > personality.Agreeableness) ? result : (playerPersonality.Agreeableness < personality.Agreeableness) ? -result : 0;

            int newSympathy = npc.GetSympathyTo(Hero.MainHero);

            if (result > 0 && newSympathy != oldSympathy)
            {
                TextObject banner = new TextObject("{=Dramalord477}{HERO.LINK} has more sympathy for you. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationHelper.FormatNumber(newSympathy - oldSympathy));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }
            else if (newSympathy != oldSympathy)
            {
                TextObject banner = new TextObject("{=Dramalord478}{HERO.LINK} has less sympathy for you. ({NUMBER})");
                StringHelpers.SetCharacterProperties("HERO", npc.CharacterObject, banner);
                banner.SetTextVariable("NUMBER", ConversationHelper.FormatNumber(newSympathy - oldSympathy));
                MBInformationManager.AddQuickInformation(banner, 1000, npc.CharacterObject, "event:/ui/notification/relation");
            }
        }
    }
}
