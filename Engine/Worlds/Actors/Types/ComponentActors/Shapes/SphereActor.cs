using BulletSharp;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Worlds.Actors
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
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", sphere.GetMesh(10.0f, false), sphere, m));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
