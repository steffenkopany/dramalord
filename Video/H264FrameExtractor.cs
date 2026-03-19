using System;
using System.Collections.Generic;
using System.IO;
using cscodec.av;
using cscodec.h264.decoder;
using cscodec.util;

namespace Dramalord.Video
{
    /// <summary>
    /// Extracts frames from H264 video files using cscodec (pure C# decoder).
    /// On-demand decoding - only decodes frames as they are needed.
    /// </summary>
    public class H264FrameExtractor : IDisposable
    {
        private int _width;
        private int _height;
        
        // Raw H264 data
        private byte[] _h264Data;
        
        // NAL unit index (offset, size, type)
        private List<NalUnit> _nalUnits;
        
        // Decoder state (kept alive for sequential decoding)
        private H264Decoder _codec;
        private MpegEncContext _context;
        private AVFrame _picture;
        private AVPacket _avpkt;
        private int[] _gotPicture;
        private byte[] _inbuf;
        private const int INBUF_SIZE = 1024 * 1024;
        
        // Decoding state
        private int _nextNalIndex;      // Next NAL to feed to decoder
        private int _decodedFrameCount; // How many frames we've decoded so far
        
        // Frame cache
        private Dictionary<int, byte[]> _frameCache;
        private int _cacheSize = 10;
        
        // Pre-decode settings
        private int _preDecodeCount = 3;

        public int Width => _width;
        public int Height => _height;
        public int FrameCount { get; private set; }

        public static bool DebugLogging = false;

        // Pre-computed lookup tables for YUV to RGB conversion
        private static readonly int[] YTable = new int[256];
        private static readonly int[] RVTable = new int[256];
        private static readonly int[] GUTable = new int[256];
        private static readonly int[] GVTable = new int[256];
        private static readonly int[] BUTable = new int[256];
        private static readonly byte[] ClampTable = new byte[1024];
        private static bool _tablesInitialized = false;

        private class NalUnit
        {
            public int Offset;  // Offset in _h264Data (including start code)
            public int Size;    // Size including start code
            public int Type;    // NAL unit type
        }

        static H264FrameExtractor()
        {
            InitializeLookupTables();
        }

        private static void InitializeLookupTables()
        {
            if (_tablesInitialized) return;

            for (int i = 0; i < 256; i++)
            {
                YTable[i] = (int)(1.164 * 256 * (i - 16));
                RVTable[i] = (int)(1.596 * 256 * (i - 128));
                GUTable[i] = (int)(0.392 * 256 * (i - 128));
                GVTable[i] = (int)(0.813 * 256 * (i - 128));
                BUTable[i] = (int)(2.017 * 256 * (i - 128));
            }

            for (int i = 0; i < 1024; i++)
            {
                int val = i - 512;
                if (val < 0) ClampTable[i] = 0;
                else if (val > 255) ClampTable[i] = 255;
                else ClampTable[i] = (byte)val;
            }

            _tablesInitialized = true;
        }

        public H264FrameExtractor()
        {
            _nalUnits = new List<NalUnit>();
            _frameCache = new Dictionary<int, byte[]>();
            _gotPicture = new int[1];
        }

