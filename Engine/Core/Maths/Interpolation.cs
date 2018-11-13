using static System.TMath;
using static System.Math;
using TheraEngine.Core.Maths.Transforms;
using System;
using System.Drawing;

namespace TheraEngine.Core.Maths
{
    /// <summary>
    /// Provides tools pertaining to interpolation.
    /// </summary>
    public static class Interp
    {
        #region Polynomials
        public static float EvaluatePolynomial(float third, float second, float first, float zero, float x)
        {
            float x2 = x * x;
            return third * x2 * x + second * x2 + first * x + zero;
        }
        public static Vec2 EvaluatePolynomial(Vec2 third, Vec2 second, Vec2 first, Vec2 zero, Vec2 x)
        {
            Vec2 x2 = x * x;
            return third * x2 * x + second * x2 + first * x + zero;
        }
        public static Vec3 EvaluatePolynomial(Vec3 third, Vec3 second, Vec3 first, Vec3 zero, Vec3 x)
        {
            Vec3 x2 = x * x;
            return third * x2 * x + second * x2 + first * x + zero;
        }
        public static Vec4 EvaluatePolynomial(Vec4 third, Vec4 second, Vec4 first, Vec4 zero, Vec4 x)
        {
            Vec4 x2 = x * x;
            return third * x2 * x + second * x2 + first * x + zero;
        }
        public static float EvaluatePolynomial(float second, float first, float zero, float x)
        {
            float x2 = x * x;
            return second * x2 + first * x + zero;
        }
        public static Vec2 EvaluatePolynomial(Vec2 second, Vec2 first, Vec2 zero, Vec2 x)
        {
            Vec2 x2 = x * x;
            return second * x2 + first * x + zero;
        }
        public static Vec3 EvaluatePolynomial(Vec3 second, Vec3 first, Vec3 zero, Vec3 x)
        {
            Vec3 x2 = x * x;
            return second * x2 + first * x + zero;
        }
        public static Vec4 EvaluatePolynomial(Vec4 second, Vec4 first, Vec4 zero, Vec4 x)
        {
            Vec4 x2 = x * x;
            return second * x2 + first * x + zero;
        }
        public static float EvaluatePolynomial(float first, float zero, float x)
        {
            return first * x + zero;
        }
        public static Vec2 EvaluatePolynomial(Vec2 first, Vec2 zero, Vec2 x)
        {
            return first * x + zero;
        }
        public static Vec3 EvaluatePolynomial(Vec3 first, Vec3 zero, Vec3 x)
        {
            return first * x + zero;
        }
        public static Vec4 EvaluatePolynomial(Vec4 first, Vec4 zero, Vec4 x)
        {
            return first * x + zero;
        }
        #endregion

        #region Bezier

        #region Point Approximation
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
        #endregion

        #region Coefficients

        #region Position
        public static void CubicBezierCoefs(
            float p0, float t0, float t1, float p1,
            out float third, out float second, out float first, out float zero)
        {
            third =        -p0 + 3.0f * t0 - 3.0f * t1 + p1;
            second = 3.0f * p0 - 6.0f * t0 + 3.0f * t1;
            first = -3.0f * p0 + 3.0f * t0;
            zero =          p0;
        }
        public static void CubicBezierCoefs(
            Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
            out Vec2 third, out Vec2 second, out Vec2 first, out Vec2 zero)
        {
            third =        -p0 + 3.0f * t0 - 3.0f * t1 + p1;
            second = 3.0f * p0 - 6.0f * t0 + 3.0f * t1;
            first = -3.0f * p0 + 3.0f * t0;
            zero =          p0;
        }
        public static void CubicBezierCoefs(
            Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
            out Vec3 third, out Vec3 second, out Vec3 first, out Vec3 zero)
        {
            third =        -p0 + 3.0f * t0 - 3.0f * t1 + p1;
            second = 3.0f * p0 - 6.0f * t0 + 3.0f * t1;
            first = -3.0f * p0 + 3.0f * t0;
            zero =          p0;
        }
        public static void CubicBezierCoefs(
            Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
            out Vec4 third, out Vec4 second, out Vec4 first, out Vec4 zero)
        {
            third =        -p0 + 3.0f * t0 - 3.0f * t1 + p1;
            second = 3.0f * p0 - 6.0f * t0 + 3.0f * t1;
            first = -3.0f * p0 + 3.0f * t0;
            zero =          p0;
        }
        #endregion

