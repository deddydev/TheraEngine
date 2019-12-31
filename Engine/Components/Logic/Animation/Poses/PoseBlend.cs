using System;
using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Core.Files;
using Extensions;
using System.Collections.Generic;

namespace TheraEngine.Components.Logic.Animation
{
    public class PoseBlend : PoseGenBase
    {
        public PoseBlend() { }
        public PoseBlend(GlobalFileRef<SkeletalAnimation> anim1, GlobalFileRef<SkeletalAnimation> anim2)
        {
            AnimationRef1 = anim1;
            AnimationRef2 = anim2;
        }

        [TSerialize]
        public GlobalFileRef<SkeletalAnimation> AnimationRef1 { get; set; } = null;

        [TSerialize]
        public GlobalFileRef<SkeletalAnimation> AnimationRef2 { get; set; } = null;

        private float _interpValue;

        public float InterpValue 
        {
            get => _interpValue; 
            set => Set(ref _interpValue, value.Clamp(0.0f, 1.0f));
        }

        public override SkeletalAnimationPose GetPose()
        {
            SkeletalAnimationPose frame1 = AnimationRef1?.File?.GetFrame();
            SkeletalAnimationPose frame2 = AnimationRef2?.File?.GetFrame();
            return frame1?.BlendedWith(frame2, InterpValue);
        }
        public override void Tick(float delta)
        {
            AnimationRef1?.File?.Tick(delta);
            AnimationRef2?.File?.Tick(delta);
        }
    }
}
