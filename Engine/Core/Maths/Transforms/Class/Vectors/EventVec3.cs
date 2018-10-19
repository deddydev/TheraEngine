using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using static System.Math;
using static System.TMath;

namespace TheraEngine.Core.Maths.Transforms
{
    public delegate void DelFloatChange(float oldValue, float newValue);
    public delegate Vec3 DelGetVec3Value(float delta);

    /// <summary>
    /// A wrapper class for <see cref="Vec3"/> that supports events and synchronization when its values change.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public unsafe class EventVec3 : TObject, IEquatable<EventVec3>, IUniformable3Float, IBufferable, ISerializableString, IPoolable
    {
        public event Action XChanged;
        public event Action YChanged;
        public event Action ZChanged;
        public event DelFloatChange XValueChanged;
        public event DelFloatChange YValueChanged;
        public event DelFloatChange ZValueChanged;
        public event Action Changed;

        //private int _updating = 0;
        private float _oldX, _oldY, _oldZ;
        [TSerialize("XYZ", NodeType = ENodeType.Attribute)]
        private Vec3 _data;
        private EventVec3 _syncX, _syncY, _syncZ, _syncAll;
        private DelGetVec3Value _setTick = null;

        public void Reset()
        {
            _data = Vec3.Zero;
            _oldX = 0.0f;
            _oldY = 0.0f;
            _oldZ = 0.0f;
            _syncX = null;
            _syncY = null;
            _syncZ = null;
            _syncAll = null;
            XChanged = null;
            YChanged = null;
            ZChanged = null;
            XValueChanged = null;
            YValueChanged = null;
            ZValueChanged = null;
            Changed = null;
        }
        /// <summary>
        /// Sets the internal <see cref="Vec3"/> value and does not fire any events.
        /// </summary>
        public void SetRawNoUpdate(Vec3 raw)
        {
            _data = raw;
        }
        [Browsable(false)]
        public Vec3 Raw
        {
            get => _data;
            set
            {
                BeginUpdate();
                _data = value;
                EndUpdate();
            }
        }
        public float X
        {
            get => _data.X;
            set
            {
                BeginUpdate();
                _data.X = value;
                EndUpdate();
            }
        }
        public float Y
        {
            get => _data.Y;
            set
            {
                BeginUpdate();
                _data.Y = value;
                EndUpdate();
            }
        }
        public float Z
        {
            get => _data.Z;
            set
            {
                BeginUpdate();
                _data.Z = value;
                EndUpdate();
            }
        }
        [Browsable(false)]
        public float* Data { get { return _data.Data; } }

        public Matrix4 AsScaleMatrix()
            => _data.AsScaleMatrix();
        public Matrix4 AsTranslationMatrix()
            => _data.AsTranslationMatrix();

        [Browsable(false)]
        public VoidPtr Address => _data.Address;
        [Browsable(false)]
        public DataBuffer.ComponentType ComponentType => DataBuffer.ComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 3;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        public void Write(VoidPtr address)
        {
            _data.Write(address);
        }
        public void Read(VoidPtr address)
        {
            BeginUpdate();
            _data.Read(address);
            EndUpdate();
        }
        private void SyncByFrame(float delta) => Raw = _setTick(delta);
        public void SyncByFrame(DelGetVec3Value tickMethod)
        {
            _setTick = tickMethod;

            RegisterTick(
                ETickGroup.PostPhysics,
                ETickOrder.Logic,
                SyncByFrame,
                Input.Devices.EInputPauseType.TickAlways);
        }
        public void ClearSyncByFrame()
        {
            UnregisterTick(
                ETickGroup.PostPhysics,
                ETickOrder.Logic,
                SyncByFrame,
                Input.Devices.EInputPauseType.TickAlways);

            _setTick = null;
        }
        public void SyncXFrom(EventVec3 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            other.XValueChanged += Other_XChanged;
            _syncX = other;
            X = other.X;
        }
        public void SyncYFrom(EventVec3 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            other.YValueChanged += Other_YChanged;
            _syncY = other;
            Y = other.Y;
        }
        public void SyncRollFrom(EventVec3 other)
        {
            if (_syncAll != null)
            {
                _syncAll.Changed -= Other_Changed;
                _syncAll = null;
            }
            other.ZValueChanged += Other_ZChanged;
            _syncZ = other;
            Z = other.Z;
        }
        public void SyncFrom(EventVec3 other)
        {
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
            other.Changed += Other_Changed;
            _syncAll = other;
            Xyz = other.Raw;
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
        }

        private void Other_Changed()
        {
            Xyz = _syncAll.Raw;
        }
        private void Other_XChanged(float newValue, float oldValue)
        {
            X = newValue;
        }
        private void Other_YChanged(float newValue, float oldValue)
        {
            Y = newValue;
        }
        private void Other_ZChanged(float newValue, float oldValue)
        {
            Z = newValue;
        }


        public EventVec3() { }
        public EventVec3(float x, float y, float z)
        {
            _data = new Vec3(x, y, z);
        }
        public EventVec3(Vec3 v)
        {
            _data = v;
        }
        public EventVec3(float value) : this(value, value, value) { }
        public EventVec3(Vec2 v) : this(v.X, v.Y, 0.0f) { }
        public EventVec3(Vec2 v, float z) : this(v.X, v.Y, z) { }
        public EventVec3(float x, Vec2 v) : this(x, v.X, v.Y) { }
        public EventVec3(Vec4 v, bool divideByW)
        {
            _data = new Vec3(v, divideByW);
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        [Browsable(false)]
        public float LengthSquared => _data.LengthSquared;
        [Browsable(false)]
        public float LengthFast => _data.LengthFast;
        [Browsable(false)]
        public float Length => _data.Length;

        public float DistanceTo(Vec3 point) => _data.DistanceTo(point);
        public float DistanceToFast(Vec3 point) => _data.DistanceToFast(point);
        public float DistanceToSquared(Vec3 point) => _data.DistanceToSquared(point);

        private void BeginUpdate()
        {
            //++_updating;
            _oldX = X;
            _oldY = Y;
            _oldZ = Z;
        }
        private void EndUpdate()
        {
            float x = X, y = Y, z = Z;
            float ox = _oldX, oy = _oldY, oz = _oldZ;

            //--_updating;
            //if (_updating > 0)
            //    return;
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
            if (anyChanged)
                Changed?.Invoke();
        }
        public void Normalize()
        {
            BeginUpdate();
            _data.Normalize();
            EndUpdate();
        }
        public void NormalizeFast()
        {
            BeginUpdate();
            _data.NormalizeFast();
            EndUpdate();
        }

        public void SetLequalTo(Vec3 other)
        {
            if (!(_data <= other))
            {
                BeginUpdate();
                X = other.X;
                Y = other.Y;
                Z = other.Z;
                EndUpdate();
            }
        }
        public void SetGequalTo(Vec3 other)
        {
            if (!(_data >= other))
            {
                BeginUpdate();
                X = other.X;
                Y = other.Y;
                Z = other.Z;
                EndUpdate();
            }
        }
        public void Clamp(Vec3 min, Vec3 max)
        {
            BeginUpdate();
            X = X < min.X ? min.X : X > max.X ? max.X : X;
            Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
            Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
            EndUpdate();
        }

        public float Dot(Vec3 right)
            => X * right.X + Y * right.Y + Z * right.Z;

        /// <summary>
        ///
        ///        |
        /// normal |  /
        /// l x r, | / right
        /// -r x l |/_______ 
        ///            left
        /// </summary>
        public Vec3 Cross(Vec3 right)
            => _data ^ right;
        
        /// <summary>
        /// Calculates the angle (in degrees) between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        /// <remarks>Note that the returned angle is never bigger than 180.</remarks>
        public float CalculateAngle(Vec3 second)
            => RadToDeg((float)Acos((Dot(second) / (LengthFast * second.LengthFast)).Clamp(-1.0f, 1.0f)));
        /// <summary>
        /// Projects a vector from object space into screen space.
        /// </summary>
        /// <param name="worldPoint">The vector to project.</param>
        /// <param name="x">The X coordinate of the viewport.</param>
        /// <param name="y">The Y coordinate of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The world-view-projection matrix.</param>
        /// <returns>The vector in screen space.</returns>
        /// <remarks>
        /// To project to normalized device coordinates (NDC) use the following parameters:
        /// Project(vector, -1, -1, 2, 2, -1, 1, worldViewProjection).
        /// </remarks>
        public Vec3 Project(float x, float y, float width, float height, float minZ, float maxZ, Matrix4 worldViewProjection)
        {
            Vec3 result = Vec3.TransformPerspective(worldViewProjection, _data);

            result.X = x + (width * ((result.X + 1.0f) / 2.0f));
            result.Y = y + (height * ((result.Y + 1.0f) / 2.0f));
            result.Z = minZ + ((maxZ - minZ) * ((result.Z + 1.0f) / 2.0f));

            return result;
        }
        /// <summary>
        /// Projects a vector from screen space into object space.
        /// </summary>
        /// <param name="screenPoint">The vector to project.</param>
        /// <param name="x">The X coordinate of the viewport.</param>
        /// <param name="y">The Y coordinate of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The inverse of the world-view-projection matrix.</param>
        /// <returns>The vector in object space.</returns>
        /// <remarks>
        /// To project from normalized device coordinates (NDC) use the following parameters:
        /// Project(vector, -1, -1, 2, 2, -1, 1, inverseWorldViewProjection).
        /// </remarks>
        public Vec3 Unproject(float x, float y, float width, float height, float minZ, float maxZ, Matrix4 inverseWorldViewProjection)
        {
            return inverseWorldViewProjection * new Vec3(
                (X - x) / width * 2.0f - 1.0f,
                (Y - y) / height * 2.0f - 1.0f,
                Z / (maxZ - minZ) * 2.0f - 1.0f);
        }
        /// <summary>
        /// Returns a YPR rotator with azimuth as yaw, elevation as pitch, and 0 as roll.
        /// </summary>
        public Rotator LookatAngles()
        {
            return _data.LookatAngles();
        }
        
        public Vec3 GetSafeNormal(float tolerance = 1.0e-8f)
        {
            return _data.GetSafeNormal(tolerance);
        }

        /// <summary>
        /// Returns the portion of this Vec3 that is parallel to the given normal.
        /// </summary>
        public Vec3 ParallelComponent(Vec3 normal)
        {
            return _data.ParallelComponent(normal);
        }
        /// <summary>
        /// Returns the portion of this Vec3 that is perpendicular to the given normal.
        /// </summary>
        public Vec3 PerpendicularComponent(Vec3 normal)
        {
            return _data.PerpendicularComponent(normal);
        }

        //public EventVec3 GetAngles() { return new EventVec3(AngleX(), AngleY(), AngleZ()); }
        //public EventVec3 GetAngles(EventVec3 origin) { return (this - origin).GetAngles(); }
        //public float AngleX() { return (float)Atan2(Y, -Z); }
        //public float AngleY() { return (float)Atan2(-Z, X); }
        //public float AngleZ() { return (float)Atan2(Y, X); }

        public bool IsInTriangle(Vec3 triPt1, Vec3 triPt2, Vec3 triPt3)
        {
            return _data.IsInTriangle(triPt1, triPt2, triPt3);
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
        public void SetYz(float y, float z)
        {
            BeginUpdate();
            Y = y;
            Z = z;
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
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Xy
        {
            get { return new Vec2(X, Y); }
            set { SetXy(value.X, value.Y); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Xz
        {
            get { return new Vec2(X, Z); }
            set { SetXz(value.X, value.Y); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Yx
        {
            get { return new Vec2(Y, X); }
            set { SetXy(value.Y, value.X); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Yz
        {
            get { return new Vec2(Y, Z); }
            set { SetYz(value.X, value.Y); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Zx
        {
            get { return new Vec2(Z, X); }
            set { SetXz(value.Y, value.X); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Zy
        {
            get { return new Vec2(Z, Y); }
            set { SetYz(value.Y, value.X); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public EventVec3 Xzy
        {
            get { return new EventVec3(X, Z, Y); }
            set { SetXyz(value.X, value.Z, value.Y); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public EventVec3 Yxz
        {
            get { return new EventVec3(Y, X, Z); }
            set { SetXyz(value.Y, value.X, value.Z); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public EventVec3 Yzx
        {
            get { return new EventVec3(Y, Z, X); }
            set { SetXyz(value.Y, value.Z, value.X); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public EventVec3 Zxy
        {
            get { return new EventVec3(Z, X, Y); }
            set { SetXyz(value.Z, value.X, value.Y); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public EventVec3 Zyx
        {
            get { return new EventVec3(Z, Y, X); }
            set { SetXyz(value.Z, value.Y, value.X); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public EventVec3 Xyz
        {
            get { return new EventVec3(X, Y, Z); }
            set { SetXyz(value.X, value.Y, value.Z); }
        }

        public static Vec3 operator +(float left, EventVec3 right) { return left + right._data; }
        public static Vec3 operator +(EventVec3 left, float right) { return left._data + right; }
        public static Vec3 operator -(float left, EventVec3 right) { return left - right._data; }
        public static Vec3 operator -(EventVec3 left, float right) { return left._data - right; }
        
        public static Vec3 operator +(EventVec3 left, EventVec3 right) { return left._data + right._data; }
        public static Vec3 operator -(EventVec3 left, EventVec3 right) { return left._data - right._data; }
        public static Vec3 operator +(EventVec3 left, Vec3 right) { return left._data + right; }
        public static Vec3 operator -(EventVec3 left, Vec3 right) { return left._data - right; }
        public static Vec3 operator +(Vec3 left, EventVec3 right) { return left + right._data; }
        public static Vec3 operator -(Vec3 left, EventVec3 right) { return left - right._data; }

        public static Vec3 operator -(EventVec3 vec) { return -vec._data; }

        public static Vec3 operator *(EventVec3 vec, float scale) { return vec._data * scale; }
        public static Vec3 operator /(EventVec3 vec, float scale) { return vec._data / scale; }
        public static Vec3 operator *(float scale, EventVec3 vec) { return vec._data * scale; }
        public static Vec3 operator /(float scale, EventVec3 vec) { return scale / vec._data; }

        public static Vec3 operator /(EventVec3 left, EventVec3 right) { return left._data / right._data; }
        public static Vec3 operator *(EventVec3 left, EventVec3 right) { return left._data * right._data; }
        public static Vec3 operator /(EventVec3 left, Vec3 right) { return left._data / right; }
        public static Vec3 operator *(EventVec3 left, Vec3 right) { return left._data * right; }
        public static Vec3 operator /(Vec3 left, EventVec3 right) { return left / right._data; }
        public static Vec3 operator *(Vec3 left, EventVec3 right) { return left * right._data; }

        public static bool operator <(EventVec3 left, EventVec3 right) { return left._data < right._data; }
        public static bool operator <(Vec3 left, EventVec3 right) { return left < right._data; }
        public static bool operator <(EventVec3 left, Vec3 right) { return left._data < right; }

        public static bool operator >(EventVec3 left, EventVec3 right) { return left._data > right._data; }
        public static bool operator >(Vec3 left, EventVec3 right) { return left > right._data; }
        public static bool operator >(EventVec3 left, Vec3 right) { return left._data > right; }

        public static bool operator <=(EventVec3 left, EventVec3 right) { return left._data <= right._data; }
        public static bool operator <=(Vec3 left, EventVec3 right) { return left <= right._data; }
        public static bool operator <=(EventVec3 left, Vec3 right) { return left._data <= right; }

        public static bool operator >=(EventVec3 left, EventVec3 right) { return left._data >= right._data; }
        public static bool operator >=(Vec3 left, EventVec3 right) { return left >= right._data; }
        public static bool operator >=(EventVec3 left, Vec3 right) { return left._data >= right; }

        public static bool operator ==(EventVec3 left, EventVec3 right)
        {
            if (ReferenceEquals(left, null))
                if (ReferenceEquals(right, null))
                    return true;
                else
                    return false;
            else if (ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }
        public static bool operator !=(EventVec3 left, EventVec3 right)
        {
            if (ReferenceEquals(left, null))
                if (ReferenceEquals(right, null))
                    return false;
                else
                    return true;
            else if (ReferenceEquals(right, null))
                return true;

            return !left.Equals(right);
        }

        public static implicit operator Vec3(EventVec3 v) { return v._data; }
        public static implicit operator EventVec3(Vec3 v) { return new EventVec3(v); }

        public static explicit operator EventVec3(Vec2 v) { return new EventVec3(v.X, v.Y, 0.0f); }
        public static explicit operator EventVec3(ColorF4 v) { return new EventVec3(v.R, v.G, v.B); }
        public static explicit operator EventVec3(ColorF3 v) { return new EventVec3(v.R, v.G, v.B); }

        private const float _colorFactor = 1.0f / 255.0f;
        public static explicit operator EventVec3(Color c) { return new EventVec3(c.R * _colorFactor, c.G * _colorFactor, c.B * _colorFactor); }
        public static explicit operator Color(EventVec3 v) { return Color.FromArgb((int)(v.X / _colorFactor), (int)(v.Y / _colorFactor), (int)(v.Z / _colorFactor)); }

        public static implicit operator BulletSharp.Vector3(EventVec3 v) { return new BulletSharp.Vector3(v.X, v.Y, v.Z); }
        public static implicit operator EventVec3(BulletSharp.Vector3 v) { return new EventVec3(v.X, v.Y, v.Z); }

        private static string listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
        {
            return String.Format("({0}{3} {1}{3} {2})", X, Y, Z, listSeparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is EventVec3))
                return false;

            return Equals((EventVec3)obj);
        }
        public bool Equals(EventVec3 other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }
        public bool Equals(EventVec3 other, float precision)
        {
            return
                Abs(X - other.X) < precision &&
                Abs(Y - other.Y) < precision &&
                Abs(Z - other.Z) < precision;
        }

        public string WriteToString()
            => _data.WriteToString();
        public void ReadFromString(string str)
            => _data.ReadFromString(str);
    }
}
