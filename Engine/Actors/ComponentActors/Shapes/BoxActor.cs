﻿using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using System.Collections.Generic;
using TheraEngine.Rendering.Models;
using System;

namespace TheraEngine.Actors.Types.ComponentActors.Shapes
{
    public class BoxActor : ShapeActor
    {
        public BoxActor()
            : this(Vec3.Half) { }
        public BoxActor(Vec3 halfExtents)
            : this(halfExtents, Vec3.Zero) { }
        public BoxActor(Vec3 halfExtents, Vec3 translation)
            : this(halfExtents, translation, Quat.Identity) { }
        public BoxActor(Vec3 halfExtents, Quat rotation)
            : this(halfExtents, Vec3.Zero, rotation) { }
        public BoxActor(Vec3 halfExtents, Vec3 translation, Quat rotation)
            : this(halfExtents, translation, rotation, TMaterial.CreateLitColorMaterial(Engine.InvalidColor)) { }
        public BoxActor(Vec3 halfExtents, Vec3 translation, Quat rotation, TMaterial material)
            : this("BoxActor", halfExtents, translation, rotation, material) { }

        public BoxActor(string name)
            : this(name, Vec3.Half) { }
        public BoxActor(string name, Vec3 halfExtents)
            : this(name, halfExtents, Vec3.Zero) { }
        public BoxActor(string name, Vec3 halfExtents, Vec3 translation)
            : this(name, halfExtents, translation, Quat.Identity) { }
        public BoxActor(string name, Vec3 halfExtents, Vec3 translation, Quat rotation)
            : this(name, halfExtents, translation, rotation, TMaterial.CreateLitColorMaterial(Engine.InvalidColor)) { }
        public BoxActor(string name, Vec3 halfExtents, Vec3 translation, Quat rotation, TMaterial material)
            : this(name, halfExtents, translation, rotation, material, null) { }

        public BoxActor(string name, Vec3 halfExtents, Vec3 translation, Quat rotation, TMaterial material, TRigidBodyConstructionInfo info) : base(
                name, 
                new Box(halfExtents),
                translation,
                rotation,
                new EventList<ILOD>()
                {
                    new LOD(material, BoundingBox.SolidMesh(-halfExtents, halfExtents), 0.0f),
                },
                TCollisionBox.New(halfExtents),
                info) { }
    }
}
