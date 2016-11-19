using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using static System.CustomMath;
using static System.Math;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vec2 : IEquatable<Vec2>, IUniformable2Float, IBufferable
    {
        public float X, Y;

        public float* Data { get { return (float*)Address; } }
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType { get { return VertexBuffer.ComponentType.Float; } }
        public int ComponentCount { get { return 2; } }
        bool IBufferable.Normalize { get { return false; } }
        public void Write(VoidPtr address)
        {
            float* data = (float*)address;
            for (int i = 0; i < ComponentCount; ++i)
                *data++ = Data[i];
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

        public float Length { get { return (float)Sqrt(LengthSquared); } }
        public float LengthFast { get { return 1.0f / InverseSqrtFast(LengthSquared); } }
        public float LengthSquared { get { return X * X + Y * Y; } }

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        public Vec2 PerpendicularRight { get { return new Vec2(Y, -X); } }
        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        public Vec2 PerpendicularLeft { get { return new Vec2(-Y, X); } }

        public static readonly Vec2 UnitX = new Vec2(1.0f, 0.0f);
        public static readonly Vec2 UnitY = new Vec2(0.0f, 1.0f);
        public static readonly Vec2 Zero = new Vec2(0.0f, 0.0f);
        public static readonly Vec2 One = new Vec2(1.0f, 1.0f);
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vec2());
        
        public static Vec2 ComponentMin(Vec2 a, Vec2 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            return a;
        }
        public static Vec2 ComponentMax(Vec2 a, Vec2 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            return a;
        }
        public static Vec2 MagnitudeMin(Vec2 left, Vec2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }
        public static Vec2 MagnitudeMax(Vec2 left, Vec2 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }
        public static Vec2 Clamp(Vec2 vec, Vec2 min, Vec2 max)
        {
            vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            return vec;
        }
        public Vec2 Normalized()
        {
            Vec2 v = this;
            v.Normalize();
            return v;
        }
        public Vec2 NormalizedFast()
        {
            Vec2 v = this;
            v.NormalizeFast();
            return v;
        }
        public void Normalize()
        {
            float scale = 1.0f / Length;
            X *= scale;
            Y *= scale;
        }
        public void NormalizeFast()
        {
            float scale = InverseSqrtFast(X * X + Y * Y);
            X *= scale;
            Y *= scale;
        }
        public static float Dot(Vec2 left, Vec2 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }
        public static void Dot(ref Vec2 left, ref Vec2 right, out float result)
        {
            result = left.X * right.X + left.Y * right.Y;
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

        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The perpendicular dot product of the two inputs</param>
        public static void PerpDot(ref Vec2 left, ref Vec2 right, out float result)
        {
            result = left.X * right.Y - left.Y * right.X;
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
        public static Vec2 Transform(Vec2 vec, Quaternion quat)
        {
            Quaternion v = new Quaternion(vec.X, vec.Y, 0, 0);
            Quaternion t = quat * v;
            v = t * quat.Inverted();
            return new Vec2(v.X, v.Y);
        }

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
        public static bool operator ==(Vec2 left, Vec2 right) { return left.Equals(right); }
        public static bool operator !=(Vec2 left, Vec2 right) { return !left.Equals(right); }
        public static explicit operator Vec2(Vec3 v) { return new Vec2(v.X, v.Y); }
        public static explicit operator Vec2(Vec4 v) { return new Vec2(v.X, v.Y); }

        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
        {
            return String.Format("({0}{2} {1})", X, Y, listSeparator);
        }
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
        {
            return
                X == other.X &&
                Y == other.Y;
        }
        public bool Equals(Vec2 other, float precision)
        {
            return
                Abs(X - other.X) < precision &&
                Abs(Y - other.Y) < precision;
        }
    }
}
