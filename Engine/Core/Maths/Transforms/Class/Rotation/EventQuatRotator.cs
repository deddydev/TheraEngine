using static System.Math;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.ComponentModel;

namespace System
{
    public delegate void EventQuatRotatorChange(EventQuatRotator rotation);
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe class EventQuatRotator : IEquatable<EventQuatRotator>
    {
        public EventQuatRotator() : this(0.0f, 0.0f, 0.0f) { }
        public EventQuatRotator(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public EventQuatRotator(Vec3 xyz)
        {
            XYZ = xyz;
        }
        public EventQuatRotator(QuatRotator rotator)
        {
            XYZ = rotator.XYZ;
        }

        public event FloatChange XChanged;
        public event FloatChange YChanged;
        public event FloatChange ZChanged;
        public event EventQuatRotatorChange AllChanged;
        public event Action Changed;
        private int _updateIndex = 0;
        private Vec3 _prevXYZ;
        private EventQuatRotator _syncX, _syncY, _syncZ, _syncAll;
        //private readonly object _lock = new object();

        [Browsable(false)]
        public Vec3 RawXYZ
        {
            get => XYZ;
            set
            {
                BeginUpdate();
                XYZ = value;
                EndUpdate();
            }
        }

        [Serialize("XYZ", XmlNodeType = EXmlNodeType.Attribute)]
        private Vec3 XYZ;
        [Serialize("LockX", XmlNodeType = EXmlNodeType.Attribute)]
        private bool _lockX = false;
        [Serialize("LockY", XmlNodeType = EXmlNodeType.Attribute)]
        private bool _lockY = false;
        [Serialize("LockZ", XmlNodeType = EXmlNodeType.Attribute)]
        private bool _lockZ = false;

        private void BeginUpdate()
        {
            _prevXYZ = XYZ;
        }
        private void EndUpdate()
        {
            bool anyChanged = false;
            if (!_prevXYZ.X.EqualTo(XYZ.X))
            {
                anyChanged = true;
                XChanged?.Invoke(XYZ.X, _prevXYZ.X);
            }
            if (!_prevXYZ.Y.EqualTo(XYZ.Y))
            {
                anyChanged = true;
                YChanged?.Invoke(XYZ.Y, _prevXYZ.Y);
            }
            if (!_prevXYZ.Z.EqualTo(XYZ.Z))
            {
                anyChanged = true;
                ZChanged?.Invoke(XYZ.Z, _prevXYZ.Z);
            }
            if (anyChanged)
            {
                AllChanged?.Invoke(this);
                Changed?.Invoke();
            }
        }
        public void SyncXFrom(EventQuatRotator other)
        {
            if (_syncAll != null)
            {
                _syncAll.AllChanged -= Other_Changed;
                _syncAll = null;
            }
            other.XChanged += Other_XChanged;
            _syncX = other;
            X = other.X;
        }
        public void SyncYFrom(EventQuatRotator other)
        {
            if (_syncAll != null)
            {
                _syncAll.AllChanged -= Other_Changed;
                _syncAll = null;
            }
            other.YChanged += Other_YChanged;
            _syncY = other;
            Y = other.Y;
        }
        public void SyncZFrom(EventQuatRotator other)
        {
            if (_syncAll != null)
            {
                _syncAll.AllChanged -= Other_Changed;
                _syncAll = null;
            }
            other.ZChanged += Other_ZChanged;
            _syncZ = other;
            Z = other.Z;
        }
        public void SyncFrom(EventQuatRotator other)
        {
            if (_syncX != null)
            {
                _syncX.XChanged -= Other_XChanged;
                _syncX = null;
            }
            if (_syncY != null)
            {
                _syncY.YChanged -= Other_YChanged;
                _syncY = null;
            }
            if (_syncZ != null)
            {
                _syncZ.ZChanged -= Other_ZChanged;
                _syncZ = null;
            }
            other.AllChanged += Other_Changed;
            _syncAll = other;
            SetRotations(other);
        }
        public void StopSynchronization()
        {
            if (_syncAll != null)
            {
                _syncAll.AllChanged -= Other_Changed;
                _syncAll = null;
            }
            if (_syncX != null)
            {
                _syncX.XChanged -= Other_XChanged;
                _syncX = null;
            }
            if (_syncY != null)
            {
                _syncY.YChanged -= Other_YChanged;
                _syncY = null;
            }
            if (_syncZ != null)
            {
                _syncZ.ZChanged -= Other_ZChanged;
                _syncZ = null;
            }
        }

        private void Other_Changed(EventQuatRotator other)
        {
            SetRotations(other);
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

        public enum RotOrder
        {
            XYZ,
            XZY,
            YXZ,
            YZX,
            ZXY,
            ZYX,
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="intrinsic">True if the rotation coordinates follows the rotated body. If false, stays static.</param>
        /// <returns></returns>
        public Quat GetResult(RotOrder order)
        {
            Quat X = Quat.FromAxisAngle(Vec3.UnitX, XYZ.X);
            Quat Y = Quat.FromAxisAngle(Vec3.UnitY, XYZ.Y);
            Quat Z = Quat.FromAxisAngle(Vec3.UnitZ, XYZ.Z);
            switch (order)
            {
                case RotOrder.XYZ: return X * Y * Z;
                case RotOrder.XZY: return X * Z * Y;
                case RotOrder.YXZ: return Y * X * Z;
                case RotOrder.YZX: return Y * Z * X;
                case RotOrder.ZXY: return Z * X * Y;
                case RotOrder.ZYX: return Z * Y * X;
            }
            return Z * Y * Z;
        }
        public Matrix4 GetMatrix()
            => Matrix4.CreateFromQuaternion(GetResult());
        
        public Vec3 GetDirection()
            => TransformVector(Vec3.Forward);

        public Vec3 TransformVector(Vec3 vector) => GetResult() * vector;

        public Matrix4 GetYMatrix()
            => Matrix4.CreateRotationY(Y);
        public Matrix4 GetXMatrix()
            => Matrix4.CreateRotationX(X);
        public Matrix4 GetZMatrix()
            => Matrix4.CreateRotationZ(Z);

        public Quat GetYQuat() 
            => Quat.FromAxisAngle(Vec3.Up, Y);
        public Quat GetXQuat()
            => Quat.FromAxisAngle(Vec3.Right, X);
        public Quat GetZQuat()
            => Quat.FromAxisAngle(-Vec3.Forward, Z);
        
        public EventQuatRotator HardCopy()
            => new EventQuatRotator(X, Y, Z);

        public EventQuatRotator WithNegatedRotations()
            => new EventQuatRotator(-X, -Y, -Z);

        public void NegateRotations()
        {
            BeginUpdate();
            Y     = -Y;
            X   = -X;
            Z    = -Z;
            EndUpdate();
        }
        public void ReverseRotations()
        {
            BeginUpdate();
            Y     = Y   + 180.0f;
            X   = X + 180.0f;
            Z    = Z  + 180.0f;
            EndUpdate();
        }
        public void ClearWinding0to360()
        {
            BeginUpdate();
            Y = Y.RemapToRange(0.0f, 360.0f);
            X = X.RemapToRange(0.0f, 360.0f);
            Z = Z.RemapToRange(0.0f, 360.0f);
            EndUpdate();
        }
        public void ClearWindingNeg180to180()
        {
            BeginUpdate();
            Y = Y.RemapToRange(-180.0f, 180.0f);
            X = X.RemapToRange(-180.0f, 180.0f);
            Z = Z.RemapToRange(-180.0f, 180.0f);
            EndUpdate();
        }

        public int GetYWindCount() 
            => (int)Floor(Y / 360.0f);
        public int GetXWindCount() 
            => (int)Floor(X / 360.0f);
        public int GetZWindCount() 
            => (int)Floor(Z / 360.0f);

        public void Invert()
        {
            BeginUpdate();
            NegateRotations();
            EndUpdate();
        }

        public static EventQuatRotator ComponentMin(EventQuatRotator a, EventQuatRotator b)
            => new EventQuatRotator(
                a.X < b.X ? a.X : b.X,
                a.Y < b.Y ? a.Y : b.Y,
                a.Z < b.Z ? a.Z : b.Z);
        public static EventQuatRotator ComponentMax(EventQuatRotator a, EventQuatRotator b)
            => new EventQuatRotator(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y,
                a.Z > b.Z ? a.Z : b.Z);
        public static EventQuatRotator Clamp(EventQuatRotator value, EventQuatRotator min, EventQuatRotator max)
            => new EventQuatRotator(
                value.X.Clamp(min.X, max.X),
                value.Y.Clamp(min.Y, max.Y),
                value.Z.Clamp(min.Z, max.Z));
        public static EventQuatRotator Lerp(EventQuatRotator r1, EventQuatRotator r2, float time)
            => new EventQuatRotator(
                CustomMath.Lerp(r1.X, r2.X, time),
                CustomMath.Lerp(r1.Y, r2.Y, time),
                CustomMath.Lerp(r1.Z, r2.Z, time));

        public void Clamp(EventQuatRotator min, EventQuatRotator max)
        {
            BeginUpdate();
            X = X.Clamp(min.X, max.X);
            Y = Y.Clamp(min.Y, max.Y);
            Z = Z.Clamp(min.Z, max.Z);
            EndUpdate();
        }

        public EventQuatRotator Clamped(EventQuatRotator min, EventQuatRotator max)
            => new EventQuatRotator(X.Clamp(min.X, max.X), Y.Clamp(min.Y, max.Y), Z.Clamp(min.Z, max.Z));
        
        public void SetRotations(EventQuatRotator other)
        {
            BeginUpdate();
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            EndUpdate();
        }
        public void SetRotations(float x, float y, float z)
        {
            BeginUpdate();
            X = x;
            Y = y;
            Z = z;
            EndUpdate();
        }

        public void SetRotationsNoUpdate(EventQuatRotator other)
        {
            if (!_lockX) XYZ.X = other.X;
            if (!_lockY) XYZ.Y = other.Y;
            if (!_lockZ) XYZ.Z = other.Z;
        }
        public void SetRotationsNoUpdate(QuatRotator other)
        {
            if (!_lockX) XYZ.X = other.X;
            if (!_lockY) XYZ.Y = other.Y;
            if (!_lockZ) XYZ.Z = other.Z;
        }
        public void SetRotationsNoUpdate(float X, float Y, float Z)
        {
            if (!_lockX) XYZ.X = X;
            if (!_lockY) XYZ.Y = Y;
            if (!_lockZ) XYZ.Z = Z;
        }

        [Browsable(false)]
        [XmlIgnore]
        public Vec2 YX
        {
            get => new Vec2(Y, X);
            set
            {
                BeginUpdate();
                Y = value.X;
                X = value.Y;
                EndUpdate();
            }
        }
        [Category("Rotator")]
        [XmlIgnore]
        public float Y
        {
            get => XYZ.Y;
            set
            {
                if (_lockY)
                    return;
                BeginUpdate();
                XYZ.Y = value;
                EndUpdate();
            }
        }
        [Category("Rotator")]
        [XmlIgnore]
        public float X
        {
            get => XYZ.X;
            set
            {
                if (_lockX)
                    return;
                BeginUpdate();
                XYZ.X = value;
                EndUpdate();
            }
        }
        [Category("Rotator")]
        [XmlIgnore]
        public float Z
        {
            get => XYZ.Z;
            set
            {
                if (_lockZ)
                    return;
                BeginUpdate();
                XYZ.Z = value;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 YZ
        {
            get => new Vec2(Y, Z);
            set
            {
                BeginUpdate();
                Y = value.X;
                Z = value.Y;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 XY
        {
            get => new Vec2(X, Y);
            set
            {
                BeginUpdate();
                X = value.X;
                Y = value.Y;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 XZ
        {
            get => new Vec2(X, Z);
            set
            {
                BeginUpdate();
                X = value.X;
                Z = value.Y;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 ZY
        {
            get => new Vec2(Z, Y);
            set
            {
                BeginUpdate();
                Z = value.X;
                Y = value.Y;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 ZX
        {
            get => new Vec2(Z, X);
            set
            {
                BeginUpdate();
                Z = value.X;
                X = value.Y;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 YXZ
        {
            get => new Vec3(Y, X, Z);
            set
            {
                BeginUpdate();
                Y = value.X;
                X = value.Y;
                Z = value.Z;
                EndUpdate();
            }
        }

        public void ChangeZupToYup()
        {
            float temp = XYZ.X;
            XYZ.X = XYZ.Y;
            XYZ.Y = XYZ.Z;
            XYZ.Z = temp;
        }

        [Browsable(false)]
        [XmlIgnore]
        public Vec3 YZX
        {
            get => new Vec3(Y, Z, X);
            set
            {
                BeginUpdate();
                Y = value.X;
                Z = value.Y;
                X = value.Z;
                EndUpdate();
            }
        }
        //[Browsable(false)]
        //[XmlIgnore]
        //public Vec3 XYZ
        //{
        //    get => new Vec3(X, Y, Z);
        //    set
        //    {
        //        BeginUpdate();
        //        X = value.X;
        //        Y = value.Y;
        //        Z = value.Z;
        //        EndUpdate();
        //    }
        //}
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 XZY
        {
            get => new Vec3(X, Z, Y);
            set
            {
                BeginUpdate();
                X = value.X;
                Z = value.Y;
                Y = value.Z;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 ZYX
        {
            get => new Vec3(Z, Y, X);
            set
            {
                BeginUpdate();
                Z = value.X;
                Y = value.Y;
                X = value.Z;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 ZXY
        {
            get => new Vec3(Z, X, Y);
            set
            {
                BeginUpdate();
                Z = value.X;
                X = value.Y;
                Y = value.Z;
                EndUpdate();
            }
        }
        
        public void Round(int decimalPlaces)
        {
            BeginUpdate();
            X = (float)Math.Round(X, decimalPlaces);
            Y = (float)Math.Round(Y, decimalPlaces);
            Z = (float)Math.Round(Z, decimalPlaces);
            EndUpdate();
        }

        public static bool operator ==(EventQuatRotator left, EventQuatRotator right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }
        public static bool operator !=(EventQuatRotator left, EventQuatRotator right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);
            return !left.Equals(right);
        }
        
        public static explicit operator Vec3(EventQuatRotator v)  => v.XYZ;
        public static explicit operator EventQuatRotator(Vec3 v) => new EventQuatRotator(v);

        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        public static EventQuatRotator GetZero()
           => new EventQuatRotator(0.0f, 0.0f, 0.0f);
        
        public static EventQuatRotator Parse(string value)
            => new EventQuatRotator(new Vec3(value));
        
        public override string ToString() => XYZ.ToString();
        public override int GetHashCode() => XYZ.GetHashCode();
        public override bool Equals(object obj)
        {
            if (!(obj is EventQuatRotator))
                return false;

            return Equals((EventQuatRotator)obj);
        }
        public bool Equals(EventQuatRotator other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return
                Y == other.Y &&
                X == other.X &&
                Z == other.Z;
        }
        public bool Equals(EventQuatRotator other, float precision)
        {
            if (ReferenceEquals(other, null))
                return false;
            return
                Abs(Y - other.Y) < precision &&
                Abs(X - other.X) < precision &&
                Abs(Z - other.Z) < precision;
        }
        public bool IsZero()
        {
            return
                X.IsZero() && 
                Y.IsZero() && 
                Z.IsZero();
        }
    }
}
