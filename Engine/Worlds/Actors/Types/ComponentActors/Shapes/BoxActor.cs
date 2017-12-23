using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds.Actors.Components.Scene.Mesh;
using TheraEngine.Physics;

namespace TheraEngine.Worlds.Actors.Types.ComponentActors.Shapes
{
    public class BoxActor : Actor<StaticMeshComponent>
    {
        public BoxActor(string name, Vec3 halfExtents, Vec3 translation, Rotator rotation, TMaterial material)
            : this(name, halfExtents, translation, rotation, material, null) { }
        public BoxActor(string name, Vec3 halfExtents, Vec3 translation, Rotator rotation, TMaterial material, TRigidBodyConstructionInfo info) : base(true)
        {
            _name = name;
            BoundingBox box = new BoundingBox(halfExtents, Vec3.Zero);
            StaticModel model = new StaticModel(_name + "_Model")
            {
                Collision = TCollisionBox.New(halfExtents)
            };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", true, new Box(box), box.GetMesh(false), material));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
