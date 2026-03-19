using Dramalord.Conversations;
using Dramalord.UI;
using System;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;

namespace Dramalord.Notifications
{
    public enum InquiryContext
    {
        AcceptSex,
        Orphanize,
        ConfrontDate,
        ConfrontSex,
        ConfrontBirth,
        ConfrontMarriage,
        MarriagePermission,
        Threesome
    }

    public static class DramalordInquiry
    {
        public static void CreateYesNoInquiry(Hero actor1, string title, string text, Action yesAction, Action noAction) => CreateYesNoInquiry(actor1, new TextObject(title), new TextObject(text), yesAction, noAction);

        public static void CreateYesNoInquiry(Hero actor1, TextObject title, TextObject text, Action yesAction, Action noAction)
        {
            if(InformationManager.IsAnyInquiryActive())
            {
                return;
            }

            ConversationTools.SetCharacterObjects(text, actor1);

            InformationManager.ShowInquiry(
                    new InquiryData(
                        title.ToString(),
                        text.ToString(),
                        true,
                        true,
                        GameTexts.FindText("str_yes").ToString(),
                        GameTexts.FindText("str_no").ToString(),
                        () => { yesAction.Invoke(); /*Campaign.Current.SetTimeSpeed(speed);*/ },
                        () => { noAction.Invoke(); /*Campaign.Current.SetTimeSpeed(speed);*/ }
                        ), 
                    true,
                    true);
        }

        public static void CreateYesNoInquiry(Hero actor1, Hero actor2, TextObject title, TextObject text, Action yesAction, Action noAction)
        {
            if (InformationManager.IsAnyInquiryActive())
            {
                return;
            }

            ConversationTools.SetCharacterObjects(text, actor1, actor2);

            InformationManager.ShowInquiry(
                    new InquiryData(
                        title.ToString(),
                        text.ToString(),
                        true,
                        true,
                        GameTexts.FindText("str_yes").ToString(),
                        GameTexts.FindText("str_no").ToString(),
                        () => { yesAction.Invoke(); /*Campaign.Current.SetTimeSpeed(speed);*/ },
                        () => { noAction.Invoke(); /*Campaign.Current.SetTimeSpeed(speed);*/ }
                        ),
                    true,
                    true);
        }

        

