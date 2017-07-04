using TheraEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
