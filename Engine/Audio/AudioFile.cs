using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Audio
{
    /// <summary>
    /// Stores audio samples and information for playing them.
    /// </summary>
    [TFileDef("Audio File")]
    [TFileExt("aud", new string[] { "wav", "ogg" /*, "mp3", "flac"*/ }, null)]
    [TFile3rdPartyExt("wav")]
    public class AudioFile : TFileObject
    {
        private bool _useStreaming = false;

        [Category("Audio")]
        [Browsable(false)]
        [TSerialize(IsStreamable = true, NodeType = ENodeType.ElementContent)]
        public byte[] Samples { get; set; }

        [Category("Audio")]
        [TSerialize(IsAttribute = true)]
        public int Channels { get; set; }
        [Category("Audio")]
        [TSerialize(IsAttribute = true)]
        public int BitsPerSample { get; set; }
        [Category("Audio")]
        [TSerialize(IsAttribute = true)]
        public int SampleRate { get; set; }

        [Category("Streaming")]
        [TSerialize(IsAttribute = true)]
        public bool UseStreaming 
        {
            get => _useStreaming;
            set
            {
                Set(ref _useStreaming, value,
                    () => 
                    {
                        if (_useStreaming)
                            StreamingInstances = null;
                    },
                    () => 
                    {
                        if (_useStreaming)
                            StreamingInstances = new EventList<AudioInstance>(false, false);
                    });

            }
        }
        [Category("Streaming")]
        [TSerialize(IsAttribute = true)]
        public int StreamingChunkSize { get; set; } = 0;
        [Category("Streaming")]
        [TSerialize(IsAttribute = true)]
        public int StreamingMaxBufferedChunks { get; set; } = 0;

        //public bool GetStreamChunk(int index, out byte[] buffer)
        //{

        //}

        public override void ManualRead3rdParty(string filePath)
        {
            string ext = filePath.GetExtensionLowercase();
            switch (ext)
            {
                case "wav": ReadWave(filePath); break;
                case "ogg": ReadOgg(filePath); break;
            }
        }

        private void ReadOgg(string filePath)
        {
            Samples = OggFile.ReadSamples(filePath, out int channels, out int bps, out int sampleRate);
            Channels = channels;
            BitsPerSample = bps;
            SampleRate = sampleRate;
        }

        private void ReadWave(string filePath)
        {
            Samples = WaveFile.ReadAllSamples(filePath, out int channels, out int bps, out int sampleRate);
            Channels = channels;
            BitsPerSample = bps;
            SampleRate = sampleRate;
        }

        public EventList<AudioInstance> StreamingInstances { get; private set; }
    }
}
