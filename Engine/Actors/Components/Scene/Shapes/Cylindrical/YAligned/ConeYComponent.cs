using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class ConeYComponent : CommonShape3DComponent<ConeY>
    {
        public ConeYComponent(float radius, float height, TRigidBodyConstructionInfo info)
            : base(new ConeY(radius, height), info) { }

        public ConeYComponent(float radius, float height)
            : base(new ConeY(radius, height)) { }

        public ConeYComponent(ConeY cone, TRigidBodyConstructionInfo info)
            : base(cone, info) { }

        public ConeYComponent(ConeY cone)
            : base(cone) { }
    }
}
