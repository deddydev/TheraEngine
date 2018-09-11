using System.ComponentModel;

namespace TheraEngine.Animation
{
    [FileExt("kpanm")]
    [FileDef("Keyframed Property Animation")]
    public abstract class BasePropAnimKeyframed : BasePropAnim
    {
        public BasePropAnimKeyframed(float lengthInSeconds, bool looped, bool isBaked = false)
            : base(lengthInSeconds, looped, isBaked) { }
        public BasePropAnimKeyframed(int frameCount, float framesPerSecond, bool looped, bool isBaked = false)
            : base(frameCount, framesPerSecond, looped, isBaked) { }
        public override void SetLength(float lengthInSeconds, bool stretchAnimation)
        {
            InternalKeyframes.SetLength(lengthInSeconds, stretchAnimation);
            base.SetLength(lengthInSeconds, stretchAnimation);
        }
        [Category(PropAnimCategory)]
        protected abstract BaseKeyframeTrack InternalKeyframes { get; }
    }
}