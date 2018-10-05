using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Animation;
using TheraEngine.Files;

namespace TheraEngine.Components.Logic.Animation
{
    public class SkelAnimPose : SkelAnimPoseBase
    {
        private GlobalFileRef<SkeletalAnimation> _animation = null;
        public GlobalFileRef<SkeletalAnimation> Animation
        {
            get => _animation;
            set
            {
                _animation = value;
            }
        }

        public SkelAnimPose() { }
        public SkelAnimPose(SkeletalAnimation anim)
        {
            Animation = anim;
        }
        public SkelAnimPose(GlobalFileRef<SkeletalAnimation> anim)
        {
            Animation = anim;
        }
        public override float CurrentTime
        {
            get => Animation == null ? 0.0f : Animation.File.CurrentTime;
            set
            {
                if (Animation?.File != null)
                    Animation.File.CurrentTime = value;
            }
        }
        public override SkeletalAnimationFrame GetFrame()
        {
            return Animation?.File?.GetFrame();
        }
        public override void Tick(float delta)
        {
            Animation?.File?.Tick(delta);
        }
        public override GlobalFileRef<SkeletalAnimation>[] GetAnimations()
        {
            return new GlobalFileRef<SkeletalAnimation>[] { Animation };
        }
    }
}
