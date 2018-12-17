using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class ConeZComponent : CommonShape3DComponent<ConeZ>
    {
        public ConeZComponent()
            : this(1.0f, 1.0f) { }

        public ConeZComponent(float radius, float height)
            : this(new ConeZ(radius, height)) { }

        public ConeZComponent(float radius, float height, TRigidBodyConstructionInfo info)
            : this(new ConeZ(radius, height), info) { }

        public ConeZComponent(BasicTransform transform)
            : base(new ConeZ(transform, 1.0f, 1.0f)) { }

        public ConeZComponent(BasicTransform transform, float radius, float halfHeight)
            : base(new ConeZ(transform, radius, halfHeight)) { }

        public ConeZComponent(BasicTransform transform, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new ConeZ(transform, radius, halfHeight), info) { }

        public ConeZComponent(ConeZ cone)
            : base(cone) { }

        public ConeZComponent(ConeZ cone, TRigidBodyConstructionInfo info)
            : base(cone, info) { }
    }
}
