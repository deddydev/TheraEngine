using System;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Audio
{
    /// <summary>
    /// Stores audio samples and information for playing them.
    /// </summary>
    [TFileDef("Audio File")]
    [TFileExt("aud", new string[] { "wav"/*, "mp3", "ogg", "flac"*/ }, null)]
    public class AudioFile : TFileObject
    {
        [TSerialize(IsElementString = true)]
        public byte[] Samples { get; private set; }
        [TSerialize(IsAttribute = true)]
        public int Channels { get; private set; }
        [TSerialize(IsAttribute = true)]
        public int BitsPerSample { get; private set; };
        [TSerialize(IsAttribute = true)]
        public int SampleRate { get; private set; };
        
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
