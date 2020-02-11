using static System.Math;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.ComponentModel;

namespace System
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct QuatRotator : IEquatable<QuatRotator>
    {
        public static QuatRotator Zero => new QuatRotator(0.0f, 0.0f, 0.0f);
        public QuatRotator(float x, float y, float z)
            => XYZ = new Vec3(x, y, z);
        public QuatRotator(Vec3 xyz)
            => XYZ = xyz;
        
        public Vec3 XYZ;

        public float X { get => XYZ.X; set => XYZ.X = value; }
        public float Y { get => XYZ.Y; set => XYZ.Y = value; }
        public float Z { get => XYZ.Z; set => XYZ.Z = value; }

        public Quat GetResult() =>
            Quat.FromAxisAngle(Vec3.UnitX, XYZ.X) *
            Quat.FromAxisAngle(Vec3.UnitY, XYZ.Y) *
            Quat.FromAxisAngle(Vec3.UnitZ, XYZ.Z);
        public Matrix4 GetMatrix()
            => Matrix4.CreateFromQuaternion(GetResult());

        public void Round(int decimalPlaces)
        {
            XYZ.X = (float)Math.Round(XYZ.X, decimalPlaces);
            XYZ.Y = (float)Math.Round(XYZ.Y, decimalPlaces);
            XYZ.Z = (float)Math.Round(XYZ.Z, decimalPlaces);
        }

        public static bool operator ==(QuatRotator left, QuatRotator right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            return left.Equals(right);
        }
        public static bool operator !=(QuatRotator left, QuatRotator right)
        {
            if (ReferenceEquals(left, null))
                return !ReferenceEquals(right, null);
            return !left.Equals(right);
        }
        
        public static explicit operator Vec3(QuatRotator v) => v.XYZ;
        public static explicit operator QuatRotator(Vec3 v) => new QuatRotator(v);
        
        public override string ToString()
            => XYZ.ToString();
        public override int GetHashCode()
            => XYZ.GetHashCode();
        
        public override bool Equals(object obj)
        {
            if (!(obj is QuatRotator))
                return false;

            return Equals((QuatRotator)obj);
        }
        public bool Equals(QuatRotator other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return
                XYZ.X == other.XYZ.X &&
                XYZ.Y == other.XYZ.Y &&
                XYZ.Z == other.XYZ.Z;
        }
        public bool Equals(QuatRotator other, float precision)
        {
            if (ReferenceEquals(other, null))
                return false;
            return
                Abs(XYZ.X - other.XYZ.X) < precision &&
                Abs(XYZ.Y - other.XYZ.Y) < precision &&
                Abs(XYZ.Z - other.XYZ.Z) < precision;
        }
        public bool IsZero()
        {
            return
                XYZ.X.IsZero() &&
                XYZ.Y.IsZero() &&
                XYZ.Z.IsZero();
        }
    }
}
