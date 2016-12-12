using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine;
using CustomEngine.Rendering.Models;

namespace System
{
    public abstract class Shape : RenderableObject
    {
        public event Action AttributeChanged;
        
        public Vec3 ToUntransformedShapeSpace(Vec3 point) { return InverseWorldMatrix * point; }
        public Vec3 FromUntransformedShapeSpace(Vec3 point) { return WorldMatrix * point; }

        public EContainment IsWithin(Shape shape) { return shape == null ? EContainment.Disjoint : shape.Contains(this); }
        public EContainment Contains(Shape shape)
        {
            if (shape is Box)
                return Contains((Box)shape);
            else if (shape is Sphere)
                return Contains((Sphere)shape);
            else if (shape is Capsule)
                return Contains((Capsule)shape);
            else if (shape is Cone)
                return Contains((Cone)shape);
            return EContainment.Disjoint;
        }
        public abstract EContainment Contains(Cone cone);
        public abstract EContainment Contains(Box box);
        public abstract EContainment Contains(Sphere sphere);
        public abstract EContainment Contains(Capsule capsule);
        public abstract bool Contains(Vec3 point);
        public override Shape CullingVolume { get { return this; } }
        public abstract void Render(bool solid);
        public abstract CollisionShape GetCollisionShape();
        public Mesh ToMesh() { return new Mesh(this); }
    }
}
