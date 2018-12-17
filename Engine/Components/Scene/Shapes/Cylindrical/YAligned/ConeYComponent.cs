using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class ConeYComponent : CommonShape3DComponent<ConeY>
    {
        public ConeYComponent()
            : this(1.0f, 1.0f) { }

        public ConeYComponent(float radius, float height)
            : this(new ConeY(radius, height)) { }

        public ConeYComponent(float radius, float height, TRigidBodyConstructionInfo info)
            : this(new ConeY(radius, height), info) { }

        public ConeYComponent(BasicTransform transform)
            : base(new ConeY(transform, 1.0f, 1.0f)) { }

        public ConeYComponent(BasicTransform transform, float radius, float halfHeight)
            : base(new ConeY(transform, radius, halfHeight)) { }
        
        public ConeYComponent(BasicTransform transform, float radius, float halfHeight, TRigidBodyConstructionInfo info)
            : base(new ConeY(transform, radius, halfHeight), info) { }

        public ConeYComponent(ConeY cone)
            : base(cone) { }

        public ConeYComponent(ConeY cone, TRigidBodyConstructionInfo info)
            : base(cone, info) { }
    }
}
