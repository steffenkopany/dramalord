using Dramalord.Conversations;
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
            Adoption
        }

        public static void ShowDramalordImageNotification(Hero hero1, Hero hero2, Hero? hero3 = null, ImageContext context = ImageContext.Adoption)
        {
            string imageFile = "notification_";

            if (context == ImageContext.Adoption)
            {
                imageFile += "adopt_";
            }

            if(hero3 == null)
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
            else
            {
                if (hero1.IsFemale != hero3.IsFemale)
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
                    imageTitle = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_ADOPT_2), hero1, hero2, hero3).ToString();
                }
                else
                {
                    imageTitle = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_ADOPT), hero1, hero2).ToString();
                }

                imageSound = "wedding_chime";
            }

            imageTitle = Regex.Replace(imageTitle, @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");

            //ImageNotificationHelper.Show(imagePath, imageTitle);
            SimpleImagePopupHelper.ShowImage(imagePath, imageTitle, imageSound);
        }
    }
}
