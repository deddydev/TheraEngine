﻿using Extensions;
using System;
using System.Linq;
using System.Numerics;
using TheraEngine.Core.Maths.Transforms;
using static System.Math;

namespace TheraEngine.Core.Maths
{
    /// <summary>
    /// Thera Math: contains static methods and constants not included in the built-in System.Math class.<para></para>
    /// Easily call methods in this class by implementing 'using static TheraEngine.Core.Maths.TMath;' at the top of code files.
    /// </summary>
    public unsafe static class TMath
    {
        /// <summary>
        /// A small number.
        /// </summary>
        public const float Epsilon = 0.00001f;
        /// <summary>
        /// 2 * PI represented as a float value.
        /// </summary>
        public static readonly float TwoPIf = 2.0f * PIf;
        /// <summary>
        /// 2 * PI represented as a double value.
        /// </summary>
        public static readonly double TwoPI = 2.0 * PI;
        /// <summary>
        /// PI represented as a float value.
        /// </summary>
        public const float PIf = 3.1415926535897931f;
        /// <summary>
        /// e represented as a double value.
        /// </summary>
        //public const double E = 2.7182818284590451;
        /// <summary>
        /// e represented as a float value.
        /// </summary>
        public const float Ef = 2.7182818284590451f;
        /// <summary>
        /// Multiply this constant by a degree value to convert to radians.
        /// </summary>
        public static readonly double DegToRadMult = PI / 180.0;
        /// <summary>
        /// Multiply this constant by a degree value to convert to radians.
        /// </summary>
        public static readonly float DegToRadMultf = PIf / 180.0f;
        /// <summary>
        /// Multiply this constant by a radian value to convert to degrees.
        /// </summary>
        public static readonly double RadToDegMult = 180.0 / PI;
        /// <summary>
        /// Multiply this constant by a radian value to convert to degrees.
        /// </summary>
        public static readonly float RadToDegMultf = 180.0f / PIf;
        /// <summary>
        /// Converts the given value in degrees to radians.
        /// </summary>
        public static double DegToRad(double degrees) => degrees * DegToRadMult;
        /// <summary>
        /// Converts the given value in radians to degrees.
        /// </summary>
        public static double RadToDeg(double radians) => radians * RadToDegMult;
        /// <summary>
        /// Converts the given value in degrees to radians.
        /// </summary>
        public static float DegToRad(float degrees) => degrees * DegToRadMultf;
        /// <summary>
        /// Converts the given value in radians to degrees.
        /// </summary>
        public static float RadToDeg(float radians) => radians * RadToDegMultf;
        /// <summary>
        /// Converts the given value in degrees to radians.
        /// </summary>
        public static Vec2 DegToRad(Vec2 degrees) => degrees * DegToRadMultf;
        /// <summary>
        /// Converts the given value in radians to degrees.
        /// </summary>
        public static Vec2 RadToDeg(Vec2 radians) => radians * RadToDegMultf;
        /// <summary>
        /// Converts the given value in degrees to radians.
        /// </summary>
        public static Vec3 DegToRad(Vec3 degrees) => degrees * DegToRadMultf;
        /// <summary>
        /// Converts the given value in radians to degrees.
        /// </summary>
        public static Vec3 RadToDeg(Vec3 radians) => radians * RadToDegMultf;

        /// <summary>
        /// Returns the most significant decimal digit.
        /// <para>250 -> 100</para>
        /// <para>12 -> 10</para>
        /// <para>5 -> 1</para>
        /// <para>0.5 -> 0.1</para>
        /// </summary>
        public static float MostSignificantDigit(float value)
        {
            float n = 1;

            float abs = Abs(value);
            float sig = Sign(value);

            if (abs > 1.0f)
            {
                while (n < abs)
                    n *= 10.0f;

                return (int)Floor(sig * n * 0.1f);
            }
            else // n <= 1
            {
                while (n > abs)
                    n *= 0.1f;
                
                return sig * n;
            }
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
            x *= (1.5f - xhalf * x * x);    // Perform left single Newton-Raphson step.
            return x;
        }

