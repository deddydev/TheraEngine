﻿using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Audio;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
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
    public class AudioComponent : TranslationComponent, IAudioSource, IEditorPreviewIconRenderable
    {
        protected HashSet<AudioInstance> _instances = new HashSet<AudioInstance>();

        //[Browsable(false)]
        public IReadOnlyCollection<AudioInstance> Instances => _instances;

        [Category("Audio")]
        [TSerialize]
        public bool PlayOnSpawn { get; set; }
        [Category("Audio")]
        [TSerialize]
        public int Priority { get; set; } = 0;
        [Category("Audio")]
        [TSerialize]
        public GlobalFileRef<AudioFile> AudioFileRef { get; set; }
        [Category("Audio")]
        [TSerialize]
        public LocalFileRef<AudioParameters> ParametersRef { get; set; }

        public override void OnSpawned()
        {
            base.OnSpawned();
            if (PlayOnSpawn)
                Play();
        }

        public void StopAllInstances()
        {
            foreach (AudioInstance instance in _instances)
                Engine.Audio.Stop(instance);
            _instances.Clear();
        }

        AudioFile IAudioSource.Audio => AudioFileRef?.File;
        AudioParameters IAudioSource.Parameters => ParametersRef?.File;

        /// <summary>
        /// Plays the sound given a priority value.
        /// A larger value means a higher priority.
        /// Low priority audio may be cancelled to play higher priority audio.
        /// </summary>
        /// <param name="priority">The priority of this audio file.</param>
        /// <returns>A unique identifier for the new instance of this audio.</returns>
        public AudioInstance Play()
        {
            var file = AudioFileRef?.File;
            if (file is null)
                return null;

            AudioInstance instance = Engine.Audio.Play(this);
            _instances.Add(instance);
            return instance;
        }

        protected override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();
            var file = ParametersRef?.File;
            if (file != null)
                file.Position.OverrideValue = WorldPoint;
        }

#if EDITOR
        [Category("Audio")]
        [TSerialize]
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(true, true);
        [Category("Editor Traits")]
        public bool ScalePreviewIconByDistance { get; set; } = true;
        [Category("Editor Traits")]
        public float PreviewIconScale { get; set; } = 0.05f;

        string IEditorPreviewIconRenderable.PreviewIconName => PreviewIconName;
        protected string PreviewIconName { get; } = "AudioIcon.png";

        RenderCommandMesh3D IEditorPreviewIconRenderable.PreviewIconRenderCommand
        {
            get => PreviewIconRenderCommand;
            set => PreviewIconRenderCommand = value;
        }
        private RenderCommandMesh3D PreviewIconRenderCommand { get; set; }

        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            AddPreviewRenderCommand(PreviewIconRenderCommand, passes, camera, ScalePreviewIconByDistance, PreviewIconScale);
        }
#endif
    }
}
