using System;
using System.Runtime.InteropServices;
using static System.Math;
using static System.CustomMath;
using System.Xml.Serialization;
using System.Drawing;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vector3 : IEquatable<Vector3>
    {
        public float X, Y, Z;

        public float* Data { get { fixed (void* p = &this) return (float*)p; } }

        public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
        public Vector3(float value) : this(value, value, value) { }
        public Vector3(Vector2 v) : this(v.X, v.Y, 0.0f) { }
        public Vector3(Vector4 v) : this(v.X, v.Y, v.Z) { }

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

        public float Length { get { return (float)Sqrt(LengthSquared); } }
        public float LengthFast { get { return 1.0f / InverseSqrtFast(LengthSquared); } }
        public float LengthSquared { get { return Dot(this); } }

        public Vector3 Normalized()
        {
            Vector3 v = this;
            v.Normalize();
            return v;
        }
        public Vector3 NormalizedFast()
        {
            Vector3 v = this;
            v.NormalizeFast();
            return v;
        }
        public Vector3 Normalized(Vector3 origin)
        {
            return (this - origin).Normalized();
        }
        public Vector3 NormalizedFast(Vector3 origin)
        {
            return (this - origin).NormalizedFast();
        }
        public void Normalize() { this /= Length; }
        public void NormalizeFast() { this /= LengthFast; }

        public void SetLequalTo(Vector3 other)
        {
            if (!(this <= other))
            {
                X = other.X;
                Y = other.Y;
                Z = other.Z;
            }
        }
        public void SetGequalTo(Vector3 other)
        {
            if (!(this >= other))
            {
                X = other.X;
                Y = other.Y;
                Z = other.Z;
            }
        }

        public static readonly Vector3 UnitX = new Vector3(1.0f, 0.0f, 0.0f);
        public static readonly Vector3 UnitY = new Vector3(0.0f, 1.0f, 0.0f);
        public static readonly Vector3 UnitZ = new Vector3(0.0f, 0.0f, 1.0f);
        public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);
        public static readonly Vector3 One = new Vector3(1.0f, 1.0f, 1.0f);
        public static readonly int SizeInBytes = Marshal.SizeOf(new Vector3());
        
        public static Vector3 ComponentMin(Vector3 a, Vector3 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            a.Z = a.Z < b.Z ? a.Z : b.Z;
            return a;
        }
        public static Vector3 ComponentMax(Vector3 a, Vector3 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            a.Z = a.Z > b.Z ? a.Z : b.Z;
            return a;
        }
        public static Vector3 MagnitudeMin(Vector3 left, Vector3 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }
        public static Vector3 MagnitudeMax(Vector3 left, Vector3 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }
        public void Clamp(Vector3 min, Vector3 max)
        {
            X = X < min.X ? min.X : X > max.X ? max.X : X;
            Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
            Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        }

        public float Dot(Vector3 right)
        {
            return X * right.X + Y * right.Y + Z * right.Z;
        }
        public Vector3 Cross(Vector3 right)
        {
            return new Vector3(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X);
        }
        /// <summary>
        /// Returns a new Vector that is the linear blend of the 2 given Vectors
        /// </summary>
        /// <param name="a">First input vector</param>
        /// <param name="b">Second input vector</param>
        /// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
        /// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float blend)
        {
            a.X = blend * (b.X - a.X) + a.X;
            a.Y = blend * (b.Y - a.Y) + a.Y;
            a.Z = blend * (b.Z - a.Z) + a.Z;
            return a;
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
        public static Vector3 BaryCentric(Vector3 a, Vector3 b, Vector3 c, float u, float v)
        {
            return a + u * (b - a) + v * (c - a);
        }

        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static Vector3 TransformVector(Vector3 vec, Matrix4 mat)
        {
            return new Vector3(
                vec.Dot(new Vector3(mat.Column0)), 
                vec.Dot(new Vector3(mat.Column1)), 
                vec.Dot(new Vector3(mat.Column2)));
        }

        /// <summary>Transform a Normal by the given Matrix</summary>
        /// <remarks>
        /// This calculates the inverse of the given matrix, use TransformNormalInverse if you
        /// already have the inverse to avoid this extra calculation
        /// </remarks>
        /// <param name="norm">The normal to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed normal</returns>
        public static Vector3 TransformNormal(Vector3 norm, Matrix4 mat)
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
        public static Vector3 TransformNormalInverse(Vector3 norm, Matrix4 invMat)
        {
            return TransformVector(norm, invMat.Transposed());
        }
        /// <summary>Transform a Position by the given Matrix</summary>
        /// <param name="pos">The position to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed position</returns>
        public static Vector3 TransformPosition(Vector3 pos, Matrix4 mat)
        {
            Vector3 p;
            p.X = pos.Dot(new Vector3(mat.Column0)) + mat.Row3.X;
            p.Y = pos.Dot(new Vector3(mat.Column1)) + mat.Row3.Y;
            p.Z = pos.Dot(new Vector3(mat.Column2)) + mat.Row3.Z;
            return p;
        }
        /// <summary>
        /// Transforms a vector by a quaternion rotation.
        /// </summary>
        /// <param name="vec">The vector to transform.</param>
        /// <param name="quat">The quaternion to rotate the vector by.</param>
        /// <returns>The result of the operation.</returns>
        public Vector3 Transform(Quaternion quat)
        {
            // Since vec.W == 0, we can optimize quat * vec * quat^-1 as follows:
            // vec + 2.0 * cross(quat.xyz, cross(quat.xyz, vec) + quat.w * vec)
            Vector3 xyz = quat.Xyz;
            return this + 2.0f * xyz.Cross(xyz.Cross(this) + this * quat.W);
        }
        /// <summary>Transform a Vector by the given Matrix</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static Vector3 Transform(Vector3 vec, Matrix4 mat)
        {
            return new Vector3(
                vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X,
                vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y,
                vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z);
        }
        /// <summary>Transform a Vector by the given Matrix using right-handed notation</summary>
        /// <param name="mat">The desired transformation</param>
        /// <param name="vec">The vector to transform</param>
        /// <param name="result">The transformed vector</param>
        public static Vector3 Transform(Matrix4 mat, Vector3 vec)
        {
            return new Vector3(
                mat.Row0.X * vec.X + mat.Row0.Y * vec.Y + mat.Row0.Z * vec.Z,
                mat.Row1.X * vec.X + mat.Row1.Y * vec.Y + mat.Row1.Z * vec.Z,
                mat.Row2.X * vec.X + mat.Row2.Y * vec.Y + mat.Row2.Z * vec.Z);
        }
        /// <summary>Transform a Vector3 by the given Matrix, and project the resulting Vector4 back to a Vector3</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static Vector3 TransformPerspective(Vector3 vec, Matrix4 mat)
        {
            Vector4 v = new Vector4(vec, 1.0f) * mat;
            return v.Xyz / v.W;
        }
        /// <summary>Transform a Vector3 by the given Matrix using right-handed notation, and project the resulting Vector4 back to a Vector3</summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <param name="result">The transformed vector</param>
        public static Vector3 TransformPerspective(Matrix4 mat, Vector3 vec)
        {
            Vector4 v = mat * new Vector4(vec, 1.0f);
            return v.Xyz / v.W;
        }

        /// <summary>
        /// Calculates the angle (in degrees) between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        /// <remarks>Note that the returned angle is never bigger than 180.</remarks>
        public float CalculateAngle(Vector3 second)
        {
            return RadToDeg((float)Acos((Dot(second) / (LengthFast * second.LengthFast)).Clamp(-1.0f, 1.0f)));
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
        public Vector3 Project(float x, float y, float width, float height, float minZ, float maxZ, Matrix4 worldViewProjection)
        {
            Vector3 result = this * worldViewProjection;

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
        public Vector3 Unproject(float x, float y, float width, float height, float minZ, float maxZ, Matrix4 inverseWorldViewProjection)
        {
            return new Vector3(
                (X - x) / width * 2.0f - 1.0f,
                (Y - y) / height * 2.0f - 1.0f,
                Z / (maxZ - minZ) * 2.0f - 1.0f) * inverseWorldViewProjection;
        }

        [XmlIgnore]
        public Vector2 Xy { get { return new Vector2(X, Y); } set { X = value.X; Y = value.Y; } }
        [XmlIgnore]
        public Vector2 Xz { get { return new Vector2(X, Z); } set { X = value.X; Z = value.Y; } }
        [XmlIgnore]
        public Vector2 Yx { get { return new Vector2(Y, X); } set { Y = value.X; X = value.Y; } }
        [XmlIgnore]
        public Vector2 Yz { get { return new Vector2(Y, Z); } set { Y = value.X; Z = value.Y; } }
        [XmlIgnore]
        public Vector2 Zx { get { return new Vector2(Z, X); } set { Z = value.X; X = value.Y; } }
        [XmlIgnore]
        public Vector2 Zy { get { return new Vector2(Z, Y); } set { Z = value.X; Y = value.Y; } }
        [XmlIgnore]
        public Vector3 Xzy { get { return new Vector3(X, Z, Y); } set { X = value.X; Z = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector3 Yxz { get { return new Vector3(Y, X, Z); } set { Y = value.X; X = value.Y; Z = value.Z; } }
        [XmlIgnore]
        public Vector3 Yzx { get { return new Vector3(Y, Z, X); } set { Y = value.X; Z = value.Y; X = value.Z; } }
        [XmlIgnore]
        public Vector3 Zxy { get { return new Vector3(Z, X, Y); } set { Z = value.X; X = value.Y; Y = value.Z; } }
        [XmlIgnore]
        public Vector3 Zyx { get { return new Vector3(Z, Y, X); } set { Z = value.X; Y = value.Y; X = value.Z; } }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            return left;
        }
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            return left;
        }
        public static Vector3 operator -(Vector3 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
            return vec;
        }
        public static Vector3 operator *(Vector3 vec, float scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }
        public static Vector3 operator *(float scale, Vector3 vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }
        public static Vector3 operator *(Vector3 vec, Vector3 scale)
        {
            vec.X *= scale.X;
            vec.Y *= scale.Y;
            vec.Z *= scale.Z;
            return vec;
        }
        public static Vector3 operator *(Quaternion quat, Vector3 vec)
        {
            return vec.Transform(quat);
        }
        public static Vector3 operator /(Vector3 vec, float scale)
        {
            float mult = 1.0f / scale;
            vec.X *= mult;
            vec.Y *= mult;
            vec.Z *= mult;
            return vec;
        }
        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !left.Equals(right);
        }
        public static bool operator <(Vector3 left, Vector3 right)
        {
            return left.X < right.X && left.Y < right.Y && left.Z < right.Z;
        }
        public static bool operator >(Vector3 left, Vector3 right)
        {
            return left.X > right.X && left.Y > right.Y && left.Z > right.Z;
        }
        public static bool operator <=(Vector3 left, Vector3 right)
        {
            return left.X <= right.X && left.Y <= right.Y && left.Z <= right.Z;
        }
        public static bool operator >=(Vector3 left, Vector3 right)
        {
            return left.X >= right.X && left.Y >= right.Y && left.Z >= right.Z;
        }
        public static Vector3 operator /(float scale, Vector3 vec)
        {
            return new Vector3(scale) / vec;
        }
        public static Vector3 operator /(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.X / vec2.X, vec1.Y / vec2.Y, vec1.Z / vec2.Z);
        }
        public static Vector3 operator *(Matrix4 left, Vector3 right)
        {
            return TransformPerspective(left, right);
        }
        public static Vector3 operator *(Vector3 left, Matrix4 right)
        {
            return TransformPerspective(left, right);
        }
        public static explicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.X, v.Y, 0.0f);
        }
        private const float _colorFactor = 1.0f / 255.0f;
        public static explicit operator Vector3(Color c) { return new Vector3(c.R * _colorFactor, c.G * _colorFactor, c.B * _colorFactor); }
        public static explicit operator Color(Vector3 v) { return Color.FromArgb((int)(v.X / _colorFactor), (int)(v.Y / _colorFactor), (int)(v.Z / _colorFactor)); }

        private static string listSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
        {
            return String.Format("({0}{3} {1}{3} {2})", X, Y, Z, listSeparator);
        }
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
            if (!(obj is Vector3))
                return false;

            return this.Equals((Vector3)obj);
        }
        public bool Equals(Vector3 other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }
    }
}
