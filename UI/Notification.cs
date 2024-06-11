using Dramalord.Behaviors;
using Dramalord.Data;
using Dramalord.Data.Deprecated;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting;

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

        internal static void PrintNpcStats()
        {
            if (Hero.OneToOneConversationHero != null && Hero.MainHero != null)
            {
                
                PrintText("Attraction: " + Hero.OneToOneConversationHero.GetDramalordAttractionTo(Hero.MainHero));
                PrintText("Emotion: " + Hero.MainHero.GetDramalordFeelings(Hero.OneToOneConversationHero).Emotion);
                PrintText("Trait score: " + Hero.OneToOneConversationHero.GetDramalordTraitScore(Hero.MainHero));
                PrintText("Horny: " + Hero.OneToOneConversationHero.GetDramalordTraits().Horny);
                if(Hero.OneToOneConversationHero.Spouse != null && Hero.OneToOneConversationHero.Spouse.IsDramalordLegit())
                {
                    PrintText("Married: " + Hero.OneToOneConversationHero.Spouse);
                    PrintText(Hero.OneToOneConversationHero.Spouse + " attraction: " + Hero.OneToOneConversationHero.GetDramalordAttractionTo(Hero.OneToOneConversationHero.Spouse));
                    PrintText(Hero.OneToOneConversationHero.Spouse + " emotion: " + Hero.OneToOneConversationHero.GetDramalordFeelings(Hero.OneToOneConversationHero.Spouse).Emotion);
                }
                else
                {
                    PrintText("Single " + Hero.OneToOneConversationHero.Spouse);
                }
            }
        }

        internal static void PrintPlayerAttraction()
        {
            PrintText("Attracted to player: " + Hero.OneToOneConversationHero.GetDramalordAttractionTo(Hero.MainHero));
        }
    }
}

