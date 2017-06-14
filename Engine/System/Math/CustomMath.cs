using System.Collections.Generic;
using System.Linq;
using static System.Math;
namespace System
{
    public unsafe static class CustomMath
    {
        public static readonly float PIf = (float)PI;

        public static double DegToRad(double degrees)
            => degrees * PI / 180.0;
        public static double RadToDeg(double radians)
            => radians * 180.0 / PI;
        public static float DegToRad(float degrees)
            => degrees * PIf / 180.0f;
        public static float RadToDeg(float radians)
            => radians * 180.0f / PIf;
        public static Vec2 DegToRad(Vec2 degrees)
            => degrees * PIf / 180.0f;
        public static Vec2 RadToDeg(Vec2 radians)
            => radians * 180.0f / PIf;
        public static Vec3 DegToRad(Vec3 degrees)
            => degrees * PIf / 180.0f;
        public static Vec3 RadToDeg(Vec3 radians)
            => radians * 180.0f / PIf;
        public static Vec2[] GetBezierPoints(Vec2 p0, Vec2 p1, Vec2 p2, Vec2 p3, int pointCount, out float length)
        {
            Vec2[] points = new Vec2[pointCount];

            // var q is the change in t between successive evaluations.
            float q = 1.0f / (pointCount - 1); // q is dependent on the number of GAPS = POINTS-1

            // coefficients of the cubic polynomial that we're FDing -
            Vec2 a = p0;
            Vec2 b = 3 * (p1 - p0);
            Vec2 c = 3 * (p2 - 2 * p1 + p0);
            Vec2 d = p3 - 3 * p2 + 3 * p1 - p0;

            // initial values of the poly and the 3 diffs -
            Vec2 s = a;                                     // the poly value
            Vec2 u = b * q + c * q * q + d * q * q * q;     // 1st order diff (quadratic)
            Vec2 v = 2 * c * q * q + 6 * d * q * q * q;     // 2nd order diff (linear)
            Vec2 w = 6 * d * q * q * q;                     // 3rd order diff (constant)
            
            length = 0.0f;

            Vec2 OldPos = p0;
            points[0] = p0;

            for (int i = 1; i < pointCount; ++i)
            {
                s += u;
                u += v;
                v += w;
                
                length += s.DistanceTo(OldPos);
                OldPos = s;

                points[i] = s;
            }
            return points;
        }

