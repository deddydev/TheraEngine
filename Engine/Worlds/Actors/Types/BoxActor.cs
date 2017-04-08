using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors
{
    public class BoxActor : Actor<StaticMeshComponent>
    {
        public BoxActor(string name, PhysicsConstructionInfo info, Vec3 halfExtents, Vec3 translation, Rotator rotation, Material m) : base(true)
        {
            _name = name;
            BoundingBox box = new BoundingBox(halfExtents, Vec3.Zero);
            StaticMesh model = new StaticMesh(_name + "_Model", box);
            model.RigidChildren.Add(new StaticRigidSubMesh(box.GetMesh(false), box, m, _name + "_Mesh"));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        public override void OnSpawned(World world)
        {
            base.OnSpawned(world);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
