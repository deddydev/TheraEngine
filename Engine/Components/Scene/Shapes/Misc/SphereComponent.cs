using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class SphereComponent : CommonShape3DComponent<Sphere>
    {
        public SphereComponent()
            : this(1.0f) { }

        public SphereComponent(float radius) 
            : this(radius, null) { }

        public SphereComponent(float radius, TRigidBodyConstructionInfo info)
            : base(new Sphere(radius, Vec3.Zero), info) { }
    }
}
