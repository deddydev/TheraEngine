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
        public BoxActor(string name, TRigidBodyConstructionInfo info, Vec3 halfExtents, Vec3 translation, Rotator rotation, TMaterial m) : base(true)
        {
            _name = name;
            BoundingBox box = new BoundingBox(halfExtents, Vec3.Zero);
            StaticModel model = new StaticModel(_name + "_Model")
            {
                Collision = TCollisionBox.New(halfExtents)
            };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", true, new Box(box), box.GetMesh(false), m));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
