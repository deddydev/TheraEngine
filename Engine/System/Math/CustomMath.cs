using OpenTK;

namespace System
{
    public class CustomMath
    {
        //Smoothed interpolation between two points. Eases in and out.
        //time is a value from 0.0f to 1.0f symbolizing the time between the two points
        public float InterpCosineTo(float from, float to, float time, float speed = 1.0f)
        {
            float time2 = (1.0f - (float)Math.Cos(time * speed * (float)Math.PI)) / 2.0f;
            return from * (1.0f - time2) + to * time2;
        }
        //Constant interpolation directly from one point to another.
        //time is a value from 0.0f to 1.0f symbolizing the time between the two points
        public float InterpLinearTo(float from, float to, float time, float speed = 1.0f)
        {
            return from + (to - from) * time * speed;
        }
        public static Vector3 RotateAboutPoint(Vector3 point, Vector3 center, Vector3 angles)
        {
            return point * Matrix4.CreateTranslation(-center) * Matrix4.CreateFromEuler(angles) * Matrix4.CreateTranslation(center);
        }
        public static Vector3 RotateAboutPoint(Vector3 point, Vector3 center, Quaternion angles)
        {
            return point * Matrix4.CreateTranslation(-center) * Matrix4.CreateFromQuaternion(angles) * Matrix4.CreateTranslation(center);
        }
        public static Vector2 RotateAboutPoint(Vector2 point, Vector2 center, float angle)
        {
            return (Vector2)((Vector3)point * Matrix4.CreateTranslation((Vector3)(-center)) * Matrix4.CreateRotationZ(angle) * Matrix4.CreateTranslation((Vector3)center));
        }
        public static Vector3 ScaleAboutPoint(Vector3 point, Vector3 center, Vector3 scale)
        {
            return point * Matrix4.CreateTranslation(-center) * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(center);
        }
        public static Vector2 ScaleAboutPoint(Vector2 point, Vector2 center, Vector2 scale)
        {
            return (Vector2)((Vector3)point * Matrix4.CreateTranslation((Vector3)(-center)) * Matrix4.CreateScale(scale.X, scale.Y, 1.0f) * Matrix4.CreateTranslation((Vector3)center));
        }
        public static Vector3 TransformAboutPoint(Vector3 point, Vector3 center, Matrix4 transform)
        {
            return point * Matrix4.CreateTranslation(-center) * transform * Matrix4.CreateTranslation(center);
        }
        public static bool LineSphereIntersect(Vector3 start, Vector3 end, Vector3 center, float radius, out Vector3 result)
        {
            Vector3 diff = end - start;
            float a = diff.LengthSquared;

            //Use quadratic formula
            if (a > 0.0f)
            {
                float b = 2.0f * diff.Dot(start - center);
                float c = (center.LengthSquared + start.LengthSquared) - (2.0f * center.Dot(start)) - (radius * radius);

                float magnitude = (b * b) - (4.0f * a * c);

                if (magnitude >= 0.0f)
                {
                    magnitude = (float)Math.Sqrt(magnitude);
                    a *= 2.0f;

                    float scale = (-b + magnitude) / a;
                    float dist2 = (-b - magnitude) / a;

                    if (dist2 < scale)
                        scale = dist2;

                    result = start + (diff * scale);
                    return true;
                }
            }

            result = new Vector3();
            return false;
        }
        public static bool LinePlaneIntersect(Vector3 lineStart, Vector3 lineEnd, Vector3 planePoint, Vector3 planeNormal, out Vector3 result)
        {
            Vector3 diff = lineEnd - lineStart;
            float scale = -planeNormal.Dot(lineStart - planePoint) / planeNormal.Dot(diff);

            if (float.IsNaN(scale) || scale < 0.0f || scale > 1.0f)
            {
                result = new Vector3();
                return false;
            }

            result = lineStart + (diff * scale);
            return true;
        }
        public static Vector3 PointAtLineDistance(Vector3 start, Vector3 end, float distance)
        {
            Vector3 diff = end - start;
            return start + (diff * (distance / diff.TrueDistance()));
        }
        public static Vector3 PointLineIntersect(Vector3 start, Vector3 end, Vector3 point)
        {
            Vector3 diff = end - start;
            return start + (diff * (diff.Dot(point - start) / diff.LengthSquared));
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
