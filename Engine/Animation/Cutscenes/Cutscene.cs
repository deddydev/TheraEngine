using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Components.Scene;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine.Animation.Cutscenes
{
    [Serializable]
    public class WorldObjectReference
    {
        public string WorldPath { get; set; }
        public int MapIndex { get; set; }
        public string ActorKey { get; set; }
    }
    [Serializable]
    [TFileExt("cut")]
    [TFileDef("Cutscene")]
    public class Cutscene : BaseAnimation
    {
        public Cutscene() : base(0.0f, false)
        {
            SubScenes = new EventList<(float Second, Clip<Cutscene> Clip)>();
        }
        public Cutscene(float lengthInSeconds, bool looped)
            : base(lengthInSeconds, looped)
        {
            SubScenes = new EventList<(float Second, Clip<Cutscene> Clip)>();
        }
        public Cutscene(int frameCount, float FPS, bool looped)
            : base(FPS <= 0.0f ? 0.0f : frameCount / FPS, looped)
        {
            SubScenes = new EventList<(float Second, Clip<Cutscene> Clip)>();
        }

        private PerspCamKeyCollection _cameraTrack = new PerspCamKeyCollection();
        private EventList<(float Second, Clip<Cutscene> Clip)> _scenes;
        private EventDictionary<string, (float Second, Clip<BaseAnimation> Clip)> _animationTracks = new EventDictionary<string, (float Second, Clip<BaseAnimation> Clip)>();

        public List<GlobalFileRef<BaseActor>> InvolvedActors { get; set; }

        private (float Second, Clip<Cutscene> Clip)? _currentSceneClip;
        public (float Second, Clip<Cutscene> Clip)? CurrentSceneClip
        {
            get => _currentSceneClip;
            private set
            {
                if (_currentSceneClip?.Clip?.Animation != null)
                    _currentSceneClip.Value.Clip.Animation.Camera = null;
                _currentSceneClip = value;
                if (_currentSceneClip?.Clip?.Animation != null)
                    _currentSceneClip.Value.Clip.Animation.Camera = Camera;
            }
        }
        private int CurrentSceneIndex { get; set; } = -1;

        public PerspectiveCamera Camera { get; private set; }
        public Pawn<CameraComponent> CameraPawn { get; private set; }
        public World TargetWorld { get; set; }
        
        [TSerialize]
        public PerspCamKeyCollection CameraTrack
        {
            get => _cameraTrack;
            set => _cameraTrack = value ?? new PerspCamKeyCollection();
        }
        [TSerialize]
        public EventDictionary<string, (float Second, Clip<BaseAnimation> Clip)> AnimationTracks
        {
            get => _animationTracks;
            set => _animationTracks = value;
        }
        [TSerialize]
        public EventList<(float Second, Clip<Cutscene> Clip)> SubScenes
        {
            get => _scenes;
            set
            {
                if (_scenes != null)
                {
                    _scenes.PostAnythingAdded -= _scenes_PostAnythingAdded;
                    _scenes.PostAnythingRemoved -= _scenes_PostAnythingRemoved;
                }
                _scenes = value;
                if (_scenes != null)
                {
                    _scenes.PostAnythingAdded += _scenes_PostAnythingAdded;
                    _scenes.PostAnythingRemoved += _scenes_PostAnythingRemoved;
                }
            }
        }

        #region Loading
        public void LoadSubScenes()
        {
            foreach (var scene in SubScenes)
            {
                var cut = scene.Clip.AnimationRef.GetInstance();
                cut?.LoadSubScenes();
            }
        }
        public void LoadSubScene(int index)
        {
            if (SubScenes.IndexInRange(index))
                SubScenes[index].Clip.AnimationRef.GetInstance();
        }
        public async Task LoadSubScenesAsync()
        {
            foreach (var scene in SubScenes)
            {
                var cut = await scene.Clip.AnimationRef.GetInstanceAsync();
                await cut?.LoadSubScenesAsync();
            }
        }
        public async Task LoadSubSceneAsync(int index)
        {
            if (SubScenes.IndexInRange(index))
                await SubScenes[index].Clip.AnimationRef.GetInstanceAsync();
        }
        public void LoadAnimations()
        {
            foreach (var anim in AnimationTracks)
                anim.Value.Clip.AnimationRef.GetInstance();
        }
        public void LoadAnimationsParallel()
        {
            Parallel.ForEach(AnimationTracks, anim => anim.Value.Clip.AnimationRef.GetInstance());
        }
        public async Task LoadAnimationsAsync()
        {
            foreach (var anim in AnimationTracks)
                await anim.Value.Clip.AnimationRef.GetInstanceAsync();
            //Task.WaitAll(AnimationTracks.Select(x => x.AnimationRef.GetInstanceAsync()).ToArray());
        }
        #endregion

        protected override void PreStarted()
        {
            if (SubScenes.Count == 0)
            {
                CurrentSceneIndex = -1;
                CurrentSceneClip = null;
            }
            else
            {
                CurrentSceneClip = Speed < 0 ? SubScenes[SubScenes.Count - 1] : SubScenes[0];
                CurrentSceneIndex = 0;
                foreach (var anim in AnimationTracks)
                {
                    anim.Value.Clip.Start();
                }
            }

            Camera = new PerspectiveCamera();
            CameraComponent comp = new CameraComponent(Camera);
            CameraPawn = new Pawn<CameraComponent>(comp);
            TargetWorld.SpawnActor(CameraPawn);
            CameraTrack.UpdateCamera(Camera);
            CameraPawn.ForcePossessionBy(ELocalPlayerIndex.One);
        }
        protected override void PostStopped()
        {

        }

        private void _scenes_PostAnythingRemoved((float Second, Clip<Cutscene> Clip) item)
        {

        }
        private void _scenes_PostAnythingAdded((float Second, Clip<Cutscene> Clip) item)
        {

        }

        public void SetCameraKeyframe(PerspectiveCamera cameraReference)
        {
            CameraTrack.SetCameraKeyframe(CurrentTime, cameraReference);
        }

        /// <summary>
        /// Initializes starting positions of all cutscene actors and animations.
        /// </summary>
        public void Reset()
        {

        }
        protected override void OnProgressed(float delta)
        {
            if (CurrentSceneClip == null)
            {
                //Progress animations in this cutscene level

                CameraTrack.Progress(delta);
                CameraTrack.UpdateCamera(Camera);

                foreach (var anim in AnimationTracks)
                    anim.Value.Clip.Progress(delta, out float over);
            }
            else
            {
                CurrentSceneClip.Value.Clip.Progress(delta, out float overShootDelta);

                if (overShootDelta > 0.0f)
                {
                    //New time is beyond current scene, need to move to the next scene (or further, so use a while loop)
                    if (++CurrentSceneIndex >= SubScenes.Count)
                        CurrentSceneIndex = 0;

                    CurrentSceneClip = SubScenes[CurrentSceneIndex];
                    CurrentSceneClip.Value.Clip.Start(overShootDelta);
                }
                else if (overShootDelta < 0.0f)
                {
                    //New time is before current scene, need to move to the previous scene (or further, so use a while loop)
                    if (--CurrentSceneIndex < 0)
                        CurrentSceneIndex = SubScenes.Count - 1;

                    CurrentSceneClip = SubScenes[CurrentSceneIndex];
                    CurrentSceneClip.Value.Clip.Start(CurrentSceneClip.Value.Clip.LengthInSeconds + overShootDelta);
                }
            }
        }
    }
    [Serializable]
    [TFileExt("clip")]
    [TFileDef("Animation Clip")]
    public class Clip<T> : TFileObject where T : BaseAnimation
    {
        [TSerialize]
        public LocalFileRef<T> AnimationRef { get; set; }
        
        [TSerialize(IsAttribute = true)]
        public float StartSecond { get; set; }

        [TSerialize(IsAttribute = true)]
        public float EndSecond { get; set; }

        public float LengthInSeconds => EndSecond - StartSecond;

        public void Start(float offset = 0.0f)
        {
            Animation.TickSelf = false;
            Animation.CurrentTime = StartSecond + offset;
            Animation.Start();
        }
        public void Progress(float delta, out float overShootDelta)
        {
            float time = Animation.CurrentTime + delta;
            if (Animation.Looped)
            {
                time = time.RemapToRange(StartSecond, EndSecond);
                Animation.Progress(time - Animation.CurrentTime);
                overShootDelta = 0.0f;
            }
            else
            {
                bool shouldStop = time <= StartSecond || time >= EndSecond;
                float time2 = time.Clamp(StartSecond, EndSecond);
                Animation.Progress(time2 - Animation.CurrentTime);
                if (shouldStop)
                    Animation.Stop();
                overShootDelta = time - time2;
            }
        }
        
        [Browsable(false)]
        public T Animation => AnimationRef.File;
    }
}
