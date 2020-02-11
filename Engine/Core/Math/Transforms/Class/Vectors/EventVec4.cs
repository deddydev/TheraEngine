using System.ComponentModel;
using System.Xml.Serialization;
using TheraEngine;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;
using static System.Math;
using static TheraEngine.Core.Maths.TMath;

namespace System
{
    public delegate Vec4 DelGetVec4Value(float delta);

    /// <summary>
    /// A wrapper class for <see cref="Vec4"/> that supports events and synchronization when its values change.
    /// </summary>
    public unsafe class EventVec4 : TObject, IEquatable<EventVec4>, IUniformable4Float, IBufferable, ISerializableString
    {
        public EventVec4() { }

        public EventVec4(Vec4 xyzw) => _data = xyzw;
        public EventVec4(float x, float y, float z, float w) => _data = new Vec4(x, y, z, w);
        public EventVec4(float xyzw) : this(xyzw, xyzw, xyzw, xyzw) { }

        public EventVec4(Vec3 xyz) : this(xyz.X, xyz.Y, xyz.Z, 0.0f) { }
        public EventVec4(Vec3 xyz, float w) : this(xyz.X, xyz.Y, xyz.Z, w) { }
        public EventVec4(float x, Vec3 yzw) : this(x, yzw.X, yzw.Y, yzw.Z) { }
        
        public EventVec4(Vec2 xy) : this(xy.X, xy.Y, 0.0f, 0.0f) { }
        public EventVec4(Vec2 xy, Vec2 zw) : this(xy.X, xy.Y, zw.X, zw.Y) { }
        public EventVec4(Vec2 xy, float z, float w) : this(xy.X, xy.Y, z, w) { }
        public EventVec4(float x, float y, Vec2 zw) : this(x, y, zw.X, zw.Y) { }
        public EventVec4(float x, Vec2 yz, float w) : this(x, yz.X, yz.Y, w) { }

        public static EventVec4 UnitX => new EventVec4(1.0f, 0.0f, 0.0f, 0.0f);
        public static EventVec4 UnitY => new EventVec4(0.0f, 1.0f, 0.0f, 0.0f);
        public static EventVec4 UnitZ => new EventVec4(0.0f, 0.0f, 1.0f, 0.0f);
        public static EventVec4 UnitW => new EventVec4(0.0f, 0.0f, 0.0f, 1.0f);
        public static EventVec4 Zero => new EventVec4(0.0f);
        public static EventVec4 Half => new EventVec4(0.5f);
        public static EventVec4 One => new EventVec4(1.0f);
        public static EventVec4 Min => new EventVec4(float.MinValue);
        public static EventVec4 Max => new EventVec4(float.MaxValue);

        public event Action XChanged;
        public event Action YChanged;
        public event Action ZChanged;
        public event Action WChanged;
        public event DelFloatChange XValueChanged;
        public event DelFloatChange YValueChanged;
        public event DelFloatChange ZValueChanged;
        public event DelFloatChange WValueChanged;
        public event Action Changed;

        private int _recursiveUpdates = 0;
        private float _oldX, _oldY, _oldZ, _oldW;
        [TSerialize("XYZW", NodeType = ENodeType.ElementContent)]
        private Vec4 _data;

        public EventVec4 _syncX, _syncY, _syncZ, _syncW, _syncAll;

        public void Reset()
        {
            _data = Vec4.Zero;

            _oldX = 0.0f;
            _oldY = 0.0f;
            _oldZ = 0.0f;
            _oldW = 0.0f;

            _syncX = null;
            _syncY = null;
            _syncZ = null;
            _syncW = null;
            _syncAll = null;

            XChanged = null;
            YChanged = null;
            ZChanged = null;
            WChanged = null;

            XValueChanged = null;
            YValueChanged = null;
            ZValueChanged = null;
            WValueChanged = null;

            Changed = null;
        }

        /// <summary>
        /// Sets the internal <see cref="Vec4"/> value and does not fire any events.
        /// </summary>
        public void SetRawNoUpdate(Vec4 raw)
            => _data = raw;

