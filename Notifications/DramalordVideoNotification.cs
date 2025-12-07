using Dramalord.Conversations;
using Dramalord.Data;
using Dramalord.Extensions;
using Dramalord.UI;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;

namespace Dramalord.Notifications
{
    internal class DramalordVideoNotification
    {
        public enum VideoContext
        {
            Lovers,
            Intercourse,
            Wedding,
            Divorce,
            PrisonSex,
            Birth
        }

        public static void ShowDramalordVideoNotification(Hero hero1, Hero hero2, Hero? hero3 = null, VideoContext context = VideoContext.Lovers)
        {
            string videoFile = string.Empty;

            if (context == VideoContext.Lovers)
            {
                videoFile = "lovers_";
            }
            else if (context == VideoContext.Intercourse)
            {
                videoFile = "sex_";
            }
            else if (context == VideoContext.Wedding)
            {
                videoFile = "wedding_";
            }
            else if (context == VideoContext.Divorce)
            {
                videoFile = "divorce_";
            }
            else if (context == VideoContext.PrisonSex)
            {
                videoFile = "prisonsex_";
            }
            else if (context == VideoContext.Birth)
            {
                videoFile = "birth_";
            }

            if (hero1.CurrentSettlement != null)
            {
                if (context != VideoContext.PrisonSex && context != VideoContext.Birth)
                {
                    if (hero1.CurrentSettlement.Culture.StringId == "sturgia" || hero1.CurrentSettlement.Culture.StringId == "nord")
                    {
                        videoFile += "ice_";
                    }
                    else if (hero1.CurrentSettlement.Culture.StringId == "aserai" || hero1.CurrentSettlement.Culture.StringId == "khuzait")
                    {
                        videoFile += "desert_";
                    }
                    else
                    {
                        videoFile += "green_";
                    }
                }
                else
                {
                    videoFile += "green_";
                }
            }
            else
            {
                if (hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea && (context == VideoContext.Intercourse || context == VideoContext.Lovers))
                {
                    videoFile += "ship_";
                }
                else
                {
                    videoFile += "tent_";
                }
            }

            if (context != VideoContext.Birth)
            {
                if (hero1.IsFemale != hero2.IsFemale)
                {
                    videoFile += "m_f";
                    if (context == VideoContext.Intercourse)
                    {
                        int random = (MBRandom.RandomInt() % 2) + 1;
                        videoFile += "_" + random.ToString();
                    }
                }
                else if (hero1.IsFemale)
                {
                    videoFile += "f_f";
                }
                else
                {
                    videoFile += "m_m";
                }
            }
            else
            {
                if (hero2.Father.IsCloseTo(hero1))
                {
                    videoFile += "m_f";
                }
                else
                {
                    videoFile += "f_f";

                }
            }

            videoFile += ".dat";

            string basePath = ModuleHelper.GetModuleFullPath("Dramalord");
            string videoPath = System.IO.Path.Combine(basePath, "GUI", "Videos", videoFile);

            string videoTitle = string.Empty;
            string videoSound = string.Empty;

            if (context == VideoContext.Lovers)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_RElATIONSHIP_START), hero1, hero2);
                ConversationTools.SetTextVariables(txt, DramalordTexts.RELATIONSHIP_LOVERS);

                videoTitle = txt.ToString();
                videoSound = "romantic_chime";
            }
            else if (context == VideoContext.Intercourse)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_SEX_GOOD), hero1, hero2);

                videoTitle = txt.ToString();
                videoSound = "romantic_chime";
            }
            else if (context == VideoContext.Wedding)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new TextObject(DramalordTexts.LOG_MARRIAGE), hero1, hero2);

                videoTitle = txt.ToString();
                videoSound = "wedding_chime";
            }
            else if (context == VideoContext.Divorce)
            {
                HeroRelation relation = hero1.GetRelationTo(hero2);
                TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_RElATIONSHIP_END), hero1, hero2);
                ConversationTools.SetTextVariables(txt, DramalordTexts.GetRelationshipStatusName(relation.Relationship));

                videoTitle = txt.ToString();
                videoSound = "dramatic_chime";
            }
            else if (context == VideoContext.PrisonSex)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_SEX_PRISON), hero1, hero2);

                videoTitle = txt.ToString();
                videoSound = "dramatic_chime";
            }
            else if (context == VideoContext.Birth)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_BIRTH), hero1, hero2, hero3);

                videoTitle = txt.ToString();
                videoSound = "wedding_chime";
            }

            videoTitle = Regex.Replace(videoTitle, @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");

            SimpleImagePopupHelper.ShowVideo(
                        zipPath: videoPath,
                        frameRate: 12,
                        loop: false,
                        title: videoTitle,
                        soundPath: videoSound
                    );
       
        }
    }
}
