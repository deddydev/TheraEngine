using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class ConeXComponent : CommonShape3DComponent<ConeX>
    {
        public ConeXComponent(float radius, float height, TRigidBodyConstructionInfo info)
            : base(new ConeX(radius, height), info) { }

        public ConeXComponent(float radius, float height)
            : base(new ConeX(radius, height)) { }

        public ConeXComponent(ConeX cone, TRigidBodyConstructionInfo info)
            : base(cone, info) { }
        
        public ConeXComponent(ConeX cone)
            : base(cone) { }
    }
}
