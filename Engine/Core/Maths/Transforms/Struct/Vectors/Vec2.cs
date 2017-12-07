using TheraEngine;
using TheraEngine.Rendering.Models;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using static System.TMath;
using static System.Math;
using System.ComponentModel;
using System.Globalization;

namespace System
{
    /// <summary>
    /// A struct containing 2 float values.
    /// For a class version, use EventVec2.
    /// Also see DVec2, IVec2, UVec2, BoolVec2, BVec2
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(Vec2StringConverter))]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vec2 : IEquatable<Vec2>, IUniformable2Float, IBufferable, IParsable
    {
        public float X, Y;

        [Browsable(false)]
        public float* Data => (float*)Address;
        [Browsable(false)]
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        [Browsable(false)]
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 2;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        public void Write(VoidPtr address)
        {
            *(Vec2*)address = this;
        }
        public void Read(VoidPtr address)
        {
            this = *(Vec2*)address;
        }

        public Vec2(float value)
        {
            X = value;
            Y = value;
        }
        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vec2(Vec3 v, bool normalizeWithZ)
        {
            if (normalizeWithZ)
            {
                X = v.X / v.Z;
                Y = v.Y / v.Z;
            }
            else
            {
                X = v.X;
                Y = v.Y;
            }
        }
        public Vec2(string s)
        {
            X = Y = 0.0f;

            char[] delims = new char[] { ',', '(', ')', ' ' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 2)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out X);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out Y);
            }
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 1)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 1)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }
        
        public bool Contains(Vec2 point) => 
            point.X <= X &&
            point.Y <= Y && 
            point.X >= 0.0f && 
            point.Y >= 0.0f;

        public float DistanceToFast(Vec2 otherPoint)
            => (otherPoint - this).LengthFast;
        public float DistanceTo(Vec2 otherPoint)
            => (otherPoint - this).Length;
        public float DistanceToSquared(Vec2 otherPoint)
            => (otherPoint - this).LengthSquared;

        [Browsable(false)]
        public float Length => (float)Sqrt(LengthSquared);
        [Browsable(false)]
        public float LengthFast
        {
            get
            {
                float invLen = InverseSqrtFast(LengthSquared);
                if (invLen != 0.0f)
                    return 1.0f / invLen;
                return 0.0f;
            }
        }
        [Browsable(false)]
        public float LengthSquared => X * X + Y * Y;

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        [Browsable(false)]
        public Vec2 PerpendicularRight => new Vec2(Y, -X);
        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        [Browsable(false)]
        public Vec2 PerpendicularLeft => new Vec2(-Y, X);

        public static readonly Vec2 UnitX = new Vec2(1.0f, 0.0f);
        public static readonly Vec2 UnitY = new Vec2(0.0f, 1.0f);
        public static readonly Vec2 Zero = new Vec2(0.0f);
        public static readonly Vec2 Half = new Vec2(0.5f);
        public static readonly Vec2 One = new Vec2(1.0f);
        
        public static Vec2 ComponentMin(Vec2 a, Vec2 b)
            => new Vec2(
                a.X < b.X ? a.X : b.X,
                a.Y < b.Y ? a.Y : b.Y);

        public static Vec2 ComponentMax(Vec2 a, Vec2 b)
            => new Vec2(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y);

        public static Vec2 MagnitudeMin(Vec2 left, Vec2 right)
            => left.LengthSquared < right.LengthSquared ? left : right;
        public static Vec2 MagnitudeMax(Vec2 left, Vec2 right)
            => left.LengthSquared >= right.LengthSquared ? left : right;

        public void Normalize()
        {
            float len = Length;
            if (len != 0.0f)
                this *= (1.0f / len);
        }
        public Vec2 Normalized()
        {
            Vec2 v = this;
            v.Normalize();
            return v;
        }
        public void NormalizeFast()
        {
            this *= InverseSqrtFast(LengthSquared);
        }
        public Vec2 NormalizedFast()
        {
            Vec2 v = this;
            v.NormalizeFast();
            return v;
        }
        public static float Dot(Vec2 left, Vec2 right)
        {
            return left.Dot(right);
        }
        public float Dot(Vec2 right)
        {
            return X * right.X + Y * right.Y;
        }

        public static Vec2 Clamp(Vec2 value, Vec2 min, Vec2 max)
        {
            Vec2 v;
            v.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
            v.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
            return v;
        }
        public void Clamp(Vec2 min, Vec2 max)
        {
            X = X < min.X ? min.X : X > max.X ? max.X : X;
            Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        }
        public Vec2 Clamped(Vec2 min, Vec2 max)
        {
            Vec2 v;
            v.X = X < min.X ? min.X : X > max.X ? max.X : X;
            v.Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
            return v;
        }

        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The perpendicular dot product of the two inputs</returns>
        public static float PerpDot(Vec2 left, Vec2 right)
        {
            return left.X * right.Y - left.Y * right.X;
        }
        public static Vec2 Lerp(Vec2 a, Vec2 b, float time)
        {
            return a + (b - a) * time;
        }
        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates
        /// </summary>
        /// <param name="a">First input Vector</param>
        /// <param name="b">Second input Vector</param>
        /// <param name="c">Third input Vector</param>
        /// <param name="u">First Barycentric Coordinate</param>
        /// <param name="v">Second Barycentric Coordinate</param>
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
        public static Vec2 BaryCentric(Vec2 a, Vec2 b, Vec2 c, float u, float v)
        {
            return a + u * (b - a) + v * (c - a);
        }
        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <returns>The result of the operation.</returns>
        public static Vec2 Transform(Vec2 vec, Quat quat)
        {
            Quat v = new Quat(vec.X, vec.Y, 0, 0);
            Quat t = quat * v;
            v = t * quat.Inverted();
            return new Vec2(v.X, v.Y);
        }

        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Yx { get { return new Vec2(Y, X); } set { Y = value.X; X = value.Y; } }

        public static Vec2 operator +(Vec2 left, Vec2 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }
        public static Vec2 operator -(Vec2 left, Vec2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }
        public static Vec2 operator -(Vec2 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            return vec;
        }
        public static Vec2 operator *(Vec2 vec, float scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }
        public static Vec2 operator *(float scale, Vec2 vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }
        public static Vec2 operator *(Vec2 vec1, Vec2 vec2)
        {
            vec1.X *= vec2.X;
            vec1.Y *= vec2.Y;
            return vec1;
        }
        public static Vec2 operator /(Vec2 vec1, Vec2 vec2)
        {
            vec1.X /= vec2.X;
            vec1.Y /= vec2.Y;
            return vec1;
        }
        public static Vec2 operator /(Vec2 vec, float scale)
        {
            vec.X /= scale;
            vec.Y /= scale;
            return vec;
        }
        public static Vec2 operator /(float scale, Vec2 vec)
        {
            vec.X = scale / vec.X;
            vec.Y = scale / vec.Y;
            return vec;
        }
        public static Vec2 operator +(Vec2 vec, float amount)
        {
            vec.X = vec.X + amount;
            vec.Y = vec.Y + amount;
            return vec;
        }
        public static Vec2 operator -(Vec2 vec, float amount)
        {
            vec.X = vec.X - amount;
            vec.Y = vec.Y - amount;
            return vec;
        }
        public static Vec2 operator -(float amount, Vec2 vec)
        {
            vec.X = amount - vec.X;
            vec.Y = amount - vec.Y;
            return vec;
        }

        public static bool operator ==(Vec2 left, Vec2 right) => left.Equals(right);
        public static bool operator !=(Vec2 left, Vec2 right) => !left.Equals(right);

        public static explicit operator IVec2(Vec2 v)   => new IVec2((int)Round(v.X), (int)Round(v.Y));
        public static explicit operator Vec2(Vec3 v)    => new Vec2(v.X, v.Y);
        public static explicit operator Vec2(Vec4 v)    => new Vec2(v.X, v.Y);
        public static implicit operator Vec2(PointF v)  => new Vec2(v.X, v.Y);
        public static implicit operator PointF(Vec2 v)  => new PointF(v.X, v.Y);
        public static implicit operator Vec2(SizeF v)   => new Vec2(v.Width, v.Height);
        public static implicit operator SizeF(Vec2 v)   => new SizeF(v.X, v.Y);
        public static implicit operator Vec2(Size v)    => new Vec2(v.Width, v.Height);
        public static explicit operator Size(Vec2 v)    => new Size((int)Round(v.X), (int)Round(v.Y));

        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
            => ToString(true, true);
        public string ToString(bool includeParentheses, bool includeSeparator)
            => String.Format("{3}{0}{2} {1}{4}", X, Y, includeSeparator ? listSeparator : "", includeParentheses ? "(" : "", includeParentheses ? ")" : "");

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vec2))
                return false;

            return Equals((Vec2)obj);
        }
        public bool Equals(Vec2 other)
            =>
                X == other.X &&
                Y == other.Y;
        public bool Equals(Vec2 other, float precision)
            =>
                Abs(X - other.X) < precision &&
                Abs(Y - other.Y) < precision;

        public string WriteToString()
            => ToString(false, false);
        public void ReadFromString(string str)
            => this = new Vec2(str);
    }
}
