using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Files;

namespace TheraEngine.Components.Logic.Animation
{
    public class SkelAnimDirectPose : SkelAnimPoseGenBase
    {
        [TSerialize]
        public GlobalFileRef<SkeletalAnimation> Animation { get; set; } = null;

        public SkelAnimDirectPose() { }
        public SkelAnimDirectPose(SkeletalAnimation anim)
        {
            Animation = anim;
        }
        public SkelAnimDirectPose(GlobalFileRef<SkeletalAnimation> anim)
        {
            Animation = anim;
        }
        public override SkeletalAnimationPose GetPose()
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
