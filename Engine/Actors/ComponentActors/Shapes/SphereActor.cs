using Extensions;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Actors.Types.ComponentActors.Shapes
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
        
        public SphereActor(string name, float radius, Vec3 translation, Rotator rotation, TMaterial material, TRigidBodyConstructionInfo info, uint meshPrecision = 40u) : base(
                name, 
                new Sphere(radius),
                translation,
                rotation,
                new EventList<ILOD>()
                {
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, meshPrecision), radius * 8),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.8f).ClampMin(1.0f)), radius * 16),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.6f).ClampMin(1.0f)), radius * 32),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.4f).ClampMin(1.0f)), radius * 64),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.2f).ClampMin(1.0f)), radius * 128),
                },
                //new List<LOD>()
                //{
                //    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, meshPrecision), radius * 2),
                //    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.7f).ClampMin(1.0f)), radius * 4),
                //    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.5f).ClampMin(1.0f)), radius * 8),
                //    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.3f).ClampMin(1.0f)), radius * 16),
                //    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.15f).ClampMin(1.0f)), radius * 32),
                //},
                TCollisionSphere.New(radius),
                info) { }
    }
}
