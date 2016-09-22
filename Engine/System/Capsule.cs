using OpenTK;
using static System.Math;
using System;

namespace CustomEngine.System
{
    public class Capsule
    {
        public float _radius, _halfHeight;

        public Capsule(float radius, float halfHeight)
        {
            _radius = Abs(radius);
            _halfHeight = Abs(halfHeight);
        }
        public bool ContainsPoint(Vector3 point)
        {
            float totalHalfHeight = GetTotalHalfHeight();
            if (point.Z < totalHalfHeight && point.Z > -totalHalfHeight)
            {
                //Adjust Z to origin
                if (point.Z > _halfHeight)
                    point.Z -= _halfHeight;
                else if (point.Z < -_halfHeight)
                    point.Z += _halfHeight;
                return Abs(point.LengthSquared) < _radius * _radius;
            }
            return false;
        }
        public EContainsShape ContainsBox(Box box)
        {
            return false;
        }
        public float GetTotalHalfHeight()
        {
            return _halfHeight + _radius;
        }
        public float GetTotalHeight()
        {
            return GetTotalHalfHeight() * 2.0f;
        }
    }
}
