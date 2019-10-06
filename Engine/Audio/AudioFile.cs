using Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Audio
{
    /// <summary>
    /// Stores audio samples and information for playing them.
    /// </summary>
    [TFileDef("Audio File")]
    [TFileExt("aud", new string[] { "wav"/*, "mp3", "ogg", "flac"*/ }, null)]
    [TFile3rdPartyExt("wav")]
    public class AudioFile : TFileObject
    {
        [Browsable(false)]
        [TSerialize]
        public byte[] Samples { get; set; }
        [TSerialize(IsAttribute = true)]
        public int Channels { get; set; }
        [TSerialize(IsAttribute = true)]
        public int BitsPerSample { get; set; }
        [TSerialize(IsAttribute = true)]
        public int SampleRate { get; set; }

        [Browsable(false)]
        public HashSet<AudioInstance> Instances { get; } = new HashSet<AudioInstance>();
        [Browsable(false)]
        public int BufferId { get; internal set; }

        public override void ManualRead3rdParty(string filePath)
        {
            string ext = filePath.GetExtensionLowercase();
            switch (ext)
            {
                case "wav":

                    Samples = WaveFile.ReadSamples(filePath,
                        out int channels, out int bps, out int sampleRate);

                    Channels = channels;
                    BitsPerSample = bps;
                    SampleRate = sampleRate;

                    break;
            }
        }
    }
}
