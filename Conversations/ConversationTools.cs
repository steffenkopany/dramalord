using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.Notifications;
using Helpers;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Conversations
{
    internal static class ConversationTools
    {

        internal static TextObject GetHeroGreeting(Hero hero, Hero target, bool capital)
        {
            bool name = MBRandom.RandomInt(1, 100) < 50;
            RelationshipType relationship = hero.GetRelationTo(target).Relationship;
            string text;
            if (relationship == RelationshipType.Spouse || hero.Spouse == target)
            {
                text = target.IsFemale ? ((name) ? target.FirstName.ToString() : new TextObject("{=8eHRth3U}my wife").ToString()) : ((name) ? target.FirstName.ToString() : new TextObject("{=QuVgluRH}my husband").ToString());
            }
            else if (relationship == RelationshipType.Lover)
            {
                text = target.IsFemale ? ((name) ? target.FirstName.ToString() : new TextObject(DramalordTexts.NAME_MY_LOVE).ToString()) : ((name) ? target.FirstName.ToString() : new TextObject(DramalordTexts.NAME_MY_LOVER).ToString());
            }
            else if (hero.Father == target)
            {
                text = target.IsFemale ? GameTexts.FindText("str_mother").ToString() : GameTexts.FindText("str_father").ToString();
            }
            else if (hero.Mother == target)
            {
                text = target.IsFemale ? GameTexts.FindText("str_mother").ToString() : GameTexts.FindText("str_father").ToString();
            }
            else if (hero.Siblings.Contains(target))
            {
                text = target.IsFemale ? ((name) ? target.FirstName.ToString() : new TextObject(DramalordTexts.NAME_SISTER).ToString()) : ((name) ? target.FirstName.ToString() : new TextObject(DramalordTexts.NAME_BROTHER).ToString());
            }
            else if (target.Father == hero || target.Mother == hero)
            {
                text = target.IsFemale ? (name) ? target.FirstName.ToString() : new TextObject(DramalordTexts.NAME_DAUGHTER).ToString() : (name) ? target.FirstName.ToString() : new TextObject(DramalordTexts.NAME_SON).ToString();
            }
            else if (relationship == RelationshipType.Friend)
            {
                text = (name) ? target.FirstName.ToString() : new TextObject("{=edRggEQ4}my friend").ToString();
            }
            else
            {
                if (hero.IsLord && target.IsLord)
                {
                    text = target.FirstName.ToString();
                }
                else if ((!hero.IsLord && target.IsLord) || target.MapFaction.Leader == target)
                {
                    text = GameTexts.FindText(target.IsFemale ? "str_player_salutation_my_lady" : "str_player_salutation_my_lord").ToString();
                }
                else if (hero.IsPlayerCompanion)
                {
                    text = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_player_salutation_captain", target.CharacterObject).ToString();
                }
                else if (target.IsFemale)
                {
                    text = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_player_salutation_madame", target.CharacterObject).ToString();
                }
                else
                {
                    text = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_player_salutation_sir", target.CharacterObject).ToString();
                }
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

        internal static TextObject GetHeroRelation(Hero hero, Hero partner)
        {
            RelationshipType relation = hero.GetRelationTo(partner).Relationship;
            if (relation == RelationshipType.Friend) return new TextObject(DramalordTexts.NAME_MY_FRIEND);
            if (relation == RelationshipType.Lover) return new TextObject(DramalordTexts.NAME_MY_LOVER);
            if (relation == RelationshipType.Spouse || hero.Spouse == partner) return new TextObject(DramalordTexts.NAME_MY_SPOUSE);
            return new TextObject(DramalordTexts.NAME_MY_ACQUAINTENCE);
        }

        internal static TextObject FormatNumber(int number)
        {
            return new TextObject((number > 0) ? "+" + number.ToString() : number.ToString());
        }

        public static bool SetConversationHero(Hero actor1)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject);
            return true;
        }

        public static bool SetConversationHero(Hero actor1, Hero actor2)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject);
            StringHelpers.SetCharacterProperties("ACTOR2", actor2.CharacterObject);
            return true;
        }

        public static bool SetConversationHero(Hero actor1, Hero actor2, Hero actor3)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject);
            StringHelpers.SetCharacterProperties("ACTOR2", actor2.CharacterObject);
            StringHelpers.SetCharacterProperties("ACTOR3", actor3.CharacterObject);
            return true;
        }

        public static bool SetConversationText(TextObject text1)
        {
            MBTextManager.SetTextVariable("TEXT1", text1);
            return true;
        }

        public static bool SetConversationText(TextObject text1, TextObject text2)
        {
            MBTextManager.SetTextVariable("TEXT1", text1);
            MBTextManager.SetTextVariable("TEXT2", text2);
            return true;
        }
        public static bool SetConversationText(TextObject text1, TextObject text2, TextObject text3)
        {
            MBTextManager.SetTextVariable("TEXT1", text1);
            MBTextManager.SetTextVariable("TEXT2", text2);
            MBTextManager.SetTextVariable("TEXT3", text3);
            return true;
        }

        public static TextObject SetCharacterObjects(TextObject textObject, Hero actor1)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject, textObject);
            return textObject;
        }

        public static TextObject SetCharacterObjects(TextObject textObject, Hero actor1, Hero actor2)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("ACTOR2", actor2.CharacterObject, textObject);
            return textObject;
        }

        public static TextObject SetCharacterObjects(TextObject textObject, Hero actor1, Hero actor2, Hero actor3)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("ACTOR2", actor2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("ACTOR3", actor3.CharacterObject, textObject);
            return textObject;
        }

        public static TextObject SetCharacterObjects(TextObject textObject, Hero actor1, Hero actor2, Hero actor3, Hero actor4)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("ACTOR2", actor2.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("ACTOR3", actor3.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("ACTOR4", actor4.CharacterObject, textObject);
            return textObject;
        }

        public static TextObject SetTextVariables(TextObject textObject, string text1) => SetTextVariables(textObject, new TextObject(text1));
        public static TextObject SetTextVariables(TextObject textObject, string text1, string text2) => SetTextVariables(textObject, new TextObject(text1), new TextObject(text2));
        public static TextObject SetTextVariables(TextObject textObject, string text1, string text2, string text3) => SetTextVariables(textObject, new TextObject(text1), new TextObject(text2), new TextObject(text3));

        public static TextObject SetTextVariables(TextObject textObject, TextObject text1)
        {
            return textObject.SetTextVariable("TEXT1", text1);
        }

        public static TextObject SetTextVariables(TextObject textObject, TextObject text1, TextObject text2)
        {
            textObject.SetTextVariable("TEXT1", text1);
            return textObject.SetTextVariable("TEXT2", text2);
        }

        public static TextObject SetTextVariables(TextObject textObject, TextObject text1, TextObject text2, TextObject text3)
        {
            textObject.SetTextVariable("TEXT1", text1);
            textObject.SetTextVariable("TEXT2", text2);
            return textObject.SetTextVariable("TEXT3", text3);
        }
    }
}
