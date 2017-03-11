using System.Runtime.InteropServices;
using static System.Math;
using static System.CustomMath;
using System.Xml.Serialization;
using System.Drawing;
using CustomEngine;
using CustomEngine.Rendering.Models;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Vec3 : IEquatable<Vec3>, IUniformable3Float, IBufferable
    {
        public float X, Y, Z;

        public float* Data => (float*)Address;
        public VoidPtr Address { get { fixed (void* p = &this) return p; } }
        public VertexBuffer.ComponentType ComponentType => VertexBuffer.ComponentType.Float;
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
        public Vec3(Vec2 v)
            : this(v.X, v.Y, 0.0f) { }
        public Vec3(Vec2 v, float z)
            : this(v.X, v.Y, z) { }
        public Vec3(float x, Vec2 v)
            : this(x, v.X, v.Y) { }
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

        public float LengthSquared => Dot(this);
        public float Length => (float)Sqrt(LengthSquared);
        public float LengthFast => 1.0f / InverseSqrtFast(LengthSquared);

        public float DistanceTo(Vec3 point)
            => (point - this).Length;
        public float DistanceToFast(Vec3 point)
            => (point - this).LengthFast;
        public float DistanceToSquared(Vec3 point)
            => (point - this).LengthSquared;

        public void Normalize()
        {
            float length = Length;
            if (!length.IsZero())
                this *= (1.0f / length);
        }
        public Vec3 Normalized(Vec3 origin)
            => (this - origin).Normalized();
        public Vec3 Normalized()
        {
            Vec3 v = this;
            v.Normalize();
            return v;
        }
        public void NormalizeFast()
        {
            float lengthSq = LengthSquared;
            if (!lengthSq.IsZero())
                this *= InverseSqrtFast(lengthSq);
        }
        public Vec3 NormalizedFast(Vec3 origin) 
            => (this - origin).NormalizedFast();
        public Vec3 NormalizedFast()
        {
            Vec3 v = this;
            v.NormalizeFast();
            return v;
        }

        public void SetLequalTo(Vec3 other)
        {
            if (!(this <= other))
            {
                X = other.X;
                Y = other.Y;
                Z = other.Z;
            }
        }
        public void SetGequalTo(Vec3 other)
        {
            if (!(this >= other))
            {
                X = other.X;
                Y = other.Y;
                Z = other.Z;
            }
        }

        public void ChangeZupToYup(bool negateX = true)
        {
            Swap(ref Z, ref Y);
            if (negateX)
                X = -X;
        }

        public static readonly Vec3 UnitX = new Vec3(1.0f, 0.0f, 0.0f);
        public static readonly Vec3 UnitY = new Vec3(0.0f, 1.0f, 0.0f);
        public static readonly Vec3 UnitZ = new Vec3(0.0f, 0.0f, 1.0f);
        public static readonly Vec3 Zero = new Vec3(0.0f);
        public static readonly Vec3 Half = new Vec3(0.5f);
        public static readonly Vec3 One = new Vec3(1.0f);
        public static readonly Vec3 Min = new Vec3(float.MinValue);
        public static readonly Vec3 Max = new Vec3(float.MaxValue);
        public static readonly Vec3 Right = UnitX;
        public static readonly Vec3 Forward = -UnitZ;
        public static readonly Vec3 Up = UnitY;
        public static readonly Vec3 Left = -Right;
        public static readonly Vec3 Backward = -Forward;
        public static readonly Vec3 Down = -Up;
        
        public static Vec3 ComponentMin(Vec3 a, Vec3 b)
        {
            a.X = a.X < b.X ? a.X : b.X;
            a.Y = a.Y < b.Y ? a.Y : b.Y;
            a.Z = a.Z < b.Z ? a.Z : b.Z;
            return a;
        }
        public static Vec3 ComponentMax(Vec3 a, Vec3 b)
        {
            a.X = a.X > b.X ? a.X : b.X;
            a.Y = a.Y > b.Y ? a.Y : b.Y;
            a.Z = a.Z > b.Z ? a.Z : b.Z;
            return a;
        }
        public static Vec3 MagnitudeMin(Vec3 left, Vec3 right)
            => left.LengthSquared < right.LengthSquared ? left : right;
        public static Vec3 MagnitudeMax(Vec3 left, Vec3 right)
            => left.LengthSquared > right.LengthSquared ? left : right;
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
        public static float Dot(Vec3 left, Vec3 right)
            => left.Dot(right);
        public float Dot(Vec3 right)
            => X * right.X + Y * right.Y + Z * right.Z;

        /// <summary>
        ///        |
        /// normal |  /
        /// l x r, | / right
        /// -r x l |/_______ 
        ///            left
        /// </summary>
        public Vec3 Cross(Vec3 right) 
            => new Vec3(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X);

        public static Vec3 Cross(Vec3 left, Vec3 right)
            => left ^ right;

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
        /// Calculates the angle (in degrees) between two vectors.
        /// </summary>
        /// <param name="first">The first vector.</param>
        /// <param name="second">The second vector.</param>
        /// <returns>Angle (in radians) between the vectors.</returns>
        /// <remarks>Note that the returned angle is never bigger than 180.</remarks>
        public float CalculateAngle(Vec3 second)
            => RadToDeg((float)Acos((Dot(second) / (LengthFast * second.LengthFast)).Clamp(-1.0f, 1.0f)));

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

        /// <summary>
        /// Returns a YPR rotator relative to -Z with azimuth as yaw, elevation as pitch, and 0 as roll.
        /// </summary>
        public Rotator LookatAngles()
            => LookatAngles(Forward);
        /// <summary>
        /// Returns a YPR rotator relative to the start normal with azimuth as yaw, elevation as pitch, and 0 as roll.
        /// </summary>
        public Rotator LookatAngles(Vec3 startNormal)
        {
            Vec3 thisNorm = NormalizedFast();
            Vec3 thatNorm = startNormal.NormalizedFast();
            Vec3 axis = (thisNorm ^ thatNorm).NormalizedFast();
            float angle = RadToDeg((float)Acos(thisNorm | thatNorm));
            Quaternion q = Quaternion.FromAxisAngle(axis, angle);
            Rotator r = q.ToEuler();
            //Rotator r = startNormal.LookatAngles(Forward);
            //return new Rotator(
            //    RadToDeg((float)Atan2(Y, Sqrt(X * X + Z * Z))),
            //    //RadToDeg((float)Atan2(-Z, X)),
            //    RadToDeg((float)Atan2(-X, -Z)),
            //    0.0f, Rotator.Order.YPR);
            return r;
        }
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
        /// <summary>
        /// Radians
        /// </summary>
        /// <returns></returns>
        public Vec3 GetAngles() 
            => new Vec3(AngleX(), AngleY(), AngleZ());
        public Vec3 GetAngles(Vec3 origin)
            => (this - origin).GetAngles();
        public float AngleX()
            => (float)Atan2(Y, -Z);
        public float AngleY()
            => (float)Atan2(-Z, X);
        public float AngleZ()
            => (float)Atan2(Y, X);

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

            //Get barycentric coordinates
            float d = (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) / d;
            float v = (dot00 * dot12 - dot01 * dot02) / d;

            return u >= 0 && v >= 0 && u + v < 1;
        }
        
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
        
        /// <summary>
        /// Cross
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static Vec3 operator ^(Vec3 vec1, Vec3 vec2)
            => vec1.Cross(vec2);

        /// <summary>
        /// Dot
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static float operator |(Vec3 vec1, Vec3 vec2)
            => vec1.Dot(vec2);

        public static Vec3 operator *(Matrix4 left, Vec3 right)
            => TransformPerspective(left, right);
        
        public static Vec3 operator *(Vec3 left, Matrix4 right)
            => TransformPerspective(left, right);
        
        public static explicit operator Vec3(Vec2 v)
            => new Vec3(v.X, v.Y, 0.0f);
        
        public static explicit operator Vec3(ColorF4 v)
            => new Vec3(v.R, v.G, v.B);

        private const float _colorFactor = 1.0f / 255.0f;
        public static explicit operator Vec3(Color c)
            => new Vec3(c.R * _colorFactor, c.G * _colorFactor, c.B * _colorFactor);
        public static explicit operator Color(Vec3 v)
            => Color.FromArgb((int)(v.X / _colorFactor), (int)(v.Y / _colorFactor), (int)(v.Z / _colorFactor));

        public static implicit operator BulletSharp.Vector3(Vec3 v) 
            => new BulletSharp.Vector3(v.X, v.Y, v.Z);
        public static implicit operator Vec3(BulletSharp.Vector3 v)
            => new Vec3(v.X, v.Y, v.Z);

        public static implicit operator Vec3(float v)
            => new Vec3(v);

        private static string listSeparator = 
            Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public static Vec3 Parse(string value)
        {
            value = value.Trim();

            if (value.StartsWith("("))
                value = value.Substring(1);
            if (value.EndsWith(")"))
                value = value.Substring(0, value.Length - 1);

            string[] parts = value.Split(' ');
            if (parts[0].EndsWith(listSeparator))
                parts[0].Substring(0, parts[0].Length - 1);
            if (parts[1].EndsWith(listSeparator))
                parts[1].Substring(0, parts[1].Length - 1);

            return new Vec3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
        }
        public override string ToString()
            => ToString(true, true);
        
        public string ToString(bool includeParentheses, bool includeSeparator)
            => String.Format("{4}{0}{3} {1}{3} {2}{5}", X, Y, Z, includeSeparator ? listSeparator : "", includeParentheses ? "(" : "", includeParentheses ? ")" : "");

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
    }
}
