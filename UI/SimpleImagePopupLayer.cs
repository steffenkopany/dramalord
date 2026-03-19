using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace Dramalord.UI
{
    public class SimpleImagePopupLayer : GlobalLayer
    {
        private GauntletLayer _gauntletLayer;
        private SimpleImagePopupVM _viewModel;
        private SoundEvent _sound;

        private readonly string _imagePath;
        private readonly string _title;
        private readonly bool _isVideo;
        private readonly bool _autoPlay;
        private readonly bool _closeOnFinished;
        private readonly string _imageSoundPath;

        private bool _closeRequested = false;
        //private CampaignTimeControlMode _timeSpeed = CampaignTimeControlMode.Stop;

        public SimpleImagePopupLayer(string imagePath, string title = null, string imageSoundPath = null)
        {
            _imagePath = imagePath;
            _title = title;
            _isVideo = false;
            _autoPlay = false;
            _closeOnFinished = false;
            _imageSoundPath = imageSoundPath;

            Initialize();
        }

        public SimpleImagePopupLayer(string title, bool autoPlay, bool closeOnFinished)
        {
            _imagePath = "VideoCache"; // Special marker to use VideoCache
            _title = title;
            _isVideo = true;
            _autoPlay = autoPlay;
            _closeOnFinished = closeOnFinished;

            Initialize();
        }

        private void Initialize()
        {
            //_timeSpeed = Campaign.Current.TimeControlMode;
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;

            _viewModel = new SimpleImagePopupVM(RequestClose, _imagePath, _title);

            _gauntletLayer = new GauntletLayer("SimpleImagePopupLayer", 10000);
            _gauntletLayer.LoadMovie("SimpleImagePopup", _viewModel);
            _gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            _gauntletLayer.IsFocusLayer = true;

            base.Layer = _gauntletLayer;

            if (_isVideo)
            {
                _viewModel.OnVideoFinished += OnVideoFinished;

                if (_autoPlay)
                {
                    _viewModel.Play();
                }

                // Play sound if specified in VideoCache
                if (!string.IsNullOrEmpty(VideoCache.SoundPath))
                {
                    PlaySoundFromPath(VideoCache.SoundPath);
                }
            }
            else
            {
                //_viewModel.ImagePath = _imagePath;
                PlaySoundFromPath(_imageSoundPath);
            }

            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        private void OnVideoFinished()
        {
            if (!VideoCache.Loop && _closeOnFinished)
            {
                // Optionally auto-close when video finishes
                RequestClose();
            }
        }

        private void PlaySoundFromPath(string soundPath)
        {
            try
            {
                if (string.IsNullOrEmpty(soundPath))
                    return;

                string soundName = System.IO.Path.GetFileNameWithoutExtension(soundPath);
                int eventId = SoundEvent.GetEventIdFromString(soundName);

                if (eventId >= 0)
                {
                    _sound = SoundEvent.CreateEvent(eventId, ((MapScene)Campaign.Current.MapSceneWrapper).Scene);
                    if (_sound != null)
                    {
                        _sound.Play();
                        Debug.Print($"[SimpleImagePopupLayer] Playing sound: {soundName}");
                    }
                }
                else
                {
                    Debug.Print($"[SimpleImagePopupLayer] Sound event not registered: {soundName}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.Print($"[SimpleImagePopupLayer] Error playing sound: {ex.Message}");
            }
        }

        private void StopSound()
        {
            try
            {
                if (_sound != null)
                {
                    if (_sound.IsPlaying())
                    {
                        _sound.Stop();
                    }
                    _sound.Release();
                    _sound = null;
                }
            }
            catch (System.Exception ex)
            {
                Debug.Print($"[SimpleImagePopupLayer] Error stopping sound: {ex.Message}");
            }
        }

        private void RequestClose()
        {
            _closeRequested = true;
        }

        private void Close()
        {
            StopSound();
            VideoCache.Stop();

            if (_viewModel != null)
            {
                _viewModel.OnVideoFinished -= OnVideoFinished;
                _viewModel.OnFinalize();
                _viewModel = null;
            }

            VideoCache.Clear();
            ScreenManager.RemoveGlobalLayer(this);

            //Campaign.Current.TimeControlMode = _timeSpeed;
        }

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);

            // Handle delayed close
            if (_closeRequested)
            {
                Close();
                return;
            }

            // Update video playback
            _viewModel?.Tick(dt);

            // Check for escape key
            if (_gauntletLayer.Input.IsKeyPressed(InputKey.Escape))
            {
                RequestClose();
            }
        }
    }

    /// <summary>
    /// Helper class to open popups
    /// </summary>
    public static class SimpleImagePopupHelper
    {
        /// <summary>
        /// Show a popup with a single image
        /// </summary>
        public static void ShowImage(string imagePath, string title = null, string soundPath = null)
        {
            ScreenManager.AddGlobalLayer(new SimpleImagePopupLayer(imagePath, title, soundPath), true);
        }


        /// <summary>
        /// Show a popup playing a video from a ZIP file containing image frames
        /// </summary>
        public static void ShowZipVideo(
            string zipPath,
            string title = null,
            double frameRate = 30,
            bool loop = false,
            bool autoPlay = true,
            bool closeOnFinished = false,
            string soundPath = null)
        {
            // Pre-load all frames from ZIP into VideoCache
            if (VideoCache.LoadFromZip(zipPath, frameRate, loop, soundPath))
            {
                ScreenManager.AddGlobalLayer(new SimpleImagePopupLayer(title, autoPlay, closeOnFinished), true);
            }
            else
            {
                Debug.Print($"[SimpleImagePopupHelper] Failed to load video from ZIP: {zipPath}");
            }
        }

        /// <summary>
        /// Show a popup playing a video from a ZIP file containing image frames
        /// </summary>
        public static void ShowH264Video(
            string zipPath,
            string title = null,
            double frameRate = 30,
            bool loop = false,
            bool autoPlay = true,
            bool closeOnFinished = false,
            string soundPath = null)
        {
            // Pre-load all frames from ZIP into VideoCache
            if (VideoCache.LoadFromH264(zipPath, (float)frameRate, loop, soundPath))
            {
                ScreenManager.AddGlobalLayer(new SimpleImagePopupLayer(title, autoPlay, closeOnFinished), true);
            }
            else
            {
                Debug.Print($"[SimpleImagePopupHelper] Failed to load video from ZIP: {zipPath}");
            }
        }
    }
}
