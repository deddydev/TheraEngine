using System;
using System.Drawing;
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

        public BoxComponent(TRigidBodyConstructionInfo info)
            : base(new Box(1.0f), info) { }

        public BoxComponent(Vec3 halfExtents, TRigidBodyConstructionInfo info)
            : base(new Box(halfExtents), info) { }

        public BoxComponent(float extentsX, float extentsY, float extentsZ, TRigidBodyConstructionInfo info)
            : base(new Box(extentsX, extentsY, extentsZ), info) { }

        public BoxComponent(float uniformExtents, TRigidBodyConstructionInfo info)
            : base(new Box(uniformExtents), info) { }
        
        public Vec3[] GetTransformedCorners() => _shape.GetCorners();
        public Vec3[] GetUntransformedCorners() => BoundingBox.GetCorners(_shape.HalfExtents, Matrix4.Identity);
    }
}
