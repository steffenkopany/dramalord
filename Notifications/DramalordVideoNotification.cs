using Dramalord.Conversations;
using Dramalord.UI;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.Notifications
{
    internal class DramalordVideoNotification
    {
        public enum VideoContext
        {
            Lovers,
            Intercourse
        }

        public static void ShowDramalordVideoNotification(Hero hero1, Hero hero2, VideoContext context)
        {
            string videoFolder = string.Empty;

            if(context == VideoContext.Lovers)
            {
                videoFolder = "lovers_";
            }
            else if(context == VideoContext.Intercourse)
            {
                videoFolder = "sex_";
            }

            if(hero1.CurrentSettlement != null)
            {
                if(hero1.CurrentSettlement.Culture.StringId == "sturgia" || hero1.CurrentSettlement.Culture.StringId == "battania")
                {
                    videoFolder += "ice_";
                }
                else if (hero1.CurrentSettlement.Culture.StringId == "aserai" || hero1.CurrentSettlement.Culture.StringId == "khuzait")
                {
                    videoFolder += "desert_";
                }
                else
                {
                    videoFolder += "green_";
                }
            }
            else
            {
                if(hero1.PartyBelongedTo != null && !hero1.PartyBelongedTo.Ships.IsEmpty())
                {
                    videoFolder += "ship_";
                }
                else
                {
                    videoFolder += "tent_";
                }
            }

            if (hero1.IsFemale != hero2.IsFemale)
            {
                videoFolder += "m_f";
                if(context == VideoContext.Intercourse)
                {
                    int random = MBRandom.RandomInt(1, 2);
                    videoFolder += "_" + random.ToString();
                }
            }
            else if(hero1.IsFemale)
            {
                videoFolder += "f_f";
            }
            else
            {
                videoFolder += "m_m";
            }

            string videoPath = System.IO.Path.Combine(BasePath.Name, "Modules", "Dramalord", "GUI", "VideoFrames", videoFolder);

            string videoTitle = string.Empty;
            string videoSound = string.Empty;

            if (context == VideoContext.Lovers)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_RElATIONSHIP_START), hero1, hero2);
                ConversationTools.SetTextVariables(txt, DramalordTexts.RELATIONSHIP_LOVERS);

                videoTitle = txt.ToString();
                videoSound = "romance_music";
            }
            else if(context == VideoContext.Intercourse)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_SEX_GOOD), hero1, hero2);

                videoTitle = txt.ToString();
                videoSound = "romance_music";
            }

            videoTitle = Regex.Replace(videoTitle, @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");

            SimpleImagePopupHelper.ShowVideo(
                        frameDirectory: videoPath,
                        framePrefix: "frame",
                        frameExtension: ".jpg",
                        frameRate: 12,
                        loop: false,
                        title: videoTitle,
                        soundEvent: videoSound
                    );
        }
    }
}
