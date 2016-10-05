using OpenTK;

namespace System
{
    public unsafe static class CustomMath
    {
        public static float DegToRad(float degrees)
        {
            return degrees * (float)Math.PI / 180.0f;
        }
        public static float RadToDeg(float radians)
        {
            return radians * 180.0f / (float)Math.PI;
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
        public static bool Quadratic(float a, float b, float c, out float answer1, out float answer2)
        {
            if (a > 0.0f)
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
        public static Vec3 RotateAboutPoint(Vec3 point, Vec3 center, Vec3 angles)
        {
            return point * Matrix4.CreateTranslation(-center) * Matrix4.CreateFromEuler(angles) * Matrix4.CreateTranslation(center);
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
