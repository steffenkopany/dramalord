using Dramalord.Conversations;
using Dramalord.Extensions;
using Dramalord.UI;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;

namespace Dramalord.Notifications
{
    internal class DramalordImageNotification
    {
        public enum ImageContext
        {
            Adoption,
            Masturbation,
            Neglection
        }

        public static void ShowDramalordImageNotification(Hero hero1, Hero? hero2 = null, Hero? hero3 = null, ImageContext context = ImageContext.Adoption)
        {
            string imageFile = "notification_";

            if (context == ImageContext.Adoption)
            {
                imageFile += "adopt_";
            }
            else if(context == ImageContext.Masturbation)
            {
                imageFile += "masturbation_";

                if (hero1.CurrentSettlement != null)
                {
                    imageFile += "castle_";
                }
                else if(hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea) 
                {
                    imageFile += "ship_";
                }
                else
                {
                    imageFile += "tent_";
                }
            }
            else if (context == ImageContext.Neglection)
            {
                imageFile += "neglected_";

                if (hero1.CurrentSettlement != null)
                {
                    imageFile += "castle_";
                }
                else if (hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea)
                {
                    imageFile += "ship_";
                }
                else
                {
                    imageFile += "tent_";
                }
            }

            if (context == ImageContext.Masturbation)
            {
                if (hero1.IsFemale)
                {
                    imageFile += "f";
                }
                else
                {
                    imageFile += "m";
                }
            }
            else if (context == ImageContext.Neglection)
            {
                Hero otherHero = hero1 == Hero.MainHero ? hero2 : hero1;
                if (Hero.MainHero.IsFemale)
                {
                    imageFile += "f_";
                }
                else
                {
                    imageFile += "m_";
                }

                if (otherHero.IsFemale)
                {
                    imageFile += "f";
                }
                else
                {
                    imageFile += "m";
                }
            }
            else
            {
                Hero otherHero = hero1 == Hero.MainHero ? hero2 : hero1;
                if (hero1.IsFemale != hero2.IsFemale)
                {
                    imageFile += "m_f";
                }
                else if (hero1.IsFemale)
                {
                    imageFile += "f_f";
                }
                else
                {
                    imageFile += "m_m";
                }
            }
            

            imageFile += ".jpg";

            string basePath = ModuleHelper.GetModuleFullPath("Dramalord");
            string imagePath = System.IO.Path.Combine(basePath, "GUI", "Images", "notification", imageFile);

            string imageTitle = string.Empty;
            string imageSound = string.Empty;

            if (context == ImageContext.Adoption)
            {
                if (hero3 != null)
                {
                    imageTitle = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_ADOPT_2), hero1, hero3, hero2).ToString();
                }
                else
                {
                    imageTitle = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_ADOPT), hero1, hero2).ToString();
                }

                imageSound = "wedding_chime";
            }
            else if (context == ImageContext.Masturbation)
            {
                int loveLoss = (hero1.GetRelationTo(Hero.MainHero).Love / 4) * -1;
                TextObject textObject = new TextObject(DramalordTexts.BANNER_MASTURBATION);
                textObject = ConversationTools.SetTextVariables(textObject, ConversationTools.FormatNumber(loveLoss));
                imageTitle = ConversationTools.SetCharacterObjects(textObject, hero1).ToString();
                imageSound = "dramatic_chime";

                hero1.ChangeRelationTo(hero2, 0, loveLoss, true);
            }
            else if (context == ImageContext.Neglection)
            {
                int loveLoss = (hero1.GetRelationTo(Hero.MainHero).Love / 2) * -1;
                TextObject textObject = new TextObject(DramalordTexts.BANNER_NEGLECTION);
                textObject = ConversationTools.SetTextVariables(textObject, ConversationTools.FormatNumber(loveLoss));
                imageTitle = ConversationTools.SetCharacterObjects(textObject, hero1).ToString();
                imageSound = "dramatic_chime";

                hero1.ChangeRelationTo(hero2, 0, loveLoss, true);
            }

            imageTitle = Regex.Replace(imageTitle, @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");

            //ImageNotificationHelper.Show(imagePath, imageTitle);
            SimpleImagePopupHelper.ShowImage(imagePath, imageTitle, imageSound);
        }
    }
}
