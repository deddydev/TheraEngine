using TheraEngine.Rendering.Models;
using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Physics;
using System.Collections.Generic;
using TheraEngine.Rendering;

namespace TheraEngine.Actors.Types.ComponentActors.Shapes
{
    public abstract class ShapeActor : Actor<StaticMeshComponent>
    {
        public ShapeActor(
            string name,
            TShape shape,
            Vec3 translation,
            Rotator rotation,
            List<LOD> lods)
            : this(name, shape, translation, rotation, lods, null, null) { }

        public ShapeActor(
            string name,
            TShape shape,
            Vec3 translation,
            Rotator rotation,
            List<LOD> lods,
            TCollisionShape collisionShape,
            TRigidBodyConstructionInfo info) : base(true)
        {
            _name = name;
            StaticModel model = new StaticModel(_name + "_Model") { CollisionShape = collisionShape };
            RenderInfo3D renderInfo = new RenderInfo3D(true, false) { CullingVolume = shape };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", renderInfo, ERenderPass.OpaqueDeferredLit, lods));
            RootComponent = new StaticMeshComponent(model, translation, rotation, Vec3.One, info);
            Initialize();
        }
    }
}
