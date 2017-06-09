using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors
{
    public class SphereActor : Actor<StaticMeshComponent>
    {
        public SphereActor(string name, PhysicsConstructionInfo info, float radius, Vec3 translation, Rotator rotation, Material m) : base(true)
        {
            _name = name;
            Sphere sphere = new Sphere(radius);
            StaticMesh model = new StaticMesh(_name + "_Model")
            {
                Collision = new SphereShape(radius)
            };
            model.RigidChildren.Add(new StaticRigidSubMesh(sphere.GetMesh(10.0f, false), sphere, m, _name + "_Mesh"));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
