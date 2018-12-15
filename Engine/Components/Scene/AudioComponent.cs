using System;
using System.Collections.Generic;
using TheraEngine.Audio;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    public interface IAudioSource
    {
        AudioFile Audio { get; }
        AudioParameters Parameters { get; }
        int Priority { get; set; }

    }
    public class AudioComponent : TranslationComponent, IAudioSource, I3DRenderable, IPreRendered
    {
        public AudioComponent()
        {

        }

        protected internal int _bufferId;
        protected HashSet<int> _sourceIds = new HashSet<int>();

        public bool PlayOnSpawn { get; set; }
        public int BufferID
        {
            get => _bufferId;
            set => _bufferId = value;
        }
        public int Priority { get; set; } = 0;
        public GlobalFileRef<AudioFile> AudioFileRef { get; set; }
        public LocalFileRef<AudioParameters> ParametersRef { get; set; }

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

        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPass.OnTopForward, false, true);
        public Shape CullingVolume { get; } = null;
        public IOctreeNode OctreeNode { get; set; }
        public bool PreRenderEnabled { get; set; }

        AudioFile IAudioSource.Audio => AudioFileRef?.File;
        AudioParameters IAudioSource.Parameters => ParametersRef?.File;

        /// <summary>
        /// Plays the sound given a priority value.
        /// A larger value means a higher priority.
        /// Low priority audio may be cancelled to play higher priority audio.
        /// </summary>
        /// <param name="priority">The priority of this audio file.</param>
        /// <returns>A unique identifier for the new instance of this audio.</returns>
        public int Play()
        {
            var file = AudioFileRef?.File;
            if (file is null)
                return -1;

            int id = Engine.Audio.Play(this);
            _sourceIds.Add(id);
            return id;
        }

        protected override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();
            ParametersRef.File.Position.OverrideValue = WorldPoint;
        }

        private RenderCommandMesh3D _rc;
        public void AddRenderables(RenderPasses passes, Camera camera) => throw new NotImplementedException();

        public void PreRenderUpdate(Camera camera)
        {
            throw new NotImplementedException();
        }

        public void PreRenderSwap()
        {
            throw new NotImplementedException();
        }

        public void PreRender(Viewport viewport, Camera camera)
        {
            throw new NotImplementedException();
        }
    }
}
