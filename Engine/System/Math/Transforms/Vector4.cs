using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using static System.Math;
using static System.CustomMath;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vector4 : IEquatable<Vector4>
    {
        public float X, Y, Z, W;

        public float* Data { get { fixed (void* p = &this) return (float*)p; } }

        public static readonly Vector4 UnitX = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Vector4 UnitY = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
        public static readonly Vector4 UnitZ = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
        public static readonly Vector4 UnitW = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Vector4 Zero = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Vector4 One = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector4());
        
        public Vector4(float value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }
        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Vector4(Vector2 v)
        {
            X = v.X;
            Y = v.Y;
            Z = 0.0f;
            W = 0.0f;
        }
        public Vector4(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = 0.0f;
        }
        public Vector4(Vector3 v, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }
        public Vector4(Vector4 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }
        public float Length { get { return (float)Sqrt(X * X + Y * Y + Z * Z + W * W); } }
        public float LengthFast { get { return 1.0f / InverseSqrtFast(X * X + Y * Y + Z * Z + W * W); } }
        public float LengthSquared { get { return X * X + Y * Y + Z * Z + W * W; } }

        public Vector4 Normalized()
        {
            Vector4 v = this;
            v.Normalize();
            return v;
        }
        public Vector4 NormalizedFast()
        {
            Vector4 v = this;
            v.NormalizeFast();
            return v;
        }
        public void Normalize() { this /= Length; }
        public void NormalizeFast() { this /= LengthFast; }

        public static Vector4 Min(Vector4 a, Vector4 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            a.Z = a.Z < b.Z ? a.Z : b.Z;
            a.W = a.W < b.W ? a.W : b.W;
            return a;
        }
        public static Vector4 Max(Vector4 a, Vector4 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            a.Z = a.Z > b.Z ? a.Z : b.Z;
            a.W = a.W > b.W ? a.W : b.W;
            return a;
        }
        public static Vector4 Clamp(Vector4 vec, Vector4 min, Vector4 max)
        {
            vec.X = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
            vec.Y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
            vec.Z = vec.X < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
            vec.W = vec.Y < min.W ? min.W : vec.W > max.W ? max.W : vec.W;
            return vec;
        }
        public static float Dot(Vector4 left, Vector4 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }
        public float Dot(Vector4 right)
        {
            return X * right.X + Y * right.Y + Z * right.Z + W * right.W;
        }
        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static Vector4 Lerp(Vector4 a, Vector4 b, float blend)
        {
            a.X = blend * (b.X - a.X) + a.X;
            a.Y = blend * (b.Y - a.Y) + a.Y;
            a.Z = blend * (b.Z - a.Z) + a.Z;
            a.W = blend * (b.W - a.W) + a.W;
            return a;
        }
        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
        public static void Lerp(ref Vector4 a, ref Vector4 b, float blend, out Vector4 result)
        {
            result.X = blend * (b.X - a.X) + a.X;
            result.Y = blend * (b.Y - a.Y) + a.Y;
            result.Z = blend * (b.Z - a.Z) + a.Z;
            result.W = blend * (b.W - a.W) + a.W;
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
        public static Vector4 BaryCentric(Vector4 a, Vector4 b, Vector4 c, float u, float v)
        {
            return a + u * (b - a) + v * (c - a);
        }
        /// <summary>Transform a Vector by the given Matrix</summary>
        public static Vector4 operator *(Vector4 vec, Matrix4 mat)
        {
            return new Vector4(
                vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X + vec.W * mat.Row3.X,
                vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y + vec.W * mat.Row3.Y,
                vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z + vec.W * mat.Row3.Z,
                vec.X * mat.Row0.W + vec.Y * mat.Row1.W + vec.Z * mat.Row2.W + vec.W * mat.Row3.W);
        }
        /// <summary>Transform a Vector by the given Matrix using right-handed notation</summary>
        public static Vector4 operator *(Matrix4 mat, Vector4 vec)
        {
            return new Vector4(
                mat.Row0.X * vec.X + mat.Row0.Y * vec.Y + mat.Row0.Z * vec.Z + mat.Row0.W * vec.W,
                mat.Row1.X * vec.X + mat.Row1.Y * vec.Y + mat.Row1.Z * vec.Z + mat.Row1.W * vec.W,
                mat.Row2.X * vec.X + mat.Row2.Y * vec.Y + mat.Row2.Z * vec.Z + mat.Row2.W * vec.W,
                mat.Row3.X * vec.X + mat.Row3.Y * vec.Y + mat.Row3.Z * vec.Z + mat.Row3.W * vec.W);
        }
        
        [XmlIgnore]
        public Vector2 Xy { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }
        [XmlIgnore]
        public Vector2 Xz { get { return new Vector2(X, Z); } set { X = value.X; Z = value.Y; } }
        [XmlIgnore]
        public Vector2 Xw { get { return new Vector2(X, W); } set { X = value.X; W = value.Y; } }
        [XmlIgnore]
        public Vector2 Yx { get { return new Vector2(Y, X); } set { Y = value.X; X = value.Y; } }
        [XmlIgnore]
        public Vector2 Yz { get { return new Vector2(Y, Z); } set { Y = value.X; Z = value.Y; } }
        [XmlIgnore]
        public Vector2 Yw { get { return new Vector2(Y, W); } set { Y = value.X; W = value.Y; } }
        [XmlIgnore]
        public Vector2 Zx { get { return new Vector2(Z, X); } set { Z = value.X; X = value.Y; } }
        [XmlIgnore]
        public Vector2 Zy { get { return new Vector2(Z, Y); } set { Z = value.X; Y = value.Y; } }
        [XmlIgnore]
        public Vector2 Zw { get { return new Vector2(Z, W); } set { Z = value.X; W = value.Y; } }
        [XmlIgnore]
        public Vector2 Wx { get { return new Vector2(W, X); } set { W = value.X; X = value.Y; } }
        [XmlIgnore]
        public Vector2 Wy { get { return new Vector2(W, Y); } set { W = value.X; Y = value.Y; } }
        [XmlIgnore]
        public Vector2 Wz { get { return new Vector2(W, Z); } set { W = value.X; Z = value.Y; } }
        [XmlIgnore]
        public Vector3 Xyz { get { return new Vector3(X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }
        [XmlIgnore]
        public Vector3 Xyw { get { return new Vector3(X, Y, W); } set { X = value.X; Y = value.Y; W = value.Z; } }
        [XmlIgnore]
        public Vector3 Xzy { get { return new Vector3(X, Z, Y); } set { X = value.X; Z = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector3 Xzw { get { return new Vector3(X, Z, W); } set { X = value.X; Z = value.Y; W = value.Z; } }
        [XmlIgnore]
        public Vector3 Xwy { get { return new Vector3(X, W, Y); } set { X = value.X; W = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector3 Xwz { get { return new Vector3(X, W, Z); } set { X = value.X; W = value.Y; Z = value.Z; } }
        [XmlIgnore]
        public Vector3 Yxz { get { return new Vector3(Y, X, Z); } set { Y = value.X; X = value.Y; Z = value.Z; } }
        [XmlIgnore]
        public Vector3 Yxw { get { return new Vector3(Y, X, W); } set { Y = value.X; X = value.Y; W = value.Z; } }
        [XmlIgnore]
        public Vector3 Yzx { get { return new Vector3(Y, Z, X); } set { Y = value.X; Z = value.Y; X = value.Z; } }
        [XmlIgnore]
        public Vector3 Yzw { get { return new Vector3(Y, Z, W); } set { Y = value.X; Z = value.Y; W = value.Z; } }
        [XmlIgnore]
        public Vector3 Ywx { get { return new Vector3(Y, W, X); } set { Y = value.X; W = value.Y; X = value.Z; } }
        [XmlIgnore]
        public Vector3 Ywz { get { return new Vector3(Y, W, Z); } set { Y = value.X; W = value.Y; Z = value.Z; } }
        [XmlIgnore]
        public Vector3 Zxy { get { return new Vector3(Z, X, Y); } set { Z = value.X; X = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector3 Zxw { get { return new Vector3(Z, X, W); } set { Z = value.X; X = value.Y; W = value.Z; } }
        [XmlIgnore]
        public Vector3 Zyx { get { return new Vector3(Z, Y, X); } set { Z = value.X; Y = value.Y; X = value.Z; } }
        [XmlIgnore]
        public Vector3 Zyw { get { return new Vector3(Z, Y, W); } set { Z = value.X; Y = value.Y; W = value.Z; } }
        [XmlIgnore]
        public Vector3 Zwx { get { return new Vector3(Z, W, X); } set { Z = value.X; W = value.Y; X = value.Z; } }
        [XmlIgnore]
        public Vector3 Zwy { get { return new Vector3(Z, W, Y); } set { Z = value.X; W = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector3 Wxy { get { return new Vector3(W, X, Y); } set { W = value.X; X = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector3 Wxz { get { return new Vector3(W, X, Z); } set { W = value.X; X = value.Y; Z = value.Z; } }
        [XmlIgnore]
        public Vector3 Wyx { get { return new Vector3(W, Y, X); } set { W = value.X; Y = value.Y; X = value.Z; } }
        [XmlIgnore]
        public Vector3 Wyz { get { return new Vector3(W, Y, Z); } set { W = value.X; Y = value.Y; Z = value.Z; } }
        [XmlIgnore]
        public Vector3 Wzx { get { return new Vector3(W, Z, X); } set { W = value.X; Z = value.Y; X = value.Z; } }
        [XmlIgnore]
        public Vector3 Wzy { get { return new Vector3(W, Z, Y); } set { W = value.X; Z = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector4 Xywz { get { return new Vector4(X, Y, W, Z); } set { X = value.X; Y = value.Y; W = value.Z; Z = value.W; } }
        [XmlIgnore]
        public Vector4 Xzyw { get { return new Vector4(X, Z, Y, W); } set { X = value.X; Z = value.Y; Y = value.Z; W = value.W; } }
        [XmlIgnore]
        public Vector4 Xzwy { get { return new Vector4(X, Z, W, Y); } set { X = value.X; Z = value.Y; W = value.Z; Y = value.W; } }
        [XmlIgnore]
        public Vector4 Xwyz { get { return new Vector4(X, W, Y, Z); } set { X = value.X; W = value.Y; Y = value.Z; Z = value.W; } }
        [XmlIgnore]
        public Vector4 Xwzy { get { return new Vector4(X, W, Z, Y); } set { X = value.X; W = value.Y; Z = value.Z; Y = value.W; } }
        [XmlIgnore]
        public Vector4 Yxzw { get { return new Vector4(Y, X, Z, W); } set { Y = value.X; X = value.Y; Z = value.Z; W = value.W; } }
        [XmlIgnore]
        public Vector4 Yxwz { get { return new Vector4(Y, X, W, Z); } set { Y = value.X; X = value.Y; W = value.Z; Z = value.W; } }
        [XmlIgnore]
        public Vector4 Yyzw { get { return new Vector4(Y, Y, Z, W); } set { X = value.X; Y = value.Y; Z = value.Z; W = value.W; } }
        [XmlIgnore]
        public Vector4 Yywz { get { return new Vector4(Y, Y, W, Z); } set { X = value.X; Y = value.Y; W = value.Z; Z = value.W; } }
        [XmlIgnore]
        public Vector4 Yzxw { get { return new Vector4(Y, Z, X, W); } set { Y = value.X; Z = value.Y; X = value.Z; W = value.W; } }
        [XmlIgnore]
        public Vector4 Yzwx { get { return new Vector4(Y, Z, W, X); } set { Y = value.X; Z = value.Y; W = value.Z; X = value.W; } }
        [XmlIgnore]
        public Vector4 Ywxz { get { return new Vector4(Y, W, X, Z); } set { Y = value.X; W = value.Y; X = value.Z; Z = value.W; } }
        [XmlIgnore]
        public Vector4 Ywzx { get { return new Vector4(Y, W, Z, X); } set { Y = value.X; W = value.Y; Z = value.Z; X = value.W; } }
        [XmlIgnore]
        public Vector4 Zxyw { get { return new Vector4(Z, X, Y, W); } set { Z = value.X; X = value.Y; Y = value.Z; W = value.W; } }
        [XmlIgnore]
        public Vector4 Zxwy { get { return new Vector4(Z, X, W, Y); } set { Z = value.X; X = value.Y; W = value.Z; Y = value.W; } }
        [XmlIgnore]
        public Vector4 Zyxw { get { return new Vector4(Z, Y, X, W); } set { Z = value.X; Y = value.Y; X = value.Z; W = value.W; } }
        [XmlIgnore]
        public Vector4 Zywx { get { return new Vector4(Z, Y, W, X); } set { Z = value.X; Y = value.Y; W = value.Z; X = value.W; } }
        [XmlIgnore]
        public Vector4 Zwxy { get { return new Vector4(Z, W, X, Y); } set { Z = value.X; W = value.Y; X = value.Z; Y = value.W; } }
        [XmlIgnore]
        public Vector4 Zwyx { get { return new Vector4(Z, W, Y, X); } set { Z = value.X; W = value.Y; Y = value.Z; X = value.W; } }
        [XmlIgnore]
        public Vector4 Zwzy { get { return new Vector4(Z, W, Z, Y); } set { X = value.X; W = value.Y; Z = value.Z; Y = value.W; } }
        [XmlIgnore]
        public Vector4 Wxyz { get { return new Vector4(W, X, Y, Z); } set { W = value.X; X = value.Y; Y = value.Z; Z = value.W; } }
        [XmlIgnore]
        public Vector4 Wxzy { get { return new Vector4(W, X, Z, Y); } set { W = value.X; X = value.Y; Z = value.Z; Y = value.W; } }
        [XmlIgnore]
        public Vector4 Wyxz { get { return new Vector4(W, Y, X, Z); } set { W = value.X; Y = value.Y; X = value.Z; Z = value.W; } }
        [XmlIgnore]
        public Vector4 Wyzx { get { return new Vector4(W, Y, Z, X); } set { W = value.X; Y = value.Y; Z = value.Z; X = value.W; } }
        [XmlIgnore]
        public Vector4 Wzxy { get { return new Vector4(W, Z, X, Y); } set { W = value.X; Z = value.Y; X = value.Z; Y = value.W; } }
        [XmlIgnore]
        public Vector4 Wzyx { get { return new Vector4(W, Z, Y, X); } set { W = value.X; Z = value.Y; Y = value.Z; X = value.W; } }
        [XmlIgnore]
        public Vector4 Wzyw { get { return new Vector4(W, Z, Y, W); } set { X = value.X; Z = value.Y; Y = value.Z; W = value.W; } }

        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            left.W += right.W;
            return left;
        }
        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            left.W -= right.W;
            return left;
        }
        public static Vector4 operator -(Vector4 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
            vec.W = -vec.W;
            return vec;
        }
        public static Vector4 operator *(Vector4 vec, float scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            vec.W *= scale;
            return vec;
        }
        public static Vector4 operator *(float scale, Vector4 vec)
        {
            return vec * scale;
        }
        public static Vector4 operator *(Vector4 vec, Vector4 scale)
        {
            vec.X *= scale.X;
            vec.Y *= scale.Y;
            vec.Z *= scale.Z;
            vec.W *= scale.W;
            return vec;
        }
        public static Vector4 operator *(Vector4 vec, Quaternion quat)
        {
            Quaternion v = new Quaternion(vec.X, vec.Y, vec.Z, vec.W);
            v = quat * v * quat.Inverted();
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static Vector4 operator /(Vector4 vec, float scale)
        {
            vec.X /= scale;
            vec.Y /= scale;
            vec.Z /= scale;
            vec.W /= scale;
            return vec;
        }
        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !left.Equals(right);
        }
        
        private static string listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
        {
            return String.Format("({0}{4} {1}{4} {2}{4} {3})", X, Y, Z, W, listSeparator);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vector4))
                return false;

            return Equals((Vector4)obj);
        }
        public bool Equals(Vector4 other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                W == other.W;
        }
    }
}