        public static Vec2[] GetBezierPoints(Vec2 p0, Vec2 p1, Vec2 p2, Vec2 p3, int pointCount)
        {
            if (pointCount < 2)
                throw new InvalidOperationException();

            Vec2[] points = new Vec2[pointCount];
            float timeDelta = 1.0f / (pointCount - 1);
            for (int i = 0; i < pointCount; ++i)
                points[i] = CubicBezier(p0, p1, p2, p3, timeDelta);

            return points;
        }
        public static float CubicBezier(float p0, float p1, float p2, float p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float invT3 = invT2 * invT;
            float t2 = time * time;
            float t3 = t2 * time;
            return
                p0 * invT3 +
                p1 * 3.0f * invT2 * time +
                p2 * 3.0f * invT * t2 +
                p3 * t3;
        }
        public static float CubicBezierVelocity(float p0, float p1, float p2, float p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float t2 = time * time;
            return
                p0 * (-3.0f * invT2) +
                p1 * (9.0f * t2 - 12.0f * time + 3.0f) +
                p2 * (6.0f * time - 9.0f * t2) +
                p3 * 3.0f * t2;
        }
        public static float CubicBezierAcceleration(float p0, float p1, float p2, float p3, float time)
        {
            float invT = 1.0f - time;
            return
                p0 * (6.0f * invT) +
                p1 * (18.0f * time - 12.0f) +
                p2 * (-18.0f * time + 6.0f) +
                p3 * 6.0f * time;
        }
        public static Vec2 CubicBezier(Vec2 p0, Vec2 p1, Vec2 p2, Vec2 p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float invT3 = invT2 * invT;
            float t2 = time * time;
            float t3 = t2 * time;
            return
                p0 * invT3 +
                p1 * 3.0f * invT2 * time +
                p2 * 3.0f * invT * t2 +
                p3 * t3;
        }
        public static Vec2 CubicBezierVelocity(Vec2 p0, Vec2 p1, Vec2 p2, Vec2 p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float t2 = time * time;
            return
                p0 * (-3.0f * invT2) +
                p1 * (9.0f * t2 - 12.0f * time + 3.0f) +
                p2 * (6.0f * time - 9.0f * t2) +
                p3 * 3.0f * t2;
        }
        public static Vec2 CubicBezierAcceleration(Vec2 p0, Vec2 p1, Vec2 p2, Vec2 p3, float time)
        {
            float invT = 1.0f - time;
            return
                p0 * (6.0f * invT) +
                p1 * (18.0f * time - 12.0f) +
                p2 * (-18.0f * time + 6.0f) +
                p3 * 6.0f * time;
        }
        public static Vec3 CubicBezier(Vec3 p0, Vec3 p1, Vec3 p2, Vec3 p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float invT3 = invT2 * invT;
            float t2 = time * time;
            float t3 = t2 * time;
            return
                p0 * invT3 +
                p1 * 3.0f * invT2 * time +
                p2 * 3.0f * invT * t2 +
                p3 * t3;
        }
        public static Vec3 CubicBezierVelocity(Vec3 p0, Vec3 p1, Vec3 p2, Vec3 p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float t2 = time * time;
            return
                p0 * (-3.0f * invT2) +
                p1 * (9.0f * t2 - 12.0f * time + 3.0f) +
                p2 * (6.0f * time - 9.0f * t2) +
                p3 * 3.0f * t2;
        }
        public static Vec3 CubicBezierAcceleration(Vec3 p0, Vec3 p1, Vec3 p2, Vec3 p3, float time)
        {
            float invT = 1.0f - time;
            return
                p0 * (6.0f * invT) +
                p1 * (18.0f * time - 12.0f) +
                p2 * (-18.0f * time + 6.0f) +
                p3 * 6.0f * time;
        }
        public static Vec4 CubicBezier(Vec4 p0, Vec4 p1, Vec4 p2, Vec4 p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float invT3 = invT2 * invT;
            float t2 = time * time;
            float t3 = t2 * time;
            return
                p0 * invT3 +
                p1 * 3.0f * invT2 * time +
                p2 * 3.0f * invT * t2 +
                p3 * t3;
        }
        public static Vec4 CubicBezierVelocity(Vec4 p0, Vec4 p1, Vec4 p2, Vec4 p3, float time)
        {
            float invT = 1.0f - time;
            float invT2 = invT * invT;
            float t2 = time * time;
            return
                p0 * (-3.0f * invT2) +
                p1 * (9.0f * t2 - 12.0f * time + 3.0f) +
                p2 * (6.0f * time - 9.0f * t2) +
                p3 * 3.0f * t2;
        }
        public static Vec4 CubicBezierAcceleration(Vec4 p0, Vec4 p1, Vec4 p2, Vec4 p3, float time)
        {
            float invT = 1.0f - time;
            return
                p0 * (6.0f * invT) +
                p1 * (18.0f * time - 12.0f) +
                p2 * (-18.0f * time + 6.0f) +
                p3 * 6.0f * time;
        }
        public static float CubicHermite(float p0, float t0, float t1, float p1, float time)
        {
            float time2 = time * time;
            float time3 = time2 * time;
            return
                p0 * (2.0f * time3 - 3.0f * time2 + 1.0f) +
                t0 * (time3 - 2.0f * time2 + time) +
                p1 * (-2.0f * time3 + 3.0f * time2) +
                t1 * (time3 - time2);
        }
        public static float CubicHermiteVelocity(float p0, float t0, float t1, float p1, float time)
        {
            float time2 = time * time;
            return
                p0 * (6.0f * time2 - 6.0f * time) +
                t0 * (3.0f * time2 - 4.0f * time + 1.0f) +
                p1 * (-6.0f * time2 + 6.0f * time) +
                t1 * (3.0f * time2 - 2.0f * time);
        }
        public static float CubicHermiteAcceleration(float p0, float t0, float t1, float p1, float time)
        {
            return
                p0 * (12.0f * time - 6.0f) +
                t0 * (6.0f * time - 4.0f) +
                p1 * (-12.0f * time + 6.0f) +
                t1 * (6.0f * time - 2.0f);
        }
        public static Vec2 CubicHermite(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            float time2 = time * time;
            float time3 = time2 * time;
            return
                p0 * (2.0f * time3 - 3.0f * time2 + 1.0f) +
                t0 * (time3 - 2.0f * time2 + time) +
                p1 * (-2.0f * time3 + 3.0f * time2) +
                t1 * (time3 - time2);
        }
        public static Vec2 CubicHermiteVelocity(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            float time2 = time * time;
            return
                p0 * (6.0f * time2 - 6.0f * time) +
                t0 * (3.0f * time2 - 4.0f * time + 1.0f) +
                p1 * (-6.0f * time2 + 6.0f * time) +
                t1 * (3.0f * time2 - 2.0f * time);
        }
        public static Vec2 CubicHermiteAcceleration(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            return
                p0 * (12.0f * time - 6.0f) +
                t0 * (6.0f * time - 4.0f) +
                p1 * (-12.0f * time + 6.0f) +
                t1 * (6.0f * time - 2.0f);
        }
        public static Vec3 CubicHermite(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            float time2 = time * time;
            float time3 = time2 * time;
            return
                p0 * (2.0f * time3 - 3.0f * time2 + 1.0f) +
                t0 * (time3 - 2.0f * time2 + time) +
                p1 * (-2.0f * time3 + 3.0f * time2) +
                t1 * (time3 - time2);
        }
        public static Vec3 CubicHermiteVelocity(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            float time2 = time * time;
            return
                p0 * (6.0f * time2 - 6.0f * time) +
                t0 * (3.0f * time2 - 4.0f * time + 1.0f) +
                p1 * (-6.0f * time2 + 6.0f * time) +
                t1 * (3.0f * time2 - 2.0f * time);
        }
        public static Vec3 CubicHermiteAcceleration(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            return
                p0 * (12.0f * time - 6.0f) +
                t0 * (6.0f * time - 4.0f) +
                p1 * (-12.0f * time + 6.0f) +
                t1 * (6.0f * time - 2.0f);
        }
        public static Vec4 CubicHermite(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            float time2 = time * time;
            float time3 = time2 * time;
            return
                p0 * (2.0f * time3 - 3.0f * time2 + 1.0f) +
                t0 * (time3 - 2.0f * time2 + time) +
                p1 * (-2.0f * time3 + 3.0f * time2) +
                t1 * (time3 - time2);
        }
        public static Vec4 CubicHermiteVelocity(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            float time2 = time * time;
            return
                p0 * (6.0f * time2 - 6.0f * time) +
                t0 * (3.0f * time2 - 4.0f * time + 1.0f) +
                p1 * (-6.0f * time2 + 6.0f * time) +
                t1 * (3.0f * time2 - 2.0f * time);
        }
        public static Vec4 CubicHermiteAcceleration(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            return
                p0 * (12.0f * time - 6.0f) +
                t0 * (6.0f * time - 4.0f) +
                p1 * (-12.0f * time + 6.0f) +
                t1 * (6.0f * time - 2.0f);
        }
        /// <summary>
        /// Gets an approximation of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This function uses an approximation of the square root function to calculate vector magnitude, with
        /// an upper error bound of 0.001.
        /// </remarks>
        public static float InverseSqrtFast(float x)
        {
            float xhalf = 0.5f * x;
            int i = *(int*)&x;              // Read bits as integer.
            i = 0x5F375A86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
            x = *(float*)&i;                // Convert bits back to float
            x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
            return x;
        }
        public static void Swap(ref float value1, ref float value2)
        {
            float temp = value1;
            value1 = value2;
            value2 = temp;
        }

