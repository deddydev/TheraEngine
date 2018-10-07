using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    public partial class TSerializer
    {
        private class WriterBinary : AbstractWriter
        {
            public WriterBinary(TSerializer owner) : base(owner)
            {

            }
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public unsafe struct FileCommonHeader
            {
                public const int Size = 0x30;

                public byte _encrypted;
                public byte _compressed;
                public bushort _endian;
                public bint _stringTableLength;
                public bint _fileLength;
                public bint _typeNameStringOffset;
                public fixed byte _hash[0x20];

                public bool Encrypted
                {
                    get => _encrypted != 0;
                    set => _encrypted = (byte)(value ? 1 : 0);
                }
                public bool Compressed
                {
                    get => _compressed != 0;
                    set => _compressed = (byte)(value ? 1 : 0);
                }
                public Endian.EOrder Endian
                {
                    get => (Endian.EOrder)(ushort)_endian;
                    set => _endian = (ushort)value;
                }
                public VoidPtr Strings => Address + Size;
                public VoidPtr Data => Strings + _stringTableLength;
                public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
            }
        }
    }
}
