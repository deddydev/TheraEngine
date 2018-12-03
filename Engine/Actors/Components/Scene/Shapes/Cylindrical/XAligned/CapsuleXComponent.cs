using TheraEngine.Core.Maths.Transforms;
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

        public CapsuleXComponent(Transform transform)
            : base(new CapsuleX(transform, 1.0f, 1.0f)) { }

        public CapsuleXComponent(Transform transform, float radius, float halfHeight)
            : base(new CapsuleX(transform, radius, halfHeight)) { }
        
        public CapsuleXComponent(Transform transform, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new CapsuleX(transform, radius, halfHeight), info) { }
    }
}