        public static void CartesianToPolarDeg(Vec2 vector, out float angle, out float radius)
        {
            radius = vector.Length;
            angle = Atan2df(vector.Y, vector.X);
        }
        public static void CartesianToPolarRad(Vec2 vector, out float angle, out float radius)
        {
            radius = vector.Length;
            angle = Atan2f(vector.Y, vector.X);
        }
        public static Vec2 PolarToCartesianDeg(float degree, float radius)
        {
            SinCosdf(degree, out float sin, out float cos);
            return new Vec2(cos * radius, sin * radius);
        }
        public static Vec2 PolarToCartesianRad(float radians, float radius)
        {
            SinCosf(radians, out float sin, out float cos);
            return new Vec2(cos * radius, sin * radius);
        }

        /// <summary>
        /// Returns a translation value representing a rotation of the cameraPoint around the focusPoint.
        /// Assumes the Y axis is up. Yaw is performed before pitch.
        /// </summary>
        /// <param name="pitch">Rotation about the X axis, after yaw.</param>
        /// <param name="yaw">Rotation about the Y axis.</param>
        /// <param name="focusPoint">The point to rotate around.</param>
        /// <param name="cameraPoint">The point to move.</param>
        /// <param name="cameraRightDir">The direction representing the right side of a camera. This is the reference axis rotated around (at the focusPoint) using the pitch value.</param>
        /// <returns></returns>
        public static Vec3 ArcballTranslation(float pitch, float yaw, Vec3 focusPoint, Vec3 cameraPoint, Vec3 cameraRightDir)
            => ArcballTranslation(Quat.FromAxisAngleDeg(Vec3.Up, yaw) * Quat.FromAxisAngleDeg(cameraRightDir, pitch), focusPoint, cameraPoint);
        /// <summary>
        /// Returns a translation value representing a rotation of the cameraPoint around the focusPoint.
        /// Assumes the Y axis is up. Yaw is performed before pitch.
        /// </summary>
        /// <param name="rotation">Rotation about the X axis, after yaw.</param>
        /// <param name="focusPoint">The point to rotate around.</param>
        /// <param name="cameraPoint">The point to move.</param>
        /// <returns></returns>
        public static Vec3 ArcballTranslation(Quat rotation, Vec3 focusPoint, Vec3 cameraPoint)
            => focusPoint + rotation * (cameraPoint - focusPoint);
        
        /// <summary>
        /// Returns the sine and cosine of a radian angle simultaneously as doubles.
        /// </summary>
        public static void SinCos(double rad, out double sin, out double cos)
        {
            sin = Sin(rad);
            cos = Cos(rad);
        }
        /// <summary>
        /// Returns the sine and cosine of a radian angle simultaneously as floats.
        /// </summary>
        public static void SinCosf(float rad, out float sin, out float cos)
        {
            sin = Sinf(rad);
            cos = Cosf(rad);
        }
        /// <summary>
        /// Returns the sine and cosine of a degree angle simultaneously as doubles.
        /// </summary>
        public static void SinCosd(double deg, out double sin, out double cos)
        {
            sin = Sind(deg);
            cos = Cosd(deg);
        }
        /// <summary>
        /// Returns the sine and cosine of a degree angle simultaneously as floats.
        /// </summary>
        public static void SinCosdf(float deg, out float sin, out float cos)
        {
            sin = Sindf(deg);
            cos = Cosdf(deg);
        }

        /// <summary>
        /// Cosine as float, from radians
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static float Cosf(float rad) => (float)Cos(rad);
        /// <summary>
        /// Sine as float, from radians
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static float Sinf(float rad) => (float)Sin(rad);
        /// <summary>
        /// Tangent as float, from radians
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static float Tanf(float rad) => (float)Tan(rad);

        /// <summary>
        /// Cosine from degrees, as float
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static float Cosdf(float deg) => Cosf(deg * DegToRadMultf);
        /// <summary>
        /// Sine from degrees, as float
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static float Sindf(float deg) => Sinf(deg * DegToRadMultf);
        /// <summary>
        /// Tangent from degrees, as float
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static float Tandf(float deg) => Tanf(deg * DegToRadMultf);