        public static float Lerp(float startValue, float endValue, float time)
        {
            //return startValue * (1.0f - time) + endValue * time;
            return startValue + (endValue - startValue) * time;
        }

        public static void Swap(ref int value1, ref int value2)
        {
            int temp = value1;
            value1 = value2;
            value2 = temp;
        }
        public static bool Quadratic(float a, float b, float c, out float answer1, out float answer2)
        {
            if (a != 0.0f)
            {
                float mag = b * b - 4.0f * a * c;
                if (!(mag < 0.0f))
                {
                    mag = (float)Sqrt(mag);
                    a *= 2.0f;

                    answer1 = (-b + mag) / a;
                    answer2 = (-b - mag) / a;
                    return true;
                }
            }
            answer1 = 0.0f;
            answer2 = 0.0f;
            return false;
        }

        public static float AngleBetween(Vec3 vector1, Vec3 vector2)
        {
            vector1.NormalizeFast();
            vector2.NormalizeFast();

            float dot = vector1 | vector2;

            //dot is the cosine adj/hyp ratio between the two vectors, so
            //dot == 1 is same direction
            //dot == -1 is opposite direction
            //dot == 0 is a 90 degree angle

            if (dot > 0.999f)
                return 0.0f;            
            else if (dot < -0.999f)
                return 180.0f;
            else
                return RadToDeg((float)Acos(dot));
        }
        public static Vec3 AxisBetween(Vec3 initialVector, Vec3 finalVector)
        {
            initialVector.NormalizeFast();
            finalVector.NormalizeFast();

            float dot = initialVector | finalVector;

            //dot is the cosine adj/hyp ratio between the two vectors, so
            //dot == 1 is same direction
            //dot == -1 is opposite direction
            //dot == 0 is a 90 degree angle

            if (dot > 0.999f || dot < -0.999f)
                return Vec3.Forward;
            else
                return initialVector ^ finalVector;
        }
        public static void AxisAngleBetween(Vec3 initialVector, Vec3 finalVector, out Vec3 axis, out float angle)
        {
            initialVector.NormalizeFast();
            finalVector.NormalizeFast();

            float dot = initialVector | finalVector;

            //dot is the cosine adj/hyp ratio between the two vectors, so
            //dot == 1 is same direction
            //dot == -1 is opposite direction
            //dot == 0 is a 90 degree angle

            if (dot > 0.999f)
            {
                axis = -Vec3.Forward;
                angle = 0.0f;
            }
            else if (dot < -0.999f)
            {
                axis = -Vec3.Forward;
                angle = 180.0f;
            }
            else
            {
                axis = initialVector ^ finalVector;
                angle = RadToDeg((float)Acos(dot));
            }
        }
        public static float InterpQuadraticEaseEnd(float start, float end, float time, float speed = 1.0f, float power = 2.0f)
            => Lerp(start, end, 1.0f - (float)Pow(1.0f - (time * speed), power));
        public static float InterpQuadraticEaseStart(float start, float end, float time, float speed = 1.0f, float power = 2.0f)
            => Lerp(start, end, (float)Pow(time * speed, power));
        /// <summary>
        /// Smoothed interpolation between two points. Eases in and out.
        /// A speed of 2 symbolizes the interpolation will occur in half a second
        /// if update/frame delta is used as time.
        /// </summary>
        public static float InterpCosineTo(float start, float end, float time, float speed = 1.0f)
            => Lerp(start, end, (1.0f - (float)Cos(time * speed * PIf)) / 2.0f);
        /// <summary>
        /// Smoothed interpolation between two points. Eases in and out.
        /// A speed of 2 symbolizes the interpolation will occur in half a second
        /// if update/frame delta is used as time.
        /// </summary>
        public static Vec3 InterpCosineTo(Vec3 start, Vec3 end, float time, float speed = 1.0f)
            => Vec3.Lerp(start, end, (1.0f - (float)Cos(time * speed * PIf)) / 2.0f);
        /// <summary>
        /// Constant interpolation directly from one point to another.
        /// A speed of 2 symbolizes the interpolation will occur in half a second
        /// if update/frame delta is used as time.
        /// </summary>
        public static float InterpLinearTo(float start, float end, float time, float speed = 1.0f)
            => Lerp(start, end, time * speed);
        public static Vec2 InterpLinearTo(Vec2 start, Vec2 end, float time, float speed = 1.0f)
            => Vec2.Lerp(start, end, time * speed);
        public static Vec3 InterpLinearTo(Vec3 start, Vec3 end, float time, float speed = 1.0f)
            => Vec3.Lerp(start, end, time * speed);
        public static Vec4 InterpLinearTo(Vec4 start, Vec4 end, float time, float speed = 1.0f)
            => Vec4.Lerp(start, end, time * speed);

