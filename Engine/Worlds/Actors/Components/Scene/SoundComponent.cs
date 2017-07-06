using TheraEngine.Audio;
using TheraEngine.Rendering;
using TheraEngine.Files;

namespace TheraEngine.Worlds.Actors
{
    public class SoundComponent : SphereComponent
    {
        public SingleFileRef<SoundFile> File
        {
            get => _file;
            set => _file = value;
        }

        private SingleFileRef<SoundFile> _file;

        public SoundComponent() : base(1.0f) { }
        public SoundComponent(float radius) : base(radius) { }
        public SoundComponent(float radius, PhysicsConstructionInfo info) : base(radius, info) { }
    }
}