        #region Velocity
        public static void CubicBezierVelocityCoefs(
            float p0, float t0, float t1, float p1,
            out float second, out float first, out float zero)
        {
            second = 3.0f * ( 3.0f * t0 - p0 - 3.0f * t1 + p1);
            first  = 6.0f * (-2.0f * t0 + p0 +        t1);
            zero   = 3.0f * (        t0 - p0);
        }
        public static void CubicBezierVelocityCoefs(
            Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
            out Vec2 second, out Vec2 first, out Vec2 zero)
        {
            second = 3.0f * ( 3.0f * t0 - p0 - 3.0f * t1 + p1);
            first  = 6.0f * (-2.0f * t0 + p0 +        t1);
            zero   = 3.0f * (        t0 - p0);
        }
        public static void CubicBezierVelocityCoefs(
            Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
            out Vec3 second, out Vec3 first, out Vec3 zero)
        {
            second = 3.0f * ( 3.0f * t0 - p0 - 3.0f * t1 + p1);
            first  = 6.0f * (-2.0f * t0 + p0 +        t1);
            zero   = 3.0f * (        t0 - p0);
        }
        public static void CubicBezierVelocityCoefs(
            Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
            out Vec4 second, out Vec4 first, out Vec4 zero)
        {
            second = 3.0f * ( 3.0f * t0 - p0 - 3.0f * t1 + p1);
            first  = 6.0f * (-2.0f * t0 + p0 +        t1);
            zero   = 3.0f * (        t0 - p0);
        }
        #endregion

        #region Acceleration
        public static void CubicBezierAccelerationCoefs(
           float p0, float t0, float t1, float p1,
           out float first, out float zero)
        {
            first = 6.0f * (-3.0f * t1 + 3.0f * t0 - p0 + p1);
            zero  = 6.0f * (        t1 - 2.0f * t0 + p0);
        }
        public static void CubicBezierAccelerationCoefs(
           Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
           out Vec2 first, out Vec2 zero)
        {
            first = 6.0f * (-3.0f * t1 + 3.0f * t0 - p0 + p1);
            zero  = 6.0f * (        t1 - 2.0f * t0 + p0);
        }
        public static void CubicBezierAccelerationCoefs(
           Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
           out Vec3 first, out Vec3 zero)
        {
            first = 6.0f * (-3.0f * t1 + 3.0f * t0 - p0 + p1);
            zero  = 6.0f * (        t1 - 2.0f * t0 + p0);
        }
        public static void CubicBezierAccelerationCoefs(
           Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
           out Vec4 first, out Vec4 zero)
        {
            first = 6.0f * (-3.0f * t1 + 3.0f * t0 - p0 + p1);
            zero  = 6.0f * (        t1 - 2.0f * t0 + p0);
        }
        #endregion

        #endregion

        #region Position
        public static float CubicBezier(float p0, float t0, float t1, float p1, float time)
        {
            CubicBezierCoefs(p0, t0, t1, p1, out float third, out float second, out float first, out float zero);
            return EvaluatePolynomial(third, second, first, zero, time);
        }
        public static Vec2 CubicBezier(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            CubicBezierCoefs(p0, t0, t1, p1, out Vec2 third, out Vec2 second, out Vec2 first, out Vec2 zero);
            return EvaluatePolynomial(third, second, first, zero, time);
        }
        public static Vec3 CubicBezier(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            CubicBezierCoefs(p0, t0, t1, p1, out Vec3 third, out Vec3 second, out Vec3 first, out Vec3 zero);
            return EvaluatePolynomial(third, second, first, zero, time);
        }
        public static Vec4 CubicBezier(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            CubicBezierCoefs(p0, t0, t1, p1, out Vec4 third, out Vec4 second, out Vec4 first, out Vec4 zero);
            return EvaluatePolynomial(third, second, first, zero, new Vec4(time));
        }
        #endregion

