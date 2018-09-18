using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine.Cutscenes
{
    public class Cutscene : BaseAnimation
    {
        public Cutscene() : base(0.0f, false)
        {
            Scenes = new EventList<GlobalFileRef<Scene>>();
        }
        public Cutscene(float lengthInSeconds, bool looped)
            : base(lengthInSeconds, looped)
        {
            Scenes = new EventList<GlobalFileRef<Scene>>();
        }
        public Cutscene(int frameCount, float FPS, bool looped)
            : base(FPS <= 0.0f ? 0.0f : frameCount / FPS, looped)
        {
            Scenes = new EventList<GlobalFileRef<Scene>>();
        }

        private EventList<GlobalFileRef<Scene>> _scenes;
        private Scene _nextScene;
        private int _currentSceneIndex = -1;
        private bool _isNextSceneLoading = false;
        private bool _isNextSceneLoaded = false;

        public GlobalFileRef<World> WorldRef { get; set; }
        public Scene CurrentScene { get; private set; }
        public Scene NextScene
        {
            get => _nextScene;
            private set
            {
                _nextScene = value;
                _isNextSceneLoaded = true;
            }
        }
        private int CurrentSceneIndex
        {
            get => _currentSceneIndex;
            set
            {
                _currentSceneIndex = value;
            }
        }

        [TSerialize]
        public EventList<GlobalFileRef<Scene>> Scenes
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
        public void PreLoadScenes()
        {
            foreach (var scene in Scenes)
                scene.GetInstance();
        }
        public void PreLoadScene(int index)
        {
            if (Scenes.IndexInRange(index))
                Scenes[index].GetInstance();
        }
        public async Task PreLoadScenesAsync()
        {
            foreach (var scene in Scenes)
                await scene.GetInstanceAsync();
        }
        public async Task PreLoadSceneAsync(int index)
        {
            if (Scenes.IndexInRange(index))
                await Scenes[index].GetInstanceAsync();
        }
        protected override void PreStarted()
        {
            if (Scenes.Count == 0)
            {
                CurrentSceneIndex = -1;
                CurrentScene = null;
                return;
            }
            CurrentScene = Scenes[0].GetInstance();
            CurrentSceneIndex = 0;
            _isNextSceneLoaded = false;
        }

        private void _scenes_PostAnythingRemoved(GlobalFileRef<Scene> item)
        {

        }
        private void _scenes_PostAnythingAdded(GlobalFileRef<Scene> item)
        {

        }
        [PostDeserialize]
        private void PostDeserialize()
        {

        }

        /// <summary>
        /// Initializes starting positions of all cutscene actors and animations.
        /// </summary>
        public void Reset()
        {

        }
        protected override void OnProgressed(float delta)
        {
            if (CurrentScene == null)
                return;
            float newTime = CurrentScene.CurrentTime + delta;
            if (newTime > CurrentScene.LengthInSeconds)
            {
                //New time is beyond current scene, need to move to the next scene (or further, so use a while loop)
                while (newTime > CurrentScene.LengthInSeconds)
                {
                    newTime -= CurrentScene.LengthInSeconds;
                    ++CurrentSceneIndex;
                    if (CurrentSceneIndex < Scenes.Count)
                    {
                        CurrentScene = Scenes[CurrentSceneIndex]?.File;
                        if (CurrentScene != null)
                        {

                        }
                    }
                    else
                        return;
                }
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

            CurrentScene.Progress(delta);
        }
    }
}
