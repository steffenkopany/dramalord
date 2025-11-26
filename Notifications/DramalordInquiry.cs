using Dramalord.Conversations;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Notifications
{
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
    }
}
