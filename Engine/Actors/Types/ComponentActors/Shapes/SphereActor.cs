using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Worlds.Actors.Types.ComponentActors.Shapes
{
    public class SphereActor : ShapeActor
    {
        public SphereActor() 
            : this(0.5f) { }
        public SphereActor(float radius)
            : this(radius, Vec3.Zero) { }
        public SphereActor(float radius, Vec3 translation)
            : this(radius, translation, Rotator.GetZero()) { }
        public SphereActor(float radius, Rotator rotation)
            : this(radius, Vec3.Zero, rotation) { }
        public SphereActor(float radius, Vec3 translation, Rotator rotation)
            : this(radius, translation, rotation, TMaterial.CreateLitColorMaterial(Engine.InvalidColor)) { }
        public SphereActor(float radius, Vec3 translation, Rotator rotation, TMaterial material)
            : this("SphereActor", radius, translation, rotation, material) { }

        public SphereActor(string name)
            : this(name, 0.5f) { }
        public SphereActor(string name, float radius)
            : this(name, radius, Vec3.Zero) { }
        public SphereActor(string name, float radius, Vec3 translation)
            : this(name, radius, translation, Rotator.GetZero()) { }
        public SphereActor(string name, float radius, Vec3 translation, Rotator rotation)
            : this(name, radius, translation, rotation, TMaterial.CreateLitColorMaterial(Engine.InvalidColor)) { }
        public SphereActor(string name, float radius, Vec3 translation, Rotator rotation, TMaterial material)
            : this(name, radius, translation, rotation, material, null) { }
        
        public SphereActor(string name, float radius, Vec3 translation, Rotator rotation, TMaterial material, TRigidBodyConstructionInfo info, float meshPrecision = 30.0f) : base(
                name, 
                new Sphere(radius),
                Sphere.SolidMesh(Vec3.Zero, radius, 20.0f), 
                translation,
                rotation,
                material,
                TCollisionSphere.New(radius),
                info) { }
    }
}
