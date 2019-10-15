using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Audio;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    public interface IAudioSource
    {
        AudioInstance Instance { get; set; }
        AudioFile Audio { get; }
        AudioParameters Parameters { get; }

        public int Priority { get; set; }
    }
    public class AudioComponent : TranslationComponent, IAudioSource, IEditorPreviewIconRenderable
    {
        private LocalFileRef<AudioParameters> _parametersRef;

        [Category("State")]
        public AudioInstance Instance { get; set; }

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
        {
            UpdateTransform(parameters);
        }

        public override async void OnSpawned()
        {
            base.OnSpawned();
            if (PlayOnSpawn)
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

            Instance = Engine.Audio.Play(this);
        }

        protected override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();
            UpdateTransform(ParametersRef?.File);
        }

        private void UpdateTransform(AudioParameters parameters)
        {
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

        RenderCommandMesh3D IEditorPreviewIconRenderable.PreviewIconRenderCommand
        {
            get => PreviewIconRenderCommand;
            set => PreviewIconRenderCommand = value;
        }
        private RenderCommandMesh3D PreviewIconRenderCommand { get; set; }

        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            AddPreviewRenderCommand(PreviewIconRenderCommand, passes, camera, ScalePreviewIconByDistance, PreviewIconScale);
        }
#endif
    }
}
