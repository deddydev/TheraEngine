using Extensions;
using System;
using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Memory
{
    public static class Endian
    {
        public enum EOrder : byte
        {
            Little = 0,
            Big = 1,
        }

        /// <summary>
        /// This is the endian that the engine de/serializer will write files and expect files to be written in.
        /// </summary>
        public static EOrder SerializeOrder { get; set; } = EOrder.Big;
        /// <summary>
        /// <see langword="true"/> if the de/serializer will read/write with big endian.
        /// </summary>
        public static bool SerializeBig
        {
            get => SerializeOrder == EOrder.Big;
            set => SerializeOrder = value ? EOrder.Big : EOrder.Little;
        }
        /// <summary>
        /// <see langword="true"/> if the de/serializer will read/write with little endian.
        /// </summary>
        public static bool SerializeLittle
        {
            get => SerializeOrder == EOrder.Little;
            set => SerializeOrder = value ? EOrder.Little : EOrder.Big;
        }

        /// <summary>
        /// This is the endian of the host OS.
        /// </summary>
        public static readonly EOrder SystemOrder;
        /// <summary>
        /// <see langword="true"/> if the host OS endian is big.
        /// </summary>
        public static bool SystemBig => SystemOrder == EOrder.Big;
        /// <summary>
        /// <see langword="true"/> if the host OS endian is little.
        /// </summary>
        public static bool SystemLittle => SystemOrder == EOrder.Little;
        
        static Endian()
        {
            int intValue = 1;
            unsafe { SystemOrder = *((byte*)&intValue) == 1 ? EOrder.Little : EOrder.Big; }
        }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct bshort
    {
        public short _data;

        public static implicit operator short(bshort val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator bshort(short val)
            => new bshort { _data = Endian.SerializeBig ? val.Reverse() : val };

        public short Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct bushort
    {
        public ushort _data;

        public static implicit operator ushort(bushort val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator bushort(ushort val)
            => new bushort { _data = Endian.SerializeBig ? val.Reverse() : val };

        public ushort Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bint
    {
        public int _data;

        public static implicit operator int(bint val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator bint(int val)
            => new bint { _data = Endian.SerializeBig ? val.Reverse() : val };

        public int Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr OffsetAddress
        {
            get => Address + Value;
            set => Value = value - Address;
        }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct buint
    {
        public uint _data;

        public static implicit operator uint(buint val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator buint(uint val)
            => new buint { _data = Endian.SerializeBig ? val.Reverse() : val };

        public uint Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr OffsetAddress
        {
            get => Address + Value;
            set => Value = (uint)value - (uint)Address;
        }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bfloat
    {
        public float _data;

        public static implicit operator float(bfloat val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator bfloat(float val)
            => new bfloat { _data = Endian.SerializeBig ? val.Reverse() : val };

        public float Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bdouble
    {
        public double _data;

        public static implicit operator double(bdouble val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator bdouble(double val)
            => new bdouble { _data = Endian.SerializeBig ? val.Reverse() : val };

        public double Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct blong
    {
        public long _data;

        public static implicit operator long(blong val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator blong(long val)
            => new blong { _data = Endian.SerializeBig ? val.Reverse() : val };

        public long Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr OffsetAddress
        {
            get => Address + Value;
            set => Value = (long)value - (long)Address;
        }

        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct bulong
    {
        public ulong _data;

        public static implicit operator ulong(bulong val)
            => Endian.SerializeBig ? val._data.Reverse() : val._data;
        public static implicit operator bulong(ulong val)
            => new bulong { _data = Endian.SerializeBig ? val.Reverse() : val };

        public ulong Value
        {
            get => this;
            set => this = value;
        }
        public override string ToString()
            => Value.ToString();

        public VoidPtr OffsetAddress
        {
            get => Address + Value;
            set => Value = (ulong)value - (ulong)Address;
        }

        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BUInt24
    {
        public byte _dat0, _dat1, _dat2;

        public uint Value
        {
            get => Endian.SerializeBig ?
                ((_dat0) | ((uint)_dat1 << 8) | (uint)_dat2 << 16) :
                (((uint)_dat0 << 16) | ((uint)_dat1 << 8) | _dat2);
            set
            {
                if (Endian.SerializeBig)
                {
                    _dat0 = (byte)((value) & 0xFF);
                    _dat1 = (byte)((value >> 8) & 0xFF);
                    _dat2 = (byte)((value >> 16) & 0xFF);
                }
                else
                {
                    _dat2 = (byte)((value) & 0xFF);
                    _dat1 = (byte)((value >> 8) & 0xFF);
                    _dat0 = (byte)((value >> 16) & 0xFF);
                }
            }
        }
        
        public static implicit operator uint(BUInt24 val) { return val.Value; }
        public static implicit operator BUInt24(uint val) { return new BUInt24(val); }

        public static explicit operator int(BUInt24 val) { return (int)val.Value; }
        public static explicit operator BUInt24(int val) { return new BUInt24((uint)val); }

        public static implicit operator UInt24(BUInt24 val) { return new UInt24(val.Value); }
        public static implicit operator BUInt24(UInt24 val) { return new BUInt24(val.Value); }

        public BUInt24(uint value)
        {
            _dat2 = _dat1 = _dat0 = 0;
            Value = value;
        }

        public BUInt24(byte v0, byte v1, byte v2)
        {
            _dat2 = v2;
            _dat1 = v1;
            _dat0 = v0;
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct BInt24
    {
        public byte _dat0, _dat1, _dat2;

        public int Value
        {
            get => Endian.SerializeBig ? 
                ((_dat0) | (_dat1 << 8) | _dat2 << 16) :
                ((_dat0 << 16) | (_dat1 << 8) | _dat2);
            set
            {
                if (Endian.SerializeBig)
                {
                    _dat0 = (byte)((value) & 0xFF);
                    _dat1 = (byte)((value >> 8) & 0xFF);
                    _dat2 = (byte)((value >> 16) & 0xFF);
                }
                else
                {
                    _dat2 = (byte)((value) & 0xFF);
                    _dat1 = (byte)((value >> 8) & 0xFF);
                    _dat0 = (byte)((value >> 16) & 0xFF);
                }
            }
        }
        
        public static implicit operator int(BInt24 val) => val.Value;
        public static implicit operator BInt24(int val) => new BInt24(val);

        public static implicit operator Int24(BInt24 val) => val.Value;
        public static implicit operator BInt24(Int24 val) => new BInt24(val);

        public BInt24(int value)
        {
            _dat2 = _dat1 = _dat0 = 0;
            Value = value;
        }

        public BInt24(byte v0, byte v1, byte v2)
        {
            _dat2 = v2;
            _dat1 = v1;
            _dat0 = v0;
        }

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BVec2
    {
        public const int Size = 8;

        public bfloat _x;
        public bfloat _y;

        public BVec2(float x, float y) { _x = x; _y = y; }

        public override string ToString() { return String.Format("({0}, {1})", (float)_x, (float)_y); }

        public static implicit operator Vec2(BVec2 v) { return new Vec2(v._x, v._y); }
        public static implicit operator BVec2(Vec2 v) { return new BVec2(v.X, v.Y); }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BVec3
    {
        public const int Size = 12;

        public bfloat _x;
        public bfloat _y;
        public bfloat _z;

        public BVec3(float x, float y, float z) { _x = x; _y = y; _z = z; }

        public override string ToString() { return String.Format("({0}, {1}, {2})", (float)_x, (float)_y, (float)_z); }

        public static implicit operator Vec3(BVec3 v) { return new Vec3(v._x, v._y, v._z); }
        public static implicit operator BVec3(Vec3 v) { return new BVec3(v.X, v.Y, v.Z); }

        public static implicit operator Vec4(BVec3 v) { return new Vec4(v._x, v._y, v._z, 1); }
        public static implicit operator BVec3(Vec4 v) { return new BVec3(v.X / v.W, v.Y / v.W, v.Z / v.W); }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BVec4
    {
        public bfloat _x;
        public bfloat _y;
        public bfloat _z;
        public bfloat _w;

        public BVec4(float x, float y, float z, float w) { _x = x; _y = y; _z = z; _w = w; }

        public override string ToString() { return String.Format("({0}, {1}, {2}, {3})", (float)_x, (float)_y, (float)_z, (float)_w); }

        public static implicit operator Vec4(BVec4 v) { return new Vec4(v._x, v._y, v._z, v._w); }
        public static implicit operator BVec4(Vec4 v) { return new BVec4(v.X, v.Y, v.Z, v.W); }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BMatrix43
    {
        fixed float _data[12];

        public bfloat* Data { get { fixed (float* ptr = _data) return (bfloat*)ptr; } }

        public float this[int x, int y]
        {
            get { return Data[(y << 2) + x]; }
            set { Data[(y << 2) + x] = value; }
        }
        public float this[int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        public override string ToString()
        {
            return String.Format("({0},{1},{2},{3})({4},{5},{6},{7})({8},{9},{10},{11})", this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10], this[11]);
        }

        public static implicit operator Matrix4(BMatrix43 bm)
        {
            Matrix4 m;

            bfloat* sPtr = (bfloat*)&bm;
            float* dPtr = (float*)&m;

            dPtr[0] = sPtr[0];
            dPtr[1] = sPtr[4];
            dPtr[2] = sPtr[8];
            dPtr[3] = 0.0f;
            dPtr[4] = sPtr[1];
            dPtr[5] = sPtr[5];
            dPtr[6] = sPtr[9];
            dPtr[7] = 0.0f;
            dPtr[8] = sPtr[2];
            dPtr[9] = sPtr[6];
            dPtr[10] = sPtr[10];
            dPtr[11] = 0.0f;
            dPtr[12] = sPtr[3];
            dPtr[13] = sPtr[7];
            dPtr[14] = sPtr[11];
            dPtr[15] = 1.0f;

            return m;
        }

        public static implicit operator BMatrix43(Matrix4 m)
        {
            BMatrix43 bm;

            bfloat* dPtr = (bfloat*)&bm;
            float* sPtr = (float*)&m;

            dPtr[0] = sPtr[0];
            dPtr[1] = sPtr[4];
            dPtr[2] = sPtr[8];
            dPtr[3] = sPtr[12];
            dPtr[4] = sPtr[1];
            dPtr[5] = sPtr[5];
            dPtr[6] = sPtr[9];
            dPtr[7] = sPtr[13];
            dPtr[8] = sPtr[2];
            dPtr[9] = sPtr[6];
            dPtr[10] = sPtr[10];
            dPtr[11] = sPtr[14];

            return bm;
        }

        public static implicit operator OpenTK.Matrix3x4(BMatrix43 bm)
        {
            OpenTK.Matrix3x4 m = new OpenTK.Matrix3x4();
            float* dPtr = (float*)&m;
            bfloat* sPtr = (bfloat*)&bm;
            for (int i = 0; i < 12; i++)
                dPtr[i] = sPtr[i];
            return m;
        }

        public static implicit operator BMatrix43(OpenTK.Matrix3x4 m)
        {
            BMatrix43 bm = new BMatrix43();
            bfloat* dPtr = (bfloat*)&bm;
            float* sPtr = (float*)&m;
            for (int i = 0; i < 12; i++)
                dPtr[i] = sPtr[i];
            return bm;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BMatrix4
    {
        fixed float _data[16];

        public bfloat* Data { get { fixed (float* ptr = _data) return (bfloat*)ptr; } }

        public float this[int x, int y]
        {
            get { return Data[(y << 2) + x]; }
            set { Data[(y << 2) + x] = value; }
        }
        public float this[int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        public override string ToString()
        {
            return String.Format("({0},{1},{2},{3})({4},{5},{6},{7})({8},{9},{10},{11})({12},{13},{14},{15})", this[0], this[1], this[2], this[3], this[4], this[5], this[6], this[7], this[8], this[9], this[10], this[11], this[12], this[13], this[14], this[15]);
        }

        public static implicit operator Matrix4(BMatrix4 bm)
        {
            Matrix4 m = new Matrix4();
            float* dPtr = (float*)&m;
            bfloat* sPtr = (bfloat*)&bm;
            for (int i = 0; i < 16; i++)
                dPtr[i] = sPtr[i];
            return m;
        }

        public static implicit operator BMatrix4(Matrix4 m)
        {
            BMatrix4 bm = new BMatrix4();
            bfloat* dPtr = (bfloat*)&bm;
            float* sPtr = (float*)&m;
            for (int i = 0; i < 16; i++)
                dPtr[i] = sPtr[i];
            return bm;
        }
    }
}
