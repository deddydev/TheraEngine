using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class ConeZComponent : CommonShape3DComponent<ConeZ>
    {
        public ConeZComponent(float radius, float height, TRigidBodyConstructionInfo info)
            : base(new ConeZ(radius, height), info) { }

        public ConeZComponent(float radius, float height)
            : base(new ConeZ(radius, height)) { }

        public ConeZComponent(ConeZ cone, TRigidBodyConstructionInfo info)
            : base(cone, info) { }
        
        public ConeZComponent(ConeZ cone)
            : base(cone) { }
    }
}
