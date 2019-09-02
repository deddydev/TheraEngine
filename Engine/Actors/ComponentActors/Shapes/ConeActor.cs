using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using System.Collections.Generic;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Actors.Types.ComponentActors.Shapes
{
    public class ConeActor : ShapeActor
    {
        public ConeActor()
            : this(1.0f, 1.0f) { }
        public ConeActor(float radius, float height)
            : this(radius, height, Vec3.Zero) { }
        public ConeActor(float radius, float height, Vec3 translation)
            : this(radius, height, translation, Rotator.GetZero()) { }
        public ConeActor(float radius, float height, Rotator rotation)
            : this(radius, height, Vec3.Zero, rotation) { }
        public ConeActor(float radius, float height, Vec3 translation, Rotator rotation)
            : this(radius, height, translation, rotation, TMaterial.CreateLitColorMaterial(Engine.InvalidColor)) { }
        public ConeActor(float radius, float height, Vec3 translation, Rotator rotation, TMaterial material)
            : this("ConeActor", radius, height, translation, rotation, material) { }

        public ConeActor(string name)
            : this(name, 1.0f, 1.0f) { }
        public ConeActor(string name, float radius, float height)
            : this(name, radius, height, Vec3.Zero) { }
        public ConeActor(string name, float radius, float height, Vec3 translation)
            : this(name, radius, height, translation, Rotator.GetZero()) { }
        public ConeActor(string name, float radius, float height, Vec3 translation, Rotator rotation)
            : this(name, radius, height, translation, rotation, TMaterial.CreateLitColorMaterial(Engine.InvalidColor)) { }
        public ConeActor(string name, float radius, float height, Vec3 translation, Rotator rotation, TMaterial material)
            : this(name, radius, height, translation, rotation, material, null) { }
        
        public ConeActor(string name, float radius, float height, Vec3 translation, Rotator rotation, 
            TMaterial material, TRigidBodyConstructionInfo info, int meshSides = 40, bool closeBottom = true) : base(
                name, 
                new ConeY(radius, height),
                translation,
                rotation,
                new EventList<ILOD>()
                {
                    new LOD(material, Cone.SolidMesh(Vec3.Zero, Vec3.Up, height, radius, meshSides,         closeBottom), radius *  8),
                    new LOD(material, Cone.SolidMesh(Vec3.Zero, Vec3.Up, height, radius, meshSides / 4 * 3, closeBottom), radius * 16),
                    new LOD(material, Cone.SolidMesh(Vec3.Zero, Vec3.Up, height, radius, meshSides / 2,     closeBottom), radius * 32),
                    new LOD(material, Cone.SolidMesh(Vec3.Zero, Vec3.Up, height, radius, meshSides / 4,     closeBottom), radius * 64),
                },
                TCollisionConeY.New(radius, height),
                info) { }
    }
}
