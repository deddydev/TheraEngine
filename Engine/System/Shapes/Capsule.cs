using OpenTK;
using static System.Math;
using CustomEngine;

namespace System
{
    public class Capsule : IRenderable
    {
        public float _radius, _halfHeight;
        
        public float Radius { get { return _radius; } set { _radius = value; } }
        public float HalfHeight { get { return _halfHeight; } set { _halfHeight = value; } }

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
            return EContainsShape.No;
        }
        public float GetTotalHalfHeight()
        {
            return _halfHeight + _radius;
        }
        public float GetTotalHeight()
        {
            return GetTotalHalfHeight() * 2.0f;
        }
        public void Render() { Render(false); }
        public void Render(bool solid)
        {
            if (solid)
                Engine.Renderer.DrawCapsuleSolid(this);
            else
                Engine.Renderer.DrawCapsuleWireframe(this);
        }
    }
}
