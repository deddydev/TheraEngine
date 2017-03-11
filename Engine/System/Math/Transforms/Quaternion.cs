using System.Runtime.InteropServices;
using static System.Math;
using static System.CustomMath;
using System.Collections.Generic;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Quaternion : IEquatable<Quaternion>
    {
        public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

        public float X, Y, Z, W;

        public Quaternion(Vec3 v, float w)
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

        public float* Data { get { fixed (void* p = &this) return (float*)p; } }
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
        public Vec4 Xyzw
        {
            get => new Vec4(X, Y, Z, W);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
                W = value.W;
            }
        }
        public float LengthFast
        {
            get
            {
                float invLen = InverseSqrtFast(LengthSquared);
                return invLen > 0.0f ? 1.0f / invLen : 0.0f;
            }
        }
        public float Length => (float)Sqrt(LengthSquared);
        public float LengthSquared => Xyzw.LengthSquared;

        public void ToAxisAngle(out Vec3 axis, out float angle)
        {
            Vec4 result = ToAxisAngle();
            axis = result.Xyz;
            angle = RadToDeg(result.W);
        }
        public Vec4 ToAxisAngle()
        {
            Quaternion q = this;
            q.NormalizeFast();

            float den = (float)Sqrt(1.0 - q.W * q.W);
            return new Vec4(den > 0.0001f ? q.Xyz / den : Vec3.Right, 2.0f * (float)Acos(q.W));
        }
        /// <summary>
        /// Returns a euler rotation in the order of pitch, yaw, roll.
        /// </summary>
        public Rotator ToEuler()
        {
            float sqx = X * X;
            float sqy = Y * Y;
            float sqz = Z * Z;
            float sqw = W * W;
            float unit = sqx + sqy + sqz + sqw;
            float test = X * Y + Z * W;
            float yaw, pitch, roll;
            if (test > 0.499f * unit)
            {
                //North pole singularity
                yaw = 2.0f * (float)Atan2(X, W);
                pitch = (float)PI / 2.0f;
                roll = 0.0f;
            }
            else if (test < -0.499f * unit)
            {
                //South pole singularity
                yaw = -2.0f * (float)Atan2(X, W);
                pitch = (float)-PI / 2.0f;
                roll = 0.0f;
            }
            else
            {
                yaw = (float)Atan2(2.0f * Y * W - 2.0f * X * Z, sqx - sqy - sqz + sqw);
                pitch = (float)Asin(2.0f * test / unit);
                roll = (float)Atan2(2.0f * X * W - 2.0f * Y * Z, -sqx + sqy - sqz + sqw);
            }
            return new Rotator(RadToDeg(pitch), RadToDeg(yaw), RadToDeg(roll), Rotator.Order.YPR);
        }
        public Quaternion Normalized()
        {
            Quaternion q = this;
            q.Normalize();
            return q;
        }
        public Quaternion NormalizedFast()
        {
            Quaternion q = this;
            q.NormalizeFast();
            return q;
        }
        public void Normalize()
        {
            float len = Length;
            if (len > 0.0f)
                Xyzw /= len;
        }
        public void NormalizeFast()
        {
            float invLen = InverseSqrtFast(LengthSquared);
            if (invLen > 0.0f)
                Xyzw *= invLen;
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
        /// <summary>
        /// Makes this quaternion the opposite version of itself.
        /// There are two rotations about the same axis that equal the same rotation,
        /// but from different directions.
        /// </summary>
        public void Conjugate() { Xyz = -Xyz; }
        /// <summary>
        /// Returns the opposite version of this quaternion.
        /// There are two rotations about the same axis that equal the same rotation,
        /// but from different directions.
        /// </summary>
        public Quaternion Conjugated()
        {
            var q = this;
            q.Conjugate();
            return q;
        }
        /// <summary>
        /// Returns a quaternion containing the rotation from one vector direction to another.
        /// </summary>
        /// <param name="initialVector">The starting vector</param>
        /// <param name="finalVector">The ending vector</param>
        public static Quaternion BetweenVectors(Vec3 initialVector, Vec3 finalVector)
        {
            Vec3 axis;
            float angle;

            initialVector.NormalizeFast();
            finalVector.NormalizeFast();

            float dot = initialVector | finalVector;

            //dot is the cosine adj/hyp ratio between the two vectors, so
            //dot == 1 is same direction
            //dot == -1 is opposite direction
            //dot == 0 is a 90 degree angle

            if (dot > 0.999f)
            {
                axis = Vec3.Right;
                angle = 0.0f;
            }
            else if (dot < -0.999f)
            {
                axis = Vec3.Right;
                angle = 180.0f;
            }
            else
            {
                axis = initialVector ^ finalVector;
                angle = RadToDeg((float)Acos(dot));
            }
            
            //Invert the angle; a positive angle rotates DOWN instead of UP on the unit circle
            return FromAxisAngle(axis, -angle);
        }
        public static Quaternion LookAt(Vec3 sourcePoint, Vec3 destPoint, Vec3 initialDirection)
        {
            return BetweenVectors(initialDirection, destPoint - sourcePoint);
        }
        public static Quaternion FromAxisAngle(Vec3 axis, float angle)
        {
            angle = DegToRad(angle);

            if (axis.LengthSquared == 0.0f)
                return Identity;

            Quaternion result = Identity;

            angle *= 0.5f;
            axis.NormalizeFast();
            result.Xyz = axis.Xyz * (float)Sin(angle);
            result.W = (float)Cos(angle);

            return result.NormalizedFast();
        }

        public static Quaternion FromRotator(Rotator rotator)
        {
            return FromEulerAngles(rotator.Yaw, rotator.Pitch, rotator.Roll, rotator._rotationOrder);
        }
        /// <summary>
        /// Builds a Quaternion from the given euler angles
        /// </summary>
        /// <param name="yaw">The yaw (heading), rotation around Y axis</param>
        /// <param name="pitch">The pitch (attitude), rotation around X axis</param>
        /// <param name="roll">The roll (bank), rotation around Z axis</param>
        public static Quaternion FromEulerAngles(float pitch, float yaw, float roll, Rotator.Order order = Rotator.Order.YPR)
        {
            Quaternion p = FromAxisAngle(Vec3.Right, pitch);
            Quaternion y = FromAxisAngle(Vec3.Up, yaw);
            Quaternion r = FromAxisAngle(Vec3.Forward, roll);
            switch (order)
            {
                case Rotator.Order.RYP: return r * y * p;
                case Rotator.Order.YRP: return y * r * p;
                case Rotator.Order.PRY: return p * r * y;
                case Rotator.Order.RPY: return r * p * y;
                case Rotator.Order.YPR: return y * p * r;
                case Rotator.Order.PYR: return p * y * r;
            }
            return Identity;
        }
        public static Quaternion FromMatrix(Matrix4 matrix)
        {
            Quaternion result = new Quaternion();
            float trace = matrix.Trace;

            if (trace > 0)
            {
                float s = (float)Sqrt(trace + 1.0f) * 2.0f;
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
                    float s = (float)Sqrt(1 + m00 - m11 - m22) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row2.Y - matrix.Row1.Z) * invS;
                    result.X = s * 0.25f;
                    result.Y = (matrix.Row0.Y + matrix.Row1.X) * invS;
                    result.Z = (matrix.Row0.Z + matrix.Row2.X) * invS;
                }
                else if (m11 > m22)
                {
                    float s = (float)Sqrt(1 + m11 - m00 - m22) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row0.Z - matrix.Row2.X) * invS;
                    result.X = (matrix.Row0.Y + matrix.Row1.X) * invS;
                    result.Y = s * 0.25f;
                    result.Z = (matrix.Row1.Z + matrix.Row2.Y) * invS;
                }
                else
                {
                    float s = (float)Sqrt(1.0f + m22 - m00 - m11) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row1.X - matrix.Row0.Y) * invS;
                    result.X = (matrix.Row0.Z + matrix.Row2.X) * invS;
                    result.Y = (matrix.Row1.Z + matrix.Row2.Y) * invS;
                    result.Z = s * 0.25f;
                }
            }
            return result;
        }
        public static Quaternion Scubic(Quaternion p1, Quaternion p2, Quaternion p3, Quaternion p4, float time)
        {
            Quaternion q1 = Slerp(p1, p2, time);
            Quaternion q2 = Slerp(p1, p2, time);
            Quaternion q3 = Slerp(p1, p2, time);
            return Squad(q1, q2, q3, time);
        }
        public static Quaternion Squad(Quaternion q1, Quaternion q2, Quaternion q3, float time)
        {
            Quaternion r1 = Slerp(q1, q2, time);
            Quaternion r2 = Slerp(q2, q3, time);
            return Slerp(r1, r2, time);
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
                float halfAngle = (float)Acos(cosHalfAngle);
                float sinHalfAngle = (float)Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion result = new Quaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > 0.0f)
                return result.NormalizedFast();
            else
                return Identity;
        }
        //map axes strings to/from tuples of inner axis, parity, repetition, frame
        private static readonly Dictionary<AxisCombo, IVec4> AxesToTuple = new Dictionary<AxisCombo, IVec4>()
        {
            { AxisCombo.SXYZ, new IVec4(0, 0, 0, 0) }, { AxisCombo.SXYX, new IVec4(0, 0, 1, 0) }, { AxisCombo.SXZY, new IVec4(0, 1, 0, 0) }, { AxisCombo.SXZX, new IVec4(0, 1, 1, 0) },
            { AxisCombo.SYZX, new IVec4(1, 0, 0, 0) }, { AxisCombo.SYZY, new IVec4(1, 0, 1, 0) }, { AxisCombo.SYXZ, new IVec4(1, 1, 0, 0) }, { AxisCombo.SYXY, new IVec4(1, 1, 1, 0) },
            { AxisCombo.SZXY, new IVec4(2, 0, 0, 0) }, { AxisCombo.SZXZ, new IVec4(2, 0, 1, 0) }, { AxisCombo.SZYX, new IVec4(2, 1, 0, 0) }, { AxisCombo.SZYZ, new IVec4(2, 1, 1, 0) },
            { AxisCombo.RZYX, new IVec4(0, 0, 0, 1) }, { AxisCombo.RXYX, new IVec4(0, 0, 1, 1) }, { AxisCombo.RYZX, new IVec4(0, 1, 0, 1) }, { AxisCombo.RXZX, new IVec4(0, 1, 1, 1) },
            { AxisCombo.RXZY, new IVec4(1, 0, 0, 1) }, { AxisCombo.RYZY, new IVec4(1, 0, 1, 1) }, { AxisCombo.RZXY, new IVec4(1, 1, 0, 1) }, { AxisCombo.RYXY, new IVec4(1, 1, 1, 1) },
            { AxisCombo.RYXZ, new IVec4(2, 0, 0, 1) }, { AxisCombo.RZXZ, new IVec4(2, 0, 1, 1) }, { AxisCombo.RXYZ, new IVec4(2, 1, 0, 1) }, { AxisCombo.RZYZ, new IVec4(2, 1, 1, 1) }
        };
        private static readonly int[] NextAxis = new int[4] { 1, 2, 0, 1 };
        public enum AxisCombo
        {
            SXYZ, SXYX, SXZY, SXZX,
            SYZX, SYZY, SYXZ, SYXY,
            SZXY, SZXZ, SZYX, SZYZ,

            RXYZ, RXYX, RXZY, RXZX,
            RYZX, RYZY, RYXZ, RYXY,
            RZXY, RZXZ, RZYX, RZYZ,
        }
        public static Quaternion FromEulerAngles(float pitch, float yaw, float roll, AxisCombo axes)
        {
            float
                ai = DegToRad(roll) * 0.5f,
                aj = DegToRad(pitch) * 0.5f, 
                ak = DegToRad(yaw) * 0.5f;

            IVec4 tuple = AxesToTuple[axes];
            int firstAxis = tuple[0];
            int parity = tuple[1];
            bool repetition = tuple[2] != 0;
            bool frame = tuple[3] != 0;

            int i = firstAxis + 1;
            int j = NextAxis[i + parity - 1] + 1;
            int k = NextAxis[i - parity] + 1;

            if (frame)
            {
                float temp = ai;
                ai = ak;
                ak = temp;
            }

            if (parity != 0)
                aj = -aj;

            float ci = (float)Cos(ai);
            float si = (float)Sin(ai);
            float cj = (float)Cos(aj);
            float sj = (float)Sin(aj);
            float ck = (float)Cos(ak);
            float sk = (float)Sin(ak);
            float cc = ci * ck;
            float cs = ci * sk;
            float sc = si * ck;
            float ss = si * sk;

            //float ci = (float)Cos(yaw);
            //float si = (float)Sin(yaw);
            //float cj = (float)Cos(pitch);
            //float sj = (float)Sin(pitch);
            //float ck = (float)Cos(roll);
            //float sk = (float)Sin(roll);
            //float cc = c1 * c3;
            //float cs = c1 * s3;
            //float sc = s1 * c3;
            //float ss = s1 * s3;

            //X = s1 * s2 * c3 + c1 * c2 * s3;
            //Y = s1 * c2 * c3 + c1 * s2 * s3;
            //Z = c1 * s2 * c3 - s1 * c2 * s3;
            //W = c1 * c2 * c3 - s1 * s2 * s3;

            Quaternion q = new Quaternion();
            float* p = q.Data;
            if (repetition)
            {
                p[i] = cj * (cs + sc);
                p[j] = sj * (cc + ss);
                p[k] = sj * (cs - sc);
                p[3] = cj * (cc - ss);
            }
            else
            {
                p[i] = cj * sc - sj * cs;
                p[j] = cj * ss + sj * cc;
                p[k] = cj * cs - sj * sc;
                p[3] = cj * cc + sj * ss;
            }

            if (parity != 0)
                p[j] = -p[j];

            return q;
        }
        //Optimized form for vec3's (W == 0)
        public Vec3 Transform(Vec3 v)
        {
            Vec3 t = 2.0f * (Xyz ^ v);
            return v + W * t + (Xyz ^ t);
        }
        //Slower, traditional form
        public Vec4 Transform(Vec4 vec)
        {
            Quaternion v = new Quaternion(vec.X, vec.Y, vec.Z, vec.W);
            v = this * v * Conjugated();
            return new Vec4(v.X, v.Y, v.Z, v.W);
        }
        public static Vec3 operator *(Quaternion quat, Vec3 vec)
            => quat.Transform(vec);
        public static Vec3 operator *(Vec3 vec, Quaternion quat)
            => quat.Transform(vec);
        public static Vec4 operator *(Quaternion quat, Vec4 vec)
            => quat.Transform(vec);
        public static Vec4 operator *(Vec4 vec, Quaternion quat)
            => quat.Transform(vec);
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
                right.W * left.Xyz + left.W * right.Xyz + (left.Xyz ^ right.Xyz),
                left.W * right.W - (left.Xyz | right.Xyz));
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
            return string.Format("V: {0}, W: {1}", Xyz, W);
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
