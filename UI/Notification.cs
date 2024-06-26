using Dramalord.Data;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.UI
{
    internal class Notification
    {
        internal static void PrintText(string text)
        {
            InformationManager.DisplayMessage(new InformationMessage(text, new Color(1f, 0.08f, 0.58f)));
        }

        internal static void PrintText(TextObject text)
        {
            InformationManager.DisplayMessage(new InformationMessage(text.ToString(), new Color(1f, 0.08f, 0.58f)));
        }
  
        internal static void DrawMessageBox(TextObject title, TextObject text, bool withNo, Action? yesAction = null, Action? noAction = null)
        {
            InformationManager.ShowInquiry(new InquiryData(title.ToString(), text.ToString(), true, withNo, GameTexts.FindText("str_ok").ToString(), (withNo) ? GameTexts.FindText("str_no").ToString() : null, yesAction, noAction, "event:/ui/notification/relation"));
        }

        internal static void DrawBanner(string text)
        {
            MBInformationManager.AddQuickInformation(new TextObject(text), 0, null, "event:/ui/notification/relation");
        }

        internal static void DrawBanner(TextObject text)
        {
            MBInformationManager.AddQuickInformation(text, 0, null, "event:/ui/notification/relation");
        }
    }
}

