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
    public class Scene : BaseAnimation
    {
        public Scene() : base(0.0f, false) { _tickSelf = false; }

        public List<GlobalFileRef<IActor>> InvolvedActors { get; set; }

        [TSerialize]
        private EventList<BasePropAnim> _animationTracks = new EventList<BasePropAnim>();

        private Camera CurrentCamera { get; set; }

        protected override void OnProgressed(float delta)
        {
            foreach (BasePropAnim anim in _animationTracks)
            {
                anim.Progress(delta);
            }
        }

        public void Initialize()
        {
            foreach (IActor actor in InvolvedActors)
            {

            }
            State = EAnimationState.Playing;
        }
    }
}
