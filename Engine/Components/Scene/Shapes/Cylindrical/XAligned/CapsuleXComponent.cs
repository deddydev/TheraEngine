using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class CapsuleXComponent : CommonShape3DComponent<CapsuleX>
    {
        public CapsuleXComponent()
            : this(Transform.GetIdentity()) { }

        public CapsuleXComponent(float radius, float halfHeight)
            : this(Transform.GetIdentity(), radius, halfHeight) { }

        public CapsuleXComponent(float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : this(Transform.GetIdentity(), radius, halfHeight, info) { }

        public CapsuleXComponent(BasicTransform transform)
            : base(new CapsuleX(transform, 1.0f, 1.0f)) { }

        public CapsuleXComponent(BasicTransform transform, float radius, float halfHeight)
            : base(new CapsuleX(transform, radius, halfHeight)) { }
        
        public CapsuleXComponent(BasicTransform transform, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new CapsuleX(transform, radius, halfHeight), info) { }

        public CapsuleXComponent(CapsuleX capsule)
            : base(capsule) { }

        public CapsuleXComponent(CapsuleX capsule, TRigidBodyConstructionInfo info)
            : base(capsule, info) { }
    }
}