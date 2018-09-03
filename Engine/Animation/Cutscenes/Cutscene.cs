using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Animation;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine.Cutscenes
{
    public class Cutscene : BaseAnimation
    {
        public Cutscene() : base(0.0f, false, false) { }
        public Cutscene(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped, isBaked) { }
        public Cutscene(int frameCount, float FPS, bool looped, bool isBaked = false)
            : base(frameCount, FPS, looped, isBaked) { }

        public List<GlobalFileRef<IActor>> InvolvedActors { get; set; }
        public GlobalFileRef<World> WorldRef { get; set; }

        [TSerialize]
        private List<BaseKeyframeTrack> _keyframes = new List<BaseKeyframeTrack>();

        private Camera CurrentCamera { get; set; }

        /// <summary>
        /// Initializes starting positions of all cutscene actors and animations.
        /// </summary>
        public void Reset()
        {

        }
        protected override void PreStarted()
        {
            if (Engine.World != WorldRef.File)
                Engine.SetCurrentWorld(WorldRef.File, false, false);
        }
        protected override void BakedChanged()
        {

        }
        public override void Bake(float framesPerSecond)
        {

        }
    }
}
