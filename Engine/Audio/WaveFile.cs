using Extensions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;
using static TheraEngine.Audio.WaveFile.RIFFHeader;

namespace TheraEngine.Audio
{
    /// <summary>
    /// <see langword="static"/> helper class to read .wav audio files.
    /// </summary>
    public static class WaveFile
    {
        public static byte[] ReadAllSamples(
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

            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
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
            public bshort _extensionSize;

            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }

            public DataChunk GetDataChunk()
            {
                if (_fmtChunk.Format == EWaveFormat.PCM)
                    return *(DataChunk*)(Address + 12 + sizeof(FmtChunk));
                
                if (_fmtChunk.Format == EWaveFormat.Extensible)
                    return *(DataChunk*)(Address + 12 + sizeof(FmtChunk) + 24 + sizeof(FactChunk));

                return *(DataChunk*)(Address + 12 + sizeof(FmtChunk) + 2 + sizeof(FactChunk));
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct FmtChunk
            {
                public static readonly string Magic = "fmt ";
                
                public Magic _fmtMagic;
                public bint _size;
                public bshort _format;
                public bshort _numChannels; //Nc
                public bint _sampleRate; //F
                public bint _byteRate; //F*M*Nc
                public bshort _blockAlign; //M*Nc
                public bshort _bitsPerSample; //8/16*M

                public EWaveFormat Format
                {
                    get => (EWaveFormat)(short)_format;
                    set => _format = (short)value;
                }
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct FactChunk
            {
                public static readonly string Magic = "fact";

                public Magic _dataMagic;
                public bint _size;
                public bint _sampleSize;
            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct DataChunk
            {
                public static readonly string Magic = "data";
                
                public Magic _dataMagic;
                public bint _size;

                public int GetChunkCount(int chunkSize)
                    => _size / chunkSize;

                //Sound data here
            }
        }

        public static byte[] ReadSamples(
            Stream waveFileStream,
            out int channels,
            out int bitsPerSample,
            out int sampleRate)
        {
            if (waveFileStream is null)
                throw new ArgumentNullException(nameof(waveFileStream));

            RIFFHeader riff = waveFileStream.Read<RIFFHeader>();

            bool littleEndian = riff._riffMagic == RIFFLittleMagic;
            bool bigEndian = riff._riffMagic == RIFFBigMagic;

            if (!littleEndian && !bigEndian)
                throw new NotSupportedException("Specified stream is not a wave file.");

            Endian.SerializeOrder = bigEndian ? Endian.EOrder.Big : Endian.EOrder.Little;

            if (riff._waveMagic != WAVEMagic)
                throw new NotSupportedException("Specified stream is not a wave file.");

            if (riff._fmtChunk._fmtMagic != FmtChunk.Magic)
                throw new NotSupportedException("Specified wave file is not supported.");

            DataChunk dataChunk = riff.GetDataChunk();

            if (dataChunk._dataMagic != DataChunk.Magic)
                throw new NotSupportedException("Specified wave file is not supported.");

            channels = riff._fmtChunk._numChannels;
            sampleRate = riff._fmtChunk._sampleRate;
            bitsPerSample = riff._fmtChunk._bitsPerSample;

            int dataSize = dataChunk._size;
            byte[] buffer = new byte[dataSize];
            int bytesRead = waveFileStream.Read(buffer, 0, dataSize);
            return buffer;
        }

        public static byte[] ReadChunk(
            Stream waveFileStream,
            out int channels,
            out int bitsPerSample,
            out int sampleRate)
        {
            if (waveFileStream is null)
                throw new ArgumentNullException(nameof(waveFileStream));

            RIFFHeader riff = waveFileStream.Read<RIFFHeader>();

            bool littleEndian = riff._riffMagic == RIFFLittleMagic;
            bool bigEndian = riff._riffMagic == RIFFBigMagic;

            if (!littleEndian && !bigEndian)
                throw new NotSupportedException("Specified stream is not a wave file.");

            Endian.SerializeOrder = bigEndian ? Endian.EOrder.Big : Endian.EOrder.Little;

            if (riff._waveMagic != WAVEMagic)
                throw new NotSupportedException("Specified stream is not a wave file.");

            if (riff._fmtChunk._fmtMagic != FmtChunk.Magic)
                throw new NotSupportedException("Specified wave file is not supported.");

            DataChunk dataChunk = riff.GetDataChunk();

            if (dataChunk._dataMagic != DataChunk.Magic)
                throw new NotSupportedException("Specified wave file is not supported.");

            channels = riff._fmtChunk._numChannels;
            sampleRate = riff._fmtChunk._sampleRate;
            bitsPerSample = riff._fmtChunk._bitsPerSample;

            int dataSize = dataChunk._size;
            byte[] buffer = new byte[dataSize];
            int bytesRead = waveFileStream.Read(buffer, 0, dataSize);
            return buffer;
        }
    }
}
