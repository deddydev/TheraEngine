using System;
using System.IO;
using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;

namespace TheraEngine.Audio
{
    /// <summary>
    /// <see langword="static"/> helper class to read .wav audio files.
    /// </summary>
    public static class WaveFile
    {
        public static byte[] ReadSamples(
            string filePath,
            out int channels,
            out int bitsPerSample,
            out int sampleRate)
        {
            if (!File.Exists(filePath))
            {
                channels        = 0;
                bitsPerSample   = 0;
                sampleRate      = 0;

                return null;
            }

            using (FileStream stream = File.Open(filePath, FileMode.Open))
                return ReadSamples(stream, out channels, out bitsPerSample, out sampleRate);
        }
        public enum EWaveFormat
        {
            PCM = 1,
            Float = 3,
            ALaw = 6,
            MULaw = 7,
            Extensible = 0xFFFE,
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct RIFFHeader
        {
            public static readonly string RIFFMagic = "RIFF";
            public static readonly string WAVEMagic = "WAVE";
            public static readonly string fmtMagic = "fmt ";

            public fixed byte _riffMagic[4];
            public bint _chunkSize;
            public fixed byte _waveMagic[4];
        }
        public static byte[] ReadSamples(
            Stream waveFileStream,
            out int channels,
            out int bitsPerSample,
            out int sampleRate)
        {
            if (waveFileStream == null)
                throw new ArgumentNullException(nameof(waveFileStream));

            using (BinaryReader reader = new BinaryReader(waveFileStream))
            {
                RIFFHeader riff = reader.Read<RIFFHeader>();

                // RIFF header
                string riffMagic = new string(reader.ReadChars(4));
                if (riffMagic != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int chunkSize = reader.ReadInt32();

                string waveMagic = new string(reader.ReadChars(4));
                if (waveMagic != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string fmtMagic = new string(reader.ReadChars(4));
                if (fmtMagic != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int formatChunkSize = reader.ReadInt32(); //Chunk size: 16, 18 or 40

                int audioFormat     = reader.ReadInt16(); //Format code
//0x0001  WAVE_FORMAT_PCM PCM
//0x0003  WAVE_FORMAT_IEEE_FLOAT IEEE float
//0x0006  WAVE_FORMAT_ALAW    8 - bit ITU - T G.711 A - law
//0x0007  WAVE_FORMAT_MULAW   8 - bit ITU - T G.711 µ - law
//0xFFFE  WAVE_FORMAT_EXTENSIBLE Determined by SubFormat

                channels            = reader.ReadInt16();
                sampleRate          = reader.ReadInt32();
                int byteRate        = reader.ReadInt32();
                int blockAlign      = reader.ReadInt16();
                bitsPerSample       = reader.ReadInt16();

                string dataMagic = new string(reader.ReadChars(4));
                if (dataMagic != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int dataSize = reader.ReadInt32();
                
                return reader.ReadBytes(dataSize);
            }
        }
    }
}