        public static Vec3 VInterpNormalRotationTo(Vec3 Current, Vec3 Target, float DeltaTime, float RotationSpeedDegrees)
        {
            Quat DeltaQuat = Quat.BetweenVectors(Current, Target);

            DeltaQuat.ToAxisAngle(out Vec3 DeltaAxis, out float DeltaAngle);

            float RotationStepRadians = RotationSpeedDegrees * (PIf / 180.0f) * DeltaTime;

	        if (Abs(DeltaAngle) > RotationStepRadians)
	        {
		        DeltaAngle = DeltaAngle.Clamp(-RotationStepRadians, RotationStepRadians);
		        DeltaQuat = new Quat(DeltaAxis, DeltaAngle);
		        return DeltaQuat * Current;
	        }
	        return Target;
        }
        public static Vec3 RotateAboutPoint(Vec3 point, Vec3 center, Rotator angles)
        {
            return point * Matrix4.CreateTranslation(-center) * angles.GetMatrix() * Matrix4.CreateTranslation(center);
        }
        public static Vec3 RotateAboutPoint(Vec3 point, Vec3 center, Quat angles)
        {
            return point * Matrix4.CreateTranslation(-center) * Matrix4.CreateFromQuaternion(angles) * Matrix4.CreateTranslation(center);
        }
        public static Vec2 RotateAboutPoint(Vec2 point, Vec2 center, float angle)
        {
            return (Vec2)((Vec3)point * Matrix4.CreateTranslation(-center) * Matrix4.CreateRotationZ(angle) * Matrix4.CreateTranslation(center));
        }
        public static Vec3 ScaleAboutPoint(Vec3 point, Vec3 center, Vec3 scale)
        {
            return point * Matrix4.CreateTranslation(-center) * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(center);
        }
        public static Vec2 ScaleAboutPoint(Vec2 point, Vec2 center, Vec2 scale)
        {
            return (Vec2)((Vec3)point * Matrix4.CreateTranslation((Vec3)(-center)) * Matrix4.CreateScale(scale.X, scale.Y, 1.0f) * Matrix4.CreateTranslation((Vec3)center));
        }
        public static Vec3 TransformAboutPoint(Vec3 point, Vec3 center, Matrix4 transform)
        {
            return point * Matrix4.CreateTranslation(-center) * transform * Matrix4.CreateTranslation(center);
        }
        
