using System;
using System.Runtime.InteropServices;
using TheraEngine.Core.Memory;

namespace TheraEngine.Core.Files.Serialization
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct FileCommonHeader
    {
        public const int Size = 0x10;
        public const string Magic = "THRA";

        public bint _magic;
        public Bin8 _flags;
        public byte _languages;
        public bushort _pad2;
        public bint _stringTableLength;
        public bint _sharedObjectsCount;
        //public fixed byte _hash[0x20];

        public void WriteMagic() => Magic.Write(_magic.Address, false);
        //public void WriteHash(byte[] integrityHash)
        //{
        //    for (int i = 0; i < 0x20; ++i)
        //        _hash[i] = integrityHash[i];
        //}

        public bool Encrypted
        {
            get => _flags[0];
            set => _flags[0] = value;
        }
        public bool Compressed
        {
            get => _flags[1];
            set => _flags[1] = value;
        }
        public Endian.EOrder Endian
        {
            get => _flags[2] ? Memory.Endian.EOrder.Big : Memory.Endian.EOrder.Little;
            set => _flags[2] = (int)value != 0;
        }
        public StringTableHeader* Strings => (StringTableHeader*)(Address + Size);
        public VoidPtr Data => (VoidPtr)Strings + _stringTableLength;
        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct StringTableHeader
        {
            public const int Size = 0x8;

            public bint _encodingCodePage;
            public Bin8 _flags; //Use for localization information?
            public BUInt24 _stringCount;
            
            public int StringLengthSize
            {
                get => _flags[0, 2] + 1;
                set => _flags[0, 2] = (byte)(value.Clamp(1, 4) - 1);
            }
            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        }
    }
}
