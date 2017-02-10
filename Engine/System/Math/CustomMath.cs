using static System.Math;
namespace System
{
    public unsafe static class CustomMath
    {
        public static readonly float PIf = (float)PI;

        public static float DegToRad(float degrees)
        {
            return degrees * PIf / 180.0f;
        }
        public static float RadToDeg(float radians)
        {
            return radians * 180.0f / PIf;
        }
        public static Vec2 DegToRad(Vec2 degrees)
        {
            return degrees * PIf / 180.0f;
        }
        public static Vec2 RadToDeg(Vec2 radians)
        {
            return radians * 180.0f / PIf;
        }
        public static Vec3 DegToRad(Vec3 degrees)
        {
            return degrees * PIf / 180.0f;
        }
        public static Vec3 RadToDeg(Vec3 radians)
        {
            return radians * 180.0f / PIf;
        }
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
                points[i] = Bezier(p0, p1, p2, p3, timeDelta);

            return points;
        }
        public static Vec2 Bezier(Vec2 p0, Vec2 p1, Vec2 p2, Vec2 p3, float time)
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
        public static Vec3 Bezier(Vec3 p0, Vec3 p1, Vec3 p2, Vec3 p3, float time)
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
                    mag = (float)Math.Sqrt(mag);
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
        //Smoothed interpolation between two points. Eases in and out.
        //time is a value from 0.0f to 1.0f symbolizing the time between the two points
        public static Vec3 InterpCosineTo(Vec3 from, Vec3 to, float time, float speed = 1.0f)
        {
            float time2 = (1.0f - (float)Math.Cos(time * speed * (float)Math.PI)) / 2.0f;
            return from * (1.0f - time2) + to * time2;
        }
        //Smoothed interpolation between two points. Eases in and out.
        //time is a value from 0.0f to 1.0f symbolizing the time between the two points
        public static float InterpCosineTo(float from, float to, float time, float speed = 1.0f)
        {
            float time2 = (1.0f - (float)Math.Cos(time * speed * (float)Math.PI)) / 2.0f;
            return from * (1.0f - time2) + to * time2;
        }
        //Constant interpolation directly from one point to another.
        //time is a value from 0.0f to 1.0f symbolizing the time between the two points
        public static float InterpLinearTo(float from, float to, float time, float speed = 1.0f)
        {
            return from + (to - from) * time * speed;
        }
        public static Vec3 VInterpNormalRotationTo(Vec3 Current, Vec3 Target, float DeltaTime, float RotationSpeedDegrees)
        {
            Quaternion DeltaQuat = Quaternion.BetweenVectors(Current, Target);
            
            Vec3 DeltaAxis;
            float DeltaAngle;
            DeltaQuat.ToAxisAngle(out DeltaAxis, out DeltaAngle);
            
	        float RotationStepRadians = RotationSpeedDegrees * (PIf / 180.0f) * DeltaTime;

	        if (Abs(DeltaAngle) > RotationStepRadians)
	        {
		        DeltaAngle = DeltaAngle.Clamp(-RotationStepRadians, RotationStepRadians);
		        DeltaQuat = new Quaternion(DeltaAxis, DeltaAngle);
		        return DeltaQuat * Current;
	        }
	        return Target;
        }
        public static Vec3 RotateAboutPoint(Vec3 point, Vec3 center, Rotator angles)
        {
            return point * Matrix4.CreateTranslation(-center) * angles.GetMatrix() * Matrix4.CreateTranslation(center);
        }
        public static Vec3 RotateAboutPoint(Vec3 point, Vec3 center, Quaternion angles)
        {
            return point * Matrix4.CreateTranslation(-center) * Matrix4.CreateFromQuaternion(angles) * Matrix4.CreateTranslation(center);
        }
        public static Vec2 RotateAboutPoint(Vec2 point, Vec2 center, float angle)
        {
            return (Vec2)((Vec3)point * Matrix4.CreateTranslation((Vec3)(-center)) * Matrix4.CreateRotationZ(angle) * Matrix4.CreateTranslation((Vec3)center));
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
