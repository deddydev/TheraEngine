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
        public ConeZComponent(EventVec3 center)
            : base(new ConeZ(center, 1.0f, 1.0f)) { }
        public ConeZComponent(EventVec3 center, float radius, float halfHeight)
            : base(new ConeZ(center, radius, halfHeight)) { }
        public ConeZComponent(EventVec3 center, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new ConeZ(center, radius, halfHeight), info) { }
        public ConeZComponent(ConeZ cone)
            : base(cone) { }
        public ConeZComponent(ConeZ cone, TRigidBodyConstructionInfo info)
            : base(cone, info) { }
    }
}
