using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine.Cutscenes
{
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
        private EventList<Clip<BasePropAnim>> _animationTracks = new EventList<Clip<BasePropAnim>>();

        public List<GlobalFileRef<IActor>> InvolvedActors { get; set; }
        private Camera CurrentCamera { get; set; }
        public GlobalFileRef<TWorld> WorldRef { get; set; }
        public Cutscene CurrentScene { get; private set; }
        private int CurrentSceneIndex { get; set; } = -1;

        [TSerialize]
        public EventList<Clip<BasePropAnim>> AnimationTracks
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
                scene.Animation.GetInstance();
        }
        public void LoadSubScene(int index)
        {
            if (SubScenes.IndexInRange(index))
                SubScenes[index].Animation.GetInstance();
        }
        public async Task LoadSubScenesAsync()
        {
            foreach (var scene in SubScenes)
                await scene.Animation.GetInstanceAsync();
        }
        public async Task LoadSubSceneAsync(int index)
        {
            if (SubScenes.IndexInRange(index))
                await SubScenes[index].Animation.GetInstanceAsync();
        }
        public void LoadAnimations()
        {
            foreach (var anim in AnimationTracks)
                anim.Animation.GetInstance();
        }
        public void LoadAnimationsParallel()
        {
            Parallel.ForEach(AnimationTracks, anim => anim.Animation.GetInstance());
        }
        public async Task LoadAnimationsAsync()
        {
            foreach (var anim in AnimationTracks)
                await anim.Animation.GetInstanceAsync();
        }
        #endregion

        protected override void PreStarted()
        {
            if (SubScenes.Count == 0)
            {
                CurrentSceneIndex = -1;
                CurrentScene = null;
            }
            else
            {
                CurrentScene = SubScenes[0].Animation.File;
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
                anim?.Animation?.File?.Progress(delta);

            //Progress sub scenes
            if (CurrentScene == null)
                return;

            _lastDelta = delta;

            float newTime = CurrentScene.CurrentTime + delta;
            if (newTime > CurrentScene.LengthInSeconds)
            {
                CurrentScene.Stop();

                //New time is beyond current scene, need to move to the next scene (or further, so use a while loop)
                while (newTime > CurrentScene.LengthInSeconds)
                {
                    newTime -= CurrentScene.LengthInSeconds;
                    if (++CurrentSceneIndex < SubScenes.Count)
                        CurrentScene = SubScenes[CurrentSceneIndex]?.Animation?.File;
                    else
                        return;
                }

                CurrentScene.TickSelf = false;
                CurrentScene.CurrentTime = 0.0f;
                CurrentScene.Start();
                CurrentScene.Progress(newTime);
            }
            else if (newTime < 0.0f)
            {
                //New time is before current scene, need to move to the previous scene (or further, so use a while loop)
                while (newTime < 0.0f)
                {
                    --CurrentSceneIndex;
                    newTime += CurrentScene.LengthInSeconds;
                }
            }
            else
            {
                CurrentScene.Progress(delta);
            }
        }
    }
    [TFileExt("clip")]
    [TFileDef("Cutscene Animation Clip")]
    public class Clip<T> : TFileObject where T : BaseAnimation
    {
        [TSerialize]
        public LocalFileRef<T> Animation { get; set; }
        [TSerialize(IsAttribute = true)]
        public float StartSecond { get; set; }
        [TSerialize(IsAttribute = true)]
        public float EndSecond { get; set; }
    }
}
