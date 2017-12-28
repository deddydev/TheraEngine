using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds.Actors.Components.Scene.Mesh;
using TheraEngine.Physics;

namespace TheraEngine.Worlds.Actors.Types.ComponentActors.Shapes
{
    public abstract class ShapeActor : Actor<StaticMeshComponent>
    {
        public ShapeActor(
            string name,
            Shape shape,
            PrimitiveData data,
            Vec3 translation,
            Rotator rotation,
            TMaterial material)
            : this(name, shape, data, translation, rotation, material, null, null) { }

        public ShapeActor(
            string name,
            Shape shape,
            PrimitiveData data,
            Vec3 translation,
            Rotator rotation,
            TMaterial material,
            TCollisionShape collisionShape,
            TRigidBodyConstructionInfo info) : base(true)
        {
            _name = name;
            StaticModel model = new StaticModel(_name + "_Model") { Collision = collisionShape };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", true, shape, data, material));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
    }
}
