using System.Runtime.InteropServices;
using CustomEngine.Files.Serialization;

namespace System
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VoidPtr
    {
        //address
        public void* address;

        public byte Byte
        {
            get => *(byte*)address;
            set => *(byte*)address = value;
        }
        public sbyte SByte
        {
            get => *(sbyte*)address;
            set => *(sbyte*)address = value;
        }
        public ushort UShort
        {
            get => *(bushort*)address;
            set => *(bushort*)address = value;
        }
        public short Short
        {
            get => *(bshort*)address;
            set => *(bshort*)address = value;
        }
        public uint UInt
        {
            get => *(buint*)address;
            set => *(buint*)address = value;
        }
        public int Int
        {
            get => *(bint*)address;
            set => *(bint*)address = value;
        }
        public ulong ULong
        {
            get => *(bulong*)address;
            set => *(bulong*)address = value;
        }
        public long Long
        {
            get => *(blong*)address;
            set => *(blong*)address = value;
        }
        public float Float
        {
            get => *(bfloat*)address;
            set => *(bfloat*)address = value;
        }
        public double Double
        {
            get => *(bdouble*)address;
            set => *(bdouble*)address = value;
        }

        #region Operators
        public static int operator -(VoidPtr p1, VoidPtr p2) { return checked((int)((byte*)p1.address - (byte*)p2.address)); }
        
        public static VoidPtr operator +(VoidPtr p1, uint addr) { return new VoidPtr() { address = ((byte*)p1.address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, uint addr) { return new VoidPtr() { address = ((byte*)p1.address - addr) }; }

        public static VoidPtr operator +(VoidPtr p1, int addr) { return new VoidPtr() { address = ((byte*)p1.address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, int addr) { return new VoidPtr() { address = ((byte*)p1.address - addr) }; }

        public static VoidPtr operator +(VoidPtr p1, ulong addr) { return new VoidPtr() { address = ((byte*)p1.address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, ulong addr) { return new VoidPtr() { address = ((byte*)p1.address - addr) }; }

        public static VoidPtr operator +(VoidPtr p1, long addr) { return new VoidPtr() { address = ((byte*)p1.address + addr) }; }
        public static VoidPtr operator -(VoidPtr p1, long addr) { return new VoidPtr() { address = ((byte*)p1.address - addr) }; }

        public static bool operator >(VoidPtr p1, VoidPtr p2)
            => p1.address > p2.address;
        public static bool operator <(VoidPtr p1, VoidPtr p2)
            => p1.address < p2.address;
        public static bool operator >=(VoidPtr p1, VoidPtr p2)
            => p1.address >= p2.address;
        public static bool operator <=(VoidPtr p1, VoidPtr p2)
            => p1.address <= p2.address;
        public static bool operator ==(VoidPtr p1, VoidPtr p2)
            => p1.address == p2.address;
        public static bool operator !=(VoidPtr p1, VoidPtr p2)
            => p1.address != p2.address;
        #endregion

        #region Casts
        public static implicit operator bool(VoidPtr ptr) 
            => ptr.address != null;

        public static implicit operator void* (VoidPtr ptr) { return ptr.address; }
        public static implicit operator VoidPtr(void* ptr) { return new VoidPtr() { address = ptr }; }

        public static implicit operator uint(VoidPtr ptr) { return checked((uint)ptr.address); }
        public static implicit operator VoidPtr(uint ptr) { return new VoidPtr() { address = (void*)ptr }; }
        public static implicit operator int(VoidPtr ptr) { return checked((int)ptr.address); }
        public static implicit operator VoidPtr(int ptr) { return new VoidPtr() { address = (void*)ptr }; }

        public static implicit operator ulong(VoidPtr ptr) { return (ulong)ptr.address; }
        public static implicit operator VoidPtr(ulong ptr) { return new VoidPtr() { address = (void*)ptr }; }
        public static implicit operator long(VoidPtr ptr) { return (long)ptr.address; }
        public static implicit operator VoidPtr(long ptr) { return new VoidPtr() { address = (void*)ptr }; }

        public static implicit operator VoidPtr(IntPtr ptr) { return new VoidPtr() { address = (void*)ptr }; }
        public static implicit operator IntPtr(VoidPtr ptr) { return (IntPtr)ptr.address; }

        //public static implicit operator sbyte*(VoidPtr ptr) { return (sbyte*)ptr.address; }
        //public static implicit operator VoidPtr(sbyte* ptr) { return new VoidPtr() { address = ptr }; }

        //public static implicit operator byte* (VoidPtr ptr) { return (byte*)ptr.address; }
        //public static implicit operator VoidPtr(byte* ptr) { return new VoidPtr() { address = ptr }; }
        #endregion

        public VoidPtr this[int count, int stride]
            => this + (count * stride);

        public string GetString(int offset = 0)
            => new string((sbyte*)this + offset);

        public void WriteByte(byte value, bool incrementPointer = true)
        {
            Byte = value;
            if (incrementPointer)
                address = (byte*)address + 1;
        }
        public void WriteSByte(sbyte value, bool incrementPointer = true)
        {
            SByte = value;
            if (incrementPointer)
                address = (sbyte*)address + 1;
        }
        public void WriteShort(short value, bool incrementPointer = true)
        {
            Short = value;
            if (incrementPointer)
                address = (bshort*)address + 1;
        }
        public void WriteUShort(ushort value, bool incrementPointer = true)
        {
            UShort = value;
            if (incrementPointer)
                address = (bushort*)address + 1;
        }
        public void WriteInt(int value, bool incrementPointer = true)
        {
            Int = value;
            if (incrementPointer)
                address = (bint*)address + 1;
        }
        public void WriteUInt(uint value, bool incrementPointer = true)
        {
            UInt = value;
            if (incrementPointer)
                address = (buint*)address + 1;
        }
        public void WriteLong(long value, bool incrementPointer = true)
        {
            Long = value;
            if (incrementPointer)
                address = (blong*)address + 1;
        }
        public void WriteULong(ulong value, bool incrementPointer = true)
        {
            ULong = value;
            if (incrementPointer)
                address = (bulong*)address + 1;
        }
        public void WriteFloat(float value, bool incrementPointer = true)
        {
            Float = value;
            if (incrementPointer)
                address = (bfloat*)address + 1;
        }
        public void WriteDouble(double value, bool incrementPointer = true)
        {
            Double = value;
            if (incrementPointer)
                address = (bdouble*)address + 1;
        }
        
        public override int GetHashCode()
            => (int)address;
        public override bool Equals(object obj)
            => base.Equals(obj);

        public static void Swap(float* p1, float* p2) { float f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(int* p1, int* p2) { int f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(short* p1, short* p2) { short f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(ushort* p1, ushort* p2) { ushort f = *p1; *p1 = *p2; *p2 = f; }
        public static void Swap(byte* p1, byte* p2) { byte f = *p1; *p1 = *p2; *p2 = f; }

        public void MovePointer(int byteCount = 1)
        {
            address = (byte*)address + byteCount;
        }
    }
}
