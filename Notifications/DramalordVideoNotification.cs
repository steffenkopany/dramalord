using Dramalord.UI;
using Dramalord.Video;
using System.Text.RegularExpressions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

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
            Birth,
            Threesome,
            Itch,
            None
        }

        public enum VideoSound
        {
            RomanticChime,
            WeddingChime,
            DramaticChime,
            None
        }
        public enum VideoLocation
        {
            Castle,
            Tent,
            Ship,
            None
        }

        public static void ShowDramalordVideoNotification(TextObject text, VideoContext context, VideoSound sound, VideoLocation location)
        {
            string videoPath = VideoFileRegister.Instance.GetVideoFile(
                    scene: context,
                    location: location
                );

            string soundFile = string.Empty;
            if (sound == VideoSound.RomanticChime)
            {
                soundFile = "romantic_chime";
            }
            else if (sound == VideoSound.WeddingChime)
            {
                soundFile = "wedding_chime";
            }
            else if (sound == VideoSound.DramaticChime)
            {
                soundFile = "dramatic_chime";
            }
            string videoTitle = Regex.Replace(text.ToString(), @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");
            SimpleImagePopupHelper.ShowH264Video(
                        zipPath: videoPath,
                        frameRate: 24,
                        loop: false,
                        title: videoTitle,
                        soundPath: soundFile
                    );
        }

        public static void ShowDramalordVideoNotification(Hero hero1, TextObject text, VideoContext context, VideoSound sound)
        {
            string videoPath = VideoFileRegister.Instance.GetVideoFile(
                    scene: context,
                    location: hero1.CurrentSettlement != null ? VideoLocation.Castle : (hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea) ? VideoLocation.Ship : VideoLocation.Tent,
                    hero1: hero1
                );

            string soundFile = string.Empty;
            if(sound == VideoSound.RomanticChime)
            {
                soundFile = "romantic_chime";
            }
            else if(sound == VideoSound.WeddingChime)
            {
                soundFile = "wedding_chime";
            }
            else if(sound == VideoSound.DramaticChime)
            {
                soundFile = "dramatic_chime";
            }
            string videoTitle = Regex.Replace(text.ToString(), @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");
            SimpleImagePopupHelper.ShowH264Video(
                        zipPath: videoPath,
                        frameRate: 24,
                        loop: false,
                        title: videoTitle,
                        soundPath: soundFile
                    );
        }

        public static void ShowDramalordVideoNotification(Hero hero1, Hero hero2, TextObject text, VideoContext context, VideoSound sound)
        {
            string videoPath = VideoFileRegister.Instance.GetVideoFile(
                    scene: context,
                    location: hero1.CurrentSettlement != null ? VideoLocation.Castle : (hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea) ? VideoLocation.Ship : VideoLocation.Tent,
                    hero1: hero1,
                    hero2: hero2
                );

            string soundFile = string.Empty;
            if (sound == VideoSound.RomanticChime)
            {
                soundFile = "romantic_chime";
            }
            else if (sound == VideoSound.WeddingChime)
            {
                soundFile = "wedding_chime";
            }
            else if (sound == VideoSound.DramaticChime)
            {
                soundFile = "dramatic_chime";
            }

            string videoTitle = Regex.Replace(text.ToString(), @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");

            SimpleImagePopupHelper.ShowH264Video(
                        zipPath: videoPath,
                        frameRate: 24,
                        loop: false,
                        title: videoTitle,
                        soundPath: soundFile
                    );
        }

        public static void ShowDramalordVideoNotification(Hero hero1, Hero hero2, Hero hero3, TextObject text, VideoContext context, VideoSound sound)
        {
            string videoPath = VideoFileRegister.Instance.GetVideoFile(
                    scene: context,
                    location: hero1.CurrentSettlement != null ? VideoLocation.Castle : (hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea) ? VideoLocation.Ship : VideoLocation.Tent,
                    hero1: hero1,
                    hero2: hero2,
                    hero3: hero3
                );

            string soundFile = string.Empty;
            if (sound == VideoSound.RomanticChime)
            {
                soundFile = "romantic_chime";
            }
            else if (sound == VideoSound.WeddingChime)
            {
                soundFile = "wedding_chime";
            }
            else if (sound == VideoSound.DramaticChime)
            {
                soundFile = "dramatic_chime";
            }

            string videoTitle = Regex.Replace(text.ToString(), @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");

            SimpleImagePopupHelper.ShowH264Video(
                        zipPath: videoPath,
                        frameRate: 24,
                        loop: false,
                        title: videoTitle,
                        soundPath: soundFile
                    );
        }

        /*
        public static void ShowDramalordVideoNotification(Hero hero1, Hero hero2, Hero hero3, VideoContext context = VideoContext.Lovers)
        {
            string videoFile = string.Empty;

            if (context == VideoContext.Lovers)
            {
                videoFile = "lover";
            }
            else if (context == VideoContext.Intercourse)
            {
                videoFile = "sex";
            }
            else if (context == VideoContext.Wedding)
            {
                videoFile = "wedding";
            }
            else if (context == VideoContext.Divorce)
            {
                videoFile = "divorce";
            }
            else if (context == VideoContext.PrisonSex)
            {
                videoFile = "prisonsex";
            }
            else if (context == VideoContext.Birth)
            {
                videoFile = "birth";
            }
            else if (context == VideoContext.Threesome)
            {
                videoFile = "threesome";
            }

            if (hero1.CurrentSettlement != null)
            {
                videoFile += "_castle";
            }
            else
            {
                if (hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea)
                {
                    videoFile += "_ship";
                }
                else
                {
                    videoFile += "_tent";
                }
            }

            if (context != VideoContext.Birth && context != VideoContext.Threesome)
            {
                if (hero1.IsFemale != hero2.IsFemale)
                {
                    videoFile += "_m_f";
                }
                else if (hero1.IsFemale)
                {
                    videoFile += "_f_f";
                }
                else
                {
                    videoFile += "_m_m";
                }
            }
            else if(context != VideoContext.Birth && context == VideoContext.Threesome) 
            {
                if(!Hero.MainHero.IsFemale)
                {
                    videoFile += "_m";
                }
                else
                {
                    videoFile += "_f";
                }

                if (hero2.IsFemale && hero2.IsFemale)
                {
                    videoFile += "_f_f";
                }
                else if (!hero1.IsFemale && !hero2.IsFemale)
                {
                    videoFile += "_m_m";
                }
                else
                {
                    videoFile += "_m_f";
                }
            }
            if(context == VideoContext.Intercourse)
            {
                if (hero1.IsFemale != hero2.IsFemale)
                {
                    int variation = (MBRandom.RandomInt() % 5) + 1;
                    videoFile += "_" + variation.ToString();
                }
                else if(hero1.IsFemale)
                {
                    int variation = (MBRandom.RandomInt() % 3) + 1;
                    videoFile += "_" + variation.ToString();
                }
                else
                {
                    videoFile += "_1";
                }
            }
            else
            {
                videoFile += "_1";
            }

            //videoFile += ".h264";

            //string basePath = ModuleHelper.GetModuleFullPath("Dramalord");
            //string videoPath = System.IO.Path.Combine(basePath, "GUI", "Videos", videoFile);
            string videoPath = VideoFileRegister.Instance.GetVideoFile(
                scene: context,
                location: hero1.CurrentSettlement != null ? VideoFileRegister.Location.Castle : (hero1.PartyBelongedTo != null && hero1.PartyBelongedTo.IsCurrentlyAtSea) ? VideoFileRegister.Location.Ship : VideoFileRegister.Location.Tent,
                hero1: hero1,
                hero2: hero2,
                hero3: hero3
                );

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
                ConversationTools.SetTextVariables(txt, DramalordTexts.GetRelationshipStatusName(RelationshipType.Spouse));

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
            else if (context == VideoContext.Threesome)
            {
                TextObject txt = ConversationTools.SetCharacterObjects(new(DramalordTexts.LOG_THREESOME), hero1, hero2, hero3);

                videoTitle = txt.ToString();
                videoSound = "romantic_chime";
            }

            videoTitle = Regex.Replace(videoTitle, @"<[^>]+>", "").Replace("&nbsp;", "").Replace("\t", "");

            SimpleImagePopupHelper.ShowH264Video(
                        zipPath: videoPath,
                        frameRate: 24,
                        loop: false,
                        title: videoTitle,
                        soundPath: videoSound
                    );
       
        }
        */
    }
}
