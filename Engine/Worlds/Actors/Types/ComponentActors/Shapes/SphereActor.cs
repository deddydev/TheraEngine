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
    public class SphereActor : Actor<StaticMeshComponent>
    {
        public SphereActor(string name, float radius, Vec3 translation, Rotator rotation, TMaterial material)
            : this(name, radius, translation, rotation, material, null) { }
            
        public SphereActor(string name, float radius, Vec3 translation, Rotator rotation, TMaterial material, TRigidBodyConstructionInfo info) : base(true)
        {
            _name = name;
            Sphere sphere = new Sphere(radius);
            StaticModel model = new StaticModel(_name + "_Model")
            {
                Collision = TCollisionSphere.New(radius)
            };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", true, sphere, sphere.GetMesh(10.0f, false), material));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
        protected override StaticMeshComponent OnConstruct()
        {
            return RootComponent;
        }
    }
}