        private static void Log(string message)
        {
            if (!DebugLogging) return;
            
            System.Diagnostics.Debug.WriteLine($"[H264Extractor] {message}");
            try
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Mount and Blade II Bannerlord",
                    "h264_debug.log"
                );
                File.AppendAllText(logPath, $"{DateTime.Now:HH:mm:ss.fff} {message}\n");
            }
            catch { }
        }

        /// <summary>
        /// Load H264 file - only parses NAL structure, minimal decoding.
        /// </summary>
        public bool LoadFromAnnexB(string h264FilePath)
        {
            try
            {
                Log($"Loading file: {h264FilePath}");
                
                if (!File.Exists(h264FilePath))
                {
                    Log($"File not found: {h264FilePath}");
                    return false;
                }

                _h264Data = File.ReadAllBytes(h264FilePath);
                Log($"File size: {_h264Data.Length} bytes");
                
                return Initialize();
            }
            catch (Exception ex)
            {
                Log($"Failed to load {h264FilePath}: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Load from byte array
        /// </summary>
        public bool LoadFromBytes(byte[] h264Data)
        {
            _h264Data = h264Data;
            return Initialize();
        }

        /// <summary>
        /// Initialize: parse NAL units and set up decoder
        /// </summary>
        private bool Initialize()
        {
            _nalUnits.Clear();
            _frameCache.Clear();
            _nextNalIndex = 0;
            _decodedFrameCount = 0;
            _width = 0;
            _height = 0;

            // Step 1: Parse NAL unit boundaries (very fast - just byte scanning)
            ParseNalUnits();
            Log($"Found {_nalUnits.Count} NAL units");

            if (_nalUnits.Count == 0)
            {
                Log("ERROR: No NAL units found");
                return false;
            }

            // Step 2: Initialize decoder
            if (!InitializeDecoder())
            {
                Log("ERROR: Failed to initialize decoder");
                return false;
            }

            // Step 3: Decode first few frames to get dimensions and pre-fill cache
            // We need to decode at least until we get one frame to know dimensions
            DecodeUntilFrame(_preDecodeCount);

            if (_width == 0 || _height == 0)
            {
                Log("ERROR: Could not determine video dimensions");
                return false;
            }

            // Step 4: Count total frames by scanning NAL types
            // This is an estimate - count slice NALs (types 1 and 5)
            CountFrames();

            Log($"Initialized: {_width}x{_height}, ~{FrameCount} frames, {_decodedFrameCount} pre-decoded");
            return true;
        }

        /// <summary>
        /// Scan file for NAL unit start codes and build index
        /// </summary>
        private void ParseNalUnits()
        {
            int i = 0;
            int lastNalStart = -1;
            int lastStartCodeLen = 0;

            while (i < _h264Data.Length - 3)
            {
                // Check for start code
                bool is4Byte = i < _h264Data.Length - 3 &&
                               _h264Data[i] == 0 && _h264Data[i + 1] == 0 && 
                               _h264Data[i + 2] == 0 && _h264Data[i + 3] == 1;
                bool is3Byte = !is4Byte &&
                               _h264Data[i] == 0 && _h264Data[i + 1] == 0 && _h264Data[i + 2] == 1;

                if (is4Byte || is3Byte)
                {
                    // Save previous NAL unit
                    if (lastNalStart >= 0)
                    {
                        int nalSize = i - lastNalStart;
                        int nalTypeOffset = lastNalStart + lastStartCodeLen;
                        int nalType = (nalTypeOffset < _h264Data.Length) ? (_h264Data[nalTypeOffset] & 0x1F) : 0;
                        
                        _nalUnits.Add(new NalUnit
                        {
                            Offset = lastNalStart,
                            Size = nalSize,
                            Type = nalType
                        });
                    }

                    // Start new NAL
                    lastNalStart = i;
                    lastStartCodeLen = is4Byte ? 4 : 3;
                    i += lastStartCodeLen;
                }
                else
                {
                    i++;
                }
            }

            // Don't forget last NAL unit
            if (lastNalStart >= 0)
            {
                int nalSize = _h264Data.Length - lastNalStart;
                int nalTypeOffset = lastNalStart + lastStartCodeLen;
                int nalType = (nalTypeOffset < _h264Data.Length) ? (_h264Data[nalTypeOffset] & 0x1F) : 0;
                
                _nalUnits.Add(new NalUnit
                {
                    Offset = lastNalStart,
                    Size = nalSize,
                    Type = nalType
                });
            }
        }

        /// <summary>
        /// Count frames by counting slice NALs
        /// </summary>
        private void CountFrames()
        {
            int count = 0;
            foreach (var nal in _nalUnits)
            {
                // Type 1 = non-IDR slice, Type 5 = IDR slice
                if (nal.Type == 1 || nal.Type == 5)
                {
                    count++;
                }
            }
            FrameCount = count;
        }

        /// <summary>
        /// Initialize the H264 decoder
        /// </summary>
        private bool InitializeDecoder()
        {
            try
            {
                _codec = new H264Decoder();
                _context = MpegEncContext.avcodec_alloc_context();
                _picture = AVFrame.avcodec_alloc_frame();
                _inbuf = new byte[INBUF_SIZE + MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE];

                if ((_codec.capabilities & H264Decoder.CODEC_CAP_TRUNCATED) != 0)
                {
                    _context.flags |= MpegEncContext.CODEC_FLAG_TRUNCATED;
                }

                if (_context.avcodec_open(_codec) < 0)
                {
                    return false;
                }

                _avpkt = new AVPacket();
                _avpkt.av_init_packet();

                return true;
            }
            catch (Exception ex)
            {
                Log($"InitializeDecoder error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Decode NAL units until we have decoded at least targetFrameCount frames
        /// </summary>
        private void DecodeUntilFrame(int targetFrameCount)
        {
            while (_decodedFrameCount < targetFrameCount && _nextNalIndex < _nalUnits.Count)
            {
                DecodeNextNal();
            }
        }

        /// <summary>
        /// Decode the next NAL unit in sequence
        /// </summary>
        /// <returns>True if a frame was produced</returns>
        private bool DecodeNextNal()
        {
            if (_nextNalIndex >= _nalUnits.Count)
                return false;

            var nal = _nalUnits[_nextNalIndex++];

            // Copy NAL data to input buffer
            int copySize = Math.Min(nal.Size, INBUF_SIZE);
            Array.Copy(_h264Data, nal.Offset, _inbuf, 0, copySize);
            
            // Clear padding
            for (int i = copySize; i < copySize + MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE && i < _inbuf.Length; i++)
            {
                _inbuf[i] = 0;
            }

            _avpkt.data_base = _inbuf;
            _avpkt.data_offset = 0;
            _avpkt.size = copySize;

            int len = _context.avcodec_decode_video2(_picture, _gotPicture, _avpkt);

            if (len < 0)
            {
                return false;
            }

            if (_gotPicture[0] != 0)
            {
                AVFrame displayPicture = _context.priv_data.displayPicture;
                
                if (displayPicture != null)
                {
                    // Get dimensions on first frame
                    if (_width == 0)
                    {
                        if (_context.coded_width > 0 && _context.coded_height > 0)
                        {
                            _width = _context.coded_width;
                            _height = _context.coded_height;
                        }
                        else
                        {
                            _width = displayPicture.imageWidthWOEdge > 0 
                                ? displayPicture.imageWidthWOEdge 
                                : displayPicture.imageWidth;
                            _height = displayPicture.imageHeightWOEdge > 0 
                                ? displayPicture.imageHeightWOEdge 
                                : displayPicture.imageHeight;
                        }
                        Log($"Video dimensions: {_width}x{_height}");
                    }

                    // Convert and cache this frame
                    byte[] rgba = ConvertYuvToRgba(displayPicture);
                    _frameCache[_decodedFrameCount] = rgba;
                    _decodedFrameCount++;
                    
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get frame as RGBA - decodes on demand if needed
        /// </summary>
        public byte[] GetFrame(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= FrameCount)
                return null;

            // If frame is in cache, return it
            if (_frameCache.TryGetValue(frameIndex, out byte[] cached))
            {
                // Pre-decode ahead
                PreDecodeAhead(frameIndex);
                CleanupCache(frameIndex);
                return cached;
            }

            // Need to decode up to this frame
            // For sequential playback, we should already be close
            while (_decodedFrameCount <= frameIndex && _nextNalIndex < _nalUnits.Count)
            {
                DecodeNextNal();
            }

            // Try to get from cache again
            if (_frameCache.TryGetValue(frameIndex, out cached))
            {
                PreDecodeAhead(frameIndex);
                CleanupCache(frameIndex);
                return cached;
            }

            // Frame not available (might happen at end of stream)
            return null;
        }

        /// <summary>
        /// Pre-decode frames ahead of current position
        /// </summary>
        private void PreDecodeAhead(int currentFrame)
        {
            int targetFrame = currentFrame + _preDecodeCount;
            
            // Decode one frame ahead per call to spread the work
            if (_decodedFrameCount <= targetFrame && _nextNalIndex < _nalUnits.Count)
            {
                DecodeNextNal();
            }
        }

        /// <summary>
        /// Remove old frames from cache to save memory
        /// </summary>
        private void CleanupCache(int currentFrame)
        {
            if (_frameCache.Count > _cacheSize)
            {
                // Remove frames more than cacheSize behind current
                int removeThreshold = currentFrame - _cacheSize;
                var keysToRemove = new List<int>();
                
                foreach (var key in _frameCache.Keys)
                {
                    if (key < removeThreshold)
                    {
                        keysToRemove.Add(key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    _frameCache.Remove(key);
                }
            }
        }

        /// <summary>
        /// Convert YUV frame to RGBA using lookup tables
        /// </summary>
        private byte[] ConvertYuvToRgba(AVFrame avFrame)
        {
            byte[] rgba = new byte[_width * _height * 4];

            int yStride = avFrame.linesize[0];
            int uStride = avFrame.linesize[1];
            int vStride = avFrame.linesize[2];

            int yOffset = avFrame.data_offset[0];
            int uOffset = avFrame.data_offset[1];
            int vOffset = avFrame.data_offset[2];

            byte[] yData = avFrame.data_base[0];
            byte[] uData = avFrame.data_base[1];
            byte[] vData = avFrame.data_base[2];

            if (yData == null || uData == null || vData == null) 
                return rgba;

            int rgbaIndex = 0;

            for (int y = 0; y < _height; y++)
            {
                int yRowOffset = yOffset + y * yStride;
                int uvRowOffset = (y >> 1) * uStride;
                int uRowStart = uOffset + uvRowOffset;
                int vRowStart = vOffset + uvRowOffset;

                for (int x = 0; x < _width; x++)
                {
                    int yIdx = yRowOffset + x;
                    int uvIdx = x >> 1;

                    int yVal = yData[yIdx];
                    int uVal = uData[uRowStart + uvIdx];
                    int vVal = vData[vRowStart + uvIdx];

                    int yComponent = YTable[yVal];
                    int r = (yComponent + RVTable[vVal]) >> 8;
                    int g = (yComponent - GUTable[uVal] - GVTable[vVal]) >> 8;
                    int b = (yComponent + BUTable[uVal]) >> 8;

                    rgba[rgbaIndex++] = ClampTable[r + 512];
                    rgba[rgbaIndex++] = ClampTable[g + 512];
                    rgba[rgbaIndex++] = ClampTable[b + 512];
                    rgba[rgbaIndex++] = 255;
                }
            }

            return rgba;
        }

        /// <summary>
        /// Get all frames (for compatibility - decodes everything)
        /// </summary>
        public List<byte[]> GetAllFrames()
        {
            var allFrames = new List<byte[]>(FrameCount);
            for (int i = 0; i < FrameCount; i++)
            {
                allFrames.Add(GetFrame(i));
            }
            return allFrames;
        }

        /// <summary>
        /// Reset playback to beginning (for looping)
        /// </summary>
        public void Reset()
        {
            _frameCache.Clear();
            _nextNalIndex = 0;
            _decodedFrameCount = 0;

            // Re-initialize decoder
            try { _context?.avcodec_close(); } catch { }
            InitializeDecoder();

            // Pre-decode first frames
            DecodeUntilFrame(_preDecodeCount);
        }

        public void Dispose()
        {
            try { _context?.avcodec_close(); } catch { }
            _h264Data = null;
            _nalUnits?.Clear();
            _nalUnits = null;
            _frameCache?.Clear();
            _frameCache = null;
            _inbuf = null;
        }
    }
}
