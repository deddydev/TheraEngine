using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class CapsuleXComponent : CommonShape3DComponent<CapsuleX>
    {
        public CapsuleXComponent()
            : this(Vec3.Zero) { }
        public CapsuleXComponent(float radius, float halfHeight)
            : this(Vec3.Zero, radius, halfHeight) { }
        public CapsuleXComponent(float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(Vec3.Zero, radius, halfHeight, info) { }
        public CapsuleXComponent(EventVec3 center)
            : base(new CapsuleX(center, 1.0f, 1.0f)) { }
        public CapsuleXComponent(EventVec3 center, float radius, float halfHeight)
            : base(new CapsuleX(center, radius, halfHeight)) { }
        public CapsuleXComponent(EventVec3 center, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new CapsuleX(center, radius, halfHeight), info) { }
        public CapsuleXComponent(CapsuleX capsule)
            : base(capsule) { }
        public CapsuleXComponent(CapsuleX capsule, TRigidBodyConstructionInfo info)
            : base(capsule, info) { }
    }
}