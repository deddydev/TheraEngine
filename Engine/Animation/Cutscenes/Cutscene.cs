using TheraEngine.Worlds;
using System.Collections.Generic;
using TheraEngine.Animation;
using TheraEngine.Actors;
using System;

namespace TheraEngine.Cutscenes
{
    public class Cutscene : BaseAnimation
    {
        public Cutscene() : base(0.0f, false, false) { }
        public Cutscene(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped, isBaked) { }
        public Cutscene(int frameCount, float FPS, bool looped, bool isBaked = false)
            : base(frameCount, FPS, looped, isBaked) { }
        
        public World World
        {
            get => _world;
            set => _world = value;
        }
        public List<IActor> InvolvedActors
        {
            get => _involvedActors;
            set => _involvedActors = value;
        }

        //ONLY render actors visible in the cutsene to improve performance
        //Precompute visibility in the editor, then compile the list of visible actors here.
        private List<IActor> _involvedActors;
        //How long this cutscene runs for, in seconds
        private float _length;
        private World _world;

        /// <summary>
        /// Initializes starting positions of all cutscene actors and animations.
        /// </summary>
        public void Reset()
        {

        }
        protected override void PreStarted()
        {
            if (Engine.World != _world)
                Engine.SetCurrentWorld(_world, false, false);
        }
        public void Tick(float delta)
        {

        }

        protected override void BakedChanged()
        {
            throw new NotImplementedException();
        }

        public override void Bake(float framesPerSecond)
        {
            throw new NotImplementedException();
        }
    }
}
