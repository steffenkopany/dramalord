using Dramalord.Behaviors;
using Dramalord.Data;
using System;
using System.Collections.Generic;
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

        /// shortcuts

        internal static void DebugPrint(string txt)
        {
            if (DramalordMCM.Get.DebugOutput)
            {
                PrintText(txt);
            };
        }

        internal static void DebugPrint(TextObject txt)
        {
            if (DramalordMCM.Get.DebugOutput)
            {
                PrintText(txt);
            };
        }

        internal static void PrintNpcStats()
        {
            if (Hero.OneToOneConversationHero != null && Hero.MainHero != null)
            {
                PrintText("Attraction: " + Info.GetAttractionToHero(Hero.OneToOneConversationHero, Hero.MainHero));
                PrintText("Emotion: " + Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.MainHero));
                PrintText("Trait score: " + Info.GetTraitscoreToHero(Hero.OneToOneConversationHero, Hero.MainHero));
                PrintText("Horny: " + Info.GetHeroHorny(Hero.OneToOneConversationHero));
                if(Hero.OneToOneConversationHero.Spouse != null)
                {
                    PrintText("Married: " + Hero.OneToOneConversationHero.Spouse);
                    PrintText(Hero.OneToOneConversationHero.Spouse + " attraction: " + Info.GetAttractionToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.Spouse));
                    PrintText(Hero.OneToOneConversationHero.Spouse + " emotion: " + Info.GetEmotionToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.Spouse));
                }
                else
                {
                    PrintText("Single " + Hero.OneToOneConversationHero.Spouse);
                }
            }
        }

        internal static void PrintTopFlirtTargets()
        {
            List<Hero> list = AICampaignHelper.ScopeSurroundingsForFlirts(Hero.OneToOneConversationHero, true);
            foreach (Hero hero in list)
            {
                PrintText(hero.Name + ": " + Info.GetAttractionToHero(Hero.OneToOneConversationHero, hero));
            }
        }
    }
}

