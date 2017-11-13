using TheraEngine.Files;
using TheraEngine.Worlds;
using System.Collections.Generic;
using System;
using TheraEngine.Animation;

namespace TheraEngine.Cutscenes
{
    public class Cutscene : BaseAnimation
    {
        public World World { get => _world; set => _world = value; }
        public float Length { get => _length; set => _length = value; }
        public List<IActor> InvolvedActors { get => _involvedActors; set => _involvedActors = value; }

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
                Engine.SetCurrentWorld(_world, false, false, false);
        }
        public void Tick(float delta)
        {

        }
    }
}
