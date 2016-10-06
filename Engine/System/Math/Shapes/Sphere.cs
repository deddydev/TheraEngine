using static System.Math;
using CustomEngine;

namespace System
{
    public class Sphere
    {
        public float _radius;
        public float Radius { get { return _radius; } set { _radius = value; } }
        public Sphere(float radius) { _radius = Abs(radius); }

        public bool ContainsPoint(Vec3 point)
        {
            return Abs(point.LengthSquared) <= _radius * _radius;
        }
        public ContainsShape ContainsBox(Box box)
        {
            Vec3[] p = new Vec3[8];
            box.GetCorners(out p[0], out p[1], out p[2], out p[3], out p[4], out p[5], out p[6], out p[7]);
            bool temp = false;
            for (int i = 0; i < 8; ++i)
            {
                bool contains = ContainsPoint(p[i]);
                if (temp && !contains)
                    return ContainsShape.Partial;
                else
                    temp = contains;
            }
            return temp ? ContainsShape.Yes : ContainsShape.No;
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
