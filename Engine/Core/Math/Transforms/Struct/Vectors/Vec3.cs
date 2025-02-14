﻿using Extensions;
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
using static TheraEngine.Core.Maths.TMath;

namespace TheraEngine.Core.Maths.Transforms
{
    /// <summary>
    /// A struct containing 3 float values.
    /// For a class version, use EventVec3.
    /// Also see DVec3, IVec3, UVec3, BoolVec3, BVec3
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(Vec3StringConverter))]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vec3 : IEquatable<Vec3>, IUniformable3Float, IBufferable, ISerializableString, IByteColor
    {
        public static readonly int Size = sizeof(Vec3);

        public float X, Y, Z;

        public Color Color
        {
            get => (Color)this;
            set => Xyz = (Vec3)value;
        }

        [Browsable(false)]
        public float* Data => (float*)Address;
        [Browsable(false)]
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        [Browsable(false)]
        public DataBuffer.EComponentType ComponentType => DataBuffer.EComponentType.Float;
        [Browsable(false)]
        public int ComponentCount => 3;
        bool IBufferable.Normalize => false;
        public void Write(VoidPtr address) => *(Vec3*)address = this;
        public void Read(VoidPtr address) => this = *(Vec3*)address;
        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vec3(float value)
            : this(value, value, value) { }
        public Vec3(Vec2 xy)
            : this(xy.X, xy.Y, 0.0f) { }
        public Vec3(Vec2 xy, float z)
            : this(xy.X, xy.Y, z) { }
        public Vec3(float x, Vec2 yz)
            : this(x, yz.X, yz.Y) { }

        public Vec3(Vec4 v, bool divideByW)
        {
            if (divideByW)
            {
                X = v.X / v.W;
                Y = v.Y / v.W;
                Z = v.Z / v.W;
            }
            else
            {
                X = v.X;
                Y = v.Y;
                Z = v.Z;
            }
        }
        public Vec3(string s, params char[] delimiters)
        {
            X = Y = Z = 0.0f;

            char[] delims = delimiters != null && delimiters.Length > 0 ? delimiters : new char[] { ',', '(', ')', ' ' };
            string[] arr = s.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length >= 3)
            {
                float.TryParse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture, out X);
                float.TryParse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture, out Y);
                float.TryParse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture, out Z);
            }
        }

        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                return Data[index];
            }
            set
            {
                if (index < 0 || index > 2)
                    throw new IndexOutOfRangeException("Cannot access vector at index " + index);
                Data[index] = value;
            }
        }

        /// <summary>
        /// Converts a translation vector to a matrix transformation.
        /// </summary>
        public Matrix4 AsTranslationMatrix() => Matrix4.CreateTranslation(this);
        /// <summary>
        /// Converts a translation vector to an inverse matrix transformation.
        /// </summary>
        public Matrix4 AsInverseTranslationMatrix() => Matrix4.CreateTranslation(-this);

        /// <summary>
        /// Converts a scale vector to a matrix transformation.
        /// </summary>
        /// <returns></returns>
        public Matrix4 AsScaleMatrix() => Matrix4.CreateScale(this);
        /// <summary>
        /// Converts a scale vector to an inverse matrix transformation.
        /// </summary>
        /// <returns></returns>
        public Matrix4 AsInverseScaleMatrix() => Matrix4.CreateScale(1.0f / this);

        /// <summary>
        /// The magnitude of this vector, squared. Also the same thing as the distance to the origin, but squared.
        /// This is to avoid the use of a square root method call.
        /// </summary>
        [Browsable(false)]
        public float LengthSquared => Dot(this);
        /// <summary>
        /// The magnitude of this vector. Also the same thing as the distance to the origin.
        /// </summary>
        [Browsable(false)]
        public float Length => (float)Sqrt(LengthSquared);
        /// <summary>
        /// The magnitude of this vector. Also the same thing as the distance to the origin.
        /// Uses an approximation of square root, so results are faster but not as accurate.
        /// </summary>
        [Browsable(false)]
        public float LengthFast => 1.0f / InverseSqrtFast(LengthSquared);

        public float DistanceTo(Vec3 point) => (point - this).Length;
        public float DistanceToFast(Vec3 point) => (point - this).LengthFast;
        public float DistanceToSquared(Vec3 point) => (point - this).LengthSquared;

        /// <summary>
        /// Normalizes this vector using the requested method.
        /// </summary>
        public void Normalize(ENormalizeOption normalizeMethod)
        {
            switch (normalizeMethod)
            {
                case ENormalizeOption.None:
                    break;
                case ENormalizeOption.Safe:
                    Normalize(true);
                    break;
                case ENormalizeOption.Unsafe:
                    Normalize(false);
                    break;
                case ENormalizeOption.FastSafe:
                    NormalizeFast(true);
                    break;
                case ENormalizeOption.FastUnsafe:
                    NormalizeFast(false);
                    break;
            }
        }
        /// <summary>
        /// Normalizes and returns a copy of this vector using the requested method.
        /// </summary>
        public Vec3 Normalized(ENormalizeOption normalizeMethod)
        {
            Vec3 v = this;
            v.Normalize(normalizeMethod);
            return v;
        }
        /// <summary>
        /// Normalizes this vector to unit length.
        /// For a faster but less accurate method, use NormalizeFast.
        /// </summary>
        /// <param name="safe">If true, checks that the length of this vector is not zero. If it is, does nothing.</param>
        public void Normalize(bool safe = true)
        {
            float lengthSq = LengthSquared;
            if (safe && lengthSq.IsZero()) return;
            this *= 1.0f / (float)Sqrt(lengthSq);
        }
        /// <summary>
        /// Returns a copy of this vector safely normalized to unit length.
        /// For a faster but less accurate method, use NormalizedFast.
        /// </summary>
        public Vec3 Normalized()
        {
            Vec3 v = this;
            v.Normalize();
            return v;
        }
        /// <summary>
        /// Safely normalizes this vector to unit length using an approximation that does not use square root for a minor speed boost at the cost of accuracy.
        /// </summary>
        public void NormalizeFast(bool safe = true)
        {
            float lengthSq = LengthSquared;
            if (safe && lengthSq.IsZero()) return;
            this *= InverseSqrtFast(lengthSq);
        }
        /// <summary>
        /// Returns a copy of this vector safely normalized to unit length using an approximation that does not use square root for a minor speed boost at the cost of accuracy.
        /// </summary>
        public Vec3 NormalizedFast()
        {
            Vec3 v = this;
            v.NormalizeFast();
            return v;
        }

        public void SetLequalTo(Vec3 other)
        {
            X = X < other.X ? X : other.X;
            Y = Y < other.Y ? Y : other.Y;
            Z = Z < other.Z ? Z : other.Z;
        }
        public void SetGequalTo(Vec3 other)
        {
            X = X > other.X ? X : other.X;
            Y = Y > other.Y ? Y : other.Y;
            Z = Z > other.Z ? Z : other.Z;
        }

        public void ChangeZupToYup(bool negateX = true)
        {
            THelpers.Swap(ref Z, ref Y);
            if (negateX)
                X = -X;
        }

        /// <summary>
        /// Unit length (1.0f) vector in the direction of the X-axis.
        /// </summary>
        public static readonly Vec3 UnitX = new Vec3(1.0f, 0.0f, 0.0f);
        /// <summary>
        /// Unit length (1.0f) vector in the direction of the Y-axis.
        /// </summary>
        public static readonly Vec3 UnitY = new Vec3(0.0f, 1.0f, 0.0f);
        /// <summary>
        /// Unit length (1.0f) vector in the direction of the Z-axis.
        /// </summary>
        public static readonly Vec3 UnitZ = new Vec3(0.0f, 0.0f, 1.0f);
        /// <summary>
        /// Vector with all values set to 0.0f.
        /// </summary>
        public static readonly Vec3 Zero = new Vec3(0.0f);
        /// <summary>
        /// Vector with all values set to 0.5f.
        /// </summary>
        public static readonly Vec3 Half = new Vec3(0.5f);
        /// <summary>
        /// Vector with all values set to 1.0f.
        /// </summary>
        public static readonly Vec3 One = new Vec3(1.0f);
        /// <summary>
        /// Vector with all values set to 2.0f.
        /// </summary>
        public static readonly Vec3 Two = new Vec3(2.0f);
        /// <summary>
        /// Vector with all values set to <see cref="float.MinValue"/>.
        /// </summary>
        public static readonly Vec3 Min = new Vec3(float.MinValue);
        /// <summary>
        /// Vector with all values set to <see cref="float.MaxValue"/>.
        /// </summary>
        public static readonly Vec3 Max = new Vec3(float.MaxValue);
        /// <summary>
        /// Unit length (1.0f) vector in the right direction (positive X-axis).
        /// </summary>
        public static readonly Vec3 Right = UnitX;
        /// <summary>
        /// Unit length (1.0f) vector in the left direction (negative X-axis).
        /// </summary>
        public static readonly Vec3 Left = -Right;
        /// <summary>
        /// Unit length (1.0f) vector in the upward direction (positive Y-axis).
        /// </summary>
        public static readonly Vec3 Up = UnitY;
        /// <summary>
        /// Unit length (1.0f) vector in the downward direction (negative Y-axis).
        /// </summary>
        public static readonly Vec3 Down = -Up;
        /// <summary>
        /// Unit length (1.0f) vector in the backward direction (positive Z-axis).
        /// </summary>
        public static readonly Vec3 Backward = -Forward;
        /// <summary>
        /// Unit length (1.0f) vector in the forward direction (negative Z-axis).
        /// </summary>
        public static readonly Vec3 Forward = -UnitZ;
        /// <summary>
        /// 0.299f, 0.587f, 0.114f
        /// </summary>
        public static readonly Vec3 Luminance = new Vec3(0.299f, 0.587f, 0.114f);

        #region Max/Min
        /// <summary>
        /// Returns a Vec3 with the smallest individual components from the given Vec3 values.
        /// </summary>
        public static Vec3 ComponentMin(params Vec3[] values)
        {
            Vec3 value = new Vec3(float.MaxValue);
            foreach (Vec3 v in values)
                value = ComponentMin(v, value);
            return value;
        }
        /// <summary>
        /// Returns a Vec3 with the largest individual components from the given Vec3 values.
        /// </summary>
        public static Vec3 ComponentMax(params Vec3[] values)
        {
            Vec3 value = new Vec3(float.MinValue);
            foreach (Vec3 v in values)
                value = ComponentMax(v, value);
            return value;
        }
        /// <summary>
        /// Returns a Vec3 with the smallest individual components from the two given Vec3 values.
        /// </summary>
        public static Vec3 ComponentMin(Vec3 a, Vec3 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            a.Z = a.Z < b.Z ? a.Z : b.Z;
            return a;
        }
        /// <summary>
        /// Returns a Vec3 with the largest individual components from the two given Vec3 values.
        /// </summary>
        public static Vec3 ComponentMax(Vec3 a, Vec3 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            a.Z = a.Z > b.Z ? a.Z : b.Z;
            return a;
        }
        
        /// <summary>
        /// Returns the Vec3 whose magnitude/length is smallest.
        /// </summary>
        public static Vec3 MagnitudeMin(params Vec3[] values)
        {
            Vec3 value = new Vec3(float.MaxValue);
            foreach (Vec3 v in values)
                value = MagnitudeMin(v, value);
            return value;
        }
        /// <summary>
        /// Returns the Vec3 whose magnitude/length is largest.
        /// </summary>
        public static Vec3 MagnitudeMax(params Vec3[] values)
        {
            Vec3 value = new Vec3(float.MaxValue);
            foreach (Vec3 v in values)
                value = MagnitudeMax(v, value);
            return value;
        }
        /// <summary>
        /// Returns the Vec3 whose magnitude/length is smallest.
        /// </summary>
        public static Vec3 MagnitudeMin(Vec3 a, Vec3 b)
            => a.LengthSquared < b.LengthSquared ? a : b;
        /// <summary>
        /// Returns the Vec3 whose magnitude/length is largest.
        /// </summary>
        public static Vec3 MagnitudeMax(Vec3 a, Vec3 b)
            => a.LengthSquared > b.LengthSquared ? a : b;
        #endregion

        #region Clamping
        public static Vec3 Clamp(Vec3 value, Vec3 min, Vec3 max)
        {
            Vec3 v;
            v.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
            v.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
            v.Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z;
            return v;
        }
        public void Clamp(Vec3 min, Vec3 max)
        {
            X = X < min.X ? min.X : X > max.X ? max.X : X;
            Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
            Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        }
        public Vec3 Clamped(Vec3 min, Vec3 max)
        {
            Vec3 v;
            v.X = X < min.X ? min.X : X > max.X ? max.X : X;
            v.Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
            v.Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
            return v;
        }
        #endregion

        #region Dot/Cross
        /// <summary>
        /// Dot product; 
        /// 1 is same direction (0 degrees difference),
        /// -1 is opposite direction (180 degrees difference), 
        /// 0 is perpendicular (a 90 degree angle)
        /// </summary>
        public static float Dot(Vec3 left, Vec3 right)
            => left.Dot(right);
        /// <summary>
        /// Dot product; 
        /// 1 is same direction (0 degrees difference),
        /// -1 is opposite direction (180 degrees difference), 
        /// 0 is perpendicular (a 90 degree angle)
        /// </summary>
        public float Dot(Vec3 right)
            => X * right.X + Y * right.Y + Z * right.Z;

        //Cross Product:
        //        |
        // normal |  /
        // l x r, | / right
        // -r x l |/_______ 
        //            left

        /// <summary>
        /// Cross Product; if left is up and right is left, then the resulting vector faces you.
        /// </summary>
        public Vec3 Cross(Vec3 right) 
            => new Vec3(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X);

        public static Vec3 Cross(Vec3 left, Vec3 right)
            => left ^ right;
        #endregion

        /// <summary>
        /// Constructs a normal given three points.
        /// Points must be specified in this order 
        /// to ensure the normal points in the right direction.
        ///   ^
        ///   |   p2
        /// n |  /
        ///   | / u
        ///   |/_______ p1
        ///  p0    v
        /// </summary>
        public static Vec3 CalculateNormal(Vec3 point0, Vec3 point1, Vec3 point2)
        {
            //Get two difference vectors between points
            Vec3 v = point1 - point0;
            Vec3 u = point2 - point0;
            //Cross them to get normal vector
            Vec3 normal = v ^ u;
            normal.Normalize();
            return normal;
        }

        public static float AngleBetween(Vec3 vec1, Vec3 vec2, bool returnRadians = false)
        {
            float angle = (float)Acos(vec1 | vec2);
            if (returnRadians)
                return angle;
            return RadToDeg(angle);
        }

        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static Vec3 Lerp(Vec3 a, Vec3 b, float time)
        
            //initial value with a percentage of the difference between the two vectors added to it.
            => a + (b - a) * time;
        
        /// <summary>
        /// Interpolate 3 Vectors using Barycentric coordinates
        /// </summary>
        /// <param name="a">First input Vector</param>
        /// <param name="b">Second input Vector</param>
        /// <param name="c">Third input Vector</param>
        /// <param name="u">First Barycentric Coordinate</param>
        /// <param name="v">Second Barycentric Coordinate</param>
        /// <returns>a when u=v=0, b when u=1,v=0, c when u=0,v=1, and a linear combination of a,b,c otherwise</returns>
        public static Vec3 BaryCentric(Vec3 a, Vec3 b, Vec3 c, float u, float v)
            => a + u * (b - a) + v * (c - a);

        /// <summary>Transform a direction vector by the given Matrix. 
        /// The translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static Vec3 TransformVector(Vec3 vec, Matrix4 mat)
            => new Vec3(
                vec.Dot(new Vec3(mat.Column0, false)), 
                vec.Dot(new Vec3(mat.Column1, false)), 
                vec.Dot(new Vec3(mat.Column2, false)));

        /// <summary>Transform a Normal by the given Matrix</summary>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed normal</returns>
        public static Vec3 TransformNormal(Vec3 norm, Matrix4 mat)
        {
            mat.Invert();
            return TransformNormalInverse(norm, mat);
        }
        /// <summary>Transform a Normal by the (transpose of the) given Matrix</summary>
        /// <remarks>
        /// This version doesn't calculate the inverse matrix.
        /// Use this version if you already have the inverse of the desired transform to hand
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="invMat">The inverse of the desired transformation</param>
        /// <returns>The transformed normal</returns>
        public static Vec3 TransformNormalInverse(Vec3 norm, Matrix4 invMat)
            => TransformVector(norm, invMat.Transposed());

        public void Round(int decimalPlaces)
        {
            X = (float)Math.Round(X, decimalPlaces);
            Y = (float)Math.Round(Y, decimalPlaces);
            Z = (float)Math.Round(Z, decimalPlaces);
        }

        /// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed position</returns>
        public static Vec3 TransformPosition(Vec3 pos, Matrix4 mat)
            => new Vec3(
                pos.Dot(new Vec3(mat.Column0, false)) + mat.Row3.X,
                pos.Dot(new Vec3(mat.Column1, false)) + mat.Row3.Y,
                pos.Dot(new Vec3(mat.Column2, false)) + mat.Row3.Z);

        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static Vec3 Transform(Vec3 vec, Matrix4 mat)
            => new Vec3(
                vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X,
                vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y,
                vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z);

        /// <summary>Transform a Vector by the given Matrix using right-handed notation</summary>
        /// <param name="mat">The desired transformation</param>
        /// <param name="vec">The vector to transform</param>
        /// <param name="result">The transformed vector</param>
        public static Vec3 Transform(Matrix4 mat, Vec3 vec)
            => new Vec3(
                mat.Row0.X * vec.X + mat.Row0.Y * vec.Y + mat.Row0.Z * vec.Z,
                mat.Row1.X * vec.X + mat.Row1.Y * vec.Y + mat.Row1.Z * vec.Z,
                mat.Row2.X * vec.X + mat.Row2.Y * vec.Y + mat.Row2.Z * vec.Z);

        /// <summary>Transform a Vector3 by the given Matrix, and project the resulting Vector4 back to a Vector3</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static Vec3 TransformPerspective(Vec3 vec, Matrix4 mat)
        {
            Vec4 v = new Vec4(vec, 1.0f) * mat;
            return v.Xyz / v.W;
        }
        /// <summary>Transform a Vector3 by the given Matrix using right-handed notation, and project the resulting Vector4 back to a Vector3</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static Vec3 TransformPerspective(Matrix4 mat, Vec3 vec)
        {
            Vec4 v = mat * new Vec4(vec, 1.0f);
            return v.Xyz / v.W;
        }

        /// <summary>
        /// Projects a vector from object space into screen space.
        /// </summary>
        /// <param name="worldPoint">The vector to project.</param>
        /// <param name="x">The X coordinate of the viewport.</param>
        /// <param name="y">The Y coordinate of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The world-view-projection matrix.</param>
        /// <returns>The vector in screen space.</returns>
        /// <remarks>
        /// To project to normalized device coordinates (NDC) use the following parameters:
        /// Project(vector, -1, -1, 2, 2, -1, 1, worldViewProjection).
        /// </remarks>
        public Vec3 Project(float x, float y, float width, float height, float minZ, float maxZ, Matrix4 worldViewProjection)
        {
            Vec3 result = worldViewProjection * this;

            result.X = x + (width * ((result.X + 1.0f) / 2.0f));
            result.Y = y + (height * ((result.Y + 1.0f) / 2.0f));
            result.Z = minZ + ((maxZ - minZ) * ((result.Z + 1.0f) / 2.0f));

            return result;
        }
        /// <summary>
        /// Projects a vector from screen space into object space.
        /// </summary>
        /// <param name="screenPoint">The vector to project.</param>
        /// <param name="x">The X coordinate of the viewport.</param>
        /// <param name="y">The Y coordinate of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The inverse of the world-view-projection matrix.</param>
        /// <returns>The vector in object space.</returns>
        /// <remarks>
        /// To project from normalized device coordinates (NDC) use the following parameters:
        /// Project(vector, -1, -1, 2, 2, -1, 1, inverseWorldViewProjection).
        /// </remarks>
        public Vec3 Unproject(float x, float y, float width, float height, float minZ, float maxZ, Matrix4 inverseWorldViewProjection)
            => inverseWorldViewProjection * new Vec3(
                (X - x) / width * 2.0f - 1.0f,
                (Y - y) / height * 2.0f - 1.0f,
                Z / (maxZ - minZ) * 2.0f - 1.0f);

        #region Angles

        #region Calculate Angle
        /// <summary>
        /// Calculates the angle (in degrees) between two vectors in the range [0, 180].
        /// Uses an approximation of square root, so slightly less accurate but faster.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>Angle (in degrees) between the vectors.</returns>
        public float CalculateDegAngleFast(Vec3 other)
            => RadToDeg(CalculateRadAngleFast(other));
        /// <summary>
        /// Calculates the angle (in degrees) between two vectors in the range [0, 180].
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>Angle (in degrees) between the vectors.</returns>
        public float CalculateDegAngle(Vec3 other)
            => RadToDeg(CalculateRadAngle(other));
        /// <summary>
        /// Calculates the angle (in radians) between two vectors in the range [0, pi].
        /// Uses an approximation of square root, so slightly less accurate but faster.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        public float CalculateRadAngleFast(Vec3 other)
            => (float)Acos((Dot(other) / (LengthFast * other.LengthFast)).Clamp(-1.0f, 1.0f));
        /// <summary>
        /// Calculates the angle (in radians) between two vectors in the range [0, pi].
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        public float CalculateRadAngle(Vec3 other)
            => (float)Acos((Dot(other) / (Length * other.Length)).Clamp(-1.0f, 1.0f));

        /// <summary>
        /// Calculates the angle (in degrees) between two vectors in the range [0, 180].
        /// Uses an approximation of square root, so slightly less accurate but faster.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>Angle (in degrees) between the vectors.</returns>
        public static float CalculateDegAngleFast(Vec3 left, Vec3 right)
            => left.CalculateDegAngleFast(right);
        /// <summary>
        /// Calculates the angle (in degrees) between two vectors in the range [0, 180].
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>Angle (in degrees) between the vectors.</returns>
        public static float CalculateDegAngle(Vec3 left, Vec3 right)
            => left.CalculateDegAngle(right);
        /// <summary>
        /// Calculates the angle (in radians) between two vectors in the range [0, pi].
        /// Uses an approximation of square root, so slightly less accurate but faster.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        public static float CalculateRadAngleFast(Vec3 left, Vec3 right)
            => left.CalculateRadAngleFast(right);
        /// <summary>
        /// Calculates the angle (in radians) between two vectors in the range [0, pi].
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        public static float CalculateRadAngle(Vec3 left, Vec3 right)
            => left.CalculateRadAngle(right);
        #endregion

        ///// <summary>
        ///// Returns a YPR rotator relative to -Z with azimuth as yaw, elevation as pitch, and 0 as roll.
        ///// </summary>
        //public Rotator LookatAngles()
        //    => LookatAngles(Forward);
        ///// <summary>
        ///// Returns a YPR rotator relative to the start normal with azimuth as yaw, elevation as pitch, and 0 as roll.
        ///// </summary>
        //public Rotator LookatAngles(Vec3 startNormal)
        //{

        //    return Quat.BetweenVectors(startNormal, this).ToYawPitchRoll();
        //}

        public Vec3 GetAngles()
            => new Vec3(AngleX(), AngleY(), AngleZ());
        public Vec3 GetAngles(Vec3 origin)
            => (this - origin).GetAngles();
        public float AngleX()
            => RadToDeg((float)Atan2(Y, -Z));
        public float AngleY()
            => RadToDeg((float)Atan2(-Z, X));
        public float AngleZ()
            => RadToDeg((float)Atan2(Y, X));

        /// <summary>
        /// Returns a YPR rotator looking from the origin to the end of this vector.
        /// </summary>
        public Rotator LookatAngles() => new Rotator(
            RadToDeg((float)Atan2(Y, Sqrt(X * X + Z * Z))),
            RadToDeg((float)Atan2(-X, -Z)),
            0.0f);

        /// <summary>
        /// Returns a YPR rotator looking from the origin to this point.
        /// </summary>
        public Rotator LookatAngles(Vec3 origin)
            => (this - origin).LookatAngles();

        //public void LookatAngles(Vec3 startNormal, out float yaw, out float pitch)
        //{
        //    pitch = RadToDeg((float)Atan2(Y, Sqrt(X * X + Z * Z)));
        //    yaw = RadToDeg((float)Atan2(-X, -Z));
        //    //yaw = RadToDeg((float)Atan2(-Z, X));
        //}
        //public Rotator LookatAngles(Vec3 origin, Vec3 startNormal, Vec3 forward, Vec3 right, Vec3 up)
        //{
        //    return (this - origin).LookatAngles(startNormal, forward, right, up);
        //}
        //public Rotator LookatAngles(Vec3 forward, Vec3 right, Vec3 up)
        //{
        //    return LookatAngles(Forward, forward, right, up);
        //}
        //public Rotator LookatAngles(Vec3 startNormal, Vec3 forward, Vec3 right, Vec3 up)
        //{
        //    Vec3 NormalDir = (this - startNormal).GetSafeNormal();
        //    //Find projected point (on AxisX and AxisY, remove AxisZ component)
        //    Vec3 NoZProjDir = (NormalDir - (NormalDir | up) * up).GetSafeNormal();
        //    //Figure out if projection is on right or left.
        //    float AzimuthSign = ((NoZProjDir | right) < 0.0f) ? -1.0f : 1.0f;
        //    float ElevationSin = NormalDir | up;
        //    float AzimuthCos = NoZProjDir | forward;
        //    return new Rotator(
        //        RadToDeg((float)Acos(AzimuthCos)) * AzimuthSign, 
        //        RadToDeg((float)Asin(ElevationSin)), 
        //        0.0f, Rotator.Order.YPR);
        //}
        //public Rotator LookatAngles(Vec3 origin)
        //{
        //    return LookatAngles(origin, Forward);
        //}
        //public Rotator LookatAngles(Vec3 origin, Vec3 startNormal)
        //{
        //    return (this - origin).LookatAngles(startNormal);
        //}
        //public void LookatAngles(Vec3 origin, out float yaw, out float pitch)
        //{
        //    LookatAngles(origin, Forward, out yaw, out pitch);
        //}
        //public void LookatAngles(Vec3 origin, Vec3 startNormal, out float yaw, out float pitch)
        //{
        //    (this - origin).LookatAngles(startNormal, out yaw, out pitch);
        //}
        #endregion

        public Vec3 GetSafeNormal(float Tolerance = 1.0e-8f)
        {
            float SquareSum = LengthSquared;
            if (SquareSum == 1.0f)
		        return this;	
	        else if (SquareSum < Tolerance)
		        return Zero;
            float Scale = InverseSqrtFast(SquareSum);
	        return new Vec3(X * Scale, Y * Scale, Z * Scale);
        }
        public bool IsInTriangle(Vec3 triPt1, Vec3 triPt2, Vec3 triPt3)
        {
            Vec3 v0 = triPt2 - triPt1;
            Vec3 v1 = triPt3 - triPt1;
            Vec3 v2 = this - triPt1;

            float dot00 = v0.Dot(v0);
            float dot01 = v0.Dot(v1);
            float dot02 = v0.Dot(v2);
            float dot11 = v1.Dot(v1);
            float dot12 = v1.Dot(v2);
            
            float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return u >= 0.0f && v >= 0.0f && u + v < 1.0f;
        }
        public bool BarycentricCoordsWithin(
            Vec3 triPt1, Vec3 triPt2, Vec3 triPt3,
            out float u, out float v, out float w)
        {
            Vec3 v0 = triPt2 - triPt1;
            Vec3 v1 = triPt3 - triPt1;
            Vec3 v2 = this - triPt1;

            float d00 = v0.Dot(v0);
            float d01 = v0.Dot(v1);
            float d02 = v0.Dot(v2);
            float d11 = v1.Dot(v1);
            float d12 = v1.Dot(v2);

            float invDenom = 1.0f / (d00 * d11 - d01 * d01);
            v = (d11 * d02 - d01 * d12) * invDenom;
            w = (d00 * d12 - d01 * d02) * invDenom;
            u = 1.0f - v - w;

            return u >= 0.0f && v >= 0.0f && u + v < 1.0f;
        }
        /// <summary>
        /// Returns a vector pointing out of a plane, given the plane's normal and a vector to be reflected which is pointing at the plane.
        /// </summary>
        public static Vec3 ReflectionVector(Vec3 normal, Vec3 vector)
        {
            normal.Normalize();
            return vector - 2.0f * (vector | normal) * normal;
        }
        /// <summary>
        /// Returns a vector pointing out of a plane, given the plane's normal and this vector to be reflected which is pointing at the plane.
        /// </summary>
        public Vec3 ReflectionVector(Vec3 normal)
        {
            normal.Normalize();
            return this - 2.0f * (this | normal) * normal;
        }
        /// <summary>
        /// Returns the portion of this Vec3 that is parallel to the given normal.
        /// </summary>
        public Vec3 ParallelComponent(Vec3 normal)
        {
            normal.Normalize();
            return normal * (this | normal);
        }
        
        /// <summary>
        /// Returns the portion of this Vec3 that is perpendicular to the given normal.
        /// </summary>
        public Vec3 PerpendicularComponent(Vec3 normal) 
            => this - ParallelComponent(normal);

        #region Sub Vectors
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Xy
        {
            get => new Vec2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Xz
        {
            get => new Vec2(X, Z);
            set
            {
                X = value.X;
                Z = value.Y;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Yx
        {
            get => new Vec2(Y, X);
            set
            {
                Y = value.X;
                X = value.Y;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Yz
        {
            get => new Vec2(Y, Z);
            set
            {
                Y = value.X;
                Z = value.Y;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Zx
        {
            get => new Vec2(Z, X);
            set
            {
                Z = value.X;
                X = value.Y;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec2 Zy
        {
            get => new Vec2(Z, Y);
            set
            {
                Z = value.X;
                Y = value.Y;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 Xzy
        {
            get => new Vec3(X, Z, Y);
            set
            {
                X = value.X;
                Z = value.Y;
                Y = value.Z;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 Yxz
        {
            get => new Vec3(Y, X, Z);
            set
            {
                Y = value.X;
                X = value.Y;
                Z = value.Z;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 Yzx
        {
            get => new Vec3(Y, Z, X);
            set
            {
                Y = value.X;
                Z = value.Y;
                X = value.Z;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 Zxy
        {
            get => new Vec3(Z, X, Y);
            set
            {
                Z = value.X;
                X = value.Y;
                Y = value.Z;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 Zyx
        {
            get => new Vec3(Z, Y, X);
            set
            {
                Z = value.X;
                Y = value.Y;
                X = value.Z;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Vec3 Xyz
        {
            get => new Vec3(X, Y, Z);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
        #endregion

        #region Operators
        public static Vec3 operator +(float left, Vec3 right)
            => right + left;
        public static Vec3 operator +(Vec3 left, float right)
            => new Vec3(
                left.X + right,
                left.Y + right,
                left.Z + right);

        public static Vec3 operator -(float left, Vec3 right)
            => new Vec3(
                left - right.X,
                left - right.Y,
                left - right.Z);

        public static Vec3 operator -(Vec3 left, float right)
            => new Vec3(
                left.X - right, 
                left.Y - right, 
                left.Z - right);

        public static Vec3 operator +(Vec3 left, Vec3 right)
            => new Vec3(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z);

        public static Vec3 operator -(Vec3 left, Vec3 right)
            => new Vec3(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z);

        public static Vec3 operator -(Vec3 vec)
            => new Vec3(-vec.X, -vec.Y, -vec.Z);
        public static Vec3 operator *(Vec3 vec, float scale)
            => new Vec3(
                vec.X * scale,
                vec.Y * scale,
                vec.Z * scale);

        public static Vec3 operator *(float scale, Vec3 vec)
            => vec * scale;
        public static Vec3 operator *(Vec3 vec, Vec3 scale)
            => new Vec3(
                vec.X * scale.X,
                vec.Y * scale.Y,
                vec.Z * scale.Z);
        
        public static bool operator ==(Vec3 left, Vec3 right)
            => left.Equals(right);
        public static bool operator !=(Vec3 left, Vec3 right)
            => !left.Equals(right);
        public static bool operator <(Vec3 left, Vec3 right)
            => 
                left.X < right.X &&
                left.Y < right.Y && 
                left.Z < right.Z;
        public static bool operator >(Vec3 left, Vec3 right)
            => 
                left.X > right.X &&
                left.Y > right.Y && 
                left.Z > right.Z;

        public static bool operator <=(Vec3 left, Vec3 right)
            =>
                left.X <= right.X && 
                left.Y <= right.Y && 
                left.Z <= right.Z;

        public static bool operator >=(Vec3 left, Vec3 right)
            => 
                left.X >= right.X && 
                left.Y >= right.Y && 
                left.Z >= right.Z;
        public static Vec3 operator /(Vec3 vec, float scale)
        {
            scale = 1.0f / scale;
            return new Vec3(
                vec.X * scale,
                vec.Y * scale,
                vec.Z * scale);
        }
        public static Vec3 operator /(float scale, Vec3 vec)
            => new Vec3(
                scale / vec.X,
                scale / vec.Y,
                scale / vec.Z);

        public static Vec3 operator /(Vec3 left, Vec3 right)
            => new Vec3(
                left.X / right.X, 
                left.Y / right.Y,
                left.Z / right.Z);
        
        //Cross Product:
        //        |
        // normal |  /
        // l x r, | / right
        // -r x l |/_______ 
        //            left

        /// <summary>
        /// Cross Product; if left is up and right is left, then the resulting vector faces you.
        /// </summary>
        public static Vec3 operator ^(Vec3 left, Vec3 right)
            => left.Cross(right);

        /// <summary>
        /// Dot product; 
        /// 1 is same direction (0 degrees difference),
        /// -1 is opposite direction (180 degrees difference), 
        /// 0 is perpendicular (a 90 degree angle)
        /// </summary>
        public static float operator |(Vec3 vec1, Vec3 vec2)
            => vec1.Dot(vec2);

        public static Vec3 operator *(Matrix4 left, Vec3 right)
            => TransformPerspective(left, right);
        
        public static Vec3 operator *(Vec3 left, Matrix4 right)
            => TransformPerspective(left, right);
        
        public static explicit operator Vec3(Color c)
            => new Vec3(c.R * THelpers.ByteToFloat, c.G * THelpers.ByteToFloat, c.B * THelpers.ByteToFloat);
        public static explicit operator Color(Vec3 v)
            => Color.FromArgb(v.X.ToByte(), v.Y.ToByte(), v.Z.ToByte());

        public static implicit operator Vec3(Vec2 v)    => new Vec3(v.X, v.Y, 0.0f);
        public static explicit operator Vec3(ColorF4 v) => new Vec3(v.R, v.G, v.B);
        public static implicit operator Vec3(float v)   => new Vec3(v);
        public static explicit operator IVec3(Vec3 v)   => new IVec3((int)Math.Round(v.X), (int)Math.Round(v.Y), (int)Math.Round(v.Z));

        public static implicit operator OpenTK.Vector3(Vec3 v) => new OpenTK.Vector3(v.X, v.Y, v.Z);
        public static implicit operator Vec3(OpenTK.Vector3 v) => new Vec3(v.X, v.Y, v.Z);
        public static implicit operator BulletSharp.Vector3(Vec3 v) => new BulletSharp.Vector3(v.X, v.Y, v.Z);
        public static implicit operator Vec3(BulletSharp.Vector3 v) => new Vec3(v.X, v.Y, v.Z);
        public static implicit operator Jitter.LinearMath.JVector(Vec3 v) => new Jitter.LinearMath.JVector(v.X, v.Y, v.Z);
        public static implicit operator Vec3(Jitter.LinearMath.JVector v) => new Vec3(v.X, v.Y, v.Z);
        public static implicit operator Valve.VR.HmdVector3_t(Vec3 v) => new Valve.VR.HmdVector3_t() { v0 = v.X, v1 = v.Y, v2 = v.Z };
        public static implicit operator Vec3(Valve.VR.HmdVector3_t v) => new Vec3(v.v0, v.v1, v.v2);

        #endregion

        public override string ToString() => ToString();
        public string ToString(string openingBracket = "(", string closingBracket = ")", string separator = ", ")
            => string.Format("{4}{0}{3}{1}{3}{2}{5}", X, Y, Z, separator, openingBracket, closingBracket);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vec3))
                return false;

            return Equals((Vec3)obj);
        }
        public bool Equals(Vec3 other)
            =>
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;

        public bool Equals(Vec3 other, float precision)
            =>
                Abs(X - other.X) < precision &&
                Abs(Y - other.Y) < precision &&
                Abs(Z - other.Z) < precision;

        public string WriteToString() => ToString("", "", " ");
        public void ReadFromString(string str) => this = new Vec3(str);
    }
}
