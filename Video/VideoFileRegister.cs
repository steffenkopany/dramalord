using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using static Dramalord.Notifications.DramalordVideoNotification;

namespace Dramalord.Video
{
    internal class VideoFileRegister
    {
        class VideoFileInfo
        {
            public string FilePath { get; set; }

            public VideoContext Scene { get; set; }

            public VideoLocation Location { get; set; }

            public int MaleActors { get; set; }

            public int FemaleActors { get; set; }

            public int Variations { get; set; }

            public VideoFileInfo(string filePath, VideoContext scene, VideoLocation location, int maleActors, int femaleActors, int variations)
            {
                FilePath = filePath;
                Scene = scene;
                Location = location;
                MaleActors = maleActors;
                FemaleActors = femaleActors;
                Variations = variations;
            }

            public override bool Equals(object o)
            {
                if (!(o is VideoFileInfo))
                {
                    return false;
                }

                VideoFileInfo info = o as VideoFileInfo;


                return info.Scene == Scene && info.Location == Location && info.MaleActors == MaleActors && info.FemaleActors == FemaleActors;
            }
        }

        private readonly List<VideoFileInfo> _videoFiles = new();

        private static VideoFileRegister? _instance = null;

        public static VideoFileRegister Instance => _instance ??= new VideoFileRegister();

        private VideoFileRegister()
        {
            _videoFiles.Clear();
            string videoDirectory = System.IO.Path.Combine(ModuleHelper.GetModuleFullPath("Dramalord"), "GUI", "Videos");

            Directory.GetFiles(videoDirectory, "*.h264").ToList().ForEach(filePath =>
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                string[] parts = fileName.Split('_');

                if (parts.Length >= 3)
                {
                    VideoFileInfo videoFileInfo = new VideoFileInfo(filePath, VideoContext.None, VideoLocation.None, 0, 0, 0);

                    if (parts[0].Equals("birth"))
                    {
                        videoFileInfo.Scene = VideoContext.Birth;
                    }
                    else if (parts[0].Equals("divorce"))
                    {
                        videoFileInfo.Scene = VideoContext.Divorce;
                    }
                    else if (parts[0].Equals("lover"))
                    {
                        videoFileInfo.Scene = VideoContext.Lovers;
                    }
                    else if (parts[0].Equals("prisonsex"))
                    {
                        videoFileInfo.Scene = VideoContext.PrisonSex;
                    }
                    else if (parts[0].Equals("sex"))
                    {
                        videoFileInfo.Scene = VideoContext.Intercourse;
                    }
                    else if (parts[0].Equals("threesome"))
                    {
                        videoFileInfo.Scene = VideoContext.Threesome;
                    }
                    else if (parts[0].Equals("wedding"))
                    {
                        videoFileInfo.Scene = VideoContext.Wedding;
                    }
                    else if (parts[0].Equals("itch"))
                    {
                        videoFileInfo.Scene = VideoContext.Itch;
                    }

                    if (parts[1].Equals("castle"))
                    {
                        videoFileInfo.Location = VideoLocation.Castle;
                    }
                    else if (parts[1].Equals("tent"))
                    {
                        videoFileInfo.Location = VideoLocation.Tent;
                    }
                    else if (parts[1].Equals("ship"))
                    {
                        videoFileInfo.Location = VideoLocation.Ship;
                    }
                    else
                    {
                        videoFileInfo.Location = VideoLocation.None;
                    }

                    for (int i = 2; i < parts.Length; i++)
                    {
                        if (parts[i].Equals("m"))
                        {
                            videoFileInfo.MaleActors++;
                        }
                        else if (parts[i].Equals("f"))
                        {
                            videoFileInfo.FemaleActors++;
                        }
                    }

                    videoFileInfo.Variations = 1;

                    if (!_videoFiles.Contains(videoFileInfo))
                    {
                        _videoFiles.Add(videoFileInfo);
                    }
                    else
                    {
                        VideoFileInfo existingInfo = _videoFiles.Find(v => v.Equals(videoFileInfo));
                        existingInfo.Variations += 1;
                    }
                }
            });
        }

        public int Count()
        {
            int count = 0;
            _videoFiles.ForEach(v => count += v.Variations);
            return count; 
        }

