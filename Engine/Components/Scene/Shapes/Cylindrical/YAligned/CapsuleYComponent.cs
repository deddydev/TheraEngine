using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class CapsuleYComponent : CommonShape3DComponent<CapsuleY>
    {
        public CapsuleYComponent()
            : this(Vec3.Zero) { }
        public CapsuleYComponent(float radius, float halfHeight)
            : this(Vec3.Zero, radius, halfHeight) { }
        public CapsuleYComponent(float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(Vec3.Zero, radius, halfHeight, info) { }
        public CapsuleYComponent(EventVec3 center)
            : this(new CapsuleY(center, 1.0f, 1.0f)) { }
        public CapsuleYComponent(EventVec3 center, float radius, float halfHeight)
            : this(new CapsuleY(center, radius, halfHeight)) { }
        public CapsuleYComponent(EventVec3 center, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(new CapsuleY(center, radius, halfHeight), info) { }
        public CapsuleYComponent(CapsuleY capsule)
            : base(capsule) { }
        public CapsuleYComponent(CapsuleY capsule, TRigidBodyConstructionInfo info)
            : base(capsule, info) { }
    }
}