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
        public EventQuatRotator() : this(RotationOrder.YPR) { }

        public EventQuatRotator() : this(0.0f, 0.0f, 0.0f) { }
        public EventQuatRotator(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public EventQuatRotator(Vec3 xyz)
        {
            _xyz = xyz;
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
            get => _xyz;
            set
            {
                BeginUpdate();
                _xyz = value;
                EndUpdate();
            }
        }

        [Serialize("XYZ", IsXmlAttribute = true)]
        private Vec3 _xyz;
        [Serialize("LockX", IsXmlAttribute = true)]
        private bool _lockX = false;
        [Serialize("LockY", IsXmlAttribute = true)]
        private bool _lockY = false;
        [Serialize("LockZ", IsXmlAttribute = true)]
        private bool _lockZ = false;
        
        private void BeginUpdate()
        {
            _prevXYZ = _xyz;
        }
        private void EndUpdate()
        {
            bool anyChanged = false;
            if (!_prevXYZ.X.EqualTo(_xyz.X))
            {
                anyChanged = true;
                XChanged?.Invoke(_xyz.X, _prevXYZ.X);
            }
            if (!_prevXYZ.Y.EqualTo(_xyz.Y))
            {
                anyChanged = true;
                YChanged?.Invoke(_xyz.Y, _prevXYZ.Y);
            }
            if (!_prevXYZ.Z.EqualTo(_xyz.Z))
            {
                anyChanged = true;
                ZChanged?.Invoke(_xyz.Z, _prevXYZ.Z);
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
            other.YawChanged += Other_YawChanged;
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
            other.RollChanged += Other_RollChanged;
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

        public Quat ToQuaternion()
            => Quat.FromRotator(this);

        public Matrix4 GetMatrix() 
            => Matrix4.CreateFromRotator(this);

        public Matrix4 GetInverseMatrix()
            => Matrix4.CreateFromRotator(Inverted());

        public Vec3 GetDirection()
            => TransformVector(Vec3.Forward);

        public Vec3 TransformVector(Vec3 vector)
            => Vec3.TransformVector(vector, GetMatrix());

        public Matrix4 GetYawMatrix()
            => Matrix4.CreateRotationY(Y);
        public Matrix4 GetPitchMatrix()
            => Matrix4.CreateRotationX(X);
        public Matrix4 GetRollMatrix()
            => Matrix4.CreateRotationZ(Z);

        public Quat GetYawQuat() 
            => Quat.FromAxisAngle(Vec3.Up, Y);
        public Quat GetPitchQuat()
            => Quat.FromAxisAngle(Vec3.Right, X);
        public Quat GetRollQuat()
            => Quat.FromAxisAngle(Vec3.Forward, Z);

        public void SetDirection(Vec3 value)
            => SetRotations(value.LookatAngles());

        public Rotator HardCopy()
            => new Rotator(X, Y, Z, _rotationOrder);

        public Rotator WithNegatedRotations()
            => new Rotator(-X, -Y, -Z, _rotationOrder);

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
        public int GetYawWindCount() { return (int)(Y / 360.0f); }
        public int GetPitchWindCount() { return (int)(X / 360.0f); }
        public int GetRollWindCount() { return (int)(Z / 360.0f); }
        public void ReverseOrder()
        {
            BeginUpdate();
            _rotationOrder = OppositeOf(_rotationOrder);
            EndUpdate();
        }
        public void Invert()
        {
            BeginUpdate();
            NegateRotations();
            ReverseOrder();
            EndUpdate();
        }
        public Rotator Inverted()
        {
            return new Rotator(-X, -Y, -Z, OppositeOf(_rotationOrder));
        }
        public static RotationOrder OppositeOf(RotationOrder order)
        {
            switch (order)
            {
                case RotationOrder.PRY: return RotationOrder.YRP;
                case RotationOrder.PYR: return RotationOrder.RYP;
                case RotationOrder.RPY: return RotationOrder.YPR;
                case RotationOrder.RYP: return RotationOrder.PYR;
                case RotationOrder.YRP: return RotationOrder.PRY;
                case RotationOrder.YPR: return RotationOrder.RPY;
                default: throw new Exception();
            }
        }

        public static Rotator ComponentMin(Rotator a, Rotator b)
        {
            a.Yaw = a.Yaw < b.Yaw ? a.Yaw : b.Yaw;
            a.Pitch = a.Pitch < b.Pitch ? a.Pitch : b.Pitch;
            a.Roll = a.Roll < b.Roll ? a.Roll : b.Roll;
            return a;
        }
        public static Rotator ComponentMax(Rotator a, Rotator b)
        {
            a.Yaw = a.Yaw > b.Yaw ? a.Yaw : b.Yaw;
            a.Pitch = a.Pitch > b.Pitch ? a.Pitch : b.Pitch;
            a.Roll = a.Roll > b.Roll ? a.Roll : b.Roll;
            return a;
        }
        public static Rotator Clamp(Rotator value, Rotator min, Rotator max)
        {
            Rotator v = new Rotator()
            {
                Yaw = value.Yaw < min.Yaw ? min.Yaw : value.Yaw > max.Yaw ? max.Yaw : value.Yaw,
                Pitch = value.Pitch < min.Pitch ? min.Pitch : value.Pitch > max.Pitch ? max.Pitch : value.Pitch,
                Roll = value.Roll < min.Roll ? min.Roll : value.Roll > max.Roll ? max.Roll : value.Roll
            };
            return v;
        }

        public static Rotator Lerp(Rotator r1, Rotator r2, float time)
        {
            if (r1.Order != r2.Order)
                return null;
            return new Rotator(
                CustomMath.Lerp(r1.Pitch, r2.Pitch, time),
                CustomMath.Lerp(r1.Yaw, r2.Yaw, time),
                CustomMath.Lerp(r1.Roll, r2.Roll, time), 
                r1.Order);
        }

        public void Clamp(Rotator min, Rotator max)
        {
            BeginUpdate();
            Y = Y < min.Yaw ? min.Yaw : Y > max.Yaw ? max.Yaw : Y;
            X = X < min.Pitch ? min.Pitch : X > max.Pitch ? max.Pitch : X;
            Z = Z < min.Roll ? min.Roll : Z > max.Roll ? max.Roll : Z;
            EndUpdate();
        }
        public Rotator Clamped(Rotator min, Rotator max)
        {
            Rotator v = new Rotator()
            {
                Yaw = Y < min.Yaw ? min.Yaw : Y > max.Yaw ? max.Yaw : Y,
                Pitch = X < min.Pitch ? min.Pitch : X > max.Pitch ? max.Pitch : X,
                Roll = Z < min.Roll ? min.Roll : Z > max.Roll ? max.Roll : Z
            };
            return v;
        }

        public void SetRotations(float pitch, float yaw, float roll)
        {
            BeginUpdate();
            X = pitch;
            Y = yaw;
            Z = roll;
            EndUpdate();
        }
        public void SetRotations(Rotator other)
        {
            BeginUpdate();
            X = other.Pitch;
            Y = other.Yaw;
            Z = other.Roll;
            _rotationOrder = other._rotationOrder;
            EndUpdate();
        }
        public void SetRotations(float pitch, float yaw, float roll, RotationOrder order)
        {
            BeginUpdate();
            X = pitch;
            Y = yaw;
            Z = roll;
            _rotationOrder = order;
            EndUpdate();
        }

        public void SetRotationsNoUpdate(float pitch, float yaw, float roll)
        {
            if (!_lockPitch)
                _xyz.X = pitch;
            if (!_lockYaw)
                _xyz.Y = yaw;
            if (!_lockRoll)
                _xyz.Z = roll;
        }
        public void SetRotationsNoUpdate(Rotator other)
        {
            if (!_lockPitch)
                _xyz.X = other.Pitch;
            if (!_lockYaw)
                _xyz.Y = other.Yaw;
            if (!_lockRoll)
                _xyz.Z = other.Roll;
            _rotationOrder = other._rotationOrder;
        }
        public void SetRotationsNoUpdate(float pitch, float yaw, float roll, RotationOrder order)
        {
            if (!_lockPitch)
                _xyz.X = pitch;
            if (!_lockYaw)
                _xyz.Y = yaw;
            if (!_lockRoll)
                _xyz.Z = roll;
            _rotationOrder = order;
        }

        [Browsable(false)]
        [XmlIgnore]
        public Vec2 YawPitch
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
            get => _xyz.Y;
            set
            {
                if (_lockYaw)
                    return;
                BeginUpdate();
                _xyz.Y = value;
                EndUpdate();
            }
        }
        [Category("Rotator")]
        [XmlIgnore]
        public float X
        {
            get => _xyz.X;
            set
            {
                if (_lockPitch)
                    return;
                BeginUpdate();
                _xyz.X = value;
                EndUpdate();
            }
        }
        [Category("Rotator")]
        [XmlIgnore]
        public float Z
        {
            get => _xyz.Z;
            set
            {
                if (_lockRoll)
                    return;
                BeginUpdate();
                _xyz.Z = value;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 YawRoll
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
        public Vec2 PitchYaw
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
        public Vec2 PitchRoll
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
        public Vec2 RollYaw
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
        public Vec2 RollPitch
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
        public Vec3 YawPitchRoll
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
            float temp = _xyz.X;
            _xyz.X = _xyz.Y;
            _xyz.Y = _xyz.Z;
            _xyz.Z = temp;
        }

        [Browsable(false)]
        [XmlIgnore]
        public Vec3 YawRollPitch
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
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 PitchYawRoll
        {
            get => new Vec3(X, Y, Z);
            set
            {
                BeginUpdate();
                X = value.X;
                Y = value.Y;
                Z = value.Z;
                EndUpdate();
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 PitchRollYaw
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
        public Vec3 RollYawPitch
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
        public Vec3 RollPitchYaw
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
            Z = (float)Math.Round(Z, decimalPlaces);
            X = (float)Math.Round(X, decimalPlaces);
            Y = (float)Math.Round(Y, decimalPlaces);
            EndUpdate();
        }

        public static bool operator ==(Rotator left, Rotator right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }
        public static bool operator !=(Rotator left, Rotator right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);
            return !left.Equals(right);
        }
        
        public static explicit operator Vec3(Rotator v)
            => new Vec3(v.Yaw, v.Pitch, v.Roll);
        public static explicit operator Rotator(Vec3 v)
            => new Rotator(v.X, v.Y, v.Z, RotationOrder.PYR);

        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        public static Rotator GetZero(RotationOrder order = RotationOrder.YPR)
        {
           return new Rotator(0.0f, 0.0f, 0.0f, order);
        }
        public static Rotator Parse(string value)
        {
            string[] parts = value.Split(' ');
            return new Rotator(
            float.Parse(parts[0].Substring(1, parts[0].Length - 2)),
            float.Parse(parts[1].Substring(0, parts[1].Length - 1)),
            float.Parse(parts[2].Substring(0, parts[2].Length - 1)),
            (RotationOrder)Enum.Parse(typeof(RotationOrder), parts[3].Substring(0, parts[3].Length - 1)));
        }
        public override string ToString()
        {
            return String.Format("({0}{3} {1}{3} {2}{3} {4})", X, Y, Z, listSeparator, _rotationOrder);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Y.GetHashCode();
                hashCode = (hashCode * 397) ^ X.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Rotator))
                return false;

            return Equals((Rotator)obj);
        }
        public bool Equals(Rotator other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return
                Y == other.Yaw &&
                X == other.Pitch &&
                Z == other.Roll;
        }
        public bool Equals(Rotator other, float precision)
        {
            if (ReferenceEquals(other, null))
                return false;
            return
                Abs(Y - other.Yaw) < precision &&
                Abs(X - other.Pitch) < precision &&
                Abs(Z - other.Roll) < precision;
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
