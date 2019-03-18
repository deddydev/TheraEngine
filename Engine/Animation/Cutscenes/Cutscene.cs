using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine.Animation.Cutscenes
{
    public class WorldObjectReference
    {
        public string WorldPath { get; set; }
        public int MapIndex { get; set; }
        public int ActorIndex { get; set; }
    }
    [TFileExt("cut")]
    [TFileDef("Cutscene")]
    public class Cutscene : BaseAnimation
    {
        public Cutscene() : base(0.0f, false)
        {
            SubScenes = new EventList<Clip<Cutscene>>();
        }
        public Cutscene(float lengthInSeconds, bool looped)
            : base(lengthInSeconds, looped)
        {
            SubScenes = new EventList<Clip<Cutscene>>();
        }
        public Cutscene(int frameCount, float FPS, bool looped)
            : base(FPS <= 0.0f ? 0.0f : frameCount / FPS, looped)
        {
            SubScenes = new EventList<Clip<Cutscene>>();
        }

        private EventList<Clip<Cutscene>> _scenes = new EventList<Clip<Cutscene>>();
        private EventDictionary<string, Clip<BaseAnimation>> _animationTracks = new EventDictionary<string, Clip<BaseAnimation>>();

        public List<GlobalFileRef<IActor>> InvolvedActors { get; set; }
        private Camera CurrentCamera { get; set; }
        public GlobalFileRef<World> WorldRef { get; set; }
        public Clip<Cutscene> CurrentSceneClip { get; private set; }
        private int CurrentSceneIndex { get; set; } = -1;

        [TSerialize]
        public EventDictionary<string, Clip<BaseAnimation>> AnimationTracks
        {
            get => _animationTracks;
            set => _animationTracks = value;
        }
        [TSerialize]
        public EventList<Clip<Cutscene>> SubScenes
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
                var cut = scene.AnimationRef.GetInstance();
                cut?.LoadSubScenes();
            }
        }
        public void LoadSubScene(int index)
        {
            if (SubScenes.IndexInRange(index))
                SubScenes[index].AnimationRef.GetInstance();
        }
        public async Task LoadSubScenesAsync()
        {
            foreach (var scene in SubScenes)
            {
                var cut = await scene.AnimationRef.GetInstanceAsync();
                await cut?.LoadSubScenesAsync();
            }
        }
        public async Task LoadSubSceneAsync(int index)
        {
            if (SubScenes.IndexInRange(index))
                await SubScenes[index].AnimationRef.GetInstanceAsync();
        }
        public void LoadAnimations()
        {
            foreach (var anim in AnimationTracks)
                anim.Value.AnimationRef.GetInstance();
        }
        public void LoadAnimationsParallel()
        {
            Parallel.ForEach(AnimationTracks, anim => anim.Value.AnimationRef.GetInstance());
        }
        public async Task LoadAnimationsAsync()
        {
            foreach (var anim in AnimationTracks)
                await anim.Value.AnimationRef.GetInstanceAsync();
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

                }
            }
        }
        protected override void PostStopped()
        {

        }

        private void _scenes_PostAnythingRemoved(Clip<Cutscene> item)
        {

        }
        private void _scenes_PostAnythingAdded(Clip<Cutscene> item)
        {

        }

        /// <summary>
        /// Initializes starting positions of all cutscene actors and animations.
        /// </summary>
        public void Reset()
        {

        }
        private float _lastDelta = 0.0f;
        protected override void OnProgressed(float delta)
        {
            //Progress animations in this cutscene level
            foreach (var anim in AnimationTracks)
                anim.Value?.AnimationRef?.File?.Progress(delta);

            //Progress sub scenes
            if (CurrentSceneClip == null)
                return;

            _lastDelta = delta;

            var scene = CurrentSceneClip.Animation;
            float sceneTime = scene.CurrentTime + delta;
            if (sceneTime > CurrentSceneClip.LengthInSeconds)
            {
                scene.Stop();

                //New time is beyond current scene, need to move to the next scene (or further, so use a while loop)
                while (sceneTime > CurrentSceneClip.LengthInSeconds)
                {
                    sceneTime -= CurrentSceneClip.LengthInSeconds;

                    if (++CurrentSceneIndex >= SubScenes.Count)
                    {
                        CurrentSceneIndex = 0;
                    }

                    CurrentSceneClip = SubScenes[CurrentSceneIndex];
                }

                scene = CurrentSceneClip.Animation;
                scene.TickSelf = false;
                scene.Start();
                scene.Progress(CurrentSceneClip.StartSecond + sceneTime);
            }
            else if (sceneTime < 0.0f)
            {
                scene.Stop();

                //New time is before current scene, need to move to the previous scene (or further, so use a while loop)
                while (sceneTime < 0.0f)
                {
                    sceneTime += CurrentSceneClip.LengthInSeconds;

                    if (--CurrentSceneIndex < 0)
                    {
                        CurrentSceneIndex = SubScenes.Count - 1;
                    }

                    CurrentSceneClip = SubScenes[CurrentSceneIndex];
                }

                scene = CurrentSceneClip.Animation;
                scene.TickSelf = false;
                scene.Start();
                scene.Progress(CurrentSceneClip.EndSecond - sceneTime);
            }
            else
            {
                CurrentSceneClip.Animation.Progress(delta);
            }
        }
    }
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

        [Browsable(false)]
        public T Animation => AnimationRef.File;
    }
}
