using static System.Math;
using static System.CustomMath;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using CustomEngine.Files;
using CustomEngine;
using System.Xml;
using System.IO;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe class Rotator : IEquatable<Rotator>
    {
        internal const string XMLTag = "rotator";

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

        [Serialize("PitchYawRoll")]
        public Vec3 _pyr;
        [Serialize("Order", IsXmlAttribute = true)]
        public Order _rotationOrder = Order.YPR;

        public event Action Changed;
        private int _updateIndex = 0;

        public static readonly int SizeInBytes = Marshal.SizeOf(new Rotator());
        
        private void BeginUpdate() { ++_updateIndex; }
        private void EndUpdate()
        {
            if (--_updateIndex == 0)
                Changed?.Invoke();
        }
        public Quat GetQuaternion() { return Quat.FromRotator(this); }
        public Matrix4 GetMatrix() { return Matrix4.CreateFromRotator(this); }
        public Matrix4 GetInverseMatrix() { return Matrix4.CreateFromRotator(Inverted()); }
        public Vec3 GetDirection() { return TransformVector(Vec3.Forward); }
        public Vec3 TransformVector(Vec3 vector) { return Vec3.TransformVector(vector, GetMatrix()); }
        public Matrix4 GetYawMatrix() { return Matrix4.CreateRotationY(Yaw); }
        public Matrix4 GetPitchMatrix() { return Matrix4.CreateRotationX(Pitch); }
        public Matrix4 GetRollMatrix() { return Matrix4.CreateRotationZ(Roll); }
        public Quat GetYawQuat() { return Quat.FromAxisAngle(Vec3.Up, Yaw); }
        public Quat GetPitchQuat() { return Quat.FromAxisAngle(Vec3.Right, Pitch); }
        public Quat GetRollQuat() { return Quat.FromAxisAngle(Vec3.Forward, Roll); }
        public void SetDirection(Vec3 value) { SetRotations(value.LookatAngles()); }
        public Rotator HardCopy()
        {
            return new Rotator(Pitch, Yaw, Roll, _rotationOrder);
        }
        public Rotator WithNegatedRotations()
        {
            return new Rotator(-Pitch, -Yaw, -Roll, _rotationOrder);
        }
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

        [XmlIgnore]
        public Vec2 YawPitch
        {
            get { return new Vec2(Yaw, Pitch); }
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
            get { return _pyr.Y; }
            set
            {
                BeginUpdate();
                _pyr.Y = value;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public float Pitch
        {
            get { return _pyr.X; }
            set
            {
                BeginUpdate();
                _pyr.X = value;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public float Roll
        {
            get { return _pyr.Z; }
            set
            {
                BeginUpdate();
                _pyr.Z = value;
                EndUpdate();
            }
        }
        [XmlIgnore]
        public Vec2 YawRoll
        {
            get { return new Vec2(Yaw, Roll); }
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

        public static bool operator ==(Rotator left, Rotator right) { return left.Equals(right); }
        public static bool operator !=(Rotator left, Rotator right) { return !left.Equals(right); }
        
        public static explicit operator Vec3(Rotator v) { return new Vec3(v.Yaw, v.Pitch, v.Roll); }
        public static explicit operator Rotator(Vec3 v) { return new Rotator(v.X, v.Y, v.Z, Order.PYR); }

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
            return
                Yaw == other.Yaw &&
                Pitch == other.Pitch &&
                Roll == other.Roll;
        }
        public bool Equals(Rotator other, float precision)
        {
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
        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement(XMLTag);
            writer.WriteAttributeString("order", _rotationOrder.ToString());
            //writer.WriteElementString("order", _rotationOrder.ToString());
            if (Pitch != 0.0f)
                writer.WriteElementString("pitch", Pitch.ToString());
            if (Yaw != 0.0f)
                writer.WriteElementString("yaw", Yaw.ToString());
            if (Roll != 0.0f)
                writer.WriteElementString("roll", Roll.ToString());
            writer.WriteEndElement();
        }
        public void Read(XMLReader reader)
        {
            if (!reader.Name.Equals(XMLTag, true))
                throw new Exception();
            while (reader.ReadAttribute())
            {
                if (reader.Name.Equals("order", true))
                    _rotationOrder = (Order)Enum.Parse(typeof(Order), (string)reader.Value);
            }
            while (reader.BeginElement())
            {
                //if (reader.Name.Equals("order", true))
                //    _rotationOrder = (Order)Enum.Parse(typeof(Order), reader.ReadElementString());
                if (reader.Name.Equals("pitch", true))
                    _pyr.X = float.Parse(reader.ReadElementString());
                if (reader.Name.Equals("yaw", true))
                    _pyr.Y = float.Parse(reader.ReadElementString());
                if (reader.Name.Equals("roll", true))
                    _pyr.Z = float.Parse(reader.ReadElementString());
                reader.EndElement();
            }
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
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public const int Size = 16;

            public bint _order;
            public BVec3 _pyr;

            public static implicit operator Header(Rotator r)
            {
                return new Header()
                {
                    _order = (int)r._rotationOrder,
                    _pyr = r._pyr
                };
            }
            public static implicit operator Rotator(Header h)
            {
                return new Rotator(h._pyr, (Order)(int)h._order);
            }
        }
    }
}
