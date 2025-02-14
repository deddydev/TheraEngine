﻿using TheraEngine.Rendering.Models;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Physics;
using System.Collections.Generic;
using TheraEngine.Rendering;
using System;

namespace TheraEngine.Actors.Types.ComponentActors.Shapes
{
    public abstract class ShapeActor : Actor<StaticMeshComponent>
    {
        public ShapeActor(
            string name,
            TShape shape,
            Vec3 translation,
            Quat rotation,
            IEventList<ILOD> lods)
            : this(name, shape, translation, rotation, lods, null, null) { }

        public ShapeActor(
            string name,
            TShape shape,
            Vec3 translation,
            Quat rotation,
            IEventList<ILOD> lods,
            TCollisionShape collisionShape,
            TRigidBodyConstructionInfo info) : base(true)
        {
            _name = name;
            StaticModel model = new StaticModel(_name + "_Model") { CollisionShape = collisionShape };
            RenderInfo3D renderInfo = new RenderInfo3D(true, false) { CullingVolume = shape };
            model.RigidChildren.Add(new StaticRigidSubMesh(_name + "_Mesh", renderInfo, ERenderPass.OpaqueDeferredLit, lods));
            RootComponent = new StaticMeshComponent(model, new TTransform(translation, rotation, Vec3.One), info);
            Initialize();
        }
    }
}
