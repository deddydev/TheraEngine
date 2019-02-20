using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
        public unsafe struct Magic
        {
            private fixed sbyte _value[4];

            public Magic(string value) => Value = value;

            public string Value
            {
                get
                {
                    fixed (sbyte* ptr = _value)
                        return new string(ptr, 0, 4);
                }
                set
                {
                    fixed (sbyte* ptr = _value)
                    {
                        for (int i = 0; i < 4; ++i)
                            ptr[i] = i < value.Length ? (sbyte)value[i] : (sbyte)0;
                    }
                }
            }
            public override string ToString() => Value;
            public static implicit operator string(Magic m) => m.Value;
            public static implicit operator Magic(string str) => new Magic(str);
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct RIFFHeader
        {
            public static readonly string RIFFLittleMagic = "RIFF";
            public static readonly string RIFFBigMagic = "RIFX";
            public static readonly string WAVEMagic = "WAVE";

            public Magic _riffMagic;
            public bint _chunkSize; //4 + (8 + SubChunk1Size) + (8 + SubChunk2Size)
            public Magic _waveMagic;
            public FmtChunk _fmtChunk;
            public DataChunk _dataChunk;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct FmtChunk
            {
                public static readonly string Magic = "fmt ";
                
                public Magic _fmtMagic;
                public bint _size;
                public bshort _format;
                public bshort _numChannels;
                public bint _sampleRate;
                public bint _byteRate;
                public bshort _blockAlign;
                public bshort _bitsPerSample;
                //public bshort _extraParamSize; //Nonexistant if PCM

                //Extra params here

                public EWaveFormat Format
                {
                    get => (EWaveFormat)(short)_format;
                    set => _format = (short)value;
                }
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct DataChunk
            {
                public static readonly string Magic = "data";
                
                public Magic _dataMagic;
                public bint _size;

                //Sound data here
            }
        }
        public static byte[] ReadSamples(
            Stream waveFileStream,
            out int channels,
            out int bitsPerSample,
            out int sampleRate)
        {
            if (waveFileStream == null)
                throw new ArgumentNullException(nameof(waveFileStream));
            
            RIFFHeader riff = waveFileStream.Read<RIFFHeader>();

            bool littleEndian = riff._riffMagic == RIFFHeader.RIFFLittleMagic;
            bool bigEndian = riff._riffMagic == RIFFHeader.RIFFBigMagic;

            if (!littleEndian && !bigEndian)
                throw new NotSupportedException("Specified stream is not a wave file.");

            Endian.SerializeOrder = bigEndian ? Endian.EOrder.Big : Endian.EOrder.Little;

            if (riff._waveMagic != RIFFHeader.WAVEMagic)
                throw new NotSupportedException("Specified stream is not a wave file.");

            if (riff._fmtChunk._fmtMagic != RIFFHeader.FmtChunk.Magic)
                throw new NotSupportedException("Specified wave file is not supported.");

            if (riff._dataChunk._dataMagic != RIFFHeader.DataChunk.Magic)
                throw new NotSupportedException("Specified wave file is not supported.");

            channels = riff._fmtChunk._numChannels;
            sampleRate = riff._fmtChunk._sampleRate;
            bitsPerSample = riff._fmtChunk._bitsPerSample;

            int dataSize = riff._dataChunk._size;
            byte[] buffer = new byte[dataSize];
            int bytesRead = waveFileStream.Read(buffer, 0, dataSize);
            return buffer;
        }
    }
}
