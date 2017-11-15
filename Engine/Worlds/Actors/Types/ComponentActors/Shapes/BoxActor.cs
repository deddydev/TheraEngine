using BulletSharp;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Worlds.Actors
{
    public class BoxActor : Actor<StaticMeshComponent>
    {
        public BoxActor(string name, PhysicsConstructionInfo info, Vec3 halfExtents, Vec3 translation, Rotator rotation, Material m) : base(true)
        {
            _name = name;
            BoundingBox box = new BoundingBox(halfExtents, Vec3.Zero);
            StaticMesh model = new StaticMesh(_name + "_Model")
            {
                Collision = new BoxShape(halfExtents)
            };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", box.GetMesh(false), new Box(box), m));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
