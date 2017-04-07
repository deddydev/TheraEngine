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
        public BoxActor(string name, PhysicsDriverInfo info) : base(true)
        {
            _name = name;
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
            PhysicsDriverInfo floorInfo = new PhysicsDriverInfo()
            {
                BodyInfo = new RigidBodyConstructionInfo(
                       20.0f, null,
                       /*new DefaultMotionState(Matrix4.CreateFromAxisAngle(Vec3.Forward, 10.0f)),*/
                       new BoxShape(new Vec3(20.0f, 0.5f, 20.0f)))
                {
                    Restitution = 0.5f,
                },
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.StaticWorld,
                CollidesWith = CustomCollisionGroup.DynamicWorld | CustomCollisionGroup.Characters,
            };
            BoundingBox floorBox = new BoundingBox(new Vec3(20.0f, 0.5f, 20.0f), Vec3.Zero);
            StaticMesh floorModel = new StaticMesh(_name + "_Model", floorBox);
            floorModel.RigidChildren.Add(new StaticRigidSubMesh(floorBox.GetMesh(false), floorBox, Material.GetDefaultMaterial(), _name + "_Mesh"));
            StaticMeshComponent floorComp = new StaticMeshComponent(
                floorModel, new Vec3(0.0f, 10.0f, 0.0f),
                new Rotator(0.0f, 0.0f, 0.0f, Rotator.Order.YPR),
                Vec3.One, floorInfo);
            return floorComp;
        }
    }
}
