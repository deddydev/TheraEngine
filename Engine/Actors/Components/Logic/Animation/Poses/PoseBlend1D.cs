using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Animation;
using TheraEngine.Files;

namespace TheraEngine.Components.Logic.Animation
{
    public class SkelAnimPoseBlend1D : SkelAnimPoseBase
    {
        private GlobalFileRef<SkeletalAnimation>[] _animations = null;
        public GlobalFileRef<SkeletalAnimation>[] Animations
        {
            get => _animations;
            set
            {
                _animations = value;
            }
        }

        public SkelAnimPoseBlend1D() { }
        public SkelAnimPoseBlend1D(GlobalFileRef<SkeletalAnimation>[] anims)
        {
            Animations = anims;
        }
        public override float CurrentTime
        {
            get => 0.0f;
            set
            {

            }
        }
        public override SkeletalAnimationFrame GetFrame()
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
