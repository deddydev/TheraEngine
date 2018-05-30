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
        
        public List<GlobalFileRef<IActor>> InvolvedActors
        {
            get => _involvedActors;
            set => _involvedActors = value;
        }
        public GlobalFileRef<World> WorldRef
        {
            get => _worldRef;
            set => _worldRef = value;
        }

        [TSerialize]
        private KeyframeTrack<CutsceneKeyframe> _keyframes;

        //ONLY render actors visible in the cutsene to improve performance
        //Precompute visibility in the editor, then compile the list of visible actors here.
        private List<GlobalFileRef<IActor>> _involvedActors;
        private GlobalFileRef<World> _worldRef;
        private Camera CurrentCamera { get; set; }

        /// <summary>
        /// Initializes starting positions of all cutscene actors and animations.
        /// </summary>
        public void Reset()
        {

        }
        protected override void PreStarted()
        {
            if (Engine.World != _worldRef.File)
                Engine.SetCurrentWorld(_worldRef.File, false, false);
        }
        protected override void BakedChanged()
        {

        }
        public override void Bake(float framesPerSecond)
        {

        }
    }
    public class CutsceneKeyframe : Keyframe
    {
        public AnimationContainer Animation { get; set; }

        public override void ReadFromString(string str)
        {

        }
        public override string WriteToString()
        {
            return null;
        }
    }
}
