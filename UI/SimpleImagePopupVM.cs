using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.UI
{
    public class SimpleImagePopupVM : ViewModel
    {
        private readonly Action _onClose;
        private string _imagePath;
        private string _title;
        private string _closeButtonText;

        public event Action? OnVideoFinished;

        public SimpleImagePopupVM(Action onClose, string imagePath, string title)
        {
            _onClose = onClose;
            _imagePath = imagePath;
            _title = title ?? new TextObject("{=viFv9Fnb}Image Viewer").ToString();
            _closeButtonText = new TextObject("{=yQtzabbe}Close").ToString();
        }

        public void Tick(float dt)
        {
            if (VideoCache.IsLoaded && VideoCache.IsPlaying)
            {
                // Advance the video cache
                VideoCache.Tick(dt);

                // Check if finished
                if (VideoCache.IsFinished)
                {
                    OnVideoFinished?.Invoke();
                }
            }
        }

        /// <summary>
        /// Start video playback from VideoCache
        /// </summary>
        public void Play()
        {
            VideoCache.Play();
        }

        /// <summary>
        /// Pause video playback
        /// </summary>
        public void Pause()
        {
            VideoCache.Pause();
        }

        /// <summary>
        /// Stop video playback
        /// </summary>
        public void Stop()
        {
            VideoCache.Stop();
        }

        public bool IsPlaying => VideoCache.IsPlaying;
        public int CurrentFrame => VideoCache.CurrentFrameIndex;
        public int TotalFrames => VideoCache.FrameCount;

        [DataSourceProperty]
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChangedWithValue(value, "ImagePath");
                }
            }
        }

        [DataSourceProperty]
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChangedWithValue(value, "Title");
                }
            }
        }

        [DataSourceProperty]
        public string CloseButtonText
        {
            get => _closeButtonText;
            set
            {
                if (_closeButtonText != value)
                {
                    _closeButtonText = value;
                    OnPropertyChangedWithValue(value, "CloseButtonText");
                }
            }
        }

        public void ExecuteClose()
        {
            _onClose?.Invoke();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
        }
    }
}
