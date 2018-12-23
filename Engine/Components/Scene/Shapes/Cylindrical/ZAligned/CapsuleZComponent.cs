using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class CapsuleZComponent : CommonShape3DComponent<CapsuleZ>
    {
        public CapsuleZComponent()
            : this(Transform.GetIdentity()) { }

        public CapsuleZComponent(float radius, float halfHeight)
            : this(Transform.GetIdentity(), radius, halfHeight) { }

        public CapsuleZComponent(float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(Transform.GetIdentity(), radius, halfHeight, info) { }

        public CapsuleZComponent(Transform transform)
            : base(new CapsuleZ(transform, 1.0f, 1.0f)) { }

        public CapsuleZComponent(Transform transform, float radius, float halfHeight)
            : base(new CapsuleZ(transform, radius, halfHeight)) { }
        
        public CapsuleZComponent(Transform transform, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new CapsuleZ(transform, radius, halfHeight), info) { }

        public CapsuleZComponent(CapsuleZ capsule)
            : base(capsule) { }

        public CapsuleZComponent(CapsuleZ capsule, TRigidBodyConstructionInfo info)
            : base(capsule, info) { }
    }
}