        #region Velocity
        public static float CubicBezierVelocity(float p0, float t0, float t1, float p1, float time)
        {
            CubicBezierVelocityCoefs(p0, t0, t1, p1, out float second, out float first, out float zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        public static Vec2 CubicBezierVelocity(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            CubicBezierVelocityCoefs(p0, t0, t1, p1, out Vec2 second, out Vec2 first, out Vec2 zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        public static Vec3 CubicBezierVelocity(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            CubicBezierVelocityCoefs(p0, t0, t1, p1, out Vec3 second, out Vec3 first, out Vec3 zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        public static Vec4 CubicBezierVelocity(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            CubicBezierVelocityCoefs(p0, t0, t1, p1, out Vec4 second, out Vec4 first, out Vec4 zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        #endregion

        #region Acceleration
        public static float CubicBezierAcceleration(float p0, float t0, float t1, float p1, float time)
        {
            CubicBezierAccelerationCoefs(p0, t0, t1, p1, out float first, out float zero);
            return EvaluatePolynomial(first, zero, time);
        }
        public static Vec2 CubicBezierAcceleration(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            CubicBezierAccelerationCoefs(p0, t0, t1, p1, out Vec2 first, out Vec2 zero);
            return EvaluatePolynomial(first, zero, time);
        }
        public static Vec3 CubicBezierAcceleration(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            CubicBezierAccelerationCoefs(p0, t0, t1, p1, out Vec3 first, out Vec3 zero);
            return EvaluatePolynomial(first, zero, time);
        }
        public static Vec4 CubicBezierAcceleration(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            CubicBezierAccelerationCoefs(p0, t0, t1, p1, out Vec4 first, out Vec4 zero);
            return EvaluatePolynomial(first, zero, time);
        }
        #endregion

        #endregion

        #region Hermite

        #region Coefficients

        #region Position
        public static void CubicHermiteCoefs(
            float p0, float t0, float t1, float p1,
            out float third, out float second, out float first, out float zero)
        {
            third  = ( 2.0f * p0 +        t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first  = t0;
            zero   = p0;
        }
        public static void CubicHermiteCoefs(
            Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
            out Vec2 third, out Vec2 second, out Vec2 first, out Vec2 zero)
        {
            third  = ( 2.0f * p0 +        t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first  = t0;
            zero   = p0;
        }
        public static void CubicHermiteCoefs(
            Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
            out Vec3 third, out Vec3 second, out Vec3 first, out Vec3 zero)
        {
             third  = ( 2.0f * p0 +        t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first  = t0;
            zero   = p0;
        }
        public static void CubicHermiteCoefs(
            Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
            out Vec4 third, out Vec4 second, out Vec4 first, out Vec4 zero)
        {
            third  = ( 2.0f * p0 +        t0 - 2.0f * p1 + t1);
            second = (-3.0f * p0 - 2.0f * t0 + 3.0f * p1 - t1);
            first  = t0;
            zero   = p0;
        }
        #endregion

        #region Velocity
        public static void CubicHermiteVelocityCoefs(
           float p0, float t0, float t1, float p1,
           out float second, out float first, out float zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        public static void CubicHermiteVelocityCoefs(
           Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
           out Vec2 second, out Vec2 first, out Vec2 zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        public static void CubicHermiteVelocityCoefs(
           Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
           out Vec3 second, out Vec3 first, out Vec3 zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        public static void CubicHermiteVelocityCoefs(
           Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
           out Vec4 second, out Vec4 first, out Vec4 zero)
        {
            second = 6.0f * p0 + 3.0f * t0 - 6.0f * p1 + 3.0f * t1;
            first = -6.0f * p0 - 4.0f * t0 + 6.0f * p1 - 2.0f * t1;
            zero = t0;
        }
        #endregion

        #region Acceleration
        public static void CubicHermiteAccelerationCoefs(
           float p0, float t0, float t1, float p1,
           out float first, out float zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero  = -6.0f * p0 - 4.0f * t0 +  6.0f * p1 - 2.0f * t1;
        }
        public static void CubicHermiteAccelerationCoefs(
           Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1,
           out Vec2 first, out Vec2 zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero  = -6.0f * p0 - 4.0f * t0 +  6.0f * p1 - 2.0f * t1;
        }
        public static void CubicHermiteAccelerationCoefs(
           Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1,
           out Vec3 first, out Vec3 zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero  = -6.0f * p0 - 4.0f * t0 +  6.0f * p1 - 2.0f * t1;
        }
        public static void CubicHermiteAccelerationCoefs(
           Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1,
           out Vec4 first, out Vec4 zero)
        {
            first = 12.0f * p0 + 6.0f * t0 - 12.0f * p1 + 6.0f * t1;
            zero  = -6.0f * p0 - 4.0f * t0 +  6.0f * p1 - 2.0f * t1;
        }
        #endregion

        #endregion

        #region Position
        public static float CubicHermite(float p0, float t0, float t1, float p1, float time)
        {
            CubicHermiteCoefs(p0, t0, t1, p1, out float third, out float second, out float first, out float zero);
            return EvaluatePolynomial(third, second, first, zero, time);
        }
        public static Vec2 CubicHermite(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            CubicHermiteCoefs(p0, t0, t1, p1, out Vec2 third, out Vec2 second, out Vec2 first, out Vec2 zero);
            return EvaluatePolynomial(third, second, first, zero, time);
        }
        public static Vec3 CubicHermite(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            CubicHermiteCoefs(p0, t0, t1, p1, out Vec3 third, out Vec3 second, out Vec3 first, out Vec3 zero);
            return EvaluatePolynomial(third, second, first, zero, time);
        }
        public static Vec4 CubicHermite(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            CubicHermiteCoefs(p0, t0, t1, p1, out Vec4 third, out Vec4 second, out Vec4 first, out Vec4 zero);
            return EvaluatePolynomial(third, second, first, zero, time);
        }
        #endregion

        #region Velocity
        public static float CubicHermiteVelocity(float p0, float t0, float t1, float p1, float time)
        {
            CubicHermiteVelocityCoefs(p0, t0, t1, p1, out float second, out float first, out float zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        public static Vec2 CubicHermiteVelocity(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            CubicHermiteVelocityCoefs(p0, t0, t1, p1, out Vec2 second, out Vec2 first, out Vec2 zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        public static Vec3 CubicHermiteVelocity(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            CubicHermiteVelocityCoefs(p0, t0, t1, p1, out Vec3 second, out Vec3 first, out Vec3 zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        public static Vec4 CubicHermiteVelocity(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            CubicHermiteVelocityCoefs(p0, t0, t1, p1, out Vec4 second, out Vec4 first, out Vec4 zero);
            return EvaluatePolynomial(second, first, zero, time);
        }
        #endregion

        #region Acceleration
        public static float CubicHermiteAcceleration(float p0, float t0, float t1, float p1, float time)
        {
            CubicHermiteAccelerationCoefs(p0, t0, t1, p1, out float first, out float zero);
            return EvaluatePolynomial(first, zero, time);
        }
        public static Vec2 CubicHermiteAcceleration(Vec2 p0, Vec2 t0, Vec2 t1, Vec2 p1, float time)
        {
            CubicHermiteAccelerationCoefs(p0, t0, t1, p1, out Vec2 first, out Vec2 zero);
            return EvaluatePolynomial(first, zero, time);
        }
        public static Vec3 CubicHermiteAcceleration(Vec3 p0, Vec3 t0, Vec3 t1, Vec3 p1, float time)
        {
            CubicHermiteAccelerationCoefs(p0, t0, t1, p1, out Vec3 first, out Vec3 zero);
            return EvaluatePolynomial(first, zero, time);
        }
        public static Vec4 CubicHermiteAcceleration(Vec4 p0, Vec4 t0, Vec4 t1, Vec4 p1, float time)
        {
            CubicHermiteAccelerationCoefs(p0, t0, t1, p1, out Vec4 first, out Vec4 zero);
            return EvaluatePolynomial(first, zero, time);
        }
        #endregion

        #endregion

        private delegate byte ComponentSelector(Color color);
        private static readonly ComponentSelector _redSelector = color => color.R;
        private static readonly ComponentSelector _greenSelector = color => color.G;
        private static readonly ComponentSelector _blueSelector = color => color.B;

        public static Color Lerp(
            Color startColor,
            Color endColor,
            float time)
        {
            time = time.Clamp(0.0f, 1.0f);
            Color color = Color.FromArgb(
                InterpColorComponent(startColor, endColor, time, _redSelector),
                InterpColorComponent(startColor, endColor, time, _greenSelector),
                InterpColorComponent(startColor, endColor, time, _blueSelector)
            );
            return color;
        }
        private static byte InterpColorComponent(
            Color endPoint1,
            Color endPoint2,
            float time,
            ComponentSelector selector)
            => (byte)(selector(endPoint1) + (selector(endPoint2) - selector(endPoint1)) * time);

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
            => Lerp(start, end, (1.0f - (float)Cos(time * speed * PIf)) * 0.5f);
        /// <summary>
        /// Smoothed interpolation between two points. Eases in and out.
        /// A speed of 2 symbolizes the interpolation will occur in half a second
        /// if update/frame delta is used as time.
        /// </summary>
        public static Vec3 InterpCosineTo(Vec3 start, Vec3 end, float time, float speed = 1.0f)
            => Vec3.Lerp(start, end, (1.0f - (float)Cos(time * speed * PIf)) * 0.5f);
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

        public static Vec3 VInterpNormalRotationTo(Vec3 current, Vec3 target, float delta, float rotationSpeedDegrees)
        {
            Quat deltaQuat = Quat.BetweenVectors(current, target);

            deltaQuat.ToAxisAngle(out Vec3 deltaAxis, out float deltaAngle);

            float rotStepRads = DegToRad(rotationSpeedDegrees) * delta;

            if (Abs(deltaAngle) > rotStepRads)
            {
                deltaAngle = deltaAngle.Clamp(-rotStepRads, rotStepRads);
                deltaQuat = new Quat(deltaAxis, deltaAngle);
                return deltaQuat * current;
            }

            return target;
        }
    }
}
