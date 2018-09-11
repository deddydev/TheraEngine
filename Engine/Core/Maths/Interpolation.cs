using static System.TMath;
using static System.Math;
using TheraEngine.Core.Maths.Transforms;

namespace System
{
    /// <summary>
    /// Provides tools pertaining to interpolation.
    /// </summary>
    public static class Interp
    {
        #region Bezier
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

                length += s.DistanceToFast(OldPos);
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
                points[i] = CubicBezier(p0, p1, p2, p3, timeDelta * i);

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
        #endregion

        #region Hermite
        public static void CubicHermiteCoefs(
            float p0, float t0, float t1, float p1,
            out float third, out float second, out float first, out float zero)
        {
            third = (2.0f * p0 + t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first = t0;
            zero = p0;
        }
        public static void CubicHermiteVelocityCoefs(
           float p0, float t0, float t1, float p1,
           out float second, out float first, out float zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        public static void CubicHermiteAccelerationCoefs(
           float p0, float t0, float t1, float p1,
           out float first, out float zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
        }
        public static void CubicHermiteCoefs(
            Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
            out Vec2 third, out Vec2 second, out Vec2 first, out Vec2 zero)
        {
            third = (2.0f * p0 + t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first = t0;
            zero = p0;
        }
        public static void CubicHermiteVelocityCoefs(
           Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
           out Vec2 second, out Vec2 first, out Vec2 zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        public static void CubicHermiteAccelerationCoefs(
           Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
           out Vec2 first, out Vec2 zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
        }
        public static void CubicHermiteCoefs(
            Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
            out Vec3 third, out Vec3 second, out Vec3 first, out Vec3 zero)
        {
            third = (2.0f * p0 + t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first = t0;
            zero = p0;
        }
        public static void CubicHermiteVelocityCoefs(
           Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
           out Vec3 second, out Vec3 first, out Vec3 zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        public static void CubicHermiteAccelerationCoefs(
           Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
           out Vec3 first, out Vec3 zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
        }
        public static void CubicHermiteCoefs(
            Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
            out Vec4 third, out Vec4 second, out Vec4 first, out Vec4 zero)
        {
            third = (2.0f * p0 + t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first = t0;
            zero = p0;
        }
        public static void CubicHermiteVelocityCoefs(
           Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
           out Vec4 second, out Vec4 first, out Vec4 zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        public static void CubicHermiteAccelerationCoefs(
           Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
           out Vec4 first, out Vec4 zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
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
        #endregion

        public static float Lerp(float startValue, float endValue, float time)
        {
            //return startValue * (1.0f - time) + endValue * time;
            return startValue + (endValue - startValue) * time;
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
    }
}
