using static System.Math;
using CustomEngine;

namespace System
{
    public class Sphere
    {
        private Vec3 _center;
        private float _radius;

        public float Radius { get { return _radius; } set { _radius = value; } }
        public Vec3 Center { get { return _center; } set { _center = value; } }

        public Sphere(float radius, Vec3 center)
        {
            _radius = Abs(radius);
            _center = center;
        }

        public void Render() { Render(true); }
        public void Render(bool solid)
        {
            if (solid)
                Engine.Renderer.DrawSphereSolid(this);
            else
                Engine.Renderer.DrawSphereWireframe(this);
        }
    }
}