        [Browsable(false)]
        public Vec4 Raw
        {
            get => _data;
            set
            {
                //if (OnPropertyChanging())
                //    return;

                BeginUpdate();
                try
                {
                    //OnPropertyChanging(nameof(X));
                    //OnPropertyChanging(nameof(Y));
                    //OnPropertyChanging(nameof(Z));
                    //OnPropertyChanging(nameof(W));
                    if (Set(ref _data, value))
                        OnPropertiesChanged(nameof(X), nameof(Y), nameof(Z), nameof(W));
                }
                finally
                {
                    EndUpdate();
                }

                OnPropertyChanged();
            }
        }
        public float X
        {
            get => _data.X;
            set
            {
                if (value == _data.X)
                    return;

                //if (OnPropertyChanging())
                //    return;

                BeginUpdate();
                try
                {
                    _data.X = value;
                }
                finally
                {
                    EndUpdate();
                }

                OnPropertyChanged();
            }
        }
        public float Y
        {
            get => _data.Y;
            set
            {
                if (value == _data.Y)
                    return;

                //if (OnPropertyChanging())
                //    return;

                BeginUpdate();
                try
                {
                    _data.Y = value;
                }
                finally
                {
                    EndUpdate();
                }

                OnPropertyChanged();
            }
        }
        public float Z
        {
            get => _data.Z;
            set
            {
                if (value == _data.Z)
                    return;

                //if (OnPropertyChanging())
                //    return;

                BeginUpdate();
                try
                {
                    _data.Z = value;
                }
                finally
                {
                    EndUpdate();
                }

                OnPropertyChanged();
            }
        }
        public float W
        {
            get => _data.W;
            set
            {
                if (value == _data.W)
                    return;

                //if (OnPropertyChanging())
                //    return;

                BeginUpdate();
                try
                {
                    _data.W = value;
                }
                finally
                {
                    EndUpdate();
                }

                OnPropertyChanged();
            }
        }

