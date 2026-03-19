using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace Dramalord.UI
{
    public class FileTextureProvider : TextureProvider
    {
        private Texture _texture;
        private TaleWorlds.Engine.Texture _engineTexture;
        private string _filePath;
        private bool _isReleased;
        private bool _useVideoCache;

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    
                    // Check if we should use VideoCache instead of loading from file
                    if (_filePath == "VideoCache")
                    {
                        _useVideoCache = true;
                        ReleaseTexture();
                    }
                    else
                    {
                        _useVideoCache = false;
                        LoadTexture();
                    }
                }
            }
        }

        public bool IsReleased
        {
            get => _isReleased;
            set
            {
                _isReleased = value;
                if (_isReleased)
                {
                    ReleaseTexture();
                }
            }
        }

        public FileTextureProvider()
        {
            _isReleased = false;
            _useVideoCache = false;
        }

        private void LoadTexture()
        {
            ReleaseTexture();

            if (string.IsNullOrEmpty(_filePath))
            {
                return;
            }

            try
            {
                string directory = System.IO.Path.GetDirectoryName(_filePath) ?? "";
                string fileName = System.IO.Path.GetFileName(_filePath);

                _engineTexture = TaleWorlds.Engine.Texture.LoadTextureFromPath(fileName, directory);

                if (_engineTexture != null)
                {
                    _texture = new Texture(new EngineTexture(_engineTexture));
                    Debug.Print($"[FileTextureProvider] Loaded texture: {_filePath}");
                }
                else
                {
                    Debug.Print($"[FileTextureProvider] Failed to load texture: {_filePath}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.Print($"[FileTextureProvider] Error loading texture {_filePath}: {ex.Message}");
            }
        }

        private void ReleaseTexture()
        {
            if (_engineTexture != null && !_engineTexture.IsReleased)
            {
                _engineTexture.Release();
            }
            _engineTexture = null;
            _texture = null;
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);

            // If using VideoCache, advance playback each tick
            if (_useVideoCache && VideoCache.IsLoaded)
            {
                // VideoCache.Tick handles the timing internally
                // We don't store the texture - we get it fresh in OnGetTextureForRender
            }
        }

        protected override Texture OnGetTextureForRender(TwoDimensionContext twoDimensionContext, string name)
        {
            if (_useVideoCache && VideoCache.IsLoaded)
            {
                return VideoCache.GetCurrentTexture();
            }
            return _texture;
        }

        public override void Clear(bool clearNextFrame)
        {
            ReleaseTexture();
            base.Clear(clearNextFrame);
        }
    }
}
