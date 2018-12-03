using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class CapsuleYComponent : CommonShape3DComponent<CapsuleY>
    {
        public CapsuleYComponent()
            : this(Transform.GetIdentity()) { }

        public CapsuleYComponent(float radius, float halfHeight)
            : this(Transform.GetIdentity(), radius, halfHeight) { }

        public CapsuleYComponent(float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(Transform.GetIdentity(), radius, halfHeight, info) { }

        public CapsuleYComponent(Transform transform)
            : this(new CapsuleY(transform, 1.0f, 1.0f)) { }

        public CapsuleYComponent(Transform transform, float radius, float halfHeight)
            : this(new CapsuleY(transform, radius, halfHeight)) { }

        public CapsuleYComponent(Transform transform, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(new CapsuleY(transform, radius, halfHeight), info) { }

        public CapsuleYComponent(CapsuleY capsule)
            : base(capsule) { }

        public CapsuleYComponent(CapsuleY capsule, TRigidBodyConstructionInfo info)
            : base(capsule, info) { }
    }
}