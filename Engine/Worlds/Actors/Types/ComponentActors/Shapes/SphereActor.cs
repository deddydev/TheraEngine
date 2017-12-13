using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds.Actors.Components.Scene.Mesh;

namespace TheraEngine.Worlds.Actors.Types.ComponentActors.Shapes
{
    public class SphereActor : Actor<StaticMeshComponent>
    {
        public SphereActor(string name, PhysicsConstructionInfo info, float radius, Vec3 translation, Rotator rotation, TMaterial m) : base(true)
        {
            _name = name;
            Sphere sphere = new Sphere(radius);
            StaticModel model = new StaticModel(_name + "_Model")
            {
                Collision = new SphereShape(radius)
            };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", true, sphere, sphere.GetMesh(10.0f, false), m));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
