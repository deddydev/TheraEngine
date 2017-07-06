using BulletSharp;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;

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
                Collision = new BoxShape(halfExtents.X, halfExtents.Y, halfExtents.Z)
            };
            model.RigidChildren.Add(new StaticRigidSubMesh(box.GetMesh(false), new Box(box), m, _name + "_Mesh"));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
