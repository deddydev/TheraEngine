using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class CapsuleZComponent : CommonShape3DComponent<CapsuleZ>
    {
        public CapsuleZComponent()
            : this(Vec3.Zero) { }
        public CapsuleZComponent(float radius, float halfHeight)
            : this(Vec3.Zero, radius, halfHeight) { }
        public CapsuleZComponent(float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(Vec3.Zero, radius, halfHeight, info) { }
        public CapsuleZComponent(EventVec3 center)
            : base(new CapsuleZ(center, 1.0f, 1.0f)) { }
        public CapsuleZComponent(EventVec3 center, float radius, float halfHeight)
            : base(new CapsuleZ(center, radius, halfHeight)) { }
        public CapsuleZComponent(EventVec3 center, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new CapsuleZ(center, radius, halfHeight), info) { }
        public CapsuleZComponent(CapsuleZ capsule)
            : base(capsule) { }
        public CapsuleZComponent(CapsuleZ capsule, TRigidBodyConstructionInfo info)
            : base(capsule, info) { }
    }
}