        public static void CreateYesNoImageInquiry(Hero actor1, Hero actor2, TextObject text, Action yesAction, Action noAction, InquiryContext context)
        {
            //ConversationTools.SetCharacterObjects(text, actor2);

            string basePath = ModuleHelper.GetModuleFullPath("Dramalord");
            string imagePath = System.IO.Path.Combine(basePath, "GUI", "Images", "inquiry");
            string fileName = "inquiry_";
            if (context == InquiryContext.AcceptSex)
            {
                fileName += "acceptsex_";
            }
            else if(context == InquiryContext.Orphanize)
            {
                fileName += "orphanize_";
            }
            else if(context == InquiryContext.ConfrontDate)
            {
                fileName += "confrontdate_";
            }
            else if (context == InquiryContext.ConfrontSex)
            {
                fileName += "confrontsex_";
            }
            else if (context == InquiryContext.ConfrontBirth)
            {
                fileName += "confrontbirth_";
            }
            else if (context == InquiryContext.ConfrontMarriage)
            {
                fileName += "confrontmarriage_";
            }
            else if (context == InquiryContext.MarriagePermission)
            {
                fileName += "marriagepermission_";
            }
            else if (context == InquiryContext.Threesome)
            {
                fileName += "threesome_";
            }

            if(context != InquiryContext.Orphanize && context != InquiryContext.ConfrontMarriage)
            {
                if (actor1.CurrentSettlement != null)
                {
                    fileName += "castle_";
                }
                else if (actor1.PartyBelongedTo != null && actor1.PartyBelongedTo.IsCurrentlyAtSea)
                {
                    fileName += "ship_";
                }
                else
                {
                    fileName += "tent_";
                }

                if (Hero.MainHero.IsFemale)
                {
                    fileName += "f_";
                }
                else
                {
                    fileName += "m_";
                }
            }
            else if(context == InquiryContext.ConfrontMarriage)
            {
                fileName += "castle_";
                if (Hero.MainHero.IsFemale)
                {
                    fileName += "f_";
                }
                else
                {
                    fileName += "m_";
                }
            }
            

            if (context != InquiryContext.AcceptSex && context != InquiryContext.Orphanize)
            {
                if (actor1.IsFemale && actor2.IsFemale)
                {
                    fileName += "f_f";
                }
                else if (!actor1.IsFemale && !actor2.IsFemale)
                {
                    fileName += "m_m";
                }
                else
                {
                    fileName += "m_f";
                }
            }
            else if (context != InquiryContext.Orphanize)
            {
                if(actor2.IsFemale)
                {
                    fileName += "f";
                }
                else
                {
                    fileName += "m";
                }
            }
            else
            {
                fileName += "f";
            }
            

            fileName += ".jpg";

            imagePath = System.IO.Path.Combine(imagePath, fileName);
            string inquiryText = Regex.Replace(text.ToString(), @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");
            ImageInquiryHelper.Show(imagePath, inquiryText, yesAction, noAction);
        }

        public static void CreateCustomImageInquiry(Hero actor1, Hero actor2, TextObject text, TextObject action1name, TextObject action2name, Action action1, Action action2, InquiryContext context)
        {
            //ConversationTools.SetCharacterObjects(text, actor2);

            string basePath = ModuleHelper.GetModuleFullPath("Dramalord");
            string imagePath = System.IO.Path.Combine(basePath, "GUI", "Images", "inquiry");
            string fileName = "inquiry_";
            if (context == InquiryContext.AcceptSex)
            {
                fileName += "acceptsex_";
            }
            else if (context == InquiryContext.Orphanize)
            {
                fileName += "orphanize_";
            }
            else if (context == InquiryContext.ConfrontDate)
            {
                fileName += "confrontdate_";
            }
            else if (context == InquiryContext.ConfrontSex)
            {
                fileName += "confrontsex_";
            }
            else if (context == InquiryContext.ConfrontBirth)
            {
                fileName += "confrontbirth_";
            }
            else if (context == InquiryContext.ConfrontMarriage)
            {
                fileName += "confrontmarriage_";
            }
            else if (context == InquiryContext.MarriagePermission)
            {
                fileName += "marriagepermission_";
            }
            else if (context == InquiryContext.Threesome)
            {
                fileName += "threesome_";
            }

            if (context != InquiryContext.Orphanize && context != InquiryContext.ConfrontMarriage)
            {
                if (actor1.CurrentSettlement != null)
                {
                    fileName += "castle_";
                }
                else if (actor1.PartyBelongedTo != null && actor1.PartyBelongedTo.IsCurrentlyAtSea)
                {
                    fileName += "ship_";
                }
                else
                {
                    fileName += "tent_";
                }

                if (Hero.MainHero.IsFemale)
                {
                    fileName += "f_";
                }
                else
                {
                    fileName += "m_";
                }
            }
            else if (context == InquiryContext.ConfrontMarriage)
            {
                fileName += "castle_";
                if (Hero.MainHero.IsFemale)
                {
                    fileName += "f_";
                }
                else
                {
                    fileName += "m_";
                }
            }


            if (context != InquiryContext.AcceptSex)
            {
                if (actor1.IsFemale && actor2.IsFemale)
                {
                    fileName += "f_f";
                }
                else if (!actor1.IsFemale && !actor2.IsFemale)
                {
                    fileName += "m_m";
                }
                else
                {
                    fileName += "m_f";
                }
            }
            else if (context != InquiryContext.Orphanize)
            {
                if (actor2.IsFemale)
                {
                    fileName += "f";
                }
                else
                {
                    fileName += "m";
                }
            }
            else
            {
                fileName += "f";
            }

            fileName += ".jpg";

            imagePath = System.IO.Path.Combine(imagePath, fileName);
            string inquiryText = Regex.Replace(text.ToString(), @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");
            ImageInquiryHelper.ShowCustom(imagePath, inquiryText, action1name.ToString(), action2name.ToString(), action1, action2);
        }
    }
}
