using Dramalord.Actions;
using Dramalord.Data;
using Dramalord.Extensions;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class ConversationHelper
    {
        internal static TextObject PlayerTitle(bool capital) => GetHeroGreeting(Hero.OneToOneConversationHero, Hero.MainHero, capital);
        internal static TextObject NpcTitle(bool capital) => GetHeroGreeting(Hero.MainHero, Hero.OneToOneConversationHero, capital);

        internal static TextObject GetHeroGreeting(Hero hero, Hero target, bool capital)
        {
            RelationshipType relationship = hero.GetRelationTo(target).Relationship;
            string text;
            if (relationship == RelationshipType.Spouse)
            {
                text = target.IsFemale ? new TextObject("{=8eHRth3U}my wife").ToString() : new TextObject("{=QuVgluRH}my husband").ToString();
            }
            else if (relationship == RelationshipType.Betrothed)
            {
                text = new TextObject("{=Dramalord025}my betrothed").ToString();
            }
            else if (relationship == RelationshipType.Lover)
            {
                text = target.IsFemale ? new TextObject("{=Dramalord024}my love").ToString() : new TextObject("{=Dramalord023}my lover").ToString();
            }
            else if (relationship == RelationshipType.FriendWithBenefits)
            {
                text = new TextObject("{=Dramalord026}my special friend").ToString();
            }
            else if (relationship == RelationshipType.Friend)
            {
                text = new TextObject("{=Dramalord174}my friend").ToString();
            }
            else if(hero.Father == target)
            {
                text = target.IsFemale ? GameTexts.FindText("str_mother").ToString() : GameTexts.FindText("str_father").ToString();
            }
            else if (hero.Mother == target)
            {
                text = target.IsFemale ? GameTexts.FindText("str_mother").ToString() : GameTexts.FindText("str_father").ToString();
            }
            else
            {
                text = target.IsFemale ? GameTexts.FindText("str_my_lady").ToString() : GameTexts.FindText("str_my_lord").ToString();
            }

            if (capital)
            {
                char[] array = text.ToCharArray();
                text = array[0].ToString().ToUpper();
                for (int i = 1; i < array.Length; i++)
                {
                    text += array[i];
                }
            }

            return new TextObject(text);
        }

        internal static string FormatNumber(int number)
        {
            return (number > 0) ? "+" + number.ToString() : number.ToString();
        }

        internal static HeroIntention? ConversationEndedIntention = null;
        internal static void OnConversationEnded(IEnumerable<CharacterObject> characters)
        {
            if(ConversationEndedIntention != null)
            {
                
                Hero npc = characters.FirstOrDefault(hero => hero.HeroObject != null && hero.HeroObject != Hero.MainHero).HeroObject;
                Hero player = Hero.MainHero;
                HeroIntention intention = ConversationEndedIntention;
                List<Hero> closeHeroes = player.GetCloseHeroes();

                ConversationEndedIntention = null;

                if (intention.Type == IntentionType.SmallTalk)
                {
                    if(intention.Target == npc) TalkAction.Apply(player, npc);
                    else if (intention.Target == player) TalkAction.Apply(npc, player);

                    if(!npc.HasAnyRelationshipWith(player) && npc.GetRelationTo(player).Trust >= DramalordMCM.Instance.MinTrust)
                    {
                        FriendshipAction.Apply(player, npc);
                    }
                }
                else if(intention.Type == IntentionType.Flirt)
                {
                    if (intention.Target == npc) FlirtAction.Apply(player, npc);
                    else if (intention.Target == player) FlirtAction.Apply(npc, player);
                }
                else if(intention.Type == IntentionType.Date)
                {
                    if (intention.Target == npc) DateAction.Apply(player, npc, closeHeroes);
                    else if (intention.Target == player) DateAction.Apply(npc, player, closeHeroes);

                    if(!npc.IsEmotionalWith(player) && npc.GetRelationTo(player).Love >= DramalordMCM.Instance.MinDatingLove)
                    {
                        LoverAction.Apply(player, npc);
                    }
                }
                else if (intention.Type == IntentionType.Intercourse)
                {
                    if (intention.Target == npc) IntercourseAction.Apply(player, npc, closeHeroes);
                    else if (intention.Target == player) IntercourseAction.Apply(npc, player, closeHeroes);

                    if (npc.IsFriendOf(player))
                    {
                        FriendsWithBenefitsAction.Apply(npc, player);
                    }

                    if (npc.IsFemale != player.IsFemale && npc.IsFertile() && MBRandom.RandomInt(1,100) < DramalordMCM.Instance.PregnancyChance)
                    {
                        ConceiveAction.Apply((npc.IsFemale) ? npc : player, (npc.IsFemale) ? player : npc);
                    }
                }
                else if (intention.Type == IntentionType.Engagement)
                {
                    if (intention.Target == npc) EngagementAction.Apply(player, npc, closeHeroes);
                    else if (intention.Target == player) EngagementAction.Apply(npc, player, closeHeroes);
                }
                else if (intention.Type == IntentionType.Marriage)
                {
                    if (intention.Target == npc) MarriageAction.Apply(player, npc, closeHeroes);
                    else if (intention.Target == player) MarriageAction.Apply(npc, player, closeHeroes);
                }
                else if(intention.Type == IntentionType.BreakUp)
                {
                    if (intention.Target == npc) BreakupAction.Apply(player, npc);
                    else if (intention.Target == player) BreakupAction.Apply(npc, player);
                }

                if (npc.IsFriendlyWith(intention.Target) && npc.GetRelationTo(player).Trust <= 0)
                {
                    BreakupAction.Apply(npc, player);
                }
            }
        }
    }
}
