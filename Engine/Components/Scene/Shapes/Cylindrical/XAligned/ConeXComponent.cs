using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class ConeXComponent : CommonShape3DComponent<ConeX>
    {
        public ConeXComponent()
            : this(1.0f, 1.0f) { }
        public ConeXComponent(float radius, float height)
            : this(new ConeX(radius, height)) { }
        public ConeXComponent(float radius, float height, TRigidBodyConstructionInfo info)
            : this(new ConeX(radius, height), info) { }
        public ConeXComponent(EventVec3 center)
            : base(new ConeX(center, 1.0f, 1.0f)) { }
        public ConeXComponent(EventVec3 center, float radius, float halfHeight)
            : base(new ConeX(center, radius, halfHeight)) { }
        public ConeXComponent(EventVec3 center, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new ConeX(center, radius, halfHeight), info) { }
        public ConeXComponent(ConeX cone)
            : base(cone) { }
        public ConeXComponent(ConeX cone, TRigidBodyConstructionInfo info)
            : base(cone, info) { }
    }
}
