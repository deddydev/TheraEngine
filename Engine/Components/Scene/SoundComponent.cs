using TheraEngine.Audio;
using TheraEngine.Rendering;
using TheraEngine.Files;
using TheraEngine.Worlds.Actors.Components.Scene.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Worlds.Actors.Components.Scene
{
    public class SoundComponent : SphereComponent
    {
        public GlobalFileRef<SoundFile> File
        {
            get => _file;
            set => _file = value;
        }

        private GlobalFileRef<SoundFile> _file;

        public SoundComponent() : base(1.0f) { }
        public SoundComponent(float radius) : base(radius) { }
        public SoundComponent(float radius, TRigidBodyConstructionInfo info) : base(radius, info) { }
    }
}
