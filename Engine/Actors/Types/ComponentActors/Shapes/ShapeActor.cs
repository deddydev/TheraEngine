using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Physics;
using System.Collections.Generic;

namespace TheraEngine.Actors.Types.ComponentActors.Shapes
{
    public abstract class ShapeActor : Actor<StaticMeshComponent>
    {
        public ShapeActor(
            string name,
            Shape shape,
            Vec3 translation,
            Rotator rotation,
            List<LOD> lods)
            : this(name, shape, translation, rotation, lods, null, null) { }

        public ShapeActor(
            string name,
            Shape shape,
            Vec3 translation,
            Rotator rotation,
            List<LOD> lods,
            TCollisionShape collisionShape,
            TRigidBodyConstructionInfo info) : base(true)
        {
            _name = name;
            StaticModel model = new StaticModel(_name + "_Model") { Collision = collisionShape };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", true, shape, lods));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
    }
}
