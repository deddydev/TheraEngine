using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using System.Collections.Generic;
using TheraEngine.Rendering.Models;

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
        
        public SphereActor(string name, float radius, Vec3 translation, Rotator rotation, TMaterial material, TRigidBodyConstructionInfo info, uint meshPrecision = 30u) : base(
                name, 
                new Sphere(radius),
                translation,
                rotation,
                new List<LOD>()
                {
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, meshPrecision), radius * 4),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.66f).ClampMin(1.0f)), radius * 8),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.33f).ClampMin(1.0f)), radius * 64),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.22f).ClampMin(1.0f)), radius * 128),
                    new LOD(material, Sphere.SolidMesh(Vec3.Zero, radius, (uint)(meshPrecision * 0.11f).ClampMin(1.0f)), radius * 256),
                },
                TCollisionSphere.New(radius),
                info) { }
    }
}
