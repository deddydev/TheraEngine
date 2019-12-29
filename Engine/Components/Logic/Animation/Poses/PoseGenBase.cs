using TheraEngine.Animation;
using TheraEngine.Core.Files;

namespace TheraEngine.Components.Logic.Animation
{
    /// <summary>
    /// Used to retrieve a final skeletal animation pose.
    /// </summary>
    public abstract class PoseGenBase : TFileObject
    {
        public AnimStateMachineComponent Owner { get; internal set; }

        public abstract SkeletalAnimationPose GetPose();
        public abstract void Tick(float delta);
        public abstract GlobalFileRef<SkeletalAnimation>[] GetAnimations();
    }
}
