using System;
using System.Runtime.InteropServices;

namespace TheraEngine.Core.Memory
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VoidPtr
    {
        public void* _address;

        #region Primitive Types
        public byte Byte
        {
            get => *(byte*)_address;
            set => *(byte*)_address = value;
        }
        public byte GetByte() => Byte;
        public void SetByte(byte i) => Byte = i;
        public sbyte SByte
        {
            get => *(sbyte*)_address;
            set => *(sbyte*)_address = value;
        }
        public sbyte GetSByte() => SByte;
        public void SetSByte(sbyte i) => SByte = i;
        public ushort UShort
        {
            get => *(bushort*)_address;
            set => *(bushort*)_address = value;
        }
        public ushort GetUShort() => UShort;
        public void SetUShort(ushort i) => UShort = i;
        public short Short
        {
            get => *(bshort*)_address;
            set => *(bshort*)_address = value;
        }
        public short GetShort() => Short;
        public void SetShort(short i) => Short = i;
        public uint UInt
        {
            get => *(buint*)_address;
            set => *(buint*)_address = value;
        }
        public uint GetUInt() => UInt;
        public void SetUInt(uint i) => UInt = i;
        public int Int
        {
            get => *(bint*)_address;
            set => *(bint*)_address = value;
        }
        public int GetInt() => Int;
        public void SetInt(int i) => Int = i;
        public ulong ULong
        {
            get => *(bulong*)_address;
            set => *(bulong*)_address = value;
        }
        public ulong GetULong() => ULong;
        public void SetULong(ulong i) => ULong = i;
        public long Long
        {
            get => *(blong*)_address;
            set => *(blong*)_address = value;
        }
        public long GetLong() => Long;
        public void SetLong(long i) => Long = i;
        public float Float
        {
            get => *(bfloat*)_address;
            set => *(bfloat*)_address = value;
        }
        public float GetFloat() => Float;
        public void SetFloat(float i) => Float = i;
        public double Double
        {
            get => *(bdouble*)_address;
            set => *(bdouble*)_address = value;
        }
        public double GetDouble() => Double;
        public void GetDouble(double i) => Double = i;
        public BInt24 Int24
        {
            get => *(BInt24*)_address;
            set => *(BInt24*)_address = value;
        }
        public BInt24 GetInt24() => Int24;
        public void SetInt24(BInt24 i) => Int24 = i;
        public BUInt24 UInt24
        {
            get => *(BUInt24*)_address;
            set => *(BUInt24*)_address = value;
        }
        public BUInt24 GetUInt24() => UInt24;
        public void SetUInt24(BUInt24 i) => UInt24 = i;
        #endregion

        #region Operators
        public static int operator -(VoidPtr p1, VoidPtr p2) { return checked((int)((byte*)p1._address - (byte*)p2._address)); }
        
        public static VoidPtr operator +(VoidPtr p1, uint addr) { return new VoidPtr() { _address = ((byte*)p1._address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, uint addr) { return new VoidPtr() { _address = ((byte*)p1._address - addr) }; }

        public static VoidPtr operator +(VoidPtr p1, int addr) { return new VoidPtr() { _address = ((byte*)p1._address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, int addr) { return new VoidPtr() { _address = ((byte*)p1._address - addr) }; }

        public static VoidPtr operator +(VoidPtr p1, ulong addr) { return new VoidPtr() { _address = ((byte*)p1._address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, ulong addr) { return new VoidPtr() { _address = ((byte*)p1._address - addr) }; }

        public static VoidPtr operator +(VoidPtr p1, long addr) { return new VoidPtr() { _address = ((byte*)p1._address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, long addr) { return new VoidPtr() { _address = ((byte*)p1._address - addr) }; }

        public static bool operator >(VoidPtr p1, VoidPtr p2)
            => p1._address > p2._address;
        public static bool operator <(VoidPtr p1, VoidPtr p2)
            => p1._address < p2._address;
        public static bool operator >=(VoidPtr p1, VoidPtr p2)
            => p1._address >= p2._address;
        public static bool operator <=(VoidPtr p1, VoidPtr p2)
            => p1._address <= p2._address;
        public static bool operator ==(VoidPtr p1, VoidPtr p2)
            => p1._address == p2._address;
        public static bool operator !=(VoidPtr p1, VoidPtr p2)
            => p1._address != p2._address;
        #endregion

        #region Casts
        public static implicit operator bool(VoidPtr ptr) => ptr._address != null;

        public static implicit operator void* (VoidPtr ptr) => ptr._address;
        public static implicit operator VoidPtr(void* ptr) => new VoidPtr() { _address = ptr };

        public static implicit operator uint(VoidPtr ptr) => checked((uint)ptr._address);
        public static implicit operator VoidPtr(uint ptr) => new VoidPtr() { _address = (void*)ptr };
        public static implicit operator int(VoidPtr ptr) => checked((int)ptr._address);
        public static implicit operator VoidPtr(int ptr) => new VoidPtr() { _address = (void*)ptr };

        public static implicit operator ulong(VoidPtr ptr) => (ulong)ptr._address;
        public static implicit operator VoidPtr(ulong ptr) => new VoidPtr() { _address = (void*)ptr };
        public static implicit operator long(VoidPtr ptr) => (long)ptr._address;
        public static implicit operator VoidPtr(long ptr) => new VoidPtr() { _address = (void*)ptr };

        public static implicit operator VoidPtr(IntPtr ptr) => new VoidPtr() { _address = (void*)ptr };
        public static implicit operator IntPtr(VoidPtr ptr) => (IntPtr)ptr._address;

        //public static implicit operator sbyte*(VoidPtr ptr) { return (sbyte*)ptr.address; }
        //public static implicit operator VoidPtr(sbyte* ptr) { return new VoidPtr() { address = ptr }; }

        //public static implicit operator byte* (VoidPtr ptr) { return (byte*)ptr.address; }
        //public static implicit operator VoidPtr(byte* ptr) { return new VoidPtr() { address = ptr }; }
        #endregion
        
        #region Incremental Writing Methods
        public void WriteByte(byte value, bool incrementPointer = true)
        {
            Byte = value;
            if (incrementPointer)
                _address = (byte*)_address + 1;
        }
        public void WriteSByte(sbyte value, bool incrementPointer = true)
        {
            SByte = value;
            if (incrementPointer)
                _address = (sbyte*)_address + 1;
        }
        public void WriteShort(short value, bool incrementPointer = true)
        {
            Short = value;
            if (incrementPointer)
                _address = (bshort*)_address + 1;
        }
        public void WriteUShort(ushort value, bool incrementPointer = true)
        {
            UShort = value;
            if (incrementPointer)
                _address = (bushort*)_address + 1;
        }
        public void WriteInt(int value, bool incrementPointer = true)
        {
            Int = value;
            if (incrementPointer)
                _address = (bint*)_address + 1;
        }
        public void WriteUInt(uint value, bool incrementPointer = true)
        {
            UInt = value;
            if (incrementPointer)
                _address = (buint*)_address + 1;
        }
        public void WriteLong(long value, bool incrementPointer = true)
        {
            Long = value;
            if (incrementPointer)
                _address = (blong*)_address + 1;
        }
        public void WriteULong(ulong value, bool incrementPointer = true)
        {
            ULong = value;
            if (incrementPointer)
                _address = (bulong*)_address + 1;
        }
        public void WriteFloat(float value, bool incrementPointer = true)
        {
            Float = value;
            if (incrementPointer)
                _address = (bfloat*)_address + 1;
        }
        public void WriteDouble(double value, bool incrementPointer = true)
        {
            Double = value;
            if (incrementPointer)
                _address = (bdouble*)_address + 1;
        }
        #endregion

        public VoidPtr this[int count, int stride] => this + (count * stride);
        public string GetString(int offset = 0) => new string((sbyte*)this + offset);
        public void MovePointer(int byteCount = 1) => _address = (byte*)_address + byteCount;

        public static void Swap(float* p1, float* p2) { float f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(int* p1, int* p2) { int f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(short* p1, short* p2) { short f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(ushort* p1, ushort* p2) { ushort f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(byte* p1, byte* p2) { byte f = *p1; *p1 = *p2; *p2 = f; }

        public override int GetHashCode() => (int)_address;
        public override bool Equals(object obj) => base.Equals(obj);
    }
}
