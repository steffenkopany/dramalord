using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Dramalord.UI
{
    public class ImageInquiryVM : ViewModel
    {
        private readonly Action _onAffirmative;
        private readonly Action _onNegative;
        private readonly Action _closeLayer;

        private string _imagePath;
        private string _title;
        private string _affirmativeText;
        private string _negativeText;

        public ImageInquiryVM(
            Action closeLayer,
            string imagePath,
            string title,
            string affirmativeText,
            string negativeText,
            Action onAffirmative,
            Action onNegative)
        {
            _closeLayer = closeLayer;
            _imagePath = imagePath ?? "";
            _title = title ?? "";
            _affirmativeText = affirmativeText ?? new TextObject("{=aeouhelq}Yes").ToString();
            _negativeText = negativeText ?? new TextObject("{=8OkPHu4f}No").ToString();
            _onAffirmative = onAffirmative;
            _onNegative = onNegative;
        }

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
        public string AffirmativeText
        {
            get => _affirmativeText;
            set
            {
                if (_affirmativeText != value)
                {
                    _affirmativeText = value;
                    OnPropertyChangedWithValue(value, "AffirmativeText");
                }
            }
        }

        [DataSourceProperty]
        public string NegativeText
        {
            get => _negativeText;
            set
            {
                if (_negativeText != value)
                {
                    _negativeText = value;
                    OnPropertyChangedWithValue(value, "NegativeText");
                }
            }
        }

        public void ExecuteAffirmative()
        {
            _closeLayer?.Invoke();
            _onAffirmative?.Invoke();
        }

        public void ExecuteNegative()
        {
            _closeLayer?.Invoke();
            _onNegative?.Invoke();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
        }
    }
}
