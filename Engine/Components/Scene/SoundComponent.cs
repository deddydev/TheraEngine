using TheraEngine.Audio;
using TheraEngine.Core.Files;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene
{
    public class SoundComponent : SphereComponent, I3DRenderable
    {
        public GlobalFileRef<SoundFile> File { get; set; }

        public SoundComponent() : base(1.0f) { }
        public SoundComponent(float radius) : base(radius) { }
        public SoundComponent(float radius, TRigidBodyConstructionInfo info) : base(radius, info) { }
    }
}
