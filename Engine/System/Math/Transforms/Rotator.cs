using static System.Math;
using static System.CustomMath;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace System
{
    public delegate void FloatChange(float newValue, float oldValue);
    public delegate void RotatorChange(Rotator rotation);
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe class Rotator : IEquatable<Rotator>
    {
        public Rotator() : this(Order.YPR) { }

        public Rotator(Order order) : this(0.0f, 0.0f, 0.0f, order) { }
        public Rotator(float pitch, float yaw, float roll, Order rotationOrder)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
            _rotationOrder = rotationOrder;
        }
        public Rotator(Vec3 pyr, Order rotationOrder)
        {
            _pyr = pyr;
            _rotationOrder = rotationOrder;
        }

        public event FloatChange PitchChanged;
        public event FloatChange YawChanged;
        public event FloatChange RollChanged;
        public event RotatorChange AllChanged;
        public event Action Changed;
        private int _updateIndex = 0;
        private Vec3 _prevPyr;
        private Rotator _syncPitch, _syncYaw, _syncRoll, _syncAll;

        [Serialize("PitchYawRoll", IsXmlAttribute = true)]
        public Vec3 _pyr;
        [Serialize("Order", IsXmlAttribute = true)]
        public Order _rotationOrder = Order.YPR;
        [Serialize("LockYaw", IsXmlAttribute = true)]
        private bool _lockYaw = false;
        [Serialize("LockPitch", IsXmlAttribute = true)]
        private bool _lockPitch = false;
        [Serialize("LockRoll", IsXmlAttribute = true)]
        private bool _lockRoll = false;
        
        private void BeginUpdate()
        {
            if (_updateIndex++ == 0)
                _prevPyr = _pyr;
        }
        private void EndUpdate()
        {
            if (--_updateIndex == 0)
            {
                bool anyChanged = false;
                if (!_prevPyr.X.EqualTo(_pyr.X))
                {
                    anyChanged = true;
                    PitchChanged?.Invoke(_pyr.X, _prevPyr.X);
                }
                if (!_prevPyr.Y.EqualTo(_pyr.Y))
                {
                    anyChanged = true;
                    YawChanged?.Invoke(_pyr.Y, _prevPyr.Y);
                }
                if (!_prevPyr.Z.EqualTo(_pyr.Z))
                {
                    anyChanged = true;
                    RollChanged?.Invoke(_pyr.Z, _prevPyr.Z);
                }
                if (anyChanged)
                {
                    AllChanged?.Invoke(this);
                    Changed?.Invoke();
                }
            }
        }
        public void SyncPitchFrom(Rotator other)
        {
            if (_syncAll != null)
            {
                _syncAll.AllChanged -= Other_Changed;
                _syncAll = null;
            }
            other.PitchChanged += Other_PitchChanged;
            _syncPitch = other;
            Pitch = other.Pitch;
        }
        public void SyncYawFrom(Rotator other)
        {
            if (_syncAll != null)
            {
                _syncAll.AllChanged -= Other_Changed;
                _syncAll = null;
            }
            other.YawChanged += Other_YawChanged;
            _syncYaw = other;
            Yaw = other.Yaw;
        }
        public void SyncRollFrom(Rotator other)
        {
            if (_syncAll != null)
            {
                _syncAll.AllChanged -= Other_Changed;
                _syncAll = null;
            }
            other.RollChanged += Other_RollChanged;
            _syncRoll = other;
            Roll = other.Roll;
        }
        public void SyncFrom(Rotator other)
        {
            if (_syncPitch != null)
            {
                _syncPitch.PitchChanged -= Other_PitchChanged;
                _syncPitch = null;
            }
            if (_syncYaw != null)
            {
                _syncYaw.YawChanged -= Other_YawChanged;
                _syncYaw = null;
            }
            if (_syncRoll != null)
            {
                _syncRoll.RollChanged -= Other_RollChanged;
                _syncRoll = null;
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
            if (_syncPitch != null)
            {
                _syncPitch.PitchChanged -= Other_PitchChanged;
                _syncPitch = null;
            }
            if (_syncYaw != null)
            {
                _syncYaw.YawChanged -= Other_YawChanged;
                _syncYaw = null;
            }
            if (_syncRoll != null)
            {
                _syncRoll.RollChanged -= Other_RollChanged;
                _syncRoll = null;
            }
        }

        private void Other_Changed(Rotator other)
        {
            SetRotations(other);
        }
        private void Other_PitchChanged(float newValue, float oldValue)
        {
            Pitch = newValue;
        }
        private void Other_YawChanged(float newValue, float oldValue)
        {
            Yaw = newValue;
        }
        private void Other_RollChanged(float newValue, float oldValue)
        {
            Roll = newValue;
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
            => Matrix4.CreateRotationY(Yaw);
        public Matrix4 GetPitchMatrix()
            => Matrix4.CreateRotationX(Pitch);
        public Matrix4 GetRollMatrix()
            => Matrix4.CreateRotationZ(Roll);

        public Quat GetYawQuat() 
            => Quat.FromAxisAngle(Vec3.Up, Yaw);
        public Quat GetPitchQuat()
            => Quat.FromAxisAngle(Vec3.Right, Pitch);
        public Quat GetRollQuat()
            => Quat.FromAxisAngle(Vec3.Forward, Roll);

        public void SetDirection(Vec3 value)
            => SetRotations(value.LookatAngles());

        public Rotator HardCopy()
            => new Rotator(Pitch, Yaw, Roll, _rotationOrder);

        public Rotator WithNegatedRotations()
            => new Rotator(-Pitch, -Yaw, -Roll, _rotationOrder);

        public void NegateRotations()
        {
            BeginUpdate();
            Yaw     = -Yaw;
            Pitch   = -Pitch;
            Roll    = -Roll;
            EndUpdate();
        }
        public void ReverseRotations()
        {
            BeginUpdate();
            Yaw     = Yaw   + 180.0f;
            Pitch   = Pitch + 180.0f;
            Roll    = Roll  + 180.0f;
            EndUpdate();
        }
        public void ClearWinding0to360()
        {
            BeginUpdate();
            Yaw = Yaw.RemapToRange(0.0f, 360.0f);
            Pitch = Pitch.RemapToRange(0.0f, 360.0f);
            Roll = Roll.RemapToRange(0.0f, 360.0f);
            EndUpdate();
        }
        public void ClearWindingNeg180to180()
        {
            BeginUpdate();
            Yaw = Yaw.RemapToRange(-180.0f, 180.0f);
            Pitch = Pitch.RemapToRange(-180.0f, 180.0f);
            Roll = Roll.RemapToRange(-180.0f, 180.0f);
            EndUpdate();
        }
        public int GetYawWindCount() { return (int)(Yaw / 360.0f); }
        public int GetPitchWindCount() { return (int)(Pitch / 360.0f); }
        public int GetRollWindCount() { return (int)(Roll / 360.0f); }
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
            return new Rotator(-Pitch, -Yaw, -Roll, OppositeOf(_rotationOrder));
        }
        public static Order OppositeOf(Order order)
        {
            switch (order)
            {
                case Order.PRY: return Order.YRP;
                case Order.PYR: return Order.RYP;
                case Order.RPY: return Order.YPR;
                case Order.RYP: return Order.PYR;
                case Order.YRP: return Order.PRY;
                case Order.YPR: return Order.RPY;
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
        public void Clamp(Rotator min, Rotator max)
        {
            BeginUpdate();
            Yaw = Yaw < min.Yaw ? min.Yaw : Yaw > max.Yaw ? max.Yaw : Yaw;
            Pitch = Pitch < min.Pitch ? min.Pitch : Pitch > max.Pitch ? max.Pitch : Pitch;
            Roll = Roll < min.Roll ? min.Roll : Roll > max.Roll ? max.Roll : Roll;
            EndUpdate();
        }
        public Rotator Clamped(Rotator min, Rotator max)
        {
            Rotator v = new Rotator()
            {
                Yaw = Yaw < min.Yaw ? min.Yaw : Yaw > max.Yaw ? max.Yaw : Yaw,
                Pitch = Pitch < min.Pitch ? min.Pitch : Pitch > max.Pitch ? max.Pitch : Pitch,
                Roll = Roll < min.Roll ? min.Roll : Roll > max.Roll ? max.Roll : Roll
            };
            return v;
        }

        public void SetRotations(float pitch, float yaw, float roll)
        {
            BeginUpdate();
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
            EndUpdate();
        }
        public void SetRotations(Rotator other)
        {
            BeginUpdate();
            Pitch = other.Pitch;
            Yaw = other.Yaw;
            Roll = other.Roll;
            _rotationOrder = other._rotationOrder;
            EndUpdate();
        }
        public void SetRotations(float pitch, float yaw, float roll, Order order)
        {
            BeginUpdate();
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
            _rotationOrder = order;
            EndUpdate();
        }

        public void SetRotationsNoUpdate(float pitch, float yaw, float roll)
        {
            if (!_lockPitch)
                _pyr.X = pitch;
            if (!_lockYaw)
                _pyr.Y = yaw;
            if (!_lockRoll)
                _pyr.Z = roll;
        }
        public void SetRotationsNoUpdate(Rotator other)
        {
            if (!_lockPitch)
                _pyr.X = other.Pitch;
            if (!_lockYaw)
                _pyr.Y = other.Yaw;
            if (!_lockRoll)
                _pyr.Z = other.Roll;
            _rotationOrder = other._rotationOrder;
        }
        public void SetRotationsNoUpdate(float pitch, float yaw, float roll, Order order)
        {
            if (!_lockPitch)
                _pyr.X = pitch;
            if (!_lockYaw)
                _pyr.Y = yaw;
            if (!_lockRoll)
                _pyr.Z = roll;
            _rotationOrder = order;
        }

        [XmlIgnore]
        public Vec2 YawPitch
        {
            get => new Vec2(Yaw, Pitch);
            set
            {
                BeginUpdate();
                Yaw = value.X;
                Pitch = value.Y;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public float Yaw
        {
            get => _pyr.Y;
            set
            {
                if (_lockYaw)
                    return;
                BeginUpdate();
                _pyr.Y = value;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public float Pitch
        {
            get => _pyr.X;
            set
            {
                if (_lockPitch)
                    return;
                BeginUpdate();
                _pyr.X = value;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public float Roll
        {
            get => _pyr.Z;
            set
            {
                if (_lockRoll)
                    return;
                BeginUpdate();
                _pyr.Z = value;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec2 YawRoll
        {
            get => new Vec2(Yaw, Roll);
            set
            {
                BeginUpdate();
                Yaw = value.X;
                Roll = value.Y;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec2 PitchYaw
        {
            get { return new Vec2(Pitch, Yaw); }
            set
            {
                BeginUpdate();
                Pitch = value.X;
                Yaw = value.Y;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec2 PitchRoll
        {
            get { return new Vec2(Pitch, Roll); }
            set
            {
                BeginUpdate();
                Pitch = value.X;
                Roll = value.Y;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec2 RollYaw
        {
            get { return new Vec2(Roll, Yaw); }
            set
            {
                BeginUpdate();
                Roll = value.X;
                Yaw = value.Y;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec2 RollPitch
        {
            get { return new Vec2(Roll, Pitch); }
            set
            {
                BeginUpdate();
                Roll = value.X;
                Pitch = value.Y;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec3 YawPitchRoll
        {
            get { return new Vec3(Yaw, Pitch, Roll); }
            set
            {
                BeginUpdate();
                Yaw = value.X;
                Pitch = value.Y;
                Roll = value.Z;
                EndUpdate();
            }
        }

        public void ChangeZupToYup()
        {
            float temp = _pyr.X;
            _pyr.X = _pyr.Y;
            _pyr.Y = _pyr.Z;
            _pyr.Z = temp;
        }

        [XmlIgnore]
        public Vec3 YawRollPitch
        {
            get { return new Vec3(Yaw, Roll, Pitch); }
            set
            {
                BeginUpdate();
                Yaw = value.X;
                Roll = value.Y;
                Pitch = value.Z;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec3 PitchYawRoll
        {
            get { return new Vec3(Pitch, Yaw, Roll); }
            set
            {
                BeginUpdate();
                Pitch = value.X;
                Yaw = value.Y;
                Roll = value.Z;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec3 PitchRollYaw
        {
            get { return new Vec3(Pitch, Roll, Yaw); }
            set
            {
                BeginUpdate();
                Pitch = value.X;
                Roll = value.Y;
                Yaw = value.Z;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec3 RollYawPitch
        {
            get { return new Vec3(Roll, Yaw, Pitch); }
            set
            {
                BeginUpdate();
                Roll = value.X;
                Yaw = value.Y;
                Pitch = value.Z;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec3 RollPitchYaw
        {
            get { return new Vec3(Roll, Pitch, Yaw); }
            set
            {
                BeginUpdate();
                Roll = value.X;
                Pitch = value.Y;
                Yaw = value.Z;
                EndUpdate();
            }
        }

        public void Round(int decimalPlaces)
        {
            BeginUpdate();
            Roll = (float)Math.Round(Roll, decimalPlaces);
            Pitch = (float)Math.Round(Pitch, decimalPlaces);
            Yaw = (float)Math.Round(Yaw, decimalPlaces);
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
            => new Rotator(v.X, v.Y, v.Z, Order.PYR);

        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        public static Rotator GetZero(Order order = Order.YPR)
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
            (Order)Enum.Parse(typeof(Order), parts[3].Substring(0, parts[3].Length - 1)));
        }
        public override string ToString()
        {
            return String.Format("({0}{3} {1}{3} {2}{3} {4})", Pitch, Yaw, Roll, listSeparator, _rotationOrder);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Yaw.GetHashCode();
                hashCode = (hashCode * 397) ^ Pitch.GetHashCode();
                hashCode = (hashCode * 397) ^ Roll.GetHashCode();
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
                Yaw == other.Yaw &&
                Pitch == other.Pitch &&
                Roll == other.Roll;
        }
        public bool Equals(Rotator other, float precision)
        {
            if (ReferenceEquals(other, null))
                return false;
            return
                Abs(Yaw - other.Yaw) < precision &&
                Abs(Pitch - other.Pitch) < precision &&
                Abs(Roll - other.Roll) < precision;
        }
        public bool IsZero()
        {
            return
                Pitch.IsZero() && 
                Yaw.IsZero() && 
                Roll.IsZero();
        }
        public enum Order
        {
            YPR = 0,
            YRP,
            PRY,
            PYR,
            RPY,
            RYP,
        }
    }
}