        public static float DepthToDistance(float depth, float nearZ, float farZ)
        {
            float depthSample = 2.0f * depth - 1.0f;
            float zLinear = 2.0f * nearZ * farZ / (farZ + nearZ - depthSample * (farZ - nearZ));
            return zLinear;
        }
        public static float DistanceToDepth(float z, float nearZ, float farZ)
        {
            float nonLinearDepth = (farZ + nearZ - 2.0f * nearZ * farZ / z) / (farZ - nearZ);
            nonLinearDepth = (nonLinearDepth + 1.0f) / 2.0f;
            return nonLinearDepth;
        }
        public static Circle SmallestEnclosingCircle(params Vec2[] points)
        {
            float radius = 0;
            Vec2 center = Vec2.Zero;
            List<Vec2> Q = new List<Vec2>();
            MiniballRecurse(ref center, ref radius, points.ToList(), Q);
            return new Circle(radius, center);
        }
        private static void MiniballRecurse(ref Vec2 center, ref float radius, List<Vec2> S, List<Vec2> Q)
        {
            List<Vec2> processed = new List<Vec2>();

        }

        public static float Max(params float[] values)
        {
            float v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Max(v, values[i]);
            return v;
        }
        public static int Max(params int[] values)
        {
            int v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Max(v, values[i]);
            return v;
        }
        public static uint Max(params uint[] values)
        {
            uint v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Max(v, values[i]);
            return v;
        }
        public static short Max(params short[] values)
        {
            short v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Max(v, values[i]);
            return v;
        }
        public static ushort Max(params ushort[] values)
        {
            ushort v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Max(v, values[i]);
            return v;
        }
        public static byte Max(params byte[] values)
        {
            byte v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Max(v, values[i]);
            return v;
        }
        public static sbyte Max(params sbyte[] values)
        {
            sbyte v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Max(v, values[i]);
            return v;
        }
        public static float Min(params float[] values)
        {
            float v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Min(v, values[i]);
            return v;
        }
        public static Vec3 ComponentMin(params Vec3[] values)
        {
            Vec3 value = Vec3.Zero;
            for (int i = 0; i < 3; ++i)
            {
                float v = values[0][i];
                if (values.Length > 1)
                    for (int x = 1; x < values.Length; x++)
                        v = Math.Min(v, values[x][i]);
                value[i] = v;
            }
            return value;
        }
        public static Vec3 ComponentMax(params Vec3[] values)
        {
            Vec3 value = Vec3.Zero;
            for (int i = 0; i < 3; ++i)
            {
                float v = values[0][i];
                if (values.Length > 1)
                    for (int x = 1; x < values.Length; x++)
                        v = Math.Max(v, values[x][i]);
                value[i] = v;
            }
            return value;
        }
        public static int Min(params int[] values)
        {
            int v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Min(v, values[i]);
            return v;
        }
        public static uint Min(params uint[] values)
        {
            uint v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Min(v, values[i]);
            return v;
        }
        public static short Min(params short[] values)
        {
            short v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Min(v, values[i]);
            return v;
        }
        public static ushort Min(params ushort[] values)
        {
            ushort v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Min(v, values[i]);
            return v;
        }
        public static byte Min(params byte[] values)
        {
            byte v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Min(v, values[i]);
            return v;
        }
        public static sbyte Min(params sbyte[] values)
        {
            sbyte v = values[0];
            if (values.Length > 1)
                for (int i = 1; i < values.Length; i++)
                    v = Math.Min(v, values[i]);
            return v;
        }
    }
}
