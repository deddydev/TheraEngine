using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine;

namespace System
{
    public abstract class Shape : RenderableObject
    {
        public event Action AttributeChanged;

        public EContainment IsWithin(Shape shape) { return shape == null ? EContainment.Disjoint : shape.Contains(this); }
        public EContainment Contains(Shape shape)
        {
            if (shape is Box)
                return Contains((Box)shape);
            else if (shape is Sphere)
                return Contains((Sphere)shape);
            else if (shape is Capsule)
                return Contains((Capsule)shape);
            return EContainment.Disjoint;
        }
        public abstract EContainment Contains(Box box);
        public abstract EContainment Contains(Sphere sphere);
        public abstract EContainment Contains(Capsule capsule);
        public abstract bool Contains(Vec3 point);
        public override Shape GetCullingVolume() { return this; }
        public override Matrix4 GetWorldMatrix()
        {
            return LinkedComponent != null ? LinkedComponent.WorldMatrix : Matrix4.Identity;
        }
        public override Matrix4 GetInverseWorldMatrix()
        {
            return LinkedComponent != null ? LinkedComponent.InverseWorldMatrix : Matrix4.Identity;
        }
        public abstract void Render(bool solid);
        public abstract CollisionShape GetCollisionShape();
    }
}
