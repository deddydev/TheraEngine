using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Core.Files;

namespace TheraEngine.Components.Logic.Animation
{
    public class PoseDirect : PoseGenBase
    {
        [TSerialize]
        public GlobalFileRef<SkeletalAnimation> Animation { get; set; } = null;

        public PoseDirect() { }
        public PoseDirect(SkeletalAnimation anim) 
            => Animation = anim;
        public PoseDirect(GlobalFileRef<SkeletalAnimation> anim)
            => Animation = anim;

        public override SkeletalAnimationPose GetPose()
            => Animation?.File?.GetFrame();
        public override void Tick(float delta)
            => Animation?.File?.Tick(delta);
    }
}
