using System;
using System.ComponentModel;
using System.IO;

namespace TheraEngine.Audio
{
    public class WaveFile : IDisposable
    {
        [TSerialize(nameof(Channels), IsXmlAttribute = true)]
        private int _channels;
        [TSerialize(nameof(BitsPerSample), IsXmlAttribute = true)]
        private int _bps;
        [TSerialize(nameof(SampleRate), IsXmlAttribute = true)]
        private int _sampleRate;

        [TSerialize(IsXmlElementString = true)]
        public byte[] SoundData { get; private set; }

        public int Channels => _channels;
        public int BitsPerSample => _bps;
        public int SampleRate => _sampleRate;

        public WaveFile() { }
        public WaveFile(string filename)
            => SoundData = LoadWave(filename, out _channels, out _bps, out _sampleRate);

        public static byte[] LoadWave(string filename, out int channels, out int bitsPerSample, out int sampleRate)
        {
            if (!File.Exists(filename))
            {
                channels = 0;
                bitsPerSample = 0;
                sampleRate = 0;
                return null;
            }

            return LoadWave(File.Open(filename, FileMode.Open), out channels, out bitsPerSample, out sampleRate);
        }
        public static byte[] LoadWave(Stream stream, out int channels, out int bitsPerSample, out int sampleRate)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunk_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bitsPerSample = bits_per_sample;
                sampleRate = sample_rate;

                return reader.ReadBytes(data_chunk_size);
            }
        }

        public void Dispose()
        {
            SoundData = null;
        }
    }
}