        public string GetVideoFile(VideoContext scene, VideoLocation location)
        {
            VideoFileInfo searchInfo = new VideoFileInfo(string.Empty, scene, location, 0, 0, 0);
            VideoFileInfo? foundInfo = _videoFiles.Find(v => v.Equals(searchInfo));
            if(foundInfo != null)
            {
                if(foundInfo.Variations > 1)
                {
                    int variationIndex = MBRandom.RandomInt() % foundInfo.Variations + 1;
                    int idx = foundInfo.FilePath.LastIndexOf('.');
                    if (idx > 0)
                    {
                        StringBuilder strBld = new StringBuilder(foundInfo.FilePath);
                        strBld[idx - 1] = variationIndex.ToString().ToCharArray()[0];
                        return strBld.ToString();
                    }
                }
            }
            return foundInfo?.FilePath ?? string.Empty;
        }

        public string GetVideoFile(VideoContext scene, VideoLocation location, Hero hero1)
        {
            int maleActors = 0; 
            int femaleActors = 0;
            if (hero1.IsFemale)
            {
                femaleActors++;
            }
            else
            {
                maleActors++;
            }
            VideoFileInfo searchInfo = new VideoFileInfo(string.Empty, scene, location, maleActors, femaleActors, 0);
            VideoFileInfo? foundInfo = _videoFiles.Find(v => v.Equals(searchInfo));
            if (foundInfo != null)
            {
                if (foundInfo.Variations > 1)
                {
                    int variationIndex = MBRandom.RandomInt() % foundInfo.Variations + 1;
                    int idx = foundInfo.FilePath.LastIndexOf('.');
                    if (idx > 0)
                    {
                        StringBuilder strBld = new StringBuilder(foundInfo.FilePath);
                        strBld[idx - 1] = variationIndex.ToString().ToCharArray()[0];
                        return strBld.ToString();
                    }
                }
            }
            return foundInfo?.FilePath ?? string.Empty;
        }

        public string GetVideoFile(VideoContext scene, VideoLocation location, Hero hero1, Hero hero2)
        {
            int maleActors = 0;
            int femaleActors = 0;
            if (hero1.IsFemale)
            {
                femaleActors++;
            }
            else
            {
                maleActors++;
            }

            if (hero2.IsFemale)
            {
                femaleActors++;
            }
            else
            {
                maleActors++;
            }

            VideoFileInfo searchInfo = new VideoFileInfo(string.Empty, scene, location, maleActors, femaleActors, 0);
            VideoFileInfo? foundInfo = _videoFiles.Find(v => v.Equals(searchInfo));
            if (foundInfo != null)
            {
                if (foundInfo.Variations > 1)
                {
                    int variationIndex = MBRandom.RandomInt() % foundInfo.Variations + 1;
                    int idx = foundInfo.FilePath.LastIndexOf('.');
                    if (idx > 0)
                    {
                        StringBuilder strBld = new StringBuilder(foundInfo.FilePath);
                        strBld[idx - 1] = variationIndex.ToString().ToCharArray()[0];
                        return strBld.ToString();
                    }
                }
            }
            return foundInfo?.FilePath ?? string.Empty;
        }

        public string GetVideoFile(VideoContext scene, VideoLocation location, Hero hero1, Hero hero2, Hero hero3)
        {
            int maleActors = 0;
            int femaleActors = 0;
            if (hero1.IsFemale)
            {
                femaleActors++;
            }
            else
            {
                maleActors++;
            }

            if (hero2.IsFemale)
            {
                femaleActors++;
            }
            else
            {
                maleActors++;
            }

            if (hero3.IsFemale)
            {
                femaleActors++;
            }
            else
            {
                maleActors++;
            }

            VideoFileInfo searchInfo = new VideoFileInfo(string.Empty, scene, location, maleActors, femaleActors, 0);
            VideoFileInfo? foundInfo = _videoFiles.Find(v => v.Equals(searchInfo));
            if (foundInfo != null)
            {
                if (foundInfo.Variations > 1)
                {
                    int variationIndex = MBRandom.RandomInt() % foundInfo.Variations + 1;
                    int idx = foundInfo.FilePath.LastIndexOf('.');
                    if (idx > 0)
                    {
                        StringBuilder strBld = new StringBuilder(foundInfo.FilePath);
                        strBld[idx - 1] = variationIndex.ToString().ToCharArray()[0];
                        return strBld.ToString();
                    }
                }
            }
            return foundInfo?.FilePath ?? string.Empty;
        }
    }
}
