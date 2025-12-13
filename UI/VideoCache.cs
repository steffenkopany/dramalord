using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;

namespace Dramalord.UI
{
    public static class VideoCache
    {
        private static List<TaleWorlds.TwoDimension.Texture> _frames = new List<TaleWorlds.TwoDimension.Texture>();
        private static List<TaleWorlds.Engine.Texture> _engineTextures = new List<TaleWorlds.Engine.Texture>();
        
        private static double _frameRate = 30;
        private static double _frameDuration = 1.0 / 30;
        private static bool _loop = false;
        private static string _soundPath = null;
        
        private static int _currentFrameIndex = 0;
        private static float _timeSinceLastFrame = 0f;
        private static bool _isPlaying = false;
        private static bool _isFinished = false;

        public static bool IsLoaded => _frames.Count > 0;

        public static bool IsPlaying => _isPlaying;

        public static bool IsFinished => _isFinished;

        public static int FrameCount => _frames.Count;

        public static int CurrentFrameIndex => _currentFrameIndex;

        public static string SoundPath => _soundPath;

        public static bool Loop => _loop;


        public static bool LoadFromZip(string zipPath, double frameRate = 30, bool loop = false, string soundPath = null)
        {
            Clear();

            try
            {
                if (!File.Exists(zipPath))
                {
                    Debug.Print($"[VideoCache] ZIP file not found: {zipPath}");
                    return false;
                }

                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    // Get all image entries and sort by name
                    var imageEntries = new List<ZipArchiveEntry>();
                    
                    foreach (var entry in archive.Entries)
                    {
                        string ext = System.IO.Path.GetExtension(entry.Name).ToLowerInvariant();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                        {
                            imageEntries.Add(entry);
                        }
                    }

                    if (imageEntries.Count == 0)
                    {
                        Debug.Print($"[VideoCache] No images found in ZIP");
                        return false;
                    }

                    // Sort by filename
                    imageEntries.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

                    // Load each image directly from ZIP into memory
                    foreach (var entry in imageEntries)
                    {
                        try
                        {
                            using (var stream = entry.Open())
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);
                                byte[] imageData = memoryStream.ToArray();

                                // Create texture directly from encoded image data
                                var engineTex = TaleWorlds.Engine.Texture.CreateFromMemory(imageData);
                                if (engineTex != null)
                                {
                                    _engineTextures.Add(engineTex);
                                    _frames.Add(new TaleWorlds.TwoDimension.Texture(new EngineTexture(engineTex)));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Print($"[VideoCache] Error loading {entry.Name}: {ex.Message}");
                        }
                    }
                }

                if (_frames.Count == 0)
                {
                    Debug.Print($"[VideoCache] Failed to load any frames from ZIP");
                    return false;
                }

                _frameRate = Math.Max(1, frameRate);
                _frameDuration = 1.0 / _frameRate;
                _loop = loop;
                _soundPath = soundPath;
                _currentFrameIndex = 0;
                _timeSinceLastFrame = 0f;
                _isFinished = false;

                Debug.Print($"[VideoCache] Loaded {_frames.Count} frames from ZIP @ {_frameRate} fps");
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print($"[VideoCache] Error loading from ZIP: {ex.Message}");
                Clear();
                return false;
            }
        }

        public static void Play()
        {
            if (_frames.Count > 0)
            {
                _isPlaying = true;
                _isFinished = false;
                _timeSinceLastFrame = (float)_frameDuration; // Show first frame immediately
            }
        }

        public static void Pause()
        {
            _isPlaying = false;
        }

        public static void Stop()
        {
            _isPlaying = false;
            _currentFrameIndex = 0;
            _timeSinceLastFrame = 0f;
            _isFinished = false;
        }

        public static TaleWorlds.TwoDimension.Texture Tick(float deltaTime)
        {
            if (_frames.Count == 0)
                return null;

            if (!_isPlaying || _isFinished)
            {
                // Return current frame even if not playing
                return _frames[_currentFrameIndex];
            }

            _timeSinceLastFrame += deltaTime;

            if (_timeSinceLastFrame >= _frameDuration)
            {
                int framesToAdvance = (int)(_timeSinceLastFrame / _frameDuration);
                _timeSinceLastFrame %= (float)_frameDuration;

                _currentFrameIndex += framesToAdvance;

                if (_currentFrameIndex >= _frames.Count)
                {
                    if (_loop)
                    {
                        _currentFrameIndex %= _frames.Count;
                    }
                    else
                    {
                        _currentFrameIndex = _frames.Count - 1;
                        _isPlaying = false;
                        _isFinished = true;
                    }
                }
            }

            return _frames[_currentFrameIndex];
        }

        public static TaleWorlds.TwoDimension.Texture GetCurrentTexture()
        {
            if (_frames.Count == 0 || _currentFrameIndex >= _frames.Count)
                return null;
            return _frames[_currentFrameIndex];
        }

        public static void Clear()
        {
            
            _isPlaying = false;
            _isFinished = false;
            _currentFrameIndex = 0;
            _timeSinceLastFrame = 0f;

            // Release engine textures
            foreach (var tex in _engineTextures)
            {
                try
                {
                    if (tex != null && !tex.IsReleased)
                    {
                        tex.Release();
                    }
                }
                catch { }
            }

            _engineTextures.Clear();
            _frames.Clear();
            _soundPath = null;

            Debug.Print("[VideoCache] Cleared");
        }
    }
}
