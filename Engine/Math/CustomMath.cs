using OpenTK;

namespace System
{
    public class CustomMath
    {
        //Smoothed interpolation between two points. Eases in and out.
        //time is a value from 0.0f to 1.0f symbolizing the time between the two points
        public Vector3 InterpCosineTo(Vector3 from, Vector3 to, float time, float speed = 1.0f)
        {
            float time2 = (1.0f - (float)Math.Cos(time * speed * (float)Math.PI)) / 2.0f;
            return from * (1.0f - time2) + to * time2;
        }
        //Constant interpolation directly from one point to another.
        //time is a value from 0.0f to 1.0f symbolizing the time between the two points
        public Vector3 InterpLinearTo(Vector3 from, Vector3 to, float time, float speed = 1.0f)
        {
            Vector3 diff = to - from;
            return from + diff * time * speed;
        }
    }
}
