using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Animation;
using TheraEngine.Files;

namespace TheraEngine.Components.Logic.Animation
{
    public abstract class SkelAnimPoseBase : TFileObject
    {
        public AnimStateMachineComponent Owner { get; internal set; }

        public abstract SkeletalAnimationFrame GetFrame();
        public abstract float CurrentTime { get; set; }
        public abstract void Tick(float delta);
        public abstract GlobalFileRef<SkeletalAnimation>[] GetAnimations();
    }
}
