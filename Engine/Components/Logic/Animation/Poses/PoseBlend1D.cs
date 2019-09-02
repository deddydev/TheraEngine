using System;
using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Core.Files;

namespace TheraEngine.Components.Logic.Animation
{
    public class SkelAnimPoseBlend1D : SkelAnimPoseGenBase
    {
        private class SkelAnimKeyframe : Keyframe
        {
            [Browsable(false)]
            public override Type ValueType => throw new NotImplementedException();
            public override void ReadFromString(string str)
            {
                throw new NotImplementedException();
            }
            public override string WriteToString()
            {
                throw new NotImplementedException();
            }
        }
        private KeyframeTrack<SkelAnimKeyframe> _poses;
        
        public SkelAnimPoseBlend1D() { }
        
        public override SkeletalAnimationPose GetPose()
        {
            return null;
        }
        public override void Tick(float delta)
        {

        }
        public override GlobalFileRef<SkeletalAnimation>[] GetAnimations()
        {
            return new GlobalFileRef<SkeletalAnimation>[] { };
        }
    }
}