        public float* Data => _data.Data;
        public VoidPtr Address => _data.Address;
        public DataBuffer.EComponentType ComponentType => DataBuffer.EComponentType.Float;
        public int ComponentCount => 4;
        bool IBufferable.Normalize => false;
        public void Write(VoidPtr address) => _data.Write(address);
        public void Read(VoidPtr address)
        {
            BeginUpdate();
            try
            {
                _data.Read(address);
            }
            finally
            {
                EndUpdate();
            }
        }
        public void SyncXFrom(EventVec4 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncX != null)
            {
                _syncX.XValueChanged -= Other_XChanged;
                _syncX = null;
            }
            if (other != null)
            {
                other.XValueChanged += Other_XChanged;
                _syncX = other;
                X = other.X;
            }
        }
        public void SyncYFrom(EventVec4 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncY != null)
            {
                _syncY.YValueChanged -= Other_YChanged;
                _syncY = null;
            }
            if (other != null)
            {
                other.YValueChanged += Other_YChanged;
                _syncY = other;
                Y = other.Y;
            }
        }
        public void SyncZFrom(EventVec4 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncZ != null)
            {
                _syncZ.ZValueChanged -= Other_ZChanged;
                _syncZ = null;
            }
            if (other != null)
            {
                other.ZValueChanged += Other_ZChanged;
                _syncZ = other;
                Z = other.Z;
            }
        }
        public void SyncWFrom(EventVec4 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncW != null)
            {
                _syncW.WValueChanged -= Other_WChanged;
                _syncW = null;
            }
            if (other != null)
            {
                other.WValueChanged += Other_WChanged;
                _syncW = other;
                W = other.W;
            }
        }
        public void SyncFrom(EventVec4 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncX != null)
            {
                _syncX.XValueChanged -= Other_XChanged;
                _syncX = null;
            }
            if (_syncY != null)
            {
                _syncY.YValueChanged -= Other_YChanged;
                _syncY = null;
            }
            if (_syncZ != null)
            {
                _syncZ.ZValueChanged -= Other_ZChanged;
                _syncZ = null;
            }
            if (_syncW != null)
            {
                _syncW.WValueChanged -= Other_WChanged;
                _syncW = null;
            }
            if (other != null)
            {
                other.Changed += Other_Changed;
                _syncAll = other;
                Raw = other.Raw;
            }
        }
        public void StopSynchronization()
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            if (_syncX != null)
            {
                _syncX.XValueChanged -= Other_XChanged;
                _syncX = null;
            }
            if (_syncY != null)
            {
                _syncY.YValueChanged -= Other_YChanged;
                _syncY = null;
            }
            if (_syncZ != null)
            {
                _syncZ.ZValueChanged -= Other_ZChanged;
                _syncZ = null;
            }
            if (_syncW != null)
            {
                _syncW.WValueChanged -= Other_WChanged;
                _syncW = null;
            }
        }

        private void Other_Changed() => Raw = _syncAll.Raw;
        private void Other_XChanged(float newValue, float oldValue) => X = newValue;
        private void Other_YChanged(float newValue, float oldValue) => Y = newValue;
        private void Other_ZChanged(float newValue, float oldValue) => Z = newValue;
        private void Other_WChanged(float newValue, float oldValue) => W = newValue;

        private void BeginUpdate()
        {
            //Interlocked.Increment(ref _recursiveUpdates);
            _oldX = X;
            _oldY = Y;
            _oldZ = Z;
            _oldW = W;
        }
        private void EndUpdate()
        {
            //if (Interlocked.Decrement(ref _recursiveUpdates) > 0)
            //    return;

            float x = X, y = Y, z = Z, w = W;
            float ox = _oldX, oy = _oldY, oz = _oldZ, ow = _oldW;

            bool anyChanged = false;
            if (*(int*)&x != *(int*)&ox)
            {
                XChanged?.Invoke();
                XValueChanged?.Invoke(_oldX, X);
                anyChanged = true;
            }
            if (*(int*)&y != *(int*)&oy)
            {
                YChanged?.Invoke();
                YValueChanged?.Invoke(_oldY, Y);
                anyChanged = true;
            }
            if (*(int*)&z != *(int*)&oz)
            {
                ZChanged?.Invoke();
                ZValueChanged?.Invoke(_oldZ, Z);
                anyChanged = true;
            }
            if (*(int*)&w != *(int*)&ow)
            {
                WChanged?.Invoke();
                WValueChanged?.Invoke(_oldW, W);
                anyChanged = true;
            }
            if (anyChanged)
                Changed?.Invoke();
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        public float Length => (float)Sqrt(LengthSquared);
        public float LengthFast => 1.0f / InverseSqrtFast(LengthSquared);
        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

        public EventVec4 Normalized()
        {
            EventVec4 v = this;
            v.Normalize();
            return v;
        }
        public EventVec4 NormalizedFast()
        {
            EventVec4 v = this;
            v.NormalizeFast();
            return v;
        }
        public void Normalize()
        {
            BeginUpdate();
            try
            {
                _data.Normalize();
            }
            finally
            {
                EndUpdate();
            }
        }
        public void NormalizeFast()
        {
            BeginUpdate();
            try
            {
                _data.NormalizeFast();
            }
            finally
            {
                EndUpdate();
            }
        }

        public static EventVec4 ComponentMin(EventVec4 a, EventVec4 b) => new EventVec4(
                a.X < b.X ? a.X : b.X,
                a.Y < b.Y ? a.Y : b.Y,
                a.Z < b.Z ? a.Z : b.Z,
                a.W < b.W ? a.W : b.W);
        public static EventVec4 ComponentMax(EventVec4 a, EventVec4 b) => new EventVec4(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y,
                a.Z > b.Z ? a.Z : b.Z,
                a.W > b.W ? a.W : b.W);
        public static EventVec4 Clamp(EventVec4 vec, EventVec4 min, EventVec4 max) => new EventVec4(
                vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X,
                vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y,
                vec.X < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z,
                vec.Y < min.W ? min.W : vec.W > max.W ? max.W : vec.W);
        public static float Dot(EventVec4 left, EventVec4 right) => left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        public float Dot(EventVec4 right) => X * right.X + Y * right.Y + Z * right.Z + W * right.W;
        public static EventVec4 Lerp(EventVec4 a, EventVec4 b, float time) => a + (b - a) * time;
        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates
        /// </summary>
        /// <param name="a">First input Vector</param>
        /// <param name="b">Second input Vector</param>
        /// <param name="c">Third input Vector</param>
        /// <param name="u">First Barycentric Coordinate</param>
        /// <param name="v">Second Barycentric Coordinate</param>
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
        public static EventVec4 BaryCentric(EventVec4 a, EventVec4 b, EventVec4 c, float u, float v) => a + u * (b - a) + v * (c - a);
        /// <summary>Transform a Vector by the given Matrix</summary>
        public static EventVec4 operator *(EventVec4 vec, Matrix4 mat)
        {
            EventVec4 nv = new EventVec4();
            float* dPtr = nv.Data;
            float* sPtr = vec.Data;
            float* 
                row1 = (float*)&mat, 
                row2 = row1 + 4, 
                row3 = row2 + 4, 
                row4 = row3 + 4;

            for (int i = 0; i < 4; i++)
                dPtr[i] = 
                    row1[i] * sPtr[0] + 
                    row2[i] * sPtr[1] + 
                    row3[i] * sPtr[2] + 
                    row4[i] * sPtr[3];

            return nv;
        }
        /// <summary>Transform a Vector by the given Matrix using right-handed notation</summary>
        public static EventVec4 operator *(Matrix4 mat, EventVec4 vec)
        {
            EventVec4 nv = new EventVec4();
            float* dPtr = nv.Data;
            float* sPtr = vec.Data;
            Vec4* row = (Vec4*)&mat;

            for (int i = 0; i < 4; i++)
                dPtr[i] =
                    row[i].X * sPtr[0] +
                    row[i].Y * sPtr[1] +
                    row[i].Z * sPtr[2] +
                    row[i].W * sPtr[3];

            return nv;
        }

        public void SetXy(float x, float y)
        {
            BeginUpdate();
            X = x;
            Y = y;
            EndUpdate();
        }
        public void SetXz(float x, float z)
        {
            BeginUpdate();
            X = x;
            Z = z;
            EndUpdate();
        }
        public void SetXw(float x, float w)
        {
            BeginUpdate();
            X = x;
            W = w;
            EndUpdate();
        }
        public void SetYz(float y, float z)
        {
            BeginUpdate();
            Y = y;
            Z = z;
            EndUpdate();
        }
        public void SetYw(float y, float w)
        {
            BeginUpdate();
            Y = y;
            W = w;
            EndUpdate();
        }
        public void SetZw(float z, float w)
        {
            BeginUpdate();
            Z = z;
            W = w;
            EndUpdate();
        }
        public void SetXyz(float x, float y, float z)
        {
            BeginUpdate();
            X = x;
            Y = y;
            Z = z;
            EndUpdate();
        }
        public void SetXzw(float x, float z, float w)
        {
            BeginUpdate();
            X = x;
            Z = z;
            W = w;
            EndUpdate();
        }
        public void SetXyw(float x, float y, float w)
        {
            BeginUpdate();
            X = x;
            Y = y;
            W = w;
            EndUpdate();
        }
        public void SetYzw(float y, float z, float w)
        {
            BeginUpdate();
            Y = y;
            Z = z;
            W = w;
            EndUpdate();
        }
        public void SetXyzw(float x, float y, float z, float w)
        {
            BeginUpdate();
            X = x;
            Y = y;
            Z = z;
            W = w;
            EndUpdate();
        }

        [XmlIgnore]
        public Vec2 Xy
        {
            get { return new Vec2(X, Y); }
            set { SetXy(value.X, value.Y); }
        }
        [XmlIgnore]
        public Vec2 Xz
        {
            get { return new Vec2(X, Z); }
            set { SetXz(value.X, value.Y); }
        }
        [XmlIgnore]
        public Vec2 Xw
        {
            get { return new Vec2(X, W); }
            set { SetXw(value.X, value.Y); }
        }
        [XmlIgnore]
        public Vec2 Yx
        {
            get { return new Vec2(Y, X); }
            set { SetXy(value.Y, value.X); }
        }
        [XmlIgnore]
        public Vec2 Yz
        {
            get { return new Vec2(Y, Z); }
            set { SetYz(value.X, value.Y); }
        }
        [XmlIgnore]
        public Vec2 Yw
        {
            get { return new Vec2(Y, W); }
            set { SetYw(value.X, value.Y); }
        }
        [XmlIgnore]
        public Vec2 Zx
        {
            get { return new Vec2(Z, X); }
            set { SetXz(value.Y, value.X); }
        }
        [XmlIgnore]
        public Vec2 Zy
        {
            get { return new Vec2(Z, Y); }
            set { SetYz(value.Y, value.X); }
        }
        [XmlIgnore]
        public Vec2 Zw
        {
            get { return new Vec2(Z, W); }
            set { SetZw(value.X, value.Y); }
        }
        [XmlIgnore]
        public Vec2 Wx
        {
            get { return new Vec2(W, X); }
            set { SetXw(value.Y, value.X); }
        }
        [XmlIgnore]
        public Vec2 Wy
        {
            get { return new Vec2(W, Y); }
            set { SetYw(value.Y, value.X); }
        }
        [XmlIgnore]
        public Vec2 Wz
        {
            get { return new Vec2(W, Z); }
            set { SetZw(value.Y, value.X); }
        }
        [XmlIgnore]
        public Vec3 Xyz
        {
            get { return new Vec3(X, Y, Z); }
            set { SetXyz(value.X, value.Y, value.Z); }
        }
        [XmlIgnore]
        public Vec3 Xyw
        {
            get { return new Vec3(X, Y, W); }
            set { SetXyw(value.X, value.Y, value.Z); }
        }
        [XmlIgnore]
        public Vec3 Xzy
        {
            get { return new Vec3(X, Z, Y); }
            set { SetXyz(value.X, value.Z, value.Y); }
        }
        [XmlIgnore]
        public Vec3 Xzw
        {
            get { return new Vec3(X, Z, W); }
            set { SetXzw(value.X, value.Y, value.Z); }
        }
        [XmlIgnore]
        public Vec3 Xwy
        {
            get { return new Vec3(X, W, Y); }
            set { SetXyw(value.X, value.Z, value.Y); }
        }
        [XmlIgnore]
        public Vec3 Xwz
        {
            get { return new Vec3(X, W, Z); }
            set { SetXzw(value.X, value.Z, value.Y); }
        }
        [XmlIgnore]
        public Vec3 Yxz
        {
            get { return new Vec3(Y, X, Z); }
            set { SetXyz(value.Y, value.X, value.Z); }
        }
        [XmlIgnore]
        public Vec3 Yxw
        {
            get { return new Vec3(Y, X, W); }
            set { SetXyw(value.Y, value.X, value.Z); }
        }
        [XmlIgnore]
        public Vec3 Yzx
        {
            get { return new Vec3(Y, Z, X); }
            set { SetXyz(value.Y, value.Z, value.X); }
        }
        [XmlIgnore]
        public Vec3 Yzw
        {
            get { return new Vec3(Y, Z, W); }
            set { SetYzw(value.X, value.Y, value.Z); }
        }
        [XmlIgnore]
        public Vec3 Ywx
        {
            get { return new Vec3(Y, W, X); }
            set { BeginUpdate(); Y = value.X; W = value.Y; X = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Ywz
        {
            get { return new Vec3(Y, W, Z); }
            set { BeginUpdate(); Y = value.X; W = value.Y; Z = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Zxy
        {
            get { return new Vec3(Z, X, Y); }
            set { BeginUpdate(); Z = value.X; X = value.Y; Y = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Zxw
        {
            get { return new Vec3(Z, X, W); }
            set { BeginUpdate(); Z = value.X; X = value.Y; W = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Zyx
        {
            get { return new Vec3(Z, Y, X); }
            set { BeginUpdate(); Z = value.X; Y = value.Y; X = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Zyw
        {
            get { return new Vec3(Z, Y, W); }
            set { BeginUpdate(); Z = value.X; Y = value.Y; W = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Zwx
        {
            get { return new Vec3(Z, W, X); }
            set { BeginUpdate(); Z = value.X; W = value.Y; X = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Zwy
        {
            get { return new Vec3(Z, W, Y); }
            set { BeginUpdate(); Z = value.X; W = value.Y; Y = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Wxy
        {
            get { return new Vec3(W, X, Y); }
            set { BeginUpdate(); W = value.X; X = value.Y; Y = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Wxz
        {
            get { return new Vec3(W, X, Z); }
            set { BeginUpdate(); W = value.X; X = value.Y; Z = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Wyx
        {
            get { return new Vec3(W, Y, X); }
            set { BeginUpdate(); W = value.X; Y = value.Y; X = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Wyz
        {
            get { return new Vec3(W, Y, Z); }
            set { BeginUpdate(); W = value.X; Y = value.Y; Z = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Wzx
        {
            get { return new Vec3(W, Z, X); }
            set { BeginUpdate(); W = value.X; Z = value.Y; X = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public Vec3 Wzy
        {
            get { return new Vec3(W, Z, Y); }
            set { BeginUpdate(); W = value.X; Z = value.Y; Y = value.Z; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Xywz
        {
            get { return new EventVec4(X, Y, W, Z); }
            set { BeginUpdate(); X = value.X; Y = value.Y; W = value.Z; Z = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Xzyw
        {
            get { return new EventVec4(X, Z, Y, W); }
            set { BeginUpdate(); X = value.X; Z = value.Y; Y = value.Z; W = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Xzwy
        {
            get { return new EventVec4(X, Z, W, Y); }
            set { BeginUpdate(); X = value.X; Z = value.Y; W = value.Z; Y = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Xwyz
        {
            get { return new EventVec4(X, W, Y, Z); }
            set { BeginUpdate(); X = value.X; W = value.Y; Y = value.Z; Z = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Xwzy
        {
            get { return new EventVec4(X, W, Z, Y); }
            set { BeginUpdate(); X = value.X; W = value.Y; Z = value.Z; Y = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Yxzw
        {
            get { return new EventVec4(Y, X, Z, W); }
            set { BeginUpdate(); Y = value.X; X = value.Y; Z = value.Z; W = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Yxwz
        {
            get { return new EventVec4(Y, X, W, Z); }
            set { BeginUpdate(); Y = value.X; X = value.Y; W = value.Z; Z = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Yyzw
        {
            get { return new EventVec4(Y, Y, Z, W); }
            set { BeginUpdate(); X = value.X; Y = value.Y; Z = value.Z; W = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Yywz
        {
            get { return new EventVec4(Y, Y, W, Z); }
            set { BeginUpdate(); X = value.X; Y = value.Y; W = value.Z; Z = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Yzxw
        {
            get { return new EventVec4(Y, Z, X, W); }
            set { BeginUpdate(); Y = value.X; Z = value.Y; X = value.Z; W = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Yzwx
        {
            get { return new EventVec4(Y, Z, W, X); }
            set { BeginUpdate(); Y = value.X; Z = value.Y; W = value.Z; X = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Ywxz
        {
            get { return new EventVec4(Y, W, X, Z); }
            set { BeginUpdate(); Y = value.X; W = value.Y; X = value.Z; Z = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Ywzx
        {
            get { return new EventVec4(Y, W, Z, X); }
            set { BeginUpdate(); Y = value.X; W = value.Y; Z = value.Z; X = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Zxyw
        {
            get { return new EventVec4(Z, X, Y, W); }
            set { BeginUpdate(); Z = value.X; X = value.Y; Y = value.Z; W = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Zxwy
        {
            get { return new EventVec4(Z, X, W, Y); }
            set { BeginUpdate(); Z = value.X; X = value.Y; W = value.Z; Y = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Zyxw
        {
            get { return new EventVec4(Z, Y, X, W); }
            set { BeginUpdate(); Z = value.X; Y = value.Y; X = value.Z; W = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Zywx
        {
            get { return new EventVec4(Z, Y, W, X); }
            set { BeginUpdate(); Z = value.X; Y = value.Y; W = value.Z; X = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Zwxy
        {
            get { return new EventVec4(Z, W, X, Y); }
            set { BeginUpdate(); Z = value.X; W = value.Y; X = value.Z; Y = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Zwyx
        {
            get { return new EventVec4(Z, W, Y, X); }
            set { BeginUpdate(); Z = value.X; W = value.Y; Y = value.Z; X = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Zwzy
        {
            get { return new EventVec4(Z, W, Z, Y); }
            set { BeginUpdate(); X = value.X; W = value.Y; Z = value.Z; Y = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Wxyz
        {
            get { return new EventVec4(W, X, Y, Z); }
            set { BeginUpdate(); W = value.X; X = value.Y; Y = value.Z; Z = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Wxzy
        {
            get { return new EventVec4(W, X, Z, Y); }
            set { BeginUpdate(); W = value.X; X = value.Y; Z = value.Z; Y = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Wyxz
        {
            get { return new EventVec4(W, Y, X, Z); }
            set { BeginUpdate(); W = value.X; Y = value.Y; X = value.Z; Z = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Wyzx
        {
            get { return new EventVec4(W, Y, Z, X); }
            set { BeginUpdate(); W = value.X; Y = value.Y; Z = value.Z; X = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Wzxy
        {
            get { return new EventVec4(W, Z, X, Y); }
            set { BeginUpdate(); W = value.X; Z = value.Y; X = value.Z; Y = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Wzyx
        {
            get { return new EventVec4(W, Z, Y, X); }
            set { BeginUpdate(); W = value.X; Z = value.Y; Y = value.Z; X = value.W; EndUpdate(); }
        }
        [XmlIgnore]
        public EventVec4 Wzyw
        {
            get { return new EventVec4(W, Z, Y, W); }
            set { BeginUpdate(); X = value.X; Z = value.Y; Y = value.Z; W = value.W; EndUpdate(); }
        }

        public static EventVec4 operator +(EventVec4 left, EventVec4 right)
        {
            return new EventVec4(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z,
                left.W + right.W);
        }
        public static EventVec4 operator -(EventVec4 left, EventVec4 right)
        {
            return new EventVec4(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z,
                left.W - right.W);
        }
        public static EventVec4 operator -(EventVec4 vec)
        {
            return new EventVec4(
                -vec.X,
                -vec.Y,
                -vec.Z,
                -vec.W);
        }
        public static EventVec4 operator *(EventVec4 vec, float scale)
        {
            return new EventVec4(
                vec.X * scale,
                vec.Y * scale,
                vec.Z * scale,
                vec.W * scale);
        }
        public static EventVec4 operator /(EventVec4 vec, float scale)
        {
            return vec * (1.0f / scale);
        }
        public static EventVec4 operator *(float scale, EventVec4 vec)
        {
            return vec * scale;
        }
        public static EventVec4 operator *(EventVec4 vec, EventVec4 scale)
        {
            return new EventVec4(
                vec.X * scale.X,
                vec.Y * scale.Y,
                vec.Z * scale.Z,
                vec.W * scale.W);
        }
        public static EventVec4 operator *(EventVec4 vec, Quat quat)
        {
            Quat v = new Quat(vec.X, vec.Y, vec.Z, vec.W);
            v = quat * v * quat.Inverted();
            return new EventVec4(v.X, v.Y, v.Z, v.W);
        }
        public static bool operator ==(EventVec4 left, EventVec4 right)
        {
            bool ln = left is null;
            bool rn = right is null;
            if (ln != rn)
                return false;
            if (left is null)
                return true;
            return left.Equals(right);
        }
        public static bool operator !=(EventVec4 left, EventVec4 right)
        {
            bool ln = left is null;
            bool rn = right is null;
            if (ln != rn)
                return true;
            if (left is null)
                return false;
            return !left.Equals(right);
        }
        public static implicit operator BulletSharp.Vector4(EventVec4 v)
        {
            return new BulletSharp.Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static implicit operator EventVec4(BulletSharp.Vector4 v)
        {
            return new EventVec4(v.X, v.Y, v.Z, v.W);
        }
        private static readonly string ListSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString() => String.Format("({0}{4} {1}{4} {2}{4} {3})", X, Y, Z, W, ListSeparator);
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is EventVec4))
                return false;

            return Equals((EventVec4)obj);
        }
        public bool Equals(EventVec4 other)
        {
            if (other is null)
                return false;

            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                W == other.W;
        }
        public bool Equals(EventVec4 other, float precision) => Abs(X - other.X) < precision &&
                Abs(Y - other.Y) < precision &&
                Abs(Z - other.Z) < precision &&
                Abs(W - other.W) < precision;

        public string WriteToString()
            => _data.WriteToString();
        public void ReadFromString(string str)
            => _data.ReadFromString(str);
    }
}