using SandBox;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace Dramalord.UI
{
    public class ImageInquiryLayer : GlobalLayer
    {
        private GauntletLayer _gauntletLayer;
        private ImageInquiryVM _viewModel;
        private bool _closeRequested = false;

        //private CampaignTimeControlMode _timeSpeed = CampaignTimeControlMode.Stop;

        public ImageInquiryLayer(
            string imagePath,
            string title,
            string affirmativeText,
            string negativeText,
            Action onAffirmative,
            Action onNegative)
        {
            //_timeSpeed = Campaign.Current.TimeControlMode;
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;

            _viewModel = new ImageInquiryVM(
                RequestClose,
                imagePath,
                title,
                affirmativeText,
                negativeText,
                onAffirmative,
                onNegative);

            _gauntletLayer = new GauntletLayer("ImageInquiryLayer", 10000);
            _gauntletLayer.LoadMovie("ImageInquiry", _viewModel);
            _gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
            _gauntletLayer.IsFocusLayer = true;

            base.Layer = _gauntletLayer;

            ScreenManager.TrySetFocus(_gauntletLayer);

            try
            {
                int eventId = SoundEvent.GetEventIdFromString("inquiry_chime");
                if (eventId >= 0)
                {
                    SoundEvent.CreateEvent(eventId, ((MapScene)Campaign.Current.MapSceneWrapper).Scene)?.Play();
                }
            }
            catch { }
        }

        private void RequestClose()
        {
            _closeRequested = true;
        }

        private void Close()
        {
            if (_viewModel != null)
            {
                _viewModel.OnFinalize();
                _viewModel = null;
            }

            ScreenManager.RemoveGlobalLayer(this);

            //Campaign.Current.TimeControlMode = _timeSpeed;
        }

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_closeRequested)
            {
                Close();
                return;
            }

            // Escape key triggers negative action
            if (_gauntletLayer.Input.IsKeyPressed(InputKey.Escape))
            {
                _viewModel?.ExecuteNegative();
            }
        }
    }

    public static class ImageInquiryHelper
    {
        public static void Show(
            string imagePath,
            string title,
            Action onAffirmative,
            Action onNegative = null)
        {
            ScreenManager.AddGlobalLayer(new ImageInquiryLayer(
                imagePath,
                title,
                null,  // Default "Yes"
                null,  // Default "No"
                onAffirmative,
                onNegative), true);
        }

        public static void ShowCustom(
            string imagePath,
            string title,
            string affirmativeText,
            string negativeText,
            Action onAffirmative,
            Action onNegative = null)
        {
            ScreenManager.AddGlobalLayer(new ImageInquiryLayer(
                imagePath,
                title,
                affirmativeText,
                negativeText,
                onAffirmative,
                onNegative), true);
        }
    }
}
