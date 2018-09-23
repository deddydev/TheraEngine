using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using static System.Math;
using static System.TMath;

namespace TheraEngine.Core.Maths.Transforms
{
    /// <summary>
    /// A struct containing 4 float values.
    /// For a class version, use EventVec4.
    /// Also see DVec4, IVec4, UVec4, BoolVec4, BVec4
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(Vec4StringConverter))]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vec4 : IEquatable<Vec4>, IUniformable4Float, IBufferable, IParsable, IByteColor
    {
        public static readonly int Size = sizeof(Vec4);

        public float X, Y, Z, W;

        public Color Color { get => (Color)this; set => Xyzw = (Vec4)value; }

        [Browsable(false)]
        public float* Data => (float*)Address;
        [Browsable(false)]
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        [Browsable(false)]
        public DataBuffer.ComponentType ComponentType => DataBuffer.ComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 4;
        [Browsable(false)]
        bool IBufferable.Normalize => false;

        public void Write(VoidPtr address) { *(Vec4*)address = this; }
        public void Read(VoidPtr address) { this = *(Vec4*)address; }

        public static readonly Vec4 UnitX = new Vec4(1.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Vec4 UnitY = new Vec4(0.0f, 1.0f, 0.0f, 0.0f);
        public static readonly Vec4 UnitZ = new Vec4(0.0f, 0.0f, 1.0f, 0.0f);
        public static readonly Vec4 UnitW = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Vec4 Zero = new Vec4(0.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Vec4 One = new Vec4(1.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Vec4 Min = new Vec4(float.MinValue);
        public static readonly Vec4 Max = new Vec4(float.MaxValue);

        public Vec4(float value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }
        public Vec4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Vec4(Vec2 v)
        {
            X = v.X;
            Y = v.Y;
            Z = 0.0f;
            W = 0.0f;
        }
        public Vec4(Vec3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = 0.0f;
        }
        public Vec4(Vec3 v, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }
        public Vec4(Vec4 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }
        public Vec4(string s, params char[] delimiters)
        {
            X = Y = Z = W = 0.0f;

            char[] delims = delimiters != null && delimiters.Length > 0 ? delimiters : new char[] { ',', '(', ')', ' ' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 4)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out X);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out Y);
                float.TryParse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture, out Z);
                float.TryParse(arr[3], NumberStyles.Any, CultureInfo.InvariantCulture, out W);
            }
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

        [Browsable(false)]
        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;
        [Browsable(false)]
        public float Length => (float)Sqrt(LengthSquared);
        [Browsable(false)]
        public float LengthFast => 1.0f / InverseSqrtFast(LengthSquared);

        public void Normalize() { this *= (1.0f / Length); }
        public Vec4 Normalized()
        {
            Vec4 v = this;
            v.Normalize();
            return v;
        }
        public void NormalizeFast() { this *= InverseSqrtFast(LengthSquared); }
        public Vec4 NormalizedFast()
        {
            Vec4 v = this;
            v.NormalizeFast();
            return v;
        }

        public static Vec4 ComponentMin(Vec4 a, Vec4 b)
        {
            return new Vec4(
                a.X < b.X ? a.X : b.X,
                a.Y < b.Y ? a.Y : b.Y,
                a.Z < b.Z ? a.Z : b.Z,
                a.W < b.W ? a.W : b.W);
        }
        public static Vec4 ComponentMax(Vec4 a, Vec4 b)
        {
            return new Vec4(
                a.X > b.X ? a.X : b.X,
                a.Y > b.Y ? a.Y : b.Y,
                a.Z > b.Z ? a.Z : b.Z,
                a.W > b.W ? a.W : b.W);
        }
        public static Vec4 Clamp(Vec4 vec, Vec4 min, Vec4 max)
        {
            return new Vec4(
                vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X,
                vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y,
                vec.X < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z,
                vec.Y < min.W ? min.W : vec.W > max.W ? max.W : vec.W);
        }
        public static float Dot(Vec4 left, Vec4 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }
        public float Dot(Vec4 right)
        {
            return X * right.X + Y * right.Y + Z * right.Z + W * right.W;
        }
        public static Vec4 Lerp(Vec4 a, Vec4 b, float time)
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
        public static Vec4 BaryCentric(Vec4 a, Vec4 b, Vec4 c, float u, float v)
        {
            return a + u * (b - a) + v * (c - a);
        }
        /// <summary>Transform a Vector by the given Matrix</summary>
        public static Vec4 operator *(Vec4 vec, Matrix4 mat)
        {
            Vec4 nv = new Vec4();
            float* dPtr = nv.Data;
            float* sPtr = vec.Data;
            float* 
                row1 = (float*)&mat, 
                row2 = row1 + 4, 
                row3 = row2 + 4, 
                row4 = row3 + 4;

            for (int i = 0; i < 4; i++)
                dPtr[i] = 
                    row1[i] * sPtr[0] + 
                    row2[i] * sPtr[1] + 
                    row3[i] * sPtr[2] + 
                    row4[i] * sPtr[3];

            return nv;
        }
        /// <summary>Transform a Vector by the given Matrix using right-handed notation</summary>
        public static Vec4 operator *(Matrix4 mat, Vec4 vec)
        {
            Vec4 nv = new Vec4();
            float* dPtr = nv.Data;
            float* sPtr = vec.Data;
            Vec4* row = (Vec4*)&mat;

            for (int i = 0; i < 4; i++)
                dPtr[i] =
                    row[i].X * sPtr[0] +
                    row[i].Y * sPtr[1] +
                    row[i].Z * sPtr[2] +
                    row[i].W * sPtr[3];

            return nv;
        }
        
        [XmlIgnore]
        public Vec2 Xy
        {
            get { return new Vec2(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Xz
        {
            get { return new Vec2(X, Z); }
            set
            {
                X = value.X;
                Z = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Xw
        {
            get { return new Vec2(X, W); }
            set
            {
                X = value.X;
                W = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Yx
        {
            get { return new Vec2(Y, X); }
            set
            {
                Y = value.X;
                X = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Yz
        {
            get { return new Vec2(Y, Z); }
            set
            {
                Y = value.X;
                Z = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Yw
        {
            get { return new Vec2(Y, W); }
            set
            {
                Y = value.X;
                W = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Zx
        {
            get { return new Vec2(Z, X); }
            set
            {
                Z = value.X;
                X = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Zy
        {
            get { return new Vec2(Z, Y); }
            set
            {
                Z = value.X;
                Y = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Zw
        {
            get { return new Vec2(Z, W); }
            set
            {
                Z = value.X;
                W = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Wx
        {
            get { return new Vec2(W, X); }
            set
            {
                W = value.X;
                X = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Wy
        {
            get { return new Vec2(W, Y); }
            set
            {
                W = value.X;
                Y = value.Y;
            }
        }
        [XmlIgnore]
        public Vec2 Wz
        {
            get { return new Vec2(W, Z); }
            set
            {
                W = value.X;
                Z = value.Y;
            }
        }
        [XmlIgnore]
        public Vec3 Xyz
        {
            get { return new Vec3(X, Y, Z); }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Xyw
        {
            get { return new Vec3(X, Y, W); }
            set
            {
                X = value.X;
                Y = value.Y;
                W = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Xzy
        {
            get { return new Vec3(X, Z, Y); }
            set
            {
                X = value.X;
                Z = value.Y;
                Y = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Xzw
        {
            get { return new Vec3(X, Z, W); }
            set
            {
                X = value.X;
                Z = value.Y;
                W = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Xwy
        {
            get { return new Vec3(X, W, Y); }
            set
            {
                X = value.X;
                W = value.Y;
                Y = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Xwz
        {
            get { return new Vec3(X, W, Z); }
            set
            {
                X = value.X;
                W = value.Y;
                Z = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Yxz
        {
            get { return new Vec3(Y, X, Z); }
            set
            {
                Y = value.X;
                X = value.Y;
                Z = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Yxw
        {
            get { return new Vec3(Y, X, W); }
            set
            {
                Y = value.X;
                X = value.Y;
                W = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Yzx
        {
            get { return new Vec3(Y, Z, X); }
            set
            {
                Y = value.X;
                Z = value.Y;
                X = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Yzw
        {
            get { return new Vec3(Y, Z, W); }
            set
            {
                Y = value.X;
                Z = value.Y;
                W = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Ywx
        {
            get { return new Vec3(Y, W, X); }
            set
            {
                Y = value.X;
                W = value.Y;
                X = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Ywz
        {
            get { return new Vec3(Y, W, Z); }
            set
            {
                Y = value.X;
                W = value.Y;
                Z = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Zxy
        {
            get { return new Vec3(Z, X, Y); }
            set
            {
                Z = value.X;
                X = value.Y;
                Y = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Zxw
        {
            get { return new Vec3(Z, X, W); }
            set
            {
                Z = value.X;
                X = value.Y;
                W = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Zyx
        {
            get { return new Vec3(Z, Y, X); }
            set
            {
                Z = value.X;
                Y = value.Y;
                X = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Zyw
        {
            get { return new Vec3(Z, Y, W); }
            set
            {
                Z = value.X;
                Y = value.Y;
                W = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Zwx
        {
            get { return new Vec3(Z, W, X); }
            set
            {
                Z = value.X;
                W = value.Y;
                X = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Zwy
        {
            get { return new Vec3(Z, W, Y); }
            set
            {
                Z = value.X;
                W = value.Y;
                Y = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Wxy
        {
            get { return new Vec3(W, X, Y); }
            set
            {
                W = value.X;
                X = value.Y;
                Y = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Wxz
        {
            get { return new Vec3(W, X, Z); }
            set
            {
                W = value.X;
                X = value.Y;
                Z = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Wyx
        {
            get { return new Vec3(W, Y, X); }
            set
            {
                W = value.X;
                Y = value.Y;
                X = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Wyz
        {
            get { return new Vec3(W, Y, Z); }
            set
            {
                W = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Wzx
        {
            get { return new Vec3(W, Z, X); }
            set
            {
                W = value.X;
                Z = value.Y;
                X = value.Z;
            }
        }
        [XmlIgnore]
        public Vec3 Wzy
        {
            get { return new Vec3(W, Z, Y); }
            set
            {
                W = value.X;
                Z = value.Y;
                Y = value.Z;
            }
        }
        [XmlIgnore]
        public Vec4 Xywz
        {
            get { return new Vec4(X, Y, W, Z); }
            set { X = value.X; Y = value.Y; W = value.Z; Z = value.W; }
        }
        [XmlIgnore]
        public Vec4 Xzyw
        {
            get { return new Vec4(X, Z, Y, W); }
            set { X = value.X; Z = value.Y; Y = value.Z; W = value.W; }
        }
        [XmlIgnore]
        public Vec4 Xzwy
        {
            get { return new Vec4(X, Z, W, Y); }
            set { X = value.X; Z = value.Y; W = value.Z; Y = value.W; }
        }
        [XmlIgnore]
        public Vec4 Xwyz
        {
            get { return new Vec4(X, W, Y, Z); }
            set { X = value.X; W = value.Y; Y = value.Z; Z = value.W; }
        }
        [XmlIgnore]
        public Vec4 Xwzy
        {
            get { return new Vec4(X, W, Z, Y); }
            set { X = value.X; W = value.Y; Z = value.Z; Y = value.W; }
        }
        [XmlIgnore]
        public Vec4 Yxzw
        {
            get { return new Vec4(Y, X, Z, W); }
            set { Y = value.X; X = value.Y; Z = value.Z; W = value.W; }
        }
        [XmlIgnore]
        public Vec4 Yxwz
        {
            get { return new Vec4(Y, X, W, Z); }
            set { Y = value.X; X = value.Y; W = value.Z; Z = value.W; }
        }
        [XmlIgnore]
        public Vec4 Yyzw
        {
            get { return new Vec4(Y, Y, Z, W); }
            set { X = value.X; Y = value.Y; Z = value.Z; W = value.W; }
        }
        [XmlIgnore]
        public Vec4 Yywz
        {
            get { return new Vec4(Y, Y, W, Z); }
            set { X = value.X; Y = value.Y; W = value.Z; Z = value.W; }
        }
        [XmlIgnore]
        public Vec4 Yzxw
        {
            get { return new Vec4(Y, Z, X, W); }
            set { Y = value.X; Z = value.Y; X = value.Z; W = value.W; }
        }
        [XmlIgnore]
        public Vec4 Yzwx
        {
            get { return new Vec4(Y, Z, W, X); }
            set { Y = value.X; Z = value.Y; W = value.Z; X = value.W; }
        }
        [XmlIgnore]
        public Vec4 Ywxz
        {
            get { return new Vec4(Y, W, X, Z); }
            set { Y = value.X; W = value.Y; X = value.Z; Z = value.W; }
        }
        [XmlIgnore]
        public Vec4 Ywzx
        {
            get { return new Vec4(Y, W, Z, X); }
            set { Y = value.X; W = value.Y; Z = value.Z; X = value.W; }
        }
        [XmlIgnore]
        public Vec4 Zxyw
        {
            get { return new Vec4(Z, X, Y, W); }
            set { Z = value.X; X = value.Y; Y = value.Z; W = value.W; }
        }
        [XmlIgnore]
        public Vec4 Zxwy
        {
            get { return new Vec4(Z, X, W, Y); }
            set { Z = value.X; X = value.Y; W = value.Z; Y = value.W; }
        }
        [XmlIgnore]
        public Vec4 Zyxw
        {
            get { return new Vec4(Z, Y, X, W); }
            set { Z = value.X; Y = value.Y; X = value.Z; W = value.W; }
        }
        [XmlIgnore]
        public Vec4 Zywx
        {
            get { return new Vec4(Z, Y, W, X); }
            set { Z = value.X; Y = value.Y; W = value.Z; X = value.W; }
        }
        [XmlIgnore]
        public Vec4 Zwxy
        {
            get { return new Vec4(Z, W, X, Y); }
            set { Z = value.X; W = value.Y; X = value.Z; Y = value.W; }
        }
        [XmlIgnore]
        public Vec4 Zwyx
        {
            get { return new Vec4(Z, W, Y, X); }
            set { Z = value.X; W = value.Y; Y = value.Z; X = value.W; }
        }
        [XmlIgnore]
        public Vec4 Zwzy
        {
            get { return new Vec4(Z, W, Z, Y); }
            set { X = value.X; W = value.Y; Z = value.Z; Y = value.W; }
        }
        [XmlIgnore]
        public Vec4 Wxyz
        {
            get { return new Vec4(W, X, Y, Z); }
            set { W = value.X; X = value.Y; Y = value.Z; Z = value.W; }
        }
        [XmlIgnore]
        public Vec4 Wxzy
        {
            get { return new Vec4(W, X, Z, Y); }
            set { W = value.X; X = value.Y; Z = value.Z; Y = value.W; }
        }
        [XmlIgnore]
        public Vec4 Wyxz
        {
            get { return new Vec4(W, Y, X, Z); }
            set { W = value.X; Y = value.Y; X = value.Z; Z = value.W; }
        }
        [XmlIgnore]
        public Vec4 Wyzx
        {
            get { return new Vec4(W, Y, Z, X); }
            set { W = value.X; Y = value.Y; Z = value.Z; X = value.W; }
        }
        [XmlIgnore]
        public Vec4 Wzxy
        {
            get { return new Vec4(W, Z, X, Y); }
            set { W = value.X; Z = value.Y; X = value.Z; Y = value.W; }
        }
        [XmlIgnore]
        public Vec4 Wzyx
        {
            get { return new Vec4(W, Z, Y, X); }
            set { W = value.X; Z = value.Y; Y = value.Z; X = value.W; }
        }
        [XmlIgnore]
        public Vec4 Wzyw
        {
            get { return new Vec4(W, Z, Y, W); }
            set { X = value.X; Z = value.Y; Y = value.Z; W = value.W; }
        }
        [XmlIgnore]
        public Vec4 Xyzw
        {
            get { return new Vec4(X, Y, Z, W); }
            set { X = value.X; Y = value.Y; Z = value.Z; W = value.W; }
        }

        public static Vec4 operator +(Vec4 left, Vec4 right)
        {
            return new Vec4(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z,
                left.W + right.W);
        }
        public static Vec4 operator -(Vec4 left, Vec4 right)
        {
            return new Vec4(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z,
                left.W - right.W);
        }
        public static Vec4 operator -(Vec4 vec)
        {
            return new Vec4(
                -vec.X,
                -vec.Y,
                -vec.Z,
                -vec.W);
        }
        public static Vec4 operator *(Vec4 vec, float scale)
        {
            return new Vec4(
                vec.X * scale,
                vec.Y * scale,
                vec.Z * scale,
                vec.W * scale);
        }
        public static Vec4 operator /(Vec4 vec, float scale)
        {
            return vec * (1.0f / scale);
        }
        public static Vec4 operator *(float scale, Vec4 vec)
        {
            return vec * scale;
        }
        public static Vec4 operator *(Vec4 vec, Vec4 scale)
            => new Vec4(
                vec.X * scale.X,
                vec.Y * scale.Y,
                vec.Z * scale.Z,
                vec.W * scale.W);
        public static bool operator ==(Vec4 left, Vec4 right)
            => left.Equals(right);
        public static bool operator !=(Vec4 left, Vec4 right)
            => !left.Equals(right);
        public static implicit operator BulletSharp.Vector4(Vec4 v)
        {
            return new BulletSharp.Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static implicit operator Vec4(BulletSharp.Vector4 v)
        {
            return new Vec4(v.X, v.Y, v.Z, v.W);
        }
        public static implicit operator Vec4(float v) => new Vec4(v);
        public static implicit operator Vec4(Color c)
            => new Vec4(c.R * THelpers.ByteToFloat, c.G * THelpers.ByteToFloat, c.B * THelpers.ByteToFloat, c.A * THelpers.ByteToFloat);
        public static explicit operator Color(Vec4 v)
            => Color.FromArgb(v.W.ToByte(), v.X.ToByte(), v.Y.ToByte(), v.Z.ToByte());
        private static readonly string listSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString() => ToString();
        public string ToString(string openingBracket = "(", string closingBracket = ")", string separator = ", ")
           => string.Format("{5}{0}{4}{1}{4}{2}{4}{3}{6}", X, Y, Z, W, separator, openingBracket, closingBracket);

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
            if (!(obj is Vec4))
                return false;

            return Equals((Vec4)obj);
        }
        public bool Equals(Vec4 other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                W == other.W;
        }
        public bool Equals(Vec4 other, float precision)
        {
            return
                Abs(X - other.X) < precision &&
                Abs(Y - other.Y) < precision &&
                Abs(Z - other.Z) < precision &&
                Abs(W - other.W) < precision;
        }
        public string WriteToString()
            => ToString("", "", " ");
        public void ReadFromString(string str)
            => this = new Vec4(str);
    }
}