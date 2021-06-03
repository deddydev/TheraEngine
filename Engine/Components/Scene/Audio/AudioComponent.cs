using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Audio;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    public interface IAudioSource
    {
        EventList<AudioInstance> Instances { get; set; }
        AudioFile Audio { get; }
        AudioParameters Parameters { get; }

        int Priority { get; set; }
    }
    public class AudioComponent : TransformComponent, IAudioSource, IEditorPreviewIconRenderable
    {
        private LocalFileRef<AudioParameters> _parametersRef;

        [Category("State")]
        public EventList<AudioInstance> Instances { get; set; }

        [Category("Playback")]
        [TSerialize]
        public bool PlayOnSpawn { get; set; }
        [Category("Playback")]
        [TSerialize]
        public int Priority { get; set; } = 0;

        [Category("Audio")]
        [TSerialize]
        public GlobalFileRef<AudioFile> AudioFileRef { get; set; }
        [Category("Audio")]
        [TSerialize]
        public LocalFileRef<AudioParameters> ParametersRef
        {
            get => _parametersRef;
            set
            {
                if (_parametersRef != null)
                {
                    _parametersRef.Loaded -= ParametersRef_Loaded;
                }
                _parametersRef = value;
                if (_parametersRef != null)
                {
                    _parametersRef.Loaded += ParametersRef_Loaded;
                }
            }
        }

        private void ParametersRef_Loaded(AudioParameters parameters)
            => UpdateTransform(parameters);

        protected override async void OnSpawned()
        {
            base.OnSpawned();
            bool play = PlayOnSpawn;
#if EDITOR
            if (Engine.EditorState.InEditMode)
                play = false;
#endif
            if (play)
                await PlayAsync();
        }

        AudioFile IAudioSource.Audio => AudioFileRef?.File;
        AudioParameters IAudioSource.Parameters => ParametersRef?.File;

        /// <summary>
        /// Plays the sound.
        /// </summary>
        public async Task PlayAsync()
        {
            AudioFile file = await AudioFileRef?.GetInstanceAsync();
            if (file is null)
                return;

            var instance = Engine.Audio.CreateNewInstance(this);
            Instances.Add(instance);
            Engine.Audio.Play(instance);
        }

        protected override void OnWorldTransformChanged(bool recalcChildWorldTransformsNow = true)
        {
            base.OnWorldTransformChanged(recalcChildWorldTransformsNow);
            UpdateTransform(ParametersRef?.File);
        }

        private void UpdateTransform(AudioParameters parameters)
        {
#if EDITOR
            PreviewIconRenderCommand.Position = WorldPoint;
#endif

            if (parameters?.Position != null)
                parameters.Position.OverrideValue = WorldPoint;

            if (parameters?.Direction != null)
                parameters.Direction.OverrideValue = WorldForwardVec;

            if (parameters?.Velocity != null)
                parameters.Velocity.OverrideValue = Velocity;
        }

#if EDITOR

        [Category("Editor Traits")]
        [TSerialize]
        public IRenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(true, true);
        [Category("Editor Traits")]
        public bool ScalePreviewIconByDistance { get; set; } = true;
        [Category("Editor Traits")]
        public float PreviewIconScale { get; set; } = 0.05f;

        string IEditorPreviewIconRenderable.PreviewIconName => PreviewIconName;
        protected string PreviewIconName { get; } = "AudioIcon.png";

        PreviewRenderCommand3D IEditorPreviewIconRenderable.PreviewIconRenderCommand
        {
            get => PreviewIconRenderCommand;
            set => PreviewIconRenderCommand = value;
        }
        private PreviewRenderCommand3D _previewIconRenderCommand;
        private PreviewRenderCommand3D PreviewIconRenderCommand
        {
            get => _previewIconRenderCommand ??= CreatePreviewRenderCommand(PreviewIconName);
            set => _previewIconRenderCommand = value;
        }

        public void AddRenderables(RenderPasses passes, ICamera camera)
            => AddPreviewRenderCommand(PreviewIconRenderCommand, passes, camera, ScalePreviewIconByDistance, PreviewIconScale);
#endif
    }
}
