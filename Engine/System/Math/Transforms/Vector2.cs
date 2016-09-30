using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using static System.CustomMath;
using static System.Math;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vector2 : IEquatable<Vector2>
    {
        public float X, Y;

        public float* Data { get { fixed (void* p = &this) return (float*)p; } }

        public Vector2(float value)
        {
            X = value;
            Y = value;
        }
        public Vector2(float x, float y)
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

        public float Length { get { return (float)Sqrt(X * X + Y * Y); } }
        public float LengthFast { get { return 1.0f / InverseSqrtFast(X * X + Y * Y); } }
        public float LengthSquared { get { return X * X + Y * Y; } }

        /// <summary>
        /// Gets the perpendicular vector on the right side of this vector.
        /// </summary>
        public Vector2 PerpendicularRight { get { return new Vector2(Y, -X); } }
        /// <summary>
        /// Gets the perpendicular vector on the left side of this vector.
        /// </summary>
        public Vector2 PerpendicularLeft { get { return new Vector2(-Y, X); } }

        public static readonly Vector2 UnitX = new Vector2(1.0f, 0.0f);
        public static readonly Vector2 UnitY = new Vector2(0.0f, 1.0f);
        public static readonly Vector2 Zero = new Vector2(0.0f, 0.0f);
        public static readonly Vector2 One = new Vector2(1.0f, 1.0f);

        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector2());
        
        public static Vector2 ComponentMin(Vector2 a, Vector2 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            return a;
        }
        public static Vector2 ComponentMax(Vector2 a, Vector2 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            return a;
        }
        public static Vector2 MagnitudeMin(Vector2 left, Vector2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }
        public static Vector2 MagnitudeMax(Vector2 left, Vector2 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }
        public static Vector2 Clamp(Vector2 vec, Vector2 min, Vector2 max)
        {
            vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            return vec;
        }
        public Vector2 Normalized()
        {
            Vector2 v = this;
            v.Normalize();
            return v;
        }
        public Vector2 NormalizedFast()
        {
            Vector2 v = this;
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
        public static float Dot(Vector2 left, Vector2 right)
        {
            return left.X * right.X + left.Y * right.Y;
        }
        public static void Dot(ref Vector2 left, ref Vector2 right, out float result)
        {
            result = left.X * right.X + left.Y * right.Y;
        }
        
        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The perpendicular dot product of the two inputs</returns>
        public static float PerpDot(Vector2 left, Vector2 right)
        {
            return left.X * right.Y - left.Y * right.X;
        }

        /// <summary>
        /// Calculate the perpendicular dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <param name="result">The perpendicular dot product of the two inputs</param>
        public static void PerpDot(ref Vector2 left, ref Vector2 right, out float result)
        {
            result = left.X * right.Y - left.Y * right.X;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float blend)
        {
            a.X = blend * (b.X - a.X) + a.X;
            a.Y = blend * (b.Y - a.Y) + a.Y;
            return a;
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
        public static void Lerp(ref Vector2 a, ref Vector2 b, float blend, out Vector2 result)
        {
            result.X = blend * (b.X - a.X) + a.X;
            result.Y = blend * (b.Y - a.Y) + a.Y;
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
        public static Vector2 BaryCentric(Vector2 a, Vector2 b, Vector2 c, float u, float v)
        {
            return a + u * (b - a) + v * (c - a);
        }
        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector2 Transform(Vector2 vec, Quaternion quat)
        {
            Quaternion v = new Quaternion(vec.X, vec.Y, 0, 0);
            Quaternion t = quat * v;
            v = t * quat.Inverted();
            return new Vector2(v.X, v.Y);
        }

        [XmlIgnore]
        public Vector2 Yx { get { return new Vector2(Y, X); } set { Y = value.X; X = value.Y; } }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }
        public static Vector2 operator -(Vector2 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            return vec;
        }
        public static Vector2 operator *(Vector2 vec, float scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }
        public static Vector2 operator *(float scale, Vector2 vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            return vec;
        }
        public static Vector2 operator *(Vector2 vec1, Vector2 vec2)
        {
            vec1.X *= vec2.X;
            vec1.Y *= vec2.Y;
            return vec1;
        }
        public static Vector2 operator /(Vector2 vec1, Vector2 vec2)
        {
            vec1.X /= vec2.X;
            vec1.Y /= vec2.Y;
            return vec1;
        }
        public static Vector2 operator /(Vector2 vec, float scale)
        {
            vec.X /= scale;
            vec.Y /= scale;
            return vec;
        }
        public static Vector2 operator /(float scale, Vector2 vec)
        {
            vec.X = scale / vec.X;
            vec.Y = scale / vec.Y;
            return vec;
        }
        public static bool operator ==(Vector2 left, Vector2 right) { return left.Equals(right); }
        public static bool operator !=(Vector2 left, Vector2 right) { return !left.Equals(right); }
        public static explicit operator Vector2(Vector3 v) { return new Vector2(v.X, v.Y); }
        public static explicit operator Vector2(Vector4 v) { return new Vector2(v.X, v.Y); }
        
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
            if (!(obj is Vector2))
                return false;

            return Equals((Vector2)obj);
        }
        public bool Equals(Vector2 other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }
    }
}
