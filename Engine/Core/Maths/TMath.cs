using static System.Math;
using TheraEngine.Core.Maths.Transforms;
using System.Linq;

namespace System
{
    /// <summary>
    /// Thera Math: contains static methods and constants not included in the built-in System.Math class.
    /// <para>Easily call utilize this class by implementing 'using static System.TMath;' at the top of code files.</para>
    /// </summary>
    public unsafe static class TMath
    {        
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
        public static readonly float PIf = (float)PI;
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

                return (float)((int)(sig * n * 0.1f));
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
            x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
            return x;
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
        /// 
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <param name="focusPoint"></param>
        /// <param name="cameraPoint"></param>
        /// <param name="cameraRightDir"></param>
        /// <returns></returns>
        public static Vec3 ArcballTranslation(float pitch, float yaw, Vec3 focusPoint, Vec3 cameraPoint, Vec3 cameraRightDir)
        {
            return focusPoint + Vec3.TransformVector(cameraPoint - focusPoint, Matrix4.CreateRotationY(yaw) * Matrix4.CreateFromAxisAngle(cameraRightDir, pitch));
        }

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
        public static float Cosf(float rad) => (float)Cos(rad);
        public static float Sinf(float rad) => (float)Sin(rad);
        public static float Tanf(float rad) => (float)Tan(rad);
        public static float Cosdf(float deg) => Cosf(deg * DegToRadMultf);
        public static float Sindf(float deg) => Sinf(deg * DegToRadMultf);
        public static float Tandf(float deg) => Tanf(deg * DegToRadMultf);
        public static double Cosd(double deg) => Cos(deg * DegToRadMult);
        public static double Sind(double deg) => Sin(deg * DegToRadMult);
        public static double Tand(double deg) => Tan(deg * DegToRadMult);

        /// <summary>
        /// Finds the two values of x where the equation ax^2 + bx + c evaluates to 0.
        /// Returns false if the solutions are not a real numbers.
        /// </summary>
        public static bool QuadraticRealRoots(float a, float b, float c, out float answer1, out float answer2)
        {
            if (a != 0.0f)
            {
                float mag = b * b - 4.0f * a * c;
                if (mag >= 0.0f)
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
        public static Vec3 Morph(Vec3 baseCoord, Vec3[] targets, float[] weights, bool relative = false)
        {
            if (targets.Length != weights.Length)
                throw new InvalidOperationException("'targets' length does not match 'weights' length.");

            if (relative)
            {
                Vec3 morphed = baseCoord;
                for (int i = 0; i < targets.Length; ++i)
                    morphed += targets[i] * weights[i];
                return morphed;
            }
            else
            {
                Vec3 morphed = Vec3.Zero;
                float weightSum = 0.0f;
                float weight;
                for (int i = 0; i < targets.Length; ++i)
                {
                    weight = weights[i];
                    morphed += targets[i] * weight;
                    weightSum += weight;
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
        /// <summary>
        /// Returns the rotation axis direction vector that is perpendicular to the two vectors.
        /// </summary>
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
        /// <summary>
        /// Returns a rotation axis and angle between two vectors.
        /// </summary>
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
        public static Vec2 ComponentMin(params Vec2[] values)
        {
            Vec2 value = Vec2.Zero;
            for (int i = 0; i < 2; ++i)
            {
                float v = values[0][i];
                if (values.Length > 1)
                    for (int x = 1; x < values.Length; x++)
                        v = Math.Min(v, values[x][i]);
                value[i] = v;
            }
            return value;
        }
        public static Vec2 ComponentMax(params Vec2[] values)
        {
            Vec2 value = Vec2.Zero;
            for (int i = 0; i < 2; ++i)
            {
                float v = values[0][i];
                if (values.Length > 1)
                    for (int x = 1; x < values.Length; x++)
                        v = Math.Max(v, values[x][i]);
                value[i] = v;
            }
            return value;
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
            x = x * x;
            double m = sigma * sigma;
            double power = -x * 0.5 / m;
            return Exp(power) / (sigma * Sqrt(2.0 * PI));
        }
        public static double[] NormalDistributionKernel(int pascalRow)
        {
            int[] rowValues = PascalTriangleRow(pascalRow, out int sum);
            return rowValues.Select(x => (double)x / sum).ToArray();
        }
        #endregion
    }
}
