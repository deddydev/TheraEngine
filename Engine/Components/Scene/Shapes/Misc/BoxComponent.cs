using System;
using TheraEngine.Core.Shapes;
using System.ComponentModel;
using TheraEngine.Physics;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Shapes
{
    [TFileDef("Box Component")]
    public class BoxComponent : CommonShape3DComponent<Box>
    {
        public BoxComponent()
            : base(new Box(1.0f)) { }

        public BoxComponent(Vec3 halfExtents)
            : base(new Box(halfExtents)) { }

        public BoxComponent(float halfExtentsX, float halfExtentsY, float halfExtentsZ)
            : base(new Box(halfExtentsX, halfExtentsY, halfExtentsZ)) { }

        public BoxComponent(float uniformHalfExtents)
            : base(new Box(uniformHalfExtents)) { }

        public BoxComponent(ICollisionObjectConstructionInfo info)
            : base(new Box(1.0f), info) { }

        public BoxComponent(Vec3 halfExtents, ICollisionObjectConstructionInfo info)
            : base(new Box(halfExtents), info) { }

        public BoxComponent(float halfExtentsX, float halfExtentsY, float halfExtentsZ, ICollisionObjectConstructionInfo info)
            : base(new Box(halfExtentsX, halfExtentsY, halfExtentsZ), info) { }

        public BoxComponent(float uniformHalfExtents, ICollisionObjectConstructionInfo info)
            : base(new Box(uniformHalfExtents), info) { }
        
        public Vec3[] GetTransformedCorners() => _shape.GetCorners();
        public Vec3[] GetUntransformedCorners() => BoundingBox.GetCorners(_shape.HalfExtents, Matrix4.Identity);
    }
}
