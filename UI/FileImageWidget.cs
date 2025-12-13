using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace Dramalord.UI
{
    public class FileImageWidget : TextureWidget
    {
        private string _filePath;

        [Editor(false)]
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(value, "FilePath");
                    SetTextureProviderProperty("FilePath", value);
                }
            }
        }

        public FileImageWidget(UIContext context) : base(context)
        {
            base.TextureProviderName = "FileTextureProvider";
        }
    }
}
