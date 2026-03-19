using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace Dramalord.Video
{
    /// <summary>
    /// Extension methods for VideoCache to load H264 videos.
    /// Add this method to your existing VideoCache.cs file.
    /// </summary>
    public static class VideoCache_H264Extension
    {
        /// <summary>
        /// Load video from H264 Annex B file (.h264 or .264)
        /// 
        /// Usage:
        /// 1. Convert your video: ffmpeg -i input.mp4 -c:v copy -bsf:v h264_mp4toannexb output.h264
        /// 2. Call: VideoCache.LoadFromH264("path/to/video.h264", frameRate: 30, loop: false);
        /// 3. Call: VideoCache.Play();
        /// </summary>
        public static void LoadFromH264(string h264FilePath, float frameRate = 30f, bool loop = false)
        {
            // Clear any existing frames
            // _frames.Clear();
            // _engineTextures.Clear();
            
            using (var extractor = new H264FrameExtractor())
            {
                if (!extractor.LoadFromAnnexB(h264FilePath))
                {
                    throw new Exception($"Failed to decode H264 file: {h264FilePath}");
                }
                
                int width = extractor.Width;
                int height = extractor.Height;
                
                System.Diagnostics.Debug.WriteLine(
                    $"VideoCache: Loaded H264 video {width}x{height}, {extractor.FrameCount} frames");
                
                // Convert each frame to a Texture
                for (int i = 0; i < extractor.FrameCount; i++)
                {
                    byte[] rgbaData = extractor.GetFrame(i);
                    
                    // Create texture using Bannerlord's API
                    // This matches your existing pattern from zipped JPGs
                    var texture = Texture.CreateFromByteArray(rgbaData, width, height);
                    
                    // Add to your frame lists
                    // _frames.Add(texture);
                    // _engineTextures.Add(texture);
                }
                
                // Set playback properties
                // _frameRate = frameRate;
                // _loop = loop;
                // _currentFrame = 0;
                // _isPlaying = false;
            }
        }
    }
    
    /// <summary>
    /// Alternative: IFrameProvider implementation for H264 videos
    /// Use this if you prefer streaming-style playback instead of pre-loading
    /// </summary>
    public class H264FrameProvider : IFrameProvider
    {
        private H264FrameExtractor _extractor;
        private List<byte[]> _frames;
        private int _currentFrame;
        private float _frameRate;
        private bool _loop;
        private bool _isPlaying;
        private DateTime _lastFrameTime;
        private float _frameDuration;
        
        public int Width => _extractor?.Width ?? 0;
        public int Height => _extractor?.Height ?? 0;
        public bool IsPlaying => _isPlaying;
        public bool IsFinished => !_loop && _currentFrame >= (_frames?.Count ?? 0);
        public float TargetFrameRate => _frameRate;
        
        public void Initialize(string filePath)
        {
            _extractor = new H264FrameExtractor();
            
            if (!_extractor.LoadFromAnnexB(filePath))
            {
                throw new Exception($"Failed to load H264 file: {filePath}");
            }
            
            _frames = _extractor.GetAllFrames();
            _currentFrame = 0;
            _frameRate = 30f; // Default, can be overridden
            _frameDuration = 1f / _frameRate;
            _isPlaying = false;
        }
        
        public void SetFrameRate(float fps)
        {
            _frameRate = fps;
            _frameDuration = 1f / _frameRate;
        }
        
        public void SetLoop(bool loop)
        {
            _loop = loop;
        }
        
        public void Play()
        {
            _isPlaying = true;
            _lastFrameTime = DateTime.Now;
        }
        
        public void Stop()
        {
            _isPlaying = false;
            _currentFrame = 0;
        }
        
        public byte[] GetNextFrame()
        {
            if (_frames == null || _frames.Count == 0)
                return null;
                
            if (!_isPlaying)
                return _frames[_currentFrame];
            
            // Check if it's time for next frame
            var now = DateTime.Now;
            var elapsed = (float)(now - _lastFrameTime).TotalSeconds;
            
            if (elapsed >= _frameDuration)
            {
                _lastFrameTime = now;
                _currentFrame++;
                
                if (_currentFrame >= _frames.Count)
                {
                    if (_loop)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = _frames.Count - 1;
                        _isPlaying = false;
                    }
                }
            }
            
            return _frames[_currentFrame];
        }
        
        public void Dispose()
        {
            _extractor?.Dispose();
            _frames = null;
        }
    }
    
    /// <summary>
    /// Interface matching your existing IFrameProvider
    /// </summary>
    public interface IFrameProvider : IDisposable
    {
        void Initialize(string filePath);
        void Play();
        void Stop();
        byte[] GetNextFrame();
        int Width { get; }
        int Height { get; }
        bool IsPlaying { get; }
        bool IsFinished { get; }
        float TargetFrameRate { get; }
    }
}