        /// <summary>
        /// Arc cosine, as float. Returns radians
        /// </summary>
        /// <param name="cos"></param>
        /// <returns></returns>
        public static float Acosf(float cos) => (float)Acos(cos);
        /// <summary>
        /// Arc sine, as float. Returns radians
        /// </summary>
        /// <param name="sin"></param>
        /// <returns></returns>
        public static float Asinf(float sin) => (float)Asin(sin);
        /// <summary>
        /// Arc tangent, as float. Returns radians
        /// </summary>
        /// <param name="tan"></param>
        /// <returns></returns>
        public static float Atanf(float tan) => (float)Atan(tan);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tanY"></param>
        /// <param name="tanX"></param>
        /// <returns></returns>
        public static float Atan2f(float tanY, float tanX) => (float)Atan2(tanY, tanX);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cos"></param>
        /// <returns></returns>
        public static float Acosdf(float cos) => Acosf(cos) * RadToDegMultf;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sin"></param>
        /// <returns></returns>
        public static float Asindf(float sin) => Asinf(sin) * RadToDegMultf;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tan"></param>
        /// <returns></returns>
        public static float Atandf(float tan) => Atanf(tan) * RadToDegMultf;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tanY"></param>
        /// <param name="tanX"></param>
        /// <returns></returns>
        public static float Atan2df(float tanY, float tanX) => Atan2f(tanY, tanX) * RadToDegMultf;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double Cosd(double deg) => Cos(deg * DegToRadMult);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double Sind(double deg) => Sin(deg * DegToRadMult);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double Tand(double deg) => Tan(deg * DegToRadMult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        public static float Powf(float value, float exponent) => (float)Pow(value, exponent);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Sigmoidf(float value) => 1.0f / (1.0f + Powf(Ef, -value));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Sigmoid(double value) => 1.0 / (1.0 + Pow(E, -value));
        /// <summary>
        /// Finds the two values of x where the equation ax^2 + bx + c evaluates to 0.
        /// Returns false if the solutions are not a real numbers.
        /// </summary>
        public static bool QuadraticRealRoots(float a, float b, float c, out float x1, out float x2)
        {
            if (a != 0.0f)
            {
                float mag = b * b - 4.0f * a * c;
                if (mag >= 0.0f)
                {
                    mag = (float)Sqrt(mag);
                    a *= 2.0f;

                    x1 = (-b + mag) / a;
                    x2 = (-b - mag) / a;
                    return true;
                }
            }
            else if (b != 0.0f)
            {
                x1 = x2 = -c / b;
                return true;
            }
            else if (c != 0.0f)
            {
                x1 = 0.0f;
                x2 = 0.0f;
                return true;
            }
            x1 = 0.0f;
            x2 = 0.0f;
            return false;
        }       
        /// <summary>
        /// Finds the two values of x where the equation ax^2 + bx + c evaluates to 0.
        /// </summary>
        public static bool QuadraticRoots(float a, float b, float c, out Complex x1, out Complex x2)
        {
            if (a != 0.0f)
            {
                float mag = b * b - 4.0f * a * c;

                a *= 2.0f;
                b /= a;

                if (mag >= 0.0f)
                {
                    mag = (float)Sqrt(mag) / a;

                    x1 = new Complex(-b + mag, 0.0);
                    x2 = new Complex(-b - mag, 0.0);
                }
                else
                {
                    mag = (float)Sqrt(-mag) / a;

                    x1 = new Complex(-b, mag);
                    x2 = new Complex(-b, -mag);
                }
                
                return true;
            }
            else if (b != 0.0f)
            {
                x1 = x2 = -c / b;
                return true;
            }
            else
            {
                x1 = x2 = 0.0f;
                return false;
            }
        }
        public static Vec3 Morph(Vec3 baseCoord, (Vec3 Position, float Weight)[] targets, bool relative = false)
        {
            if (relative)
            {
                Vec3 morphed = baseCoord;
                foreach (var (Position, Weight) in targets)
                    morphed += Position * Weight;
                return morphed;
            }
            else
            {
                Vec3 morphed = Vec3.Zero;
                float weightSum = 0.0f;
                foreach (var (Position, Weight) in targets)
                {
                    morphed += Position * Weight;
                    weightSum += Weight;
                }
                float invWeight = 1.0f - weightSum;
                return morphed + baseCoord * invWeight;
            }
        }
        /// <summary>
        /// Returns the angle between two vectors.
        /// </summary>
        public static float AngleBetween(Vec3 vector1, Vec3 vector2)
        {
            vector1.Normalize();
            vector2.Normalize();

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
        /// <summary>
        /// Returns the rotation axis direction vector that is perpendicular to the two vectors.
        /// </summary>
        public static Vec3 AxisBetween(Vec3 initialVector, Vec3 finalVector)
        {
            initialVector.Normalize();
            finalVector.Normalize();

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
        /// <summary>
        /// Returns a rotation axis and angle between two vectors.
        /// </summary>
        public static void AxisAngleBetween(Vec3 initialVector, Vec3 finalVector, out Vec3 axis, out float angle)
        {
            initialVector.Normalize();
            finalVector.Normalize();

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

        /// <summary>
        /// Converts nonlinear normalized depth between 0.0f and 1.0f
        /// to a linear distance value between nearZ and farZ.
        /// </summary>
        public static float DepthToDistance(float depth, float nearZ, float farZ)
        {
            float depthSample = 2.0f * depth - 1.0f;
            float zLinear = 2.0f * nearZ * farZ / (farZ + nearZ - depthSample * (farZ - nearZ));
            return zLinear;
        }
        /// <summary>
        /// Converts a linear distance value between nearZ and farZ
        /// to nonlinear normalized depth between 0.0f and 1.0f.
        /// </summary>
        public static float DistanceToDepth(float z, float nearZ, float farZ)
        {
            float nonLinearDepth = (farZ + nearZ - 2.0f * nearZ * farZ / z.ClampMin(0.001f)) / (farZ - nearZ);
            nonLinearDepth = (nonLinearDepth + 1.0f) / 2.0f;
            return nonLinearDepth;
        }

        public static Vec3 JacobiMethod(Matrix3 inputMatrix, Vec3 expectedOutcome, int iterations)
        {
            Vec3 solvedVector = Vec3.Zero;
            for (int step = 0; step < iterations; ++step)
            {
                for (int row = 0; row < 3; ++row)
                {
                    float sigma = 0.0f;
                    for (int col = 0; col < 3; ++col)
                    {
                        if (col != row)
                            sigma += inputMatrix[row, col] * solvedVector[col];
                    }
                    solvedVector[row] = (expectedOutcome[row] - sigma) / inputMatrix[row, row];
                }
                //Engine.PrintLine("Step #" + step + ": " + solvedVector.ToString());
            }
            return solvedVector;
        }
        public static Vec4 JacobiMethod(Matrix4 inputMatrix, Vec4 expectedOutcome, int iterations)
        {
            Vec4 solvedVector = Vec4.Zero;
            for (int step = 0; step < iterations; ++step)
            {
                for (int row = 0; row < 4; ++row)
                {
                    float sigma = 0.0f;
                    for (int col = 0; col < 4; ++col)
                    {
                        if (col != row)
                            sigma += inputMatrix[row, col] * solvedVector[col];
                    }
                    solvedVector[row] = (expectedOutcome[row] - sigma) / inputMatrix[row, row];
                }
                //Engine.PrintLine("Step #" + step + ": " + solvedVector.ToString());
            }
            return solvedVector;
        }
        
        #region Transforms
        public static Vec3 RotateAboutPoint(Vec3 point, Vec3 center, Rotator angles)
        {
            return point * (Matrix4.CreateTranslation(center) * (Matrix4.CreateTranslation(-center) * angles.GetMatrix()));
        }
        public static Vec3 RotateAboutPoint(Vec3 point, Vec3 center, Quat rotation)
        {
            return point * (Matrix4.CreateTranslation(center) * (Matrix4.CreateTranslation(-center) * Matrix4.CreateFromQuaternion(rotation)));
        }
        public static Vec3 ScaleAboutPoint(Vec3 point, Vec3 center, Vec3 scale)
        {
            return point * (Matrix4.CreateTranslation(center) * (Matrix4.CreateTranslation(-center) * Matrix4.CreateScale(scale)));
        }
        public static Vec2 RotateAboutPoint(Vec2 point, Vec2 center, float angle)
        {
            return (Vec2)((Vec3)point * (Matrix4.CreateTranslation(center) * (Matrix4.CreateTranslation(-center) * Matrix4.CreateRotationZ(angle))));
        }
        public static Vec2 ScaleAboutPoint(Vec2 point, Vec2 center, Vec2 scale)
        {
            return (Vec2)((Vec3)point * (Matrix4.CreateTranslation(center) * (Matrix4.CreateTranslation(-center) * Matrix4.CreateScale(scale.X, scale.Y, 1.0f))));
        }
        public static Vec3 TransformAboutPoint(Vec3 point, Vec3 center, Matrix4 transform)
        {
            return point * (Matrix4.CreateTranslation(center) * (Matrix4.CreateTranslation(-center) * transform));
        }
        #endregion

        #region Min/Max
        public static float Max(params float[] values)
        {
            float max = float.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static double Max(params double[] values)
        {
            double max = double.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static decimal Max(params decimal[] values)
        {
            decimal max = decimal.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static int Max(params int[] values)
        {
            int max = int.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static uint Max(params uint[] values)
        {
            uint max = uint.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static short Max(params short[] values)
        {
            short max = short.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static ushort Max(params ushort[] values)
        {
            ushort max = ushort.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static byte Max(params byte[] values)
        {
            byte max = byte.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static sbyte Max(params sbyte[] values)
        {
            sbyte max = sbyte.MinValue;
            for (int i = 0; i < values.Length; i++)
                max = Math.Max(max, values[i]);
            return max;
        }
        public static Vec2 ComponentMax(params Vec2[] values)
        {
            Vec2 max = Vec2.Min;
            for (int i = 0; i < 2; ++i)
                for (int x = 0; x < values.Length; x++)
                    max[i] = Math.Max(max[i], values[x][i]);
            return max;
        }
        public static Vec3 ComponentMax(params Vec3[] values)
        {
            Vec3 max = Vec3.Min;
            for (int i = 0; i < 3; ++i)
                for (int x = 0; x < values.Length; x++)
                    max[i] = Math.Max(max[i], values[x][i]);
            return max;
        }
        public static Vec4 ComponentMax(params Vec4[] values)
        {
            Vec4 max = Vec4.Min;
            for (int i = 0; i < 4; ++i)
                for (int x = 0; x < values.Length; x++)
                    max[i] = Math.Max(max[i], values[x][i]);
            return max;
        }
        public static float Min(params float[] values)
        {
            float min = float.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static double Min(params double[] values)
        {
            double min = double.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static decimal Min(params decimal[] values)
        {
            decimal min = decimal.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static int Min(params int[] values)
        {
            int min = int.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static uint Min(params uint[] values)
        {
            uint min = uint.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static short Min(params short[] values)
        {
            short min = short.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static ushort Min(params ushort[] values)
        {
            ushort min = ushort.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static byte Min(params byte[] values)
        {
            byte min = byte.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static sbyte Min(params sbyte[] values)
        {
            sbyte min = sbyte.MaxValue;
            for (int i = 0; i < values.Length; i++)
                min = Math.Min(min, values[i]);
            return min;
        }
        public static Vec2 ComponentMin(params Vec2[] values)
        {
            Vec2 min = Vec2.Max;
            for (int i = 0; i < 2; ++i)
                for (int x = 0; x < values.Length; x++)
                    min[i] = Math.Min(min[i], values[x][i]);
            return min;
        }
        public static Vec3 ComponentMin(params Vec3[] values)
        {
            Vec3 min = Vec3.Max;
            for (int i = 0; i < 3; ++i)
                for (int x = 0; x < values.Length; x++)
                    min[i] = Math.Min(min[i], values[x][i]);
            return min;
        }
        public static Vec4 ComponentMin(params Vec4[] values)
        {
            Vec4 min = Vec4.Max;
            for (int i = 0; i < 4; ++i)
                for (int x = 0; x < values.Length; x++)
                    min[i] = Math.Min(min[i], values[x][i]);
            return min;
        }
        public static void MinMax(out float min, out float max, params float[] values)
        {
            min = float.MaxValue;
            max = float.MinValue;
            float value;
            for (int i = 0; i < values.Length; i++)
            {
                value = values[i];
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
        }
        public static void ComponentMinMax(out Vec2 min, out Vec2 max, params Vec2[] values)
        {
            min = Vec2.Max;
            max = Vec2.Min;
            float value;
            for (int i = 0; i < 2; ++i)
                for (int x = 0; x < values.Length; x++)
                {
                    value = values[x][i];
                    min[i] = Math.Min(min[i], value);
                    max[i] = Math.Max(max[i], value);
                }
        }
        public static void ComponentMinMax(out Vec3 min, out Vec3 max, params Vec3[] values)
        {
            min = Vec3.Max;
            max = Vec3.Min;
            float value;
            for (int i = 0; i < 3; ++i)
                for (int x = 0; x < values.Length; x++)
                {
                    value = values[x][i];
                    min[i] = Math.Min(min[i], value);
                    max[i] = Math.Max(max[i], value);
                }
        }
        public static void ComponentMinMax(out Vec4 min, out Vec4 max, params Vec4[] values)
        {
            min = Vec4.Max;
            max = Vec4.Min;
            float value;
            for (int i = 0; i < 4; ++i)
                for (int x = 0; x < values.Length; x++)
                {
                    value = values[x][i];
                    min[i] = Math.Min(min[i], value);
                    max[i] = Math.Max(max[i], value);
                }
        }
        public static int[] PascalTriangleRow(int rowIndex)
        {
            int[] values = new int[rowIndex + 1];
            int c = 1;
            for (int row = 0; row <= rowIndex; ++row)
                for (int val = 0; val <= row; val++)
                {
                    if (val == 0 || row == 0)
                        c = 1;
                    else
                    {
                        c = c * (row - val + 1) / val;
                        if (row == rowIndex)
                            values[val] = c;
                    }
                }
            return values;
        }
        public static int[] PascalTriangleRow(int rowIndex, out int sum)
        {
            sum = (int)Pow(2, rowIndex);
            return PascalTriangleRow(rowIndex);
        }
        /// <summary>
        /// Returns the Y-value from a normal distribution given the following parameters.
        /// </summary>
        /// <param name="x">The X-value on the distribution.</param>
        /// <param name="sigma">The standard deviation.</param>
        /// <param name="mu">Mu is the mean or expectation of the distribution (and also its median and mode),</param>
        /// <returns>The Y-value.</returns>
        public static double NormalDistribution(double x, double sigma = 1.0, double mu = 0.0)
        {
            x -= mu;
            x *= x;
            double m = sigma * sigma;
            double power = -x * 0.5 / m;
            return Exp(power) / (sigma * Sqrt(2.0 * PI));
        }
        public static double[] NormalDistributionKernelDouble(int pascalRow)
        {
            int[] rowValues = PascalTriangleRow(pascalRow, out int sum);
            return rowValues.Select(x => (double)x / sum).ToArray();
        }
        /// <summary>
        /// Returns the Y-value from a normal distribution given the following parameters.
        /// </summary>
        /// <param name="x">The X-value on the distribution.</param>
        /// <param name="sigma">The standard deviation.</param>
        /// <param name="mu">Mu is the mean or expectation of the distribution (and also its median and mode),</param>
        /// <returns>The Y-value.</returns>
        public static float NormalDistribution(float x, float sigma = 1.0f, float mu = 0.0f)
        {
            x -= mu;
            x *= x;
            float m = sigma * sigma;
            float power = -x * 0.5f / m;
            return (float)Exp(power) / (sigma * (float)Sqrt(2.0f * PIf));
        }
        public static float[] NormalDistributionKernelFloat(int pascalRow)
        {
            int[] rowValues = PascalTriangleRow(pascalRow, out int sum);
            return rowValues.Select(x => (float)x / sum).ToArray();
        }
        #endregion
    }
}
