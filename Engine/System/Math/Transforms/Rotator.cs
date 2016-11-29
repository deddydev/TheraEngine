using static System.Math;
using static System.CustomMath;
using CustomEngine;
using System.Runtime.InteropServices;
using CustomEngine.Rendering.Models;
using System.Xml.Serialization;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Rotator : IEquatable<Rotator>, IUniformable3Float, IBufferable
    {
        public Rotator(Order order) : this(0.0f, 0.0f, 0.0f, order) { }
        public Rotator(float yaw, float pitch, float roll, Order rotationOrder)
        { Yaw = yaw; Pitch = pitch; Roll = roll; RotationOrder = rotationOrder; }

        public float Yaw, Pitch, Roll;
        public Order RotationOrder;
        
        public float* Data { get { return (float*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Float; } }
        public int ComponentCount { get { return 3; } }
        bool IBufferable.Normalize { get { return false; } }
        public void Write(VoidPtr address)
        {
            float* data = (float*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *data++ = Data[i];
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access rotator at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access rotator at index " + index);
                Data[index] = value;
            }
        }

        public static readonly int SizeInBytes = Marshal.SizeOf(new Rotator());

        public static Rotator FromQuaternion()
        {
            throw new NotImplementedException();
        }
        public Quaternion GetQuaternion() { return Quaternion.FromRotator(this); }
        public Matrix4 GetMatrix() { return Matrix4.CreateFromRotator(this); }
        public Matrix4 GetInverseMatrix() { return Matrix4.CreateFromRotator(Inverted()); }
        public Vec3 GetDirection() { return TransformVector(Vec3.Forward); }
        public Vec3 TransformVector(Vec3 vector) { return Vec3.TransformVector(vector, GetMatrix()); }
        public Matrix4 GetYawMatrix() { return Matrix4.CreateRotationY(Yaw); }
        public Matrix4 GetPitchMatrix() { return Matrix4.CreateRotationX(Pitch); }
        public Matrix4 GetRollMatrix() { return Matrix4.CreateRotationZ(Roll); }
        public Quaternion GetYawQuat() { return Quaternion.FromAxisAngle(Vec3.Up, Yaw); }
        public Quaternion GetPitchQuat() { return Quaternion.FromAxisAngle(Vec3.Right, Pitch); }
        public Quaternion GetRollQuat() { return Quaternion.FromAxisAngle(Vec3.Forward, Roll); }
        public Rotator WithNegatedRotations()
        {
            return new Rotator(-Yaw, -Pitch, -Roll, RotationOrder);
        }
        public void NegateRotations()
        {
            Yaw = -Yaw;
            Pitch = -Pitch;
            Roll = -Roll;
        }
        public void ReverseRotations()
        {
            Yaw = Yaw + 180.0f;
            Pitch = Pitch + 180.0f;
            Roll = Roll + 180.0f;
        }
        public void ClearWinding()
        {
            Yaw = Yaw.RemapToRange(0.0f, 360.0f);
            Pitch = Pitch.RemapToRange(0.0f, 360.0f);
            Roll = Roll.RemapToRange(0.0f, 360.0f);
        }
        public int GetYawWindCount() { return (int)(Yaw / 360.0f); }
        public int GetPitchWindCount() { return (int)(Pitch / 360.0f); }
        public int GetRollWindCount() { return (int)(Roll / 360.0f); }
        public void ReverseOrder() { RotationOrder = OppositeOf(RotationOrder); }
        public void Invert()
        {
            NegateRotations();
            ReverseOrder();
        }
        public Rotator Inverted()
        {
            return new Rotator(-Yaw, -Pitch, -Roll, OppositeOf(RotationOrder));
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
            Rotator v = new Rotator();
            v.Yaw = value.Yaw < min.Yaw ? min.Yaw : value.Yaw > max.Yaw ? max.Yaw : value.Yaw;
            v.Pitch = value.Pitch < min.Pitch ? min.Pitch : value.Pitch > max.Pitch ? max.Pitch : value.Pitch;
            v.Roll = value.Roll < min.Roll ? min.Roll : value.Roll > max.Roll ? max.Roll : value.Roll;
            return v;
        }
        public void Clamp(Rotator min, Rotator max)
        {
            Yaw = Yaw < min.Yaw ? min.Yaw : Yaw > max.Yaw ? max.Yaw : Yaw;
            Pitch = Pitch < min.Pitch ? min.Pitch : Pitch > max.Pitch ? max.Pitch : Pitch;
            Roll = Roll < min.Roll ? min.Roll : Roll > max.Roll ? max.Roll : Roll;
        }
        public Rotator Clamped(Rotator min, Rotator max)
        {
            Rotator v = new Rotator();
            v.Yaw = Yaw < min.Yaw ? min.Yaw : Yaw > max.Yaw ? max.Yaw : Yaw;
            v.Pitch = Pitch < min.Pitch ? min.Pitch : Pitch > max.Pitch ? max.Pitch : Pitch;
            v.Roll = Roll < min.Roll ? min.Roll : Roll > max.Roll ? max.Roll : Roll;
            return v;
        }
        
        [XmlIgnore]
        public Vec2 YawPitch { get { return new Vec2(Yaw, Pitch); } set { Yaw = value.X; Pitch = value.Y; } }
        [XmlIgnore]
        public Vec2 YawRoll { get { return new Vec2(Yaw, Roll); } set { Yaw = value.X; Roll = value.Y; } }
        [XmlIgnore]
        public Vec2 PitchYaw { get { return new Vec2(Pitch, Yaw); } set { Pitch = value.X; Yaw = value.Y; } }
        [XmlIgnore]
        public Vec2 PitchRoll { get { return new Vec2(Pitch, Roll); } set { Pitch = value.X; Roll = value.Y; } }
        [XmlIgnore]
        public Vec2 RollYaw { get { return new Vec2(Roll, Yaw); } set { Roll = value.X; Yaw = value.Y; } }
        [XmlIgnore]
        public Vec2 RollPitch { get { return new Vec2(Roll, Pitch); } set { Roll = value.X; Pitch = value.Y; } }
        [XmlIgnore]
        public Vec3 YawPitchRoll { get { return new Vec3(Yaw, Pitch, Roll); } set { Yaw = value.X; Pitch = value.Y; Roll = value.Z; } }
        [XmlIgnore]
        public Vec3 YawRollPitch { get { return new Vec3(Yaw, Roll, Pitch); } set { Yaw = value.X; Roll = value.Y; Pitch = value.Z; } }
        [XmlIgnore]
        public Vec3 PitchYawRoll { get { return new Vec3(Pitch, Yaw, Roll); } set { Pitch = value.X; Yaw = value.Y; Roll = value.Z; } }
        [XmlIgnore]
        public Vec3 PitchRollYaw { get { return new Vec3(Pitch, Roll, Yaw); } set { Pitch = value.X; Roll = value.Y; Yaw = value.Z; } }
        [XmlIgnore]
        public Vec3 RollYawPitch { get { return new Vec3(Roll, Yaw, Pitch); } set { Roll = value.X; Yaw = value.Y; Pitch = value.Z; } }
        [XmlIgnore]
        public Vec3 RollPitchYaw { get { return new Vec3(Roll, Pitch, Yaw); } set { Roll = value.X; Pitch = value.Y; Yaw = value.Z; } }
        
        public static bool operator ==(Rotator left, Rotator right) { return left.Equals(right); }
        public static bool operator !=(Rotator left, Rotator right) { return !left.Equals(right); }
        
        public static implicit operator Vec3(Rotator v) { return new Vec3(v.Yaw, v.Pitch, v.Roll); }
        public static implicit operator Rotator(Vec3 v) { return new Rotator(v.X, v.Y, v.Z, Order.YPR); }

        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public static Rotator GetZero(Order order = Order.YPR)
        {
           return  new Rotator(0.0f, 0.0f, 0.0f, order);
        }

        public override string ToString()
        {
            return String.Format("({0}{3} {1}{3} {2})", Yaw, Pitch, Roll, listSeparator);
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
