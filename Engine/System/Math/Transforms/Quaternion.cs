using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Quaternion : IEquatable<Quaternion>
    {
        public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        public float X, Y, Z, W;

        public Quaternion(Vector3 v, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }
        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Quaternion(float pitch, float yaw, float roll)
        {
            yaw *= 0.5f;
            pitch *= 0.5f;
            roll *= 0.5f;

            float c1 = (float)Math.Cos(yaw);
            float c2 = (float)Math.Cos(pitch);
            float c3 = (float)Math.Cos(roll);
            float s1 = (float)Math.Sin(yaw);
            float s2 = (float)Math.Sin(pitch);
            float s3 = (float)Math.Sin(roll);

            W = c1 * c2 * c3 - s1 * s2 * s3;
            X = s1 * s2 * c3 + c1 * c2 * s3;
            Y = s1 * c2 * c3 + c1 * s2 * s3;
            Z = c1 * s2 * c3 - s1 * c2 * s3;
        }
        public Quaternion(Vector3 eulerAngles) : this(eulerAngles.X, eulerAngles.Y, eulerAngles.Z) { }

        public float* Data { get { fixed (void* p = &this) return (float*)p; } }
        public Vector3 Xyz { get { return new Vector3(X, Y, Z); } set { X = value.X; Y = value.Y; Z = value.Z; } }
        public Vector4 Xyzw { get { return new Vector4(X, Y, Z, W); } set { X = value.X; Y = value.Y; Z = value.Z; W = value.W; } }
        public float Length { get { return (float)System.Math.Sqrt(W * W + Xyz.LengthSquared); } }
        public float LengthSquared { get { return W * W + Xyz.LengthSquared; } }

        public void ToAxisAngle(out Vector3 axis, out float angle)
        {
            Vector4 result = ToAxisAngle();
            axis = result.Xyz;
            angle = result.W;
        }
        public Vector4 ToAxisAngle()
        {
            Quaternion q = this;
            if (Math.Abs(q.W) > 1.0f)
                q.Normalize();

            Vector4 result = new Vector4();

            result.W = 2.0f * (float)System.Math.Acos(q.W); // angle
            float den = (float)System.Math.Sqrt(1.0 - q.W * q.W);
            if (den > 0.0001f)
                result.Xyz = q.Xyz / den;
            else
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                result.Xyz = Vector3.UnitX;
            return result;
        }
        public Quaternion Normalized()
        {
            Quaternion q = this;
            q.Normalize();
            return q;
        }
        public void Normalize()
        {
            float scale = Length;
            Xyz /= scale;
            W /= scale;
        }
        public void Invert() { W = -W; }
        public Quaternion Inverted()
        {
            var q = this;
            q.Invert();
            return q;
        }
        public static Quaternion Invert(Quaternion q)
        {
            float lengthSq = q.LengthSquared;
            if (lengthSq != 0.0)
                return new Quaternion(q.Xyz / -lengthSq, q.W / lengthSq);
            else
                return q;
        }
        public void Conjugate() { Xyz = -Xyz; }
        public Quaternion Conjugated()
        {
            var q = this;
            q.Conjugate();
            return q;
        }
        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(-q.Xyz, q.W);
        }
        public static Quaternion FromAxisAngle(Vector3 axis, float angle)
        {
            if (axis.LengthSquared == 0.0f)
                return Identity;

            Quaternion result = Identity;

            angle *= 0.5f;
            axis.Normalize();
            result.Xyz = axis * (float)System.Math.Sin(angle);
            result.W = (float)System.Math.Cos(angle);

            return result.Normalized();
        }
        /// <summary>
        /// Builds a Quaternion from the given euler angles
        /// </summary>
        /// <param name="pitch">The pitch (attitude), rotation around X axis</param>
        /// <param name="yaw">The yaw (heading), rotation around Y axis</param>
        /// <param name="roll">The roll (bank), rotation around Z axis</param>
        /// <returns></returns>
        public static Quaternion FromEulerAngles(float pitch, float yaw, float roll)
        {
            return new Quaternion(pitch, yaw, roll);
        }
        public static Quaternion FromEulerAngles(Vector3 eulerAngles)
        {
            return new Quaternion(eulerAngles);
        }
        public static Quaternion FromMatrix(Matrix4 matrix)
        {
            Quaternion result = new Quaternion();
            float trace = matrix.Trace;

            if (trace > 0)
            {
                float s = (float)Math.Sqrt(trace + 1.0f) * 2.0f;
                float invS = 1.0f / s;

                result.W = s * 0.25f;
                result.X = (matrix.Row2.Y - matrix.Row1.Z) * invS;
                result.Y = (matrix.Row0.Z - matrix.Row2.X) * invS;
                result.Z = (matrix.Row1.X - matrix.Row0.Y) * invS;
            }
            else
            {
                float m00 = matrix.Row0.X, m11 = matrix.Row1.Y, m22 = matrix.Row2.Z;

                if (m00 > m11 && m00 > m22)
                {
                    float s = (float)Math.Sqrt(1 + m00 - m11 - m22) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row2.Y - matrix.Row1.Z) * invS;
                    result.X = s * 0.25f;
                    result.Y = (matrix.Row0.Y + matrix.Row1.X) * invS;
                    result.Z = (matrix.Row0.Z + matrix.Row2.X) * invS;
                }
                else if (m11 > m22)
                {
                    float s = (float)Math.Sqrt(1 + m11 - m00 - m22) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row0.Z - matrix.Row2.X) * invS;
                    result.X = (matrix.Row0.Y + matrix.Row1.X) * invS;
                    result.Y = s * 0.25f;
                    result.Z = (matrix.Row1.Z + matrix.Row2.Y) * invS;
                }
                else
                {
                    float s = (float)Math.Sqrt(1.0f + m22 - m00 - m11) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row1.X - matrix.Row0.Y) * invS;
                    result.X = (matrix.Row0.Z + matrix.Row2.X) * invS;
                    result.Y = (matrix.Row1.Z + matrix.Row2.Y) * invS;
                    result.Z = s * 0.25f;
                }
            }
            return result;
        }
        /// <summary>
        /// Do Spherical linear interpolation between two quaternions 
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared == 0.0f)
            {
                if (q2.LengthSquared == 0.0f)
                    return Identity;
                return q2;
            }
            else if (q2.LengthSquared == 0.0f)
                return q1;

            float cosHalfAngle = q1.W * q2.W + q1.Xyz.Dot(q2.Xyz);
            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion result = new Quaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > 0.0f)
                return result.Normalized();
            else
                return Identity;
        }
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            left.Xyzw += right.Xyzw;
            return left;
        }
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            left.Xyzw -= right.Xyzw;
            return left;
        }
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                right.W * left.Xyz + left.W * right.Xyz + left.Xyz.Cross(right.Xyz),
                left.W * right.W - left.Xyz.Dot(right.Xyz));
        }
        public static Quaternion operator *(Quaternion quaternion, float scale)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }
        public static Quaternion operator *(float scale, Quaternion quaternion)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(right);
        }
        public override string ToString()
        {
            return String.Format("V: {0}, W: {1}", Xyz, W);
        }
        public override bool Equals(object other)
        {
            if (other is Quaternion == false)
                return false;
            return this == (Quaternion)other;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Xyz.GetHashCode() * 397) ^ W.GetHashCode();
            }
        }
        public bool Equals(Quaternion other)
        {
            return Xyz == other.Xyz && W == other.W;
        }
    }
}
