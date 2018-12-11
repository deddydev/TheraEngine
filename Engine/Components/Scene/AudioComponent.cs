using TheraEngine.Audio;
using TheraEngine.Core.Files;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Physics;
using System.Collections.Generic;

namespace TheraEngine.Components.Scene
{
    public interface IAudioSource
    {
        int BufferID { get; }
    }
    public class AudioComponent : SphereComponent, I3DRenderable
    {
        public AudioComponent() 
            : this(1.0f) { }

        public AudioComponent(float radius) 
            : base(radius) { }

        public AudioComponent(float radius, TRigidBodyConstructionInfo info)
            : base(radius, info) { }

        protected internal int _bufferId;
        protected HashSet<int> _sourceIds = new HashSet<int>();

        public bool PlayOnSpawn { get; set; }
        public int BufferID
        {
            get => _bufferId;
            internal set => _bufferId = value;
        }
        public int Priority { get; set; } = 0;
        public GlobalFileRef<AudioFile> AudioFile { get; set; }
        public LocalFileRef<AudioParameters> Parameters { get; set; }

        public override void OnSpawned()
        {
            base.OnSpawned();
            if (PlayOnSpawn)
                Play();
        }

        public void StopAllInstances()
        {
            foreach (int id in _sourceIds)
                Engine.Audio.Stop(id);
            _sourceIds.Clear();
        }
        public void StopInstance(int id)
        {
            if (_sourceIds.Remove(id))
                Engine.Audio.Stop(id);
        }

        public int PlayingCount => _sourceIds.Count;
        
        /// <summary>
        /// Plays the sound given a priority value.
        /// A larger value means a higher priority.
        /// Low priority audio may be cancelled to play higher priority audio.
        /// </summary>
        /// <param name="priority">The priority of this audio file.</param>
        /// <returns>A unique identifier for the new instance of this audio.</returns>
        public int Play()
        {
            var file = AudioFile?.File;
            if (file is null)
                return -1;

            int id = Engine.Audio.Play(file, Parameters?.File);
            _sourceIds.Add(id);
            return id;
        }
    }
}
