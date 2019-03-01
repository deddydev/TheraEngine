using System.Runtime.InteropServices;
using static System.Math;
using static System.TMath;
using System.Collections.Generic;
using TheraEngine;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core;

namespace System
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Quat : IEquatable<Quat>, ISerializableString
    {
        public static readonly Quat Identity = new Quat(0.0f, 0.0f, 0.0f, 1.0f);

        public float X, Y, Z, W;

        public Quat(Vec4 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }
        public Quat(Vec3 v, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }
        public Quat(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static implicit operator Quat(Vec4 v) => new Quat(v);

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

        public float this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        /// <summary>
        /// Compresses the quaternion into 12 bytes.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Vec3 CompressCayley(Quat q)
        {
            float s = 1.0f / (1.0f + q.W);
            return s * new Vec3(q.X, q.Y, q.Z);
        }
        /// <summary>
        /// Decompresses a quaternion from 12 bytes.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Quat DecompressCayley(Vec3 v)
        {
            float s = 2.0f / (1.0f + v.LengthSquared);
            return new Quat(s * v, 1.0f - s);
        }
        /// <summary>
        /// Compresses the value into 4 bytes.
        /// If "one" is true, returns the index of the component that is set to 1.0f (all others are 0.0f),
        /// meaning that the entire quaternion can be written in two bits (0 - 3 for the index fits in 2 bits).
        /// Otherwise the entire 4 bytes will be used.
        /// The two msb's are used for the index of the greatest value.
        /// The 3 other sets of 10 bits are used for the other values from left to right (XYZW), not including the greatest value.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="one"></param>
        /// <returns></returns>
        public static int CompressSmallest3(Quat q, out bool one, float maxValueEpsilon = 0.0001f)
        {
            int greatestIndex = 0;
            float maxValue = float.MinValue, currentValue;
            Vec4 abs = new Vec4();
            for (int i = 0; i < 4; ++i)
            {
                currentValue = Abs(q[i]);
                if (currentValue > maxValue)
                {
                    maxValue = currentValue;
                    greatestIndex = i;
                }
                abs[i] = currentValue;
            }

            if (one = maxValue.EqualTo(1.0f, maxValueEpsilon))
                return greatestIndex;
            
            int sign = (q[greatestIndex] < 0) ? -1 : 1;
            int finalValue = (greatestIndex & 3) << 30;
            int shift, comp;
            float value;
            for (int i = 0, x = 0; i < 4; ++i)
            {
                //Don't write the greatest value
                if (i == greatestIndex)
                    continue;

                shift = (2 - x++) * 10;

                //1024 = 2^10 = 10 bits unsigned possible numbers
                //This is non-inclusive 0 to 1023
                value = abs[i] * 1024.0f;

                comp = (int)(value / float.MaxValue) * sign;
                finalValue |= (comp & 0x3FF) << shift;
            }
            return finalValue;
        }
        //public static Quat DecompressSmallest3(int compressed, bool one)
        //{

        //}

        public void ToAxisAngleDeg(out Vec3 axis, out float angle)
        {
            Vec4 result = ToAxisAngleRad();
            axis = result.Xyz;
            angle = RadToDeg(result.W);
        }
        public void ToAxisAngleRad(out Vec3 axis, out float angle)
        {
            Vec4 result = ToAxisAngleRad();
            axis = result.Xyz;
            angle = result.W;
        }
        private Vec4 ToAxisAngleRad()
        {
            Quat q = this;
            q.Normalize();
            float den = (float)Sqrt(1.0f - q.W * q.W);
            return new Vec4(den > TMath.Epsilon ? q.Xyz / den : Vec3.Right, 2.0f * (float)Acos(q.W));
        }

        /// <summary>
        /// Returns a euler rotation in the order of yaw, pitch, roll.
        /// </summary>
        public Rotator ToYawPitchRoll()
        {
            Normalize();
            float sqx = X * X;
            float sqy = Y * Y;
            float sqz = Z * Z;
            float sqw = W * W;
            float unit = sqx + sqy + sqz + sqw;
            float test = (X * Y + Z * W) / unit;
            float yaw, pitch, roll;
            if (test.EqualTo(0.5f, 0.001f))
            {
                //North pole singularity
                yaw = 2.0f * (float)Atan2(X, W);
                pitch = (float)PI / 2.0f;
                roll = 0.0f;
            }
            else if (test.EqualTo(-0.5f, 0.001f))
            {
                //South pole singularity
                yaw = -2.0f * (float)Atan2(X, W);
                pitch = (float)-PI / 2.0f;
                roll = 0.0f;
            }
            else
            {
                yaw = (float)Atan2(2.0f * Y * W - 2.0f * X * Z, 1.0f - 2.0f * sqy - 2.0f * sqz);
                roll = (float)Asin(2.0f * X * Y + 2.0f * Z * W);
                pitch = (float)Atan2(2.0f * X * W - 2.0f * Y * Z, 1.0f - 2.0f * sqx - 2.0f * sqz);

                //yaw = (float)Atan2(2.0f * Y * W - 2.0f * X * Z, sqx - sqy - sqz + sqw);
                //pitch = (float)Asin(2.0f * test / unit);
                //roll = (float)Atan2(2.0f * X * W - 2.0f * Y * Z, -sqx + sqy - sqz + sqw);
            }
            return new Rotator(RadToDeg(pitch), RadToDeg(yaw), RadToDeg(roll), ERotationOrder.YPR);
        }
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
        public void Normalize(bool safe = true)
        {
            float lengthSq = LengthSquared;
            if (safe && lengthSq.IsZero()) return;
            Xyzw *= 1.0f / (float)Sqrt(lengthSq);
        }
        public void NormalizeFast(bool safe = true)
        {
            float lengthSq = LengthSquared;
            if (safe && lengthSq.IsZero()) return;
            Xyzw *= InverseSqrtFast(lengthSq);
        }
        public Quat Normalized(ENormalizeOption normalizeMethod)
        {
            Quat q = this;
            q.Normalize(normalizeMethod);
            return q;
        }
        public Quat Normalized(bool safe = true)
        {
            Quat q = this;
            q.Normalize(safe);
            return q;
        }
        public Quat NormalizedFast(bool safe = true)
        {
            Quat q = this;
            q.NormalizeFast(safe);
            return q;
        }
        public void Invert() { W = -W; }
        public Quat Inverted()
        {
            var q = this;
            q.Invert();
            return q;
        }
        public static Quat Invert(Quat q, bool safe = true)
        {
            float lengthSq = q.LengthSquared;
            if (!safe || !lengthSq.IsZero())
                return new Quat(q.Xyz / -lengthSq, q.W / lengthSq);
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
        public Quat Conjugated()
        {
            var q = this;
            q.Conjugate();
            return q;
        }
        /// <summary>
        /// Returns a quat that rotates from the first quat to the second quat: from * returned = to
        /// </summary>
        public static Quat DeltaRotation(Quat from, Quat to)
            => from.Inverted() * to;
        /// <summary>
        /// Returns a quaternion containing the rotation from one vector direction to another.
        /// </summary>
        /// <param name="initialVector">The starting vector</param>
        /// <param name="finalVector">The ending vector</param>
        public static Quat BetweenVectors(Vec3 initialVector, Vec3 finalVector)
        {
            AxisAngleBetween(initialVector, finalVector, out Vec3 axis, out float angle);
            return FromAxisAngleDeg(axis, angle);
        }
        public static Quat LookAt(Vec3 sourcePoint, Vec3 destPoint, Vec3 up)
            => BetweenVectors(up, destPoint - sourcePoint);
        public static Quat FromAxisAngleDeg(Vec3 axis, float degrees)
            => FromAxisAngleRad(axis, DegToRad(degrees));
        public static Quat FromAxisAngleRad(Vec3 axis, float radians)
        {
            if (axis.LengthSquared == 0.0f)
                return Identity;

            Quat result = Identity;

            radians *= 0.5f;
            axis.Normalize();
            result.Xyz = axis.Xyz * (float)Sin(radians);
            result.W = (float)Cos(radians);

            return result.Normalized();
        }

        public static Quat FromRotator(Rotator rotator)
            => FromEulerAngles(rotator.Yaw, rotator.Pitch, rotator.Roll, rotator.Order);
        
        /// <summary>
        /// Builds a Quaternion from the given euler angles
        /// </summary>
        /// <param name="yaw">The yaw (heading), rotation around Y axis</param>
        /// <param name="pitch">The pitch (attitude), rotation around X axis</param>
        /// <param name="roll">The roll (bank), rotation around Z axis</param>
        public static Quat FromEulerAngles(float pitch, float yaw, float roll, ERotationOrder order = ERotationOrder.YPR)
        {
            Quat p = FromAxisAngleDeg(Vec3.UnitX, pitch);
            Quat y = FromAxisAngleDeg(Vec3.UnitY, yaw);
            Quat r = FromAxisAngleDeg(Vec3.UnitZ, roll);
            switch (order)
            {
                case ERotationOrder.RYP: return r * y * p;
                case ERotationOrder.YRP: return y * r * p;
                case ERotationOrder.PRY: return p * r * y;
                case ERotationOrder.RPY: return r * p * y;
                case ERotationOrder.YPR: return y * p * r;
                case ERotationOrder.PYR: return p * y * r;
            }
            return Identity;
        }
        public static Quat FromMatrix(Matrix4 matrix)
        {
            Quat result = new Quat();
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
                    float s = (float)Sqrt(1.0f + m00 - m11 - m22) * 2.0f;
                    float invS = 1.0f / s;

                    result.W = (matrix.Row2.Y - matrix.Row1.Z) * invS;
                    result.X = s * 0.25f;
                    result.Y = (matrix.Row0.Y + matrix.Row1.X) * invS;
                    result.Z = (matrix.Row0.Z + matrix.Row2.X) * invS;
                }
                else if (m11 > m22)
                {
                    float s = (float)Sqrt(1.0f + m11 - m00 - m22) * 2.0f;
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
        public static Quat Scubic(Quat p1, Quat p2, Quat p3, Quat p4, float time)
        {
            Quat q1 = Slerp(p1, p2, time);
            Quat q2 = Slerp(p2, p3, time);
            Quat q3 = Slerp(p3, p4, time);
            return Squad(q1, q2, q3, time);
        }
        public static Quat Squad(Quat q1, Quat q2, Quat q3, float time)
        {
            Quat r1 = Slerp(q1, q2, time);
            Quat r2 = Slerp(q2, q3, time);
            return Slerp(r1, r2, time);
        }
        /// <summary>
        /// Do Spherical linear interpolation between two quaternions 
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static Quat Slerp(Quat q1, Quat q2, float blend)
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

            Quat result = new Quat(
                blendA * q1.Xyz + blendB * q2.Xyz, 
                blendA * q1.W + blendB * q2.W);

            if (result.LengthSquared > 0.0f)
                return result.Normalized();
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
        public static Quat FromEulerAngles(float pitch, float yaw, float roll, AxisCombo axes)
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

            Quat q = new Quat();
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
            Quat v = new Quat(vec.X, vec.Y, vec.Z, vec.W);
            v = this * v * Conjugated();
            return new Vec4(v.X, v.Y, v.Z, v.W);
        }
        public static Vec3 operator *(Quat quat, Vec3 vec)
            => quat.Transform(vec);
        public static Vec3 operator *(Vec3 vec, Quat quat)
            => quat.Transform(vec);
        public static Vec4 operator *(Quat quat, Vec4 vec)
            => quat.Transform(vec);
        public static Vec4 operator *(Vec4 vec, Quat quat)
            => quat.Transform(vec);
        public static Quat operator +(Quat left, Quat right)
        {
            left.Xyzw += right.Xyzw;
            return left;
        }
        public static Quat operator -(Quat left, Quat right)
        {
            left.Xyzw -= right.Xyzw;
            return left;
        }
        public static Quat operator *(Quat left, Quat right)
        {
            return new Quat(
                right.W * left.Xyz + left.W * right.Xyz + (left.Xyz ^ right.Xyz),
                left.W * right.W - (left.Xyz | right.Xyz));
        }
        
        public static Quat operator *(Quat quaternion, float scale)
        {
            return new Quat(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }
        public static Quat operator *(float scale, Quat quaternion)
        {
            return new Quat(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        #region BulletSharp
        public static implicit operator BulletSharp.Quaternion(Quat q) => new BulletSharp.Quaternion(q.X, q.Y, q.Z, q.W);
        public static implicit operator Quat(BulletSharp.Quaternion q) => new Quat(q.X, q.Y, q.Z, q.W);
        #endregion

        public static bool operator ==(Quat left, Quat right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Quat left, Quat right)
        {
            return !left.Equals(right);
        }
        private static string listSeparator = Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        public override string ToString()
        {
            return String.Format("({0}{4} {1}{4} {2}{4} {3})", X, Y, Z, W, listSeparator);
        }
        public string ToString(bool includeParentheses, bool includeSeparator)
           => String.Format("{5}{0}{4} {1}{4} {2}{4} {3}{6}", X, Y, Z, W,
               includeSeparator ? listSeparator : "", includeParentheses ? "(" : "", includeParentheses ? ")" : "");
        public static Quat Parse(string value)
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
            if (parts[2].EndsWith(listSeparator))
                parts[2].Substring(0, parts[2].Length - 1);

            return new Quat(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
        }
        public override bool Equals(object other)
        {
            if (other is Quat == false)
                return false;
            return this == (Quat)other;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Xyz.GetHashCode() * 397) ^ W.GetHashCode();
            }
        }
        public bool Equals(Quat other)
        {
            return Xyz == other.Xyz && W == other.W;
        }

        public string WriteToString()
            => ToString(false, false);
        public void ReadFromString(string str)
            => this = Parse(str);
    }
}
