using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Dramalord.Notifications
{
    public static class DramalordBanner
    {
        public static void CreateBanner(Hero actor1, string bannerText, bool useSound = false) => CreateBanner(actor1, new TextObject(bannerText), useSound);
        public static void CreateBanner(Hero actor1, string bannerText, string varText, bool useSound = false) => CreateBanner(actor1, new TextObject(bannerText), new TextObject(varText), useSound);

        public static void CreateBanner(Hero actor1, TextObject bannerText, bool useSound = false)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject, bannerText);
            if(useSound)
            {
                MBInformationManager.AddQuickInformation(bannerText, 0, actor1.CharacterObject, soundEventPath: "event:/ui/notification/relation");
            }
            else
            {
                MBInformationManager.AddQuickInformation(bannerText, 0, actor1.CharacterObject);
            }
        }

        public static void CreateBanner(Hero actor1, TextObject bannerText, TextObject text1, bool useSound = false)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject, bannerText);
            bannerText.SetTextVariable("TEXT1", text1);
            if (useSound)
            {
                MBInformationManager.AddQuickInformation(bannerText, 0, actor1.CharacterObject, soundEventPath: "event:/ui/notification/relation");
            }
            else
            {
                MBInformationManager.AddQuickInformation(bannerText, 0, actor1.CharacterObject);
            }
        }

        public static void CreateBanner(Hero actor1, TextObject bannerText, TextObject text1, TextObject text2, bool useSound = false)
        {
            StringHelpers.SetCharacterProperties("ACTOR1", actor1.CharacterObject, bannerText);
            bannerText.SetTextVariable("TEXT1", text1);
            bannerText.SetTextVariable("TEXT2", text2);
            if (useSound)
            {
                MBInformationManager.AddQuickInformation(bannerText, 0, actor1.CharacterObject, soundEventPath: "event:/ui/notification/relation");
            }
            else
            {
                MBInformationManager.AddQuickInformation(bannerText, 0, actor1.CharacterObject);
            }
        }
    }
}
